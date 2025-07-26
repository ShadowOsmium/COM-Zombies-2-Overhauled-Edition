using System;
using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class TimeAliveMissionController : MissionController
{
    public float mission_life_time = 50f;
    protected bool mission_started;
    private float last_update_info_time;
    private float update_info_rate = 0.3f;

    public int target_kill_count = 25;
    private int current_kill_count = 0;
    private float spawn_delay = 1.0f;

    public override List<EnemyType> GetMissionEnemyTypeList()
    {
        List<EnemyWaveInfo> list = null;
        foreach (int key in GameConfig.Instance.EnemyWaveInfo_Normal_Set.Keys)
        {
            if (GameSceneController.Instance.DayLevel <= key)
            {
                list = GameConfig.Instance.EnemyWaveInfo_Normal_Set[key].wave_info_list;
                break;
            }
        }

        List<EnemyType> list2 = new List<EnemyType>();
        if (list != null)
        {
            foreach (EnemyWaveInfo item in list)
            {
                foreach (EnemySpawnInfo item2 in item.spawn_info_list)
                {
                    if (!list2.Contains(item2.EType))
                    {
                        list2.Add(item2.EType);
                    }
                }
            }
        }

        return list2;
    }

    public override IEnumerator Start()
    {
        InitMissionController();
        mission_type = MissionType.Time_ALive;
        CaculateDifficulty();

        yield return null;

        PlayerController player = GameSceneController.Instance.player_controller;
        while (player == null)
        {
            yield return null;
            player = GameSceneController.Instance.player_controller;
        }

        List<EnemyWaveInfo> EnemyWaveInfo_Set = null;
        foreach (int level in GameConfig.Instance.EnemyWaveInfo_Normal_Set.Keys)
        {
            if (GameSceneController.Instance.DayLevel <= level)
            {
                EnemyWaveInfo_Set = GameConfig.Instance.EnemyWaveInfo_Normal_Set[level].wave_info_list;
                break;
            }
        }

        if (EnemyWaveInfo_Set == null || EnemyWaveInfo_Set.Count == 0)
        {
            Debug.LogError("No enemy waves defined for current day level!");
            yield break;
        }

        while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
        {
            yield return null;
        }

        GameSceneController.Instance.Invoke("ShowMissionLabel", 0.5f);

        yield return new WaitForSeconds(2.5f);

        TimeSpan span = TimeSpan.FromSeconds(mission_life_time);
        string timeStr = string.Format("{0:D2}:{1:D2}", span.Minutes, span.Seconds);

        GameSceneController.Instance.game_main_panel.ShowMissionDayLabel(
            "Survive for " + timeStr + " and kill " + target_kill_count + " enemies!");

        yield return new WaitForSeconds(2f);

        mission_started = true;

        while (mission_life_time > 0f)
        {
            int index = UnityEngine.Random.Range(0, EnemyWaveInfo_Set.Count);
            EnemyWaveInfo wave = EnemyWaveInfo_Set[index];

            foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
            {
                if (mission_life_time <= 0f)
                    break;

                EnemyType EType = spawn_info.EType;
                int Count = spawn_info.Count;
                SpawnFromType From = spawn_info.From;

                for (int i = 0; i < Count; i++)
                {
                    while (GameSceneController.Instance.Enemy_Set.Count >= 16)
                    {
                        yield return new WaitForSeconds(0.3f);
                    }

                    if (mission_life_time <= 0f)
                        break;

                    switch (From)
                    {
                        case SpawnFromType.Grave:
                            GameObject grave = FindClosedGrave(player.transform.position);
                            SpwanZombiesFromGrave(EType, grave, false);
                            break;

                        case SpawnFromType.Nest:
                            SpwanZombiesFromNest(EType, zombie_nest_array[UnityEngine.Random.Range(0, zombie_nest_array.Length)]);
                            break;
                    }

                    yield return new WaitForSeconds(spawn_delay);
                }

                yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Normal.line_interval);
            }

            yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Normal.wave_interval);
        }

        Debug.Log("Mission Life Over~");
    }

    private IEnumerator ShowObjectiveMessageLater(string msg, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameSceneController.Instance.game_main_panel.ShowMissionDayLabel(msg);
    }

    public override void Update()
    {
        base.Update();

        if (GameSceneController.Instance.GamePlayingState == PlayingState.CG || !mission_started || is_mission_finished || is_mission_paused)
        {
            return;
        }

        if (mission_life_time > 0f)
        {
            mission_life_time -= Time.deltaTime;

            if (Time.time - last_update_info_time >= update_info_rate)
            {
                last_update_info_time = Time.time;

                TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.FloorToInt(mission_life_time));
                GameSceneController.Instance.game_main_panel.time_alive_panel.SetContent(
                    string.Format("{0:D2}:{1:D2}   Kills: {2}/{3}",
                        timeSpan.Minutes, timeSpan.Seconds, current_kill_count, target_kill_count));
            }
        }
        else
        {
            if (current_kill_count >= target_kill_count)
            {
                Debug.Log("Mission succeeded.");
                MissionFinished();
            }
            else
            {
                Debug.Log("Mission failed. Killed " + current_kill_count + " of " + target_kill_count);
                MissionFailed();
            }
        }
    }

    public void RegisterEnemyKill()
    {
        current_kill_count++;
    }

    public override void CaculateDifficulty()
    {
        int day = GameSceneController.Instance.DayLevel;

        float baseTime = 60f;
        float incrementPer15Days = 10f;
        int increments = day / 15;

        mission_life_time = baseTime + increments * incrementPer15Days;

        target_kill_count = 20 + (day / 2);

        // Dynamically scale spawn delay (minimum 0.3s)
        spawn_delay = Mathf.Max(0.3f, 1.0f - (day / 50f));
    }

    public virtual void MissionFailed()
    {
        is_mission_finished = true;
        GameSceneController.Instance.GamePlayingState = PlayingState.Lose;
        GameSceneController.Instance.MissionFinished();
    }
}