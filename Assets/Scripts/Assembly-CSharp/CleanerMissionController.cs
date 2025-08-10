using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoMZ2;

public class CleanerMissionController : MissionController
{
    public float mission_life = 5f;

    private float mission_life_total = 5f;
    private float last_mission_life = 5f;

    private int enemiesKilled = 0;
    private int totalEnemies = 0;

    private EnemyWaveInfoList currentWaveInfoList;
    private EnemyWaveIntervalInfo currentWaveInterval;

    private GameObject[] zombie_nest_array;

    public override List<EnemyType> GetMissionEnemyTypeList()
    {
        List<EnemyWaveInfo> waveInfoList = null;
        foreach (int key in GameConfig.Instance.EnemyWaveInfo_Normal_Set.Keys)
        {
            if (GameSceneController.Instance.DayLevel <= key)
            {
                waveInfoList = GameConfig.Instance.EnemyWaveInfo_Normal_Set[key].wave_info_list;
                break;
            }
        }
        List<EnemyType> enemyTypes = new List<EnemyType>();
        if (waveInfoList == null)
            return enemyTypes;

        foreach (EnemyWaveInfo wave in waveInfoList)
        {
            foreach (EnemySpawnInfo spawnInfo in wave.spawn_info_list)
            {
                if (!enemyTypes.Contains(spawnInfo.EType))
                {
                    enemyTypes.Add(spawnInfo.EType);
                }
            }
        }
        return enemyTypes;
    }

    public override IEnumerator Start()
    {
        InitMissionController();
        mission_type = MissionType.Cleaner;

        enemiesKilled = 0;

        yield return null;

        PlayerController player = null;
        while (player == null)
        {
            yield return null;
            player = GameSceneController.Instance.player_controller;
        }

        zombie_nest_array = GameObject.FindGameObjectsWithTag("Zombie_Nest");

        currentWaveInterval = GameConfig.Instance.EnemyWave_Interval_Normal;
        currentWaveInfoList = null;
        foreach (int key in GameConfig.Instance.EnemyWaveInfo_Normal_Set.Keys)
        {
            if (GameSceneController.Instance.DayLevel <= key)
            {
                currentWaveInfoList = GameConfig.Instance.EnemyWaveInfo_Normal_Set[key];
                break;
            }
        }

        if (currentWaveInfoList == null)
        {
            Debug.LogError("[CleanerMissionController] No enemy waves found for day level " + GameSceneController.Instance.DayLevel);
            yield break;
        }

        totalEnemies = 0;
        float scaleFactor = Mathf.Clamp(0.2f + (GameSceneController.Instance.DayLevel / 300f), 0.3f, 0.4f);

        foreach (EnemyWaveInfo wave in currentWaveInfoList.wave_info_list)
        {
            foreach (EnemySpawnInfo spawn in wave.spawn_info_list)
            {
                int scaledCount = Mathf.CeilToInt(spawn.Count * scaleFactor);
                totalEnemies += scaledCount;
            }
        }

        Debug.Log("[CleanerMissionController] Total enemies to kill (scaled): " + totalEnemies);

        // Reset enemy map if exists
        if (GameSceneController.Instance.enemy_ref_map != null)
        {
            GameSceneController.Instance.enemy_ref_map.ResetEnemyMapInfo(GetMissionEnemyTypeList());
        }

        // Wait for CG state to finish
        while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);

        // Spawn enemies until done or mission finished
        foreach (EnemyWaveInfo wave in currentWaveInfoList.wave_info_list)
        {
            foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
            {
                EnemyType EType = spawn_info.EType;
                int Count = Mathf.CeilToInt(spawn_info.Count * scaleFactor);
                SpawnFromType From = spawn_info.From;

                for (int i = 0; i < Count; i++)
                {
                    // Increased active enemy cap
                    while (GameSceneController.Instance.Enemy_Set.Count >= 8)
                    {
                        yield return new WaitForSeconds(0.3f);
                    }

                    if (is_mission_finished)
                        break;

                    switch (From)
                    {
                        case SpawnFromType.Grave:
                            {
                                GameObject grave = FindClosedGrave(player.transform.position);
                                if (grave != null)
                                    SpwanZombiesFromGrave(EType, grave, false);
                                else
                                    Debug.LogWarning("[CleanerMissionController] No grave found to spawn from!");
                                yield return new WaitForSeconds(0.2f);
                                break;
                            }
                        case SpawnFromType.Nest:
                            if (zombie_nest_array != null && zombie_nest_array.Length > 0)
                                SpwanZombiesFromNest(EType, zombie_nest_array[Random.Range(0, zombie_nest_array.Length)]);
                            else
                                Debug.LogWarning("[CleanerMissionController] No nests available to spawn from!");
                            yield return new WaitForSeconds(0.2f);
                            break;
                    }
                }

                if (is_mission_finished)
                    break;

                yield return new WaitForSeconds(currentWaveInterval.line_interval * 0.5f);
            }

            if (is_mission_finished)
                break;

            yield return new WaitForSeconds(currentWaveInterval.wave_interval * 0.6f);
        }

        MissionFinished();
        yield break;
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

    public override void Update()
    {
        base.Update();
    }
}