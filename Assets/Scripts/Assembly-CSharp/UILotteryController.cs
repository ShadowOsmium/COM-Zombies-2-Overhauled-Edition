using System.Collections;
using System;
using UnityEngine;

public class UILotteryController : UISceneController
{
    public UILotterManager lotter_manager;

    public GameObject lottery_button;
    public GameObject lottery_free_button;

    public TUILabel lottery_price_p;
    public TUILabel lottery_price;
    public TUILabel reset_price;

    public GameObject block_bk;

    private bool isSpinInProgress = false;
    private Coroutine failSafeCoroutine;

    public UIMoneyController money_controller;

    public TUILabel free_spins_label;
    public TUILabel lottery_bar_label;

    public TUIMeshSprite lottery_bar;
    public TUIRect lottery_bar_rect;

    private float lottery_bar_rect_width = 322f;

    public bool lotter_count_award;

    private int lastDisplayedFreeSpins = -1;
    private int lastFreeLotterySpins = -1;

    private bool hasTriggeredDailyResetThisSession = false;

    private bool hasAllowedOneRollbackResetThisSession = false;

    public new static UILotteryController Instance
    {
        get { return (UILotteryController)UISceneController.instance; }
    }

    private void Awake()
    {
        UISceneController.instance = this;
        GameConfig.CheckGameConfig();
        GameData.CheckGameData();
        MenuAudioController.CheckGameMenuAudio();

        GameData.Instance.reset_nist_time_finish = OnResetServerTimeFinish;
        GameData.Instance.reset_nist_time_error = OnResetSeverTimeError;
    }

    private void Update()
    {
        int currentFreeSpins = GameData.Instance.free_lottery_spins.GetIntVal();
        if (currentFreeSpins != lastFreeLotterySpins)
        {
            lastFreeLotterySpins = currentFreeSpins;
            displayedFreeSpins = currentFreeSpins;
            UpdateFreeSpinsLabel();
        }

        bool hasFreeSpin = currentFreeSpins > 0;
        bool freeButtonActive = lottery_free_button.activeSelf;

        if (hasFreeSpin != freeButtonActive)
        {
            lottery_free_button.SetActive(hasFreeSpin);
            lottery_button.SetActive(!hasFreeSpin);
        }
    }

    private void Start()
    {
        Debug.Log("UILotteryController Start - lotter_manager: " + (lotter_manager == null ? "NULL" : "NOT NULL"));
        if (GameData.Instance != null)
            StartCoroutine(GameData.Instance.ResetCurServerTime());

        lottery_free_button.SetActive(false);
        lottery_button.SetActive(false);

        int priceVal = GameConfig.Instance.lottery_price.GetIntVal();
        lottery_price_p.Text = priceVal.ToString();
        lottery_price.Text = priceVal.ToString();

        reset_price.Text = GameConfig.Instance.lottery_reset_price.GetIntVal().ToString();

        EnableBlock(false);

        if (TapJoyScript.Instance != null)
        {
            TapJoyScript.Instance.points_add_call_back = OnTapJoyPointsAdd;
        }
        OnResetServerTimeFinish();
        Initialize();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        AwardChangePanel.DestroyPanel();
        AwardGetPanel.DestroyPanel();
        GameData.Instance.reset_nist_time_finish = null;
        GameData.Instance.reset_nist_time_error = null;

        GameMsgBoxController.DestroyMsgBox();

        if (TapJoyScript.Instance != null)
        {
            TapJoyScript.Instance.points_add_call_back = null;
        }
    }

    private void OnResetButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            if (GameData.Instance == null || GameConfig.Instance == null)
            {
                Debug.LogError("[UILotteryController] Missing GameData or GameConfig instance.");
                return;
            }

            int resetCost = Mathf.Max(0, GameConfig.Instance.lottery_reset_price.GetIntVal());

