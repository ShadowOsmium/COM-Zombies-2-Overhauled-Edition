using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public GameObject obj_ref;

	public List<GameObject> objects = new List<GameObject>();

	protected List<float> createdTime = new List<float>();

	protected float life = 1f;

	private void Update()
	{
		AutoDestruct();
	}

	public void Init(string poolName, GameObject prefab, int initNum, float life)
	{
		this.life = life;
		base.gameObject.name = poolName;
		obj_ref = prefab;
		for (int i = 0; i < initNum; i++)
		{
			GameObject gameObject = Object.Instantiate(obj_ref) as GameObject;
			objects.Add(gameObject);
			createdTime.Add(0f);
			gameObject.transform.parent = base.transform;
			if (gameObject.GetComponent<PoolItem>() != null)
			{
				gameObject.GetComponent<PoolItem>().OnItemCreate();
			}
			gameObject.SetActive(false);
		}
        if (prefab == null)
        {
            Debug.LogError("ObjectPool.Init failed: prefab is missing for pool '" + poolName + "'.");
            return;
        }
    }

	public GameObject CreateObject()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null && !objects[i].activeInHierarchy)
			{
				if (objects[i].GetComponent<PoolItem>() != null)
				{
					objects[i].GetComponent<PoolItem>().OnItemActive();
				}
				objects[i].SetActive(true);
				createdTime[i] = Time.time;
				return objects[i];
			}
		}
		GameObject gameObject = Object.Instantiate(obj_ref) as GameObject;
		objects.Add(gameObject);
		createdTime.Add(Time.time);
		gameObject.transform.parent = base.transform;
		if (gameObject.GetComponent<PoolItem>() != null)
		{
			gameObject.GetComponent<PoolItem>().OnItemActive();
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	public GameObject CreateObject(Vector3 position, Quaternion rotation)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null && !objects[i].activeInHierarchy)
			{
				objects[i].transform.position = position;
				objects[i].transform.rotation = rotation;
				objects[i].SetActive(true);
				if (objects[i].GetComponent<PoolItem>() != null)
				{
					objects[i].GetComponent<PoolItem>().OnItemActive();
				}
				createdTime[i] = Time.time;
				return objects[i];
			}
		}
		GameObject gameObject = Object.Instantiate(obj_ref) as GameObject;
		objects.Add(gameObject);
		createdTime.Add(Time.time);
		gameObject.SetActive(true);
		gameObject.transform.parent = base.transform;
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		if (gameObject.GetComponent<PoolItem>() != null)
		{
			gameObject.GetComponent<PoolItem>().OnItemActive();
		}
		return gameObject;
	}

	public GameObject CreateObject(Vector3 position, Vector3 lookAtRotation)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null && !objects[i].activeInHierarchy)
			{
				if (objects[i].GetComponent<PoolItem>() != null)
				{
					objects[i].GetComponent<PoolItem>().OnItemActive();
				}
				objects[i].SetActive(true);
				objects[i].transform.position = position;
				objects[i].transform.rotation = Quaternion.LookRotation(lookAtRotation);
				createdTime[i] = Time.time;
				return objects[i];
			}
		}
		GameObject gameObject = Object.Instantiate(obj_ref) as GameObject;
		objects.Add(gameObject);
		createdTime.Add(Time.time);
		gameObject.transform.parent = base.transform;
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.LookRotation(lookAtRotation);
		if (gameObject.GetComponent<PoolItem>() != null)
		{
			gameObject.GetComponent<PoolItem>().OnItemActive();
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	public void AutoDestruct()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null && objects[i].activeInHierarchy && life > 0f && Time.time - createdTime[i] > life)
			{
				if (objects[i].GetComponent<PoolItem>() != null)
				{
					objects[i].GetComponent<PoolItem>().OnItemDeactive();
				}
				objects[i].SetActive(false);
			}
		}
	}

	public void AutoDestructAll()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null && objects[i].activeInHierarchy)
			{
				if (objects[i].GetComponent<PoolItem>() != null)
				{
					objects[i].GetComponent<PoolItem>().OnItemDeactive();
				}
				objects[i].SetActive(false);
			}
		}
	}

	public GameObject DeleteObject(GameObject obj)
	{
		if (obj.GetComponent<PoolItem>() != null)
		{
			obj.GetComponent<PoolItem>().OnItemDeactive();
		}
		obj.SetActive(false);
		return obj;
	}
}
