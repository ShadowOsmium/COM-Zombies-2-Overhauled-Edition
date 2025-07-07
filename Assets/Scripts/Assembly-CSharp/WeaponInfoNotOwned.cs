using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class WeaponInfoNotOwned : MonoBehaviour
{
	public TUIButtonClick Button;

	public TUIMeshSprite GreenPointAD;

	public Transform[] Debris;

	public WeaponProperty[] Properties;

	public TUIMeshSprite PriceCrystal;

	public TUIMeshSprite PriceCash;

    public TUIMeshSprite PriceVoucher;

    public TUILabel PriceAmount;

	public TUILabel unlock_day_label;

	public void UpdateInfo(WeaponData data)
	{
		CheckWeaponProperty(data);
		if (data.config.BuyCurrencyType == GameCurrencyType.Crystal)
		{
			GreenPointAD.gameObject.SetActive(true);
			unlock_day_label.gameObject.SetActive(false);
			for (int i = 0; i < Debris.Length; i++)
			{
				Debris[i].gameObject.SetActive(false);
			}
		}
		else if (data.config.combination_count > 0)
		{
			List<WeaponFragmentList> weaponFragmentProbsState = GameData.Instance.GetWeaponFragmentProbsState(data.weapon_name);
			if (weaponFragmentProbsState.Count > 0)
			{
				GreenPointAD.gameObject.SetActive(false);
				unlock_day_label.gameObject.SetActive(false);
				for (int j = 0; j < weaponFragmentProbsState.Count; j++)
				{
					Debris[j].gameObject.SetActive(true);
					if (weaponFragmentProbsState[j].count > 0)
					{
						Debris[j].Find("bk").GetComponent<TUIMeshSprite>().GrayStyle = false;
						Debris[j].Find("bk2").GetComponent<TUIMeshSprite>().GrayStyle = false;
						Debris[j].Find("image").GetComponent<TUIMeshSprite>().GrayStyle = false;
						Debris[j].Find("image").GetComponent<TUIMeshSprite>().texture = weaponFragmentProbsState[j].image_name;
					}
					else
					{
						Debris[j].Find("bk").GetComponent<TUIMeshSprite>().GrayStyle = true;
						Debris[j].Find("bk2").GetComponent<TUIMeshSprite>().GrayStyle = true;
						Debris[j].Find("image").GetComponent<TUIMeshSprite>().GrayStyle = true;
						Debris[j].Find("image").GetComponent<TUIMeshSprite>().texture = weaponFragmentProbsState[j].image_name;
					}
				}
			}
		}
		else
		{
			if (data.exist_state == WeaponExistState.Locked)
			{
				GreenPointAD.gameObject.SetActive(false);
				unlock_day_label.gameObject.SetActive(true);
				unlock_day_label.Text = "COMPLETE DAY " + data.config.unlockDay + " TO UNLOCK";
			}
			else if (data.exist_state == WeaponExistState.Unlocked)
			{
				GreenPointAD.gameObject.SetActive(true);
				unlock_day_label.gameObject.SetActive(false);
			}
			for (int k = 0; k < Debris.Length; k++)
			{
				Debris[k].gameObject.SetActive(false);
			}
		}
		if (data.exist_state == WeaponExistState.Locked)
		{
			Button.m_NormalLabelObj.GetComponent<TUILabel>().Text = "Unlock";
			Button.m_PressLabelObj.GetComponent<TUILabel>().Text = "Unlock";
			PriceCrystal.gameObject.SetActive(true);
			PriceCash.gameObject.SetActive(false);
			PriceAmount.Text = data.CrystalUnlockPrice.ToString("G");

		}
		else if (data.exist_state == WeaponExistState.Unlocked)
		{
			if (data.config.BuyCurrencyType == GameCurrencyType.Cash)
			{
				Button.m_NormalLabelObj.GetComponent<TUILabel>().Text = "Craft";
				Button.m_PressLabelObj.GetComponent<TUILabel>().Text = "Craft";
				PriceCrystal.gameObject.SetActive(false);
				PriceCash.gameObject.SetActive(true);
				PriceAmount.Text = data.config.price.ToString();
			}
			else if (data.config.BuyCurrencyType == GameCurrencyType.Crystal)
			{
				Button.m_NormalLabelObj.GetComponent<TUILabel>().Text = "Buy";
				Button.m_PressLabelObj.GetComponent<TUILabel>().Text = "Buy";
				PriceCrystal.gameObject.SetActive(true);
				PriceCash.gameObject.SetActive(false);
				PriceAmount.gameObject.SetActive(true);
				PriceAmount.Text = data.config.price.ToString();
			}
            else if (data.config.BuyCurrencyType == GameCurrencyType.Voucher)
            {
                Button.m_NormalLabelObj.GetComponent<TUILabel>().Text = "Buy";
                Button.m_PressLabelObj.GetComponent<TUILabel>().Text = "Buy";

                PriceCrystal.gameObject.SetActive(false);
                PriceCash.gameObject.SetActive(false);
                PriceVoucher.gameObject.SetActive(true);
                PriceAmount.gameObject.SetActive(true);
                PriceAmount.Text = data.config.price.ToString();
            }
        }
	}

	private void CheckWeaponProperty(WeaponData data)
	{
		WeaponProperty[] properties = Properties;
		foreach (WeaponProperty weaponProperty in properties)
		{
			weaponProperty.Clear();
		}
		int num = 0;
		if (Mathf.Abs(data.damage_val_next - data.damage_val) > 1E-05f)
		{
			Properties[num].Show(WeaponPropertyType.Damage);
			Properties[num].Name.Text = "Damage";
			Properties[num].Value.Text = data.damage_val.ToString("N2");
			num++;
		}
		if (Mathf.Abs(data.frequency_val_next - data.frequency_val) > 1E-05f)
		{
			Properties[num].Show(WeaponPropertyType.Firerate);
			Properties[num].Name.Text = "Rate of Fire";
			Properties[num].Value.Text = data.frequency_val.ToString("N3");
			num++;
		}
		if (Mathf.Abs(data.stretch_max_next - data.stretch_max) > 1E-05f)
		{
			Properties[num].Show(WeaponPropertyType.Accuracy);
			Properties[num].Name.Text = "Accuracy";
			Properties[num].Value.Text = (100f - data.stretch_max).ToString("N2") + "%";
			num++;
		}
		if (num < Properties.Length)
		{
			if ((float)Mathf.Abs(data.clip_capacity_next - data.clip_capacity) > 1E-05f)
			{
				Properties[num].Show(WeaponPropertyType.Capacity);
				Properties[num].Name.Text = "Magazine Capacity";
				Properties[num].Value.Text = data.clip_capacity.ToString("N0");
				num++;
			}
			if (num < Properties.Length && Mathf.Abs(data.range_val_next - data.range_val) > 1E-05f)
			{
				Properties[num].Show(WeaponPropertyType.Range);
				Properties[num].Name.Text = "Range";
				Properties[num].Value.Text = data.range_val.ToString("N2");
				num++;
			}
		}
	}
}
