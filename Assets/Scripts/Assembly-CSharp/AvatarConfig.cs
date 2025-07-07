using System.Collections.Generic;

public class AvatarConfig
{
	public string show_name = string.Empty;

	public string avatar_name;

	public AvatarType avatar_type = AvatarType.None;

	public bool is_voucher_avatar;

	public GameDataInt price;

	public GameDataInt crystal_unlock_price;

	public int up_price_ratio;

	public float up_hp_price_ratio;

	public float up_damage_price_ratio;

	public float up_armor_price_ratio;

    public int max_level;

	public float reload_ratio;

	public float fix_val;

	public float hp_ratio;

	public AvatarExistState exist_state;

	//public string secondary_equipment = "None";

	public string first_skill = "None";

	public int avatar_worth_1;

	public int avatar_worth_2;

	public int unlockDay;

	public UpgradeConfig hp_conf;

	public UpgradeConfig speed_conf;

	public UpgradeConfig damage_conf;

	public UpgradeConfig armor_conf;

	public UpgradeConfig extension_conf;

	public Dictionary<int, string> Second_Weapon_Cfg = new Dictionary<int, string>();
}
