using System.Collections;
using CoMZ2;

public class WeaponConfig
{
	public string weapon_name;

	public WeaponType wType;

	public string show_name;

    public int unlockDay;

    public float moveSpeedDrag;

	public GameDataInt price;

	public GameDataInt bulletShopPrice;

	public GameDataInt bulletBattletPrice;

	public GameDataInt crystal_unlock_price;

	public float battle_buttle_count_ratio = 1f;

	public int initBullet;

	public int buy_bullet_count;

	public float afterFireTime;

	public float stretchRangeOffset;

	public float stretchRangeRunOffset;

	public bool is_auto_lock;

	public float recoil;

	public bool is_infinity_ammo;

	public float combo_base;

	public float damage_ratio;

	public float up_price_ratio;

	public float up_damage_price_ratio = 1f;

	public float up_frequency_price_ratio = 1f;

	public float up_clip_price_ratio = 1f;

	public float up_stretch_price_ratio = 1f;

	public float up_range_price_ratio = 1f;

	public int max_level;

	public WeaponExistState exist_state;

	public bool is_secondary;

	public AvatarType owner = AvatarType.None;

	public UpgradeConfig frequency_conf;

	public UpgradeConfig stretch_max;

	public UpgradeConfig clip_conf;

	public UpgradeConfig range_conf;

	public string comment = string.Empty;

	public int combination_count;

    public GameDataInt sell_price;

	public Hashtable Ex_conf;

	public GameCurrencyType BuyCurrencyType;

	public GameCurrencyType UpgradeCurrencyType;

	public GameCurrencyType BulletShopCurrencyType;

	public GameCurrencyType BulletBattleCurrencyType;
}
