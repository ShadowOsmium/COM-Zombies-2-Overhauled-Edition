using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class MineProjectile : ProjectileController
{
	public bool alive = true;

	public override Vector3 centroid
	{
		get
		{
			return base.transform.position + Vector3.up * 0.5f;
		}
	}

	public override void Start()
	{
		base.GetComponent<Rigidbody>().sleepVelocity = 0.01f;
		base.GetComponent<Renderer>().enabled = false;
		alive = true;
	}

	public override void OnProjectileCollideEnter(GameObject obj)
	{
		if (obj.layer == PhysicsLayer.ENEMY)
		{
			alive = false;
			ChainBoom(centroid, explode_radius);
		}
	}

	public override void OnProjectileCollideStay(GameObject obj)
	{
		if (obj.layer == PhysicsLayer.ENEMY)
		{
			alive = false;
			ChainBoom(centroid, explode_radius);
		}
	}

	public override void Update()
	{
		if (base.GetComponent<Rigidbody>().IsSleeping() && !base.GetComponent<Collider>().isTrigger)
		{
			base.GetComponent<Rigidbody>().useGravity = false;
			base.GetComponent<Rigidbody>().isKinematic = true;
			base.GetComponent<Collider>().isTrigger = true;
			base.GetComponent<Renderer>().enabled = true;
		}
	}

	public static void ChainBoom(Vector3 pos, float radius)
	{
		foreach (MineProjectile item in GameSceneController.Instance.mine_area)
		{
			if (!item.alive || !((item.centroid - pos).sqrMagnitude < radius * radius) || GameSceneController.CheckBlockBetween(pos, item.centroid))
			{
				continue;
			}
			item.alive = false;
			foreach (MineProjectile item2 in GameSceneController.Instance.mine_area)
			{
				if (item2.alive && (item2.centroid - item.centroid).sqrMagnitude < item.explode_radius * item.explode_radius && !GameSceneController.CheckBlockBetween(item.centroid, item2.centroid))
				{
					item2.alive = false;
				}
			}
		}
		List<MineProjectile> list = new List<MineProjectile>();
		foreach (MineProjectile item3 in GameSceneController.Instance.mine_area)
		{
			if (!item3.alive)
			{
				list.Add(item3);
			}
		}
		foreach (MineProjectile item4 in list)
		{
			GameSceneController.Instance.mine_area.Remove(item4);
			item4.BoomEnemy();
		}
	}

	public void BoomEnemy()
	{
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			if ((value.centroid - centroid).sqrMagnitude < explode_radius * explode_radius && !GameSceneController.CheckBlockBetween(centroid, value.centroid))
			{
				value.OnHit(damage, null, object_controller, value.centroid, Vector3.up);
			}
		}
		GameSceneController.Instance.boom_m_pool.GetComponent<ObjectPool>().CreateObject(centroid, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}
}
