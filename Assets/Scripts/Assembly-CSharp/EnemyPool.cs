using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    private Dictionary<EnemyType, Queue<GameObject>> pool = new Dictionary<EnemyType, Queue<GameObject>>();
    private Dictionary<EnemyType, GameObject> prefabMap = new Dictionary<EnemyType, GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterPrefab(EnemyType type, GameObject prefab, int preloadCount)
    {
        if (!pool.ContainsKey(type))
            pool[type] = new Queue<GameObject>();

        prefabMap[type] = prefab;

        for (int i = 0; i < preloadCount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool[type].Enqueue(obj);
        }
    }

    public GameObject GetEnemy(EnemyType type)
    {
        if (!prefabMap.ContainsKey(type))
        {
            Debug.LogError("[EnemyPool] No prefab registered for type: " + type);
            return null;
        }

        GameObject obj;
        if (pool.ContainsKey(type) && pool[type].Count > 0)
        {
            obj = pool[type].Dequeue();
        }
        else
        {
            obj = Instantiate(prefabMap[type]);
        }

        obj.SetActive(true);
        return obj;
    }

    public void ReturnEnemy(EnemyType type, GameObject obj)
    {
        obj.SetActive(false);
        if (!pool.ContainsKey(type))
            pool[type] = new Queue<GameObject>();

        pool[type].Enqueue(obj);
    }

    public bool IsRegistered(EnemyType type)
    {
        return prefabMap.ContainsKey(type);
    }
}