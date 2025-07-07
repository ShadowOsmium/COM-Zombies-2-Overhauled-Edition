using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class SceneTrapController : MonoBehaviour
{
	public GameObject Enter_door;

	public GameObject Exit_door;

	private bool is_trigered;

	private bool is_traping;

	private float mission_life = 5f;

	private List<EnemyController> enemy_trap_set = new List<EnemyController>();

	private float last_check_time;

	private float check_rate = 0.5f;

	private IEnumerator Start()
	{
		if (GameData.Instance.cur_quest_info.mission_type == MissionType.Npc_Convoy || GameData.Instance.cur_quest_info.mission_type == MissionType.Npc_Resources)
		{
			is_trigered = true;
		}
		yield return 1;
		CaculateDifficulty();
		List<EnemyWaveInfo> EnemyWaveInfo_Set = null;
		foreach (int level in GameConfig.Instance.EnemyWaveInfo_Normal_Set.Keys)
		{
			if (GameSceneController.Instance.DayLevel <= level)
			{
				EnemyWaveInfo_Set = GameConfig.Instance.EnemyWaveInfo_Normal_Set[level].wave_info_list;
				break;
			}
		}
		yield return 1;
		while (!is_trigered)
		{
			yield return 1;
		}
		while (!is_traping)
		{
			yield return 1;
		}
		while (mission_life > 0f)
		{
			int index = Random.Range(0, EnemyWaveInfo_Set.Count);
			EnemyWaveInfo wave = EnemyWaveInfo_Set[index];
			yield return new WaitForSeconds(4f);
			foreach (EnemySpawnInfo spawn_info in wave.spawn_info_list)
			{
				EnemyType EType = spawn_info.EType;
				int Count = spawn_info.Count;
				for (int i = 0; i < Count; i++)
				{
					while (enemy_trap_set.Count >= 8)
					{
						yield return new WaitForSeconds(1f);
					}
					mission_life -= SpwanZombiesFromGrave(EType);
					yield return new WaitForSeconds(1f);
					if (mission_life <= 0f)
					{
						mission_life = 0f;
						break;
					}
				}
				if (mission_life <= 0f)
				{
					mission_life = 0f;
					break;
				}
			}
		}
	}

	private void Update()
	{
		if (is_trigered && is_traping && Time.time - last_check_time >= check_rate)
		{
			last_check_time = Time.time;
			if (CheckTrapEnemyCount() == 0 && mission_life <= 0f)
			{
				Debug.Log("Open Door!");
				is_traping = false;
				Enter_door.GetComponent<SceneDoorController>().RiseDoor();
				Exit_door.GetComponent<SceneDoorController>().RiseDoor();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!is_trigered && other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			is_trigered = true;
			is_traping = true;
			Debug.Log("Trap is trigered!:" + other.gameObject.name);
			Enter_door.GetComponent<SceneDoorController>().DropDoor();
			Exit_door.GetComponent<SceneDoorController>().DropDoor();
		}
	}

    public float SpwanZombiesFromGrave(EnemyType type)
    {
        float x = Random.Range(-transform.localScale.x / 2f, transform.localScale.x / 2f);
        float z = Random.Range(-transform.localScale.z / 2f, transform.localScale.z / 2f);
        Vector3 pos = transform.position + new Vector3(x, 0f, z);

        if (type != EnemyType.E_CROW)
        {
            EnemyController enemyController = EnemyFactory.CreateEnemyForTrap(type, pos);
            enemyController.trap_controller = this;
            enemy_trap_set.Add(enemyController);
            return GameConfig.Instance.EnemyConfig_Set[type].missionWeight;
        }

        return 0f;
    }

    public void CaculateDifficulty()
	{
		mission_life = GameData.Instance.day_level * 2 + 5;
	}

	public void RemoveEnemy(EnemyController enemy)
	{
		enemy_trap_set.Remove(enemy);
	}

	public int CheckTrapEnemyCount()
	{
		int num = 0;
		foreach (EnemyController item in enemy_trap_set)
		{
			if (item != null)
			{
				num++;
			}
		}
		return num;
	}
}
