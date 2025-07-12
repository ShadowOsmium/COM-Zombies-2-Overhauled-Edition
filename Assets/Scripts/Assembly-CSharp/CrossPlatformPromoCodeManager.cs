using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossPlatformPromoCodeManager : MonoBehaviour
{
    private string weapon_combine = string.Empty;

    [System.Serializable]
    public class PromoCode
    {
        public string code;
        public int cashReward;
        public float cashRewardMultiplier;
        public int crystalReward;
        public int voucherReward;
        public bool isActive;
        public string weaponID;
        public bool useDynamicCashScaling;
        public int freeLotterySpins;
        public bool grantFullWeaponDirectly;
        public bool scaleAtDay35;
        public bool scaleAtDay55;
        public bool scaleAtDay85;
    }

    [Header("Currency UI Labels")]
    public TUILabel cashLabel;
    public TUILabel crystalLabel;
    public TUILabel voucherLabel;

    [Header("Promo Codes")]
    public List<PromoCode> promoCodes;

    [Header("References")]
    public Text feedbackText;
    public GameData gameData;

#if UNITY_STANDALONE || UNITY_EDITOR
    private TouchScreenKeyboard emulatorKeyboard;
#else
    private TouchScreenKeyboard mobileKeyboard;
#endif

    private void Awake()
    {
        if (gameData == null && GameData.Instance != null)
            gameData = GameData.Instance;
    }

    private void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (emulatorKeyboard != null && emulatorKeyboard.done)
        {
            if (!emulatorKeyboard.wasCanceled) TryRedeemCode(emulatorKeyboard.text);
            emulatorKeyboard = null;
        }
#else
        if (mobileKeyboard != null)
        {
            if (mobileKeyboard.status == TouchScreenKeyboard.Status.Done)
            {
                TryRedeemCode(mobileKeyboard.text);
                mobileKeyboard = null;
            }
            else if (mobileKeyboard.status == TouchScreenKeyboard.Status.Canceled ||
                     mobileKeyboard.status == TouchScreenKeyboard.Status.LostFocus)
                mobileKeyboard = null;
        }
#endif
    }

    public void OpenPromoCodeKeyboard()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        emulatorKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, "Enter promo code");
#else
        mobileKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, "Enter promo code");
