using CoMZ2;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMap : MonoBehaviour
{
    public Dictionary<EnemyType, GameObject> Enemy_Set = new Dictionary<EnemyType, GameObject>();

    public static EnemyMap Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple EnemyMap instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[EnemyMap] Instance assigned.");
        // DontDestroyOnLoad(gameObject);
    }

    public void ResetEnemyMapInfo(List<EnemyType> enemy_type_list)
    {
        if (enemy_type_list == null || enemy_type_list.Count == 0)
        {
            Debug.LogWarning("[EnemyMap] ResetEnemyMapInfo called with empty or null enemy_type_list.");
            return;
        }

        Debug.Log("[EnemyMap] ResetEnemyMapInfo called with enemies: " + string.Join(", ", enemy_type_list.ConvertAll(e => e.ToString()).ToArray()));

        Enemy_Set.Clear();

        GameObject prefab = null;
        foreach (EnemyType item in enemy_type_list)
        {
            prefab = Resources.Load<GameObject>("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[item].enemy_name);
            Debug.Log("[EnemyMap] Attempting to load: Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[item].enemy_name + " for type: " + item);
            if (prefab == null)
                Debug.LogError("[EnemyMap] Failed to load prefab for enemy: " + item);
            else
                Enemy_Set[item] = prefab;

            // Also explicitly add minion prefabs if boss types require them
            switch (item)
            {
                case EnemyType.E_FATCOOK:
                    prefab = Resources.Load<GameObject>("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[EnemyType.E_BOOMER_TIMER].enemy_name);
                    if (prefab == null)
                        Debug.LogError("[EnemyMap] Failed to load minion prefab: " + EnemyType.E_BOOMER_TIMER);
                    else
                        Enemy_Set[EnemyType.E_BOOMER_TIMER] = prefab;
                    break;

                case EnemyType.E_FATCOOK_E:
                    prefab = Resources.Load<GameObject>("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[EnemyType.E_BOOMER_TIMER_E].enemy_name);
                    if (prefab == null)
                        Debug.LogError("[EnemyMap] Failed to load minion prefab: " + EnemyType.E_BOOMER_TIMER_E);
                    else
                        Enemy_Set[EnemyType.E_BOOMER_TIMER_E] = prefab;
                    break;

                case EnemyType.E_HALLOWEEN:
                    prefab = Resources.Load<GameObject>("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[EnemyType.E_HALLOWEEN_SUB].enemy_name);
                    if (prefab == null)
                        Debug.LogError("[EnemyMap] Failed to load minion prefab: " + EnemyType.E_HALLOWEEN_SUB);
                    else
                        Enemy_Set[EnemyType.E_HALLOWEEN_SUB] = prefab;
                    break;

                case EnemyType.E_HALLOWEEN_E:
                    prefab = Resources.Load<GameObject>("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[EnemyType.E_HALLOWEEN_SUB_E].enemy_name);
                    if (prefab == null)
                        Debug.LogError("[EnemyMap] Failed to load minion prefab: " + EnemyType.E_HALLOWEEN_SUB_E);
                    else
                        Enemy_Set[EnemyType.E_HALLOWEEN_SUB_E] = prefab;
                    break;
            }
        }

        // Extra safeguard: Ensure minion prefabs always exist
        EnemyType[] forcedMinions = new EnemyType[]
        {
            EnemyType.E_BOOMER_TIMER,
            EnemyType.E_BOOMER_TIMER_E,
            EnemyType.E_HALLOWEEN_SUB,
            EnemyType.E_HALLOWEEN_SUB_E
        };

        foreach (var minion in forcedMinions)
        {
            if (!Enemy_Set.ContainsKey(minion))
            {
                prefab = Resources.Load<GameObject>("Prefabs/Enemy/" + GameConfig.Instance.EnemyConfig_Set[minion].enemy_name);
                if (prefab == null)
                    Debug.LogError("[EnemyMap] Forced load failed for minion prefab: " + minion);
                else
                    Enemy_Set[minion] = prefab;
            }
        }

        // Debug output all loaded keys and prefab names
        Debug.Log("[EnemyMap] Final loaded Enemy_Set keys and prefabs:");
        foreach (var kvp in Enemy_Set)
        {
            Debug.Log("  " + kvp.Key + " => " + (kvp.Value != null ? kvp.Value.name : "null"));
        }

        // Instantiate each prefab once and disable it to cache
        foreach (var key in Enemy_Set.Keys)
        {
            GameObject go = EnemyFactory.CreateEnemyObj(key);
            if (go != null)
                go.SetActive(false);
            else
                Debug.LogError("[EnemyMap] Failed to create enemy object for: " + key);
        }
    }

    // Manual test method to force load elite Halloween enemies and minions
    public void ForceLoadHalloweenElite()
    {
        List<EnemyType> halloweenEliteList = new List<EnemyType>()
        {
            EnemyType.E_HALLOWEEN_E,
            EnemyType.E_HALLOWEEN_SUB_E
        };
        Debug.Log("[EnemyMap] ForceLoadHalloweenElite called.");
        ResetEnemyMapInfo(halloweenEliteList);
    }

    public void PreloadBosses(List<EnemyType> bossTypes)
    {
        foreach (EnemyType bossType in bossTypes)
        {
            if (!Enemy_Set.ContainsKey(bossType))
            {
                Debug.LogWarning("[EnemyMap] Boss type not in map: " + bossType);
                continue;
            }

            GameObject prefab = Enemy_Set[bossType];
            if (prefab == null)
            {
                Debug.LogWarning("[EnemyMap] Prefab missing for boss type: " + bossType);
                continue;
            }

            // Instantiate once and immediately disable to cache it
            GameObject go = Instantiate(prefab);
            go.name = prefab.name + "_PreloadCache";
            go.SetActive(false);
        }
    }

}
