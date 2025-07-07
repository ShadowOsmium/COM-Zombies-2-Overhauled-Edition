using CoMZ2;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfoOwned : MonoBehaviour
{
    public TUIButtonClick EquipButton;

    public WeaponProperty[] Properties;

    public WeaponProperty Ammo;

    private WeaponData weaponData;

    private bool showUpgrade;

    public void UpdateInfo(WeaponData data)
    {
        weaponData = data;
        showUpgrade = weaponData.weapon_name == UIShopSceneController.Instance.CurrentAvatar.avatarData.primary_equipment;
        if (showUpgrade)
        {
            EquipButton.gameObject.SetActive(false);
            Ammo.gameObject.SetActive(true);
            UpdateAmmo();
        }
        else
        {
            EquipButton.gameObject.SetActive(true);
            Ammo.gameObject.SetActive(false);
        }
        CheckWeaponProperty();
    }

    private void CheckWeaponProperty()
    {
        WeaponProperty[] properties = Properties;
        foreach (WeaponProperty weaponProperty in properties)
        {
            weaponProperty.Clear();
        }
        int num = 0;
        if (weaponData.damage_level >= weaponData.config.max_level || Mathf.Abs(weaponData.damage_val_next - weaponData.damage_val) > 1E-05f)
        {
            Properties[num].Show(WeaponPropertyType.Damage);
            UpdateProperty(num);
            num++;
        }
        if (weaponData.frequency_level >= weaponData.config.max_level || Mathf.Abs(weaponData.frequency_val_next - weaponData.frequency_val) > 1E-05f)
        {
            Properties[num].Show(WeaponPropertyType.Firerate);
            UpdateProperty(num);
            num++;
        }
        if (weaponData.stretch_level >= weaponData.config.max_level || Mathf.Abs(weaponData.stretch_max_next - weaponData.stretch_max) > 1E-05f)
        {
            Properties[num].Show(WeaponPropertyType.Accuracy);
            UpdateProperty(num);
            num++;
        }
        if (weaponData.clip_level >= weaponData.config.max_level || (float)Mathf.Abs(weaponData.clip_capacity_next - weaponData.clip_capacity) > 1E-05f)
        {
            Properties[num].Show(WeaponPropertyType.Capacity);
            UpdateProperty(num);
            num++;
        }
        if (weaponData.range_level >= weaponData.config.max_level || Mathf.Abs(weaponData.range_val_next - weaponData.range_val) > 1E-05f)
        {
            Properties[num].Show(WeaponPropertyType.Range);
            UpdateProperty(num);
            num++;
        }
    }

    public void UpdateProperty(int propertyIdx)
    {
        WeaponProperty weaponProperty = Properties[propertyIdx];
        bool flag = false;
        string text = string.Empty;
        switch (weaponProperty.Type)
        {
            case WeaponPropertyType.Damage:
                weaponProperty.Name.Text = "Damage (LV " + weaponData.damage_level + ")";
                weaponProperty.Value.Text = weaponData.damage_val.ToString("N2");
                flag = weaponData.damage_level >= weaponData.config.max_level;
                weaponProperty.UpgradeValue.Text = ((!flag) ? ("+" + (weaponData.damage_val_next - weaponData.damage_val).ToString("N2")) : string.Empty);
                text = weaponData.UpgradeDamagePrice.ToString("G");
                break;
            case WeaponPropertyType.Firerate:
                weaponProperty.Name.Text = "Rate of Fire (LV " + weaponData.frequency_level + ")";
                weaponProperty.Value.Text = weaponData.frequency_val.ToString("N3");
                flag = weaponData.frequency_level >= weaponData.config.max_level;
                weaponProperty.UpgradeValue.Text = ((!flag) ? (weaponData.frequency_val_next - weaponData.frequency_val).ToString("N3") : string.Empty);
                text = weaponData.UpgradeFrequencyPrice.ToString("G");
                break;
            case WeaponPropertyType.Accuracy:
                weaponProperty.Name.Text = "Accuracy (LV " + weaponData.stretch_level + ")";
                weaponProperty.Value.Text = (100f - weaponData.stretch_max).ToString("N2") + "%";
                flag = weaponData.stretch_level >= weaponData.config.max_level;
                weaponProperty.UpgradeValue.Text = ((!flag) ? ("+" + (weaponData.stretch_max - weaponData.stretch_max_next).ToString("N2") + "%") : string.Empty);
                text = weaponData.UpgradeStretchPrice.ToString("G");
                break;
            case WeaponPropertyType.Capacity:
                weaponProperty.Name.Text = "Magazine Capacity (LV " + weaponData.clip_level + ")";
                weaponProperty.Value.Text = weaponData.clip_capacity.ToString("N0");
                flag = weaponData.clip_level >= weaponData.config.max_level;
                weaponProperty.UpgradeValue.Text = ((!flag) ? ("+" + (weaponData.clip_capacity_next - weaponData.clip_capacity).ToString("N0")) : string.Empty);
                text = weaponData.UpgradeClipPrice.ToString("G");
                break;
            case WeaponPropertyType.Range:
                weaponProperty.Name.Text = "Range (LV " + weaponData.range_level + ")";
                weaponProperty.Value.Text = weaponData.range_val.ToString("N2");
                flag = weaponData.range_level >= weaponData.config.max_level;
                weaponProperty.UpgradeValue.Text = ((!flag) ? ("+" + (weaponData.range_val_next - weaponData.range_val).ToString("N2")) : string.Empty);
                text = weaponData.UpgradeRangePrice.ToString("G");
                break;
        }
        if (showUpgrade)
        {
            weaponProperty.UpgradeButton.gameObject.SetActive(true);

            if (flag)
            {
                weaponProperty.UpgradeButton.Disable(true);
                weaponProperty.UpgradeButton.m_NormalLabelObj.GetComponent<TUILabel>().Text = "Max";
                weaponProperty.UpgradeButton.m_PressLabelObj.GetComponent<TUILabel>().Text = "Max";
                return;
            }

            weaponProperty.UpgradeButton.Disable(false);
            weaponProperty.UpgradeButton.m_NormalLabelObj.GetComponent<TUILabel>().Text = text;
            weaponProperty.UpgradeButton.m_PressLabelObj.GetComponent<TUILabel>().Text = text;
        }
        else
        {
            weaponProperty.UpgradeButton.gameObject.SetActive(false);
        }
    }

    public int GetCurrentLevelForProperty(WeaponPropertyType type)
    {
        switch (type)
        {
            case WeaponPropertyType.Damage:
                return weaponData.damage_level;
            case WeaponPropertyType.Firerate:
                return weaponData.frequency_level;
            case WeaponPropertyType.Accuracy:
                return weaponData.stretch_level;
            case WeaponPropertyType.Capacity:
                return weaponData.clip_level;
            case WeaponPropertyType.Range:
                return weaponData.range_level;
            default:
                return 0;
        }
    }

    public int GetRequiredDayLevelForUpgrade(WeaponPropertyType type, int currentLevel)
    {
        Dictionary<string, int> customBaseDays = new Dictionary<string, int>
    {
        { "FN2000", 15 },
        { "XM12", 24 },
        { "M32", 35 },
        { "Laser", 40 },
        { "PGM", 40 },
        { "IonCannon", 48 },
        { "IceGun", 60 },
        { "MP5", 0 }
    };

        int baseDay;
        if (!customBaseDays.TryGetValue(weaponData.weapon_name, out baseDay))
        {
            baseDay = weaponData.config.unlockDay;
        }
        int required = baseDay + (currentLevel / 2) * 2;

        Debug.Log("GetRequiredDayLevelForUpgrade: " + weaponData.weapon_name +
                  " unlockDay=" + weaponData.config.unlockDay +
                  " level=" + currentLevel +
                  " required=" + required);

        return required;
    }

    public bool UpgradeProperty(int index, ref int extraCash, out bool isDayLocked)
    {
        isDayLocked = false;
        WeaponPropertyType type = Properties[index].Type;

        int currentLevel = GetCurrentLevelForProperty(type);
        int requiredDayLevel = GetRequiredDayLevelForUpgrade(type, currentLevel);
        int playerDayLevel = GameData.Instance.day_level;

        if (playerDayLevel < requiredDayLevel)
        {
            isDayLocked = true;
            return false;
        }

        bool flag = false;
        switch (type)
        {
            case WeaponPropertyType.Damage:
                flag = weaponData.UpgradeDamage();
                if (!flag)
                    extraCash = weaponData.UpgradeDamagePrice - GameData.Instance.total_cash.GetIntVal();
                break;
            case WeaponPropertyType.Firerate:
                flag = weaponData.UpgradeFrequency();
                if (!flag)
                    extraCash = weaponData.UpgradeFrequencyPrice - GameData.Instance.total_cash.GetIntVal();
                break;
            case WeaponPropertyType.Accuracy:
                flag = weaponData.UpgradeStretch();
                if (!flag)
                    extraCash = weaponData.UpgradeStretchPrice - GameData.Instance.total_cash.GetIntVal();
                break;
            case WeaponPropertyType.Capacity:
                flag = weaponData.UpgradeClip();
                if (!flag)
                    extraCash = weaponData.UpgradeClipPrice - GameData.Instance.total_cash.GetIntVal();
                break;
            case WeaponPropertyType.Range:
                flag = weaponData.UpgradeRange();
                if (!flag)
                    extraCash = weaponData.UpgradeRangePrice - GameData.Instance.total_cash.GetIntVal();
                break;
        }
        if (flag)
        {
            UpdateProperty(index);
            return true;
        }
        return false;
    }

    private WeaponType GetWeaponType()
    {
        if (weaponData != null && weaponData.config != null)
        {

            return weaponData.config.wType;
        }
        return WeaponType.AssaultRifle;
    }

    private void UpdateAmmo()
    {
        Ammo.Value.Text = weaponData.total_bullet_count.ToString("G");
        if (weaponData.total_bullet_count + weaponData.config.buy_bullet_count <= 19999)
        {
            Ammo.UpgradeValue.Text = "+" + weaponData.config.buy_bullet_count.ToString("G");
            Ammo.UpgradeButton.Disable(false);
            Ammo.UpgradeButton.m_NormalLabelObj.GetComponent<TUILabel>().Text = weaponData.config.bulletShopPrice.ToString();
            Ammo.UpgradeButton.m_PressLabelObj.GetComponent<TUILabel>().Text = weaponData.config.bulletShopPrice.ToString();
        }
        else
        {
            Ammo.UpgradeValue.Text = string.Empty;
            Ammo.UpgradeButton.Disable(true);
        }
    }

    public bool BuyAmmo()
    {
        if (weaponData.BuyBulletShop())
        {
            UpdateAmmo();
            return true;
        }
        return false;
    }
}