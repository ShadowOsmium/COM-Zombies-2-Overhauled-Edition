using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoMZ2;

public class DailyMissionController : MissionController
{
    public float mission_life = 5f;
    private float mission_life_total = 5f;
    private float last_mission_life = 5f;

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
        CaculateDifficulty();

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

        Debug.Log("[DailyMissionController] Waves loaded: " + currentWaveInfoList.wave_info_list.Count);

        // DEBUG: Total spawn entries count
        int totalSpawnEntries = 0;
        foreach (var wave in currentWaveInfoList.wave_info_list)
        {
            if (wave.spawn_info_list != null)
                totalSpawnEntries += wave.spawn_info_list.Count;
        }
        Debug.Log("[DailyMissionController] Total spawn entries across all waves: " + totalSpawnEntries);

        for (int i = 0; i < currentWaveInfoList.wave_info_list.Count; i++)
        {
            var wave = currentWaveInfoList.wave_info_list[i];
            if (wave.spawn_info_list == null || wave.spawn_info_list.Count == 0)
            {
                Debug.LogWarning("[DailyMissionController] Wave " + i + " spawn_info_list is empty!");
            }
            else
            {
                Debug.Log(string.Format("Wave {0} enemy types:", i));
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

        yield return new WaitForSeconds(4f);

        while (mission_life > 0f)
        {
            List<EnemyWaveInfo> waves = currentWaveInfoList.wave_info_list;

            // Instead of picking one random wave, spawn ALL waves every loop
            for (int waveIndex = 0; waveIndex < waves.Count; waveIndex++)
            {
                EnemyWaveInfo wave = waves[waveIndex];

                Debug.Log(string.Format("[Spawn Debug] Spawning wave index: {0}, enemy count in wave: {1}", waveIndex, wave.spawn_info_list.Count));

                foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
                {
                    Debug.Log(string.Format("[Spawn Debug] Enemy: {0}, Count: {1}, SpawnFrom: {2}", spawn_info.EType, spawn_info.Count, spawn_info.From));

                    if (mission_life <= 0f) break;

                    EnemyType type = spawn_info.EType;
                    int count = spawn_info.Count;
                    SpawnFromType from = spawn_info.From;

                    for (int i = 0; i < count; i++)
                    {
                        Debug.Log("[Spawn Debug] Waiting for enemy count to be below 8...");
                        while (GameSceneController.Instance.Enemy_Set.Count >= 8)
                        {
                            Debug.Log("[Spawn Debug] Enemy count is " + GameSceneController.Instance.Enemy_Set.Count + ", waiting...");
                            yield return new WaitForSeconds(2f);
                        }

                        if (mission_life <= 0f) break;

                        Debug.Log("[Spawn Debug] Spawning enemy #" + i + " of type " + type);

                        switch (from)
                        {
                            case SpawnFromType.Grave:
                                GameObject grave = FindClosedGrave(player.transform.position);
                                Debug.Log("[Spawn Debug] Grave found: " + (grave != null));
                                if (grave != null)
                                {
                                    SpwanZombiesFromGrave(type, grave);
                                }
                                else
                                {
                                    Debug.LogWarning("[Spawn Debug] No grave found to spawn from!");
                                }
                                break;

                            case SpawnFromType.Nest:
                                if (zombie_nest_array != null && zombie_nest_array.Length > 0)
                                {
                                    Debug.Log("[Spawn Debug] Nest count: " + zombie_nest_array.Length);
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

                if (mission_life <= 0f) break;
            }

            Debug.Log("[Mission Life] Current mission life: " + mission_life);

            yield return new WaitForSeconds(currentWaveInterval.wave_interval);
        }
        MissionFinished();
    }

    public override void Update()
    {
        base.Update();

        if (!is_mission_finished && last_mission_life != mission_life)
        {
            last_mission_life = Mathf.Max(0f, mission_life);
            if (GameSceneController.Instance != null && GameSceneController.Instance.game_main_panel != null)
            {
                GameSceneController.Instance.game_main_panel.clean_panel.SetMissionBar(1f - last_mission_life / mission_life_total);
            }
        }
    }

    public override void CaculateDifficulty()
    {
        mission_life_total = (mission_life = GameSceneController.Instance.enemy_standard_reward_total);
    }
}
