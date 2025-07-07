using System.Collections.Generic;

public class CoopBossCfg
{
	public EnemyType boss_type = EnemyType.E_NONE;

	public string boss_name = string.Empty;

	public string boss_show_name = string.Empty;

	public float hp_capacity;

	public float damage_base;

	public int day_level;

	public int reward_gold_failed;

	public int reward_gold;

	public int reward_crystal;

	public string reward_weapon = string.Empty;

	public List<string> rewards_weapon_fragments = new List<string>();

	public List<string> scene_list = new List<string>();

	public override string ToString()
	{
		return string.Concat("boss_type:", boss_type, " boss_name:", boss_name, " hp_capacity:", hp_capacity, " damage_base:", damage_base, " day_level:", day_level, " reward_crystal:", reward_crystal, " reward_gold:", reward_gold, " reward_gold_failed:", reward_gold_failed);
	}
}
