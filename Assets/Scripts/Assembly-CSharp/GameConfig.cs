using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CoMZ2;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    private static GameConfig instance;

    public Dictionary<string, WeaponConfig> WeaponConfig_Set = new Dictionary<string, WeaponConfig>();

    public Dictionary<AvatarType, AvatarConfig> AvatarConfig_Set = new Dictionary<AvatarType, AvatarConfig>();

    public Dictionary<EnemyType, EnemyConfig> EnemyConfig_Set = new Dictionary<EnemyType, EnemyConfig>();

    public Dictionary<NpcType, NpcConfig> NpcConfig_Set = new Dictionary<NpcType, NpcConfig>();

    public Dictionary<int, EnemyWaveInfoList> EnemyWaveInfo_Normal_Set = new Dictionary<int, EnemyWaveInfoList>();

    public Dictionary<int, EnemyWaveInfoList> EnemyWaveInfo_Endless_Set = new Dictionary<int, EnemyWaveInfoList>();

    public Dictionary<int, EnemyWaveInfoList> EnemyWaveInfo_Endless_Bosses = new Dictionary<int, EnemyWaveInfoList>();

    public Dictionary<int, EnemyWaveInfoList> EnemyWaveInfo_Boss_Set = new Dictionary<int, EnemyWaveInfoList>();

    public Dictionary<int, EnemyWaveInfoList> EnemyWaveInfo_Npc_Set = new Dictionary<int, EnemyWaveInfoList>();

    public Dictionary<int, EnemyWaveInfoList> EnemyWaveInfo_Daily_Set = new Dictionary<int, EnemyWaveInfoList>();

    public Dictionary<int, EnemyWaveInfoList> EnemyWaveInfo_Boss_Coop_Set = new Dictionary<int, EnemyWaveInfoList>();

    public EnemyWaveIntervalInfo EnemyWave_Interval_Normal = new EnemyWaveIntervalInfo();

    public EnemyWaveIntervalInfo EnemyWave_Interval_Endless = new EnemyWaveIntervalInfo();

    public EnemyWaveIntervalInfo EnemyWave_Interval_Endless_Bosses = new EnemyWaveIntervalInfo();

    public EnemyWaveIntervalInfo EnemyWave_Interval_Boss = new EnemyWaveIntervalInfo();

    public EnemyWaveIntervalInfo EnemyWave_Interval_Npc = new EnemyWaveIntervalInfo();

    public EnemyWaveIntervalInfo EnemyWave_Interval_Daily = new EnemyWaveIntervalInfo();

    public EnemyWaveIntervalInfo EnemyWave_Interval_Boss_Coop = new EnemyWaveIntervalInfo();

    public Dictionary<string, GameProbsCfg> ProbsConfig_Set = new Dictionary<string, GameProbsCfg>();

    public Dictionary<int, QuestInfo> Side_Quest_Order = new Dictionary<int, QuestInfo>();

    public Dictionary<int, QuestInfo> Side_Quest_Order_Spare = new Dictionary<int, QuestInfo>();

    public Dictionary<int, QuestInfo> Main_Quest_Order = new Dictionary<int, QuestInfo>();

    public Dictionary<int, QuestInfo> Side_Quest_Order_Normal = new Dictionary<int, QuestInfo>();

    public Dictionary<int, QuestInfo> Side_Quest_Order_Crazy = new Dictionary<int, QuestInfo>();

    public Dictionary<int, EnemyWaveInfoList> EnemyWaveInfo_CrazyDaily_Set = new Dictionary<int, EnemyWaveInfoList>();

    public EnemyWaveIntervalInfo EnemyWave_Interval_CrazyDaily = new EnemyWaveIntervalInfo();

    public Dictionary<int, PlayerComboBuff> Player_Combo_Buff_Info = new Dictionary<int, PlayerComboBuff>();

    public string clean_mission_comment = string.Empty;

    public string time_mission_comment = string.Empty;

    public string endless_mission_comment = string.Empty;

    public string convoy_mission_comment = string.Empty;

    public string res_mission_comment = string.Empty;

    public string boss_mission_comment = string.Empty;

    public Dictionary<int, MainQuestDifficultyInfo> Main_Quest_Difficulty_Set = new Dictionary<int, MainQuestDifficultyInfo>();

    public Dictionary<int, SideQuestHpDifficultyInfo> Side_Quest_Hp_Difficulty_Set = new Dictionary<int, SideQuestHpDifficultyInfo>();

    public Dictionary<int, SideQuestDmgDifficultyInfo> Side_Quest_Dmg_Difficulty_Set = new Dictionary<int, SideQuestDmgDifficultyInfo>();

    public Dictionary<int, EndlessQuestHpDifficultyInfo> Endless_Quest_Hp_Difficulty_Set = new Dictionary<int, EndlessQuestHpDifficultyInfo>();

    public Dictionary<int, EndlessQuestDmgDifficultyInfo> Endless_Quest_Dmg_Difficulty_Set = new Dictionary<int, EndlessQuestDmgDifficultyInfo>();

    public Dictionary<int, SideQuestHpDifficultyInfo> Daily_Quest_Hp_Difficulty_Set = new Dictionary<int, SideQuestHpDifficultyInfo>();

    public Dictionary<int, SideQuestDmgDifficultyInfo> Daily_Quest_Dmg_Difficulty_Set = new Dictionary<int, SideQuestDmgDifficultyInfo>();

    public Dictionary<int, StandardWeaponDpsInfo> Standard_Weapon_Dps_Set = new Dictionary<int, StandardWeaponDpsInfo>();

    public Dictionary<int, StandardWeaponPriceInfo> Standard_Weapon_Price_Set = new Dictionary<int, StandardWeaponPriceInfo>();

    public Dictionary<int, StandardWeaponPriceInfo> Standard_Weapon_Price_Voucher_Set = new Dictionary<int, StandardWeaponPriceInfo>();

    public Dictionary<int, float> Standard_Weapon_Dps_Info = new Dictionary<int, float>();

    public Dictionary<int, float> Standard_Weapon_Price_Info = new Dictionary<int, float>();

    public Dictionary<int, float> Standard_Weapon_Price_Voucher_Info = new Dictionary<int, float>();

    public StandaryEnemyInfo Standary_Enemy_Info = new StandaryEnemyInfo();

    public AvatarHpUpInfo Avatar_Hp_Up_Info = new AvatarHpUpInfo();

    public AvatarUpPriceInfo Avatar_Up_Price_Info = new AvatarUpPriceInfo();

    public AvatarUpPriceInfo Avatar_Up_Hp_Price_Info = new AvatarUpPriceInfo();

    public AvatarUpPriceInfo Avatar_Up_Damage_Price_Info = new AvatarUpPriceInfo();

    public AvatarUpPriceInfo Avatar_Up_Armor_Price_Info = new AvatarUpPriceInfo();

    public AvatarUpPriceInfo Skill_Up_Price_Info = new AvatarUpPriceInfo();

    public MissionEnemyRewardInfo Mission_Enemy_Reward_Info = new MissionEnemyRewardInfo();

    public MissionFinishRewardInfo Mission_Finish_Reward_Info = new MissionFinishRewardInfo();

    public Dictionary<int, AvatarUpInfo> Avatar_Up_Info_Set = new Dictionary<int, AvatarUpInfo>();

    public Dictionary<int, StandaryEnemyCountInfo> Standary_Enemy_Count_Set = new Dictionary<int, StandaryEnemyCountInfo>();

    public Dictionary<int, string> Enemy_Loading_Set = new Dictionary<int, string>();

    public Dictionary<string, string> Config_Version_Set = new Dictionary<string, string>();

    public Dictionary<string, string> Remote_Config_Version_Set = new Dictionary<string, string>();

    public Dictionary<string, Color32> Channel_Scene_Color_Set = new Dictionary<string, Color32>();

    public float monster_hatred_ratio;

    public float bullet_package_ratio;

    public float hp_package_ratio;

    public int crystal_package_val;

    public int cash_package_val;

    public int init_cash;

    public int init_voucher;

    public int init_crystal;

    public int daily_price_easy1;

    public int daily_price_easy2;

    public int daily_price_hard1;

    public int daily_price_hard2;

    public int daily_price_cd;

    public float bossUpgradeChance = 0.4f;

    public int bossUpgradeInterval = 10;

    public EditorMode editor_mode = new EditorMode();

    public float rebirth_god_time;

    public bool Load_finished;

    public bool force_update_local;

    public int chaisaw_skill_percent = 20;

    public float crystal_to_cash = 1f;

    public float crystal_to_voucher = 1f;

    public Dictionary<string, SkillConfig> Skill_Avatar_Set = new Dictionary<string, SkillConfig>();

    public Dictionary<string, SkillConfig> Skill_Monster_Set = new Dictionary<string, SkillConfig>();

    public List<EnemyType> Skill_Enchant_Monster_List = new List<EnemyType>();

    public Dictionary<EnemyType, CoopBossCfg> Coop_Boss_Cfg_Set = new Dictionary<EnemyType, CoopBossCfg>();

    public Dictionary<int, int> Lottery_Seat_Count_Set = new Dictionary<int, int>();

    public Dictionary<int, int> Lottery_Seat_Weight_Set = new Dictionary<int, int>();

    public Dictionary<string, GameAwardItem> Lottery_AwardItem_Set = new Dictionary<string, GameAwardItem>();

    public int lottery_free_percent;

    public int lottery_crystal_percent;

    public GameDataInt lottery_reset_price = new GameDataInt(0);

    public GameDataInt lottery_price = new GameDataInt(0);

    public GameDataInt lottery_award_count = new GameDataInt(20);

    public GameDataInt lottery_count_award = new GameDataInt(50);

    public int server_group_index;

    public static GameConfig Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        Object.DontDestroyOnLoad(base.gameObject);
        Skill_Enchant_Monster_List.Add(EnemyType.E_ZOMBIE_COMMIS);
        Skill_Enchant_Monster_List.Add(EnemyType.E_BOOMER_TIMER);
        Skill_Enchant_Monster_List.Add(EnemyType.E_BOOMER_TIMER_E);
        Skill_Enchant_Monster_List.Add(EnemyType.E_ZOMBIE);
        Skill_Enchant_Monster_List.Add(EnemyType.E_NURSE);
        Skill_Enchant_Monster_List.Add(EnemyType.E_BOOMER);
        Skill_Enchant_Monster_List.Add(EnemyType.E_CLOWN);
        //Skill_Enchant_Monster_List.Add(EnemyType.E_ZOMBIE_COWBOY);
        Skill_Enchant_Monster_List.Add(EnemyType.E_ZOMBIE_COMMIS_E);
        Skill_Enchant_Monster_List.Add(EnemyType.E_ZOMBIE_E);
        Skill_Enchant_Monster_List.Add(EnemyType.E_NURSE_E);
        Skill_Enchant_Monster_List.Add(EnemyType.E_BOOMER_E);
        Skill_Enchant_Monster_List.Add(EnemyType.E_CLOWN_E);
        //Skill_Enchant_Monster_List.Add(EnemyType.E_ZOMBIE_COWBOY_E);
    }

    public void Init()
    {
        force_update_local = GameData.CheckSaveDataVersion();
        CleanConfig();
        LoadAvatarConfig();
        LoadWeaponConfig();
        LoadEnemyConfig();
        LoadNpcConfig();
        LoadProbsConfig();
        LoadEnemyWaveConfig();
        LoadEnemyWaveEndlessConfig();
        LoadEnemyWaveEndlessBossConfig();
        LoadMainQuestConfig();
        LoadSideQuestConfig();
        LoadSideQuestSpareConfig();
        LoadLotteryConfig();
        LoadCoopBossConfig();
        LoadSkillConfig();
        LoadMainQuestDifficultyConfig();
        LoadSideQuestDifficultyConfig();
        LoadEndlessDifficultyConfig();
        LoadDailyQuestDifficultyConfig();
        LoadStandardWeaponConfig();
        LoadEditorConfig();
        force_update_local = false;
        Application.targetFrameRate = 75;
        Load_finished = true;
    }

    public static void CheckGameConfig()
    {
        if (!GameObject.Find("GameConfig"))
        {
            GameObject gameObject = Instantiate(Resources.Load<GameObject>("GameConfig"));
            gameObject.name = "GameConfig";
            Instance.Init();
        }
        wwwClient.CheckWwwClient();
        IapCenter.CheckIapCenter();
        GameVersion.CheckGameVersionInstance();
    }

    private void CleanConfig()
    {
        WeaponConfig_Set = new Dictionary<string, WeaponConfig>();
        AvatarConfig_Set = new Dictionary<AvatarType, AvatarConfig>();
        EnemyConfig_Set = new Dictionary<EnemyType, EnemyConfig>();
        NpcConfig_Set = new Dictionary<NpcType, NpcConfig>();
        EnemyWaveInfo_Endless_Set = new Dictionary<int, EnemyWaveInfoList>();
        EnemyWaveInfo_Endless_Bosses = new Dictionary<int, EnemyWaveInfoList>();
        EnemyWaveInfo_Normal_Set = new Dictionary<int, EnemyWaveInfoList>();
        EnemyWaveInfo_Boss_Set = new Dictionary<int, EnemyWaveInfoList>();
        EnemyWaveInfo_Npc_Set = new Dictionary<int, EnemyWaveInfoList>();
        EnemyWaveInfo_Daily_Set = new Dictionary<int, EnemyWaveInfoList>();
        EnemyWave_Interval_Normal = new EnemyWaveIntervalInfo();
        EnemyWave_Interval_Endless = new EnemyWaveIntervalInfo();
        EnemyWave_Interval_Endless_Bosses = new EnemyWaveIntervalInfo();
        EnemyWave_Interval_Boss = new EnemyWaveIntervalInfo();
        EnemyWave_Interval_Npc = new EnemyWaveIntervalInfo();
        EnemyWave_Interval_Daily = new EnemyWaveIntervalInfo();
        ProbsConfig_Set = new Dictionary<string, GameProbsCfg>();
        Side_Quest_Order = new Dictionary<int, QuestInfo>();
        Main_Quest_Order = new Dictionary<int, QuestInfo>();
        Player_Combo_Buff_Info = new Dictionary<int, PlayerComboBuff>();
        clean_mission_comment = string.Empty;
        time_mission_comment = string.Empty;
        convoy_mission_comment = string.Empty;
        res_mission_comment = string.Empty;
        boss_mission_comment = string.Empty;
        endless_mission_comment = string.Empty;
        Main_Quest_Difficulty_Set = new Dictionary<int, MainQuestDifficultyInfo>();
        Side_Quest_Hp_Difficulty_Set = new Dictionary<int, SideQuestHpDifficultyInfo>();
        Side_Quest_Dmg_Difficulty_Set = new Dictionary<int, SideQuestDmgDifficultyInfo>();
        Daily_Quest_Hp_Difficulty_Set = new Dictionary<int, SideQuestHpDifficultyInfo>();
        Daily_Quest_Dmg_Difficulty_Set = new Dictionary<int, SideQuestDmgDifficultyInfo>();
        Standard_Weapon_Dps_Set = new Dictionary<int, StandardWeaponDpsInfo>();
        Standard_Weapon_Price_Set = new Dictionary<int, StandardWeaponPriceInfo>();
        Standard_Weapon_Price_Voucher_Set = new Dictionary<int, StandardWeaponPriceInfo>();
        Standard_Weapon_Dps_Info = new Dictionary<int, float>();
        Standard_Weapon_Price_Info = new Dictionary<int, float>();
        Standard_Weapon_Price_Voucher_Info = new Dictionary<int, float>();
        Standary_Enemy_Info = new StandaryEnemyInfo();
        Avatar_Hp_Up_Info = new AvatarHpUpInfo();
        Avatar_Up_Price_Info = new AvatarUpPriceInfo();
        Avatar_Up_Armor_Price_Info = new AvatarUpPriceInfo();
        Skill_Up_Price_Info = new AvatarUpPriceInfo();
        Mission_Enemy_Reward_Info = new MissionEnemyRewardInfo();
        Mission_Finish_Reward_Info = new MissionFinishRewardInfo();
        Avatar_Up_Info_Set = new Dictionary<int, AvatarUpInfo>();
        Standary_Enemy_Count_Set = new Dictionary<int, StandaryEnemyCountInfo>();
        Enemy_Loading_Set = new Dictionary<int, string>();
        Channel_Scene_Color_Set = new Dictionary<string, Color32>();
        Coop_Boss_Cfg_Set = new Dictionary<EnemyType, CoopBossCfg>();
        monster_hatred_ratio = 0f;
        bullet_package_ratio = 0f;
        hp_package_ratio = 0f;
        crystal_package_val = 0;
        init_cash = 0;
        init_voucher = 0;
        init_crystal = 0;
        daily_price_easy1 = 0;
        daily_price_easy2 = 0;
        daily_price_hard1 = 25;
        daily_price_hard2 = 25;
        daily_price_cd = 0;
        editor_mode = new EditorMode();
        rebirth_god_time = 0f;
        chaisaw_skill_percent = 20;
        crystal_to_cash = 1f;
        crystal_to_voucher = 1f;
        Skill_Avatar_Set = new Dictionary<string, SkillConfig>();
        Skill_Monster_Set = new Dictionary<string, SkillConfig>();
        Lottery_Seat_Count_Set = new Dictionary<int, int>();
        Lottery_Seat_Weight_Set = new Dictionary<int, int>();
        Lottery_AwardItem_Set = new Dictionary<string, GameAwardItem>();
        Load_finished = false;
    }

    public void LoadEditorConfig()
    {
        string xml = LoadConfigFile("EditorModeCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        XmlElement xmlElement = documentElement.GetElementsByTagName("EditorMode")[0] as XmlElement;
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            editor_mode.is_enable = int.Parse(xmlElement.GetAttribute("editorMode")) == 1;
            editor_mode.editor_mission_type = int.Parse(xmlElement.GetAttribute("missionType"));
            editor_mode.editor_camera_roam = int.Parse(xmlElement.GetAttribute("cameraRoam")) == 1;
        }
        else
        {
            editor_mode.is_enable = false;
        }
    }

    public void LoadSkillConfig()
    {
        string xml = LoadConfigFile("SkillCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;

        foreach (XmlElement item in documentElement.GetElementsByTagName("Skill"))
        {
            SkillConfig skillConfig = new SkillConfig();
            skillConfig.skill_name = item.GetAttribute("name");
            skillConfig.show_name = item.GetAttribute("showName");
            skillConfig.max_level = int.Parse(item.GetAttribute("maxLevel"));
            skillConfig.unlock_level = int.Parse(item.GetAttribute("unlockLevel"));
            skillConfig.unlock_price = int.Parse(item.GetAttribute("unlockPrice"));
            skillConfig.up_price_ratio = float.Parse(item.GetAttribute("upPriceRatio"));
            skillConfig.damage_para = float.Parse(item.GetAttribute("damagePara"));
            skillConfig.skill_content = item.GetAttribute("content");
            skillConfig.exist_state = (SkillExistState)int.Parse(item.GetAttribute("existState"));

            XmlElement element = item.GetElementsByTagName("CDTime")[0] as XmlElement;
            skillConfig.cd_time_cfg = GetUpgradeCfg(element);
            element = item.GetElementsByTagName("LifeTime")[0] as XmlElement;
            skillConfig.life_time_cfg = GetUpgradeCfg(element);
            element = item.GetElementsByTagName("HP")[0] as XmlElement;
            skillConfig.hp_cfg = GetUpgradeCfg(element);
            element = item.GetElementsByTagName("Frequency")[0] as XmlElement;
            skillConfig.frequency_cfg = GetUpgradeCfg(element);
            element = item.GetElementsByTagName("Damage")[0] as XmlElement;
            skillConfig.damage_cfg = GetUpgradeCfg(element);
            element = item.GetElementsByTagName("Range")[0] as XmlElement;
            skillConfig.range_cfg = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("Ex")[0] as XmlElement;
            if (element != null)
            {
                skillConfig.Ex_conf = GetExCfg(element);
            }

            // Owner logic
            string attribute = item.GetAttribute("ownerType");
            if (attribute == "Avatar")
            {
                skillConfig.owner_avatar = GetAvatarTypeFromCfg(item.GetAttribute("owner"));
                Skill_Avatar_Set[skillConfig.skill_name] = skillConfig;
            }
            else if (attribute == "Monster")
            {
                skillConfig.owner_enmey = GetEnemyTypeFromCfg(item.GetAttribute("owner"));
                Skill_Monster_Set[skillConfig.skill_name] = skillConfig;
            }
        }

        XmlElement xmlElement2 = documentElement.GetElementsByTagName("SkillUpPriceCfg")[0] as XmlElement;
        Skill_Up_Price_Info.ParaA = float.Parse(xmlElement2.GetAttribute("ParaA"));
        Skill_Up_Price_Info.ParaB = float.Parse(xmlElement2.GetAttribute("ParaB"));
        Skill_Up_Price_Info.ParaK = float.Parse(xmlElement2.GetAttribute("ParaK"));
    }

    public List<EnemyType> GetEnemyTypesUsedInBossCoopWaves()
    {
        HashSet<EnemyType> result = new HashSet<EnemyType>();

        foreach (KeyValuePair<int, EnemyWaveInfoList> kvp in EnemyWaveInfo_Boss_Coop_Set)
        {
            foreach (EnemyWaveInfo wave in kvp.Value.wave_info_list)
            {
                foreach (EnemySpawnInfo spawnInfo in wave.spawn_info_list)
                {
                    result.Add(spawnInfo.EType);
                }
            }   
        }

        return new List<EnemyType>(result);
    }

    public void LoadLotteryConfig()
    {
        string xml = LoadConfigFile("LotteryCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("LotterySeat"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            int value = int.Parse(item.GetAttribute("seatCount"));
            int value2 = int.Parse(item.GetAttribute("weight"));
            Lottery_Seat_Count_Set.Add(key, value);
            Lottery_Seat_Weight_Set.Add(key, value2);
        }
        XmlElement xmlElement2 = documentElement.GetElementsByTagName("LotterySeatFrashPercent")[0] as XmlElement;
        lottery_free_percent = int.Parse(xmlElement2.GetAttribute("free_val"));
        lottery_crystal_percent = int.Parse(xmlElement2.GetAttribute("crystal_val"));
        xmlElement2 = documentElement.GetElementsByTagName("LotteryAward")[0] as XmlElement;
        if (xmlElement2 != null)
        {
            lottery_award_count = new GameDataInt(int.Parse(xmlElement2.GetAttribute("count")));
            lottery_count_award = new GameDataInt(int.Parse(xmlElement2.GetAttribute("crystal")));
        }
        foreach (XmlElement item2 in documentElement.GetElementsByTagName("AwardItem"))
        {
            GameAwardItem gameAwardItem = new GameAwardItem();
            gameAwardItem.award_name = item2.GetAttribute("name");
            gameAwardItem.award_level = int.Parse(item2.GetAttribute("level"));
            gameAwardItem.award_type = (GameAwardItem.AwardType)int.Parse(item2.GetAttribute("awardType"));
            gameAwardItem.award_count = int.Parse(item2.GetAttribute("count"));
            Lottery_AwardItem_Set.Add(gameAwardItem.award_name, gameAwardItem);
        }
        xmlElement2 = documentElement.GetElementsByTagName("LotteryPrice")[0] as XmlElement;
        lottery_reset_price = new GameDataInt(int.Parse(xmlElement2.GetAttribute("resetPrice")));
        lottery_price = new GameDataInt(int.Parse(xmlElement2.GetAttribute("lotteryPrice")));
    }

    public void LoadWeaponConfig()
    {
        string xml = LoadConfigFile("WeaponCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("Weapon"))
        {
            WeaponConfig weaponConfig = new WeaponConfig();
            weaponConfig.weapon_name = item.GetAttribute("weapon_name");
            weaponConfig.wType = GetWeaponTypeFromCfg(item.GetAttribute("type"));
            weaponConfig.show_name = item.GetAttribute("showName");
            weaponConfig.moveSpeedDrag = float.Parse(item.GetAttribute("moveSpeedDrag"));

            // Clamp and assign price fields safely
            int priceVal = Mathf.Max(0, int.Parse(item.GetAttribute("price")));
            weaponConfig.price = new GameDataInt(priceVal, GetPurposeByCurrencyType(weaponConfig.BuyCurrencyType));

            weaponConfig.comment = item.GetAttribute("comment");
            weaponConfig.BuyCurrencyType = GetCurrencyType(item.GetAttribute("BuyCurrencyType"));
            weaponConfig.UpgradeCurrencyType = GetCurrencyType(item.GetAttribute("UpgradeCurrencyType"));
            weaponConfig.BulletShopCurrencyType = GetCurrencyType(item.GetAttribute("BulletShopCurrencyType"));
            weaponConfig.BulletBattleCurrencyType = GetCurrencyType(item.GetAttribute("BulletBattleCurrencyType"));
            weaponConfig.battle_buttle_count_ratio = float.Parse(item.GetAttribute("battleButtleCountRatio"));

            int bulletShopPriceVal = Mathf.Max(0, int.Parse(item.GetAttribute("bulletShopPrice")));
            weaponConfig.bulletShopPrice = new GameDataInt(bulletShopPriceVal, GetPurposeByCurrencyType(weaponConfig.BulletShopCurrencyType));

            int bulletBattletPriceVal = Mathf.Max(0, int.Parse(item.GetAttribute("bulletBalletPrice")));
            weaponConfig.bulletBattletPrice = new GameDataInt(bulletBattletPriceVal, GetPurposeByCurrencyType(weaponConfig.BulletBattleCurrencyType));

            weaponConfig.initBullet = int.Parse(item.GetAttribute("initBullet"));
            weaponConfig.buy_bullet_count = int.Parse(item.GetAttribute("buyBulletCount"));

            int crystalUnlockPriceVal = Mathf.Max(0, int.Parse(item.GetAttribute("CrystalUnlockPrice")));
            weaponConfig.crystal_unlock_price = new GameDataInt(crystalUnlockPriceVal, GameDataIntPurpose.Crystal);

            weaponConfig.damage_ratio = float.Parse(item.GetAttribute("dmgRatio"));
            weaponConfig.up_price_ratio = float.Parse(item.GetAttribute("upPriceRatio"));
            weaponConfig.up_damage_price_ratio = float.Parse(item.GetAttribute("upDamagePriceRatio"));
            weaponConfig.up_frequency_price_ratio = float.Parse(item.GetAttribute("upFrequencyPriceRatio"));
            weaponConfig.up_clip_price_ratio = float.Parse(item.GetAttribute("upClipPriceRatio"));
            weaponConfig.up_stretch_price_ratio = float.Parse(item.GetAttribute("upStretchPriceRatio"));
            weaponConfig.up_range_price_ratio = float.Parse(item.GetAttribute("upRangePriceRatio"));

            weaponConfig.max_level = int.Parse(item.GetAttribute("maxLevel"));
            weaponConfig.afterFireTime = float.Parse(item.GetAttribute("AfterFireTime"));
            weaponConfig.combo_base = float.Parse(item.GetAttribute("ComboBase"));
            weaponConfig.stretchRangeOffset = float.Parse(item.GetAttribute("stretchRange"));
            weaponConfig.stretchRangeRunOffset = float.Parse(item.GetAttribute("stretchRangeRun"));
            weaponConfig.is_auto_lock = int.Parse(item.GetAttribute("AutoLock")) == 1;
            weaponConfig.recoil = float.Parse(item.GetAttribute("Recoil"));
            weaponConfig.is_infinity_ammo = int.Parse(item.GetAttribute("infinityAmmo")) == 1;

            if (item.HasAttribute("combination"))
            {
                weaponConfig.combination_count = int.Parse(item.GetAttribute("combination"));
            }
            if (item.HasAttribute("unlockDay"))
            {
                weaponConfig.unlockDay = int.Parse(item.GetAttribute("unlockDay"));
            }
            if (item.HasAttribute("sellPrice"))
            {
                int sellPriceVal = Mathf.Max(0, int.Parse(item.GetAttribute("sellPrice")));
                weaponConfig.sell_price = new GameDataInt(sellPriceVal, GameDataIntPurpose.Cash);
            }
            weaponConfig.exist_state = (WeaponExistState)int.Parse(item.GetAttribute("existState"));
            weaponConfig.owner = GetAvatarTypeFromCfg(item.GetAttribute("owner"));

            XmlElement element = item.GetElementsByTagName("Frequency")[0] as XmlElement;
            weaponConfig.frequency_conf = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("stretchMaxRange")[0] as XmlElement;
            weaponConfig.stretch_max = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("Clip")[0] as XmlElement;
            weaponConfig.clip_conf = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("Range")[0] as XmlElement;
            weaponConfig.range_conf = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("Ex")[0] as XmlElement;
            if (element != null)
            {
                weaponConfig.Ex_conf = GetWeaponExCfg(element, weaponConfig.wType);
            }

            WeaponConfig_Set[weaponConfig.weapon_name] = weaponConfig;
        }
    }


    public void LoadEnemyConfig()
    {
        string xml = LoadConfigFile("EnemyCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("Monster"))
        {
            EnemyConfig enemyConfig = new EnemyConfig();
            enemyConfig.enemy_name = item.GetAttribute("name");
            enemyConfig.enemy_type = GetEnemyTypeFromCfg(enemyConfig.enemy_name);
            enemyConfig.view_range = float.Parse(item.GetAttribute("viewRange"));
            enemyConfig.attack_range = float.Parse(item.GetAttribute("attackRange"));
            enemyConfig.speed_val = float.Parse(item.GetAttribute("walkSpeed"));
            enemyConfig.attack_frequency = float.Parse(item.GetAttribute("attackFrequency"));
            enemyConfig.missionWeight = float.Parse(item.GetAttribute("missionWeight"));
            enemyConfig.hp_ratio = float.Parse(item.GetAttribute("hpRatio"));
            enemyConfig.damage_ratio = float.Parse(item.GetAttribute("damRatio"));
            enemyConfig.reward_ratio = float.Parse(item.GetAttribute("rewardRatio"));
            enemyConfig.attack_priority = GetEnemyPriorityFromCfg(item.GetAttribute("priority"));
            enemyConfig.load_content = item.GetAttribute("loadContent");
            XmlElement xmlElement2 = item.GetElementsByTagName("Ex")[0] as XmlElement;
            if (xmlElement2 != null)
            {
                enemyConfig.Ex_conf = GetEnemyExCfg(xmlElement2, enemyConfig.enemy_type);
            }
            EnemyConfig_Set[enemyConfig.enemy_type] = enemyConfig;
        }
        foreach (XmlElement item2 in documentElement.GetElementsByTagName("StandardMonsterCount"))
        {
            StandaryEnemyCountInfo standaryEnemyCountInfo = new StandaryEnemyCountInfo();
            int key = int.Parse(item2.GetAttribute("levelRange"));
            standaryEnemyCountInfo.base_val = float.Parse(item2.GetAttribute("base"));
            standaryEnemyCountInfo.para = float.Parse(item2.GetAttribute("para"));
            Standary_Enemy_Count_Set[key] = standaryEnemyCountInfo;
        }
        XmlElement xmlElement4 = documentElement.GetElementsByTagName("StandardMonster")[0] as XmlElement;
        Standary_Enemy_Info.ParaA = float.Parse(xmlElement4.GetAttribute("paraA"));
        Standary_Enemy_Info.ParaB = float.Parse(xmlElement4.GetAttribute("paraB"));
        Standary_Enemy_Info.ParaC = float.Parse(xmlElement4.GetAttribute("paraC"));
        Standary_Enemy_Info.ParaK = float.Parse(xmlElement4.GetAttribute("paraK"));
        XmlElement xmlElement5 = documentElement.GetElementsByTagName("MissionMonsterReward")[0] as XmlElement;
        Mission_Enemy_Reward_Info.main_ratio = float.Parse(xmlElement5.GetAttribute("ratioMain"));
        Mission_Enemy_Reward_Info.side_ratio = float.Parse(xmlElement5.GetAttribute("ratioSide"));
        Mission_Enemy_Reward_Info.daily_ratio_a = float.Parse(xmlElement5.GetAttribute("ratioDailyA"));
        Mission_Enemy_Reward_Info.daily_ratio_b = float.Parse(xmlElement5.GetAttribute("ratioDailyB"));
        xmlElement5 = documentElement.GetElementsByTagName("MissionFinishReward")[0] as XmlElement;
        Mission_Finish_Reward_Info.side_ratio = float.Parse(xmlElement5.GetAttribute("ratioSide"));
        Mission_Finish_Reward_Info.daily_ratio_a = float.Parse(xmlElement5.GetAttribute("ratioDailyA"));
        Mission_Finish_Reward_Info.daily_ratio_b = float.Parse(xmlElement5.GetAttribute("ratioDailyB"));
        Mission_Finish_Reward_Info.daily_ratio_voucher_a = float.Parse(xmlElement5.GetAttribute("ratioDailyVoucherA"));
        Mission_Finish_Reward_Info.daily_ratio_voucher_b = float.Parse(xmlElement5.GetAttribute("ratioDailyVoucherB"));
        Mission_Finish_Reward_Info.daily_ratio_voucher_base = float.Parse(xmlElement5.GetAttribute("ratioDailyVoucherBase"));
        xmlElement4 = documentElement.GetElementsByTagName("MonsterHatred")[0] as XmlElement;
        monster_hatred_ratio = float.Parse(xmlElement4.GetAttribute("ratio"));
        foreach (XmlElement item3 in documentElement.GetElementsByTagName("MonsterLoadShow"))
        {
            int key2 = int.Parse(item3.GetAttribute("index"));
            Enemy_Loading_Set[key2] = item3.GetAttribute("monster");
        }
    }

    public void LoadCoopBossConfig()
    {
        string xml = LoadConfigFile("CoopBossCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("Boss"))
        {
            CoopBossCfg coopBossCfg = new CoopBossCfg();
            coopBossCfg.boss_name = item.GetAttribute("codename");
            coopBossCfg.boss_show_name = item.GetAttribute("showName");
            coopBossCfg.boss_type = GetEnemyTypeFromCfg(coopBossCfg.boss_name);
            coopBossCfg.day_level = int.Parse(item.GetAttribute("day"));
            coopBossCfg.hp_capacity = float.Parse(item.GetAttribute("hp")) / 4;
            coopBossCfg.damage_base = float.Parse(item.GetAttribute("damage"));
            coopBossCfg.reward_gold = int.Parse(item.GetAttribute("rewardGold"));
            coopBossCfg.reward_crystal = int.Parse(item.GetAttribute("rewardCrystal"));
            coopBossCfg.reward_weapon = item.GetAttribute("rewardsWeapon");
            coopBossCfg.reward_gold_failed = int.Parse(item.GetAttribute("lose"));
            string attribute = item.GetAttribute("rewardsWeaponParts");
            if (!string.IsNullOrEmpty(attribute))
            {
                string[] array = attribute.Split('|');
                for (int i = 0; i < array.Length; i++)
                {
                    if (!string.IsNullOrEmpty(array[i]))
                        coopBossCfg.rewards_weapon_fragments.Add(array[i]);
                }
            }
            foreach (XmlElement item2 in item.GetElementsByTagName("scene"))
            {
                string attribute2 = item2.GetAttribute("val");
                coopBossCfg.scene_list.Add(attribute2);
            }
            Coop_Boss_Cfg_Set[coopBossCfg.boss_type] = coopBossCfg;
        }
    }

    public void LoadAvatarConfig()
    {
        string xml = LoadConfigFile("AvatarCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;

        foreach (XmlElement item in documentElement.GetElementsByTagName("Avatar"))
        {
            AvatarConfig avatarConfig = new AvatarConfig();

            avatarConfig.show_name = item.GetAttribute("showName");
            avatarConfig.avatar_name = item.GetAttribute("avatar_name");
            avatarConfig.avatar_type = GetAvatarTypeFromCfg(item.GetAttribute("type"));
            avatarConfig.is_voucher_avatar = int.Parse(item.GetAttribute("voucherAvatar")) == 1;
            avatarConfig.price = new GameDataInt(int.Parse(item.GetAttribute("price")));
            avatarConfig.crystal_unlock_price = new GameDataInt(int.Parse(item.GetAttribute("CrystalUnlockPrice")));
            avatarConfig.up_hp_price_ratio = float.Parse(item.GetAttribute("upHpPriceFactor"));
            avatarConfig.up_damage_price_ratio = float.Parse(item.GetAttribute("upDamagePriceFactor"));
            avatarConfig.up_armor_price_ratio = float.Parse(item.GetAttribute("upArmorPriceFactor"));
            avatarConfig.up_price_ratio = int.Parse(item.GetAttribute("upPriceFactor"));
            avatarConfig.max_level = int.Parse(item.GetAttribute("maxLevel"));
            avatarConfig.reload_ratio = float.Parse(item.GetAttribute("reloadRatio"));
            avatarConfig.fix_val = float.Parse(item.GetAttribute("fixVal"));
            avatarConfig.hp_ratio = float.Parse(item.GetAttribute("hpRatio"));

            if (item.HasAttribute("worth1"))
            {
                avatarConfig.avatar_worth_1 = int.Parse(item.GetAttribute("worth1"));
            }
            if (item.HasAttribute("worth2"))
            {
                avatarConfig.avatar_worth_2 = int.Parse(item.GetAttribute("worth2"));
            }
            if (item.HasAttribute("unlockDay"))
            {
                avatarConfig.unlockDay = int.Parse(item.GetAttribute("unlockDay"));
            }

            avatarConfig.exist_state = (AvatarExistState)int.Parse(item.GetAttribute("existState"));

            if (item.HasAttribute("firstSkill"))
            {
                avatarConfig.first_skill = item.GetAttribute("firstSkill");
            }

            XmlElement element;

            element = item.GetElementsByTagName("HP")[0] as XmlElement;
            avatarConfig.hp_conf = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("Speed")[0] as XmlElement;
            avatarConfig.speed_conf = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("Damage")[0] as XmlElement;
            avatarConfig.damage_conf = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("Armor")[0] as XmlElement;
            avatarConfig.armor_conf = GetUpgradeCfg(element);

            element = item.GetElementsByTagName("Ex")[0] as XmlElement;
            avatarConfig.extension_conf = GetUpgradeCfg(element);

            if (avatarConfig.max_level > 1)
            {
                avatarConfig.damage_conf.ratio = (avatarConfig.damage_conf.max_data / avatarConfig.damage_conf.base_data - 1f) / (avatarConfig.max_level - 1);
                avatarConfig.armor_conf.ratio = (avatarConfig.armor_conf.max_data / avatarConfig.armor_conf.base_data - 1f) / (avatarConfig.max_level - 1);
            }
            else
            {
                avatarConfig.damage_conf.ratio = 0f;
                avatarConfig.armor_conf.ratio = 0f;
            }

            AvatarConfig_Set[avatarConfig.avatar_type] = avatarConfig;
        }

        // Load AvatarExp
        foreach (XmlElement item3 in documentElement.GetElementsByTagName("AvatarExp"))
        {
            AvatarUpInfo avatarUpInfo = new AvatarUpInfo();
            int key = int.Parse(item3.GetAttribute("levelRange"));
            avatarUpInfo.ParaA = float.Parse(item3.GetAttribute("ParaA"));
            avatarUpInfo.ParaB = float.Parse(item3.GetAttribute("ParaB"));
            Avatar_Up_Info_Set[key] = avatarUpInfo;
        }

        // Load AvatarCombo
        foreach (XmlElement item4 in documentElement.GetElementsByTagName("AvatarCombo"))
        {
            PlayerComboBuff playerComboBuff = new PlayerComboBuff();
            int key2 = int.Parse(item4.GetAttribute("level"));
            playerComboBuff.damage_ratio = float.Parse(item4.GetAttribute("DmgRatio"));
            playerComboBuff.rate_ratio = float.Parse(item4.GetAttribute("RateRatio"));
            Player_Combo_Buff_Info[key2] = playerComboBuff;
        }

        // Load GameInitCurrency
        XmlElement xmlElement5 = documentElement.GetElementsByTagName("GameInitCurrency")[0] as XmlElement;
        init_cash = int.Parse(xmlElement5.GetAttribute("cash"));
        init_crystal = int.Parse(xmlElement5.GetAttribute("crystal"));
        init_voucher = int.Parse(xmlElement5.GetAttribute("voucher"));

        // Load Rebirth
        XmlElement xmlElement6 = documentElement.GetElementsByTagName("Rebirth")[0] as XmlElement;
        rebirth_god_time = float.Parse(xmlElement6.GetAttribute("time"));

        // Load AvatarHpCfg
        xmlElement6 = documentElement.GetElementsByTagName("AvatarHpCfg")[0] as XmlElement;
        Avatar_Hp_Up_Info.ParaA = float.Parse(xmlElement6.GetAttribute("ParaA"));
        Avatar_Hp_Up_Info.ParaB = float.Parse(xmlElement6.GetAttribute("ParaB"));
        Avatar_Hp_Up_Info.ParaC = float.Parse(xmlElement6.GetAttribute("ParaC"));

        // Load AvatarUpCfg
        xmlElement6 = documentElement.GetElementsByTagName("AvatarUpCfg")[0] as XmlElement;
        Avatar_Up_Price_Info.ParaA = float.Parse(xmlElement6.GetAttribute("ParaA"));
        Avatar_Up_Price_Info.ParaB = float.Parse(xmlElement6.GetAttribute("ParaB"));
        Avatar_Up_Price_Info.ParaK = float.Parse(xmlElement6.GetAttribute("ParaK"));

        // Load AvatarHpUpPriceCfg
        xmlElement6 = documentElement.GetElementsByTagName("AvatarHpUpPriceCfg")[0] as XmlElement;
        Avatar_Up_Hp_Price_Info.ParaA = float.Parse(xmlElement6.GetAttribute("ParaA"));
        Avatar_Up_Hp_Price_Info.ParaB = float.Parse(xmlElement6.GetAttribute("ParaB"));
        Avatar_Up_Hp_Price_Info.ParaK = float.Parse(xmlElement6.GetAttribute("ParaK"));

        // Load AvatarDamagePriceCfg
        xmlElement6 = documentElement.GetElementsByTagName("AvatarDamagePriceCfg")[0] as XmlElement;
        Avatar_Up_Damage_Price_Info.ParaA = float.Parse(xmlElement6.GetAttribute("ParaA"));
        Avatar_Up_Damage_Price_Info.ParaB = float.Parse(xmlElement6.GetAttribute("ParaB"));
        Avatar_Up_Damage_Price_Info.ParaK = float.Parse(xmlElement6.GetAttribute("ParaK"));

        // Load AvatarArmorPriceCfg
        xmlElement6 = documentElement.GetElementsByTagName("AvatarArmorPriceCfg")[0] as XmlElement;
        Avatar_Up_Armor_Price_Info.ParaA = float.Parse(xmlElement6.GetAttribute("ParaA"));
        Avatar_Up_Armor_Price_Info.ParaB = float.Parse(xmlElement6.GetAttribute("ParaB"));
        Avatar_Up_Armor_Price_Info.ParaK = float.Parse(xmlElement6.GetAttribute("ParaK"));

        // Load ExchgCurrcyCfg
        xmlElement6 = documentElement.GetElementsByTagName("ExchgCurrcyCfg")[0] as XmlElement;
        crystal_to_cash = float.Parse(xmlElement6.GetAttribute("toCash"));
        crystal_to_voucher = float.Parse(xmlElement6.GetAttribute("toVoucher"));

        // Load ChannelSceneColor
        foreach (XmlElement item5 in documentElement.GetElementsByTagName("ChannelSceneColor"))
        {
            string attribute = item5.GetAttribute("name");
            int num = int.Parse(item5.GetAttribute("color_r"));
            int num2 = int.Parse(item5.GetAttribute("color_g"));
            int num3 = int.Parse(item5.GetAttribute("color_b"));
            int num4 = int.Parse(item5.GetAttribute("color_a"));
            Channel_Scene_Color_Set[attribute] = new Color32((byte)num, (byte)num2, (byte)num3, (byte)num4);
        }
    }

    public void LoadMainQuestDifficultyConfig()
    {
        string xml = LoadConfigFile("MainQuestDifficultyCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("MainQuestDifficulty"))
        {
            MainQuestDifficultyInfo mainQuestDifficultyInfo = new MainQuestDifficultyInfo();
            int key = int.Parse(item.GetAttribute("level"));
            mainQuestDifficultyInfo.playerHp = float.Parse(item.GetAttribute("playerHp"));
            mainQuestDifficultyInfo.playerDps = float.Parse(item.GetAttribute("playerDps"));
            mainQuestDifficultyInfo.weaponDps = float.Parse(item.GetAttribute("weaponDps"));
            mainQuestDifficultyInfo.killTime = float.Parse(item.GetAttribute("killTime"));
            mainQuestDifficultyInfo.deathTime = float.Parse(item.GetAttribute("deathTime"));
            mainQuestDifficultyInfo.comboVal = float.Parse(item.GetAttribute("comboVal"));
            mainQuestDifficultyInfo.reward = float.Parse(item.GetAttribute("reward"));
            mainQuestDifficultyInfo.finish_reward = float.Parse(item.GetAttribute("finishReward"));
            Main_Quest_Difficulty_Set[key] = mainQuestDifficultyInfo;
        }
    }

    public void LoadSideQuestDifficultyConfig()
    {
        string xml = LoadConfigFile("SideQuestDifficultyCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("SideQuestHpDifficulty"))
        {
            SideQuestHpDifficultyInfo sideQuestHpDifficultyInfo = new SideQuestHpDifficultyInfo();
            int key = int.Parse(item.GetAttribute("levelRange"));
            sideQuestHpDifficultyInfo.hp_n = int.Parse(item.GetAttribute("hpN"));
            sideQuestHpDifficultyInfo.hpParaA = float.Parse(item.GetAttribute("hpParaA"));
            sideQuestHpDifficultyInfo.hpParaB = float.Parse(item.GetAttribute("hpParaB"));
            Side_Quest_Hp_Difficulty_Set[key] = sideQuestHpDifficultyInfo;
        }
        foreach (XmlElement item2 in documentElement.GetElementsByTagName("SideQuestDmgDifficulty"))
        {
            SideQuestDmgDifficultyInfo sideQuestDmgDifficultyInfo = new SideQuestDmgDifficultyInfo();
            int key2 = int.Parse(item2.GetAttribute("levelRange"));
            sideQuestDmgDifficultyInfo.dmg_n = int.Parse(item2.GetAttribute("dmgN"));
            sideQuestDmgDifficultyInfo.dmgParaA = float.Parse(item2.GetAttribute("dmgParaA"));
            sideQuestDmgDifficultyInfo.dmgParaB = float.Parse(item2.GetAttribute("dmgParaB"));
            Side_Quest_Dmg_Difficulty_Set[key2] = sideQuestDmgDifficultyInfo;
        }
    }

    public void LoadEndlessDifficultyConfig()
    {
        string xml = LoadConfigFile("EndlessQuestDifficultyCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;

        foreach (XmlElement item in documentElement.GetElementsByTagName("EndlessQuestHpDifficulty"))
        {
            EndlessQuestHpDifficultyInfo endlessHpDifficultyInfo = new EndlessQuestHpDifficultyInfo();
            int key = int.Parse(item.GetAttribute("levelRange"));
            endlessHpDifficultyInfo.hp_n = int.Parse(item.GetAttribute("hpN"));
            endlessHpDifficultyInfo.hpParaA = float.Parse(item.GetAttribute("hpParaA"));
            endlessHpDifficultyInfo.hpParaB = float.Parse(item.GetAttribute("hpParaB"));
            Endless_Quest_Hp_Difficulty_Set[key] = endlessHpDifficultyInfo;
        }

        foreach (XmlElement item2 in documentElement.GetElementsByTagName("EndlessQuestDmgDifficulty"))
        {
            EndlessQuestDmgDifficultyInfo endlessDmgDifficultyInfo = new EndlessQuestDmgDifficultyInfo();
            int key2 = int.Parse(item2.GetAttribute("levelRange"));
            endlessDmgDifficultyInfo.dmg_n = int.Parse(item2.GetAttribute("dmgN"));
            endlessDmgDifficultyInfo.dmgParaA = float.Parse(item2.GetAttribute("dmgParaA"));
            endlessDmgDifficultyInfo.dmgParaB = float.Parse(item2.GetAttribute("dmgParaB"));
            Endless_Quest_Dmg_Difficulty_Set[key2] = endlessDmgDifficultyInfo;
        }
        //Debug.Log("[LoadEndlessDifficultyConfig] Loaded HP difficulty keys: " + (Endless_Quest_Hp_Difficulty_Set));
        //Debug.Log("[LoadEndlessDifficultyConfig] Loaded DMG difficulty keys: " + (Endless_Quest_Dmg_Difficulty_Set));
    }

    public void LoadDailyQuestDifficultyConfig()
    {
        string xml = LoadConfigFile("DailyQuestDifficultyCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("DailyQuestHpDifficulty"))
        {
            SideQuestHpDifficultyInfo sideQuestHpDifficultyInfo = new SideQuestHpDifficultyInfo();
            int key = int.Parse(item.GetAttribute("levelRange"));
            sideQuestHpDifficultyInfo.hp_n = int.Parse(item.GetAttribute("hpN"));
            sideQuestHpDifficultyInfo.hpParaA = float.Parse(item.GetAttribute("hpParaA"));
            sideQuestHpDifficultyInfo.hpParaB = float.Parse(item.GetAttribute("hpParaB"));
            Daily_Quest_Hp_Difficulty_Set[key] = sideQuestHpDifficultyInfo;
        }
        foreach (XmlElement item2 in documentElement.GetElementsByTagName("DailyQuestDmgDifficulty"))
        {
            SideQuestDmgDifficultyInfo sideQuestDmgDifficultyInfo = new SideQuestDmgDifficultyInfo();
            int key2 = int.Parse(item2.GetAttribute("levelRange"));
            sideQuestDmgDifficultyInfo.dmg_n = int.Parse(item2.GetAttribute("dmgN"));
            sideQuestDmgDifficultyInfo.dmgParaA = float.Parse(item2.GetAttribute("dmgParaA"));
            sideQuestDmgDifficultyInfo.dmgParaB = float.Parse(item2.GetAttribute("dmgParaB"));
            Daily_Quest_Dmg_Difficulty_Set[key2] = sideQuestDmgDifficultyInfo;
        }
        XmlElement xmlElement3 = documentElement.GetElementsByTagName("DailyQuestPrice")[0] as XmlElement;
        daily_price_easy1 = int.Parse(xmlElement3.GetAttribute("easyPrice1"));
        daily_price_easy2 = int.Parse(xmlElement3.GetAttribute("easyPrice2"));
        daily_price_hard1 = int.Parse(xmlElement3.GetAttribute("hardPrice1"));
        daily_price_hard2 = int.Parse(xmlElement3.GetAttribute("hardPrice2"));
        daily_price_cd = int.Parse(xmlElement3.GetAttribute("cdPrice"));
    }

    public void LoadStandardWeaponConfig()
    {
        string xml = LoadConfigFile("StandardWeaponCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("StandardWeaponDps"))
        {
            StandardWeaponDpsInfo standardWeaponDpsInfo = new StandardWeaponDpsInfo();
            int key = int.Parse(item.GetAttribute("levelRange"));
            standardWeaponDpsInfo.base_val = float.Parse(item.GetAttribute("base"));
            standardWeaponDpsInfo.para_val = float.Parse(item.GetAttribute("para"));
            Standard_Weapon_Dps_Set[key] = standardWeaponDpsInfo;
        }
        foreach (XmlElement item2 in documentElement.GetElementsByTagName("StandardWeaponPrice"))
        {
            StandardWeaponPriceInfo standardWeaponPriceInfo = new StandardWeaponPriceInfo();
            int key2 = int.Parse(item2.GetAttribute("levelRange"));
            standardWeaponPriceInfo.base_val = float.Parse(item2.GetAttribute("base"));
            standardWeaponPriceInfo.para_val = float.Parse(item2.GetAttribute("para"));
            Standard_Weapon_Price_Set[key2] = standardWeaponPriceInfo;
        }
        foreach (XmlElement item3 in documentElement.GetElementsByTagName("StandardWeaponVoucher"))
        {
            StandardWeaponPriceInfo standardWeaponPriceInfo2 = new StandardWeaponPriceInfo();
            int key3 = int.Parse(item3.GetAttribute("levelRange"));
            standardWeaponPriceInfo2.base_val = float.Parse(item3.GetAttribute("base"));
            standardWeaponPriceInfo2.para_val = float.Parse(item3.GetAttribute("para"));
            Standard_Weapon_Price_Voucher_Set[key3] = standardWeaponPriceInfo2;
        }
        for (int i = 1; i <= WeaponConfig_Set["MP5"].max_level; i++)
        {
            int num = 0;
            foreach (int key4 in Standard_Weapon_Dps_Set.Keys)
            {
                if (i >= key4)
                {
                    num = key4;
                }
            }
            Standard_Weapon_Dps_Info[i] = Standard_Weapon_Dps_Set[num].base_val * Mathf.Pow(Standard_Weapon_Dps_Set[num].para_val, i - num);
        }
        for (int j = 2; j <= WeaponConfig_Set["MP5"].max_level; j++)
        {
            int num2 = 0;
            foreach (int key5 in Standard_Weapon_Price_Set.Keys)
            {
                if (j >= key5)
                {
                    num2 = key5;
                }
            }
            Standard_Weapon_Price_Info[j] = Standard_Weapon_Price_Set[num2].base_val * Mathf.Pow(Standard_Weapon_Price_Set[num2].para_val, j - num2);
        }
        for (int k = 2; k <= WeaponConfig_Set["MP5"].max_level; k++)
        {
            int num3 = 0;
            foreach (int key6 in Standard_Weapon_Price_Voucher_Set.Keys)
            {
                if (k >= key6)
                {
                    num3 = key6;
                }
            }
            Standard_Weapon_Price_Voucher_Info[k] = Standard_Weapon_Price_Voucher_Set[num3].base_val * Mathf.Pow(Standard_Weapon_Price_Voucher_Set[num3].para_val, k - num3);
        }
    }

    public void LoadNpcConfig()
    {
        string xml = LoadConfigFile("NpcCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("NPC"))
        {
            NpcConfig npcConfig = new NpcConfig();
            npcConfig.npc_name = item.GetAttribute("name");
            npcConfig.npc_type = GetNpcTypeFromCfg(npcConfig.npc_name);
            npcConfig.speed_val = float.Parse(item.GetAttribute("walkSpeed"));
            npcConfig.hp_ratio = float.Parse(item.GetAttribute("hpRatio"));
            NpcConfig_Set[npcConfig.npc_type] = npcConfig;
        }
    }

    private GameDataIntPurpose GetPurposeByCurrencyType(GameCurrencyType currencyType)
    {
        switch (currencyType)
        {
            case GameCurrencyType.Cash:
                return GameDataIntPurpose.Cash;
            case GameCurrencyType.Crystal:
                return GameDataIntPurpose.Crystal;
            case GameCurrencyType.Voucher:
                return GameDataIntPurpose.Voucher;
            default:
                return GameDataIntPurpose.Generic;
        }
    }

    public void LoadProbsConfig()
    {
        string xml = LoadConfigFile("GameProbsCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("GameStory"))
        {
            GameProbsCfg gameProbsCfg = new GameStoryProbsCfg();
            gameProbsCfg.prob_name = item.GetAttribute("name");
            ((GameStoryProbsCfg)gameProbsCfg).level = int.Parse(item.GetAttribute("level"));
            ((GameStoryProbsCfg)gameProbsCfg).weight = int.Parse(item.GetAttribute("weight"));
            ((GameStoryProbsCfg)gameProbsCfg).image_name = item.GetAttribute("image");
            ProbsConfig_Set[gameProbsCfg.prob_name] = gameProbsCfg;
        }
        foreach (XmlElement item2 in documentElement.GetElementsByTagName("WeaponFragment"))
        {
            WeaponFragmentProbsCfg gameProbsCfg2 = new WeaponFragmentProbsCfg();
            gameProbsCfg2.prob_name = item2.GetAttribute("name");
            gameProbsCfg2.weapon_name = item2.GetAttribute("weapon");
            gameProbsCfg2.image_name = item2.GetAttribute("image");
            gameProbsCfg2.type = (WeaponFragmentProbsCfg.WeaponFragmentType)int.Parse(item2.GetAttribute("type"));
            if (item2.HasAttribute("sellPrice"))
            {
                gameProbsCfg2.sell_price = new GameDataInt(int.Parse(item2.GetAttribute("sellPrice")));
            }
            ProbsConfig_Set[gameProbsCfg2.prob_name] = gameProbsCfg2;

            Debug.Log("Loaded WeaponFragment prob config: " + gameProbsCfg2.prob_name);
        }
        foreach (XmlElement item3 in documentElement.GetElementsByTagName("WeaponIntensifier"))
        {
            GameProbsCfg gameProbsCfg3 = new WeaponIntensifierProbsCfg();
            gameProbsCfg3.prob_name = item3.GetAttribute("name");
            ((WeaponIntensifierProbsCfg)gameProbsCfg3).weapon_name = item3.GetAttribute("weapon");
            ((WeaponIntensifierProbsCfg)gameProbsCfg3).weight = int.Parse(item3.GetAttribute("weight"));
            ((WeaponIntensifierProbsCfg)gameProbsCfg3).image_name = item3.GetAttribute("image");
            ProbsConfig_Set[gameProbsCfg3.prob_name] = gameProbsCfg3;
        }
        XmlElement xmlElement4 = documentElement.GetElementsByTagName("HpPackage")[0] as XmlElement;
        hp_package_ratio = float.Parse(xmlElement4.GetAttribute("ratio"));
        xmlElement4 = documentElement.GetElementsByTagName("BulletPackage")[0] as XmlElement;
        bullet_package_ratio = float.Parse(xmlElement4.GetAttribute("ratio"));
        xmlElement4 = documentElement.GetElementsByTagName("CashPackage")[0] as XmlElement;
        cash_package_val = int.Parse(xmlElement4.GetAttribute("val"));
    }

    public void LoadMainQuestConfig()
    {
        string xml = LoadConfigFile("MainQuestCfg");
        if (string.IsNullOrEmpty(xml) || xml.Trim().Length == 0)
        {
            Debug.LogError("MainQuestCfg XML content is empty or null!");
            return;
        }

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("MainQuest"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            QuestInfo questInfo = new QuestInfo();
            questInfo.mission_day_type = MissionDayType.Main;
            if (item.HasAttribute("prob"))
            {
                string text = (questInfo.reward_prob = item.GetAttribute("prob"));
            }
            else
            {
                questInfo.reward_prob = string.Empty;
            }
            if (item.HasAttribute("weapon"))
            {
                string text2 = (questInfo.reward_weapon = item.GetAttribute("weapon"));
            }
            else
            {
                questInfo.reward_weapon = string.Empty;
            }
            if (item.HasAttribute("scene"))
            {
                string text3 = (questInfo.scene_name = item.GetAttribute("scene"));
            }
            if (item.HasAttribute("mode"))
            {
                questInfo.mission_type = (MissionType)int.Parse(item.GetAttribute("mode"));
            }
            if (item.HasAttribute("boss"))
            {
                EnemyType enemyType = (questInfo.boss_type = GetEnemyTypeFromCfg(item.GetAttribute("boss")));
            }
            if (item.HasAttribute("comment"))
            {
                string text4 = (questInfo.mission_tag = item.GetAttribute("comment"));
            }
            if (item.HasAttribute("avatar"))
            {
                AvatarType avatarType = (questInfo.avatar = GetAvatarTypeFromCfg(item.GetAttribute("avatar")));
            }
            else
            {
                questInfo.avatar = AvatarType.None;
            }
            Main_Quest_Order[key] = questInfo;
        }
    }

    public void LoadSideQuestConfig()
    {
        string xml = LoadConfigFile("SideQuestCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("SideQuestOrder"))
        {
            QuestInfo questInfo = new QuestInfo();
            questInfo.mission_day_type = MissionDayType.Side;
            int key = int.Parse(item.GetAttribute("level"));
            if (item.HasAttribute("scene"))
            {
                string text = (questInfo.scene_name = item.GetAttribute("scene"));
            }
            if (item.HasAttribute("mode"))
            {
                questInfo.mission_type = (MissionType)int.Parse(item.GetAttribute("mode"));
            }
            if (item.HasAttribute("boss"))
            {
                EnemyType enemyType = (questInfo.boss_type = GetEnemyTypeFromCfg(item.GetAttribute("boss")));
            }
            if (item.HasAttribute("scene"))
            {
                questInfo.scene_name = item.GetAttribute("scene");
            }
            Side_Quest_Order[key] = questInfo;
        }
        XmlElement xmlElement2 = documentElement.GetElementsByTagName("CleanQuest")[0] as XmlElement;
        clean_mission_comment = xmlElement2.GetAttribute("comment");
        xmlElement2 = documentElement.GetElementsByTagName("TimeQuest")[0] as XmlElement;
        time_mission_comment = xmlElement2.GetAttribute("comment");
        xmlElement2 = documentElement.GetElementsByTagName("NpcResQuest")[0] as XmlElement;
        res_mission_comment = xmlElement2.GetAttribute("comment");
        xmlElement2 = documentElement.GetElementsByTagName("NpcConvoyQuest")[0] as XmlElement;
        convoy_mission_comment = xmlElement2.GetAttribute("comment");
        xmlElement2 = documentElement.GetElementsByTagName("BossQuest")[0] as XmlElement;
        boss_mission_comment = xmlElement2.GetAttribute("comment");
        xmlElement2 = documentElement.GetElementsByTagName("EndlessQuest")[0] as XmlElement;
        endless_mission_comment = xmlElement2.GetAttribute("comment");
    }

    public void LoadSideQuestSpareConfig()
    {
        string xml = LoadConfigFile("SideQuestSpareCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("SideQuestOrder"))
        {
            QuestInfo questInfo = new QuestInfo();
            questInfo.mission_day_type = MissionDayType.Side;
            int key = int.Parse(item.GetAttribute("level"));
            if (item.HasAttribute("scene"))
            {
                string text = (questInfo.scene_name = item.GetAttribute("scene"));
            }
            if (item.HasAttribute("mode"))
            {
                questInfo.mission_type = (MissionType)int.Parse(item.GetAttribute("mode"));
            }
            if (item.HasAttribute("boss"))
            {
                EnemyType enemyType = (questInfo.boss_type = GetEnemyTypeFromCfg(item.GetAttribute("boss")));
            }
            if (item.HasAttribute("scene"))
            {
                questInfo.scene_name = item.GetAttribute("scene");
            }
            Side_Quest_Order_Spare[key] = questInfo;
        }
    }

    public NpcType GetNpcTypeFromCfg(string type_name)
    {
        NpcType result = NpcType.N_NONE;
        switch (type_name)
        {
            case "follower":
                result = NpcType.N_FOLLOWER;
                break;
            case "worker":
                result = NpcType.N_WORKER;
                break;
        }
        return result;
    }

    public EnemyType GetEnemyTypeFromCfg(string type_name)
    {
        EnemyType result = EnemyType.E_NONE;
        switch (type_name)
        {
            case "Zombie":
                result = EnemyType.E_ZOMBIE;
                break;
            case "Zombie_E":
                result = EnemyType.E_ZOMBIE_E;
                break;
            case "Zombie_Commis":
                result = EnemyType.E_ZOMBIE_COMMIS;
                break;
            case "Zombie_Commis_E":
                result = EnemyType.E_ZOMBIE_COMMIS_E;
                break;
            case "Nurse":
                result = EnemyType.E_NURSE;
                break;
            case "Nurse_E":
                result = EnemyType.E_NURSE_E;
                break;
            case "Boomer":
                result = EnemyType.E_BOOMER;
                break;
            case "Boomer_E":
                result = EnemyType.E_BOOMER_E;
                break;
            case "BoomerTimer":
                result = EnemyType.E_BOOMER_TIMER;
                break;
            case "BoomerTimer_E":
                result = EnemyType.E_BOOMER_TIMER_E;
                break;
            case "Crow":
                result = EnemyType.E_CROW;
                break;
            case "Clown":
                result = EnemyType.E_CLOWN;
                break;
            case "Clown_E":
                result = EnemyType.E_CLOWN_E;
                break;
            case "FatCook":
                result = EnemyType.E_FATCOOK;
                break;
            case "FatCook_E":
                result = EnemyType.E_FATCOOK_E;
                break;
            case "Haoke_A":
                result = EnemyType.E_HAOKE_A;
                break;
            case "Haoke_B":
                result = EnemyType.E_HAOKE_B;
                break;
            case "Wrestler":
                result = EnemyType.E_WRESTLER;
                break;
            case "Wrestler_E":
                result = EnemyType.E_WRESTLER_E;
                break;
            case "Halloween":
                result = EnemyType.E_HALLOWEEN;
                break;
            case "Halloween_E":
                result = EnemyType.E_HALLOWEEN_E;
                break;
            case "HalloweenSub":
                result = EnemyType.E_HALLOWEEN_SUB;
                break;
            case "HalloweenSub_E":
                result = EnemyType.E_HALLOWEEN_SUB_E;
                break;
            case "Shark":
                result = EnemyType.E_SHARK;
                break;
            case "Shark_E":
                result = EnemyType.E_SHARK_E;
                break;
        }
        return result;
    }

    public WeaponType GetWeaponTypeFromCfg(string type_name)
    {
        WeaponType result = WeaponType.NoGun;
        switch (type_name)
        {
            case "Rifle":
                result = WeaponType.AssaultRifle;
                break;
            case "ShotGun":
                result = WeaponType.ShotGun;
                break;
            case "Pistol":
                result = WeaponType.Pistol;
                break;
            case "RPG":
                result = WeaponType.RocketLauncher;
                break;
            case "MachineGun":
                result = WeaponType.MachineGun;
                break;
            case "Laser":
                result = WeaponType.Laser;
                break;
            case "PGM":
                result = WeaponType.PGM;
                break;
            case "Gatlin":
                result = WeaponType.Gatling;
                break;
            case "Saw":
                result = WeaponType.Saw;
                break;
            case "Baseball":
                result = WeaponType.Baseball;
                break;
            case "Sword":
                result = WeaponType.Sword;
                break;
            case "M32":
                result = WeaponType.M32;
                break;
            case "Flame":
                result = WeaponType.Flame;
                break;
            case "Mine":
                result = WeaponType.Mines;
                break;
            case "Medicine":
                result = WeaponType.Medicine;
                break;
            case "Shield":
                result = WeaponType.Shield;
                break;
            case "IonCannon":
                result = WeaponType.IonCannon;
                break;
            case "IceGun":
                result = WeaponType.IceGun;
                break;
        }
        return result;
    }

    public GameCurrencyType GetCurrencyType(string val)
    {
        GameCurrencyType result = GameCurrencyType.None;
        switch (val)
        {
            case "1":
                result = GameCurrencyType.Cash;
                break;
            case "2":
                result = GameCurrencyType.Voucher;
                break;
            case "3":
                result = GameCurrencyType.Crystal;
                break;
        }
        return result;
    }

    public UpgradeConfig GetUpgradeCfg(XmlElement element)
    {
        UpgradeConfig config = new UpgradeConfig();

        if (element == null)
        {
            UnityEngine.Debug.LogWarning("GetUpgradeCfg: XmlElement is null. Returning default UpgradeConfig.");
            config.base_data = 0f;
            config.max_data = 0f;
            config.ratio = 0f;
            return config;
        }

        string baseStr = element.GetAttribute("base");
        float baseVal;
        if (string.IsNullOrEmpty(baseStr) || !float.TryParse(baseStr, out baseVal))
        {
            UnityEngine.Debug.LogWarning("GetUpgradeCfg: Missing or invalid 'base' attribute in element '" + element.Name + "'. Using default 0.");
            baseVal = 0f;
        }

        string maxStr = element.GetAttribute("max");
        float maxVal;
        if (string.IsNullOrEmpty(maxStr) || !float.TryParse(maxStr, out maxVal))
        {
            UnityEngine.Debug.LogWarning("GetUpgradeCfg: Missing or invalid 'max' attribute in element '" + element.Name + "'. Using base value.");
            maxVal = baseVal;
        }

        config.base_data = baseVal;
        config.max_data = maxVal;
        config.ratio = 0f;

        return config;
    }

    public AIUpgradeConfig GetAIUpgradeCfg(XmlElement element)
    {
        AIUpgradeConfig aIUpgradeConfig = new AIUpgradeConfig();
        aIUpgradeConfig.base_data = float.Parse(element.GetAttribute("base"));
        aIUpgradeConfig.up_factor_boss = float.Parse(element.GetAttribute("upFactorBoss"));
        aIUpgradeConfig.up_factor_daily = float.Parse(element.GetAttribute("upFactorDaily"));
        aIUpgradeConfig.up_factor_normal = float.Parse(element.GetAttribute("upFactorNormal"));
        return aIUpgradeConfig;
    }

    public AvatarType GetAvatarTypeFromCfg(string type_name)
    {
        AvatarType result = AvatarType.None;
        switch (type_name)
        {
            case "Human":
                result = AvatarType.Human;
                break;
            case "Doctor":
                result = AvatarType.Doctor;
                break;
            case "Swat":
                result = AvatarType.Swat;
                break;
            case "Mike":
                result = AvatarType.Mike;
                break;
            case "Armer":
                result = AvatarType.Armer;
                break;
            case "Cowboy":
                result = AvatarType.Cowboy;
                break;
            case "All":
                result = AvatarType.All;
                break;
        }
        return result;
    }

    public EnemyAttackPriority GetEnemyPriorityFromCfg(string type_name)
    {
        EnemyAttackPriority result = EnemyAttackPriority.Player;
        switch (type_name)
        {
            case "Player":
                result = EnemyAttackPriority.Player;
                break;
            case "Hostage":
                result = EnemyAttackPriority.Hostage;
                break;
        }
        return result;
    }

    public Hashtable GetExCfg(XmlElement element)
    {
        Hashtable hashtable = new Hashtable();
        foreach (XmlAttribute attribute in element.Attributes)
        {
            hashtable.Add(attribute.Name, float.Parse(attribute.Value));
        }
        return hashtable;
    }

    public Hashtable GetEnemyExCfg(XmlElement element, EnemyType enemy_type)
    {
        Hashtable hashtable = new Hashtable();
        foreach (XmlAttribute attribute in element.Attributes)
        {
            hashtable.Add(attribute.Name, float.Parse(attribute.Value));
        }
        return hashtable;
    }

    public Hashtable GetWeaponExCfg(XmlElement element, WeaponType weapon_type)
    {
        Hashtable hashtable = new Hashtable();
        foreach (XmlAttribute attribute in element.Attributes)
        {
            hashtable.Add(attribute.Name, float.Parse(attribute.Value));
        }
        if (weapon_type == WeaponType.Saw && element.HasAttribute("skillPara"))
        {
            chaisaw_skill_percent = int.Parse(element.GetAttribute("skillPara"));
        }
        return hashtable;
    }

    public SpawnFromType GetSpawnTypeFromCfg(string type_name)
    {
        SpawnFromType result = SpawnFromType.Grave;
        switch (type_name)
        {
            case "grave":
                result = SpawnFromType.Grave;
                break;
            case "nest":
                result = SpawnFromType.Nest;
                break;
        }
        return result;
    }

    public void LoadEnemyWaveConfig()
    {
        LoadEnemyWaveBossConfig();
        LoadEnemyWaveNpcConfig();
        LoadEnemyWaveNormalConfig();
        LoadEnemyWaveEndlessConfig();
        LoadEnemyWaveEndlessBossConfig();
        LoadEnemyWaveDailyConfig();
        LoadEnemyWaveCrazyDailyConfig();
        LoadEnemyWaveBossCoopConfig();
    }

    public void LoadEnemyWaveBossConfig()
    {
        string xml = LoadConfigFile("EnemyWaveBossCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("WaveBoss"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            EnemyWaveInfo enemyWaveInfo = new EnemyWaveInfo();
            foreach (XmlElement item2 in item.GetElementsByTagName("enemy"))
            {
                EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                enemySpawnInfo.EType = GetEnemyTypeFromCfg(item2.GetAttribute("type"));
                enemySpawnInfo.Count = int.Parse(item2.GetAttribute("count"));
                enemySpawnInfo.From = GetSpawnTypeFromCfg(item2.GetAttribute("spawn"));
                enemyWaveInfo.spawn_info_list.Add(enemySpawnInfo);
            }
            if (EnemyWaveInfo_Boss_Set.ContainsKey(key))
            {
                EnemyWaveInfoList enemyWaveInfoList = EnemyWaveInfo_Boss_Set[key];
                enemyWaveInfoList.wave_info_list.Add(enemyWaveInfo);
            }
            else
            {
                EnemyWaveInfoList enemyWaveInfoList2 = new EnemyWaveInfoList();
                enemyWaveInfoList2.wave_info_list.Add(enemyWaveInfo);
                EnemyWaveInfo_Boss_Set.Add(key, enemyWaveInfoList2);
            }
        }
        XmlElement xmlElement3 = documentElement.GetElementsByTagName("IntervalCfg")[0] as XmlElement;
        EnemyWave_Interval_Boss.wave_interval = float.Parse(xmlElement3.GetAttribute("wave"));
        EnemyWave_Interval_Boss.line_interval = float.Parse(xmlElement3.GetAttribute("line"));
    }

    public void LoadEnemyWaveBossCoopConfig()
    {
        string xml = LoadConfigFile("EnemyWaveBossCoopCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("WaveBoss"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            EnemyWaveInfo enemyWaveInfo = new EnemyWaveInfo();
            foreach (XmlElement item2 in item.GetElementsByTagName("enemy"))
            {
                EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                enemySpawnInfo.EType = GetEnemyTypeFromCfg(item2.GetAttribute("type"));
                enemySpawnInfo.Count = int.Parse(item2.GetAttribute("count"));
                enemySpawnInfo.From = GetSpawnTypeFromCfg(item2.GetAttribute("spawn"));
                enemyWaveInfo.spawn_info_list.Add(enemySpawnInfo);
            }
            if (EnemyWaveInfo_Boss_Coop_Set.ContainsKey(key))
            {
                EnemyWaveInfoList enemyWaveInfoList = EnemyWaveInfo_Boss_Coop_Set[key];
                enemyWaveInfoList.wave_info_list.Add(enemyWaveInfo);
            }
            else
            {
                EnemyWaveInfoList enemyWaveInfoList2 = new EnemyWaveInfoList();
                enemyWaveInfoList2.wave_info_list.Add(enemyWaveInfo);
                EnemyWaveInfo_Boss_Coop_Set.Add(key, enemyWaveInfoList2);
            }
        }
        XmlElement xmlElement3 = documentElement.GetElementsByTagName("IntervalCfg")[0] as XmlElement;
        EnemyWave_Interval_Boss_Coop.wave_interval = float.Parse(xmlElement3.GetAttribute("wave"));
        EnemyWave_Interval_Boss_Coop.line_interval = float.Parse(xmlElement3.GetAttribute("line"));
    }

    public void LoadEnemyWaveNpcConfig()
    {
        string xml = LoadConfigFile("EnemyWaveNpcCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("WaveNpc"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            EnemyWaveInfo enemyWaveInfo = new EnemyWaveInfo();
            foreach (XmlElement item2 in item.GetElementsByTagName("enemy"))
            {
                EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                enemySpawnInfo.EType = GetEnemyTypeFromCfg(item2.GetAttribute("type"));
                enemySpawnInfo.Count = int.Parse(item2.GetAttribute("count"));
                enemySpawnInfo.From = GetSpawnTypeFromCfg(item2.GetAttribute("spawn"));
                enemyWaveInfo.spawn_info_list.Add(enemySpawnInfo);
            }
            if (EnemyWaveInfo_Npc_Set.ContainsKey(key))
            {
                EnemyWaveInfoList enemyWaveInfoList = EnemyWaveInfo_Npc_Set[key];
                enemyWaveInfoList.wave_info_list.Add(enemyWaveInfo);
            }
            else
            {
                EnemyWaveInfoList enemyWaveInfoList2 = new EnemyWaveInfoList();
                enemyWaveInfoList2.wave_info_list.Add(enemyWaveInfo);
                EnemyWaveInfo_Npc_Set.Add(key, enemyWaveInfoList2);
            }
        }
        XmlElement xmlElement3 = documentElement.GetElementsByTagName("IntervalCfg")[0] as XmlElement;
        EnemyWave_Interval_Npc.wave_interval = float.Parse(xmlElement3.GetAttribute("wave"));
        EnemyWave_Interval_Npc.line_interval = float.Parse(xmlElement3.GetAttribute("line"));
    }

    public void LoadEnemyWaveNormalConfig()
    {
        string xml = LoadConfigFile("EnemyWaveNormalCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("WaveNormal"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            EnemyWaveInfo enemyWaveInfo = new EnemyWaveInfo();
            foreach (XmlElement item2 in item.GetElementsByTagName("enemy"))
            {
                EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                enemySpawnInfo.EType = GetEnemyTypeFromCfg(item2.GetAttribute("type"));
                enemySpawnInfo.Count = int.Parse(item2.GetAttribute("count"));
                enemySpawnInfo.From = GetSpawnTypeFromCfg(item2.GetAttribute("spawn"));
                enemyWaveInfo.spawn_info_list.Add(enemySpawnInfo);
            }
            if (EnemyWaveInfo_Normal_Set.ContainsKey(key))
            {
                EnemyWaveInfoList enemyWaveInfoList = EnemyWaveInfo_Normal_Set[key];
                enemyWaveInfoList.wave_info_list.Add(enemyWaveInfo);
            }
            else
            {
                EnemyWaveInfoList enemyWaveInfoList2 = new EnemyWaveInfoList();
                enemyWaveInfoList2.wave_info_list.Add(enemyWaveInfo);
                EnemyWaveInfo_Normal_Set.Add(key, enemyWaveInfoList2);
            }
        }
        XmlElement xmlElement3 = documentElement.GetElementsByTagName("IntervalCfg")[0] as XmlElement;
        EnemyWave_Interval_Normal.wave_interval = float.Parse(xmlElement3.GetAttribute("wave"));
        EnemyWave_Interval_Normal.line_interval = float.Parse(xmlElement3.GetAttribute("line"));
    }

    public void LoadEnemyWaveEndlessConfig()
    {
        string xml = LoadConfigFile("EnemyWaveEndlessCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("WaveEndless"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            EnemyWaveInfo enemyWaveInfo = new EnemyWaveInfo();
            foreach (XmlElement item2 in item.GetElementsByTagName("enemy"))
            {
                EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                enemySpawnInfo.EType = GetEnemyTypeFromCfg(item2.GetAttribute("type"));
                enemySpawnInfo.Count = int.Parse(item2.GetAttribute("count"));
                enemySpawnInfo.From = GetSpawnTypeFromCfg(item2.GetAttribute("spawntype"));
                enemyWaveInfo.spawn_info_list.Add(enemySpawnInfo);
            }
            if (EnemyWaveInfo_Endless_Set.ContainsKey(key))
            {
                EnemyWaveInfoList enemyWaveInfoList = EnemyWaveInfo_Endless_Set[key];
                enemyWaveInfoList.wave_info_list.Add(enemyWaveInfo);
            }
            else
            {
                EnemyWaveInfoList enemyWaveInfoList2 = new EnemyWaveInfoList();
                enemyWaveInfoList2.wave_info_list.Add(enemyWaveInfo);
                EnemyWaveInfo_Endless_Set.Add(key, enemyWaveInfoList2);
            }
        }
        XmlElement xmlElement3 = documentElement.GetElementsByTagName("IntervalCfg")[0] as XmlElement;
        EnemyWave_Interval_Normal.wave_interval = float.Parse(xmlElement3.GetAttribute("wave"));
        EnemyWave_Interval_Normal.line_interval = float.Parse(xmlElement3.GetAttribute("line"));
    }

    public void LoadEnemyWaveEndlessBossConfig()
    {
        Debug.Log("[Config] Loading Endless Boss Config");

        string xml = LoadConfigFile("EnemyWaveEndlessBossCfg");
        if (string.IsNullOrEmpty(xml))
        {
            Debug.LogError("[Config] Endless Boss Config XML is empty or missing!");
            return;
        }

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;


        foreach (XmlElement item in documentElement.GetElementsByTagName("WaveEndlessBoss"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            EnemyWaveInfo enemyWaveInfo = new EnemyWaveInfo();

            foreach (XmlElement item2 in item.GetElementsByTagName("enemy"))
            {
                EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                enemySpawnInfo.EType = GetEnemyTypeFromCfg(item2.GetAttribute("type"));
                enemySpawnInfo.Count = int.Parse(item2.GetAttribute("count"));
                enemySpawnInfo.From = GetSpawnTypeFromCfg(item2.GetAttribute("spawntype"));
                enemyWaveInfo.spawn_info_list.Add(enemySpawnInfo);
            }

            if (EnemyWaveInfo_Endless_Bosses.ContainsKey(key))
            {
                EnemyWaveInfoList enemyWaveInfoList = EnemyWaveInfo_Endless_Bosses[key];
                enemyWaveInfoList.wave_info_list.Add(enemyWaveInfo);
            }
            else
            {
                EnemyWaveInfoList enemyWaveInfoList2 = new EnemyWaveInfoList();
                enemyWaveInfoList2.wave_info_list.Add(enemyWaveInfo);
                EnemyWaveInfo_Endless_Bosses.Add(key, enemyWaveInfoList2);
            }
        }

        XmlElement xmlElement3 = documentElement.GetElementsByTagName("IntervalCfg")[0] as XmlElement;
        EnemyWave_Interval_Boss.wave_interval = float.Parse(xmlElement3.GetAttribute("wave"));
        EnemyWave_Interval_Boss.line_interval = float.Parse(xmlElement3.GetAttribute("line"));
    }

    public void LoadEnemyWaveDailyConfig()
    {
        string xml = LoadConfigFile("EnemyWaveDailyCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;
        foreach (XmlElement item in documentElement.GetElementsByTagName("DailyWave"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            EnemyWaveInfo enemyWaveInfo = new EnemyWaveInfo();
            foreach (XmlElement item2 in item.GetElementsByTagName("enemy"))
            {
                EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                enemySpawnInfo.EType = GetEnemyTypeFromCfg(item2.GetAttribute("type"));
                enemySpawnInfo.Count = int.Parse(item2.GetAttribute("count"));
                enemySpawnInfo.From = GetSpawnTypeFromCfg(item2.GetAttribute("spawn"));
                enemyWaveInfo.spawn_info_list.Add(enemySpawnInfo);
            }
            if (EnemyWaveInfo_Daily_Set.ContainsKey(key))
            {
                EnemyWaveInfoList enemyWaveInfoList = EnemyWaveInfo_Daily_Set[key];
                enemyWaveInfoList.wave_info_list.Add(enemyWaveInfo);
            }
            else
            {
                EnemyWaveInfoList enemyWaveInfoList2 = new EnemyWaveInfoList();
                enemyWaveInfoList2.wave_info_list.Add(enemyWaveInfo);
                EnemyWaveInfo_Daily_Set.Add(key, enemyWaveInfoList2);
            }
        }
        XmlElement xmlElement3 = documentElement.GetElementsByTagName("IntervalCfg")[0] as XmlElement;
        EnemyWave_Interval_Daily.wave_interval = float.Parse(xmlElement3.GetAttribute("wave"));
        EnemyWave_Interval_Daily.line_interval = float.Parse(xmlElement3.GetAttribute("line"));
    }

    public void LoadEnemyWaveCrazyDailyConfig()
    {
        string xml = LoadConfigFile("EnemyWaveCrazyDailyCfg");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlElement documentElement = xmlDocument.DocumentElement;

        foreach (XmlElement item in documentElement.GetElementsByTagName("DailyWave"))
        {
            int key = int.Parse(item.GetAttribute("level"));
            EnemyWaveInfo enemyWaveInfo = new EnemyWaveInfo();

            foreach (XmlElement item2 in item.GetElementsByTagName("enemy"))
            {
                EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
                enemySpawnInfo.EType = GetEnemyTypeFromCfg(item2.GetAttribute("type"));
                enemySpawnInfo.Count = int.Parse(item2.GetAttribute("count"));
                enemySpawnInfo.From = GetSpawnTypeFromCfg(item2.GetAttribute("spawn"));
                enemyWaveInfo.spawn_info_list.Add(enemySpawnInfo);
            }

            if (EnemyWaveInfo_CrazyDaily_Set.ContainsKey(key))
            {
                EnemyWaveInfoList list = EnemyWaveInfo_CrazyDaily_Set[key];
                list.wave_info_list.Add(enemyWaveInfo);
            }
            else
            {
                EnemyWaveInfoList list = new EnemyWaveInfoList();
                list.wave_info_list.Add(enemyWaveInfo);
                EnemyWaveInfo_CrazyDaily_Set.Add(key, list);
            }
        }

        XmlElement interval = documentElement.GetElementsByTagName("IntervalCfg")[0] as XmlElement;
        EnemyWave_Interval_CrazyDaily.wave_interval = float.Parse(interval.GetAttribute("wave"));
        EnemyWave_Interval_CrazyDaily.line_interval = float.Parse(interval.GetAttribute("line"));
    }

    public float GetAvatarUpBattleTime(int avatar_lv)
    {
        int num = 0;
        foreach (int key in Avatar_Up_Info_Set.Keys)
        {
            if (avatar_lv >= key)
            {
                num = key;
            }
        }
        return Avatar_Up_Info_Set[num].ParaA + Avatar_Up_Info_Set[num].ParaB * (float)(avatar_lv - num);
    }

    public float GetStandaryEnemyCount(int lv)
    {
        int num = 0;
        foreach (int key in Standary_Enemy_Count_Set.Keys)
        {
            if (lv >= key)
            {
                num = key;
            }
        }
        return Standary_Enemy_Count_Set[num].base_val + Standary_Enemy_Count_Set[num].para * (float)(lv - num);
    }

    public string LoadConfigFile(string file)
    {
        if (!Config_Version_Set.ContainsKey(file))
        {
            Config_Version_Set.Add(file, "2.1.2");
        }
        if (!Remote_Config_Version_Set.ContainsKey(file))
        {
            Remote_Config_Version_Set.Add(file, "2.1.2");
        }
        string content = string.Empty;
        if (GameDefine.LOAD_CONFIG_SAVE_PATH && IsEditorMode())
        {
            string md5String = MD5Sample.GetMd5String(file);
            Utils.FileReadString(Utils.SavePath() + "Config/" + md5String + ".bytes", ref content);
            Debug.Log(content);
            content = XXTEAUtils.Decrypt(content, GameData.GetImportContent());
            content = GameData.Encipher(content);
            Debug.Log(content);
            return content;
        }
        if (GameDefine.LOAD_CONFIG_DATA_PATH && IsEditorMode())
        {
            Utils.FileReadString(Utils.DataPath() + "Config/" + file + ".xml", ref content);
            return content;
        }
        if (!IsEditorMode())
        {
            string md5String2 = MD5Sample.GetMd5String(file);
            if (!force_update_local && Utils.FileReadString(Utils.SavePath() + md5String2 + ".bytes", ref content))
            {
                content = XXTEAUtils.Decrypt(content, GameData.GetImportContent());
                content = GameData.Encipher(content);
            }
            else
            {
                content = Utils.LoadResourcesFileForText("Config/" + file);
                OutputCfgFile(file, content);
                Config_Version_Set[file] = "2.1.2";
                Remote_Config_Version_Set[file] = "2.1.2";
            }
        }
        else
        {
            content = Utils.LoadResourcesFileForText("Config/" + file);
        }
        if (GameDefine.IS_CONFIG_OUTPUT)
        {
            string text = GameData.Encipher(content);
            text = XXTEAUtils.Encrypt(text, GameData.GetImportContent());
            string md5String3 = MD5Sample.GetMd5String(file);
            if (!Directory.Exists(Utils.SavePath() + "Config/"))
            {
                Directory.CreateDirectory(Utils.SavePath() + "Config/");
            }
            Utils.FileWriteString(Utils.SavePath() + "Config/" + md5String3 + ".bytes", text);
        }
        return content;
    }

    private void OutputCfgFile(string file, string content)
    {
        string text = GameData.Encipher(content);
        text = XXTEAUtils.Encrypt(text, GameData.GetImportContent());
        string md5String = MD5Sample.GetMd5String(file);
        Utils.FileWriteString(Utils.SavePath() + md5String + ".bytes", text);
    }

    public List<WeaponFragmentProbsCfg> GetWeaponFragmentProb(string weapon_name)
    {
        List<WeaponFragmentProbsCfg> list = new List<WeaponFragmentProbsCfg>();
        GameProbsCfg gameProbsCfg = null;
        foreach (string key in ProbsConfig_Set.Keys)
        {
            gameProbsCfg = ProbsConfig_Set[key];
            if (gameProbsCfg.prob_type == GameProbsCfg.ProbType.WeaponFragment)
            {
                WeaponFragmentProbsCfg weaponFragmentProbsCfg = gameProbsCfg as WeaponFragmentProbsCfg;
                if (weaponFragmentProbsCfg.weapon_name == weapon_name)
                {
                    list.Add(weaponFragmentProbsCfg);
                }
            }
        }
        return list;
    }

    public static bool IsEditorMode()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return false;
        }
        return true;
    }

    public static EnemyType GetEnemyTypeFromBossType(CoopBossType boss_type)
    {
        EnemyType result = EnemyType.E_NONE;
        switch (boss_type)
        {
            case CoopBossType.E_FATCOOK:
                result = EnemyType.E_FATCOOK;
                break;
            case CoopBossType.E_FATCOOK_E:
                result = EnemyType.E_FATCOOK_E;
                break;
            case CoopBossType.E_HAOKE_A:
                result = EnemyType.E_HAOKE_A;
                break;
            case CoopBossType.E_HAOKE_B:
                result = EnemyType.E_HAOKE_B;
                break;
            case CoopBossType.E_WRESTLER:
                result = EnemyType.E_WRESTLER;
                break;
            case CoopBossType.E_WRESTLER_E:
                result = EnemyType.E_WRESTLER_E;
                break;
            case CoopBossType.E_HALLOWEEN:
                result = EnemyType.E_HALLOWEEN;
                break;
            case CoopBossType.E_HALLOWEEN_E:
                result = EnemyType.E_HALLOWEEN_E;
                break;
            case CoopBossType.E_SHARK:
                result = EnemyType.E_SHARK;
                break;
            case CoopBossType.E_SHARK_E:
                result = EnemyType.E_SHARK_E;
                break;
        }
        return result;
    }

    public List<GameAwardItem> GetLotteryAwardWithLevel(int level)
    {
        List<GameAwardItem> list = new List<GameAwardItem>();
        foreach (GameAwardItem value in Lottery_AwardItem_Set.Values)
        {
            if (value.award_level == level)
            {
                list.Add(value);
            }
        }
        return list;
    }
}
