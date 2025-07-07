using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class UIShopSubWeaponPanelController : UIShopPanelController
{
	private WeaponData currentWeapon;

	private WeaponScaleScrollerObj currentWeaponObj;

	private List<WeaponData> weaponList;

	public TUIScrollerEx Scroller;

	public Transform ScrollerObjList;

	public TUILabel Name;

	public TUILabel Description;

	public WeaponProperty[] WeaponInfoObjs;

	public GameObject Upgrade;

	public TUILabel UpgradePrice;

	public GameObject UpgradeMax;

	private GameMsgBoxController msgBox;

	private IEnumerator CreateWeaponScalerObj()
	{
		Scroller.borderXMin = (0f - UIShopWeaponPanelController.ScrollerPageDistance) * (float)weaponList.Count;
		Scroller.rangeXMin = Scroller.borderXMin;
		Scroller.pageX = new float[weaponList.Count];
		int index = 0;
		foreach (WeaponData weaponData in weaponList)
		{
			GameObject obj = Object.Instantiate(Resources.Load("Prefab/WeaponScaleScrollerObj")) as GameObject;
			obj.transform.parent = ScrollerObjList;
			obj.transform.localPosition = new Vector3(UIShopWeaponPanelController.ScrollerPageDistance * (float)index, Scroller.transform.localPosition.y, UIShopWeaponPanelController.ScrollerPageDepthDistance * (float)index);
			obj.GetComponent<WeaponScaleScrollerObj>().Init(weaponData);
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
	}

    public override void Show()
    {
        base.Show();
        ChangeMask("Mask");
        if (ScrollerObjList.childCount <= 0)
        {
            string primary_equipment = UIShopSceneController.Instance.CurrentAvatar.avatarData.primary_equipment;
            currentWeapon = GameData.Instance.WeaponData_Set[primary_equipment];
            weaponList = GameData.Instance.GetSecondaryWeaponsFor(currentWeapon.owner);
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
			WeaponProperty[] weaponInfoObjs = WeaponInfoObjs;
			foreach (WeaponProperty weaponProperty in weaponInfoObjs)
			{
				weaponProperty.gameObject.SetActive(false);
			}
		}
	}

	private void OnExchangeMoneyOk()
	{
		if (((CrystalMsgBoxController)msgBox).Exchange.Crystal <= GameData.Instance.total_crystal.GetIntVal())
		{
			GameData.Instance.OnExchgCurrcy(GameCurrencyType.Crystal, GameCurrencyType.Voucher, ((CrystalMsgBoxController)msgBox).Exchange.Crystal, ((CrystalMsgBoxController)msgBox).Exchange.Voucher);
			currentWeapon.Upgrade();
			UpdateWeaponInfo();
			currentWeaponObj.LevelUp();
			UISceneController.Instance.MoneyController.UpdateInfo();
		}
		else
		{
			//UISceneController.Instance.MoneyController.IapPanel.Show();
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

	private void OnUpgradeEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			if (currentWeapon.Upgrade())
			{
				UpdateWeaponInfo();
				currentWeaponObj.LevelUp();
				UISceneController.Instance.MoneyController.UpdateInfo();
				return;
			}
			int num = currentWeapon.UpgradePrice - GameData.Instance.total_voucher.GetIntVal();
			int crystal = Mathf.CeilToInt((float)num / GameConfig.Instance.crystal_to_voucher);
			string content = "You need " + num + " more vouchers to complete the action.";
			CrystalExchangeCash crystalExchangeCash = new CrystalExchangeCash();
			crystalExchangeCash.Crystal = crystal;
			crystalExchangeCash.Voucher = num;
			CrystalExchangeCash exchange = crystalExchangeCash;
			msgBox = CrystalMsgBoxController.ShowMsgBox(exchange, base.gameObject, content, OnExchangeMoneyOk, OnMsgboxCancel);
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
		Name.Text = currentWeapon.weapon_name;
		Description.Text = currentWeapon.config.comment;
		switch (currentWeapon.weapon_type)
		{
		case WeaponType.Pistol:
			UpdateProperty(0, true, WeaponPropertyType.Damage);
			UpdateProperty(1, true, WeaponPropertyType.Firerate);
			UpdateProperty(2, true, WeaponPropertyType.Accuracy);
			UpdateProperty(3, true, WeaponPropertyType.Ammo);
			break;
		case WeaponType.Saw:
		case WeaponType.Baseball:
			UpdateProperty(0, true, WeaponPropertyType.Damage);
			UpdateProperty(1, true, WeaponPropertyType.Firerate);
			UpdateProperty(2, true, WeaponPropertyType.Durability);
			UpdateProperty(3, false, WeaponPropertyType.None);
			break;
		case WeaponType.Medicine:
			UpdateProperty(0, true, WeaponPropertyType.Ammo);
			UpdateProperty(1, true, WeaponPropertyType.Damage);
			UpdateProperty(2, false, WeaponPropertyType.None);
			UpdateProperty(3, false, WeaponPropertyType.None);
			break;
		case WeaponType.Shield:
			UpdateProperty(0, true, WeaponPropertyType.Durability);
			UpdateProperty(1, false, WeaponPropertyType.None);
			UpdateProperty(2, false, WeaponPropertyType.None);
			UpdateProperty(3, false, WeaponPropertyType.None);
			break;
		}
		if (currentWeapon.level < currentWeapon.config.max_level)
		{
			Upgrade.SetActive(true);
			UpgradePrice.Text = currentWeapon.UpgradePrice.ToString("G");
			UpgradeMax.SetActive(false);
		}
		else
		{
			Upgrade.SetActive(false);
			UpgradeMax.SetActive(true);
		}
	}

	private void UpdateProperty(int propertyIdx, bool isShow, WeaponPropertyType type)
	{
		WeaponInfoObjs[propertyIdx].gameObject.SetActive(isShow);
		if (!isShow)
		{
			return;
		}
		switch (type)
		{
		case WeaponPropertyType.Damage:
			if (currentWeapon.weapon_type == WeaponType.Medicine)
			{
				WeaponInfoObjs[propertyIdx].Name.Text = "Treatment";
				WeaponInfoObjs[propertyIdx].Value.Text = currentWeapon.damage_val.ToString("N0");
				float f = currentWeapon.damage_val_next - currentWeapon.damage_val;
				WeaponInfoObjs[propertyIdx].UpgradeValue.Text = ((!(Mathf.Abs(f) > 1E-05f)) ? string.Empty : ("+" + f.ToString("N0")));
			}
			else
			{
				WeaponInfoObjs[propertyIdx].Name.Text = "Damage";
				WeaponInfoObjs[propertyIdx].Value.Text = currentWeapon.damage_val.ToString("N2");
				float f = currentWeapon.damage_val_next - currentWeapon.damage_val;
				WeaponInfoObjs[propertyIdx].UpgradeValue.Text = ((!(Mathf.Abs(f) > 1E-05f)) ? string.Empty : ("+" + f.ToString("N2")));
			}
			break;
		case WeaponPropertyType.Firerate:
		{
			WeaponInfoObjs[propertyIdx].Name.Text = "Rate of Fire";
			WeaponInfoObjs[propertyIdx].Value.Text = currentWeapon.frequency_val.ToString("N3");
			float f = currentWeapon.frequency_val_next - currentWeapon.frequency_val;
			WeaponInfoObjs[propertyIdx].UpgradeValue.Text = ((!(Mathf.Abs(f) > 1E-05f)) ? string.Empty : f.ToString("N3"));
			break;
		}
		case WeaponPropertyType.Ammo:
			if (currentWeapon.weapon_type == WeaponType.Medicine)
			{
				WeaponInfoObjs[propertyIdx].Name.Text = "Amount";
				WeaponInfoObjs[propertyIdx].Value.Text = currentWeapon.clip_capacity.ToString("N0");
				WeaponInfoObjs[propertyIdx].UpgradeValue.Text = ((currentWeapon.clip_capacity_next - currentWeapon.clip_capacity < 1) ? string.Empty : ("+" + currentWeapon.config.buy_bullet_count.ToString("N0")));
			}
			else
			{
				WeaponInfoObjs[propertyIdx].Name.Text = "Ammo";
				WeaponInfoObjs[propertyIdx].Value.Text = currentWeapon.total_bullet_count.ToString("N0");
				WeaponInfoObjs[propertyIdx].UpgradeValue.Text = "+" + currentWeapon.config.buy_bullet_count.ToString("N0");
			}
			break;
		case WeaponPropertyType.Accuracy:
		{
			WeaponInfoObjs[propertyIdx].Name.Text = "Accuracy";
			WeaponInfoObjs[propertyIdx].Value.Text = (100f - currentWeapon.stretch_max).ToString("N2") + "%";
			float f = currentWeapon.stretch_max_next - currentWeapon.stretch_max;
			WeaponInfoObjs[propertyIdx].UpgradeValue.Text = ((!(Mathf.Abs(f) > 1E-05f)) ? string.Empty : ("+" + (0f - f).ToString("N2")));
			break;
		}
		case WeaponPropertyType.Durability:
			WeaponInfoObjs[propertyIdx].Name.Text = "Durability";
			if (currentWeapon.weapon_type == WeaponType.Shield)
			{
				WeaponInfoObjs[propertyIdx].Value.Text = currentWeapon.clip_capacity.ToString("N0");
				float f = currentWeapon.clip_capacity_next - currentWeapon.clip_capacity;
				WeaponInfoObjs[propertyIdx].UpgradeValue.Text = ((!(Mathf.Abs(f) > 1E-05f)) ? string.Empty : ("+" + f.ToString("N0")));
			}
			else
			{
				WeaponInfoObjs[propertyIdx].Value.Text = "N/A";
				WeaponInfoObjs[propertyIdx].UpgradeValue.Text = string.Empty;
			}
			break;
		case WeaponPropertyType.Capacity:
		case WeaponPropertyType.Range:
			break;
		}
	}
}
