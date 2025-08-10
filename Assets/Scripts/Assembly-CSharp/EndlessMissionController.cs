using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CoMZ2;
using System.Linq;

public class EndlessMissionController : MissionController
{
    private static EndlessMissionController _instance;
    public static EndlessMissionController Instance
    {
        get { return _instance; }
    }
    protected EnemyController boss;

    private const int DayLevelStart = 30;
    private const int DayLevelIncrement = 5;
    private const float DayLevelIncrementInterval = 45f;
    private int lastBossDayLevel = -1;
    private bool isBossCoroutineRunning = false;
    private float bossSpawnLockTimestamp = 0f;
    EnemyType lastSpawnedBossType = EnemyType.E_NONE;

    private Coroutine bossCoroutine;
    private bool spawnBossInProgress = false;
    private bool bossActive = false;
    private List<EnemyType> spawnedBossTypes = new List<EnemyType>();
    private static bool spawnLock = false;

    private const float UpdateInfoRate = 0.3f;

    public float mission_life_time = 0f;

    protected bool mission_started;
    private float last_update_info_time = 0f;

    public PlayingState GamePlayingState = PlayingState.Gaming;

    private int maxEnemyCount = 12;
    private float spawnRate = 0.75f;

    public float hpDropChance = 0.25f;
    public float bulletDropChance = 0.5f;

    private int currentBossWaveDay = -1;
    private int lastBossSpawnedDay = -1;
    public float crystalDropChance = 1f;
    public float maxCrystalDropChance = 10f;
    public float crystalChanceIncreaseAmount = 0.5f;
    private float crystalChanceInterval = 120f;
    private float lastCrystalChanceIncreaseTime = 0f;

    public float goldDropChance = 1f;
    public float maxGoldDropChance = 15f;
    public float goldChanceIncreaseAmount = 1f;
    private float goldChanceInterval = 60f;
    private float lastGoldChanceIncreaseTime = 0f;
    private int lastSpawnedBossWaveDay = -1;
    public EnemyController Boss;

    private int bossSpawnCount = 0;
    public int goldCollected = 0;
    public int crystalsCollected = 0;

    private float lastBossSpawnTime = -999f;
    private const float BossSpawnCooldown = 1.5f;

    public Text enemyCounterText;

    private int originalDayLevel;
    private bool originalDayLevelSet = false;

    private float dayLevelTimer = 0f;

    private bool isSpawningBoss = false;
    private Coroutine spawnCoroutine;
    private Coroutine scaleCoroutine;

    private int currentSpawnDayLevel = -1;
    private List<EnemyWaveInfo> currentEnemyWaveInfoSet = null;

    protected bool is_logic_paused = false;
    protected bool mission_check_finished = false;

