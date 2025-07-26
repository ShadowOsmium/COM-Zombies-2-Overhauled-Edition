using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoMZ2;

public class DailyMissionController : MissionController
{
    public float mission_life = 5f;
    private float mission_life_total = 5f;
    private float last_mission_life = 5f;
    private int totalEnemies = 0;    // NEW dedicated enemy count
    private int enemiesKilled = 0;

    private EnemyWaveInfoList currentWaveInfoList;
    private EnemyWaveIntervalInfo currentWaveInterval;

    private new GameObject[] zombie_nest_array;

    private PlayerController player;

    private EnemyWaveInfoList GetWaveInfoList()
    {
        int dayLevel = GameSceneController.Instance.DayLevel;
        Dictionary<int, EnemyWaveInfoList> waveDict;

        if (GameData.Instance.is_crazy_daily)
            waveDict = GameConfig.Instance.EnemyWaveInfo_CrazyDaily_Set;
        else
            waveDict = GameConfig.Instance.EnemyWaveInfo_Daily_Set;

        if (waveDict == null || waveDict.Count == 0)
        {
            Debug.LogError("Enemy wave dictionary is empty!");
            return null;
        }

        if (waveDict.ContainsKey(dayLevel))
        {
            Debug.Log("[DailyMissionController] Exact wave config found for day level: " + dayLevel);
            return waveDict[dayLevel];
        }

        int closestKey = -1;
        foreach (int key in waveDict.Keys)
        {
            if (key <= dayLevel)
            {
                if (closestKey == -1 || key > closestKey)
                    closestKey = key;
            }
        }

        if (closestKey != -1)
        {
            Debug.LogWarning("Exact day level config missing for day level: " + dayLevel +
                             ". Using closest lower day level config: " + closestKey);
            return waveDict[closestKey];
        }

        Debug.LogError((GameData.Instance.is_crazy_daily ? "Crazy daily" : "Normal daily") +
                       " config missing for day level: " + dayLevel + " and no lower fallback found.");
        return null;
    }

    private EnemyWaveIntervalInfo GetWaveInterval()
    {
        return GameData.Instance.is_crazy_daily
            ? GameConfig.Instance.EnemyWave_Interval_CrazyDaily
            : GameConfig.Instance.EnemyWave_Interval_Normal;
    }

    public override List<EnemyType> GetMissionEnemyTypeList()
    {
        List<EnemyType> enemyTypes = new List<EnemyType>();
        if (currentWaveInfoList == null) return enemyTypes;

        foreach (EnemyWaveInfo wave in currentWaveInfoList.wave_info_list)
        {
            foreach (EnemySpawnInfo spawn in wave.spawn_info_list)
            {
                if (!enemyTypes.Contains(spawn.EType))
                    enemyTypes.Add(spawn.EType);
            }
        }
        return enemyTypes;
    }

