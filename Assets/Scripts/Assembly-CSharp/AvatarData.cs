using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarData
{
	public enum AvatarState
	{
		Normal = 1,
		Strong,
		Super
	}

	public string show_name = string.Empty;

	public string avatar_name = string.Empty;

	public AvatarType avatar_type = AvatarType.None;

	public string primary_equipment = "None";

	public float move_speed;

	public float hp_capacity;

	public float cur_hp;

	public float damage_val;

	public float armor_val;

	public float hp_capacity_next;

	public float damage_val_next;

	public float armor_val_next;

	public float reload_speed_val;

	public int level = 1;

	public GameDataInt cur_exp = new GameDataInt(0);

	public GameDataInt next_level_exp = new GameDataInt(0);

	public AvatarExistState exist_state;

	public AvatarConfig config;

	public int hp_level = 1;

	public int damage_level = 1;

	public int armor_level = 1;

	public List<string> skill_list = new List<string>();

	public int avatar_worth;

	public AvatarState avatar_state = AvatarState.Normal;

	public int UpgradePrice
	{
		get
		{
			if (level >= config.max_level)
			{
				return 0;
			}
			float paraA = GameConfig.Instance.Avatar_Up_Price_Info.ParaA;
			float paraB = GameConfig.Instance.Avatar_Up_Price_Info.ParaB;
			float paraK = GameConfig.Instance.Avatar_Up_Price_Info.ParaK;
			float f = level - 1;
			return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * (float)config.up_price_ratio);
		}
	}

	public int UpgradeArmorPrice
	{
		get
		{
			return GetUpgradeArmorPrice(armor_level);
		}
	}

	public int UpgradeHpPrice
	{
		get
		{
			return GetUpgradeHpPrice(hp_level);
		}
	}

	public int UpgradeDamagePrice
	{
		get
		{
			return GetUpgradeDamagePrice(damage_level);
		}
	}

    public void ResetData()
    {
        level = Mathf.Clamp(level, 1, config.max_level);
        damage_level = Mathf.Clamp(damage_level, 1, config.max_level);
        armor_level = Mathf.Clamp(armor_level, 1, config.max_level);
        hp_level = Mathf.Clamp(hp_level, 1, config.max_level);

        int safeDenominator = (config.max_level - 1) > 0 ? (config.max_level - 1) : 1;

        float moveNum = level - 1;
        move_speed = config.speed_conf.base_data + moveNum * (config.speed_conf.max_data - config.speed_conf.base_data) / safeDenominator;
        move_speed = Mathf.Clamp(move_speed, config.speed_conf.base_data, config.speed_conf.max_data);

        damage_val = GetScaledStat(config.damage_conf.base_data, config.damage_conf.max_data, damage_level, config.max_level);
        armor_val = GetScaledStat(config.armor_conf.base_data, config.armor_conf.max_data, armor_level, config.max_level);

        float paraA = GameConfig.Instance.Avatar_Hp_Up_Info.ParaA;
        float paraB = GameConfig.Instance.Avatar_Hp_Up_Info.ParaB;
        float paraC = GameConfig.Instance.Avatar_Hp_Up_Info.ParaC;
        float calculatedHp = config.hp_conf.base_data + (paraA * Mathf.Pow(hp_level, 2f) + paraB * hp_level + paraC) * config.hp_ratio;
        hp_capacity = calculatedHp;
        cur_hp = hp_capacity;

        reload_speed_val = config.reload_ratio;

        if (hp_level < config.max_level)
        {
            int nextLevel = hp_level + 1;
            hp_capacity_next = config.hp_conf.base_data + (paraA * Mathf.Pow(nextLevel, 2f) + paraB * nextLevel + paraC) * config.hp_ratio;
        }
        else
        {
            hp_capacity_next = hp_capacity;
        }

        if (damage_level < config.max_level)
        {
            damage_val_next = GetScaledStat(config.damage_conf.base_data, config.damage_conf.max_data, damage_level + 1, config.max_level);
        }
        else
        {
            damage_val_next = damage_val;
        }

        if (armor_level < config.max_level)
        {
            armor_val_next = GetScaledStat(config.armor_conf.base_data, config.armor_conf.max_data, armor_level + 1, config.max_level);
        }
        else
        {
            armor_val_next = armor_val;
        }

        // Avatar worth calculation
        avatar_worth = 0;
        for (int i = 1; i <= hp_level - 1; i++)
        {
            avatar_worth += GetUpgradeHpPrice(i);
        }
        for (int j = 1; j <= damage_level - 1; j++)
        {
            avatar_worth += GetUpgradeDamagePrice(j);
        }
        for (int k = 1; k <= armor_level - 1; k++)
        {
            avatar_worth += GetUpgradeArmorPrice(k);
        }

        // Avatar state
        if (avatar_worth >= config.avatar_worth_2)
        {
            avatar_state = AvatarState.Super;
        }
        else if (avatar_worth >= config.avatar_worth_1)
        {
            avatar_state = AvatarState.Strong;
        }
        else
        {
            avatar_state = AvatarState.Normal;
        }

        cur_hp = hp_capacity;
    }

    public bool Upgrade()
	{
		if (level < config.max_level && GameData.Instance.total_voucher >= UpgradePrice)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("AvartarName", config.avatar_name);
			hashtable.Add("AvartarLevel", level + 1);
			hashtable.Add("AvartarPrice", UpgradePrice);
			GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Avartar", hashtable);
            GameData.Instance.total_voucher.SetIntVal(
                GameData.Instance.total_voucher.GetIntVal() - UpgradePrice, GameDataIntPurpose.Voucher);
            level++;
			hp_level++;
			damage_level++;
			ResetData();
			CheckUnlockSecondaryWeapon();
			return true;
		}
		return false;
	}

    float GetScaledStat(float baseVal, float maxVal, int level, int maxLevel)
    {
        if (level <= 1) return baseVal;

        float t = (float)(level - 1) / (maxLevel - 1);
        t = Mathf.Pow(t, 2f);
        return baseVal + (maxVal - baseVal) * t;
    }


    public bool UpgradeArmor()
    {
        int price = UpgradeArmorPrice;

        if (armor_level < config.max_level && GameData.Instance.total_voucher.GetIntVal() >= price)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("AvartarName", config.avatar_name);
            hashtable.Add("Level", armor_level + 1);
            hashtable.Add("Price", price);
            GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Avartar_Armor", hashtable);

            GameData.Instance.total_voucher.SetIntVal(GameData.Instance.total_voucher.GetIntVal() - price, GameDataIntPurpose.Voucher);
            armor_level++;
            ResetData();
            GameData.Instance.SaveData();
            return true;
        }
        return false;
    }

    public bool UpgradeHp()
    {
        int price = UpgradeHpPrice;

        if (hp_level < config.max_level && GameData.Instance.total_voucher.GetIntVal() >= price)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("AvartarName", config.avatar_name);
            hashtable.Add("Level", hp_level + 1);
            hashtable.Add("Price", price);
            GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Avartar_Hp", hashtable);

            GameData.Instance.total_voucher.SetIntVal(GameData.Instance.total_voucher.GetIntVal() - price, GameDataIntPurpose.Voucher);
            hp_level++;
            ResetData();
            GameData.Instance.SaveData();
            return true;
        }
        return false;
    }

    public bool UpgradeDamage()
    {
        int price = UpgradeDamagePrice;

        if (damage_level < config.max_level && GameData.Instance.total_voucher.GetIntVal() >= price)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("AvartarName", config.avatar_name);
            hashtable.Add("Level", damage_level + 1);
            hashtable.Add("Price", price);
            GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Avartar_Damage", hashtable);

            GameData.Instance.total_voucher.SetIntVal(GameData.Instance.total_voucher.GetIntVal() - price, GameDataIntPurpose.Voucher);
            damage_level++;
            ResetData();
            GameData.Instance.SaveData();
            return true;
        }
        return false;
    }

    public bool AddExp(int exp)
    {
        if (level >= config.max_level)
        {
            return false;
        }

        int newExp = cur_exp.GetIntVal() + exp;
        cur_exp.SetIntVal(newExp);

        if (cur_exp.GetIntVal() >= next_level_exp.GetIntVal())
        {
            int leftoverExp = cur_exp.GetIntVal() - next_level_exp.GetIntVal();
            cur_exp.SetIntVal(leftoverExp);
            Upgrade();
            return true;
        }

        return false;
    }

    public bool EquipWeapon(WeaponData weapon)
	{
		if (weapon.exist_state != WeaponExistState.Owned)
		{
			return false;
		}
		if (!weapon.is_secondary)
		{
			primary_equipment = weapon.weapon_name;
			return true;
		}
		if (weapon.owner == avatar_type)
		{
			return true;
		}
		return false;
	}

	public void Injured(float damage)
	{
		float num = damage * (1f - armor_val / 100f);
		if (num < 0f)
		{
			num = 0f;
		}
		cur_hp -= num;
	}

	public void CheckUnlockSecondaryWeapon()
	{
		foreach (int key in config.Second_Weapon_Cfg.Keys)
		{
			if (key != level)
			{
			}
		}
	}

	public bool Unlock()
	{
		if (exist_state == AvatarExistState.Locked)
		{
			exist_state = AvatarExistState.Unlocked;
			return true;
		}
		return false;
	}

    public bool Buy()
    {
        if (exist_state != AvatarExistState.Unlocked)
            return false;

        int priceVal = config.price.GetIntVal();

        if (priceVal < 0)
        {
            Debug.LogWarning("Attempted to buy avatar with negative price: " + priceVal);
            GameData.Instance.blackname = true;
            GameData.Instance.SaveData();
            return false;
        }

        if (config.is_voucher_avatar)
        {
            if (GameData.Instance.total_voucher.GetIntVal() >= priceVal)
            {
                GameData.Instance.total_voucher.SetIntVal(GameData.Instance.total_voucher.GetIntVal() - priceVal, GameDataIntPurpose.Voucher);

                Hashtable hashtable = new Hashtable();
                hashtable.Add("AvatarName", config.avatar_name);
                hashtable.Add("AvatarPrice", priceVal);
                GameData.Instance.UploadStatistics("Vouchers_Use_Buy_Avartar", hashtable);

                exist_state = AvatarExistState.Owned;
                GameData.Instance.SaveData();
                return true;
            }
        }
        else
        {
            if (GameData.Instance.total_cash.GetIntVal() >= priceVal)
            {
                GameData.Instance.total_cash.SetIntVal(GameData.Instance.total_cash.GetIntVal() - priceVal, GameDataIntPurpose.Cash);

                exist_state = AvatarExistState.Owned;
                GameData.Instance.SaveData();
                return true;
            }
        }

        return false;
    }

    public float GetVatarHpWithLv(int lv)
	{
		int num = lv - 1;
		return config.hp_conf.base_data + (float)num * (config.hp_conf.max_data - config.hp_conf.base_data) / (float)(config.max_level - 1);
	}

    public bool CrystalUnlock()
    {
        if (exist_state == AvatarExistState.Locked)
        {
            int crystalPrice = config.crystal_unlock_price.GetIntVal();

            if (crystalPrice > 0 && GameData.Instance.total_crystal.GetIntVal() >= crystalPrice)
            {
                GameData.Instance.total_crystal.SetIntVal(GameData.Instance.total_crystal.GetIntVal() - crystalPrice, GameDataIntPurpose.Crystal);

                exist_state = AvatarExistState.Owned;

                Hashtable hashtable = new Hashtable();
                hashtable.Add("AvartarName", config.avatar_name);
                hashtable.Add("AvartarPrice", crystalPrice);
                GameData.Instance.UploadStatistics("tCrystal_Use_Buy_Avartar", hashtable);

                GameData.Instance.SaveData();
                return true;
            }
            return false;
        }
        return false;
    }

    private int GetUpgradeHpPrice(int level)
	{
		if (level >= config.max_level)
		{
			return 0;
		}
		float paraA = GameConfig.Instance.Avatar_Up_Hp_Price_Info.ParaA;
		float paraB = GameConfig.Instance.Avatar_Up_Hp_Price_Info.ParaB;
		float paraK = GameConfig.Instance.Avatar_Up_Hp_Price_Info.ParaK;
		float f = level - 1;
		return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * config.up_hp_price_ratio);
	}

	private int GetUpgradeArmorPrice(int level)
	{
		if (level >= config.max_level)
		{
			return 0;
		}
		float paraA = GameConfig.Instance.Avatar_Up_Armor_Price_Info.ParaA;
		float paraB = GameConfig.Instance.Avatar_Up_Armor_Price_Info.ParaB;
		float paraK = GameConfig.Instance.Avatar_Up_Armor_Price_Info.ParaK;
		float f = level - 1;
		return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * config.up_armor_price_ratio);
	}

	private int GetUpgradeDamagePrice(int level)
	{
		if (level >= config.max_level)
		{
			return 0;
		}
		float paraA = GameConfig.Instance.Avatar_Up_Damage_Price_Info.ParaA;
		float paraB = GameConfig.Instance.Avatar_Up_Damage_Price_Info.ParaB;
		float paraK = GameConfig.Instance.Avatar_Up_Damage_Price_Info.ParaK;
		float f = level - 1;
		return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * config.up_damage_price_ratio);
	}
}
