using UnityEngine;

public class MedicineProjectile : ProjectileController
{
	public GameObject explode;

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
	}

	public override void OnProjectileCollideEnter(GameObject obj)
	{
	}

	public override void Update()
	{
	}

	public void MedicineBoom()
	{
		Invoke("Recover", 0.55f);
	}

	private void Recover()
	{
		Debug.Log("heal:" + damage + " range:" + explode_radius);
		GameSceneController.Instance.player_controller.GetComponent<PlayerController>().Recover(damage);
		GameSceneController.Instance.hp_add_ring_pool.GetComponent<ObjectPool>().CreateObject(centroid, Quaternion.identity);
		foreach (NPCController value in GameSceneController.Instance.NPC_Set.Values)
		{
			if ((value.centroid - centroid).sqrMagnitude < explode_radius * explode_radius && !GameSceneController.CheckBlockBetween(centroid, value.centroid))
			{
				value.Recover(damage);
			}
		}
		Invoke("DeleteSelf", 0.2f);
	}

	private void DeleteSelf()
	{
		Object.Destroy(base.gameObject);
	}
}
