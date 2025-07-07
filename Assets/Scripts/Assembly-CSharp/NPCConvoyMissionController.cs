using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class NPCConvoyMissionController : MissionController
{
	public FollowerController npc_follower;

	private float last_check_info_time;

	private float check_info_rate = 0.5f;

	private bool is_spawn_enemy;

	public override List<EnemyType> GetMissionEnemyTypeList()
	{
		List<EnemyWaveInfo> list = null;
		foreach (int key in GameConfig.Instance.EnemyWaveInfo_Npc_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= key)
			{
				list = GameConfig.Instance.EnemyWaveInfo_Npc_Set[key].wave_info_list;
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
		mission_type = MissionType.Npc_Convoy;
		yield return 1;
		Transform trans_spawn = GameObject.FindGameObjectWithTag("Npc_Convoy_Spawn").transform;
		Transform trans_home = GameObject.FindGameObjectWithTag("Npc_Convoy_Home").transform;
		GameObject npc_home = Object.Instantiate(Resources.Load("Prefabs/NPC/Res_Home"), trans_home.position, trans_home.rotation) as GameObject;
		GameObject npc_convoy_triger_obj = Object.Instantiate(Resources.Load("Prefabs/NPC/NpcConvoyTriger"), trans_spawn.position, trans_spawn.rotation) as GameObject;
		NpcConvoyTriger npc_convoy_triger = npc_convoy_triger_obj.GetComponent<NpcConvoyTriger>();
		npc_convoy_triger.target_trans = trans_home;
		if (GameData.Instance.cur_quest_info.avatar != AvatarType.None && GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Main)
		{
			npc_follower = NPCFactory.CreateNPC(NpcType.N_FOLLOWER, trans_spawn.position, trans_spawn.rotation, GameData.Instance.cur_quest_info.avatar) as FollowerController;
			string frame_name = GameData.Instance.AvatarData_Set[GameData.Instance.cur_quest_info.avatar].avatar_name + "_0" + (int)GameData.Instance.AvatarData_Set[GameData.Instance.cur_quest_info.avatar].avatar_state + "_icon_s";
			GameSceneController.Instance.game_main_panel.npc_convoy_panel.SetIcon(frame_name);
			GameSceneController.Instance.game_main_panel.SetNpcBarIcon(frame_name);
		}
		else
		{
			npc_follower = NPCFactory.CreateNPC(NpcType.N_FOLLOWER, trans_spawn.position, trans_spawn.rotation) as FollowerController;
			GameSceneController.Instance.game_main_panel.npc_convoy_panel.SetIcon("Npc_icon_s");
			GameSceneController.Instance.game_main_panel.SetNpcBarIcon("Npc_icon_s");
		}
		npc_convoy_triger.npc_follower = npc_follower;
		GameSceneController.Instance.game_main_panel.npc_convoy_panel.SetContent(0 + " / " + 1);
		List<EnemyWaveInfo> EnemyWaveInfo_Set = null;
		foreach (int level in GameConfig.Instance.EnemyWaveInfo_Npc_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= level)
			{
				EnemyWaveInfo_Set = GameConfig.Instance.EnemyWaveInfo_Npc_Set[level].wave_info_list;
				break;
			}
		}
		PlayerController player = GameSceneController.Instance.player_controller;
		while (player == null)
		{
			yield return 1;
			player = GameSceneController.Instance.player_controller;
		}
		while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
		{
			yield return 1;
		}
		while (!is_spawn_enemy)
		{
			yield return 1;
		}
		yield return new WaitForSeconds(1f);
		while (!is_mission_finished)
		{
			int index = Random.Range(0, EnemyWaveInfo_Set.Count);
			EnemyWaveInfo wave = EnemyWaveInfo_Set[index];
			foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
			{
				if (is_mission_finished)
				{
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
					if (is_mission_finished)
					{
						break;
					}
					switch (From)
					{
					case SpawnFromType.Grave:
					{
						GameObject grave = FindClosedGrave(player.transform.position);
						SpwanZombiesFromGrave(EType, grave);
						yield return new WaitForSeconds(0.3f);
						break;
					}
					case SpawnFromType.Nest:
						SpwanZombiesFromNest(EType, zombie_nest_array[Random.Range(0, zombie_nest_array.Length)]);
						yield return new WaitForSeconds(0.3f);
						break;
					}
				}
				yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Npc.wave_interval);
			}
			yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Npc.line_interval);
		}
		Debug.Log("Mission Life Over~");
	}

	public override void Update()
	{
		base.Update();
		if (!is_mission_finished && !(npc_follower == null) && !is_mission_paused && Time.time - last_check_info_time >= check_info_rate)
		{
			last_check_info_time = Time.time;
			if (npc_follower.Npc_State == npc_follower.DEAD_STATE)
			{
				GameSceneController.Instance.OnKeyManDead(npc_follower);
			}
			else if (npc_follower.arrived_home)
			{
				MissionFinished();
			}
		}
	}

	public void StartSpawnEnemy()
	{
		is_spawn_enemy = true;
	}
}
