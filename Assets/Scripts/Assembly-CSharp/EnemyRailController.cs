using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class EnemyRailController : MonoBehaviour
{
	public bool rail_triger;

	public List<EnemyController> rail_enemys = new List<EnemyController>();

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("other:" + other.gameObject.name);
		if (rail_triger && other.gameObject.layer == PhysicsLayer.ENEMY)
		{
			EnemyController component = other.gameObject.GetComponent<EnemyController>();
			if (component != null && component.RailEnable && !component.IsRail)
			{
				component.StartRailing();
				component.cur_rail = this;
				rail_enemys.Add(component);
			}
		}
	}

	public void RemoveEnemy(EnemyController enemy)
	{
		if (rail_enemys.Contains(enemy))
		{
			rail_enemys.Remove(enemy);
		}
	}

	public void ReleaseRailEnemys()
	{
		rail_triger = false;
		foreach (EnemyController rail_enemy in rail_enemys)
		{
			rail_enemy.cur_rail = null;
			rail_enemy.EndRailing();
		}
		rail_enemys.Clear();
	}

	public void StartRail()
	{
		rail_triger = true;
	}
}
