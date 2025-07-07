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
        public int crystalReward;
        public int voucherReward;
        public bool isActive;
        public string weaponID;
        public int freeLotterySpins;
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

        PopulateDefaultPromoCodes();
    }

    private void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (emulatorKeyboard != null && emulatorKeyboard.done)
        {
            if (!emulatorKeyboard.wasCanceled)
            {
                TryRedeemCode(emulatorKeyboard.text);
            }
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
            {
                mobileKeyboard = null;
            }
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

        if (GameData.Instance.HasUsedPromoCode(enteredCode))
        {
            return "USED";
        }

        PromoCode promo = null;
        for (int i = 0; i < promoCodes.Count; i++)
        {
            if (promoCodes[i].isActive && promoCodes[i].code.ToUpper() == enteredCode)
            {
                promo = promoCodes[i];
                break;
            }
        }

        if (promo == null)
        {
            GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, gameObject, "Invalid or inactive promo code.", null, null, true);
            return null;
        }

        GameData.Instance.AddUsedPromoCode(enteredCode);

        string rewardSummary = "";

        // Free lottery spins
        if (promo.freeLotterySpins > 0)
        {
            int currentSpins = GameData.Instance.free_lottery_spins.GetIntVal();
            int spinsToAdd = Mathf.Min(promo.freeLotterySpins, 5 - currentSpins);

            if (spinsToAdd > 0)
            {
                GameData.Instance.AddFreeLotterySpins(spinsToAdd, true);

                AwardGetPanel.ShowAwardGetPanel(this.gameObject, null, "tCrystal_Cent499", spinsToAdd);

                rewardSummary += "+ " + spinsToAdd + " Free Lottery Spin(s)\n";
            }
            else
            {
                Debug.Log("[Promo] Free spins already at cap (" + currentSpins + "), no spins added.");
            }
        }

        // Cash reward
        if (promo.cashReward > 0)
        {
            int cur = GameData.Instance.total_cash.GetIntVal();
            GameData.Instance.total_cash.SetIntVal(cur + promo.cashReward, GameDataIntPurpose.Cash);
            GameData.Instance.lastSavedCash = GameData.Instance.total_cash.GetIntVal();
            AwardGetPanel.ShowAwardGetPanel(this.gameObject, null, "Cash_s", promo.cashReward);
            rewardSummary += "+ " + promo.cashReward + " Cash\n";
        }

        // Crystal reward
        if (promo.crystalReward > 0)
        {
            int cur = GameData.Instance.total_crystal.GetIntVal();
            GameData.Instance.total_crystal.SetIntVal(cur + promo.crystalReward, GameDataIntPurpose.Crystal);
            GameData.Instance.lastSavedCrystal = GameData.Instance.total_crystal.GetIntVal();
            AwardGetPanel.ShowAwardGetPanel(this.gameObject, null, "tCrystal_Cent499", promo.crystalReward);
            rewardSummary += "+ " + promo.crystalReward + " Crystals\n";
        }

        // Voucher reward
        if (promo.voucherReward > 0)
        {
            int cur = GameData.Instance.total_voucher.GetIntVal();
            GameData.Instance.total_voucher.SetIntVal(cur + promo.voucherReward, GameDataIntPurpose.Voucher);
            GameData.Instance.lastSavedVoucher = GameData.Instance.total_voucher.GetIntVal();
            AwardGetPanel.ShowAwardGetPanel(this.gameObject, null, "Voucher", promo.voucherReward);
            rewardSummary += "+ " + promo.voucherReward + " Vouchers\n";
        }

        // Weapon or fragment rewards
        if (!string.IsNullOrEmpty(promo.weaponID))
        {
            rewardSummary += HandleWeaponOrFragmentReward(promo.weaponID);
        }

        GameData.Instance.SaveData();
        RefreshCurrencyLabels();

        if (string.IsNullOrEmpty(rewardSummary))
        {
            GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, gameObject, "Code valid, but no rewards.", null, null, true);
            return null;
        }

        return rewardSummary.Trim();
    }

    private string HandleWeaponOrFragmentReward(string weaponID)
    {
        string rewardText = "";

        if (GameData.Instance.WeaponData_Set.ContainsKey(weaponID))
        {
            WeaponData weaponData = (WeaponData)GameData.Instance.WeaponData_Set[weaponID];
            bool owned = weaponData.exist_state == WeaponExistState.Owned;

            if (owned)
            {
                int sellPrice = GameConfig.Instance.WeaponConfig_Set[weaponID].sell_price.GetIntVal();
                int curCash = GameData.Instance.total_cash.GetIntVal();
                GameData.Instance.total_cash.SetIntVal(curCash + sellPrice, GameDataIntPurpose.Cash);
                AwardChangePanel.ShowAwardChangePanel(this.gameObject, OnAwardOk, weaponID, "Cash_s", sellPrice, false);
                rewardText += "+ " + sellPrice + " Cash (sold duplicate " + weaponID + ")\n";
            }
            else
            {
                string fragmentKey = weaponID + "_Fragment";
                WeaponFragmentProbsCfg fragCfg = null;
                GameProb fragmentProb = null;

                if (GameConfig.Instance.ProbsConfig_Set.ContainsKey(fragmentKey))
                {
                    fragCfg = (WeaponFragmentProbsCfg)GameConfig.Instance.ProbsConfig_Set[fragmentKey];
                }

                if (fragCfg == null)
                {
                    if (weaponData.Unlock())
                    {
                        AwardGetPanel.ShowAwardGetPanel(this.gameObject, OnAwardOk, weaponID, 1);
                        rewardText += "+ Weapon " + weaponID + " unlocked!\n";
                    }
                }
                else
                {
                    if (GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(fragmentKey))
                    {
                        fragmentProb = (GameProb)GameData.Instance.WeaponFragmentProbs_Set[fragmentKey];
                        fragmentProb.count++;
                    }
                    else
                    {
                        fragmentProb = new GameProb();
                        fragmentProb.prob_cfg = fragCfg;
                        fragmentProb.count = 1;
                        GameData.Instance.WeaponFragmentProbs_Set.Add(fragmentKey, fragmentProb);
                    }

                    bool unlocked = GameData.Instance.CheckFragmentProbCombine(weaponID) && weaponData.Unlock();

                    if (unlocked)
                    {
                        AwardGetPanel.ShowAwardGetPanel(this.gameObject, OnFragmentCombine, weaponID, 1);
                        rewardText += "+ Weapon " + weaponID + " unlocked!\n";
                    }
                    else
                    {
                        AwardGetPanel.ShowAwardGetPanel(this.gameObject, OnAwardOk, fragmentKey, 1, true);
                        rewardText += "+ Weapon fragment for " + weaponID + "\n";
                    }
                }
            }
        }
        else if (GameConfig.Instance.ProbsConfig_Set.ContainsKey(weaponID))
        {
            WeaponFragmentProbsCfg fragCfg = (WeaponFragmentProbsCfg)GameConfig.Instance.ProbsConfig_Set[weaponID];
            GameProb fragmentProb = null;
            bool isDuplicateFragment = false;

            if (GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(weaponID))
            {
                fragmentProb = (GameProb)GameData.Instance.WeaponFragmentProbs_Set[weaponID];
                if (fragmentProb.count >= 1)
                {
                    isDuplicateFragment = true;
                }
                else
                {
                    fragmentProb.count = 1;
                }
            }
            else
            {
                fragmentProb = new GameProb();
                fragmentProb.prob_cfg = fragCfg;
                fragmentProb.count = 1;
                GameData.Instance.WeaponFragmentProbs_Set.Add(weaponID, fragmentProb);
            }

            if (isDuplicateFragment)
            {
                string weaponNameFromFragment = fragCfg.weapon_name;
                int sellPrice = 0;
                if (GameConfig.Instance.WeaponConfig_Set.ContainsKey(weaponNameFromFragment))
                {
                    sellPrice = GameConfig.Instance.WeaponConfig_Set[weaponNameFromFragment].sell_price.GetIntVal();
                }

                int curCash = GameData.Instance.total_cash.GetIntVal();
                GameData.Instance.total_cash.SetIntVal(curCash + sellPrice, GameDataIntPurpose.Cash);
                AwardChangePanel.ShowAwardChangePanel(this.gameObject, OnAwardOk, weaponNameFromFragment, "Cash_s", sellPrice, false);
                return "+ " + sellPrice + " Cash (sold duplicate part of " + weaponNameFromFragment + ")\n";
            }
            else
            {
                string weaponNameFromFragment = fragCfg.weapon_name;
                WeaponData weaponData = null;
                if (GameData.Instance.WeaponData_Set.ContainsKey(weaponNameFromFragment))
                {
                    weaponData = (WeaponData)GameData.Instance.WeaponData_Set[weaponNameFromFragment];
                }

                bool unlocked = weaponData != null && GameData.Instance.CheckFragmentProbCombine(weaponNameFromFragment) && weaponData.Unlock();

                if (unlocked)
                {
                    AwardGetPanel.ShowAwardGetPanel(this.gameObject, OnFragmentCombine, weaponNameFromFragment, 1);
                    return "+ Weapon " + weaponNameFromFragment + " unlocked!\n";
                }
                else
                {
                    AwardGetPanel.ShowAwardGetPanel(this.gameObject, OnAwardOk, weaponID, 1, true);
                    return "+ Weapon fragment for " + weaponNameFromFragment + "\n";
                }
            }
        }
        else
        {
            Debug.LogWarning("PromoCode weaponID not found in WeaponData_Set or ProbsConfig_Set: " + weaponID);
        }

        return rewardText;
    }

    private void OnFragmentCombine()
    {
        AwardGetPanel.ShowAwardGetPanel(this.gameObject, OnAwardOk, weapon_combine, 1);
        GameData.Instance.rewardSafeMode = false;
    }

    private void OnAwardOk()
    {
        GameData.Instance.rewardSafeMode = false;
    }

    public void AddFreeLotterySpins(int amount, bool force = false)
    {
        if (amount <= 0) return;

        int currentSpins = GameData.Instance.free_lottery_spins.GetIntVal();
        int newTotal = currentSpins + amount;

        if (newTotal > 5)
        {
            newTotal = 5;

            ShowMaxFreeSpinsMessage();
        }

        GameData.Instance.SetFreeLotterySpins(newTotal, force);
    }

    private void ShowMaxFreeSpinsMessage()
    {
        GameMsgBoxController.ShowMsgBox(
            GameMsgBoxController.MsgBoxType.SingleButton,
            null,
            "You have reached the maximum of 5 free spins. Use them before earning more!",
            null,
            null
        );
    }

    public bool ConsumeFreeLotterySpin()
    {
        int currentSpins = GameData.Instance.free_lottery_spins.GetIntVal();
        if (currentSpins > 0)
        {
            GameData.Instance.free_lottery_spins.SetIntVal(currentSpins - 1, GameDataIntPurpose.FreeSpin);
            return true;
        }
        return false;
    }

    public void RefreshCurrencyLabels()
    {
        if (cashLabel != null)
            cashLabel.Text = GameData.Instance.total_cash.GetIntVal().ToString();

        if (crystalLabel != null)
            crystalLabel.Text = GameData.Instance.total_crystal.GetIntVal().ToString();

        if (voucherLabel != null)
            voucherLabel.Text = GameData.Instance.total_voucher.GetIntVal().ToString();
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

    private void PopulateDefaultPromoCodes()
    {
        if (promoCodes == null || promoCodes.Count == 0)
        {
            promoCodes = new List<PromoCode>();
            promoCodes.Add(new PromoCode { code = "CRYSTAL", cashReward = 0, crystalReward = 15, voucherReward = 0, isActive = true });
            promoCodes.Add(new PromoCode { code = "GAMBLING", cashReward = 2000, crystalReward = 15, voucherReward = 25, isActive = true });
            promoCodes.Add(new PromoCode { code = "VOUCHER50", cashReward = 0, crystalReward = 0, voucherReward = 50, isActive = true });
            promoCodes.Add(new PromoCode { code = "BOOMER", cashReward = 1500, crystalReward = 5, voucherReward = 10, isActive = true });
            promoCodes.Add(new PromoCode { code = "FREEAK47", cashReward = 0, crystalReward = 0, voucherReward = 0, isActive = true, weaponID = "AK47" });
            promoCodes.Add(new PromoCode { code = "XM12GIFT", cashReward = 0, crystalReward = 0, voucherReward = 0, isActive = true, weaponID = "XM12_2" });
            promoCodes.Add(new PromoCode { code = "LOTTERY3", cashReward = 0, crystalReward = 0, voucherReward = 0, isActive = true, freeLotterySpins = 3 });
        }
    }
}
