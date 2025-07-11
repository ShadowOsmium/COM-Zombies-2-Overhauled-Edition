using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class WeaponData
{
    public string weapon_name = string.Empty;

    public Dictionary<string, WeaponData> playerWeapons = new Dictionary<string, WeaponData>();

    public WeaponType weapon_type;

    public float damage_val;

    public float frequency_val;

    public int level;

    public float stretch_max;

    public int clip_capacity;

    public float range_val;

    public int clip_bullet_count;

    public int total_bullet_count;

    public float damage_val_next;

    public float frequency_val_next;

    public float stretch_max_next;

    public int clip_capacity_next;

    public float range_val_next;

    public WeaponExistState exist_state;

    public bool is_secondary;

    public AvatarType owner = AvatarType.None;

    public WeaponConfig config;

    public int intensifier_drop_weight;

    public int damage_level = 1;

    public int frequency_level = 1;

    public int clip_level = 1;

    public int range_level = 1;

    public int stretch_level = 1;

    private static WeaponData instance;

    public static WeaponData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WeaponData();
            }
            return instance;
        }
    }

    public int UpgradePrice
    {
        get
        {
            if (level >= config.max_level)
                return 0;

            if (weapon_type == WeaponType.Shield || weapon_type == WeaponType.Medicine)
            {
                int idx = level;
                float basePrice = (float)config.Ex_conf["priceBase"];
                float maxPrice = (float)config.Ex_conf["priceMax"];
                return (int)(basePrice + idx * (maxPrice - basePrice) / (config.max_level - 1));
            }

            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Voucher_Info[level + 1] * config.up_price_ratio);

            if (config.UpgradeCurrencyType == GameCurrencyType.Cash)
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Info[level + 1] * config.up_price_ratio);

            Debug.LogWarning("UpgradeCurrencyType:" + config.UpgradeCurrencyType);
            return 0;
        }
    }

    public int UpgradeDamagePrice
    {
        get
        {
            if (damage_level >= config.max_level)
            {
                return 0;
            }
            if (weapon_type == WeaponType.Medicine)
            {
                int num = damage_level + 1 - 1;
                float num2 = (float)config.Ex_conf["priceBase"];
                float num3 = (float)config.Ex_conf["priceMax"];
                return (int)(num2 + (float)num * (num3 - num2) / (float)(config.max_level - 1));
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Voucher_Info[damage_level + 1] * config.up_price_ratio * config.up_damage_price_ratio);
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Cash)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Info[damage_level + 1] * config.up_price_ratio * config.up_damage_price_ratio);
            }
            Debug.LogWarning("UpgradeCurrencyType:" + config.UpgradeCurrencyType);
            return 0;
        }
    }

    public bool IsFullyOwned()
    {
        return this.exist_state == WeaponExistState.Owned;
    }

    public int UpgradeFrequencyPrice
    {
        get
        {
            if (frequency_level >= config.max_level)
            {
                return 0;
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Voucher_Info[frequency_level + 1] * config.up_price_ratio * config.up_frequency_price_ratio);
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Cash)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Info[frequency_level + 1] * config.up_price_ratio * config.up_frequency_price_ratio);
            }
            Debug.LogWarning("UpgradeCurrencyType:" + config.UpgradeCurrencyType);
            return 9999;
        }
    }

    public int UpgradeClipPrice
    {
        get
        {
            if (clip_level >= config.max_level)
            {
                return 0;
            }
            if (weapon_type == WeaponType.Shield)
            {
                int num = clip_level + 1 - 1;
                float num2 = (float)config.Ex_conf["priceBase"];
                float num3 = (float)config.Ex_conf["priceMax"];
                return (int)(num2 + (float)num * (num3 - num2) / (float)(config.max_level - 1));
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Voucher_Info[clip_level + 1] * config.up_price_ratio * config.up_clip_price_ratio);
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Cash)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Info[clip_level + 1] * config.up_price_ratio * config.up_clip_price_ratio);
            }
            Debug.LogWarning("UpgradeCurrencyType:" + config.UpgradeCurrencyType);
            return 0;
        }
    }

    public int UpgradeRangePrice
    {
        get
        {
            if (range_level >= config.max_level)
            {
                return 0;
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Voucher_Info[range_level + 1] * config.up_price_ratio * config.up_range_price_ratio);
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Cash)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Info[range_level + 1] * config.up_price_ratio * config.up_range_price_ratio);
            }
            Debug.LogWarning("UpgradeCurrencyType:" + config.UpgradeCurrencyType);
            return 0;
        }
    }

    public int UpgradeStretchPrice
    {
        get
        {
            if (stretch_level >= config.max_level)
            {
                return 0;
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Voucher_Info[stretch_level + 1] * config.up_price_ratio * config.up_stretch_price_ratio);
            }
            if (config.UpgradeCurrencyType == GameCurrencyType.Cash)
            {
                return (int)(GameConfig.Instance.Standard_Weapon_Price_Info[stretch_level + 1] * config.up_price_ratio * config.up_stretch_price_ratio);
            }
            Debug.LogWarning("UpgradeCurrencyType:" + config.UpgradeCurrencyType);
            return 0;
        }
    }

    public int CrystalUnlockPrice
    {
        get
        {
            if (exist_state == WeaponExistState.Locked && config.BuyCurrencyType != GameCurrencyType.Crystal)
            {
                List<GameProb> weaponFragmentProbs = GameData.Instance.GetWeaponFragmentProbs(weapon_name);
                int count = weaponFragmentProbs.Count;
                if (count < config.combination_count)
                {
                    float num = (float)(config.combination_count - count) / (float)config.combination_count;
                    return (int)((float)config.crystal_unlock_price.GetIntVal() * num);
                }
                return config.crystal_unlock_price.GetIntVal();
            }
            return 0;
        }
    }

    public void ResetData()
    {
        level = Mathf.Clamp(level, 1, config.max_level);
        damage_level = Mathf.Clamp(damage_level, 1, config.max_level);
        frequency_level = Mathf.Clamp(frequency_level, 1, config.max_level);
        clip_level = Mathf.Clamp(clip_level, 1, config.max_level);
        range_level = Mathf.Clamp(range_level, 1, config.max_level);
        stretch_level = Mathf.Clamp(stretch_level, 1, config.max_level);
        int num = stretch_level - 1;
        stretch_max = config.stretch_max.base_data + (float)num * (config.stretch_max.max_data - config.stretch_max.base_data) / (float)(config.max_level - 1);
        num = frequency_level - 1;
        frequency_val = config.frequency_conf.base_data + (float)num * (config.frequency_conf.max_data - config.frequency_conf.base_data) / (float)(config.max_level - 1);
        num = clip_level - 1;
        clip_capacity = (int)(config.clip_conf.base_data + (float)num * (config.clip_conf.max_data - config.clip_conf.base_data) / (float)(config.max_level - 1));
        num = range_level - 1;
        range_val = config.range_conf.base_data + (float)num * (config.range_conf.max_data - config.range_conf.base_data) / (float)(config.max_level - 1);
        num = damage_level - 1;
        if (weapon_type == WeaponType.Medicine)
        {
            float num2 = (float)config.Ex_conf["base"];
            float num3 = (float)config.Ex_conf["max"];
            damage_val = num2 + (float)num * (num3 - num2) / (float)(config.max_level - 1);
        }
        else
        {
            damage_val = GameConfig.Instance.Standard_Weapon_Dps_Info[damage_level] * GetFrequencyWithLevel(damage_level) * config.damage_ratio;
        }
        if (damage_level < config.max_level)
        {
            num = damage_level - 1 + 1;
            if (weapon_type == WeaponType.Medicine)
            {
                float num4 = (float)config.Ex_conf["base"];
                float num5 = (float)config.Ex_conf["max"];
                damage_val_next = num4 + (float)num * (num5 - num4) / (float)(config.max_level - 1);
            }
            else
            {
                damage_val_next = GameConfig.Instance.Standard_Weapon_Dps_Info[damage_level + 1] * GetFrequencyWithLevel(damage_level + 1) * config.damage_ratio;
            }
        }
        else
        {
            damage_val_next = damage_val;
        }
        if (frequency_level < config.max_level)
        {
            num = frequency_level - 1 + 1;
            frequency_val_next = config.frequency_conf.base_data + (float)num * (config.frequency_conf.max_data - config.frequency_conf.base_data) / (float)(config.max_level - 1);
        }
        else
        {
            frequency_val_next = frequency_val;
        }
        if (stretch_level < config.max_level)
        {
            num = stretch_level - 1 + 1;
            stretch_max_next = config.stretch_max.base_data + (float)num * (config.stretch_max.max_data - config.stretch_max.base_data) / (float)(config.max_level - 1);
        }
        else
        {
            stretch_max_next = stretch_max;
        }
        if (clip_level < config.max_level)
        {
            num = clip_level - 1 + 1;
            clip_capacity_next = (int)(config.clip_conf.base_data + (float)num * (config.clip_conf.max_data - config.clip_conf.base_data) / (float)(config.max_level - 1));
        }
        else
        {
            clip_capacity_next = clip_capacity;
        }
        if (range_level < config.max_level)
        {
            num = range_level - 1 + 1;
            range_val_next = (int)(config.range_conf.base_data + (float)num * (config.range_conf.max_data - config.range_conf.base_data) / (float)(config.max_level - 1));
        }
        else
        {
            range_val_next = range_val;
        }
        if (is_secondary)
        {
            total_bullet_count = clip_capacity;
        }
    }

    public bool Upgrade()
    {
        level = Mathf.Clamp(level, 1, config.max_level);
        damage_level = Mathf.Clamp(damage_level, 1, config.max_level);
        frequency_level = Mathf.Clamp(frequency_level, 1, config.max_level);
        clip_level = Mathf.Clamp(clip_level, 1, config.max_level);
        range_level = Mathf.Clamp(range_level, 1, config.max_level);
        stretch_level = Mathf.Clamp(stretch_level, 1, config.max_level);

        if (level < config.max_level)
        {
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= UpgradePrice)
                {
                    GameData.Instance.total_voucher.SetIntVal(
                        GameData.Instance.total_voucher.GetIntVal() - UpgradePrice,
                        GameDataIntPurpose.Voucher
                    );

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("WeaponLevel", level + 1);
                    hashtable.Add("WeaponPrice", UpgradePrice);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Weapon", hashtable);

                    level++;
                    damage_level++;
                    frequency_level++;
                    clip_level++;
                    range_level++;
                    stretch_level++;

                    ResetData();
                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.UpgradeCurrencyType == GameCurrencyType.Cash && GameData.Instance.total_cash.GetIntVal() >= UpgradePrice)
            {
                GameData.Instance.total_cash.SetIntVal(
                    GameData.Instance.total_cash.GetIntVal() - UpgradePrice,
                    GameDataIntPurpose.Cash
                );

                Hashtable hashtable2 = new Hashtable();
                hashtable2.Add("WeaponName", config.weapon_name);
                hashtable2.Add("WeaponLevel", level + 1);
                hashtable2.Add("WeaponPrice", UpgradePrice);
                GameData.Instance.UploadStatistics("Gold_Use_Upgrade_Weapon", hashtable2);

                level++;
                damage_level++;
                frequency_level++;
                clip_level++;
                range_level++;
                stretch_level++;

                ResetData();
                GameData.Instance.SaveData();
                return true;
            }
        }
        return false;
    }

    public bool Buy()
    {
        if (exist_state == WeaponExistState.Unlocked)
        {
            if (config.BuyCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= config.price)
                {
                        GameData.Instance.total_voucher.SetIntVal(
                        GameData.Instance.total_voucher.GetIntVal() - config.price.GetIntVal(),
                            GameDataIntPurpose.Voucher
                    );

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("WeaponPrice", config.price);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Buy_Weapon", hashtable);

                    exist_state = WeaponExistState.Owned;
                    GameData.Instance.SaveData();
                        return true;
                }
            }
            else if (config.BuyCurrencyType == GameCurrencyType.Cash)
            {
                if (GameData.Instance.total_cash.GetIntVal() >= config.price)
                {
                    GameData.Instance.total_cash.SetIntVal(
                        GameData.Instance.total_cash.GetIntVal() - config.price.GetIntVal(),
                        GameDataIntPurpose.Cash
                    );

                    Hashtable hashtable2 = new Hashtable();
                    hashtable2.Add("WeaponName", config.weapon_name);
                    hashtable2.Add("WeaponPrice", config.price);
                    GameData.Instance.UploadStatistics("Gold_Use_Buy_Weapon", hashtable2);

                    exist_state = WeaponExistState.Owned;
                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.BuyCurrencyType == GameCurrencyType.Crystal)
            {
                if (GameData.Instance.total_crystal.GetIntVal() >= config.price)
                {
                    GameData.Instance.total_crystal.SetIntVal(
                        GameData.Instance.total_crystal.GetIntVal() - config.price.GetIntVal(),
                        GameDataIntPurpose.Crystal
                    );

                    Hashtable hashtable3 = new Hashtable();
                    hashtable3.Add("WeaponName", config.weapon_name);
                    hashtable3.Add("WeaponPrice", config.price);
                    GameData.Instance.UploadStatistics("tCrystal_Use_Buy_Weapon", hashtable3);

                    exist_state = WeaponExistState.Owned;
                    GameData.Instance.SaveData();
                    return true;
                }
            }
        }
        return false;
    }

    public bool BuyBulletShop()
    {
        if (exist_state == WeaponExistState.Owned)
        {
            if (total_bullet_count + config.buy_bullet_count > 19999)
                return false;

            if (config.BulletShopCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= config.bulletShopPrice)
                {
                    GameData.Instance.total_voucher.SetIntVal(
                        GameData.Instance.total_voucher.GetIntVal() - config.bulletShopPrice.GetIntVal(),
                        GameDataIntPurpose.Voucher
                    );

                    total_bullet_count += config.buy_bullet_count;

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("AmmoPrice", config.bulletShopPrice);
                    hashtable.Add("AmmoNumber", config.buy_bullet_count);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Buy_Ammo", hashtable);

                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.BulletShopCurrencyType == GameCurrencyType.Cash)
            {
                if (GameData.Instance.total_cash.GetIntVal() >= config.bulletShopPrice)
                {
                    GameData.Instance.total_cash.SetIntVal(
                        GameData.Instance.total_cash.GetIntVal() - config.bulletShopPrice.GetIntVal(),
                        GameDataIntPurpose.Cash
                    );

                    total_bullet_count += config.buy_bullet_count;

                    Hashtable hashtable2 = new Hashtable();
                    hashtable2.Add("WeaponName", config.weapon_name);
                    hashtable2.Add("AmmoPrice", config.bulletShopPrice);
                    hashtable2.Add("AmmoNumber", config.buy_bullet_count);
                    GameData.Instance.UploadStatistics("Gold_Use_Buy_Ammo", hashtable2);

                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.BulletShopCurrencyType == GameCurrencyType.Crystal)
            {
                if (GameData.Instance.total_crystal.GetIntVal() >= config.bulletShopPrice)
                {
                    GameData.Instance.total_crystal.SetIntVal(
                        GameData.Instance.total_crystal.GetIntVal() - config.bulletShopPrice.GetIntVal(),
                        GameDataIntPurpose.Crystal
                    );

                    total_bullet_count += config.buy_bullet_count;

                    Hashtable hashtable3 = new Hashtable();
                    hashtable3.Add("WeaponName", config.weapon_name);
                    hashtable3.Add("AmmoPrice", config.bulletShopPrice);
                    hashtable3.Add("AmmoNumber", config.buy_bullet_count);
                    GameData.Instance.UploadStatistics("tCrystal_Use_Buy_Ammo", hashtable3);

                    GameData.Instance.SaveData();
                    return true;
                }
            }
        }
        return false;
    }

    public bool BuyBulletBattle()
    {
        if (exist_state == WeaponExistState.Owned)
        {
            int ammoToAdd = (int)(config.buy_bullet_count * config.battle_buttle_count_ratio);

            if (config.BulletBattleCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= config.bulletBattletPrice)
                {
                    GameData.Instance.total_voucher.SetIntVal(
                        GameData.Instance.total_voucher.GetIntVal() - config.bulletBattletPrice.GetIntVal(),
                        GameDataIntPurpose.Voucher
                    );

                    total_bullet_count += ammoToAdd;

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("AmmoPrice", config.bulletBattletPrice);
                    hashtable.Add("AmmoNumber", ammoToAdd);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Buy_Ammo", hashtable);

                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.BulletBattleCurrencyType == GameCurrencyType.Cash)
            {
                if (GameData.Instance.total_cash.GetIntVal() >= config.bulletBattletPrice)
                {
                    GameData.Instance.total_cash.SetIntVal(
                        GameData.Instance.total_cash.GetIntVal() - config.bulletBattletPrice.GetIntVal(),
                        GameDataIntPurpose.Cash
                    );

                    total_bullet_count += ammoToAdd;

                    Hashtable hashtable2 = new Hashtable();
                    hashtable2.Add("WeaponName", config.weapon_name);
                    hashtable2.Add("AmmoPrice", config.bulletBattletPrice);
                    hashtable2.Add("AmmoNumber", ammoToAdd);
                    GameData.Instance.UploadStatistics("Gold_Use_Buy_Ammo", hashtable2);

                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.BulletBattleCurrencyType == GameCurrencyType.Crystal)
            {
                if (GameData.Instance.total_crystal.GetIntVal() >= config.bulletBattletPrice)
                {
                    GameData.Instance.total_crystal.SetIntVal(
                        GameData.Instance.total_crystal.GetIntVal() - config.bulletBattletPrice.GetIntVal(),
                        GameDataIntPurpose.Crystal
                    );

                    total_bullet_count += ammoToAdd;

                    Hashtable hashtable3 = new Hashtable();
                    hashtable3.Add("WeaponName", config.weapon_name);
                    hashtable3.Add("AmmoPrice", config.bulletBattletPrice);
                    hashtable3.Add("AmmoNumber", ammoToAdd);
                    GameData.Instance.UploadStatistics("tCrystal_Use_Buy_Ammo", hashtable3);

                    GameData.Instance.SaveData();
                    return true;
                }
            }
        }
        return false;
    }

    public void AddBullet(int count)
    {
        if (exist_state == WeaponExistState.Owned)
        {
            int newTotal = total_bullet_count + count;

            int maxAllowed = 19999;
            int tolerance = 800;

            if (newTotal > maxAllowed + tolerance)
            {
                Debug.LogWarning("Suspicious ammo detected: " + newTotal);
                newTotal = maxAllowed;

                if (GameData.Instance != null)
                {
                    GameData.Instance.blackname = true;
                    GameData.Instance.SaveData();
                }
            }
            else if (newTotal > maxAllowed)
            {
                newTotal = maxAllowed;
            }

            total_bullet_count = newTotal;
        }
    }

    public bool LotteryReward(bool is_lottery)
    {
        if (exist_state != WeaponExistState.Owned)
        {
            exist_state = WeaponExistState.Owned;
            if (is_lottery)
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("WeaponName", config.weapon_name);
                GameData.Instance.UploadStatistics("Lottery_Weapon", hashtable);
            }
            else
            {
                Hashtable hashtable2 = new Hashtable();
                hashtable2.Add("WeaponName", config.weapon_name);
                GameData.Instance.UploadStatistics("Coop_Reward_Weapon", hashtable2);
            }
            return true;
        }
        return false;
    }

    public bool Unlock()
    {
        if (exist_state == WeaponExistState.Locked)
        {
            exist_state = WeaponExistState.Unlocked;
            return true;
        }
        return false;
    }

    public bool Reload()
    {
        if (clip_bullet_count >= clip_capacity)
        {
            return false;
        }
        if (clip_bullet_count <= clip_capacity && clip_bullet_count == total_bullet_count)
        {
            return false;
        }
        if (total_bullet_count > 0)
        {
            if (total_bullet_count > clip_capacity)
            {
                clip_bullet_count = clip_capacity;
            }
            else
            {
                clip_bullet_count = total_bullet_count;
            }
            return true;
        }
        return false;
    }

    public bool OnFire()
    {
        if (config.is_infinity_ammo)
        {
            return true;
        }
        if (clip_bullet_count > 0)
        {
            clip_bullet_count--;
            total_bullet_count--;
            GameSceneController.Instance.player_controller.UpdateWeaponUIShow();
            return true;
        }
        return false;
    }

    public bool EnableFire()
    {
        if (clip_bullet_count > 0)
        {
            return true;
        }
        return false;
    }

    public int GetRemainCount()
    {
        return total_bullet_count - clip_bullet_count;
    }

    public bool NeedReload()
    {
        return clip_bullet_count <= 0;
    }

    public bool EnableReload()
    {
        if (clip_bullet_count >= clip_capacity)
        {
            return false;
        }
        if (clip_bullet_count <= clip_capacity && clip_bullet_count == total_bullet_count)
        {
            return false;
        }
        if (total_bullet_count > 0)
        {
            return true;
        }
        return false;
    }

    public bool EnableBuyButtletInBattlet()
    {
        if (exist_state == WeaponExistState.Owned)
        {
            if (config.BulletBattleCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher >= config.bulletBattletPrice)
                {
                    return true;
                }
            }
            else if (config.BulletBattleCurrencyType == GameCurrencyType.Cash)
            {
                if (GameData.Instance.total_cash >= config.bulletShopPrice)
                {
                    return true;
                }
            }
            else if (config.BulletBattleCurrencyType == GameCurrencyType.Crystal && GameData.Instance.total_crystal >= config.bulletShopPrice)
            {
                return true;
            }
        }
        return false;
    }

    public bool UpgradeDamage()
    {
        if (damage_level < config.max_level)
        {
            int upgradeDamagePrice = UpgradeDamagePrice;
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= upgradeDamagePrice)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("WeaponLevel", damage_level + 1);
                    hashtable.Add("WeaponPrice", upgradeDamagePrice);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Weapon_Damage", hashtable);

                    GameData.Instance.total_voucher.SetIntVal(
                        GameData.Instance.total_voucher.GetIntVal() - upgradeDamagePrice,
                        GameDataIntPurpose.Voucher
                    );

                    damage_level++;
                    ResetData();
                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.UpgradeCurrencyType == GameCurrencyType.Cash &&
                     GameData.Instance.total_cash.GetIntVal() >= upgradeDamagePrice)
            {
                Hashtable hashtable2 = new Hashtable();
                hashtable2.Add("WeaponName", config.weapon_name);
                hashtable2.Add("WeaponLevel", damage_level + 1);
                hashtable2.Add("WeaponPrice", upgradeDamagePrice);
                GameData.Instance.UploadStatistics("Gold_Use_Upgrade_Weapon_Damage", hashtable2);

                GameData.Instance.total_cash.SetIntVal(
                    GameData.Instance.total_cash.GetIntVal() - upgradeDamagePrice,
                    GameDataIntPurpose.Cash
                );

                damage_level++;
                ResetData();
                GameData.Instance.SaveData();
                return true;
            }
        }
        return false;
    }

    public bool UpgradeFrequency()
    {
        if (frequency_level < config.max_level)
        {
            int upgradeFrequencyPrice = UpgradeFrequencyPrice;
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= upgradeFrequencyPrice)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("WeaponLevel", frequency_level + 1);
                    hashtable.Add("WeaponPrice", upgradeFrequencyPrice);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Weapon_Frequency", hashtable);

                    int newVoucherValue = GameData.Instance.total_voucher.GetIntVal() - upgradeFrequencyPrice;
                    GameData.Instance.total_voucher.SetIntVal(newVoucherValue, GameDataIntPurpose.Voucher);

                    frequency_level++;
                    ResetData();
                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.UpgradeCurrencyType == GameCurrencyType.Cash)
            {
                if (GameData.Instance.total_cash.GetIntVal() >= upgradeFrequencyPrice)
                {
                    Hashtable hashtable2 = new Hashtable();
                    hashtable2.Add("WeaponName", config.weapon_name);
                    hashtable2.Add("WeaponLevel", frequency_level + 1);
                    hashtable2.Add("WeaponPrice", upgradeFrequencyPrice);
                    GameData.Instance.UploadStatistics("Gold_Use_Upgrade_Weapon_Frequency", hashtable2);

                    int newCashValue = GameData.Instance.total_cash.GetIntVal() - upgradeFrequencyPrice;
                    GameData.Instance.total_cash.SetIntVal(newCashValue, GameDataIntPurpose.Cash);

                    frequency_level++;
                    ResetData();
                    GameData.Instance.SaveData();
                    return true;
                }
            }
        }
        return false;
    }

    public bool UpgradeClip()
    {
        if (clip_level < config.max_level)
        {
            int upgradeClipPrice = UpgradeClipPrice;
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= upgradeClipPrice)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("WeaponLevel", clip_level + 1);
                    hashtable.Add("WeaponPrice", upgradeClipPrice);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Weapon_Clip", hashtable);

                    GameData.Instance.total_voucher.SetIntVal(
                        GameData.Instance.total_voucher.GetIntVal() - upgradeClipPrice,
                        GameDataIntPurpose.Voucher
                    );

                    clip_level++;
                    ResetData();
                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.UpgradeCurrencyType == GameCurrencyType.Cash &&
                     GameData.Instance.total_cash.GetIntVal() >= upgradeClipPrice)
            {
                Hashtable hashtable2 = new Hashtable();
                hashtable2.Add("WeaponName", config.weapon_name);
                hashtable2.Add("WeaponLevel", clip_level + 1);
                hashtable2.Add("WeaponPrice", upgradeClipPrice);
                GameData.Instance.UploadStatistics("Gold_Use_Upgrade_Weapon_Clip", hashtable2);

                GameData.Instance.total_cash.SetIntVal(
                    GameData.Instance.total_cash.GetIntVal() - upgradeClipPrice,
                    GameDataIntPurpose.Cash
                );

                clip_level++;
                ResetData();
                GameData.Instance.SaveData();
                return true;
            }
        }
        return false;
    }

    public bool UpgradeStretch()
    {
        if (stretch_level < config.max_level)
        {
            int upgradeStretchPrice = UpgradeStretchPrice;
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= upgradeStretchPrice)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("WeaponLevel", stretch_level + 1);
                    hashtable.Add("WeaponPrice", upgradeStretchPrice);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Weapon_Stretch", hashtable);

                    GameData.Instance.total_voucher.SetIntVal(
                        GameData.Instance.total_voucher.GetIntVal() - upgradeStretchPrice,
                        GameDataIntPurpose.Voucher
                    );

                    stretch_level++;
                    ResetData();
                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.UpgradeCurrencyType == GameCurrencyType.Cash &&
                     GameData.Instance.total_cash.GetIntVal() >= upgradeStretchPrice)
            {
                Hashtable hashtable2 = new Hashtable();
                hashtable2.Add("WeaponName", config.weapon_name);
                hashtable2.Add("WeaponLevel", stretch_level + 1);
                hashtable2.Add("WeaponPrice", upgradeStretchPrice);
                GameData.Instance.UploadStatistics("Gold_Use_Upgrade_Weapon_Stretch", hashtable2);

                GameData.Instance.total_cash.SetIntVal(
                    GameData.Instance.total_cash.GetIntVal() - upgradeStretchPrice,
                    GameDataIntPurpose.Cash
                );

                stretch_level++;
                ResetData();
                GameData.Instance.SaveData();
                return true;
            }
        }
        return false;
    }

    public bool UpgradeRange()
    {
        if (range_level < config.max_level)
        {
            int upgradeRangePrice = UpgradeRangePrice;
            if (config.UpgradeCurrencyType == GameCurrencyType.Voucher)
            {
                if (GameData.Instance.total_voucher.GetIntVal() >= upgradeRangePrice)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("WeaponName", config.weapon_name);
                    hashtable.Add("WeaponLevel", range_level + 1);
                    hashtable.Add("WeaponPrice", upgradeRangePrice);
                    GameData.Instance.UploadStatistics("Vouchers_Use_Upgrade_Weapon_Range", hashtable);

                    GameData.Instance.total_voucher.SetIntVal(
                        GameData.Instance.total_voucher.GetIntVal() - upgradeRangePrice,
                        GameDataIntPurpose.Voucher
                    );

                    range_level++;
                    ResetData();
                    GameData.Instance.SaveData();
                    return true;
                }
            }
            else if (config.UpgradeCurrencyType == GameCurrencyType.Cash &&
                     GameData.Instance.total_cash.GetIntVal() >= upgradeRangePrice)
            {
                Hashtable hashtable2 = new Hashtable();
                hashtable2.Add("WeaponName", config.weapon_name);
                hashtable2.Add("WeaponLevel", range_level + 1);
                hashtable2.Add("WeaponPrice", upgradeRangePrice);
                GameData.Instance.UploadStatistics("Gold_Use_Upgrade_Weapon_Range", hashtable2);

                GameData.Instance.total_cash.SetIntVal(
                    GameData.Instance.total_cash.GetIntVal() - upgradeRangePrice,
                    GameDataIntPurpose.Cash
                );

                range_level++;
                ResetData();
                GameData.Instance.SaveData();
                return true;
            }
        }
        return false;
    }

    public float GetFrequencyWithLevel(int lv)
    {
        int num = lv - 1;
        return config.frequency_conf.base_data + (float)num * (config.frequency_conf.max_data - config.frequency_conf.base_data) / (float)(config.max_level - 1);
    }

    public bool CrystalUnlock()
    {
        if (exist_state == WeaponExistState.Locked && config.BuyCurrencyType != GameCurrencyType.Crystal)
        {
            List<GameProb> weaponFragmentProbs = GameData.Instance.GetWeaponFragmentProbs(weapon_name);
            int crystalUnlockPrice = CrystalUnlockPrice;

            if (crystalUnlockPrice > 0 &&
                GameData.Instance.total_crystal.GetIntVal() >= crystalUnlockPrice)
            {
                GameData.Instance.total_crystal.SetIntVal(
                    GameData.Instance.total_crystal.GetIntVal() - crystalUnlockPrice,
                    GameDataIntPurpose.Crystal
                );

                foreach (GameProb item in weaponFragmentProbs)
                {
                    GameData.Instance.WeaponFragmentProbs_Set.Remove(item.prob_cfg.prob_name);
                }

                Hashtable hashtable = new Hashtable();
                hashtable.Add("WeaponName", config.weapon_name);
                hashtable.Add("WeaponPrice", crystalUnlockPrice);
                GameData.Instance.UploadStatistics("tCrystal_Use_Buy_Weapon", hashtable);

                exist_state = WeaponExistState.Owned;
                GameData.Instance.SaveData();
                return true;
            }
        }
        return false;
    }
}
