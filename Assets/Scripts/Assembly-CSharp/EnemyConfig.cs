using System.Collections;

public class EnemyConfig
{
	public string enemy_name;

	public EnemyType enemy_type = EnemyType.E_NONE;

	public float view_range;

	public float attack_range;

	public float speed_val;

	public float missionWeight;

	public float attack_frequency;

	public float hp_ratio;

	public float damage_ratio;

	public float reward_ratio;

	public EnemyAttackPriority attack_priority;

	public string load_content;

	public Hashtable Ex_conf;
}
