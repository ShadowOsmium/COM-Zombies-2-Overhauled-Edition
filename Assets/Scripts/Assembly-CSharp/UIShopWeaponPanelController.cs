using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class UIShopWeaponPanelController : UIShopPanelController
{
	private WeaponData currentWeapon;

	private WeaponScaleScrollerObj currentWeaponObj;

	private List<WeaponData> weaponList;

	public TUIScrollerEx Scroller;

	public Transform ScrollerObjList;

	public static float ScrollerPageDistance = 100f;

	public static float ScrollerPageDepthDistance = 3f;

	public TUILabel Name;

	public WeaponInfoNotOwned AreaNotOwned;

	public WeaponInfoOwned AreaOwned;

	private int recordPage = -1;

    private GameMsgBoxController msgBox;

	private int upgradePropertyIdx = -1;

	private IEnumerator CreateWeaponScalerObj()
	{
		Scroller.borderXMin = (0f - ScrollerPageDistance) * (float)weaponList.Count;
		Scroller.rangeXMin = Scroller.borderXMin;
		Scroller.pageX = new float[weaponList.Count];
		int index = 0;
		foreach (WeaponData weaponData in weaponList)
		{
			GameObject obj = Object.Instantiate(Resources.Load("Prefab/WeaponScaleScrollerObj")) as GameObject;
			obj.transform.parent = ScrollerObjList;
			obj.transform.localPosition = new Vector3(ScrollerPageDistance * (float)index, Scroller.transform.localPosition.y, ScrollerPageDepthDistance * (float)index);
			obj.GetComponent<WeaponScaleScrollerObj>().Init(weaponData);
			obj.GetComponent<WeaponScaleScrollerObj>().OnAnimateCraftEffectEnd = UpdateWeaponInfo;
			Scroller.pageX[index] = 0f - obj.transform.localPosition.x;
			if (currentWeapon.weapon_name == weaponData.weapon_name)
			{
				Scroller.cur_page = index;
				ScrollerObjList.transform.localPosition = new Vector3(Scroller.pageX[index], 0f, ScrollerObjList.transform.localPosition.z);
				Scroller.position = new Vector2(Scroller.pageX[index], 0f);
			}
			index++;
		}
		yield return 0;
		UpdateScaleScrollerObjectsView();
		yield return 0;
		yield return 0;
	}

	public override void Show()
	{
		base.Show();
		ChangeMask("Mask");
		if (ScrollerObjList.childCount <= 0)
		{
			string primary_equipment = UIShopSceneController.Instance.CurrentAvatar.avatarData.primary_equipment;
			currentWeapon = GameData.Instance.WeaponData_Set[primary_equipment];
			weaponList = GameData.Instance.GetPrimaryWeapons();
			StartCoroutine(CreateWeaponScalerObj());
			UpdateWeaponInfo();
		}
	}

	public override void Hide(bool isPopFromStack)
	{
		base.Hide(isPopFromStack);
		if (isPopFromStack)
		{
			for (int i = 0; i < ScrollerObjList.childCount; i++)
			{
				Object.Destroy(ScrollerObjList.GetChild(i).gameObject);
			}
		}
	}

	private void OnCrystalUnlockOk()
	{
		Object.Destroy(msgBox.gameObject);
		if (currentWeapon.CrystalUnlock())
		{
			UISceneController.Instance.MoneyController.UpdateInfo();
			currentWeaponObj.AnimateCraftEffect();
		}
		else
		{
			//UISceneController.Instance.MoneyController.IapPanel.Show();
		}
		UpdateWeaponInfo();
	}

    private void OnExchangeMoneyOk()
    {
        if (((CrystalMsgBoxController)msgBox).Exchange.Crystal <= GameData.Instance.total_crystal.GetIntVal())
        {
            GameData.Instance.OnExchgCurrcy(
                GameCurrencyType.Crystal,
                GameCurrencyType.Cash,
                ((CrystalMsgBoxController)msgBox).Exchange.Crystal,
                ((CrystalMsgBoxController)msgBox).Exchange.Cash
            );

            if (upgradePropertyIdx == -1)
            {
                currentWeapon.Buy();
                UISceneController.Instance.MoneyController.UpdateInfo();
                currentWeaponObj.AnimateCraftEffect();
            }
            else
            {
                if (upgradePropertyIdx == 3)
                {
                    AreaOwned.BuyAmmo();
                    UISceneController.Instance.MoneyController.UpdateInfo();
                }
                else
                {
                    int extraCash = 0;
                    bool isDayLocked = false;

                    if (AreaOwned.UpgradeProperty(upgradePropertyIdx, ref extraCash, out isDayLocked))
                    {
                        UISceneController.Instance.MoneyController.UpdateInfo();
                        currentWeaponObj.AnimateCraftEffect();
                    }
                    else
                    {
                        if (isDayLocked)
                        {
                            int currentLevel = AreaOwned.GetCurrentLevelForProperty(AreaOwned.Properties[upgradePropertyIdx].Type);
                            int requiredDayLevel = AreaOwned.GetRequiredDayLevelForUpgrade(AreaOwned.Properties[upgradePropertyIdx].Type, currentLevel);
                            string message = AreaOwned.Properties[upgradePropertyIdx].Type.ToString() +
                                " upgrade is locked.\nReach Day " + requiredDayLevel + " to unlock.";
                            GameMsgBoxController.ShowMsgBox(
                                GameMsgBoxController.MsgBoxType.SingleButton,
                                base.gameObject,
                                message,
                                null,
                                null
                            );
                        }
                        else
                        {
                            int crystal = Mathf.CeilToInt((float)extraCash / GameConfig.Instance.crystal_to_cash);
                            string content = "You need " + extraCash + " more cash to complete the action.";
                            CrystalExchangeCash crystalExchangeCash = new CrystalExchangeCash();
                            crystalExchangeCash.CashOrVoucher = true;
                            crystalExchangeCash.Crystal = crystal;
                            crystalExchangeCash.Cash = extraCash;
                            CrystalExchangeCash exchange = crystalExchangeCash;
                            msgBox = CrystalMsgBoxController.ShowMsgBox(
                                exchange,
                                base.gameObject,
                                content,
                                OnExchangeMoneyOk,
                                OnMsgboxCancel
                            );
                        }
                    }
                }
                upgradePropertyIdx = -1;
            }
        }
        else
        {
            // UISceneController.Instance.MoneyController.IapPanel.Show();
        }

        Object.Destroy(msgBox.gameObject);
    }

    private void OnMsgboxCancel()
	{
		Object.Destroy(msgBox.gameObject);
	}

	private void OnScrollEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType != 3)
		{
			UpdateScaleScrollerObjectsView();
			return;
		}
		currentWeapon = weaponList[Scroller.cur_page];
		UpdateScaleScrollerObjectsView();
		UpdateWeaponInfo();
	}

    private void OnWeaponCraftBuyEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 3)
            return;

        int priceVal = currentWeapon.config.price.GetIntVal();
        if (priceVal < 0)
        {
            Debug.LogWarning("AntiCheat: Negative price detected for weapon: " + currentWeapon.weapon_name);
            GameData.Instance.blackname = true;
            GameData.Instance.SaveData();
            return;
        }

        if (((TUIButtonClick)control).m_NormalLabelObj.GetComponent<TUILabel>().Text == "Unlock")
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            string text = "Spend " + currentWeapon.CrystalUnlockPrice.ToString("G") + " tCrystals to own this weapon directly?";
            msgBox = GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.DoubleButton, base.gameObject, text, OnCrystalUnlockOk, OnMsgboxCancel);
            return;
        }

        if (currentWeapon.Buy())
        {
            UISceneController.Instance.MoneyController.UpdateInfo();
            currentWeaponObj.AnimateCraftEffect();
        }
        else if (currentWeapon.config.BuyCurrencyType == GameCurrencyType.Voucher)
        {
            if (GameData.Instance.total_voucher >= currentWeapon.config.price)
            {
                int newVal = GameData.Instance.total_voucher.GetIntVal() - currentWeapon.config.price.GetIntVal();
                if (newVal < 0) newVal = 0;
                GameData.Instance.total_voucher.SetIntVal(newVal, GameDataIntPurpose.Voucher);

                if (currentWeapon.Unlock())
                {
                    UISceneController.Instance.MoneyController.UpdateInfo();
                    currentWeaponObj.AnimateCraftEffect();
                    GameData.Instance.SaveData();
                }
            }
            else
            {
                GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton,
                    base.gameObject,
                    "Not enough vouchers to buy this weapon.",
                    null, null);
            }
        }
        else
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            if (currentWeapon.config.BuyCurrencyType == GameCurrencyType.Cash)
            {
                int intVal = (currentWeapon.config.price - GameData.Instance.total_cash).GetIntVal();
                int crystal = Mathf.CeilToInt((float)intVal / GameConfig.Instance.crystal_to_cash);
                string content = "You need " + intVal + " more cash to complete the action.";
                CrystalExchangeCash crystalExchangeCash = new CrystalExchangeCash();
                crystalExchangeCash.CashOrVoucher = true;
                crystalExchangeCash.Crystal = crystal;
                crystalExchangeCash.Cash = intVal;
                CrystalExchangeCash exchange = crystalExchangeCash;
                msgBox = CrystalMsgBoxController.ShowMsgBox(exchange, base.gameObject, content, OnExchangeMoneyOk, OnMsgboxCancel);
            }
            else
            {
                // UISceneController.Instance.MoneyController.IapPanel.Show();
            }
        }

        UpdateWeaponInfo();
    }

    private void OnEquipWeaponEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType != 3)
		{
			return;
		}
		UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
		UIShopSceneController.Instance.CurrentAvatar.avatarData.primary_equipment = currentWeapon.weapon_name;
		WeaponScaleScrollerObj[] componentsInChildren = ScrollerObjList.GetComponentsInChildren<WeaponScaleScrollerObj>();
		WeaponScaleScrollerObj[] array = componentsInChildren;
		foreach (WeaponScaleScrollerObj weaponScaleScrollerObj in array)
		{
			if (weaponScaleScrollerObj.WeaponData == currentWeapon)
			{
				weaponScaleScrollerObj.UpdateEquipState(true);
			}
			else
			{
				weaponScaleScrollerObj.UpdateEquipState(false);
			}
		}
        GameData.Instance.SaveData();
        UpdateWeaponInfo();
	}

	private void OnBuyAmmoEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
            int priceVal = currentWeapon.config.price.GetIntVal();
            if (priceVal < 0)
            {
                Debug.LogWarning("AntiCheat: Negative price detected for weapon: " + currentWeapon.weapon_name);
                GameData.Instance.blackname = true;
                GameData.Instance.SaveData();
                return;
            }

            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			if (AreaOwned.BuyAmmo())
			{
				UISceneController.Instance.MoneyController.UpdateInfo();
				return;
			}
			int intVal = (currentWeapon.config.bulletShopPrice - GameData.Instance.total_cash).GetIntVal();
			int crystal = Mathf.CeilToInt((float)intVal / GameConfig.Instance.crystal_to_cash);
			string content = "You need " + intVal + " more cash to complete the action.";
			CrystalExchangeCash crystalExchangeCash = new CrystalExchangeCash();
			crystalExchangeCash.CashOrVoucher = true;
			crystalExchangeCash.Crystal = crystal;
			crystalExchangeCash.Cash = intVal;
			CrystalExchangeCash exchange = crystalExchangeCash;
			msgBox = CrystalMsgBoxController.ShowMsgBox(exchange, base.gameObject, content, OnExchangeMoneyOk, OnMsgboxCancel);
			upgradePropertyIdx = 3;
		}
	}

    private void OnUpgradePropertyEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 3) return;

        int priceVal = currentWeapon.config.price.GetIntVal();
        if (priceVal < 0)
        {
            Debug.LogWarning("AntiCheat: Negative price detected for weapon: " + currentWeapon.weapon_name);
            GameData.Instance.blackname = true;
            GameData.Instance.SaveData();
            return;
        }

        UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
        upgradePropertyIdx = int.Parse(control.transform.parent.name.Substring("Prop".Length)) - 1;

        int extraCurrencyNeeded = 0;
        bool isDayLocked = false;

        var property = AreaOwned.Properties[upgradePropertyIdx];
        GameCurrencyType currencyType = currentWeapon.config.BuyCurrencyType;

        if (AreaOwned.UpgradeProperty(upgradePropertyIdx, ref extraCurrencyNeeded, out isDayLocked))
        {
            UISceneController.Instance.MoneyController.UpdateInfo();
            return;
        }

        if (isDayLocked)
        {
            int currentLevel = AreaOwned.GetCurrentLevelForProperty(property.Type);
            int requiredDayLevel = AreaOwned.GetRequiredDayLevelForUpgrade(property.Type, currentLevel);
            string message = property.Type.ToString() + " upgrade is locked.\nReach Day " + requiredDayLevel + " to unlock.";
            msgBox = GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, base.gameObject, message, null, null);
            return;
        }

        if (currencyType == GameCurrencyType.Voucher)
        {
            msgBox = GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton,
                base.gameObject,
                "Not enough vouchers to buy this upgrade.",
                null, null);
            return;
        }
        else // Cash shortage fallback
        {
            int crystal = Mathf.CeilToInt((float)extraCurrencyNeeded / GameConfig.Instance.crystal_to_cash);
            string content = "You need " + extraCurrencyNeeded + " more cash to complete the action.";
            CrystalExchangeCash crystalExchangeCash = new CrystalExchangeCash();
            crystalExchangeCash.CashOrVoucher = true;
            crystalExchangeCash.Crystal = crystal;
            crystalExchangeCash.Cash = extraCurrencyNeeded;
            msgBox = CrystalMsgBoxController.ShowMsgBox(crystalExchangeCash, base.gameObject, content, OnExchangeMoneyOk, OnMsgboxCancel);
        }
    }

    private void UpdateScaleScrollerObjectsView()
	{
		WeaponScaleScrollerObj[] componentsInChildren = ScrollerObjList.GetComponentsInChildren<WeaponScaleScrollerObj>();
		WeaponScaleScrollerObj[] array = componentsInChildren;
		foreach (WeaponScaleScrollerObj weaponScaleScrollerObj in array)
		{
			weaponScaleScrollerObj.UpdateScaleScrollerView();
			if (weaponScaleScrollerObj.WeaponData == currentWeapon)
			{
				currentWeaponObj = weaponScaleScrollerObj;
			}
		}
	}

	private void UpdateWeaponInfo()
	{
		Name.Text = currentWeapon.config.show_name;
		if (currentWeapon.exist_state != WeaponExistState.Owned)
		{
			AreaNotOwned.gameObject.SetActive(true);
			AreaNotOwned.UpdateInfo(currentWeapon);
			AreaOwned.gameObject.SetActive(false);
		}
		else
		{
			AreaNotOwned.gameObject.SetActive(false);
			AreaOwned.gameObject.SetActive(true);
			AreaOwned.UpdateInfo(currentWeapon);
		}
	}
}