    private bool isMissionEnding = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override IEnumerator Start()
    {
        foreach (var key in GameConfig.Instance.EnemyWaveInfo_Endless_Bosses.Keys)
            Debug.Log("Boss wave config key: " + key);

        if (!originalDayLevelSet)
        {
            originalDayLevel = GameData.Instance.day_level;
            originalDayLevelSet = true;
        }

        if (GameData.Instance.cur_quest_info == null ||
            GameData.Instance.cur_quest_info.mission_day_type != MissionDayType.Endless)
        {
            Destroy(gameObject);
            yield break;
        }

        originalDayLevel = GameData.Instance.day_level;
        int missionDayLevel = DayLevelStart;

        GameData.Instance.day_level = missionDayLevel;
        if (GameSceneController.Instance != null)
        {
            GameSceneController.Instance.UpdateEndlessEnemyStandardStats(missionDayLevel);

            List<EnemyType> allTypesForDay = GetMissionEnemyTypeList(missionDayLevel);

            if (GameSceneController.Instance.enemy_ref_map != null)
            {
                List<EnemyType> allRequiredTypes = new List<EnemyType>(allTypesForDay);

                EnemyType[] bossesToForceLoad = new EnemyType[]
                {
                EnemyType.E_FATCOOK,
                EnemyType.E_FATCOOK_E,
                EnemyType.E_HAOKE_A,
                EnemyType.E_HAOKE_B,
                EnemyType.E_WRESTLER,
                EnemyType.E_WRESTLER_E,
                EnemyType.E_HALLOWEEN,
                EnemyType.E_HALLOWEEN_E,
                EnemyType.E_SHARK,
                EnemyType.E_SHARK_E,

                EnemyType.E_BOOMER_TIMER,
                EnemyType.E_BOOMER_TIMER_E,
                EnemyType.E_HALLOWEEN_SUB,
                EnemyType.E_HALLOWEEN_SUB_E
                };

                foreach (var boss in bossesToForceLoad)
                {
                    if (!allRequiredTypes.Contains(boss))
                        allRequiredTypes.Add(boss);
                }

                GameSceneController.Instance.enemy_ref_map.ResetEnemyMapInfo(allRequiredTypes);
            }

            currentSpawnDayLevel = missionDayLevel;
            currentEnemyWaveInfoSet = GetEnemyWaveInfoForCurrentDay(currentSpawnDayLevel);
        }

        InitMissionController();
        mission_type = MissionType.Endless;
        crystalsCollected = 0;
        mission_started = false;

        yield return null;

        if (enemyCounterText == null)
        {
            GameObject counterGO = GameObject.Find("EnemyCounterText");
            if (counterGO != null)
                enemyCounterText = counterGO.GetComponent<Text>();
        }

        PlayerController player = null;
        while (player == null)
        {
            yield return null;
            if (GameSceneController.Instance != null)
                player = GameSceneController.Instance.player_controller;
        }

        while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
            yield return null;

        mission_started = true;
        lastCrystalChanceIncreaseTime = Time.time;
        lastGoldChanceIncreaseTime = Time.time;

        scaleCoroutine = StartCoroutine(ScaleSpawnCap());
        yield return new WaitForSeconds(1f);
        spawnCoroutine = StartCoroutine(StartSpawningEnemies());

        Debug.Log("[Endless] Mission started.");
    }

    private float lastDebugLogTime;

    public override void Update()
    {
        base.Update();

        if (GameSceneController.Instance == null) return;

        if (GameSceneController.Instance.GamePlayingState == PlayingState.CG ||
            !mission_started || is_mission_finished || is_mission_paused)
        {
            return;
        }

        mission_life_time += Time.deltaTime;
        dayLevelTimer += Time.deltaTime;

        if (dayLevelTimer >= DayLevelIncrementInterval)
        {
            dayLevelTimer = 0f;
            GameData.Instance.day_level += DayLevelIncrement;

            int currentDay = GameData.Instance.day_level;
            Debug.Log("[Endless] Day level incremented to: " + currentDay);

            GameSceneController.Instance.UpdateEndlessEnemyStandardStats(currentDay);
            CaculateDifficulty();

            if (currentDay != currentSpawnDayLevel)
            {
                currentSpawnDayLevel = currentDay;

                List<EnemyType> enemyTypesForDay = GetMissionEnemyTypeList(currentDay);

                if (IsBossDay(currentDay))
                {
                    List<EnemyWaveInfo> bossWaveInfo = GetBossWaveInfoForDay(currentDay);

                    HashSet<EnemyType> uniqueBossTypes = new HashSet<EnemyType>();
                    foreach (EnemyWaveInfo wave in bossWaveInfo)
                    {
                        foreach (EnemySpawnInfo spawn in wave.spawn_info_list)
                        {
                            uniqueBossTypes.Add(spawn.EType);
                        }
                    }

                    foreach (EnemyType bossType in uniqueBossTypes)
                    {
                        if (!enemyTypesForDay.Contains(bossType))
                            enemyTypesForDay.Add(bossType);
                    }

                    Debug.Log("[Endless] Boss types added to enemy list for day " + currentDay);
                }

                if (GameSceneController.Instance.enemy_ref_map != null)
                {
                    GameSceneController.Instance.enemy_ref_map.ResetEnemyMapInfo(enemyTypesForDay);
                    Debug.Log("[Endless] Enemy ref map reset for day " + currentDay);
                }

                currentEnemyWaveInfoSet = GetEnemyWaveInfoForCurrentDay(currentDay);
            }
        }

        TrySpawnBoss();

        if (Time.time - lastCrystalChanceIncreaseTime >= crystalChanceInterval)
        {
            lastCrystalChanceIncreaseTime = Time.time;
            crystalDropChance = Mathf.Min(crystalDropChance + crystalChanceIncreaseAmount, maxCrystalDropChance);
        }

        if (Time.time - lastGoldChanceIncreaseTime >= goldChanceInterval)
        {
            lastGoldChanceIncreaseTime = Time.time;
            goldDropChance = Mathf.Min(goldDropChance + goldChanceIncreaseAmount, maxGoldDropChance);
        }

        if (Time.time - last_update_info_time >= UpdateInfoRate)
        {
            last_update_info_time = Time.time;
            UpdateEnemyCounterUI();
        }
    }

