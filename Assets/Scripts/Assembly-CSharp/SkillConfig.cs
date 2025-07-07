using System.Collections;

public class SkillConfig
{
	public string show_name = string.Empty;

	public string skill_name = string.Empty;

	public AvatarType owner_avatar = AvatarType.None;

	public EnemyType owner_enmey = EnemyType.E_NONE;

	public int max_level = 5;

	public int unlock_level = 2;

	public int unlock_price = 100;

	public float up_price_ratio = 1f;

	public float damage_para;

	public string skill_content = string.Empty;

	public SkillExistState exist_state;

	public UpgradeConfig cd_time_cfg;

	public UpgradeConfig life_time_cfg;

	public UpgradeConfig hp_cfg;

	public UpgradeConfig frequency_cfg;

	public UpgradeConfig damage_cfg;

	public UpgradeConfig range_cfg;

	public Hashtable Ex_conf;
}
