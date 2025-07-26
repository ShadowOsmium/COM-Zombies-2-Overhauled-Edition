using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;
using System.Linq;

public class MissionController : MonoBehaviour
{
	public MissionType mission_type;

	public GameObject[] zombie_nest_array;

	public GameObject[] zombie_grave_array;

	public GameObject[] zombie_boss_array;

	protected bool is_mission_finished;

	protected bool is_mission_paused;

	public float mission_total_time;

	public virtual IEnumerator Start()
	{
		yield return 1;
	}

	public virtual void Update()
	{
		mission_total_time += Time.deltaTime;
	}

	public virtual void CaculateDifficulty()
	{
	}

	public virtual void InitMissionController()
	{
		zombie_nest_array = GameObject.FindGameObjectsWithTag("Zombie_Nest");
		zombie_grave_array = GameObject.FindGameObjectsWithTag("Zombie_Grave");
		zombie_boss_array = GameObject.FindGameObjectsWithTag("Zombie_Boss");
	}

	public GameObject FindClosedGrave(Vector3 pos)
	{
		GameObject result = null;
		if (zombie_grave_array != null)
		{
			float num = 9999f;
			float num2 = 9999f;
			GameObject[] array = zombie_grave_array;
			foreach (GameObject gameObject in array)
			{
				num2 = (gameObject.transform.position - pos).sqrMagnitude;
				if (num2 < num)
				{
					result = gameObject;
					num = num2;
				}
			}
		}
		return result;
	}

    public virtual EnemyController SpwanZombiesFromGrave(EnemyType type, GameObject grave, bool bypassSpawnLimit)
    {
        if (grave == null)
        {
            Debug.LogError("Spawn zombie from grave, grave is null.");
            return null;
        }

        float x = Random.Range(-grave.transform.localScale.x / 2f, grave.transform.localScale.x / 2f);
        float z = Random.Range(-grave.transform.localScale.z / 2f, grave.transform.localScale.z / 2f);
        Vector3 spawnPos = grave.transform.position + new Vector3(x, 0f, z);

        EnemyController spawned = null;

        if (type == EnemyType.E_CROW && Application.loadedLevelName != "test_new1")
        {
            Vector3 crowSpawnPos = GameSceneController.Instance.way_points[0].transform.position;
            spawned = EnemyFactory.CreateEnemyGetEnemyController(type, crowSpawnPos, Quaternion.identity);
        }
        else
        {
            spawned = EnemyFactory.CreateEnemyGetEnemyController(type, spawnPos, Quaternion.identity);

            if (type != EnemyType.E_FATCOOK && type != EnemyType.E_FATCOOK_E &&
                type != EnemyType.E_HAOKE_A && type != EnemyType.E_HAOKE_B &&
                GameData.Instance.cur_quest_info.mission_type != MissionType.Tutorial)
            {
                GameSceneController.Instance.ground_stone_pool.GetComponent<ObjectPool>().CreateObject(spawnPos, Quaternion.identity);
            }
        }

        if (spawned != null)
        {
            spawned.ignoreSpawnLimit = bypassSpawnLimit;
            spawned.MissionWeight = GameConfig.Instance.EnemyConfig_Set[type].missionWeight;
        }

        return spawned;
    }

    public virtual float SpwanZombiesFromNest(EnemyType type, GameObject nest)
    {
        if (nest == null)
        {
            Debug.LogError("Spawn zombie from nest, nest is null.");
            return 0f;
        }

        if (type == EnemyType.E_CROW && Application.loadedLevelName != "test_new1")
        {
            Vector3 crowSpawnPos = GameSceneController.Instance.way_points[0].transform.position;
            EnemyFactory.CreateEnemyGetEnemyController(type, crowSpawnPos, Quaternion.identity);
        }
        else
        {
            EnemyFactory.CreateEnemyGetEnemyController(type, nest.transform.position, nest.transform.rotation);
        }

        return GameConfig.Instance.EnemyConfig_Set[type].missionWeight;
    }

    public virtual IEnumerator SummonBossMinions(EnemyController boss)
    {
        if (boss == null)
        {
            Debug.LogWarning("[MissionController] Boss is null.");
            yield break;
        }

        EnemyType bossType = boss.enemy_data.enemy_type;
        Debug.Log("[MissionController] Boss type for summon: " + bossType);

        EnemyType minionType = EnemyType.E_ZOMBIE;
        int minionCount = 0;

        switch (bossType)
        {
            case EnemyType.E_FATCOOK:
                minionType = EnemyType.E_BOOMER_TIMER;
                minionCount = 8;
                break;
            case EnemyType.E_FATCOOK_E:
                minionType = EnemyType.E_BOOMER_TIMER_E;
                minionCount = 12;
                break;
            case EnemyType.E_HALLOWEEN:
                minionType = EnemyType.E_HALLOWEEN_SUB;
                minionCount = 2;
                break;
            case EnemyType.E_HALLOWEEN_E:
                minionType = EnemyType.E_HALLOWEEN_SUB_E;
                minionCount = 3;
                break;
            default:
                Debug.LogWarning("[MissionController] Unknown boss type: " + bossType);
                yield break;
        }

        GameObject grave = FindClosedGrave(boss.transform.position);
        Debug.Log("[MissionController] Spawning " + minionCount + " minions of type " + minionType);

        for (int i = 0; i < minionCount; i++)
        {
            EnemyController minion = SpwanZombiesFromGrave(minionType, grave, true);
            if (minion != null)
            {
                minion.gameObject.SetActive(true);
                minion.ignoreSpawnLimit = true;
                if (!GameSceneController.Instance.Enemy_Set.ContainsKey(minion.EnemyID))
                    GameSceneController.Instance.Enemy_Set.Add(minion.EnemyID, minion);
            }
            else
            {
                Debug.LogWarning("[SummonBossMinions] Failed to spawn minion #" + i);
            }
            yield return null;
        }
    }


    public virtual void MissionFinished()
	{
        GameSceneController.Instance.pause_button_obj.SetActive(false);
        is_mission_finished = true;
		GameSceneController.Instance.MissionControllerFinished();
	}

	public virtual void SetMissionPaused(bool state)
	{
		is_mission_paused = state;
	}

	public virtual List<EnemyType> GetMissionEnemyTypeList()
	{
		return null;
	}
}