    private IEnumerator HandleBossWaveUsingBossConfig(int dayLevel)
    {
        Debug.Log("[BossCoroutine] Starting for day " + dayLevel);
        spawnBossInProgress = true;

        spawnedBossTypes.Clear();

        List<EnemyWaveInfo> waveList = GetBossWaveInfoForDay(dayLevel);
        if (waveList == null || waveList.Count == 0)
        {
            Debug.Log("[Endless] No boss wave data found for day " + dayLevel);
            spawnBossInProgress = false;
            bossActive = false;
            bossCoroutine = null;
            yield break;
        }

        int bossCount = GetBossCountForDay(dayLevel);
        int spawnedCount = 0;

        List<EnemyType> availableBosses = new List<EnemyType>();
        foreach (EnemyWaveInfo wave in waveList)
        {
            foreach (EnemySpawnInfo spawnInfo in wave.spawn_info_list)
            {
                if (!availableBosses.Contains(spawnInfo.EType))
                    availableBosses.Add(spawnInfo.EType);
            }
        }

        while (spawnedCount < bossCount && availableBosses.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, availableBosses.Count);
            EnemyType selectedBoss = availableBosses[index];

            GameObject grave = FindClosedGrave(GameSceneController.Instance.player_controller.transform.position);
            if (grave == null)
            {
                Debug.LogWarning("[Endless] No grave found for boss spawn: " + selectedBoss);
                break;
            }

            EnemyController boss = SpawnBossFromNest(selectedBoss, grave);
            if (boss != null)
            {
                if (GameData.Instance.cur_quest_info != null &&
                    GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Endless)
                {
                    float originalHp = boss.enemy_data.hp_capacity;
                    boss.enemy_data.hp_capacity *= 2.5f;
                    boss.enemy_data.cur_hp = boss.enemy_data.hp_capacity;

                    Debug.Log(string.Format("[Endless] Boss spawned: {0}, HP scaled from {1} to {2}",
                        selectedBoss, originalHp, boss.enemy_data.cur_hp));
                }
                else
                {
                    Debug.Log("[Endless] Boss spawned: " + selectedBoss + ", HP: " + boss.enemy_data.cur_hp);
                }

                GameSceneController.Instance.SetBossData(boss.enemy_data);
                boss.IsBoss = true;
                boss.enemyType = selectedBoss;
                boss.ignoreSpawnLimit = true;

                spawnedBossTypes.Add(selectedBoss);
                spawnedCount++;
            }
            else
            {
                Debug.LogError("[Endless] Failed to spawn boss: " + selectedBoss);
            }

            availableBosses.RemoveAt(index);

            yield return new WaitForSeconds(3f);
        }

        Debug.Log("[BossCoroutine] Boss wave spawned, finishing coroutine.");

