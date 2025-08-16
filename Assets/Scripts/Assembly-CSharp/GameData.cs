using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Boomlagoon.JSON;
using CoMZ2;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class GameData : MonoBehaviour
{
    public enum DailyMissionStatus
    {
        Disenable,
        Free,
        CrystalEnable,
        CrystalDisenable
    }

    public enum GamePlayType
    {
        Normal,
        Coop
    }

    public enum GameSceneState
    {
        None,
        Shop,
        Gaming
    }

    public DateTime last_checked_date_now;

    public DateTime next_cd_date;

    public DateTime lastLoginDate = DateTime.MinValue;

    public string cur_save_date = string.Empty;

    public string save_date = string.Empty;

    private string loadedSavePath;

    public int daily_mission_count;

    public bool didResetSave = false;

    public QuestInfo EndlessQuestInfo;

    public int lottery_reset_count;

    private int lastSavedFreeSpinCount = 0;

    private const int FreeSpinCap = 10;

    private bool hasLoadedData = false;

    private bool hasAttemptedLoad = false;

    public bool justReceivedLotteryReward = false;

    public int lottery_count;

    public bool needsUpdate = false;

    public bool showNicknamePrompt = true;

    public bool blackname = false;

    public int last_saved_free_spin_count = -1;

    public bool hasEnteredLottery = false;

    private bool isResetting = false;

    public bool daily_mode_enable;

    public bool isNewSave;

    public GameDataInt free_lottery_spins = new GameDataInt(0, GameDataIntPurpose.FreeSpin);

    public bool is_crazy_daily;

    private HashSet<string> usedPromoCodes = new HashSet<string>();

    public string lastMissionCompleteDate = "";

    public int lastSavedCash = 0;

    public int lastSavedCrystal = 0;

    public int lastSavedVoucher = 0;

    private DateTime lastSaveTime = DateTime.MinValue;

    public int suspiciousSaveCount = 0;

    private readonly TimeSpan rapidSaveThreshold = TimeSpan.FromSeconds(8);
    private readonly int maxSuspiciousSaves = 6;

    public static DateTime lastLegitActionTime = DateTime.MinValue;

    public static TimeSpan legitActionGracePeriod = TimeSpan.FromSeconds(5);

    private GameDataInt _total_cash = new GameDataInt(0);

    private GameDataInt _total_crystal = new GameDataInt(0);

    private GameDataInt _total_voucher = new GameDataInt(0);

    private Queue<DateTime> recentSaveTimestamps = new Queue<DateTime>();

    private const int MaxSavesInWindow = 6;

    private readonly TimeSpan SaveWindow = TimeSpan.FromSeconds(3);


    public MissionType mission_type
    {
        get
        {
            if (GameSceneController.Instance != null)
                return GameSceneController.Instance.mission_type;

            return MissionType.None;
        }
    }

    public int tapjoyPoints;

    public int day_level;

    public const int DEFAULT_FREE_SPIN_COUNT = -1;

    public DateTime maxDateReached;

    public AvatarType cur_avatar = AvatarType.None;

    public Dictionary<string, WeaponData> WeaponData_Set = new Dictionary<string, WeaponData>();

    public Dictionary<AvatarType, AvatarData> AvatarData_Set = new Dictionary<AvatarType, AvatarData>();

    public Dictionary<string, GameProb> GameStoryProbs_Set = new Dictionary<string, GameProb>();

    public Dictionary<string, GameProb> WeaponFragmentProbs_Set = new Dictionary<string, GameProb>();

    public Dictionary<string, GameProb> WeaponIntensifierProbs_Set = new Dictionary<string, GameProb>();

    public Dictionary<string, string> ConfigVersion_Set = new Dictionary<string, string>();

    public Dictionary<string, int> Enemy_Loading_Set = new Dictionary<string, int>();

    public Dictionary<string, string> Iap_failed_info = new Dictionary<string, string>();

    public int Iap_failed_info_count;

    public GameSceneState scene_state;

    private bool timeWentBackwards = false;

    public string loading_to_scene = "test_new1";

    public DateTime lastResetDate = DateTime.MinValue;

    public DateTime lastResetSystemTime = DateTime.MinValue;

    public DateTime lastRollbackDate = DateTime.MinValue;

    public QuestInfo cur_quest_info;

    private int _free_lottery_spins = 0;

    private Configure configure;

    public Action reset_nist_time_error;

    public Action reset_nist_time_finish;

    public float sensitivity_ratio = 0.4f;

    private bool loadedFromLegacySave = false;

    public DateTime lastLotteryAwardTime = DateTime.MinValue;

    public int lastSavedFreeSpins = 0;

    private int sessionCashSpent = 0;

    private int sessionCrystalSpent = 0;

    private int sessionVoucherSpent = 0;

    public bool is_enter_tutorial = false;

    public bool show_ui_tutorial = true;

    public bool show_ui_tutorial_weapon = true;

    public bool is_daily_cd_crystal;

    protected string user_id = string.Empty;

    public string NickName = string.Empty;

    public int FightStrength;

    private bool isLoading = false;

    private bool isLoaded = false;

    private bool isSaving = false;

    public bool rewardSafeMode = false;

    public string game_version = "";

    public int enter_shop_count;

    public bool isTesterSave = false;

    public string timeserver_url = "http://72.167.165.221:7600/gameapi/GameCommonNo.do?action=groovy&json=%7B%22cmd%22:%22GetServerTime%22%7D";

    public string statistics_url = "http://208.109.176.89:8081/gameapi/turboPlatform2.do?action=logAllInfo&json=";

    public string iap_check_url = "http://192.225.224.97:7600/gameapi/GameCommon.do?action=groovy&json=";

    public string redeem_get_url = "http://184.168.72.188:9218/gameapi/comzb.do?action=comzb/GetGiftPackage&json=";

    public string redeem_accept_url = "http://184.168.72.188:9218/gameapi/comzb.do?action=comzb/AcceptGiftPackage&json=";

    public float redeem_change_ratio = 0.2f;

    protected DateTime finally_save_date;

    public bool TRINITI_IAP_CEHCK;

    public List<UnlockInGame> UnlockList = new List<UnlockInGame>();

    public Dictionary<string, SkillData> Skill_Avatar_Set = new Dictionary<string, SkillData>();

    public bool showUpdatePoster;

    public readonly GameDataInt total_cash = new GameDataInt(0, GameDataIntPurpose.Cash);

    public readonly GameDataInt total_crystal = new GameDataInt(0, GameDataIntPurpose.Crystal);

    public readonly GameDataInt total_voucher = new GameDataInt(0, GameDataIntPurpose.Voucher);

    public GamePlayType cur_game_type;

    public CoopBossType cur_coop_boss = CoopBossType.E_NONE;

    public List<string> lottery_seat_state = new List<string>();

    public static GameData Instance { get; private set; }

    public string UserId
    {
        get
        {
            return user_id;
        }
    }

    private void Awake()
    {
        if (GameObject.Find("_AndroidPlatform") == null)
        {
            GameObject gameObject = new GameObject("_AndroidPlatform");
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
            DontDestroyOnLoad(gameObject);
            DevicePlugin.InitAndroidPlatform();
            gameObject.AddComponent<TrinitiAdAndroidPlugin>();
        }
        Instance = this;
        DontDestroyOnLoad(base.gameObject);
        finally_save_date = GetCurDateTime();
        TRINITI_IAP_CEHCK = true;
    }

    public void Init()
    {
        daily_mission_count = 0;
        day_level = 1;
        total_cash.SetIntVal(0, GameDataIntPurpose.Cash);
        total_crystal.SetIntVal(0, GameDataIntPurpose.Crystal);
        total_voucher.SetIntVal(0, GameDataIntPurpose.Voucher);
        cur_avatar = AvatarType.Human;

        foreach (AvatarConfig value in GameConfig.Instance.AvatarConfig_Set.Values)
        {
            AvatarData avatarData = new AvatarData();
            avatarData.show_name = value.show_name;
            avatarData.avatar_name = value.avatar_name;
            avatarData.avatar_type = value.avatar_type;
            avatarData.config = value;
            avatarData.level = 1;
            avatarData.cur_exp = new GameDataInt(0);
            avatarData.exist_state = value.exist_state;
            avatarData.primary_equipment = "MP5";
            for (int i = 0; i < 2; i++)
            {
                avatarData.skill_list.Add(i == 0 ? value.first_skill : "null");
            }
            AvatarData_Set[avatarData.avatar_type] = avatarData;
        }

        foreach (SkillConfig value2 in GameConfig.Instance.Skill_Avatar_Set.Values)
        {
            SkillData skillData = new SkillData();
            skillData.skill_name = value2.skill_name;
            skillData.level = 1;
            skillData.exist_state = value2.exist_state;
            skillData.config = value2;
            skillData.ResetData();
            Skill_Avatar_Set[skillData.skill_name] = skillData;
        }

        foreach (WeaponConfig value3 in GameConfig.Instance.WeaponConfig_Set.Values)
        {
            WeaponData weaponData = new WeaponData();
            weaponData.weapon_name = value3.weapon_name;
            weaponData.weapon_type = value3.wType;
            weaponData.is_secondary = value3.is_secondary;
            weaponData.owner = value3.owner;
            weaponData.config = value3;
            weaponData.total_bullet_count = value3.initBullet;
            weaponData.level = 1;
            weaponData.exist_state = value3.exist_state;
            WeaponData_Set[weaponData.weapon_name] = weaponData;
        }

        foreach (string value4 in GameConfig.Instance.Enemy_Loading_Set.Values)
        {
            Enemy_Loading_Set[value4] = 0;
        }

        for (int j = 0; j < 14; j++)
        {
            lottery_seat_state.Add("null");
        }

        cur_quest_info = new QuestInfo();
        cur_quest_info.mission_type = MissionType.Cleaner;

        user_id = DevicePlugin.GetUUID();

        if (!LoadData())
        {
            Debug.Log("Save failed to load. Initializing new save.");

            isNewSave = true;

            // Always start tutorial on brand new save or after reset
            is_enter_tutorial = true;

            if (Application.platform == RuntimePlatform.IPhonePlayer && Screen.height >= 700)
            {
                sensitivity_ratio = 0.6f;
            }

            SaveData();
            UploadStatistics("FirstTime", new Hashtable());
            showUpdatePoster = true;
        }
        else
        {
            // If save loaded successfully, check if reset flag is set
            if (didResetSave)
            {
                Debug.Log("Detected save reset flag. Triggering tutorial.");

                is_enter_tutorial = true;
                didResetSave = false;   // Clear the flag so tutorial only runs once
                SaveData();            // Save flag clear immediately
            }
        }

        // Set the game version AFTER attempting to load
        game_version = VersionHelper.LoadVersionFromFile();

        GameEnhancer enhancer = GameObject.FindObjectOfType(typeof(GameEnhancer)) as GameEnhancer;

        if (!isNewSave)
        {
            string latestVersion = VersionHelper.LoadVersionFromFile();
            Debug.Log("Comparing versions: '" + game_version + "' vs '" + latestVersion + "'");

            if (game_version != latestVersion)
            {
                Debug.Log("Version mismatch detected. Forcing update.");
                game_version = latestVersion;
                OnGameDataVersionDifferent();
                showUpdatePoster = true;
            }
        }

        foreach (WeaponData value5 in WeaponData_Set.Values)
        {
            if (value5.exist_state == WeaponExistState.Locked &&
                value5.config.unlockDay >= 0 &&
                value5.config.unlockDay < day_level)
            {
                value5.Unlock();
            }
            value5.ResetData();
        }

        foreach (AvatarData value6 in AvatarData_Set.Values)
        {
            if (value6.exist_state == AvatarExistState.Locked &&
                value6.config.unlockDay < day_level)
            {
                value6.Unlock();
            }
            value6.ResetData();
        }
    }

    public static void CheckGameData()
    {
        if (!GameObject.Find("GameData"))
        {
            GameObject gameObject = Instantiate(Resources.Load<GameObject>("GameData"));
            gameObject.name = "GameData";
            Instance.Init();
            TapJoyScript.CreateTapjoyObj();
        }
    }

    public void SaveData()
    {
        if (isResetting)
        {
            Debug.LogWarning("[SaveData] Reset already in progress, skipping SaveData to prevent recursion.");
            return;
        }

        if (blackname == true)
        {
            Debug.LogWarning("Suspicious save detected.");
            isResetting = true;
            MenuAudioController.DestroyGameMenuAudio();
            Instance.didResetSave = true;
            Instance.Init();
            Instance.blackname = false;

            isResetting = false;  // Reset here before reload

            SceneManager.LoadScene("InitScene");
            return;
        }

        if (isSaving)
        {
            Debug.LogWarning("SaveData is already running, skipping duplicate call.");
            return;
        }
        isSaving = true;

        try
        {
            Configure configure = new Configure();
            configure.AddSection("Save", string.Empty, string.Empty);

            // === CORE VALUES ===
            configure.AddValueSingle("Save", "IAPResend_android", Iap_Resend.Get_IAP_android_list(), "", "");
            configure.AddValueSingle("Save", "Cash", total_cash.ToString(), "", "");
            configure.AddValueSingle("Save", "Crystal", total_crystal.ToString(), "", "");
            configure.AddValueSingle("Save", "Voucher", total_voucher.ToString(), "", "");
            configure.AddValueSingle("Save", "TapjoyPoints", tapjoyPoints.ToString(), "", "");
            configure.AddValueSingle("Save", "DayLevel", day_level.ToString(), "", "");
            configure.AddValueSingle("Save", "AvatarType", ((int)cur_avatar).ToString(), "", "");
            configure.AddValueSingle("Save", "SensitivityRatio", sensitivity_ratio.ToString(), "", "");
            configure.AddValueSingle("Save", "Version", game_version, "", "");
            configure.AddValueSingle("Save", "EnterShopCount", enter_shop_count.ToString(), "", "");
            configure.AddValueSingle("Save", "TimeserverUrl", timeserver_url, "", "");
            configure.AddValueSingle("Save", "StatisticsUrl", statistics_url, "", "");
            configure.AddValueSingle("Save", "IapUrl", iap_check_url, "", "");
            configure.AddValueSingle("Save", "RedeemGetUrl", redeem_get_url, "", "");
            configure.AddValueSingle("Save", "RedeemAcceptUrl", redeem_accept_url, "", "");
            configure.AddValueSingle("Save", "RedeemAwardRatio", redeem_change_ratio.ToString(), "", "");
            configure.AddValueSingle("Save", "NickName", NickName, "", "");
            configure.AddValueSingle("Save", "ShowNicknamePrompt", showNicknamePrompt ? "0" : "1", "", "");
            configure.AddValueSingle("Save", "Blackname", blackname ? "1" : "0", "", "");
            configure.AddValueSingle("Save", "EnterTutorial", is_enter_tutorial ? "1" : "0", "", "");
            configure.AddValueSingle("Save", "ShowUITutorial", show_ui_tutorial ? "1" : "0", "", "");
            configure.AddValueSingle("Save", "ShowUITutorialWeapon", show_ui_tutorial_weapon ? "1" : "0", "", "");
            configure.AddValueSingle("Save", "IapCheck", TRINITI_IAP_CEHCK ? "1" : "0", "", "");
            configure.AddValueSingle("Save", "NeedsUpdate", needsUpdate ? "1" : "0", "", "");
            configure.AddValueSingle("Save", "LastResetDate", lastResetDate.ToString("o"), "", "");
            configure.AddValueSingle("Save", "LastResetSystemTime", lastResetSystemTime.ToString("o"), "", "");
            configure.AddValueSingle("Save", "MaxDateReached", maxDateReached.ToString("o"), "", "");
            configure.AddValueSingle("Save", "LastSafeCash", lastSavedCash.ToString(), "", "");
            configure.AddValueSingle("Save", "LastSafeCrystal", lastSavedCrystal.ToString(), "", "");
            configure.AddValueSingle("Save", "LastSafeVoucher", lastSavedVoucher.ToString(), "", "");
            configure.AddValueSingle("Save", "LastSavedFreeSpins", lastSavedFreeSpins.ToString(), "", "");
            configure.AddValueSingle("Save", "DailyMissionCount", daily_mission_count.ToString(), "", "");
            configure.AddValueSingle("Save", "LastMissionCompleteDate", lastMissionCompleteDate, "", "");

            string dataString = total_cash + "|" + total_crystal + "|" + total_voucher + "|" +
                                tapjoyPoints + "|" + day_level + "|" + enter_shop_count + "|" +
                                lastSavedCash + "|" + lastSavedCrystal + "|" + lastSavedVoucher;
            string dataHash = GenerateDataHash(configure);
            configure.AddValueSingle("Save", "DataHash", dataHash, "", "");

            // === USED PROMO CODES ===
            string usedPromoCodesJoined = string.Join(",", new List<string>(usedPromoCodes).ToArray());
            configure.AddValueSingle("Save", "UsedPromoCodes", usedPromoCodesJoined, "", "");

            // === SAVE DATE ===
            finally_save_date = GetCurDateTime();
            configure.AddValueSingle("Save", "SaveDate", finally_save_date.ToString(), "", "");

            // === WEAPON DATA ===
            ArrayList arrayList = new ArrayList();
            foreach (WeaponData value in WeaponData_Set.Values)
            {
                StringLine line = new StringLine();
                line.AddString(value.weapon_name);
                line.AddString(((int)value.weapon_type).ToString());
                line.AddString(value.level.ToString());
                line.AddString(value.total_bullet_count.ToString());
                line.AddString(((int)value.exist_state).ToString());
                line.AddString(value.damage_level.ToString());
                line.AddString(value.frequency_level.ToString());
                line.AddString(value.clip_level.ToString());
                line.AddString(value.range_level.ToString());
                line.AddString(value.stretch_level.ToString());
                arrayList.Add(line.content);
            }
            configure.AddValueArray2("Save", "WeaponsData", arrayList, "", "");
            configure.AddValueSingle("Save", "WeaponsDataCount", arrayList.Count.ToString(), "", "");

            // === AVATAR DATA ===
            ArrayList arrayList2 = new ArrayList();
            foreach (AvatarData value2 in AvatarData_Set.Values)
            {
                StringLine line2 = new StringLine();
                line2.AddString(value2.avatar_name);
                line2.AddString(((int)value2.avatar_type).ToString());
                line2.AddString("null");
                line2.AddString(value2.level.ToString());
                line2.AddString(value2.armor_level.ToString());
                line2.AddString(value2.cur_exp.ToString());
                line2.AddString(((int)value2.exist_state).ToString());
                line2.AddString(value2.primary_equipment);
                line2.AddString(value2.hp_level.ToString());
                line2.AddString(value2.damage_level.ToString());
                foreach (string item in value2.skill_list)
                {
                    line2.AddString(item);
                }
                arrayList2.Add(line2.content);
            }
            configure.AddValueArray2("Save", "AvatarsData", arrayList2, "", "");
            configure.AddValueSingle("Save", "AvatarsDataCount", arrayList2.Count.ToString(), "", "");

            // === SKILL DATA ===
            ArrayList arrayList3 = new ArrayList();
            foreach (SkillData value3 in Skill_Avatar_Set.Values)
            {
                StringLine line3 = new StringLine();
                line3.AddString(value3.skill_name);
                line3.AddString(value3.level.ToString());
                line3.AddString(((int)value3.exist_state).ToString());
                arrayList3.Add(line3.content);
            }
            configure.AddValueArray2("Save", "SkillAvatarData", arrayList3, "", "");
            configure.AddValueSingle("Save", "SkillAvatarDataCount", arrayList3.Count.ToString(), "", "");

            // === STORY PROB DATA ===
            ArrayList arrayList4 = new ArrayList();
            foreach (string key in GameStoryProbs_Set.Keys)
            {
                StringLine line4 = new StringLine();
                line4.AddString(key);
                line4.AddString(GameStoryProbs_Set[key].count.ToString());
                arrayList4.Add(line4.content);
            }
            configure.AddValueArray2("Save", "StoryProbData", arrayList4, "", "");
            configure.AddValueSingle("Save", "StoryProbDataCount", arrayList4.Count.ToString(), "", "");

            // === FRAGMENT PROB DATA ===
            ArrayList arrayList5 = new ArrayList();
            foreach (string key2 in WeaponFragmentProbs_Set.Keys)
            {
                StringLine line5 = new StringLine();
                line5.AddString(key2);
                line5.AddString(WeaponFragmentProbs_Set[key2].count.ToString());
                arrayList5.Add(line5.content);
            }
            configure.AddValueArray2("Save", "FragmentProbData", arrayList5, "", "");
            configure.AddValueSingle("Save", "FragmentProbDataCount", arrayList5.Count.ToString(), "", "");

            // === INTENSIFIER PROB DATA ===
            ArrayList arrayList6 = new ArrayList();
            foreach (string key3 in WeaponIntensifierProbs_Set.Keys)
            {
                StringLine line6 = new StringLine();
                line6.AddString(key3);
                line6.AddString(WeaponIntensifierProbs_Set[key3].count.ToString());
                arrayList6.Add(line6.content);
            }
            configure.AddValueArray2("Save", "IntensifierProbData", arrayList6, "", "");
            configure.AddValueSingle("Save", "IntensifierProbDataCount", arrayList6.Count.ToString(), "", "");

            // === ENEMY LOAD DATA ===
            ArrayList arrayList7 = new ArrayList();
            foreach (string key4 in Enemy_Loading_Set.Keys)
            {
                StringLine line7 = new StringLine();
                line7.AddString(key4);
                line7.AddString(Enemy_Loading_Set[key4].ToString());
                arrayList7.Add(line7.content);
            }
            configure.AddValueArray2("Save", "EnemyLoadData", arrayList7, "", "");
            configure.AddValueSingle("Save", "EnemyLoadDataCount", arrayList7.Count.ToString(), "", "");

            // === LOTTERY SEAT DATA ===
            StringLine line8 = new StringLine();
            foreach (string item2 in lottery_seat_state)
            {
                line8.AddString(item2);
            }
            configure.AddValueArray("Save", "LotterySeatData", line8.content, "", "");
            configure.AddValueSingle("Save", "LotterySeatCount", lottery_seat_state.Count.ToString(), "", "");

            // === CONFIG VERSION DATA ===
            ArrayList arrayList8 = new ArrayList();
            foreach (string key5 in GameConfig.Instance.Config_Version_Set.Keys)
            {
                StringLine line9 = new StringLine();
                line9.AddString(key5);
                line9.AddString(GameConfig.Instance.Config_Version_Set[key5]);
                arrayList8.Add(line9.content);
            }
            configure.AddValueArray2("Save", "ConfigVersion", arrayList8, "", "");
            configure.AddValueSingle("Save", "ConfigVersionCount", arrayList8.Count.ToString(), "", "");

            // === IAP FAILED INFO ===
            /*configure.AddValueSingle("Save", "IapFailedCount", Iap_failed_info.Count.ToString(), "", "");

            if (Iap_failed_info.Count > 0)
            {
                int num2 = 1;
                foreach (string key6 in Iap_failed_info.Keys)
                {
                    configure.AddValueSingle("Save", "IapFailedInfoTid" + num2, key6, "", "");
                    string data = Iap_failed_info[key6];
                    data = DataEncipher(data);
                    Utils.FileWriteString(Utils.SavePath() + MD5Sample.GetMd5String("IapFailedInfoReceipt" + num2) + ".bytes", data);
                    num2++;
                }
            }*/

            string data2 = configure.Save();
            data2 = DataEncipher(data2);
            Utils.FileWriteString(Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2") + ".bytes", data2);
        }
        catch (Exception ex)
        {
            Debug.LogError("[Save] Exception while saving data: " + ex);
        }
        finally
        {
            isSaving = false;
            Debug.Log("[Save] Save data successfully written.");
        }
    }

    public bool LoadData()
    {
        if (isLoaded)
        {
            Debug.Log("[GameData] LoadData called but data is already loaded — skipping.");
            return true;
        }

        string mainSavePath = Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2") + ".bytes";

        Func<string, bool> tryLoadSave = (string path) =>
        {
            string content = string.Empty;
            if (!Utils.FileReadString(path, ref content))
            {
                Debug.LogWarning("[Load] Save file not found or unreadable: " + path);
                return false;
            }

            try
            {
                Configure configure = new Configure();
                content = DataDecrypt(content);
                configure.Load(content);

                // Verify integrity with hash check
                string savedHash = configure.GetSingle("Save", "DataHash");
                string calculatedHash = GenerateDataHash(configure);

                if (string.IsNullOrEmpty(savedHash) || savedHash != calculatedHash)
                {
                    Debug.LogWarning("[Load] DataHash mismatch or missing! Save file corrupted: " + path);
                    return false;
                }

                // If hash matches, proceed with loading all data
                LoadBasicStats(configure);
                LoadPlayerSettings(configure);
                LoadTutorialFlagsOnly(configure);
                LoadUrlsAndFlags(configure);
                LoadUsedPromoCodes(configure);
                HandleDailyReset(configure);
                LoadResetDatesSafely(configure);
                LoadIAPResendAndroid(configure);
                LoadWeapons(configure);
                LoadAvatars(configure);
                LoadSkills(configure);
                LoadProbabilities(configure);
                LoadFreeSpinCount(configure);
                LoadLotterySeatState(configure);
                LoadRollbackDate(configure);

                // Promo codes again in case needed
                string usedCodes = configure.GetSingle("Save", "UsedPromoCodes");
                if (!string.IsNullOrEmpty(usedCodes))
                    usedPromoCodes = new HashSet<string>(usedCodes.Split(','));

                isLoaded = true;
                Debug.Log("[Load] Successfully loaded save file: " + path);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("[Load] Exception while loading save file: " + path + "\n" + ex);
                return false;
            }
        };

        // Try main save first
        if (tryLoadSave(mainSavePath))
            return true;

        Debug.LogError("[Load] Main save failed to load. Resetting to default data.");
        Init();
        return false;
    }

    private void EnsureDefault(string section, string key, string defaultValue)
    {
        if (configure.GetSingle(section, key) == null)
        {
            configure.AddSingle(section, key, defaultValue);
            Debug.Log("[SaveData] Added missing default: " + key + " = " + defaultValue);
        }
    }

    private void LoadBasicStats(Configure configure)
    {
        game_version = SafeGetSingle(configure, "Save", "Version", "0");

        total_cash.SetIntVal(ParseIntSafe(configure.GetSingle("Save", "Cash")), GameDataIntPurpose.Cash);
        total_crystal.SetIntVal(ParseIntSafe(configure.GetSingle("Save", "Crystal")), GameDataIntPurpose.Crystal);
        total_voucher.SetIntVal(ParseIntSafe(configure.GetSingle("Save", "Voucher")), GameDataIntPurpose.Voucher);

        lastSavedCash = ParseIntSafe(configure.GetSingle("Save", "LastSafeCash"), total_cash.GetIntVal());
        lastSavedCrystal = ParseIntSafe(configure.GetSingle("Save", "LastSafeCrystal"), total_crystal.GetIntVal());
        lastSavedVoucher = ParseIntSafe(configure.GetSingle("Save", "LastSafeVoucher"), total_voucher.GetIntVal());
        lastSavedFreeSpins = ParseIntSafe(configure.GetSingle("Save", "LastSavedFreeSpins"), 0);

        tapjoyPoints = ParseIntSafe(configure.GetSingle("Save", "TapjoyPoints"), tapjoyPoints);
        day_level = ParseIntSafe(configure.GetSingle("Save", "DayLevel"), day_level);
    }

    private void LoadPlayerSettings(Configure configure)
    {
        string avatarTypeStr = configure.GetSingle("Save", "AvatarType");
        int avatarTypeInt;
        if (avatarTypeStr != null && int.TryParse(avatarTypeStr, out avatarTypeInt))
            cur_avatar = (AvatarType)avatarTypeInt;
        else
            cur_avatar = AvatarType.Human;

        sensitivity_ratio = ParseFloatSafe(configure.GetSingle("Save", "SensitivityRatio"), sensitivity_ratio);

        daily_mission_count = ParseIntSafe(configure.GetSingle("Save", "DailyMissionCount"));
        Debug.Log("[LoadGameData] daily_mission_count loaded: " + daily_mission_count);

        showNicknamePrompt = configure.GetSingle("Save", "ShowNicknamePrompt") == "0";

        blackname = configure.GetSingle("Save", "Blackname") == "1";

        string nickNameStr = configure.GetSingle("Save", "NickName");
        if (!string.IsNullOrEmpty(nickNameStr))
            NickName = nickNameStr;
    }

    public void LoadTutorialFlagsOnly(Configure configure)
    {
        string enterTutorialStr = configure.GetSingle("Save", "EnterTutorial");
        bool loadedTutorialFlag = ParseBoolSafe(enterTutorialStr, false);
        Debug.Log("[LoadPlayerSettings] Loaded EnterTutorial flag: " + loadedTutorialFlag + " (raw string: " + enterTutorialStr + ")");
        is_enter_tutorial = loadedTutorialFlag;
        show_ui_tutorial = ParseBoolSafe(configure.GetSingle("Save", "ShowUITutorial"), show_ui_tutorial);
        show_ui_tutorial_weapon = ParseBoolSafe(configure.GetSingle("Save", "ShowUITutorialWeapon"), show_ui_tutorial_weapon);
    }

    private void LoadUrlsAndFlags(Configure configure)
    {
        enter_shop_count = ParseIntSafe(configure.GetSingle("Save", "EnterShopCount"), enter_shop_count);

        timeserver_url = SafeGetSingle(configure, "Save", "TimeserverUrl", "");
        statistics_url = SafeGetSingle(configure, "Save", "StatisticsUrl", "");
        redeem_get_url = SafeGetSingle(configure, "Save", "RedeemGetUrl", redeem_get_url);
        redeem_accept_url = SafeGetSingle(configure, "Save", "RedeemAcceptUrl", redeem_accept_url);

        needsUpdate = ParseBoolSafe(configure.GetSingle("Save", "NeedsUpdate"), false);
        Debug.Log("Loaded NeedsUpdate: " + needsUpdate);

        //iap_check_url = "http://192.225.224.97:7600/gameapi/GameCommon.do?action=groovy&json=";
        TRINITI_IAP_CEHCK = ParseBoolSafe(configure.GetSingle("Save", "IapCheck"), TRINITI_IAP_CEHCK);
    }

    private void LoadUsedPromoCodes(Configure configure)
    {
        string usedCodesStr = SafeGetSingle(configure, "Save", "UsedPromoCodes", "");
        usedPromoCodes = string.IsNullOrEmpty(usedCodesStr) ? new HashSet<string>() : new HashSet<string>(usedCodesStr.Split(','));
    }

    private void HandleDailyReset(Configure configure, bool skipSave = false)
    {
        if (configure == null)
        {
            Debug.LogWarning("[HandleDailyReset] Configure is null, skipping daily reset.");
            return;
        }

        save_date = SafeGetSingle(configure, "Save", "SaveDate", "");
        lastMissionCompleteDate = configure.GetSingle("Save", "LastMissionCompleteDate") ?? "";
        cur_save_date = DateTime.Now.ToString("yyyy-MM-dd");

        if (string.IsNullOrEmpty(lastMissionCompleteDate) ||
            string.Compare(lastMissionCompleteDate, cur_save_date, StringComparison.Ordinal) < 0)
        {
            daily_mission_count = 0;
            lottery_reset_count = 0;
            lottery_count = 0;
            is_daily_cd_crystal = false;

            if (!skipSave)
            {
                try
                {
                    SaveData();
                }
                catch (Exception ex)
                {
                    Debug.LogError("[HandleDailyReset] Failed to save after daily reset: " + ex.Message);
                }
            }
        }
        else
        {
            lottery_reset_count = ParseIntSafe(configure.GetSingle("Save", "LotteryResetCount"));
            lottery_count = ParseIntSafe(configure.GetSingle("Save", "LotteryCount"));
            daily_mission_count = ParseIntSafe(configure.GetSingle("Save", "DailyMissionCount"));
        }
    }

    private void LoadResetDatesSafely(Configure configure)
    {
        string lastResetDateStr = configure.GetSingle("Save", "LastResetDate");
        if (!string.IsNullOrEmpty(lastResetDateStr))
        {
            DateTime parsedDate;
            if (DateTime.TryParse(lastResetDateStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsedDate))
                lastResetDate = parsedDate.Date;
            else
            {
                Debug.LogWarning("[LoadData] Failed to parse LastResetDate: " + lastResetDateStr);
                lastResetDate = DateTime.MinValue;
            }
        }
        else
        {
            lastResetDate = DateTime.MinValue;
        }

        string maxDateStr = configure.GetSingle("Save", "MaxDateReached");
        if (!string.IsNullOrEmpty(maxDateStr))
        {
            DateTime parsedMax;
            if (DateTime.TryParse(maxDateStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsedMax))
                maxDateReached = parsedMax.Date;
            else
                maxDateReached = lastResetDate;
        }
        else
        {
            maxDateReached = lastResetDate;
        }

        string lastResetSystemTimeStr = configure.GetSingle("Save", "LastResetSystemTime");
        DateTime parsedSystemTime;
        if (!string.IsNullOrEmpty(lastResetSystemTimeStr) && DateTime.TryParse(lastResetSystemTimeStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsedSystemTime))
        {
            if (DateTime.Now < parsedSystemTime)
            {
                Debug.LogWarning("System time was rolled back.");
                timeWentBackwards = true;
            }
            else
            {
                lastResetSystemTime = DateTime.Now;
                timeWentBackwards = false;
            }
        }
        else
        {
            lastResetSystemTime = DateTime.Now;
            timeWentBackwards = false;
        }
    }

    private void LoadIAPResendAndroid(Configure configure)
    {
        string iapResendStr = configure.GetSingle("Save", "IAPResend_android");
        if (!string.IsNullOrEmpty(iapResendStr) && iapResendStr != "false")
        {
            string[] parts = iapResendStr.Split('|');
            if (parts.Length >= 5)
            {
                IapCenter.Instance.SendIAPVerifyRequest_for_Android(parts[0], parts[1], parts[2], parts[3], parts[4]);
            }
        }
    }

    private void LoadLotterySeatState(Configure configure)
    {
        lottery_seat_state.Clear();
        for (int i = 0; i < 14; i++)
        {
            string seat = configure.GetArray2("Save", "LotterySeatState", i, 0);
            lottery_seat_state.Add(string.IsNullOrEmpty(seat) ? "null" : seat);
        }
    }

    private void LoadRollbackDate(Configure configure)
    {
        string rollbackDateStr = configure.GetSingle("Save", "LastRollbackDate");
        DateTime parsedRollback;
        if (!string.IsNullOrEmpty(rollbackDateStr) &&
            DateTime.TryParseExact(rollbackDateStr, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out parsedRollback))
        {
            GameData.Instance.lastRollbackDate = parsedRollback;
        }
    }

    private int ParseSafeInt(string s)
    {
        int val;
        return int.TryParse(s, out val) ? val : 0;
    }

    private float ParseSafeFloat(string s)
    {
        float val;
        return float.TryParse(s, out val) ? val : 0f;
    }

    private DateTime ParseSafeDate(string s)
    {
        DateTime val;
        return DateTime.TryParse(s, out val) ? val : DateTime.MinValue;
    }

    int ParseIntSafe(string str, int fallback = 0)
    {
        int val;
        return int.TryParse(str, out val) ? val : fallback;
    }

    float ParseFloatSafe(string str, float fallback = 0f)
    {
        float val;
        return float.TryParse(str, out val) ? val : fallback;
    }

    bool ParseBoolSafe(string str, bool fallback = false)
    {
        if (string.IsNullOrEmpty(str)) return fallback;
        return str == "1" || str.ToLower() == "true";
    }


    private void SetDefaultSkillLevelsFromWeapons()
    {
        SkillData skillData;
        if (Skill_Avatar_Set.TryGetValue("Whirlwind", out skillData))
        {
            skillData.level = WeaponData_Set["Chainsaw"].level;
            skillData.ResetData();
        }
        if (Skill_Avatar_Set.TryGetValue("Enchant", out skillData))
        {
            skillData.level = WeaponData_Set["Medicine"].level;
            skillData.ResetData();
        }
        if (Skill_Avatar_Set.TryGetValue("BaseballRobot", out skillData))
        {
            skillData.level = WeaponData_Set["Baseball"].level;
            skillData.ResetData();
        }
        if (Skill_Avatar_Set.TryGetValue("Grenade", out skillData))
        {
            skillData.level = WeaponData_Set["Shield"].level;
            skillData.ResetData();
        }
        if (Skill_Avatar_Set.TryGetValue("Scarecrow", out skillData))
        {
            skillData.level = WeaponData_Set["44Magnum"].level;
            skillData.ResetData();
        }
    }

    private void LoadGameProbData(Configure configure, string section, Dictionary<string, GameProb> targetSet)
    {
        int count = ParseIntSafe(configure.GetSingle("Save", section + "Count"));
        for (int i = 0; i < count; i++)
        {
            string key = configure.GetArray2("Save", section, i, 0);
            string countStr = configure.GetArray2("Save", section, i, 1);
            int val = ParseIntSafe(countStr);

            if (string.IsNullOrEmpty(key))
                continue;

            GameProbsCfg probCfg;
            if (GameConfig.Instance.ProbsConfig_Set.TryGetValue(key, out probCfg))
            {
                GameProb gameProb = new GameProb();
                gameProb.prob_cfg = probCfg;
                gameProb.count = val;
                targetSet[key] = gameProb;
            }
        }
    }

    private float ParseFloatOrDefault(string input, float defaultValue)
    {
        float result;
        if (!float.TryParse(input, out result))
            return defaultValue;
        return result;
    }

    private void SetOrAddSingle(string section, string key, string value)
    {
        if (configure.GetSection(section) == null)
        {
            configure.AddSection(section, "", "");
        }

        if (configure.GetValue(section, key) == null)
        {
            configure.AddValueSingle(section, key, value, "", "");
        }
        else
        {
            configure.SetSingle(section, key, value);
        }
    }

    private string SafeGetSingle(Configure config, string section, string key, string defaultValue = "")
    {
        string value = config.GetSingle(section, key);
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }

    private string SafeGetArray2(Configure config, string section, string key, int i, int j, string defaultValue = "")
    {
        string value = config.GetArray2(section, key, i, j);
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }

    private void LoadWeapons(Configure configure)
    {
        string weaponsCountStr = configure.GetSingle("Save", "WeaponsDataCount");
        int weaponsCount = 0;
        if (weaponsCountStr != null)
            int.TryParse(weaponsCountStr, out weaponsCount);

        for (int i = 0; i < weaponsCount; i++)
        {
            string weaponId = configure.GetArray2("Save", "WeaponsData", i, 0);
            if (weaponId == null)
                continue;

            WeaponData weaponData;
            if (!WeaponData_Set.TryGetValue(weaponId, out weaponData))
                continue;

            int tempInt;

            string levelStr = configure.GetArray2("Save", "WeaponsData", i, 2);
            if (levelStr != null && int.TryParse(levelStr, out tempInt))
                weaponData.level = tempInt;

            string bulletStr = configure.GetArray2("Save", "WeaponsData", i, 3);
            if (bulletStr != null && int.TryParse(bulletStr, out tempInt))
                weaponData.total_bullet_count = tempInt;

            string existStateStr = configure.GetArray2("Save", "WeaponsData", i, 4);
            if (existStateStr != null && int.TryParse(existStateStr, out tempInt))
                weaponData.exist_state = (WeaponExistState)tempInt;

            weaponData.damage_level = ParseOrDefault(configure.GetArray2("Save", "WeaponsData", i, 5), weaponData.level);
            weaponData.frequency_level = ParseOrDefault(configure.GetArray2("Save", "WeaponsData", i, 6), weaponData.level);
            weaponData.clip_level = ParseOrDefault(configure.GetArray2("Save", "WeaponsData", i, 7), weaponData.level);
            weaponData.range_level = ParseOrDefault(configure.GetArray2("Save", "WeaponsData", i, 8), weaponData.level);
            weaponData.stretch_level = ParseOrDefault(configure.GetArray2("Save", "WeaponsData", i, 9), weaponData.level);
        }
    }

    private void LoadAvatars(Configure configure)
    {
        string countStr = configure.GetSingle("Save", "AvatarsDataCount");
        int count = 0;
        if (countStr != null)
            int.TryParse(countStr, out count);

        for (int j = 0; j < count; j++)
        {
            string avatarKeyStr = configure.GetArray2("Save", "AvatarsData", j, 1);
            if (avatarKeyStr == null)
                continue;

            int avatarKeyInt;
            if (!int.TryParse(avatarKeyStr, out avatarKeyInt))
                continue;

            AvatarType avatarKey = (AvatarType)avatarKeyInt;

            AvatarData avatarData;
            if (!AvatarData_Set.TryGetValue(avatarKey, out avatarData))
                continue;

            int tempInt;

            string levelStr = configure.GetArray2("Save", "AvatarsData", j, 3);
            if (levelStr != null && int.TryParse(levelStr, out tempInt))
                avatarData.level = tempInt;

            string armorStr = configure.GetArray2("Save", "AvatarsData", j, 4);
            if (armorStr != null && int.TryParse(armorStr, out tempInt))
                avatarData.armor_level = tempInt;

            string expStr = configure.GetArray2("Save", "AvatarsData", j, 5);
            if (expStr != null && int.TryParse(expStr, out tempInt))
                avatarData.cur_exp = new GameDataInt(tempInt);

            string existStr = configure.GetArray2("Save", "AvatarsData", j, 6);
            if (existStr != null && int.TryParse(existStr, out tempInt))
                avatarData.exist_state = (AvatarExistState)tempInt;

            string equipmentStr = configure.GetArray2("Save", "AvatarsData", j, 7);
            if (equipmentStr != null)
                avatarData.primary_equipment = equipmentStr;

            avatarData.hp_level = ParseOrDefault(configure.GetArray2("Save", "AvatarsData", j, 8), avatarData.level);
            avatarData.damage_level = ParseOrDefault(configure.GetArray2("Save", "AvatarsData", j, 9), avatarData.level);

            for (int k = 0; k < 2; k++)
            {
                string skillVal = configure.GetArray2("Save", "AvatarsData", j, 10 + k);
                if (skillVal != null)
                    avatarData.skill_list[k] = skillVal;
            }
        }
    }

    private void LoadSkills(Configure configure)
    {
        int count = ParseIntSafe(configure.GetSingle("Save", "SkillAvatarDataCount"));
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                string skillKey = configure.GetArray2("Save", "SkillAvatarData", i, 0);
                if (string.IsNullOrEmpty(skillKey)) continue;

                SkillData skillData;
                if (!Skill_Avatar_Set.TryGetValue(skillKey, out skillData)) continue;

                skillData.level = ParseIntSafe(configure.GetArray2("Save", "SkillAvatarData", i, 1), skillData.level);
                skillData.exist_state = (SkillExistState)ParseIntSafe(configure.GetArray2("Save", "SkillAvatarData", i, 2), (int)skillData.exist_state);

                skillData.ResetData();
            }
        }
        else
        {
            SetDefaultSkillLevelsFromWeapons();
        }
    }

    private void LoadProbabilities(Configure configure)
    {
        LoadProbSet(configure, "StoryProbData", GameStoryProbs_Set);
        LoadProbSet(configure, "FragmentProbData", WeaponFragmentProbs_Set);
        LoadProbSet(configure, "IntensifierProbData", WeaponIntensifierProbs_Set);
    }

    private void LoadProbSet(Configure configure, string keyPrefix, Dictionary<string, GameProb> targetSet)
    {
        string countStr = configure.GetSingle("Save", keyPrefix + "Count");
        int count = 0;
        if (countStr != null)
            int.TryParse(countStr, out count);

        for (int i = 0; i < count; i++)
        {
            string key = configure.GetArray2("Save", keyPrefix, i, 0);
            string countVal = configure.GetArray2("Save", keyPrefix, i, 1);
            int parsedCount = 0;
            if (countVal != null)
                int.TryParse(countVal, out parsedCount);

            if (string.IsNullOrEmpty(key))
                continue;

            GameProbsCfg probCfg;
            if (GameConfig.Instance.ProbsConfig_Set.TryGetValue(key, out probCfg))
            {
                GameProb prob = new GameProb();
                prob.prob_cfg = probCfg;
                prob.count = parsedCount;
                targetSet[key] = prob;
            }
        }
    }

    private const int MAX_FREE_SPINS = 5;

    public void SetFreeLotterySpins(int newValue)
    {
        SetFreeLotterySpins(newValue, false);
    }

    public void SetFreeLotterySpins(int newValue, bool force)
    {
        newValue = Mathf.Clamp(newValue, 0, MAX_FREE_SPINS);

        int current = free_lottery_spins.GetIntVal();

        if (force || newValue <= current)
        {
            free_lottery_spins.SetIntVal(newValue, GameDataIntPurpose.FreeSpin);
            Debug.Log("[GameData] Free spins set: " + current + " -> " + newValue + " (force=" + force + ")");
        }
        else
        {
            Debug.LogWarning("[GameData] Blocked unauthorized free_lottery_spins increase: " + current + " -> " + newValue);
        }
    }

    private bool CheckCurrencyJumpAndUpdateState()
    {
        if (!CheckSaveFrequency())
        {
            return false;
        }

        DateTime now = DateTime.Now;

        int currentCash = Math.Min(1500000, Math.Max(0, total_cash.GetIntVal()));
        int currentCrystal = Math.Min(1000, Math.Max(0, total_crystal.GetIntVal()));
        int currentVoucher = Math.Min(1250, Math.Max(0, total_voucher.GetIntVal()));
        int currentFreeSpins = Math.Max(0, GameData.Instance.free_lottery_spins.GetIntVal());

        const int CashCap = 500000;
        const int CrystalCap = 85;
        const int VoucherCap = 230;
        const int FreeSpinCap = 5;

        string sceneName = SceneManager.GetActiveScene().name;
        bool skipCashJumpCheck = sceneName.StartsWith("COMZ2");

        bool cashJump = !skipCashJumpCheck && (currentCash - lastSavedCash) > CashCap;
        bool crystalJump = (currentCrystal - lastSavedCrystal) > CrystalCap;
        int voucherDiff = currentVoucher - lastSavedVoucher;
        bool voucherJump = voucherDiff > VoucherCap;
        bool freeSpinJump = (currentFreeSpins - lastSavedFreeSpins) > FreeSpinCap;

        TimeSpan gracePeriod = TimeSpan.FromSeconds(2);
        bool inGracePeriod = (now - lastLotteryAwardTime) < gracePeriod;

        if (justReceivedLotteryReward || inGracePeriod)
        {
            suspiciousSaveCount = 0;
            lastSavedCash = currentCash;
            lastSavedCrystal = currentCrystal;
            lastSavedVoucher = currentVoucher;
            lastSavedFreeSpins = currentFreeSpins;
            lastSaveTime = now;
            justReceivedLotteryReward = false;
            return true;
        }

        // First save initialization
        if (lastSaveTime == DateTime.MinValue || (lastSavedCash == 0 && lastSavedCrystal == 0 && lastSavedVoucher == 0))
        {
            suspiciousSaveCount = 0;
            lastSavedCash = currentCash;
            lastSavedCrystal = currentCrystal;
            lastSavedVoucher = currentVoucher;
            lastSavedFreeSpins = currentFreeSpins;
            lastSaveTime = now;
            return true;
        }

        if (cashJump || crystalJump || voucherJump || freeSpinJump)
        {
            suspiciousSaveCount++;
            Debug.LogWarning(string.Format("Suspicious currency jump detected! suspiciousSaveCount incremented to {0}.", suspiciousSaveCount));

            if (suspiciousSaveCount >= maxSuspiciousSaves)
            {
                blackname = true;
                Debug.LogWarning("Player blacklisted due to repeated suspicious currency jumps!");
            }

            return false;
        }

        bool isFirstSave = lastSaveTime == DateTime.MinValue;
        if (!isFirstSave)
        {
            TimeSpan legitActionGracePeriod = TimeSpan.FromSeconds(5);
            bool inLegitActionWindow = (now - lastLegitActionTime) < legitActionGracePeriod;

            if (inLegitActionWindow)
            {
                suspiciousSaveCount = 0;
                lastSavedCash = currentCash;
                lastSavedCrystal = currentCrystal;
                lastSavedVoucher = currentVoucher;
                lastSavedFreeSpins = currentFreeSpins;
                lastSaveTime = now;
                return true;
            }

            TimeSpan rapidSaveThreshold = TimeSpan.FromSeconds(5);
            bool rapidSave = (now - lastSaveTime) < rapidSaveThreshold;

            bool cashFrozen = (currentCash == lastSavedCash) && currentCash >= CashCap;
            bool crystalFrozen = (currentCrystal == lastSavedCrystal) && currentCrystal >= CrystalCap;
            bool voucherFrozen = (currentVoucher == lastSavedVoucher) && currentVoucher >= VoucherCap;
            bool freeSpinFrozen = (currentFreeSpins == lastSavedFreeSpins) && currentFreeSpins >= FreeSpinCap;

            if (rapidSave && cashFrozen && crystalFrozen && voucherFrozen && freeSpinFrozen)
            {
                suspiciousSaveCount++;

                if (suspiciousSaveCount >= maxSuspiciousSaves)
                {
                    blackname = true;
                    Debug.LogWarning("Player blacklisted due to repeated frozen state saves!");
                }
            }
            else
            {
                suspiciousSaveCount = 0;
            }

            lastSavedCash = currentCash;
            lastSavedCrystal = currentCrystal;
            lastSavedVoucher = currentVoucher;
            lastSavedFreeSpins = currentFreeSpins;
            lastSaveTime = now;

            return true;
        }
        return true;
    }

    public void AddFreeLotterySpins(int amount, bool force = false)
    {
        if (amount <= 0) return;

        int current = free_lottery_spins.GetIntVal();
        SetFreeLotterySpins(current + amount, force);
    }

    public bool ConsumeFreeLotterySpin()
    {
        int current = free_lottery_spins.GetIntVal();
        Debug.Log("[Lottery] ConsumeFreeLotterySpin called. Current spins: " + current);

        if (current > 0)
        {
            SetFreeLotterySpins(current - 1);
            Debug.Log("[Lottery] Free spin consumed. New spins: " + (current - 1));
            return true;
        }
        Debug.Log("[Lottery] No free spins to consume.");
        return false;
    }

    private int ParseOrDefault(string str, int defaultVal)
    {
        int val;
        return int.TryParse(str, out val) ? val : defaultVal;
    }

    private string SafeGetSingle(Configure config, string section, string key)
    {
        try
        {
            return config.GetSingle(section, key);
        }
        catch
        {
            return null;
        }
    }

    public List<WeaponData> GetOwnerWeapons()
    {
        List<WeaponData> list = new List<WeaponData>();
        foreach (WeaponData value in WeaponData_Set.Values)
        {
            if (value.exist_state == WeaponExistState.Owned)
            {
                list.Add(value);
            }
        }
        return list;
    }

    public static class VersionHelper
    {
        public static string LoadVersionFromFile()
        {
            TextAsset versionText = Resources.Load<TextAsset>("version");
            if (versionText != null)
            {
                return versionText.text.Trim();
            }
            else
            {
                Debug.LogWarning("[VersionHelper] Resources/version.txt not found. Using fallback version.");
                return "1.0";
            }
        }
    }

    public List<WeaponData> GetCouldIntensifierWeapons()
    {
        List<WeaponData> list = new List<WeaponData>();
        foreach (WeaponData value in WeaponData_Set.Values)
        {
            if (value.exist_state == WeaponExistState.Owned)
            {
                list.Add(value);
            }
        }
        return list;
    }

    public void DropIntensifierRandom(List<WeaponData> WeaponSet)
    {
        int num = 0;
        foreach (WeaponData item in WeaponSet)
        {
            num += item.intensifier_drop_weight;
        }
        int num2 = UnityEngine.Random.Range(1, num);
        int num3 = 0;
        foreach (WeaponData item2 in WeaponSet)
        {
            num3 += item2.intensifier_drop_weight;
            if (num2 <= num3)
            {
                Debug.Log("drop intensifier, weapon name:" + item2.weapon_name);
                break;
            }
        }
    }

    public bool CheckStoryProbCombine(int level)
    {
        List<GameProb> list = new List<GameProb>();
        foreach (GameProb value in GameStoryProbs_Set.Values)
        {
            GameStoryProbsCfg gameStoryProbsCfg = value.prob_cfg as GameStoryProbsCfg;
            if (gameStoryProbsCfg.level == level)
            {
                list.Add(value);
            }
        }
        int num = 0;
        foreach (GameProb item in list)
        {
            GameStoryProbsCfg gameStoryProbsCfg2 = item.prob_cfg as GameStoryProbsCfg;
            num += gameStoryProbsCfg2.weight * item.count;
        }
        if (num >= 10)
        {
            foreach (GameProb item2 in list)
            {
                GameStoryProbs_Set.Remove(item2.prob_cfg.prob_name);
            }
            return true;
        }
        return false;
    }

    public void DumpConfigureSectionFull(string sectionName)
    {
        /*if (configure == null)
        {
            Debug.LogWarning("DumpConfigureSectionFull: configure is null!");
            return;
        }

        var keys = configure.GetAllKeysInSection(sectionName);
        Debug.Log("[DumpConfigureSectionFull] Dumping keys and values for section: " + sectionName);

        foreach (var key in keys)
        {
            var val = configure.GetSingle(sectionName, key);

            if (val != null)
            {
                // Single value, print directly
                Debug.Log(string.Format("[{0}] {1} = {2}", sectionName, key, val));
                continue;
            }

            int countArray = configure.CountArray(sectionName, key);
            if (countArray > 0)
            {
                string arrayValues = "";
                for (int i = 0; i < countArray; i++)
                {
                    arrayValues += configure.GetArray(sectionName, key, i);
                    if (i < countArray - 1) arrayValues += ", ";
                }
                Debug.Log(string.Format("[{0}] {1} = [ {2} ]", sectionName, key, arrayValues));
                continue;
            }

            int countArray2 = configure.CountArray2(sectionName, key);
            if (countArray2 > 0)
            {
                Debug.Log(string.Format("[{0}] {1} = [ Array2 count: {2} ]", sectionName, key, countArray2));
                for (int i = 0; i < countArray2; i++)
                {
                    int innerCount = configure.CountArray2(sectionName, key, i);
                    string innerValues = "";
                    for (int j = 0; j < innerCount; j++)
                    {
                        innerValues += configure.GetArray2(sectionName, key, i, j);
                        if (j < innerCount - 1) innerValues += ", ";
                    }
                    Debug.Log(string.Format("\t[{0}]: {1}", i, innerValues));
                }
                continue;
            }

            Debug.Log(string.Format("[{0}] {1} = <unknown or empty>", sectionName, key));
        }*/
    }

    private void SaveFreeSpinCount()
    {
        int spins = Instance.free_lottery_spins.GetIntVal();
        SetOrAddSingle("Save", "FreeLotterySpins", spins.ToString());
        //Debug.Log("[SaveFreeSpinCount] Saved spins: " + spins);
    }

    private void LoadFreeSpinCount(Configure configure)
    {
        int spins = ParseIntSafe(SafeGetSingle(configure, "Save", "FreeLotterySpins", "0"));
        Instance.free_lottery_spins.SetIntVal(spins, GameDataIntPurpose.FreeSpin);
        //Debug.Log("[LoadFreeSpinCount] Loaded spins: " + spins);
    }

    public List<GameProb> GetWeaponFragmentProbs(string weapon_name)
    {
        List<GameProb> list = new List<GameProb>();
        foreach (GameProb value in WeaponFragmentProbs_Set.Values)
        {
            WeaponFragmentProbsCfg weaponFragmentProbsCfg = value.prob_cfg as WeaponFragmentProbsCfg;
            if (weaponFragmentProbsCfg.weapon_name == weapon_name)
            {
                list.Add(value);
            }
        }
        return list;
    }

    private int _lastSavedFreeSpinCount = DEFAULT_FREE_SPIN_COUNT;
    public int LastSavedFreeSpinCount
    {
        get { return _lastSavedFreeSpinCount; }
        set
        {
            if (value < 0) _lastSavedFreeSpinCount = DEFAULT_FREE_SPIN_COUNT;
            else _lastSavedFreeSpinCount = value;
        }
    }

    public int[] GetWeaponFragmentProbsCountOrder(string weapon_name)
    {
        if (WeaponData_Set[weapon_name].config.combination_count == 0)
        {
            return null;
        }
        List<GameProb> weaponFragmentProbs = GetWeaponFragmentProbs(weapon_name);
        int[] array = new int[WeaponData_Set[weapon_name].config.combination_count];
        for (int i = 0; i < weaponFragmentProbs.Count; i++)
        {
            bool flag = false;
            foreach (GameProb item in weaponFragmentProbs)
            {
                WeaponFragmentProbsCfg weaponFragmentProbsCfg = item.prob_cfg as WeaponFragmentProbsCfg;
                int num = (int)(weaponFragmentProbsCfg.type - 1);
                if (num == i)
                {
                    array[i] = item.count;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                array[i] = 0;
            }
        }
        return array;
    }

    public void ClearUsedPromoCodes()
    {
        usedPromoCodes.Clear();
    }

    /*private const int FakeDisplayValue = 999999;

    public int DisplayCash
    {
        get { return blackname ? FakeDisplayValue : _total_cash.GetIntVal(); }
    }

    public int DisplayCrystal
    {
        get { return blackname ? FakeDisplayValue : _total_crystal.GetIntVal(); }
    }

    public int DisplayVoucher
    {
        get { return blackname ? FakeDisplayValue : _total_voucher.GetIntVal(); }
    }*/

    public bool CheckFragmentProbCombine(string weapon_name)
    {
        if (WeaponData_Set[weapon_name].exist_state != 0)
        {
            Debug.Log("Weapon:" + weapon_name + " is combined already");
            return false;
        }
        int[] weaponFragmentProbsCountOrder = GetWeaponFragmentProbsCountOrder(weapon_name);
        if (weaponFragmentProbsCountOrder == null)
        {
            return false;
        }
        bool result = true;
        for (int i = 0; i < weaponFragmentProbsCountOrder.Length; i++)
        {
            if (weaponFragmentProbsCountOrder[i] == 0)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    public bool WeaponCombine(string weapon_name)
    {
        if (!CheckFragmentProbCombine(weapon_name))
        {
            return false;
        }
        List<GameProb> weaponFragmentProbs = GetWeaponFragmentProbs(weapon_name);
        foreach (GameProb item in weaponFragmentProbs)
        {
            WeaponFragmentProbs_Set.Remove(item.prob_cfg.prob_name);
        }
        Instance.WeaponData_Set[weapon_name].exist_state = WeaponExistState.Owned;
        return true;
    }

    public List<WeaponData> GetPrimaryWeapons()
    {
        List<WeaponData> list = new List<WeaponData>();
        foreach (WeaponData value in WeaponData_Set.Values)
        {
            if (!value.is_secondary)
            {
                list.Add(value);
            }
        }
        return list;
    }

    public List<WeaponData> GetSecondaryWeaponsFor(AvatarType avatar_type)
    {
        List<WeaponData> list = new List<WeaponData>();
        foreach (WeaponData value in WeaponData_Set.Values)
        {
            if (value.is_secondary && value.owner == avatar_type)
            {
                list.Add(value);
            }
        }
        return list;
    }

    public void ResetCurNistTime()
    {
        Debug.Log("ResetCurNistTime...");
        StartCoroutine(ResetCurServerTime());
    }

    public IEnumerator ResetCurServerTime()
    {
        DateTime currentDateTime = DateTime.Now;
        last_checked_date_now = currentDateTime;
        next_cd_date = currentDateTime.AddDays(1.0);
        next_cd_date = new DateTime(next_cd_date.Year, next_cd_date.Month, next_cd_date.Day, 0, 0, 0);
        Debug.Log("last_checked_date_now:" + last_checked_date_now);
        Debug.Log("next_cd_date:" + next_cd_date);
        double totalMinutes = (next_cd_date - last_checked_date_now).TotalMinutes;
        Debug.Log("cd:" + (next_cd_date - last_checked_date_now) + " totalMin:" + totalMinutes.ToString("F2"));
        string date = cur_save_date = last_checked_date_now.Year.ToString()
            + last_checked_date_now.Month.ToString("D2")
            + last_checked_date_now.Day.ToString("D2");
        daily_mode_enable = true;
        if (!LoadData())
        {
            SaveData();
        }
        if (reset_nist_time_finish != null)
        {
            reset_nist_time_finish();
        }
        yield break;
    }

    public bool HasUsedPromoCode(string code)
    {
        return usedPromoCodes.Contains(code);
    }

    public void AddUsedPromoCode(string code)
    {
        if (!usedPromoCodes.Contains(code))
        {
            usedPromoCodes.Add(code);
        }
    }

    public IEnumerable<string> GetAllUsedPromoCodes()
    {
        return usedPromoCodes;
    }

    public void SaveBlacklistFlagOnly()
    {
        Configure configure = new Configure();
        configure.AddSection("Save", string.Empty, string.Empty);

        // Save just the blacklist flag + required fields with safe defaults
        configure.AddValueSingle("Save", "Blackname", "1", "", "");

        // Add minimal required fields with default/empty values to keep format valid
        configure.AddValueSingle("Save", "Cash", total_cash.ToString(), "", "");
        configure.AddValueSingle("Save", "Crystal", total_crystal.ToString(), "", "");
        configure.AddValueSingle("Save", "Voucher", total_voucher.ToString(), "", "");
        // ... add any other required fields with defaults or last known good values

        // Generate the hash for the current minimal configure
        string dataHash = GenerateDataHash(configure);
        configure.AddValueSingle("Save", "DataHash", dataHash, "", "");

        string dataString = configure.Save();
        dataString = DataEncipher(dataString);
        Utils.FileWriteString(Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2") + ".bytes", dataString);
    }


    public void SetMapMissionList(ref List<QuestInfo> mInfos)
    {
        int key;
        QuestInfo questInfo;

        if (Instance.day_level > GameConfig.Instance.Side_Quest_Order.Count)
        {
            key = (Instance.day_level - 1) % GameConfig.Instance.Side_Quest_Order_Spare.Count + 1;
            questInfo = GameConfig.Instance.Side_Quest_Order_Spare[key];
        }
        else
        {
            key = Instance.day_level;
            questInfo = GameConfig.Instance.Side_Quest_Order[key];
        }

        questInfo.SetQuestComment();

        if (questInfo.mission_type == MissionType.MainMission)
        {
            questInfo = GameConfig.Instance.Main_Quest_Order[key];
        }

        mInfos.Add(questInfo);

        List<string> list = new List<string> { "Church", "Depot", "Junkyard" };

        QuestInfo normalDaily = new QuestInfo();
        normalDaily.mission_type = MissionType.Cleaner;
        normalDaily.mission_day_type = MissionDayType.Daily;
        normalDaily.scene_name = list[UnityEngine.Random.Range(0, list.Count)];
        normalDaily.is_crazy_daily = false;
        normalDaily.SetQuestComment();
        mInfos.Add(normalDaily);

        QuestInfo coopQuest = new QuestInfo();
        coopQuest.mission_type = MissionType.Coop;
        coopQuest.mission_day_type = MissionDayType.None;
        coopQuest.scene_name = list[UnityEngine.Random.Range(0, list.Count)];
        coopQuest.SetQuestComment();
        mInfos.Add(coopQuest);

        QuestInfo endlessQuest = new QuestInfo();
        endlessQuest.mission_type = MissionType.Endless;
        endlessQuest.mission_day_type = MissionDayType.Endless;
        endlessQuest.scene_name = "Street";
        endlessQuest.SetQuestComment();
        mInfos.Add(endlessQuest);
    }

    public void MapSceneQuestInfoWrite(QuestInfo info)
    {
        Debug.Log(string.Concat("info:", info.mission_day_type, " ff:", info.mission_type));
        Instance.cur_quest_info = info;
        Instance.loading_to_scene = Instance.cur_quest_info.scene_name;

        if (Instance.cur_quest_info.mission_day_type == MissionDayType.Daily)
        {
            Hashtable hashtable = new Hashtable();
            int dailyPrice = GetDailyMissionPrice(is_crazy_daily);
            hashtable.Add("tCrystalNum", dailyPrice);
            Instance.UploadStatistics("tCrystal_DaiLy", hashtable);

            int newCrystalValue = GameData.Instance.total_crystal.GetIntVal() - dailyPrice;
            if (newCrystalValue < 0)
            {
                newCrystalValue = 0;
                Debug.LogWarning("Not enough crystals for daily mission.");
            }
            GameData.Instance.total_crystal.SetIntVal(newCrystalValue, GameDataIntPurpose.Crystal);

            Instance.SaveData();
        }
    }

    public void AddWeapon(string weaponID)
    {
        if (WeaponData.Instance.playerWeapons.ContainsKey(weaponID))
        {
            Debug.Log("Weapon already owned: " + weaponID);
            return;
        }

        WeaponData configData;
        if (!WeaponData_Set.TryGetValue(weaponID, out configData))
        {
            Debug.LogWarning("Weapon config not found for: " + weaponID);
            return;
        }

        WeaponData newWeapon = new WeaponData();
        newWeapon.weapon_name = configData.weapon_name;
        newWeapon.weapon_type = configData.weapon_type;
        newWeapon.config = configData.config;
        newWeapon.exist_state = WeaponExistState.Owned;

        newWeapon.damage_level = 1;
        newWeapon.frequency_level = 1;
        newWeapon.clip_level = 1;
        newWeapon.range_level = 1;
        newWeapon.stretch_level = 1;

        WeaponData.Instance.playerWeapons.Add(weaponID, newWeapon);

        Debug.Log("Weapon granted via promo: " + weaponID);

        SaveData();
    }


    public bool HasWeapon(string weaponID)
    {
        return WeaponData.Instance.playerWeapons.ContainsKey(weaponID);
    }

    public List<WeaponFragmentList> GetWeaponFragmentProbsState(string weapon_name)
    {
        WeaponFragmentList weaponFragmentList = null;
        List<WeaponFragmentList> list = new List<WeaponFragmentList>();
        List<GameProb> weaponFragmentProbs = GetWeaponFragmentProbs(weapon_name);
        List<WeaponFragmentProbsCfg> weaponFragmentProb = GameConfig.Instance.GetWeaponFragmentProb(weapon_name);
        foreach (WeaponFragmentProbsCfg item in weaponFragmentProb)
        {
            weaponFragmentList = new WeaponFragmentList(item.prob_name, 0, item.image_name);
            foreach (GameProb item2 in weaponFragmentProbs)
            {
                WeaponFragmentProbsCfg weaponFragmentProbsCfg = item2.prob_cfg as WeaponFragmentProbsCfg;
                if (weaponFragmentProbsCfg != null && item.prob_name == weaponFragmentProbsCfg.prob_name)
                {
                    weaponFragmentList.count += item2.count;
                }
            }
            list.Add(weaponFragmentList);
        }
        return list;
    }

    private bool CheckSaveFrequency()
    {
        DateTime now = DateTime.Now;

        while (recentSaveTimestamps.Count > 0 && (now - recentSaveTimestamps.Peek()) > SaveWindow)
        {
            recentSaveTimestamps.Dequeue();
        }

        recentSaveTimestamps.Enqueue(now);

        if (recentSaveTimestamps.Count > MaxSavesInWindow)
        {
            suspiciousSaveCount++;
            Debug.LogWarning("Save spam detected, incrementing suspiciousSaveCount: " + suspiciousSaveCount);

            if (suspiciousSaveCount >= maxSuspiciousSaves)
            {
                blackname = true;
                Debug.LogWarning("Player blacklisted due to rapid save spam!");
            }

            return false;
        }

        return true;
    }

    public float GetSideEnemyStandardReward()
    {
        float paraA = GameConfig.Instance.Standary_Enemy_Info.ParaA;
        float paraB = GameConfig.Instance.Standary_Enemy_Info.ParaB;
        float paraC = GameConfig.Instance.Standary_Enemy_Info.ParaC;
        float paraK = GameConfig.Instance.Standary_Enemy_Info.ParaK;
        float num = Instance.day_level;
        return paraA * Mathf.Pow(num - paraB, paraK) + paraC;
    }

    public float GetSideEnemyStandardRewardTotal()
    {
        float sideEnemyStandardReward = GetSideEnemyStandardReward();
        return sideEnemyStandardReward * GameConfig.Instance.GetStandaryEnemyCount(Instance.day_level);
    }

    // Important Daily Stuff
    public int GetMissionRewardCash(MissionType mission_type, MissionDayType mission_day_type, int crazy_daily = 0, bool isPostMission = false)
    {
        float sideEnemyStandardRewardTotal = GetSideEnemyStandardRewardTotal();
        int result = 0;
        Debug.Log("[RewardCalc] is_crazy_daily: " + Instance.is_crazy_daily + ", crazy_daily param: " + crazy_daily);

        switch (mission_day_type)
        {
            case MissionDayType.Daily:
                switch (crazy_daily)
                {
                    case 0:
                        result = (!Instance.is_crazy_daily)
                            ? (int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_a * sideEnemyStandardRewardTotal)
                            : (int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_b * sideEnemyStandardRewardTotal);
                        break;
                    case 1:
                        result = (int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_a * sideEnemyStandardRewardTotal);
                        break;
                    case 2:
                        result = (int)(GameConfig.Instance.Mission_Finish_Reward_Info.daily_ratio_b * sideEnemyStandardRewardTotal);
                        break;
                }
                break;

            case MissionDayType.Tutorial:
                result = GameConfig.Instance.init_cash;
                break;

            case MissionDayType.Main:
                {
                    int rewardDayLevel = GameData.Instance.day_level;

                    if (isPostMission)
                        rewardDayLevel -= 1;

                    if (rewardDayLevel < 1)
                        rewardDayLevel = 1;

                    if (GameConfig.Instance.Main_Quest_Difficulty_Set.ContainsKey(rewardDayLevel))
                        result = (int)GameConfig.Instance.Main_Quest_Difficulty_Set[rewardDayLevel].finish_reward;
                    else
                    {
                        Debug.LogWarning("No reward data for Main mission day_level: " + rewardDayLevel);
                        result = 0;
                    }
                    break;
                }

            default:
                result = (int)(GameConfig.Instance.Mission_Finish_Reward_Info.side_ratio * sideEnemyStandardRewardTotal / 1.5f);
                if (GameData.Instance.day_level > 85)
                    result = (int)sideEnemyStandardRewardTotal * 4;
                else if (GameData.Instance.day_level > 55)
                    result = (int)sideEnemyStandardRewardTotal * 3;
                else if (GameData.Instance.day_level > 35)
                    result = (int)sideEnemyStandardRewardTotal * 2;
                break;
        }

        return result;
    }

    public int GetMissionRewardCrystal(MissionType mission_type, MissionDayType mission_day_type)
    {
        int result = 0;

        switch (mission_day_type)
        {
            case MissionDayType.Main:
                result = 5;
                break;

            case MissionDayType.Tutorial:
                result = GameConfig.Instance.init_crystal;
                break;

            case MissionDayType.Side:
                if (GameData.Instance.day_level > 85)
                    result = 6;
                else if (GameData.Instance.day_level > 35)
                    result = 3;
                break;
        }

        return result;
    }

    // Important Daily Stuff
    public int GetMissionRewardVoucher(MissionType mission_type, MissionDayType mission_day_type, int crazy_daily = 0, bool applyMultiplier = true)
    {
        int result = 0;
        var info = GameConfig.Instance.Mission_Finish_Reward_Info;
        float multiplier = 1f;

        if (applyMultiplier)
        {
            if (Instance.day_level >= 85)
                multiplier = 2f;
            else if (Instance.day_level >= 35)
                multiplier = 1.5f;
        }

        switch (mission_day_type)
        {
            case MissionDayType.Daily:
                switch (crazy_daily)
                {
                    case 0:  // Normal daily
                        result = (int)(info.daily_ratio_voucher_base * info.daily_ratio_voucher_a * multiplier);
                        break;
                    case 1:  // Crazy daily type A
                        result = (int)(info.daily_ratio_voucher_base * info.daily_ratio_voucher_a * multiplier);
                        break;
                    case 2:  // Crazy daily type B
                        result = (int)(info.daily_ratio_voucher_base * info.daily_ratio_voucher_b * multiplier);
                        break;
                    default:
                        result = (int)(info.daily_ratio_voucher_base * info.daily_ratio_voucher_a * multiplier);
                        break;
                }
                break;

            case MissionDayType.Tutorial:
                result = GameConfig.Instance.init_voucher;
                break;

            case MissionDayType.Side:
                if (GameData.Instance.day_level > 85)
                    result = 40;
                else if (GameData.Instance.day_level > 35)
                    result = 20;
                else
                    result = 10;
                break;

            case MissionDayType.Main:
                result = 10;
                break;

            default:
                result = 5;
                break;
        }

        return result;
    }

    private void AddValueArray(string section, string key, ArrayList values)
    {
        if (configure.GetSection(section) == null)
            configure.AddSection(section, "", "");

        if (configure.GetValue(section, key) != null)
            configure.RemoveValue(section, key);

        string combined = string.Join(",", (string[])values.ToArray(typeof(string)));

        configure.AddValueArray(section, key, combined, "", "");
    }

    private void AddValueArray(string section, string key, string singleValue)
    {
        if (configure.GetSection(section) == null)
            configure.AddSection(section, "", "");

        if (configure.GetValue(section, key) != null)
            configure.RemoveValue(section, key);

        configure.AddValueArray(section, key, singleValue, "", "");
    }


    private void AddValueArray2(string section, string key, ArrayList values)
    {
        if (configure.GetSection(section) == null)
        {
            configure.AddSection(section, "", "");
        }

        // This overwrites just the key, not the entire section
        configure.AddValueArray2(section, key, values, "", "");
    }

    public DailyMissionStatus EnableDailyMission()
    {
        if (daily_mode_enable && daily_mission_count < 1)
        {
            return DailyMissionStatus.Free;
        }
        if (daily_mode_enable && daily_mission_count < 10)
        {
            return DailyMissionStatus.CrystalEnable;
        }
        if (daily_mode_enable && daily_mission_count >= 10)
        {
            return DailyMissionStatus.CrystalDisenable;
        }
        return DailyMissionStatus.Disenable;
    }

    public bool EnableEnterDailyMission()
    {
        if ((EnableDailyMission() == DailyMissionStatus.Free || EnableDailyMission() == DailyMissionStatus.CrystalEnable) && total_crystal >= GetDailyMissionPrice(is_crazy_daily))
        {
            return true;
        }
        return false;
    }

    public int GetDailyMissionPrice(bool is_hard)
    {
        if (is_hard)
        {
            if (EnableDailyMission() == DailyMissionStatus.Free)
            {
                return GameConfig.Instance.daily_price_hard1;
            }
            if (EnableDailyMission() == DailyMissionStatus.CrystalEnable)
            {
                return GameConfig.Instance.daily_price_hard2;
            }
        }
        else
        {
            if (EnableDailyMission() == DailyMissionStatus.Free)
            {
                return GameConfig.Instance.daily_price_easy1;
            }
            if (EnableDailyMission() == DailyMissionStatus.CrystalEnable)
            {
                return GameConfig.Instance.daily_price_easy2;
            }
        }
        return 99999;
    }

    public int GetDailyCDPrice()
    {
        if (EnableDailyMission() == DailyMissionStatus.CrystalEnable)
        {
            return GameConfig.Instance.daily_price_cd;
        }
        return 99999;
    }

    public bool ResetDailyCD()
    {
        if (EnableDailyMission() == DailyMissionStatus.CrystalEnable && total_crystal >= GetDailyCDPrice())
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("tCrystalNum", GetDailyCDPrice());
            Instance.UploadStatistics("tCrystal_SpeedUp", hashtable);
            GameData.Instance.total_crystal.SetIntVal(
                GameData.Instance.total_crystal.GetIntVal() - GetDailyCDPrice(),
                GameDataIntPurpose.Crystal
            );
            is_daily_cd_crystal = true;
            return true;
        }
        return false;
    }

    public static string Encipher(string data)
    {
        byte[] key = new byte[] { 17, 53, 29, 101 };
        char[] chars = data.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            int keyByte = key[i % key.Length];
            int charCode = (int)chars[i];
            charCode ^= keyByte;
            chars[i] = (char)charCode;
        }
        return new string(chars);
    }

    public static bool IsHighEffect()
    {
        return true;
    }

    public void OnExchgCurrcy(GameCurrencyType src_type, GameCurrencyType des_type, int src_count, int des_count)
    {
        Hashtable hashtable = new Hashtable();

        if (src_type == GameCurrencyType.Crystal)
        {
            int oldVal = GameData.Instance.total_crystal.GetIntVal();
            GameData.Instance.total_crystal.SetIntVal(oldVal - src_count, GameDataIntPurpose.Crystal);
            if (GameData.Instance.total_crystal.GetIntVal() <= 0)
            {
                Debug.LogWarning("ExchgCurrcy warning src type:" + src_type + " count:" + src_count);
                GameData.Instance.total_crystal.SetIntVal(0, GameDataIntPurpose.Crystal);
            }
        }
        else if (src_type == GameCurrencyType.Voucher)
        {
            int oldVal = GameData.Instance.total_voucher.GetIntVal();
            GameData.Instance.total_voucher.SetIntVal(oldVal - src_count, GameDataIntPurpose.Voucher);
            if (GameData.Instance.total_voucher.GetIntVal() <= 0)
            {
                Debug.LogWarning("ExchgCurrcy warning src type:" + src_type + " count:" + src_count);
                GameData.Instance.total_voucher.SetIntVal(0, GameDataIntPurpose.Voucher);
            }
        }
        else if (src_type == GameCurrencyType.Cash)
        {
            int oldVal = GameData.Instance.total_cash.GetIntVal();
            GameData.Instance.total_cash.SetIntVal(oldVal - src_count, GameDataIntPurpose.Cash);
            if (GameData.Instance.total_cash.GetIntVal() <= 0)
            {
                Debug.LogWarning("ExchgCurrcy warning src type:" + src_type + " count:" + src_count);
                GameData.Instance.total_cash.SetIntVal(0, GameDataIntPurpose.Cash);
            }
        }

        if (des_type == GameCurrencyType.Crystal)
        {
            int oldVal = GameData.Instance.total_crystal.GetIntVal();
            GameData.Instance.total_crystal.SetIntVal(oldVal + des_count, GameDataIntPurpose.Crystal);
        }
        else if (des_type == GameCurrencyType.Voucher)
        {
            int oldVal = GameData.Instance.total_voucher.GetIntVal();
            GameData.Instance.total_voucher.SetIntVal(oldVal + des_count, GameDataIntPurpose.Voucher);
            hashtable.Add("tCrystalNum", src_count);
            hashtable.Add("GoldNum", des_count);
            Instance.UploadStatistics("tCrystal_Gold", hashtable);
        }
        else if (des_type == GameCurrencyType.Cash)
        {
            int oldVal = GameData.Instance.total_cash.GetIntVal();
            GameData.Instance.total_cash.SetIntVal(oldVal + des_count, GameDataIntPurpose.Cash);
            hashtable.Add("tCrystalNum", src_count);
            hashtable.Add("VouchersNum", des_count);
            Instance.UploadStatistics("tCrystal_Vouchers", hashtable);
        }
    }

    public void CheckEnemyFirstShow(string enemy_name)
    {
        if (Enemy_Loading_Set.ContainsKey(enemy_name) && Enemy_Loading_Set[enemy_name] != 1)
        {
            Enemy_Loading_Set[enemy_name] = 1;
        }
    }

    public void ClampAllWeaponBullets(int suspiciousThreshold)
    {
        if (suspiciousThreshold <= 0)
        {
            suspiciousThreshold = 25000;
        }

        foreach (KeyValuePair<string, WeaponData> pair in WeaponData_Set)
        {
            WeaponData weapon = pair.Value;
            if (weapon.exist_state == WeaponExistState.Owned)
            {
                if (weapon.total_bullet_count > suspiciousThreshold)
                {
                    Debug.LogWarning("Weapon " + weapon.weapon_name + " bullet count " + weapon.total_bullet_count +
                                     " exceeds suspicious threshold " + suspiciousThreshold + ", clamping to 19999.");
                    weapon.total_bullet_count = 19999;

                }
            }
        }
    }

    public void UploadStatistics(string action_id, Hashtable action_data)
    {
        Hashtable hashtable = new Hashtable();
        hashtable["gamename"] = "com.trinitigame.callofminizombies2";
        hashtable["uuid"] = user_id;
        hashtable["action"] = action_id;
        string text2 = (string)(hashtable["data"] = JsonMapper.ToJson(action_data));
        string s = JsonMapper.ToJson(hashtable);
        byte[] post_data = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
        wwwClient.Instance.SendHttpRequest(statistics_url, post_data, OnStatisticsResponse, OnStatisticsRequestError, action_id);
    }

    public void OnStatisticsResponse(string action, byte[] response_data)
    {
        string jsonString = string.Empty;
        if (response_data == null)
        {
            Debug.LogError("OnStatisticsResponse:" + action + " response_data error");
            return;
        }
        byte[] array = XXTEAUtils.Decrypt(response_data, Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
        if (array != null)
        {
            jsonString = Encoding.UTF8.GetString(array);
        }
        JSONObject jSONObject = JSONObject.Parse(jsonString);
        if (jSONObject.ContainsKey("code"))
        {
            string text = jSONObject["code"].ToString();
            if (!(text == "0"))
            {
            }
        }
    }


    public void OnStatisticsRequestError(string action, byte[] post_data)
    {
        //Debug.Log("OnRequestError");
    }

    private void OnApplicationPause(bool pause)
    {
        //Debug.Log("OnApplicationPause:" + pause);
        if (is_enter_tutorial)
        {
            return;
        }
        if (!pause)
        {
            DateTime curDateTime = GetCurDateTime();
            TimeSpan timeSpan = (curDateTime - finally_save_date).Duration();
            if (timeSpan >= TimeSpan.FromMinutes(120.0))
            {
                Debug.Log("Long time no see.");
                TAudioManager.instance.soundVolume = 1f;
                TAudioManager.instance.musicVolume = 1f;
                Time.timeScale = 1f;
                LoadingUIController.FinishedLoading();
                Application.LoadLevel("GameCover");
            }
            PushNotification.ReSetNotifications();
        }
        else
        {
            finally_save_date = GetCurDateTime();
            Hashtable hashtable = new Hashtable();
            hashtable.Add("count", 1);
            Instance.UploadStatistics("Logout", hashtable);
        }
    }

    private string CalculateSimpleHash(string input)
    {
        return SHA256Sample.GetSha256String(input);
    }

    private string GenerateDataHash(Configure configure)
    {
        const string prefix = "s4v3H@sh_";
        const string suffix = "_3ndH@sh!";

        string dataToHash = prefix +
            (configure.GetSingle("Save", "Cash") ?? "0").Trim() +
            (configure.GetSingle("Save", "Crystal") ?? "0").Trim() +
            (configure.GetSingle("Save", "Voucher") ?? "0").Trim() +
            (configure.GetSingle("Save", "LastSafeCash") ?? "0").Trim() +
            (configure.GetSingle("Save", "LastSafeCrystal") ?? "0").Trim() +
            (configure.GetSingle("Save", "LastSafeVoucher") ?? "0").Trim() +
            (configure.GetSingle("Save", "TapjoyPoints") ?? "0").Trim() +
            (configure.GetSingle("Save", "DayLevel") ?? "0").Trim() +
            (configure.GetSingle("Save", "AvatarType") ?? "0").Trim() +
            (configure.GetSingle("Save", "SensitivityRatio") ?? "0").Trim() +
            (configure.GetSingle("Save", "Version") ?? "").Trim() +
            (configure.GetSingle("Save", "EnterShopCount") ?? "0").Trim() +
            (configure.GetSingle("Save", "NickName") ?? "").Trim() +
            suffix;

        return CalculateSimpleHash(dataToHash);
    }


    private bool CheckDataIntegrity(Configure configure)
    {
        string savedHash = configure.GetSingle("Save", "DataHash");
        if (string.IsNullOrEmpty(savedHash))
            return false;

        string computedHash = GenerateDataHash(configure);

        return savedHash == computedHash;
    }

    public static string DataEncipher(string data)
    {
        if (!GameConfig.IsEditorMode())
        {
            string key = GetImportContent();

            string hash = ComputeHMAC(data, key);
            string combined = data + "|HASH|" + hash;

            string enciphered = Encipher(combined);
            return XXTEAUtils.Encrypt(enciphered, key);
        }
        return data;
    }

    public static string DataDecrypt(string encryptedData)
    {
        if (!GameConfig.IsEditorMode())
        {
            string key = GetImportContent();

            string decrypted = XXTEAUtils.Decrypt(encryptedData, key);
            string deciphered = Encipher(decrypted);

            var parts = deciphered.Split(new string[] { "|HASH|" }, StringSplitOptions.None);
            if (parts.Length != 2)
                throw new Exception("Save data corrupted or invalid.");

            string data = parts[0];
            string storedHash = parts[1];
            string computedHash = ComputeHMAC(data, key);

            if (storedHash != computedHash)
                throw new Exception("Save data integrity check failed!");

            return data;
        }
        return encryptedData;
    }

    public static string GetImportContent()
    {
        return "in0yt@n5#f.o71[";
    }

    public static string ComputeHMAC(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using (var hmac = new HMACSHA256(keyBytes))
        {
            var hashBytes = hmac.ComputeHash(dataBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    public static DateTime GetCurDateTime()
    {
        return new DateTime(MiscPlugin.GetIOSYear(), MiscPlugin.GetIOSMonth(), MiscPlugin.GetIOSDay(), MiscPlugin.GetIOSHour(), MiscPlugin.GetIOSMin(), MiscPlugin.GetIOSSec());
    }

    private void OnGameDataVersionDifferent()
    {
        Debug.Log("OnGameDataVersionDifferent...");
        GameConfig.Instance.force_update_local = true;
        GameConfig.Instance.Init();
        SaveData();
    }

    public void SaveIapFailedInfo(string Random, int Rat, string product_Id, string tid, string receipt)
    {
        Iap_failed_info[Random + "|" + Rat + "|" + product_Id + "|" + tid] = receipt;
        SaveData();
    }

    public void RemoveIapFailedInfo(string Random, int Rat, string product_Id, string tid, string receipt)
    {
        if (Iap_failed_info.ContainsKey(Random + "|" + Rat + "|" + product_Id + "|" + tid))
        {
            Iap_failed_info.Remove(Random + "|" + Rat + "|" + product_Id + "|" + tid);
        }
        SaveData();
    }

    public static bool CheckSaveDataVersion()
    {
        string content = string.Empty;
        string filePath = Utils.SavePath() + SHA256Sample.GetSha256String("CoMZ2").Substring(0, 32) + ".bytes";

        if (Utils.FileReadString(filePath, ref content))
        {
            Configure configure = new Configure();
            content = DataDecrypt(content);
            configure.Load(content);

            string saveVersion = configure.GetSingle("Save", "Version");
            string latestVersion = VersionHelper.LoadVersionFromFile();

            if (saveVersion != latestVersion)
            {
                Debug.Log("[Version Check] Save version mismatch: save=" + saveVersion + ", expected=" + latestVersion);
                return true;
            }
        }
        return false;
    }
}