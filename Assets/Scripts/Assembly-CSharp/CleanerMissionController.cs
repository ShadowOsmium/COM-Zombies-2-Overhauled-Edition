using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class CleanerMissionController : MissionController
{
	public float mission_life = 5f;

	private float mission_life_total = 5f;

	private float last_mission_life = 5f;

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
		return list2;
	}

	public override IEnumerator Start()
	{
		InitMissionController();
		mission_type = MissionType.Cleaner;
		CaculateDifficulty();
		yield return 1;
		PlayerController player = GameSceneController.Instance.player_controller;
		while (player == null)
		{
			yield return 1;
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
		while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
		{
			yield return 1;
		}
		yield return new WaitForSeconds(4f);
		while (mission_life > 0f)
		{
			int index = Random.Range(0, EnemyWaveInfo_Set.Count);
			EnemyWaveInfo wave = EnemyWaveInfo_Set[index];
			foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
			{
				if (mission_life <= 0f)
				{
					mission_life = 0f;
					break;
				}
				EnemyType EType = spawn_info.EType;
				int Count = spawn_info.Count;
				SpawnFromType From = spawn_info.From;
				for (int i = 0; i < Count; i++)
				{
					while (GameSceneController.Instance.Enemy_Set.Count >= 8)
					{
						yield return new WaitForSeconds(1f);
					}
					if (mission_life <= 0f)
					{
						mission_life = 0f;
						break;
					}
					switch (From)
					{
					case SpawnFromType.Grave:
					{
						GameObject grave = FindClosedGrave(player.transform.position);
						SpwanZombiesFromGrave(EType, grave, false);
						yield return new WaitForSeconds(0.3f);
						break;
					}
					case SpawnFromType.Nest:
						SpwanZombiesFromNest(EType, zombie_nest_array[Random.Range(0, zombie_nest_array.Length)]);
						yield return new WaitForSeconds(0.3f);
						break;
					}
				}
				yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Normal.line_interval);
			}
			yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Normal.wave_interval);
		}
		MissionFinished();
    }

	public override void Update()
	{
		base.Update();
		if (!is_mission_finished && last_mission_life != mission_life)
		{
			last_mission_life = mission_life;
			if (last_mission_life < 0f)
			{
				last_mission_life = 0f;
			}
			GameSceneController.Instance.game_main_panel.clean_panel.SetMissionBar(1f - last_mission_life / mission_life_total);
		}
	}

	public override void CaculateDifficulty()
	{
		mission_life_total = (mission_life = GameSceneController.Instance.enemy_standard_reward_total);
	}
}