            if (GameData.Instance.total_crystal.GetIntVal() < resetCost)
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    TUIControls.gameObject,
                    "Not enough crystals to reset lottery.\nYou need " + resetCost + " crystals.",
                    null,
                    null
                );
                return;
            }

            int newVal = GameData.Instance.total_crystal.GetIntVal() - resetCost;
            if (newVal < 0) newVal = 0;
            GameData.Instance.total_crystal.SetIntVal(newVal, GameDataIntPurpose.Crystal);

            lotter_manager.ResetSeatLevel(true);

            GameData.Instance.lottery_reset_count++;
            GameData.Instance.SaveData();

            money_controller.UpdateInfo();

            Hashtable hashtable = new Hashtable();
            hashtable.Add("tCrystalNum", resetCost);
            GameData.Instance.UploadStatistics("tCrystal_Use_Lottery_Reset", hashtable);
        }
    }

    public void UpdateFreeSpinsLabel()
    {
        if (free_spins_label == null)
        {
            Debug.LogWarning("[Lottery] free_spins_label is null!");
            return;
        }

        if (displayedFreeSpins > 0)
        {
            free_spins_label.Text = "Free Spins Left: " + displayedFreeSpins;
            free_spins_label.gameObject.SetActive(true);
        }
        else
        {
            free_spins_label.gameObject.SetActive(false);
        }
    }

    private void TryStartLotterySpin(bool isFreeSpin)
    {
        if (UILotterManager.Instance == null)
        {
            Debug.LogError("[UILotteryController] UILotterManager.Instance is null — cannot start spin.");
            return;
        }
        if (isSpinInProgress || (UILotterManager.Instance != null && UILotterManager.Instance.IsSpinning))
        {
            Debug.LogWarning("[UILotteryController] Spin already in progress.");

            GameMsgBoxController.ShowMsgBox(
                GameMsgBoxController.MsgBoxType.SingleButton,
                TUIControls.gameObject,
                "Please wait for the current spin to finish before spinning again.",
                null, null);
            return;
        }

        if (isFreeSpin)
        {
            int previousSpins = GameData.Instance.free_lottery_spins.GetIntVal();

            Debug.Log("[FreeSpin] Before decrement: " + previousSpins);
            ConsumeFreeSpin();
            int currentSpins = GameData.Instance.free_lottery_spins.GetIntVal();
            Debug.Log("[FreeSpin] After decrement: " + currentSpins);

            StartRewardSafeMode();

            UILotterManager.Instance.StartLottery(true);
        }
        else
        {
            int cost = GameConfig.Instance.lottery_price.GetIntVal();
            int crystals = GameData.Instance.total_crystal.GetIntVal();

            if (crystals < cost)
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    TUIControls.gameObject,
                    "You do not have enough Crystals to spin the Lottery.",
                    null, null);
                return;
            }

            GameMsgBoxController.ShowMsgBox(
                GameMsgBoxController.MsgBoxType.DoubleButton,
                TUIControls.gameObject,
                "Spend " + cost + " crystals to spin the lottery?",
                new Action(OnConfirmPaidSpin),
                new Action(OnCancelPaidSpin));
        }

        isSpinInProgress = true;
        UpdateSpinButtons();
        if (failSafeCoroutine != null)
        {
            StopCoroutine(failSafeCoroutine);
        }
        failSafeCoroutine = StartCoroutine(FailSafeSpinReset(10f));
    }


    private void OnConfirmPaidSpin()
    {
        int cost = GameConfig.Instance.lottery_price.GetIntVal();
        int crystals = GameData.Instance.total_crystal.GetIntVal();
        GameData.Instance.total_crystal.SetIntVal(crystals - cost, GameDataIntPurpose.Crystal);
        money_controller.UpdateInfo();

        GameData.Instance.rewardSafeMode = true;
        GameData.Instance.SaveData();

        UILotterManager.Instance.StartLottery(false);
    }

    private void OnCancelPaidSpin()
    {
        isSpinInProgress = false;
    }

    private IEnumerator FailSafeSpinReset(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (GameData.Instance.rewardSafeMode)
        {
            GameData.Instance.rewardSafeMode = false;
            GameData.Instance.SaveData();
            Debug.Log("[FailSafe] rewardSafeMode was still true. Force reset after delay.");
        }

        isSpinInProgress = false;
        if (UILotterManager.Instance != null)
        {
            UILotterManager.Instance.ForceStopSpin();
        }
    }

    public void ForceStopSpin()
    {
        if (isSpinInProgress)
        {
            isSpinInProgress = false;
            StopAllCoroutines();
            Debug.Log("[ForceStopSpin] Spin manually stopped.");
        }
    }

    public void UpdateSpinButtons()
    {
        if (GameData.Instance.free_lottery_spins.GetIntVal() > 0)
        {
            lottery_free_button.SetActive(true);
            lottery_button.SetActive(false);
        }
        else
        {
            lottery_free_button.SetActive(false);
            lottery_button.SetActive(true);
        }
    }

    private void LotteryButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            TryStartLotterySpin(false);
        }
    }

    private void OnMsgBoxOkButton()
    {
        Debug.Log("OnMsgBoxOkButton");
        Fade.FadeOut("UIShop");
    }

    private void OnBackButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            Debug.Log("OnBackButton");
            SceneAudio.PlayAudio("UI_back");
            Fade.FadeOut("UIShop");
        }
    }

    private void LotteryFreeButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            TryStartLotterySpin(true);
        }
    }

    public void SetFreeLotterySpins(int newValue)
    {
        int currentValue = GameData.Instance.free_lottery_spins.GetIntVal();

        if (newValue > 5)
        {
            Debug.Log("[UILotteryController] Attempt to increase free spins beyond cap blocked: " + currentValue + " -> " + newValue);
            return;
        }

        GameData.Instance.free_lottery_spins.SetIntVal(newValue);
        displayedFreeSpins = newValue;
        UpdateFreeSpinsLabel();
    }

    private bool hasResetLotteryThisSession = false;

    private void OnResetServerTimeFinish()
    {
        IndicatorBlockController.Hide();

        DateTime trustedNow = DateTime.UtcNow;
        DateTime today = trustedNow.Date;
        DateTime lastReset = GameData.Instance.lastResetDate.Date;

        if (today < GameData.Instance.maxDateReached)
        {
            Debug.LogWarning("[AntiCheat] Time rollback detected. Today: " + today + ", MaxDateReached: " + GameData.Instance.maxDateReached);

            if (!hasAllowedOneRollbackResetThisSession)
            {
                Debug.Log("[AntiCheat] First rollback reset allowed this session.");
                hasAllowedOneRollbackResetThisSession = true;
            }
            else
            {
                Debug.LogWarning("[AntiCheat] Second rollback reset in one session — blacklisting.");
                if (GameEnhancer.Instance != null)
                {
                    GameEnhancer.Instance.BlacklistPlayer("[AntiCheat] Multiple time rollback resets in one session.");
                }
                return;
            }
        }

        if (today > GameData.Instance.maxDateReached)
        {
            GameData.Instance.maxDateReached = today;
            Debug.Log("[AntiCheat] Updated maxDateReached to: " + GameData.Instance.maxDateReached.ToShortDateString());
        }

        if (today > lastReset)
        {
            if (hasTriggeredDailyResetThisSession)
            {
                Debug.LogWarning("[AntiCheat] Detected multiple daily resets in a single session!");

                if (GameEnhancer.Instance != null)
                {
                    GameEnhancer.Instance.BlacklistPlayer("[AntiCheat] Multiple day resets in a single session.");
                }

                return;
            }

            hasTriggeredDailyResetThisSession = true;

            if (GameData.Instance.free_lottery_spins.GetIntVal() == 0)
            {
                GameData.Instance.free_lottery_spins.SetIntVal(1, GameDataIntPurpose.FreeSpin);
                displayedFreeSpins = 1;
                Debug.Log("[Lottery] Free spin granted for today.");
            }
            else
            {
                Debug.Log("[Lottery] Free spin not granted (still unused from yesterday).");
            }

            GameData.Instance.lastResetDate = today;
            GameData.Instance.lastResetSystemTime = DateTime.Now;
            GameData.Instance.SaveData();
        }
        else
        {
            Debug.Log("[Lottery] Same day – no free spin logic triggered.");
        }

        if (GameData.Instance.lottery_reset_count == 0)
        {
            Debug.Log("reset lottery seat for free.");
            lotter_manager.ResetSeatLevel(false);
        }

        lottery_free_button.SetActive(GameData.Instance.lottery_count > 0);
        lottery_button.SetActive(GameData.Instance.lottery_count == 0);

        UpdateLotteryBar();
    }

    private void OnResetSeverTimeError()
    {
        // Uncomment if you want error handling here
        // IndicatorBlockController.Hide();
        // GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Unable to connect to the server! Please try again later.", OnMsgBoxOkButton, null);
    }

    public void EnableBlock(bool state)
    {
        block_bk.SetActive(state);
    }

    private Coroutine rewardSafeModeResetCoroutine;

    private IEnumerator ResetRewardSafeModeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameData.Instance.rewardSafeMode = false;
        GameData.Instance.SaveData();
        Debug.Log("[Lottery] rewardSafeMode reset to false after delay");
    }

    private void StartRewardSafeMode()
    {
        GameData.Instance.rewardSafeMode = true;
        GameData.Instance.SaveData();

        if (rewardSafeModeResetCoroutine != null)
        {
            StopCoroutine(rewardSafeModeResetCoroutine);
        }
        rewardSafeModeResetCoroutine = StartCoroutine(ResetRewardSafeModeAfterDelay(8f));
    }

    private int displayedFreeSpins = 0;

    public void Initialize()
    {
        displayedFreeSpins = GameData.Instance.free_lottery_spins.GetIntVal();
        UpdateFreeSpinsLabel();
        RefreshFreeSpins();
    }

    public void ConsumeFreeSpin()
    {
        int currentSpins = GameData.Instance.free_lottery_spins.GetIntVal();
        currentSpins = Mathf.Max(0, currentSpins - 1);
        GameData.Instance.free_lottery_spins.SetIntVal(currentSpins, GameDataIntPurpose.FreeSpin);

        GameData.Instance.last_saved_free_spin_count = currentSpins;

        GameData.Instance.SaveData();
        UpdateFreeSpinsLabel();
        Debug.Log("[UILotteryController] Consumed free spin, remaining: " + currentSpins);
    }


    public void RefreshFreeSpins()
    {
        displayedFreeSpins = GameData.Instance.free_lottery_spins.GetIntVal();
        UpdateFreeSpinsLabel();
    }

    public void UpdateLotteryBar()
    {
        if (GameData.Instance.lottery_count >= GameConfig.Instance.lottery_award_count.GetIntVal() + 1)
        {
            GameData.Instance.lottery_count = 1;
            lotter_count_award = true;
        }
        int num = GameData.Instance.lottery_count - 1;
        if (num < 0) num = 0;

        float x = (float)num / (float)GameConfig.Instance.lottery_award_count.GetIntVal() * lottery_bar_rect_width;
        lottery_bar_rect.Size = new Vector2(x, 20f);
        lottery_bar_rect.NeedUpdate = true;
        lottery_bar.NeedUpdate = true;

        lottery_bar_label.Text = "Spin 5 Times To Refresh The Board And Get 30 Crystals: " + num + " / 5";

        GameData.Instance.SaveData();
    }

    private void OnTapJoyPointsAdd()
    {
        money_controller.UpdateInfo();
    }
}