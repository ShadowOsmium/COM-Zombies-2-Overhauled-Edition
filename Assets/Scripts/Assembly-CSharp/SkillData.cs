using System.Collections;
using UnityEngine;

public class SkillData
{
	public string skill_name = string.Empty;

	public int level = 1;

	public SkillExistState exist_state;

	public float cd_interval = 10f;

	public float life_time = 10f;

	public float hp_capcity;

	public float damage_val;

	public float frequency_val;

	public float range_val;

	public float cd_interval_next = 10f;

	public float life_time_next = 10f;

	public float hp_capcity_next;

	public float damage_val_next;

	public float frequency_val_next;

	public float range_val_next;

	public SkillConfig config;

	public int UpgradePrice
	{
		get
		{
			if (level >= config.max_level)
			{
				return 0;
			}
			float paraA = GameConfig.Instance.Skill_Up_Price_Info.ParaA;
			float paraB = GameConfig.Instance.Skill_Up_Price_Info.ParaB;
			float paraK = GameConfig.Instance.Skill_Up_Price_Info.ParaK;
			float f = level - 1;
			return (int)((paraA * Mathf.Pow(f, paraK) + paraB) * config.up_price_ratio);
		}
	}

    public void ResetData()
    {
        int clampedLevel = Mathf.Clamp(level, 1, config.max_level > 0 ? config.max_level : 1);
        level = clampedLevel;
        int num = clampedLevel - 1;
        cd_interval = config.cd_time_cfg.base_data + (config.cd_time_cfg.max_data - config.cd_time_cfg.base_data) * num / (float)(config.max_level - 1);
        life_time = config.life_time_cfg.base_data + (config.life_time_cfg.max_data - config.life_time_cfg.base_data) * num / (float)(config.max_level - 1);
        hp_capcity = config.hp_cfg.base_data + (config.hp_cfg.max_data - config.hp_cfg.base_data) * num / (float)(config.max_level - 1);
        frequency_val = config.frequency_cfg.base_data + (config.frequency_cfg.max_data - config.frequency_cfg.base_data) * num / (float)(config.max_level - 1);
        range_val = config.range_cfg.base_data + (config.range_cfg.max_data - config.range_cfg.base_data) * num / (float)(config.max_level - 1);
        damage_val = config.damage_cfg.base_data * Mathf.Pow(config.damage_para, num);
        int nextLevel = Mathf.Clamp(clampedLevel + 1, 1, config.max_level);
        int nextNum = nextLevel - 1;
        cd_interval_next = config.cd_time_cfg.base_data + (config.cd_time_cfg.max_data - config.cd_time_cfg.base_data) * nextNum / (float)(config.max_level - 1);
        life_time_next = config.life_time_cfg.base_data + (config.life_time_cfg.max_data - config.life_time_cfg.base_data) * nextNum / (float)(config.max_level - 1);
        hp_capcity_next = config.hp_cfg.base_data + (config.hp_cfg.max_data - config.hp_cfg.base_data) * nextNum / (float)(config.max_level - 1);
        frequency_val_next = config.frequency_cfg.base_data + (config.frequency_cfg.max_data - config.frequency_cfg.base_data) * nextNum / (float)(config.max_level - 1);
        range_val_next = config.range_cfg.base_data + (config.range_cfg.max_data - config.range_cfg.base_data) * nextNum / (float)(config.max_level - 1);
        damage_val_next = config.damage_cfg.base_data * Mathf.Pow(config.damage_para, nextNum);
    }

    public bool Unlock()
	{
		if (exist_state == SkillExistState.Locked)
		{
			exist_state = SkillExistState.Unlocked;
			GameData.Instance.SaveData();
			return true;
		}
		return false;
	}

	public bool Upgrade()
	{
		if (level < config.max_level && GameData.Instance.total_voucher >= UpgradePrice)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("SkillName", config.skill_name);
			hashtable.Add("Level", level + 1);
			hashtable.Add("Price", UpgradePrice);
			GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Skill", hashtable);
            GameData.Instance.total_voucher.SetIntVal(
                GameData.Instance.total_voucher.GetIntVal() - UpgradePrice, GameDataIntPurpose.Voucher);
            level++;
			ResetData();
			GameData.Instance.SaveData();
			return true;
		}
		return false;
	}
}
