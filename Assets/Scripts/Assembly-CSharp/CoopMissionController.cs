using System.Collections;
using System.Collections.Generic;
using CoMZ2;

public class CoopMissionController : MissionController
{
	private float last_check_time;

	private float check_rate = 1f;

	protected EnemyController boss;

	public override List<EnemyType> GetMissionEnemyTypeList()
	{
		List<EnemyWaveInfo> list = null;
		foreach (int key in GameConfig.Instance.EnemyWaveInfo_Boss_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= key)
			{
				list = GameConfig.Instance.EnemyWaveInfo_Boss_Set[key].wave_info_list;
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
		list2.Add(GameData.Instance.cur_quest_info.boss_type);
		return list2;
	}

	public override IEnumerator Start()
	{
		InitMissionController();
		mission_type = MissionType.Coop;
		CaculateDifficulty();
		yield return 1;
		PlayerController player = GameSceneController.Instance.player_controller;
	}

	public override void Update()
	{
	}

	public override void CaculateDifficulty()
	{
	}
}
