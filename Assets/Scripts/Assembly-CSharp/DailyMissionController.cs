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

    private const int maxEnemyCount = 12;

    private PlayerController player;

    private EnemyWaveInfoList GetWaveInfoList()
    {
        if (GameData.Instance.is_crazy_daily)
            return GameConfig.Instance.EnemyWaveInfo_CrazyDaily_Set.ContainsKey(GameSceneController.Instance.DayLevel)
                ? GameConfig.Instance.EnemyWaveInfo_CrazyDaily_Set[GameSceneController.Instance.DayLevel]
                : null;

        return GameConfig.Instance.EnemyWaveInfo_Normal_Set.ContainsKey(GameSceneController.Instance.DayLevel)
            ? GameConfig.Instance.EnemyWaveInfo_Normal_Set[GameSceneController.Instance.DayLevel]
            : null;
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

        currentWaveInfoList = GetWaveInfoList();
        currentWaveInterval = GetWaveInterval();

        if (currentWaveInfoList == null)
        {
            Debug.LogError("[DailyMissionController] No enemy waves found for day level " + GameSceneController.Instance.DayLevel);
            yield break;
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

        List<EnemyWaveInfo> waves = currentWaveInfoList.wave_info_list;

        while (mission_life > 0f)
        {
            int waveIndex = Random.Range(0, waves.Count);
            EnemyWaveInfo wave = waves[waveIndex];

            foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
            {
                if (mission_life <= 0f) break;

                EnemyType type = spawn_info.EType;
                int count = spawn_info.Count;
                SpawnFromType from = spawn_info.From;

                for (int i = 0; i < count; i++)
                {
                    while (GameSceneController.Instance.Enemy_Set.Count >= maxEnemyCount)
                    {
                        yield return new WaitForSeconds(1f);
                    }

                    if (mission_life <= 0f) break;

                    switch (from)
                    {
                        case SpawnFromType.Grave:
                            GameObject grave = FindClosedGrave(player.transform.position);
                            if (grave != null)
                            {
                                SpwanZombiesFromGrave(type, grave);
                            }
                            break;

                        case SpawnFromType.Nest:
                            if (zombie_nest_array != null && zombie_nest_array.Length > 0)
                            {
                                GameObject nest = zombie_nest_array[Random.Range(0, zombie_nest_array.Length)];
                                SpwanZombiesFromNest(type, nest);
                            }
                            break;
                    }

                    yield return new WaitForSeconds(0.3f);
                }

                yield return new WaitForSeconds(currentWaveInterval.line_interval);
            }

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
