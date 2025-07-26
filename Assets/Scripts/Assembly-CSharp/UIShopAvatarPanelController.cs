using CoMZ2;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIShopAvatarPanelController : UIShopPanelController
{
	private AvatarData currentPlayer;

	public TUILabel Name;

	public WeaponProperty[] Properties;

	public TUIMeshSprite PrimaryWeapon;

	public TUIMeshSprite SecondaryWeapon;

	public GameObject BuyAvatar;

	public GameObject UnlockAvatar;

    public TUIButtonClick CycleLookButton;

    private AvatarType? lastSavedAvatarType = null;

    private AvatarData.AvatarState _selectedVisualState;

    private GameMsgBoxController msgBox;

	private int upgradePropertyIdx = -1;

    private delegate bool UpgradeAction();

    private UpgradeAction[] upgradeActions;

    private Func<int>[] priceGetters;

    private GameObject newLabel;

	public AvatarLevelTipController avatar_level_tip;

    private struct PropertyUpdate
    {
        public string Name;
        public string Value;
        public string UpgradeValue;
        public string CostText;
        public bool CanUpgrade;
    }

    private void Awake()
    {
        upgradeActions = new UpgradeAction[] {
        () => currentPlayer.UpgradeHp(),
        () => currentPlayer.UpgradeDamage(),
        () => currentPlayer.UpgradeArmor()
    };

        priceGetters = new Func<int>[] {
        () => currentPlayer.UpgradeHpPrice,
        () => currentPlayer.UpgradeDamagePrice,
        () => currentPlayer.UpgradeArmorPrice
    };
    }

    private PropertyUpdate GetHealthPropertyUpdate()
    {
        var update = new PropertyUpdate();
        update.Name = string.Format("Health (LV {0} Out Of {1})", currentPlayer.hp_level, currentPlayer.config.max_level);
        update.Value = currentPlayer.hp_capacity.ToString("N0");

        bool maxed = currentPlayer.hp_level >= currentPlayer.config.max_level;
        update.UpgradeValue = !maxed ? "+" + (currentPlayer.hp_capacity_next - currentPlayer.hp_capacity).ToString("N0") : string.Empty;
        update.CostText = currentPlayer.UpgradeHpPrice.ToString("G");
        update.CanUpgrade = !maxed;
        return update;
    }

    private PropertyUpdate GetDamagePropertyUpdate()
    {
        var update = new PropertyUpdate();
        int curLevel = currentPlayer.damage_level;
        int maxLevel = Mathf.Max(2, currentPlayer.config.max_level);

        update.Name = string.Format("Damage (LV {0} Out Of {1})", curLevel, maxLevel);

        float normalizedLevel = Mathf.Clamp01((float)(curLevel - 1) / (maxLevel - 1));
        float curvedFraction = Mathf.Pow(normalizedLevel, 1.5f);
        float currentDamageVal = Mathf.Lerp(currentPlayer.config.damage_conf.base_data, currentPlayer.config.damage_conf.max_data, curvedFraction);

        update.Value = (1f + currentDamageVal / 100f).ToString("F2") + "x";

        bool maxed = curLevel >= maxLevel;
        if (!maxed)
        {
            float nextNormalizedLevel = Mathf.Clamp01((float)(curLevel) / (maxLevel - 1));
            float nextCurvedFraction = Mathf.Pow(nextNormalizedLevel, 1.5f);
            float nextDamageVal = Mathf.Lerp(currentPlayer.config.damage_conf.base_data, currentPlayer.config.damage_conf.max_data, nextCurvedFraction);

            float percentUpgrade = nextDamageVal - currentDamageVal;
            update.UpgradeValue = percentUpgrade > 0.01f ? "+" + percentUpgrade.ToString("F2") + " Damage" : "+<0% Damage";
        }
        else
        {
            update.UpgradeValue = string.Empty;
        }

        update.CostText = currentPlayer.UpgradeDamagePrice.ToString("G");
        update.CanUpgrade = !maxed;
        return update;
    }

    private PropertyUpdate GetArmorPropertyUpdate()
    {
        var update = new PropertyUpdate();
        int level = currentPlayer.armor_level;
        int maxLevel = currentPlayer.config.max_level;

        update.Name = string.Format("Armor (LV {0} Out Of {1})", level, maxLevel);

        float baseArmor = currentPlayer.config.armor_conf.base_data;
        float currentArmor = currentPlayer.armor_val;

        float reduction = currentArmor / (currentArmor + 100f);
        float percent = reduction * 100f;

        update.Value = "-" + percent.ToString("F2") + "% dmg";

        bool maxed = level >= maxLevel;
        if (!maxed)
        {
            float nextArmor = currentPlayer.GetScaledStat(
                currentPlayer.config.armor_conf.base_data,
                currentPlayer.config.armor_conf.max_data,
                level + 1,
                maxLevel
            );

            float nextReduction = nextArmor / (nextArmor + 100f);
            float upgrade = nextReduction - reduction;

            update.UpgradeValue = upgrade >= 0.0001f ? "+" + (upgrade * 100f).ToString("F2") + "% resist" : "+<0.1% resist";
        }
        else
        {
            update.UpgradeValue = string.Empty;
        }

        update.CostText = currentPlayer.UpgradeArmorPrice.ToString("G");
        update.CanUpgrade = !maxed;
        return update;
    }

    private void UpdateProperty(int index)
    {
        if (currentPlayer == null || Properties == null || index < 0 || index >= Properties.Length)
            return;

        PropertyUpdate update;
        switch (index)
        {
            case 0: update = GetHealthPropertyUpdate(); break;
            case 1: update = GetDamagePropertyUpdate(); break;
            case 2: update = GetArmorPropertyUpdate(); break;
            default: return;
        }

        Properties[index].Name.Text = update.Name;
        Properties[index].Value.Text = update.Value;
        Properties[index].UpgradeValue.Text = update.UpgradeValue;

        var upgradeBtn = Properties[index].UpgradeButton;
        if (currentPlayer.exist_state == AvatarExistState.Owned)
        {
            upgradeBtn.gameObject.SetActive(true);
            upgradeBtn.Disable(!update.CanUpgrade);
            upgradeBtn.m_NormalLabelObj.GetComponent<TUILabel>().Text = update.CostText;
            upgradeBtn.m_PressLabelObj.GetComponent<TUILabel>().Text = update.CostText;
        }
        else
        {
            upgradeBtn.gameObject.SetActive(false);
        }
    }


    float GetArmorDamageReduction(float armorVal)
    {
        return armorVal / (armorVal + 100f);
    }

    float GetDisplayMultiplier(float baseData, float maxData, float currentData)
    {
        if (baseData <= 0f)
        {
            Debug.LogWarning(string.Format("Invalid baseData: {0}", baseData));
            return 1f;
        }
        float realMultiplier = currentData / baseData;
        return realMultiplier;
    }

    private void UpdateAvatarInfo()
	{
		if (currentPlayer.exist_state == AvatarExistState.Owned)
		{
			PrimaryWeapon.transform.parent.Find("Btn").GetComponent<TUIButtonClick>().Disable(false);
			SecondaryWeapon.transform.parent.Find("Btn").GetComponent<TUIButtonClick>().Disable(false);
			BuyAvatar.SetActive(false);
			UnlockAvatar.SetActive(false);
		}
		else
		{
			PrimaryWeapon.transform.parent.Find("Btn").GetComponent<TUIButtonClick>().Disable(true);
			SecondaryWeapon.transform.parent.Find("Btn").GetComponent<TUIButtonClick>().Disable(true);
			if (currentPlayer.exist_state == AvatarExistState.Locked)
			{
				BuyAvatar.SetActive(false);
				UnlockAvatar.SetActive(true);
				UnlockAvatar.transform.Find("Amount").GetComponent<TUILabel>().Text = currentPlayer.config.crystal_unlock_price.ToString();
				UnlockAvatar.transform.Find("TipUnlock").GetComponent<TUILabel>().Text = "COMPLETE  DAY  " + currentPlayer.config.unlockDay + "  TO  UNLOCK";
			}
			else
			{
				BuyAvatar.SetActive(true);
				BuyAvatar.transform.Find("Amount").GetComponent<TUILabel>().Text = currentPlayer.config.price.ToString();
				UnlockAvatar.SetActive(false);
			}
		}
		for (int i = 0; i < Properties.Length; i++)
		{
			UpdateProperty(i);
		}
		avatar_level_tip.ResetTip();
	}

    public override void Show()
    {
        if (UIShopSceneController.Instance.PanelStack.Count == 0 || UIShopSceneController.Instance.PanelStack.Peek() != this)
        {
            AnimationUtil.PlayAnimate(base.gameObject, "AvatarPanelIn", WrapMode.Once);
        }
        base.Show();
        ChangeMask("MaskAva");

        currentPlayer = UIShopSceneController.Instance.CurrentAvatar.avatarData;
        avatar_level_tip.Init(currentPlayer);

        if (lastSavedAvatarType == null || lastSavedAvatarType != currentPlayer.avatar_type)
        {
            if (GameEnhancer.Instance.TryMarkSafeSave())
            {
                lastSavedAvatarType = currentPlayer.avatar_type;
                GameData.Instance.SaveData();
            }
            else
            {
                Debug.Log("Safe save cooldown active, ignoring rapid avatar save.");
            }
        }

        Name.Text = currentPlayer.show_name;
        UpdateAvatarInfo();

        PrimaryWeapon.texture = "weapon_" + currentPlayer.primary_equipment;
        SecondaryWeapon.texture = "skill_" + currentPlayer.skill_list[0];

        if (GameData.Instance.UnlockList.Count <= 0)
        {
            return;
        }

        foreach (UnlockInGame unlock in GameData.Instance.UnlockList)
        {
            if (unlock.Type == UnlockInGame.UnlockType.Weapon)
            {
                newLabel = Object.Instantiate(Resources.Load("Prefab/NewLabel")) as GameObject;
                newLabel.transform.parent = PrimaryWeapon.transform.parent;
                newLabel.transform.localPosition = new Vector3(-78f, 35f, -1f);
                break;
            }
        }
        _selectedVisualState = currentPlayer.avatar_state;
        ApplyVisualLook();
    }

    private void ApplyVisualLook()
    {
        UIShopSceneController.Instance.AvatarDisplay.ShowAvatar(_selectedVisualState);

        avatar_level_tip.tip_icon.texture =
            currentPlayer.config.avatar_name + "_0" + (int)_selectedVisualState + "_icon";
    }

    public override void Hide(bool isPopFromStack)
	{
		base.Hide(isPopFromStack);
		if (isPopFromStack)
		{
			UIShopSceneController.Instance.MainCamera.SmoothCameraTo(UIShopSceneController.Instance.CameraOrigin);
			UIShopSceneController.Instance.CurrentAvatar.OnBackFromClicked();
		}
		if (newLabel != null)
		{
			Object.Destroy(newLabel);
		}
	}

	private void OnCrystalUnlockOk()
	{
		Object.Destroy(msgBox.gameObject);
		if (currentPlayer.CrystalUnlock())
		{
			UISceneController.Instance.MoneyController.UpdateInfo();
			UIShopSceneController.Instance.UpdateAvatarShaders();
			UpdateAvatarInfo();
			ShowNewAvatarTip();
		}
		else
		{
			//UISceneController.Instance.MoneyController.IapPanel.Show();
		}
	}

    private void ShowNewAvatarTip()
    {
        if (currentPlayer.avatar_type == AvatarType.Doctor)
        {
            int lastUnlockedMonsterIndex = GetLastUnlockedMonsterIndex();

            if (lastUnlockedMonsterIndex >= 0 && lastUnlockedMonsterIndex < GameConfig.Instance.Skill_Enchant_Monster_List.Count)
            {
                string unlockedMonster = GameConfig.Instance.Skill_Enchant_Monster_List[lastUnlockedMonsterIndex].ToString();
                ImageMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, unlockedMonster, base.gameObject, "Now charm a " + unlockedMonster + " as your sidekick!", null, null);
            }
        }
    }

    private int lastUnlockedMonsterIndex = -1;

    public void UnlockMonster(int monsterIndex)
    {
        if (monsterIndex >= 0 && monsterIndex < GameConfig.Instance.Skill_Enchant_Monster_List.Count)
        {
            lastUnlockedMonsterIndex = monsterIndex;
        }
    }

    private int GetLastUnlockedMonsterIndex()
    {
        return lastUnlockedMonsterIndex;
    }

    private void OnExchangeMoneyOk()
    {
        int playerCrystals = GameData.Instance.total_crystal.GetIntVal();
        int requiredCrystals = ((CrystalMsgBoxController)msgBox).Exchange.Crystal;

        if (requiredCrystals <= playerCrystals)
        {
            GameData.Instance.OnExchgCurrcy(GameCurrencyType.Crystal, GameCurrencyType.Voucher, requiredCrystals, ((CrystalMsgBoxController)msgBox).Exchange.Voucher);

            if (upgradePropertyIdx == -1)
            {
                currentPlayer.Buy();
                UISceneController.Instance.MoneyController.UpdateInfo();
                UIShopSceneController.Instance.UpdateAvatarShaders();
                UpdateAvatarInfo();
                ShowNewAvatarTip();
            }
            else
            {
                if (upgradePropertyIdx == 0)
                {
                    currentPlayer.UpgradeHp();
                }
                else if (upgradePropertyIdx == 1)
                {
                    currentPlayer.UpgradeDamage();
                }
                else
                {
                    currentPlayer.UpgradeArmor();
                }
                UISceneController.Instance.MoneyController.UpdateInfo();
                UpdateProperty(upgradePropertyIdx);
                upgradePropertyIdx = -1;
                UIShopSceneController.Instance.CheckAvatarPosInShop();
                avatar_level_tip.ResetTip();
            }
        }
        else
        {
            Debug.Log("Not enough crystals to exchange for vouchers.");
        }
        Destroy(msgBox.gameObject);
    }

    private void OnMsgboxCancel()
	{
        Destroy(msgBox.gameObject);
	}

	private void OnUnlockAvatarEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			string text = "Spend ";
			text += currentPlayer.config.crystal_unlock_price.ToString();
			text += " tCrystals to own this character permanently?";
			msgBox = GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.DoubleButton, base.gameObject, text, OnCrystalUnlockOk, OnMsgboxCancel);
		}
	}

	private void OnBuyAvatarEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			if (currentPlayer.Buy())
			{
				UISceneController.Instance.MoneyController.UpdateInfo();
				UIShopSceneController.Instance.UpdateAvatarShaders();
				UpdateAvatarInfo();
				ShowNewAvatarTip();
				return;
			}
			int intVal = (currentPlayer.config.price - GameData.Instance.total_voucher).GetIntVal();
			int crystal = Mathf.CeilToInt((float)intVal / GameConfig.Instance.crystal_to_voucher);
			string content = "You need " + intVal + " more vouchers to buy this.";
			CrystalExchangeCash crystalExchangeCash = new CrystalExchangeCash();
			crystalExchangeCash.Crystal = crystal;
			crystalExchangeCash.Voucher = intVal;
			CrystalExchangeCash exchange = crystalExchangeCash;
			msgBox = CrystalMsgBoxController.ShowMsgBox(exchange, base.gameObject, content, OnExchangeMoneyOk, OnMsgboxCancel);
		}
	}

    private void OnUpgradePropertyEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 3) return;
        PlayClickSound();

        upgradePropertyIdx = int.Parse(control.transform.parent.name.Substring("Prop".Length)) - 1;

        if (upgradePropertyIdx < 0 || upgradePropertyIdx >= upgradeActions.Length) return;

        bool success = upgradeActions[upgradePropertyIdx]();

        UIShopSceneController.Instance.CheckAvatarPosInShop();
        avatar_level_tip.ResetTip();

        if (success)
        {
            UISceneController.Instance.MoneyController.UpdateInfo();
            UpdateProperty(upgradePropertyIdx);
            upgradePropertyIdx = -1;
            return;
        }
        else
        {
            int neededVouchers = priceGetters[upgradePropertyIdx]() - GameData.Instance.total_voucher.GetIntVal();
            int crystals = Mathf.CeilToInt((float)neededVouchers / GameConfig.Instance.crystal_to_voucher);
            string content = "You need " + neededVouchers + " more vouchers to buy this.";
            var exchange = new CrystalExchangeCash { Crystal = crystals, Voucher = neededVouchers };
            msgBox = CrystalMsgBoxController.ShowMsgBox(exchange, base.gameObject, content, OnExchangeMoneyOk, OnMsgboxCancel);
        }
    }

    private void PlayClickSound()
    {
        UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
    }

    private void CloseMsgBox()
    {
        if (msgBox != null)
        {
            Object.Destroy(msgBox.gameObject);
            msgBox = null;
        }
    }

    float GetPercentIncrease(UpgradeConfig conf, int level, int maxLevel)
    {
        float baseRatio = (conf.max_data / conf.base_data - 1f);
        float normalizedLevel = (float)(level - 1) / (maxLevel - 1);
        float adjustedRatio = baseRatio * normalizedLevel;
        float percent = adjustedRatio * 100f;

        Debug.Log(string.Format("GetPercentIncrease: base_data={0}, max_data={1}, level={2}, maxLevel={3}, percent={4}",
            conf.base_data, conf.max_data, level, maxLevel, percent));

        return percent;
    }

    private void OnCycleLookButtonClick(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 3) return;

        UISceneController.Instance.SceneAudio.PlayAudio("UI_click");

        string confirmText = "Do you want to change the avatar's look?";

        msgBox = GameMsgBoxController.ShowMsgBox(
            GameMsgBoxController.MsgBoxType.DoubleButton,
            base.gameObject,
            confirmText,
            OnCycleLookConfirmed,
            OnCycleLookCancelled
        );
    }

    private void OnCycleLookConfirmed()
    {
        Debug.Log("Confirmed called!");

        int maxIdx = (int)currentPlayer.avatar_state;
        int oldIdx = (int)_selectedVisualState;
        int newIdx = oldIdx + 1;
        if (newIdx > maxIdx) newIdx = 0;

        Debug.Log(string.Format("Changing visual state from {0} to {1}", oldIdx, newIdx));

        _selectedVisualState = (AvatarData.AvatarState)newIdx;

        ApplyVisualLook();

        // Force update UI elements tied to visual state if needed
        avatar_level_tip.ResetTip();  // if this updates UI tips/icons

        Debug.Log("Visual look applied.");

        // Destroy dialog box manually to be sure
        if (msgBox != null)
            Object.Destroy(msgBox.gameObject);
    }

    private void OnCycleLookCancelled()
    {
        Debug.Log("Cancelled called!");
        if (msgBox != null)
            Object.Destroy(msgBox.gameObject);
    }

    private void OnShowPrimaryWeaponEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			UIShopSceneController.Instance.PrimaryWeaponPanel.Show();
		}
	}

	private void OnShowSecondaryWeaponEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			UIShopSceneController.Instance.SecondaryWeaponPanel.Show();
		}
	}

	private void OnShowSkillPanelEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			((UIShopSkillDetailPanelController)UIShopSceneController.Instance.SkillDetalPanel).ResetSkill(GameData.Instance.Skill_Avatar_Set[currentPlayer.skill_list[0]]);
			UIShopSceneController.Instance.SkillDetalPanel.Show();
		}
	}
}
