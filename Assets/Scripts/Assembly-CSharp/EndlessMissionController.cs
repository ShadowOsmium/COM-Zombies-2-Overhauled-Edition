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

    private const int DayLevelStart = 45;
    private const int DayLevelIncrement = 5;
    private const float DayLevelIncrementInterval = 30f;
    private int lastBossDayLevel = -1;
    private bool isBossCoroutineRunning = false;
    private float bossSpawnLockTimestamp = 0f;
    private float bossSpawnLockDuration = 5f;

    private const float SpawnIncreaseInterval = 30f;

    private Coroutine bossCoroutine = null;

    private bool bossActive = false;

    private const float UpdateInfoRate = 0.3f;

    private int lastLoggedDay = -1;
    private float lastBossSpawnLogTime = 0f;
    private const float bossSpawnLogCooldown = 5f;  // seconds
    private int lastLoggedDayForSpawn = -1;

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

    public int goldCollected = 0;
    public int crystalsCollected = 0;

    private bool spawnBossInProgress = false;
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
    private HashSet<EnemyType> spawnedBossTypes = new HashSet<EnemyType>();

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
            /*
            if (!allTypesForDay.Contains(EnemyType.E_FATCOOK_E))
                allTypesForDay.Add(EnemyType.E_FATCOOK_E);
            // if (!allTypesForDay.Contains(EnemyType.E_HALLOWEEN_E))
            //     allTypesForDay.Add(EnemyType.E_HALLOWEEN_E);*/

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
            lastSpawnedBossWaveDay = -1;

            int currentDay = GameData.Instance.day_level;
            Debug.Log("[Endless] Day level incremented to: " + currentDay);

            GameSceneController.Instance.UpdateEndlessEnemyStandardStats(currentDay);
            CaculateDifficulty();

            if (currentDay != currentSpawnDayLevel)
            {
                currentSpawnDayLevel = currentDay;

                // Get enemy types for the new day
                List<EnemyType> enemyTypesForDay = GetMissionEnemyTypeList(currentDay);

                if (IsBossDay(currentDay))
                {
                    List<EnemyWaveInfo> bossWaveInfo = GetBossWaveInfoForDay(currentDay);
                    int bossCount = GetBossCountForDay(currentDay);
                    List<EnemyType> bossTypes = GetBossTypesFromWaveList(bossWaveInfo, bossCount);

                    foreach (EnemyType bossType in bossTypes)
                    {
                        if (!enemyTypesForDay.Contains(bossType))
                            enemyTypesForDay.Add(bossType);
                    }

                    Debug.Log("[Endless] Boss types added to enemy list for day " + currentDay);
                }

                // Update enemy map
                if (GameSceneController.Instance.enemy_ref_map != null)
                {
                    GameSceneController.Instance.enemy_ref_map.ResetEnemyMapInfo(enemyTypesForDay);
                    Debug.Log("[Endless] Enemy ref map reset for day " + currentDay);
                }

                // Refresh current enemy wave info
                currentEnemyWaveInfoSet = GetEnemyWaveInfoForCurrentDay(currentDay);
            }
        }

        // 💣 Boss spawn attempt
        TrySpawnBoss();

        // 💰 Crystal drop chance increase
        if (Time.time - lastCrystalChanceIncreaseTime >= crystalChanceInterval)
        {
            lastCrystalChanceIncreaseTime = Time.time;
            crystalDropChance = Mathf.Min(crystalDropChance + crystalChanceIncreaseAmount, maxCrystalDropChance);
        }

        // 💰 Gold drop chance increase
        if (Time.time - lastGoldChanceIncreaseTime >= goldChanceInterval)
        {
            lastGoldChanceIncreaseTime = Time.time;
            goldDropChance = Mathf.Min(goldDropChance + goldChanceIncreaseAmount, maxGoldDropChance);
        }

        // 🧠 Update enemy counter
        if (Time.time - last_update_info_time >= UpdateInfoRate)
        {
            last_update_info_time = Time.time;
            UpdateEnemyCounterUI();
        }
    }

    private IEnumerator HandleBossWaveUsingBossConfig(int dayLevel)
    {
        yield return null;

        spawnedBossTypes.Clear();

        Debug.Log("[Endless] Boss wave triggered at day " + dayLevel);

        List<EnemyWaveInfo> waveList = GetBossWaveInfoForDay(dayLevel);
        if (waveList == null || waveList.Count == 0)
        {
            Debug.LogWarning("[Endless] No boss wave data found for day " + dayLevel);
            bossCoroutine = null;
            spawnBossInProgress = false;
            bossActive = false;
            isBossCoroutineRunning = false;
            yield break;
        }

        GameObject grave = FindClosedGrave(GameSceneController.Instance.player_controller.transform.position);
        Debug.Log("[Endless] Grave found: " + (grave != null ? grave.name : "null"));

        int bossCount = GetBossCountForDay(dayLevel);
        List<EnemyType> bossTypesForDay = GetBossTypesFromWaveList(waveList, bossCount);
        int spawnedCount = 0;

        foreach (EnemyType bossType in bossTypesForDay)
        {
            if (spawnedCount >= bossCount)
                break;

            if (spawnedBossTypes.Contains(bossType))
                continue;

            if (grave == null)
            {
                Debug.LogWarning("[Endless] No grave found for boss spawn: " + bossType);
                continue;
            }

            boss = SpawnBossFromNest(bossType, grave);
            GameSceneController.Instance.SetBossData(boss.enemy_data);

            if (boss != null)
            {
                GameSceneController.Instance.SetBossData(boss.enemy_data);
                Debug.Log("[Endless] Boss spawned: " + bossType + ", HP: " + boss.enemy_data.cur_hp);
                boss.IsBoss = true;
                boss.enemyType = bossType;
                boss.ignoreSpawnLimit = true;
                spawnedBossTypes.Add(bossType);
                spawnedCount++;
            }
            else
            {
                Debug.LogError("[Endless] Failed to spawn boss: " + bossType);
            }

            yield return new WaitForSeconds(3f);
        }

        Debug.Log("[Endless] Boss wave spawned, coroutine finishing.");

        spawnBossInProgress = false;
        bossActive = false;
        bossCoroutine = null;

        bossSpawnLockTimestamp = Time.time + 3f;
    }

    private List<EnemyWaveInfo> GetBossWaveInfoForDay(int dayLevel)
    {
        var waveDict = GameConfig.Instance.EnemyWaveInfo_Endless_Bosses;
        List<EnemyWaveInfo> result = new List<EnemyWaveInfo>();

        int maxKey = -1;
        foreach (int key in waveDict.Keys)
        {
            if (key <= dayLevel && key > maxKey)
                maxKey = key;
        }

        if (maxKey == -1)
            return result;

        List<EnemyWaveInfo> originalWaves = waveDict[maxKey].wave_info_list;

        int intervalsPassed = (dayLevel - maxKey) / GameConfig.Instance.bossUpgradeInterval;
        float baseChance = GameConfig.Instance.bossUpgradeChance;

        foreach (EnemyWaveInfo wave in originalWaves)
        {
            EnemyWaveInfo clonedWave = new EnemyWaveInfo();

            foreach (EnemySpawnInfo spawn in wave.spawn_info_list)
            {
                EnemySpawnInfo newSpawn = spawn.Clone();
                newSpawn.TryApplyBoost(intervalsPassed, baseChance);
                clonedWave.spawn_info_list.Add(newSpawn);
            }

            result.Add(clonedWave);
        }

        return result;
    }

    private List<EnemyType> GetBossTypesFromWaveList(List<EnemyWaveInfo> waveList, int maxCount)
    {
        List<EnemyType> types = new List<EnemyType>();

        foreach (EnemyWaveInfo wave in waveList)
        {
            foreach (EnemySpawnInfo info in wave.spawn_info_list)
            {
                if (!types.Contains(info.EType))
                {
                    types.Add(info.EType);
                    if (types.Count >= maxCount)
                        return types;
                }
            }
        }
        Debug.Log("[Endless] Boss types found: " + string.Join(", ", types.ConvertAll(t => t.ToString()).ToArray()));
        return types;
    }

    void TrySpawnBoss()
    {
        int currentDay = GameData.Instance.day_level;
        float now = Time.time;

        // Optional: Log TrySpawnBoss call once per day (to keep logs clean)
        if (currentDay != lastLoggedDay)
        {
            Debug.Log("[Endless] TrySpawnBoss() called at day " + currentDay);
            lastLoggedDay = currentDay;
        }

        // Check if it's a boss day
        if (!IsBossDay(currentDay))
            return;

        // Prevent spawning if already in progress or already spawned this wave for this day
        if (spawnBossInProgress || lastSpawnedBossWaveDay == currentDay)
        {
            Debug.LogWarning("[Endless] BLOCKED: spawnBossInProgress is " + spawnBossInProgress +
                             " or boss wave already spawned for day " + currentDay);
            return;
        }

        // Optional: Rate-limit log for passing spawn conditions
        if ((currentDay != lastLoggedDayForSpawn) || (now - lastBossSpawnLogTime > bossSpawnLogCooldown))
        {
            Debug.Log("[Endless] Boss spawn conditions passed for day " + currentDay);
            lastBossSpawnLogTime = now;
            lastLoggedDayForSpawn = currentDay;
        }

        // Set this so next call knows this wave already spawned
        lastSpawnedBossWaveDay = currentDay;
        spawnBossInProgress = true;

        Debug.Log("[Endless] Spawning boss wave for day " + currentDay);

        bossCoroutine = StartCoroutine(HandleBossWaveUsingBossConfig(currentDay));
    }

    private IEnumerator StartSpawningEnemies()
    {
        int lastCheckedBossDay = -1;
        List<EnemyType> currentBossTypes = new List<EnemyType>();

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
                    int bossCount = GetBossCountForDay(GameData.Instance.day_level);
                    List<EnemyWaveInfo> waveList = GetBossWaveInfoForDay(GameData.Instance.day_level);
                    currentBossTypes = GetBossTypesFromWaveList(waveList, bossCount);
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

    private int GetBossCountForDay(int dayLevel)
    {
        if (GameConfig.Instance.EnemyWaveInfo_Endless_Bosses.ContainsKey(dayLevel))
        {
            // Count bosses defined in that day's wave config
            var waveList = GameConfig.Instance.EnemyWaveInfo_Endless_Bosses[dayLevel].wave_info_list;
            int count = 0;
            foreach (var wave in waveList)
            {
                count += wave.spawn_info_list.Count(spawn => IsBossType(spawn.EType));
            }
            return count > 0 ? count : 1; // fallback 1 boss
        }

        return 0; // no bosses this day
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

    private EnemyController SpawnBossFromNest(EnemyType type, GameObject nest)
    {
        if (nest == null)
        {
            //Debug.LogError("[Endless] SpawnBossFromNest failed: nest is null.");
            return null;
        }
        EnemyController boss = EnemyFactory.CreateEnemyGetEnemyController(type, nest.transform.position, nest.transform.rotation);
        GameSceneController.Instance.SetBossData(boss.enemy_data);
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

        // Make sure bosses for this day are included
        int bossCount = GetBossCountForDay(dayLevel);
        List<EnemyType> bossTypes = GetBossTypesFromWaveList(waveList, bossCount);
        List<EnemyType> currentBossTypes = bossTypes;
        foreach (EnemyType bossType in bossTypes)
        {
            if (!enemyTypes.Contains(bossType))
                enemyTypes.Add(bossType);
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
        return GameConfig.Instance.EnemyWaveInfo_Endless_Bosses.ContainsKey(dayLevel);
    }

    private void UpdateEnemyCounterUI()
    {
        if (enemyCounterText == null || GameSceneController.Instance == null) return;

        TimeSpan timeSpan = TimeSpan.FromSeconds((int)mission_life_time);
        GameSceneController.Instance.game_main_panel.time_alive_panel.SetContent(
            timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2"));

        int count = GameSceneController.Instance.Enemy_Set.Count;
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
                spawnRate = Mathf.Max(0.05f, 0.75f * Mathf.Pow(0.80f, levelIndex));
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

        EnemyType bossType = boss.enemy_data.enemy_type; // Use enemy_data.enemy_type (safer)

        EnemyType minionType = EnemyType.E_HALLOWEEN_SUB;
        int minionCount = 2;

        if (bossType == EnemyType.E_HALLOWEEN_E)
        {
            minionType = EnemyType.E_HALLOWEEN_SUB_E;
            minionCount = 3;
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

        float ratio = 0.15f; // fallback
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