        spawnBossInProgress = false;
        bossActive = false;
        bossCoroutine = null;
        bossSpawnLockTimestamp = Time.time + 3f;
        spawnLock = false;
    }

    private bool IsBossTypeAlive(EnemyType bossType)
    {
        if (GameSceneController.Instance == null)
            return false;

        foreach (var enemyPair in GameSceneController.Instance.Enemy_Set)
        {
            EnemyController enemy = enemyPair.Value;
            if (enemy != null && enemy.IsBoss && enemy.enemyType == bossType && enemy.enemy_data.cur_hp > 0)
            {
                return true;
            }
        }
        return false;
    }

    private List<EnemyWaveInfo> GetBossWaveInfoForDay(int dayLevel)
    {
        var waveDict = GameConfig.Instance.EnemyWaveInfo_Endless_Bosses;
        List<int> sortedKeys = new List<int>(waveDict.Keys);
        sortedKeys.Sort();

        if (waveDict.ContainsKey(dayLevel))
        {
            return waveDict[dayLevel].wave_info_list;
        }

        Debug.LogWarning("[Endless] No exact boss wave key found for day " + dayLevel);
        return new List<EnemyWaveInfo>();
    }

    void TrySpawnBoss()
    {
        int currentDay = GameData.Instance.day_level;

        if (lastSpawnedBossWaveDay == currentDay)
            return;

        var bossWave = GetBossWaveInfoForDay(currentDay);
        if (bossWave == null || bossWave.Count == 0)
            return;

        lastSpawnedBossWaveDay = currentDay;
        spawnLock = true;
        spawnBossInProgress = true;

        Debug.Log("[TrySpawnBoss] Spawning boss wave for day " + currentDay);
        bossCoroutine = StartCoroutine(HandleBossWaveUsingBossConfig(currentDay));
    }

    private int GetBossCountForDay(int dayLevel)
    {
        if (!GameConfig.Instance.EnemyWaveInfo_Endless_Bosses.ContainsKey(dayLevel))
            return 0;

        var waveList = GameConfig.Instance.EnemyWaveInfo_Endless_Bosses[dayLevel].wave_info_list;
        HashSet<EnemyType> uniqueBossTypes = new HashSet<EnemyType>();

        int count = 0;
        foreach (var wave in waveList)
        {
            foreach (var spawn in wave.spawn_info_list)
            {
                if (IsBossType(spawn.EType))
                {
                    uniqueBossTypes.Add(spawn.EType);
                    count += spawn.Count;
                }
            }
        }

        return count > 0 ? count : 1;
    }

    private IEnumerator StartSpawningEnemies()
    {
        int lastCheckedBossDay = -1;

        while (mission_started)
        {
            if (GameSceneController.Instance == null)
            {
                yield return null;
                continue;
            }

            int cappedEnemyCount = GameSceneController.Instance.Enemy_Set.Values
                .Count(e => e != null && !e.ignoreSpawnLimit);

            if (cappedEnemyCount < maxEnemyCount)
            {
                if (GameData.Instance.day_level != lastCheckedBossDay)
                {
                    lastCheckedBossDay = GameData.Instance.day_level;
                }

                if (currentEnemyWaveInfoSet == null || currentEnemyWaveInfoSet.Count == 0)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                int waveIndex = UnityEngine.Random.Range(0, currentEnemyWaveInfoSet.Count);
                EnemyWaveInfo wave = currentEnemyWaveInfoSet[waveIndex];

                EnemySpawnInfo selected = null;
                if (wave.spawn_info_list != null && wave.spawn_info_list.Count > 0)
                {
                    selected = wave.spawn_info_list[UnityEngine.Random.Range(0, wave.spawn_info_list.Count)];
                }

                if (selected != null &&
                    IsBossType(selected.EType))
                {
                    yield return new WaitForSeconds(spawnRate);
                    continue;
                }

                if (selected != null &&
                    GameSceneController.Instance.enemy_ref_map.Enemy_Set.ContainsKey(selected.EType))
                {
                    GameObject spawnTarget = null;

                    if (selected.From == SpawnFromType.Grave)
                    {
                        spawnTarget = FindClosedGrave(GameSceneController.Instance.player_controller.transform.position);
                        if (spawnTarget != null)
                            SpwanZombiesFromGrave(selected.EType, spawnTarget, false);
                    }
                    else if (selected.From == SpawnFromType.Nest)
                    {
                        spawnTarget = FindClosedNest(GameSceneController.Instance.player_controller.transform.position);
                        if (spawnTarget != null)
                            SpwanZombiesFromNest(selected.EType, spawnTarget);
                    }
                }
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    private GameObject FindClosedNest(Vector3 playerPos)
    {
        GameObject[] nests = GameObject.FindGameObjectsWithTag("Zombie_Nest");
        Debug.Log("[Endless] Nests found: " + nests.Length);
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (GameObject nest in nests)
        {
            float dist = Vector3.Distance(playerPos, nest.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = nest;
            }
        }

        return closest;
    }

    private void SpawnEnemyFromSource(EnemySpawnInfo spawnInfo, Vector3 playerPos)
    {
        if (spawnInfo == null) return;

        GameObject source = null;

        switch (spawnInfo.From)
        {
            case SpawnFromType.Grave:
                source = FindClosedGrave(playerPos);
                break;
            case SpawnFromType.Nest:
                source = FindClosedNest(playerPos);
                break;
        }

        if (source != null)
        {
            if (GameSceneController.Instance.enemy_ref_map.Enemy_Set.ContainsKey(spawnInfo.EType))
            {
                EnemyFactory.CreateEnemyGetEnemyController(spawnInfo.EType, source.transform.position, source.transform.rotation);
            }
            else
            {
                //Debug.LogWarning("[Endless] EnemyType not found in ref map: " + spawnInfo.EType);
            }
        }
        else
        {
            //Debug.LogWarning("[Endless] No spawn source found for type: " + spawnInfo.From);
        }
    }

    public EnemyController SpawnBossFromNest(EnemyType bossType, GameObject grave)
    {
        if (grave == null)
        {
            //Debug.LogError("[Endless] SpawnBossFromNest failed: grave is null.");
            return null;
        }

        EnemyController boss = EnemyFactory.CreateEnemyGetEnemyController(
            bossType,
            grave.transform.position,
            grave.transform.rotation
        );

        if (boss == null)
        {
            Debug.LogError("[Endless] SpawnBossFromNest failed to create boss of type: " + bossType);
            return null;
        }

        GameSceneController.Instance.SetBossData(boss.enemy_data);

        boss.IsBoss = true;
        boss.enemyType = bossType;

        Debug.Log("[Endless] Boss spawned: " + bossType + ", Base HP: " + boss.enemy_data.hp_capacity);

        return boss;
    }

    public override List<EnemyType> GetMissionEnemyTypeList()
    {
        return GetMissionEnemyTypeList(GameData.Instance.day_level);
    }

    public List<EnemyType> GetMissionEnemyTypeList(int dayLevel)
    {
        List<EnemyWaveInfo> waveList = GetEnemyWaveInfoForCurrentDay(dayLevel);

        List<EnemyType> enemyTypes = new List<EnemyType>();
        foreach (EnemyWaveInfo wave in waveList)
        {
            foreach (EnemySpawnInfo spawn in wave.spawn_info_list)
            {
                if (!enemyTypes.Contains(spawn.EType))
                    enemyTypes.Add(spawn.EType);
            }
        }

        return enemyTypes;
    }

    private List<EnemyWaveInfo> GetEnemyWaveInfoForCurrentDay(int dayLevel)
    {
        int selectedKey = -1;
        var keys = GameConfig.Instance.EnemyWaveInfo_Endless_Set.Keys;

        foreach (int key in keys)
        {
            if (key >= dayLevel && (selectedKey == -1 || key < selectedKey))
                selectedKey = key;
        }
        if (selectedKey == -1)
        {
            foreach (int key in keys)
            {
                if (key <= dayLevel && (selectedKey == -1 || key > selectedKey))
                    selectedKey = key;
            }
        }
        Debug.Log("[Endless] Selected enemy wave key for day " + dayLevel + " is: " + selectedKey);
        if (selectedKey != -1)
            return GameConfig.Instance.EnemyWaveInfo_Endless_Set[selectedKey].wave_info_list;

        Debug.LogWarning("[Endless] No enemy wave data found for day " + dayLevel);
        return new List<EnemyWaveInfo>();
    }

    private bool IsBossDay(int dayLevel)
    {
        var waveDict = GameConfig.Instance.EnemyWaveInfo_Endless_Bosses;

        if (waveDict == null || waveDict.Count == 0)
            return false;

        int maxKey = -1;
        foreach (int key in waveDict.Keys)
        {
            if (key <= dayLevel && key > maxKey)
                maxKey = key;
        }

        if (maxKey == -1)
            return false;

        int interval = GameConfig.Instance.bossUpgradeInterval;

        if (dayLevel >= maxKey && ((dayLevel - maxKey) % interval == 0))
        {
            //Debug.Log("[Endless] IsBossDay: TRUE by fallback logic (day " + dayLevel + " using key " + maxKey + ")");
            return true;
        }

        return waveDict.ContainsKey(dayLevel);
    }

    private void UpdateEnemyCounterUI()
    {
        if (enemyCounterText == null || GameSceneController.Instance == null) return;

        TimeSpan timeSpan = TimeSpan.FromSeconds((int)mission_life_time);
        GameSceneController.Instance.game_main_panel.time_alive_panel.SetContent(
            timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2"));

        // Count only enemies affected by the spawn cap
        int count = GameSceneController.Instance.Enemy_Set.Values.Count(
            e => e != null && !e.ignoreSpawnLimit);

        int dayLevel = GameData.Instance.day_level;
        string waveLabel = "Wave " + dayLevel;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Day: " + dayLevel + " (" + waveLabel + ")");
        sb.AppendLine("Enemies: " + count + " / " + maxEnemyCount);
        sb.AppendLine("Spawn Rate: " + (1f / spawnRate).ToString("F1") + " per sec");
        sb.AppendLine("Crystal Drop: " + crystalDropChance.ToString("F1") + "%");
        sb.AppendLine("Crystals Collected: " + crystalsCollected);
        sb.AppendLine("Gold Drop: " + goldDropChance.ToString("F1") + "%");
        sb.AppendLine("Gold Collected: " + goldCollected);

        enemyCounterText.text = sb.ToString();
    }

    private IEnumerator ScaleSpawnCap()
    {
        int previousLevel = GameData.Instance.day_level;

        while (mission_started)
        {
            yield return new WaitForSeconds(1f);
            int currentLevel = GameData.Instance.day_level;

            if (currentLevel != previousLevel)
            {
                previousLevel = currentLevel;
                int levelIndex = (currentLevel - DayLevelStart) / DayLevelIncrement;

                maxEnemyCount = 12 + (4 * levelIndex);

                float baseRate = 0.90f;
                float minRate = 0.02f;
                spawnRate = Mathf.Max(minRate, 0.75f * Mathf.Pow(baseRate, levelIndex));

                Debug.Log("[Endless] Day " + currentLevel +
                          " | LevelIndex=" + levelIndex +
                          " | SpawnRate=" + spawnRate);
            }
        }
    }

    private bool IsBossType(EnemyType type)
    {
        return type == EnemyType.E_FATCOOK ||
               type == EnemyType.E_FATCOOK_E ||
               type == EnemyType.E_HAOKE_A ||
               type == EnemyType.E_HAOKE_B ||
               type == EnemyType.E_WRESTLER ||
               type == EnemyType.E_WRESTLER_E ||
               type == EnemyType.E_HALLOWEEN ||
               type == EnemyType.E_HALLOWEEN_E ||
               type == EnemyType.E_SHARK ||
               type == EnemyType.E_SHARK_E;
    }

    public void PauseEndlessGame()
    {
        if (is_mission_paused) return;

        is_mission_paused = true;
        is_logic_paused = true;
        Time.timeScale = 0f;

        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);

        if (GameSceneController.Instance != null && GameSceneController.Instance.game_pause_panel != null)
        {
            GameSceneController.Instance.game_pause_panel.Show();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("[Endless] Game paused.");
    }

    public IEnumerator HalloweenSummon(EnemyController boss)
    {
        if (boss == null)
        {
            Debug.LogWarning("HalloweenSummon called with null boss");
            yield break;
        }

        EnemyType bossType = boss.enemy_data.enemy_type;

        EnemyType minionType = EnemyType.E_HALLOWEEN_SUB;
        int minionCount = 1;

        if (bossType == EnemyType.E_HALLOWEEN_E)
        {
            minionType = EnemyType.E_HALLOWEEN_SUB_E;
            minionCount = 2;
        }

        Debug.Log("[HalloweenSummon] Boss enemy type: " + bossType);

        GameObject grave = FindClosedGrave(boss.transform.position);
        if (grave == null)
        {
            Debug.LogWarning("No grave found near boss for minion spawn.");
            yield break;
        }

        for (int i = 0; i < minionCount; i++)
        {
            EnemyController minion = SpwanZombiesFromGrave(minionType, grave, true);
            if (minion != null)
            {
                minion.gameObject.SetActive(true);
                minion.ignoreSpawnLimit = true;

                if (!GameSceneController.Instance.Enemy_Set.ContainsKey(minion.EnemyID))
                {
                    GameSceneController.Instance.Enemy_Set.Add(minion.EnemyID, minion);
                }
            }
            else
            {
                Debug.LogWarning("Minion spawn failed on index " + i);
            }

            yield return null;
        }
    }

    private EnemyData CreateReplicatedEnemyData(EnemyData source)
    {
        if (source == null)
        {
            Debug.LogError("[CreateReplicatedEnemyData] Source is null.");
            return null;
        }

        if (source.config == null)
        {
            Debug.LogError("[CreateReplicatedEnemyData] Source config is null.");
            return null;
        }

        EnemyData clone = new EnemyData();
        clone.config = source.config;

        float ratio = 0.15f;
        object obj;

        if (source.config.Ex_conf != null && source.config.Ex_conf.ContainsKey("replicationHpRatio"))
        {
            obj = source.config.Ex_conf["replicationHpRatio"];
            ratio = (float)obj;
        }

        clone.hp_capacity = (int)(source.hp_capacity * ratio);
        clone.cur_hp = clone.hp_capacity;

        Debug.Log("[CreateReplicatedEnemyData] Clone created. HP: " + clone.hp_capacity + ", Config: " + (clone.config != null));

        return clone;
    }

    public void ResumeEndlessGame()
    {
        if (!is_mission_paused) return;

        is_mission_paused = false;
        is_logic_paused = false;
        Time.timeScale = 1f;

        spawnCoroutine = StartCoroutine(StartSpawningEnemies());
        scaleCoroutine = StartCoroutine(ScaleSpawnCap());

        if (GameSceneController.Instance != null && GameSceneController.Instance.game_pause_panel != null)
        {
            GameSceneController.Instance.game_pause_panel.Hide();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("[Endless] Game resumed.");
    }

    private void StopMissionLogic()
    {
        if (isMissionEnding) return;
        isMissionEnding = true;

        mission_started = false;
        is_mission_finished = true;
        is_mission_paused = true;
        is_logic_paused = true;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }

        Time.timeScale = 1f;
    }

    public virtual void MissionFailed()
    {
        Debug.Log("[EndlessMissionController] MissionFailed() called.");

        if (is_mission_finished)
        {
            Debug.Log("[EndlessMissionController] Mission already finished, ignoring.");
            return;
        }

        StopMissionLogic();

        GameData.Instance.day_level = originalDayLevel;
        Debug.Log("[EndlessMissionController] Resetting day level to: " + originalDayLevel);

        GameData.Instance.SaveData();

        Invoke("SetLoseState", 1f);
    }

    public override void MissionFinished()
    {
        Debug.Log("[EndlessMissionController] MissionFinished called. Restoring day level to " + originalDayLevel);

        StopMissionLogic();

        GameData.Instance.day_level = originalDayLevel;
        GameData.Instance.SaveData();

        if (enemyCounterText != null)
            enemyCounterText.text = "";

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        StopMissionLogic();

        GameData.Instance.day_level = originalDayLevel;
        GameData.Instance.SaveData();

        Debug.Log("[EndlessMissionController] OnDisable resetting day level to: " + originalDayLevel);
    }

    private void OnDestroy()
    {
        StopMissionLogic();

        GameData.Instance.day_level = originalDayLevel;
        GameData.Instance.SaveData();

        Debug.Log("[EndlessMissionController] OnDestroy resetting day level to: " + originalDayLevel);
    }

    private void OnApplicationQuit()
    {
        GameData.Instance.day_level = originalDayLevel;
        GameData.Instance.SaveData();
        Debug.Log("[EndlessMissionController] OnApplicationQuit resetting day level to: " + originalDayLevel);
    }
}
