using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class NPCResourcesMissionController : MissionController
{
	public int cur_res_count;

	public int target_res_count = 1;

	private float last_check_info_time;

	private float check_info_rate = 0.5f;

	public WorkerController npc_worker;

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
		mission_type = MissionType.Npc_Resources;
		CaculateDifficulty();
		UpdatePanelInfo();
		yield return 1;
		Transform trans_resource = GameObject.FindGameObjectWithTag("Npc_Res_Resource").transform;
		Transform trans_home = GameObject.FindGameObjectWithTag("Npc_Res_Home").transform;
		GameObject npc_res_resource = Object.Instantiate(Resources.Load("Prefabs/NPC/Res_Resource"), trans_resource.position, trans_resource.rotation) as GameObject;
		GameObject npc_res_home = Object.Instantiate(Resources.Load("Prefabs/NPC/Res_Home"), trans_home.position, trans_home.rotation) as GameObject;
		Transform trans_npc = GameObject.FindGameObjectWithTag("Npc_Res_Spawn").transform;
		npc_worker = NPCFactory.CreateNPC(NpcType.N_WORKER, trans_npc.position, trans_npc.rotation) as WorkerController;
		npc_worker.InitRescourceTrans(npc_res_resource.transform, npc_res_home.transform);
		npc_worker.mission_controller = this;
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
		GameSceneController.Instance.player_controller.UpdateWeaponUIShow();
		while (!is_spawn_enemy)
		{
			yield return 1;
		}
		List<EnemyWaveInfo> EnemyWaveInfo_Set = null;
		foreach (int level in GameConfig.Instance.EnemyWaveInfo_Npc_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= level)
			{
				EnemyWaveInfo_Set = GameConfig.Instance.EnemyWaveInfo_Npc_Set[level].wave_info_list;
				break;
			}
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
				yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Npc.line_interval);
			}
			yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Npc.wave_interval);
		}
		Debug.Log("Mission Life Over~");
	}

	public override void Update()
	{
		base.Update();
		if (!is_mission_finished && !(npc_worker == null) && !is_mission_paused && Time.time - last_check_info_time >= check_info_rate)
		{
			last_check_info_time = Time.time;
			if (npc_worker.Npc_State == npc_worker.DEAD_STATE)
			{
				GameSceneController.Instance.OnKeyManDead(npc_worker);
			}
		}
	}

	public void UpdatePanelInfo()
	{
		GameSceneController.Instance.game_main_panel.npc_res_panel.SetContent(cur_res_count + " / " + target_res_count);
	}

	public bool AddResources(int count)
	{
		Debug.Log("Add resources:" + count);
		cur_res_count += count;
		UpdatePanelInfo();
		if (cur_res_count >= target_res_count)
		{
			MissionFinished();
			return true;
		}
		return false;
	}

	public override void CaculateDifficulty()
	{
		cur_res_count = 0;
		target_res_count = 2;
	}

	public void StartSpawnEnemy()
	{
		if (!is_spawn_enemy)
		{
			is_spawn_enemy = true;
			Debug.Log("On start npc res! Monster come out!");
		}
	}
}
