using CoMZ2;
using UnityEngine;

public class UIShopAvatarPanelController : UIShopPanelController
{
	private AvatarData currentPlayer;

	public TUILabel Name;

	public WeaponProperty[] Properties;

	public TUIMeshSprite PrimaryWeapon;

	public TUIMeshSprite SecondaryWeapon;

	public GameObject BuyAvatar;

	public GameObject UnlockAvatar;

    private AvatarType? lastSavedAvatarType = null;

    private GameMsgBoxController msgBox;

	private int upgradePropertyIdx = -1;

	private GameObject newLabel;

	public AvatarLevelTipController avatar_level_tip;

    private void UpdateProperty(int index)
    {
        bool flag = currentPlayer.exist_state == AvatarExistState.Owned;
        bool flag2 = false;
        string text = string.Empty;

        switch (index)
        {
            case 0:
                Properties[index].Name.Text = "Health (LV " + currentPlayer.hp_level + " Out Of " + currentPlayer.config.max_level + ")";
                Properties[index].Value.Text = currentPlayer.hp_capacity.ToString("N0");
                flag2 = currentPlayer.hp_level >= currentPlayer.config.max_level;
                Properties[index].UpgradeValue.Text = (!flag2) ? ("+" + (currentPlayer.hp_capacity_next - currentPlayer.hp_capacity).ToString("N0")) : string.Empty;
                text = currentPlayer.UpgradeHpPrice.ToString("G");
                break;

            case 1: // Damage
                {
                    Properties[index].Name.Text = "Damage (LV " + currentPlayer.damage_level + " Out Of " + currentPlayer.config.max_level + ")";

                    int curLevel = currentPlayer.damage_level;
                    int maxLevel = Mathf.Max(2, currentPlayer.config.max_level);

                    float normalizedLevel = (float)(curLevel - 1) / (maxLevel - 1);

                    float curvedFraction = Mathf.Pow(normalizedLevel, 1.5f);

                    float currentDamageVal = Mathf.Lerp(
                        currentPlayer.config.damage_conf.base_data,
                        currentPlayer.config.damage_conf.max_data,
                        curvedFraction
                    );

                    Properties[index].Value.Text = (1f + currentDamageVal / 100f).ToString("F2") + "x";

                    flag2 = curLevel >= maxLevel;

                    if (!flag2)
                    {
                        float nextNormalizedLevel = (float)(curLevel) / (maxLevel - 1);
                        float nextCurvedFraction = Mathf.Pow(nextNormalizedLevel, 1.5f);

                        float nextDamageVal = Mathf.Lerp(
                            currentPlayer.config.damage_conf.base_data,
                            currentPlayer.config.damage_conf.max_data,
                            nextCurvedFraction
                        );

                        float percentUpgrade = nextDamageVal - currentDamageVal;

                        if (percentUpgrade > 0.01f)
                            Properties[index].UpgradeValue.Text = "+" + percentUpgrade.ToString("F2") + "% Damage";
                        else
                            Properties[index].UpgradeValue.Text = "+<0% Damage";
                    }
                    else
                    {
                        Properties[index].UpgradeValue.Text = string.Empty;
                    }

                    text = currentPlayer.UpgradeDamagePrice.ToString("G");
                    break;
                }
            case 2: // Armor
                Properties[index].Name.Text = "Armor (LV " + currentPlayer.armor_level + " Out Of " + currentPlayer.config.max_level + ")";

                float baseArmor = currentPlayer.config.armor_conf.base_data;
                float currentArmor = currentPlayer.armor_val;

                float reduction = GetArmorDamageReduction(currentArmor); // e.g. 0.39 = 39%
                float percent = reduction * 100f;

                Debug.Log(string.Format("Armor Debug: baseArmor={0}, currentArmor={1}, armor_level={2}, damageReduction={3:F2}%",
                    baseArmor, currentArmor, currentPlayer.armor_level, percent));

                Properties[index].Value.Text = "-" + percent.ToString("F2") + "% dmg";

                flag2 = currentPlayer.armor_level >= currentPlayer.config.max_level;

                if (!flag2)
                {
                    float nextArmor = currentPlayer.GetScaledStat(
                        currentPlayer.config.armor_conf.base_data,
                        currentPlayer.config.armor_conf.max_data,
                        currentPlayer.armor_level + 1,
                        currentPlayer.config.max_level
                    );

                    float nextReduction = GetArmorDamageReduction(nextArmor);
                    float upgrade = nextReduction - reduction;

                    if (upgrade >= 0.0001f)
                        Properties[index].UpgradeValue.Text = "+" + (upgrade * 100f).ToString("F2") + "% resist";
                    else
                        Properties[index].UpgradeValue.Text = "+<0.1% resist";
                }
                else
                {
                    Properties[index].UpgradeValue.Text = string.Empty;
                }

                text = currentPlayer.UpgradeArmorPrice.ToString("G");
                break;
        }

        if (flag)
        {
            Properties[index].UpgradeButton.gameObject.SetActive(true);
            if (flag2)
            {
                Properties[index].UpgradeButton.Disable(true);
                return;
            }
            Properties[index].UpgradeButton.Disable(false);
            Properties[index].UpgradeButton.gameObject.SetActive(true);
            Properties[index].UpgradeButton.m_NormalLabelObj.GetComponent<TUILabel>().Text = text;
            Properties[index].UpgradeButton.m_PressLabelObj.GetComponent<TUILabel>().Text = text;
        }
        else
        {
            Properties[index].UpgradeButton.gameObject.SetActive(false);
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

        // Only save if avatar changed since last save
        if (lastSavedAvatarType == null || lastSavedAvatarType != currentPlayer.avatar_type)
        {
            lastSavedAvatarType = currentPlayer.avatar_type;
            GameData.Instance.SaveData();
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
        Object.Destroy(msgBox.gameObject);
    }

    private void OnMsgboxCancel()
	{
		Object.Destroy(msgBox.gameObject);
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
		if (eventType != 3)
		{
			return;
		}
		UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
		upgradePropertyIdx = int.Parse(control.transform.parent.name.Substring("Prop".Length)) - 1;
		bool flag = false;
		int num = 0;
		if (upgradePropertyIdx == 0)
		{
			flag = currentPlayer.UpgradeHp();
			UIShopSceneController.Instance.CheckAvatarPosInShop();
			avatar_level_tip.ResetTip();
			if (!flag)
			{
				num = currentPlayer.UpgradeHpPrice - GameData.Instance.total_voucher.GetIntVal();
			}
		}
		else if (upgradePropertyIdx == 1)
		{
			flag = currentPlayer.UpgradeDamage();
			UIShopSceneController.Instance.CheckAvatarPosInShop();
			avatar_level_tip.ResetTip();
			if (!flag)
			{
				num = currentPlayer.UpgradeDamagePrice - GameData.Instance.total_voucher.GetIntVal();
			}
		}
		else
		{
			flag = currentPlayer.UpgradeArmor();
			UIShopSceneController.Instance.CheckAvatarPosInShop();
			avatar_level_tip.ResetTip();
			if (!flag)
			{
				num = currentPlayer.UpgradeArmorPrice - GameData.Instance.total_voucher.GetIntVal();
			}
		}
		if (flag)
		{
			UISceneController.Instance.MoneyController.UpdateInfo();
			UpdateProperty(upgradePropertyIdx);
			return;
		}
		int crystal = Mathf.CeilToInt((float)num / GameConfig.Instance.crystal_to_voucher);
		string content = "You need " + num + " more vouchers to buy this.";
		CrystalExchangeCash crystalExchangeCash = new CrystalExchangeCash();
		crystalExchangeCash.Crystal = crystal;
		crystalExchangeCash.Voucher = num;
		CrystalExchangeCash exchange = crystalExchangeCash;
		msgBox = CrystalMsgBoxController.ShowMsgBox(exchange, base.gameObject, content, OnExchangeMoneyOk, OnMsgboxCancel);
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
