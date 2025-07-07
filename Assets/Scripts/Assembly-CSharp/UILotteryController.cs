using System.Collections;
using UnityEngine;

public class UILotteryController : UISceneController
{
    public UILotterManager lotter_manager;

    public GameObject lottery_button;        // Paid spin button
    public GameObject lottery_free_button;   // Free spin button

    public TUILabel lottery_price_p;
    public TUILabel lottery_price;
    public TUILabel reset_price;

    public GameObject block_bk;

    public UIMoneyController money_controller;

    public TUILabel free_spins_label;
    public TUILabel lottery_bar_label;

    public TUIMeshSprite lottery_bar;
    public TUIRect lottery_bar_rect;

    private float lottery_bar_rect_width = 322f;

    public bool lotter_count_award;

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
        UpdateFreeSpinsLabel();
        UpdateSpinButtons();
        RefreshFreeSpins();
    }

    private void Start()
    {
        Debug.Log("UILotteryController Start - lotter_manager: " + (lotter_manager == null ? "NULL" : "NOT NULL"));
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
        if (isFreeSpin)
        {
            if (GameData.Instance.free_lottery_spins <= 0)
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    TUIControls.gameObject,
                    "No free lottery spins left.",
                    null,
                    null
                );
                return;
            }

            GameData.Instance.rewardSafeMode = true;
            lotter_manager.StartLottery(true);

            if (GameEnhancer.Instance != null)
            {
                GameEnhancer.Instance.OnPlayerUsedFreeSpin();
            }

            if (!GameData.Instance.ConsumeFreeLotterySpin())
            {
                Debug.LogWarning("[UILotteryController] Attempted to consume free spin but none left.");
                // Handle error or block spin
            }

            UpdateSpinButtons();
        }
        else
        {
            int lotteryPrice = Mathf.Max(0, GameConfig.Instance.lottery_price.GetIntVal());
            int playerCrystals = GameData.Instance.total_crystal.GetIntVal();

            if (playerCrystals < lotteryPrice)
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    TUIControls.gameObject,
                    "You do not have enough Crystals to spin the Lottery.",
                    null,
                    null
                );
                return;
            }

            GameMsgBoxController.ShowMsgBox(
                GameMsgBoxController.MsgBoxType.DoubleButton,
                TUIControls.gameObject,
                "Spend " + lotteryPrice + " crystals to spin the lottery?",
                () =>
                {
                    GameData.Instance.total_crystal.SetIntVal(playerCrystals - lotteryPrice, GameDataIntPurpose.Crystal);
                    money_controller.UpdateInfo();

                    GameData.Instance.rewardSafeMode = true;
                    lotter_manager.StartLottery(false);

                    UpdateSpinButtons();
                },
                () => { }
            );
        }
    }

    public void UpdateSpinButtons()
    {
        if (GameData.Instance.free_lottery_spins > 0)
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

        Debug.Log("OnResetServerTimeFinish called - lotter_manager: " + (lotter_manager == null ? "NULL" : "NOT NULL"));

        if (GameData.Instance.lottery_reset_count == 0 && !hasResetLotteryThisSession)
        {
            Debug.Log("ResetSeatLevel called");
            lotter_manager.ResetSeatLevel(false);
            hasResetLotteryThisSession = true;

            lotter_manager.InitLotterSeat();
        }
        else
        {
            lotter_manager.InitLotterSeat();
        }

        UpdateSpinButtons();
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

    private bool isResettingRewardSafeMode = false;

    private IEnumerator ResetRewardSafeModeAfterDelay(float delay)
    {
        if (isResettingRewardSafeMode) yield break;
        isResettingRewardSafeMode = true;
        yield return new WaitForSeconds(delay);
        GameData.Instance.rewardSafeMode = false;
        isResettingRewardSafeMode = false;
        Debug.Log("[Lottery] rewardSafeMode reset to false");
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
        if (displayedFreeSpins > 0)
        {
            displayedFreeSpins--;
            GameData.Instance.free_lottery_spins.SetIntVal(displayedFreeSpins);
            UpdateFreeSpinsLabel();
        }
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