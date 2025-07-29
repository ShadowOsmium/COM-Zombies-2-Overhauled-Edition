using System.Collections;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{

    public static void WarmUpAllEnemyTypes()
    {
        if (GameSceneController.Instance == null || GameSceneController.Instance.enemy_ref_map == null)
        {
            //Debug.LogWarning("[EnemyFactory] Skipping warm-up — enemy_ref_map not initialized.");
            return;
        }

        System.Collections.Generic.Dictionary<EnemyType, GameObject> map = GameSceneController.Instance.enemy_ref_map.Enemy_Set;

        foreach (System.Collections.Generic.KeyValuePair<EnemyType, GameObject> kvp in map)
        {
            EnemyType type = kvp.Key;
            GameObject prefab = kvp.Value;

            if (prefab == null)
                continue;

            try
            {
                GameObject dummy = GameObject.Instantiate(prefab);
                dummy.name = "[WarmUpDummy] " + prefab.name;
                dummy.SetActive(false);

                UnityEngine.AI.NavMeshAgent agent = dummy.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null) agent.enabled = false;

                Animator animator = dummy.GetComponentInChildren<Animator>();
                if (animator != null) animator.Rebind();

                Collider col = dummy.GetComponent<Collider>();
                if (col != null) col.enabled = false;

                GameObject.Destroy(dummy);
                //Debug.Log("[EnemyFactory] Warmed up enemy: " + type);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[EnemyFactory] Warm-up failed for enemy type: " + type + " - " + ex.Message);
            }
        }

        //Debug.Log("[EnemyFactory] Warm-up complete.");
    }

    static EnemyController InitEnemy(GameObject enemyGO, GameObject prefab, EnemyType type, int id, bool isTrapped)
    {
        EnemyEditor editor = enemyGO.GetComponent<EnemyEditor>();
        if (editor != null)
        {
            editor.enemy_type = type;
            //Debug.LogFormat("[InitEnemy] Overriding EnemyEditor.enemy_type to {0}", type);
        }
        EnemyController ec = Utility.AddEnemyComponent(enemyGO, GetEnemyTypeControllerName(type));
        EnemyData data = EnemyData.CreateData(GameConfig.Instance.EnemyConfig_Set[type]);
        ec.SetEnemyData(data);
        ec.enemyType = type;
        ec.isSpawning = true;
        ec.EnemyID = id;
        ec.Accessory = prefab.GetComponent<SinglePrefabReference>().Accessory;
        ec.is_traped = isTrapped;
        enemyGO.name = "Enemy_" + id;
        enemyGO.GetComponent<EnemyAnimationEvent>().SetController(ec);
        GameSceneController.Instance.Enemy_Set.Add(id, ec);
        GameSceneController.Instance.OnEnemySpawn(data);
        ec.isSpawning = true;
        ec.StartCoroutine(DisableSpawnProtection(ec));

        return ec;
    }

    public static EnemyController CreateEnemyGetEnemyController(EnemyType type, Vector3 pos, Quaternion rot)
    {
        //Debug.Log("[EnemyFactory] CreateEnemyGetEnemyController called with type: " + type);

        if (!GameSceneController.Instance.enemy_ref_map.Enemy_Set.ContainsKey(type))
        {
            //Debug.LogError("[EnemyFactory] Enemy_Set missing key: " + type);
            return null;
        }

        GameObject prefabRef = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
        if (prefabRef == null)
        {
            //Debug.LogError("[EnemyFactory] Prefab reference is null for type: " + type);
            return null;
        }

        var instancePrefab = prefabRef.GetComponent<SinglePrefabReference>().Instance;
        if (instancePrefab == null)
        {
            //Debug.LogError("[EnemyFactory] Instance is null on SinglePrefabReference for type: " + type);
            return null;
        }

        GameObject enemyGO = Object.Instantiate(instancePrefab, pos, rot);
        if (enemyGO == null)
        {
            //Debug.LogError("[EnemyFactory] Failed to instantiate enemy GameObject for type: " + type);
            return null;
        }

        int id = GameSceneController.Instance.EnemyIndex;
        GameSceneController.Instance.EnemyIndex = id + 1;

        EnemyController result = InitEnemy(enemyGO, prefabRef, type, id, false);

        if (result != null)
        {
            /*Debug.LogFormat(
                "[EnemyFactory] Enemy spawned. Requested type: {0}, Prefab: {1}, Resulting enemyType: {2}, Controller: {3}",
                type, prefabRef.name, result.enemyType, result.GetType().Name);*/
        }
        else
        {
            //Debug.LogError("[EnemyFactory] InitEnemy returned null for type: " + type);
        }

        return result;
    }

    public static GameObject CreateEnemyObj(EnemyType type)
    {
        GameObject prefab = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
        return (GameObject)Object.Instantiate(prefab.GetComponent<SinglePrefabReference>().Instance);
    }

    public static EnemyController CreateRemoteEnemy(EnemyType type, Vector3 pos, Quaternion rot, int enemyID, bool isBoss)
    {
        GameObject prefab = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
        GameObject enemyGO = (GameObject)Object.Instantiate(prefab.GetComponent<SinglePrefabReference>().Instance, pos, rot);
        GameSceneController.Instance.EnemyIndex = enemyID + 1;
        return InitEnemy(enemyGO, prefab, type, enemyID, false);
    }

    public static EnemyController CreateEnemyForTrap(EnemyType type, Vector3 pos)
    {
        GameObject prefab = GameSceneController.Instance.enemy_ref_map.Enemy_Set[type];
        GameObject enemyGO = (GameObject)Object.Instantiate(prefab.GetComponent<SinglePrefabReference>().Instance, pos, Quaternion.identity);
        int id = GameSceneController.Instance.EnemyIndex;
        GameSceneController.Instance.EnemyIndex = id + 1;
        return InitEnemy(enemyGO, prefab, type, id, true);
    }

    public static IEnumerator CreateEnemyAsync(EnemyType type, Vector3 pos, Quaternion rot, System.Action<EnemyController> onCreated)
    {
        string path = "PrefabInstance/" + type.ToString();
        ResourceRequest req = Resources.LoadAsync(path);
        while (!req.isDone) yield return null;

        GameObject prefab = req.asset as GameObject;
        if (prefab == null)
        {
            Debug.LogError("EnemyFactory: Failed to load " + path);
            if (onCreated != null) onCreated(null);
            yield break;
        }

        GameObject enemyGO = (GameObject)Object.Instantiate(prefab, pos, rot);
        int id = GameSceneController.Instance.EnemyIndex;
        GameSceneController.Instance.EnemyIndex = id + 1;
        EnemyController ec = InitEnemy(enemyGO, prefab, type, id, false);
        if (onCreated != null) onCreated(ec);
    }

    public static string GetEnemyTypeControllerName(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.E_ZOMBIE:
            case EnemyType.E_ZOMBIE_E:
            case EnemyType.E_ZOMBIE_COMMIS:
            case EnemyType.E_ZOMBIE_COMMIS_E: return "ZombieController";
            case EnemyType.E_NURSE:
            case EnemyType.E_NURSE_E: return "NurseController";
            case EnemyType.E_BOOMER:
            case EnemyType.E_BOOMER_E: return "BoomerController";
            case EnemyType.E_BOOMER_TIMER:
            case EnemyType.E_BOOMER_TIMER_E: return "BoomerTimerController";
            case EnemyType.E_CROW: return "CrowController";
            case EnemyType.E_CLOWN:
            case EnemyType.E_CLOWN_E: return "ClownController";
            case EnemyType.E_FATCOOK:
            case EnemyType.E_FATCOOK_E: return "FatCookController";
            case EnemyType.E_HAOKE_A:
            case EnemyType.E_HAOKE_B: return "HaokeController";
            case EnemyType.E_WRESTLER:
            case EnemyType.E_WRESTLER_E: return "WrestlerController";
            case EnemyType.E_HALLOWEEN:
            case EnemyType.E_HALLOWEEN_E: return "HalloweenController";
            case EnemyType.E_HALLOWEEN_SUB:
            case EnemyType.E_HALLOWEEN_SUB_E: return "HalloweenSubController";
            case EnemyType.E_SHARK:
            case EnemyType.E_SHARK_E: return "SharkController";
            default: return "EnemyController";
        }
    }

    private static IEnumerator DisableSpawnProtection(EnemyController ec)
    {
        yield return new WaitForSeconds(0.5f); // or 1.0f depending on spawn animation time
        if (ec != null)
        {
            ec.isSpawning = false;
            // Debug.Log("Enemy " + ec.name + " is now damageable.");
        }
    }
}