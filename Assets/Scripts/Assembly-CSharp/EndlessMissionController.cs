using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CoMZ2;

public class EndlessMissionController : MissionController
{
    private static EndlessMissionController _instance;
    public static EndlessMissionController Instance
    {
        get { return _instance; }
    }

    // Configurable constants
    private const int DayLevelStart = 30;
    private const int DayLevelIncrement = 5;
    private const float DayLevelIncrementInterval = 45f;

    private const float SpawnIncreaseInterval = 30f;

    private const float UpdateInfoRate = 0.3f;

    public float mission_life_time = 0f;

    protected bool mission_started;
    private float last_update_info_time = 0f;

    public PlayingState GamePlayingState = PlayingState.Gaming;

    private int maxEnemyCount = 12;
    private float spawnRate = 0.75f;

    public float hpDropChance = 0.25f;
    public float bulletDropChance = 0.5f;

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

    public int goldCollected = 0;
    public int crystalsCollected = 0;

    public Text enemyCounterText;

    private int originalDayLevel;
    private bool originalDayLevelSet = false;

    private float dayLevelTimer = 0f;

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

            List<EnemyType> enemyTypesForDay = GetMissionEnemyTypeList(missionDayLevel);
            if (GameSceneController.Instance.enemy_ref_map != null)
            {
                GameSceneController.Instance.enemy_ref_map.ResetEnemyMapInfo(enemyTypesForDay);
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

    public override void Update()
    {
        base.Update();

        if (GameSceneController.Instance == null) return;

        if (GameSceneController.Instance.GamePlayingState == PlayingState.CG ||
            !mission_started || is_mission_finished || is_mission_paused)
            return;

        mission_life_time += Time.deltaTime;

        dayLevelTimer += Time.deltaTime;
        if (dayLevelTimer >= DayLevelIncrementInterval)
        {
            dayLevelTimer = 0f;
            GameData.Instance.day_level += DayLevelIncrement;

            GameSceneController.Instance.UpdateEndlessEnemyStandardStats(GameData.Instance.day_level);
            CaculateDifficulty();

            if (GameData.Instance.day_level != currentSpawnDayLevel)
            {
                currentSpawnDayLevel = GameData.Instance.day_level;
                List<EnemyType> enemyTypesForDay = GetMissionEnemyTypeList(currentSpawnDayLevel);
                if (GameSceneController.Instance.enemy_ref_map != null)
                {
                    GameSceneController.Instance.enemy_ref_map.ResetEnemyMapInfo(enemyTypesForDay);
                }
                currentEnemyWaveInfoSet = GetEnemyWaveInfoForCurrentDay(currentSpawnDayLevel);
            }
        }

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

    private IEnumerator StartSpawningEnemies()
    {
        while (mission_started)
        {
            if (GameSceneController.Instance == null) yield return null;

            if (GameSceneController.Instance.Enemy_Set.Count < maxEnemyCount)
            {
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

                if (selected != null && GameSceneController.Instance.enemy_ref_map.Enemy_Set.ContainsKey(selected.EType))
                {
                    GameObject spawnTarget = null;
                    if (selected.From == SpawnFromType.Grave)
                    {
                        spawnTarget = FindClosedGrave(GameSceneController.Instance.player_controller.transform.position);
                        if (spawnTarget != null)
                        {
                            SpwanZombiesFromGrave(selected.EType, spawnTarget);
                        }
                    }
                    // Add Nest spawning here if desired in future
                }
            }
            yield return new WaitForSeconds(spawnRate);
        }
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
                maxEnemyCount = 12 + (8 * levelIndex);
                spawnRate = Mathf.Max(0.05f, 0.75f * Mathf.Pow(0.80f, levelIndex));
            }
        }
    }

    public void PauseEndlessGame()
    {
        if (is_mission_paused) return;

        is_mission_paused = true;
        is_logic_paused = true;
        Time.timeScale = 0f;

        // Stop enemy spawn and scaling coroutines while paused
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);

        // Optionally disable enemy AI updates or other logic here

        // Show pause UI
        if (GameSceneController.Instance != null && GameSceneController.Instance.game_pause_panel != null)
        {
            GameSceneController.Instance.game_pause_panel.Show();
        }

        // Unlock cursor so player can interact with UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("[Endless] Game paused.");
    }

    public void ResumeEndlessGame()
    {
        if (!is_mission_paused) return;

        is_mission_paused = false;
        is_logic_paused = false;
        Time.timeScale = 1f;

        // Restart coroutines for spawning and scaling
        spawnCoroutine = StartCoroutine(StartSpawningEnemies());
        scaleCoroutine = StartCoroutine(ScaleSpawnCap());

        // Hide pause UI
        if (GameSceneController.Instance != null && GameSceneController.Instance.game_pause_panel != null)
        {
            GameSceneController.Instance.game_pause_panel.Hide();
        }

        // Lock cursor for gameplay
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

        Time.timeScale = 1f; // Reset time scale to normal
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

    protected virtual void SetLoseState()
    {
        GamePlayingState = PlayingState.Lose;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    private List<EnemyWaveInfo> GetEnemyWaveInfoForCurrentDay()
    {
        return GetEnemyWaveInfoForCurrentDay(GameData.Instance.day_level);
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
        if (selectedKey != -1)
            return GameConfig.Instance.EnemyWaveInfo_Endless_Set[selectedKey].wave_info_list;

        return new List<EnemyWaveInfo>();
    }
}