#endif
    }

    public string TryRedeemCode(string inputCode)
    {
        if (string.IsNullOrEmpty(inputCode))
        {
            GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, gameObject, "Please enter a promo code.", null, null, true);
            return null;
        }

        string enteredCode = inputCode.Trim().ToUpper();
        if (GameData.Instance.HasUsedPromoCode(enteredCode)) return "USED";

        PromoCode promo = promoCodes.Find(delegate (PromoCode p) { return p.isActive && p.code.ToUpper() == enteredCode; });
        if (promo == null)
        {
            GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, gameObject, "Invalid or inactive promo code.", null, null, true);
            return null;
        }

        GameData.Instance.AddUsedPromoCode(enteredCode);
        string rewardSummary = "";

        if (promo.freeLotterySpins > 0)
        {
            int currentSpins = GameData.Instance.free_lottery_spins.GetIntVal();
            int spinsToAdd = Mathf.Min(promo.freeLotterySpins, 5 - currentSpins);
            if (spinsToAdd > 0)
            {
                GameData.Instance.AddFreeLotterySpins(spinsToAdd, true);
                AwardGetPanel.ShowAwardGetPanel(gameObject, null, "tCrystal_Cent499", spinsToAdd);
                rewardSummary += "+ " + spinsToAdd + " Free Lottery Spin(s), ";
            }
        }

        if (promo.cashReward > 0)
        {
            int cur = GameData.Instance.total_cash.GetIntVal();
            int scaled = promo.useDynamicCashScaling ?
                Mathf.RoundToInt(promo.cashRewardMultiplier * GameData.Instance.GetSideEnemyStandardRewardTotal() * 0.1f) :
                ScaleRewardByThresholds(promo.cashReward, promo);
            GameData.Instance.total_cash.SetIntVal(cur + scaled, GameDataIntPurpose.Cash);
            GameData.Instance.lastSavedCash = GameData.Instance.total_cash.GetIntVal();
            AwardGetPanel.ShowAwardGetPanel(gameObject, null, "Cash_s", scaled);
            rewardSummary += "+ " + scaled + " Cash, ";
        }

        if (promo.crystalReward > 0)
        {
            int cur = GameData.Instance.total_crystal.GetIntVal();
            int scaled = ScaleRewardByThresholds(promo.crystalReward, promo);
            GameData.Instance.total_crystal.SetIntVal(cur + scaled, GameDataIntPurpose.Crystal);
            GameData.Instance.lastSavedCrystal = GameData.Instance.total_crystal.GetIntVal();
            AwardGetPanel.ShowAwardGetPanel(gameObject, null, "tCrystal_Cent499", scaled);
            rewardSummary += "+ " + scaled + " Crystals, ";
        }

        if (promo.voucherReward > 0)
        {
            int cur = GameData.Instance.total_voucher.GetIntVal();
            int scaled = ScaleRewardByThresholds(promo.voucherReward, promo);
            GameData.Instance.total_voucher.SetIntVal(cur + scaled, GameDataIntPurpose.Voucher);
            GameData.Instance.lastSavedVoucher = GameData.Instance.total_voucher.GetIntVal();
            AwardGetPanel.ShowAwardGetPanel(gameObject, null, "Voucher", scaled);
            rewardSummary += "+ " + scaled + " Vouchers, ";
        }

        if (!string.IsNullOrEmpty(promo.weaponID))
            rewardSummary += HandleWeaponOrFragmentReward(promo.weaponID, promo.grantFullWeaponDirectly);

        GameData.Instance.SaveData();
        RefreshCurrencyLabels();

        if (string.IsNullOrEmpty(rewardSummary))
        {
            GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, gameObject, "Code valid, but no rewards.", null, null, true);
            return null;
        }

        rewardSummary = rewardSummary.TrimEnd(',', ' ');
        GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, gameObject, rewardSummary, null, null, true);
        return rewardSummary;
    }

    private int ScaleRewardByThresholds(int baseReward, PromoCode promo)
    {
        int dayLevel = GameData.Instance.day_level;
        if (dayLevel > 85 && promo.scaleAtDay85) return baseReward * 4;
        if (dayLevel > 55 && promo.scaleAtDay55) return baseReward * 3;
        if (dayLevel > 35 && promo.scaleAtDay35) return baseReward * 2;
        return baseReward;
    }

    private void OnFragmentCombine()
    {
        AwardGetPanel.ShowAwardGetPanel(gameObject, OnAwardOk, weapon_combine, 1);
        GameData.Instance.rewardSafeMode = false;
    }

    private void OnAwardOk()
    {
        GameData.Instance.rewardSafeMode = false;
    }

    public void RefreshCurrencyLabels()
    {
        if (cashLabel != null) cashLabel.Text = GameData.Instance.total_cash.GetIntVal().ToString();
        if (crystalLabel != null) crystalLabel.Text = GameData.Instance.total_crystal.GetIntVal().ToString();
        if (voucherLabel != null) voucherLabel.Text = GameData.Instance.total_voucher.GetIntVal().ToString();
    }

    private string HandleWeaponOrFragmentReward(string weaponID, bool grantFullWeaponDirectly)
    {
        string rewardText = "";

        if (GameData.Instance.WeaponData_Set.ContainsKey(weaponID))
        {
            WeaponData weaponData = (WeaponData)GameData.Instance.WeaponData_Set[weaponID];
            bool owned = weaponData.exist_state == WeaponExistState.Owned;

            if (grantFullWeaponDirectly)
            {
                if (weaponData.IsFullyOwned())
                {
                    int sellPrice = GameConfig.Instance.WeaponConfig_Set[weaponID].sell_price.GetIntVal();
                    AddCash(sellPrice);
                    AwardChangePanel.ShowAwardChangePanel(gameObject, OnAwardOk, weaponID, "Cash_s", sellPrice, false);
                    rewardText += "+ " + sellPrice + " Cash (duplicate " + weaponID + ")\n";
                }
                else
                {
                    weaponData.exist_state = WeaponExistState.Owned;
                    Debug.Log("Directly granting weapon ownership for " + weaponID);
                    AwardGetPanel.ShowAwardGetPanel(gameObject, OnAwardOk, weaponID, 1);
                    rewardText += "+ Weapon " + weaponID + " granted directly!\n";
                }
            }
            else
            {
                if (owned)
                {
                    int sellPrice = GameConfig.Instance.WeaponConfig_Set[weaponID].sell_price.GetIntVal();
                    AddCash(sellPrice);
                    AwardChangePanel.ShowAwardChangePanel(gameObject, OnAwardOk, weaponID, "Cash_s", sellPrice, false);
                    rewardText += "+ " + sellPrice + " Cash (sold duplicate " + weaponID + ")\n";
                }
                else if (weaponData.exist_state == WeaponExistState.Unlocked)
                {
                    Debug.Log("Weapon exist_state changed from Unlocked to Owned: " + weaponID);
                    weaponData.exist_state = WeaponExistState.Owned;
                    AwardGetPanel.ShowAwardGetPanel(gameObject, OnAwardOk, weaponID, 1);
                    rewardText += "+ Weapon " + weaponID + " granted directly!\n";
                }
                else if (weaponData.Unlock())
                {
                    Debug.Log("Weapon unlocked via Unlock() method for: " + weaponID);
                    AwardGetPanel.ShowAwardGetPanel(gameObject, OnAwardOk, weaponID, 1);
                    rewardText += "+ Weapon " + weaponID + " granted directly!\n";
                }
                else
                {
                    rewardText += "+ Failed to grant weapon " + weaponID + "\n";
                }
            }
        }
        else if (GameConfig.Instance.ProbsConfig_Set.ContainsKey(weaponID))
        {
            rewardText += HandleFragmentReward(weaponID);
        }
        else
        {
            Debug.LogWarning("PromoCode weaponID not found: " + weaponID);
        }

        return rewardText;
    }


    private string HandleFragmentReward(string fragmentKey, string parentWeaponID = null, WeaponData weaponData = null)
    {
        WeaponFragmentProbsCfg fragCfg = (WeaponFragmentProbsCfg)GameConfig.Instance.ProbsConfig_Set[fragmentKey];
        GameProb fragmentProb;
        bool isDuplicate = false;

        if (GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(fragmentKey))
        {
            fragmentProb = (GameProb)GameData.Instance.WeaponFragmentProbs_Set[fragmentKey];
            isDuplicate = fragmentProb.count >= 1;
            fragmentProb.count++;
        }
        else
        {
            fragmentProb = new GameProb();
            fragmentProb.prob_cfg = fragCfg;
            fragmentProb.count = 1;
            GameData.Instance.WeaponFragmentProbs_Set.Add(fragmentKey, fragmentProb);
        }

        string weaponNameFromFragment = fragCfg.weapon_name;
        if (weaponData == null && GameData.Instance.WeaponData_Set.ContainsKey(weaponNameFromFragment))
            weaponData = (WeaponData)GameData.Instance.WeaponData_Set[weaponNameFromFragment];

        if (isDuplicate)
        {
            int sellPrice = 0;
            if (GameConfig.Instance.WeaponConfig_Set.ContainsKey(weaponNameFromFragment))
                sellPrice = GameConfig.Instance.WeaponConfig_Set[weaponNameFromFragment].sell_price.GetIntVal();

            int halfPrice = sellPrice / 2;
            AddCash(halfPrice);
            AwardChangePanel.ShowAwardChangePanel(gameObject, OnAwardOk, weaponNameFromFragment, "Cash_s", halfPrice, false);
            return "+ " + halfPrice + " Cash (sold duplicate part of " + weaponNameFromFragment + ")\n";
        }
        else
        {
            bool unlocked = weaponData != null && GameData.Instance.CheckFragmentProbCombine(weaponNameFromFragment) && weaponData.Unlock();

            if (unlocked)
            {
                weapon_combine = weaponNameFromFragment;
                AwardGetPanel.ShowAwardGetPanel(gameObject, OnFragmentCombine, weaponNameFromFragment, 1);
                return "+ Weapon " + weaponNameFromFragment + " unlocked!\n";
            }
            else
            {
                AwardGetPanel.ShowAwardGetPanel(gameObject, OnAwardOk, fragmentKey, 1, true);
                return "+ Weapon fragment for " + weaponNameFromFragment + "\n";
            }
        }
    }

    [ContextMenu("ClearUsedPromoCodes")]
    public void ClearUsedPromoCodes()
    {
        if (GameData.Instance == null)
        {
            Debug.LogError("GameData.Instance is null! Cannot clear promo codes.");
            return;
        }

        GameData.Instance.ClearUsedPromoCodes();
        GameData.Instance.SaveData();
        Debug.LogWarning("Used promo codes cleared!");
    }

    private void AddCash(int amount)
    {
        if (amount <= 0) return;
        int cur = GameData.Instance.total_cash.GetIntVal();
        GameData.Instance.total_cash.SetIntVal(cur + amount, GameDataIntPurpose.Cash);
        GameData.Instance.lastSavedCash = GameData.Instance.total_cash.GetIntVal();
    }
}