    public override IEnumerator Start()
    {
        InitMissionController();

        mission_type = MissionType.Cleaner;

        enemiesKilled = 0; // reset kill count here

        yield return null;

        player = null;
        while (player == null)
        {
            yield return null;
            player = GameSceneController.Instance.player_controller;
        }

        zombie_nest_array = GameObject.FindGameObjectsWithTag("Zombie_Nest");

        currentWaveInterval = GetWaveInterval();
        currentWaveInfoList = GetWaveInfoList();

        if (currentWaveInfoList == null)
        {
            Debug.LogError("[DailyMissionController] No enemy waves found for day level " + GameSceneController.Instance.DayLevel);
            yield break;
        }

        CaculateDifficulty();

        Debug.Log("[DailyMissionController] Waves loaded: " + currentWaveInfoList.wave_info_list.Count);

        foreach (var wave in currentWaveInfoList.wave_info_list)
        {
            if (wave.spawn_info_list == null || wave.spawn_info_list.Count == 0)
            {
                Debug.LogWarning("[DailyMissionController] Wave spawn_info_list is empty!");
            }
            else
            {
                foreach (var spawn in wave.spawn_info_list)
                {
                    Debug.Log("- " + spawn.EType + " x" + spawn.Count + " from " + spawn.From);
                }
            }
        }

        if (GameSceneController.Instance.enemy_ref_map != null)
        {
            GameSceneController.Instance.enemy_ref_map.ResetEnemyMapInfo(GetMissionEnemyTypeList());
        }

        while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        bool isCrazy = GameData.Instance.is_crazy_daily;
        int waveLoopCount = isCrazy ? 2 : 1;
        int completedLoops = 0;

        List<EnemyWaveInfo> waves = currentWaveInfoList.wave_info_list;

        while (completedLoops < waveLoopCount)
        {
            Debug.Log("[Wave Loop] Starting loop #" + (completedLoops + 1));

            for (int waveIndex = 0; waveIndex < waves.Count; waveIndex++)
            {
                EnemyWaveInfo wave = waves[waveIndex];
                if (wave.spawn_info_list == null) continue;

                foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
                {
                    EnemyType type = spawn_info.EType;
                    int count = spawn_info.Count;
                    SpawnFromType from = spawn_info.From;

                    for (int i = 0; i < count; i++)
                    {
                        // Instead of breaking early on mission_life, just yield normally.
                        while (GameSceneController.Instance.Enemy_Set.Count >= 12)
                        {
                            Debug.Log("[Spawn Debug] Enemy count is " + GameSceneController.Instance.Enemy_Set.Count + ", waiting...");
                            yield return new WaitForSeconds(2f);
                        }

                        Debug.Log("[Spawn Debug] Spawning enemy #" + i + " of type " + type);

                        switch (from)
                        {
                            case SpawnFromType.Grave:
                                GameObject grave = FindClosedGrave(player.transform.position);
                                if (grave != null)
                                    SpwanZombiesFromGrave(type, grave, false);
                                else
                                    Debug.LogWarning("[Spawn Debug] No grave found to spawn from!");
                                break;

                            case SpawnFromType.Nest:
                                if (zombie_nest_array != null && zombie_nest_array.Length > 0)
                                {
                                    GameObject nest = zombie_nest_array[Random.Range(0, zombie_nest_array.Length)];
                                    SpwanZombiesFromNest(type, nest);
                                }
                                else
                                {
                                    Debug.LogWarning("[Spawn Debug] No nests available to spawn from!");
                                }
                                break;

                            default:
                                Debug.LogWarning("[Spawn Debug] Unknown spawn from type: " + from);
                                break;
                        }

                        yield return new WaitForSeconds(1f);
                    }

                    yield return new WaitForSeconds(currentWaveInterval.line_interval);
                }

                yield return new WaitForSeconds(currentWaveInterval.wave_interval);
            }

            completedLoops++;
        }

        MissionFinished();
        GameData.Instance.SaveData();
        yield break;
    }

    public override void Update()
    {
        base.Update();
        if (GameSceneController.Instance.can_buy_ammo && Input.GetKeyDown(KeyCode.R))
        {
            GameSceneController.Instance.OnAddBulletButton();
        }
    }

    public void OnEnemyKilled()
    {
        if (is_mission_finished)
            return;

        enemiesKilled++;

        float progress = Mathf.Clamp01((float)enemiesKilled / totalEnemies);

        if (GameSceneController.Instance != null && GameSceneController.Instance.game_main_panel != null)
        {
            GameSceneController.Instance.game_main_panel.clean_panel.SetMissionBar(progress);
        }

        if (enemiesKilled >= totalEnemies)
        {
            MissionFinished();
        }
    }

    public override void CaculateDifficulty()
    {
        totalEnemies = 0;

        List<EnemyWaveInfo> waves = (currentWaveInfoList != null) ? currentWaveInfoList.wave_info_list : null;

        if (waves != null)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                EnemyWaveInfo wave = waves[i];
                if (wave == null || wave.spawn_info_list == null)
                    continue;

                for (int j = 0; j < wave.spawn_info_list.Count; j++)
                {
                    EnemySpawnInfo spawn = wave.spawn_info_list[j];
                    totalEnemies += spawn.Count;
                }
            }
        }

        int waveLoopCount = GameData.Instance.is_crazy_daily ? 2 : 1;
        totalEnemies *= waveLoopCount;

        mission_life_total = totalEnemies;
        mission_life = mission_life_total;
        last_mission_life = mission_life_total;

        Debug.Log("CaculateDifficulty: totalEnemies = " + totalEnemies);
    }
}
