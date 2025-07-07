using CoMZ2;
using UnityEngine;

public class HaokeProjectile : ProjectileController
{
	public bool is_ready;

    public override void Start()
    {
        base.Start();

        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider == null)
        {
            Debug.LogWarning("HaokeProjectile missing Collider!");
            return;
        }

        GameObject[] invisibleWalls = GameObject.FindGameObjectsWithTag("IgnoreProjectile");
        for (int i = 0; i < invisibleWalls.Length; i++)
        {
            Collider wallCollider = invisibleWalls[i].GetComponent<Collider>();
            if (wallCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, wallCollider);
            }
        }
    }

    public override void OnProjectileCollideEnter(GameObject obj)
	{
		if (!is_ready)
		{
			return;
		}
		bool flag = false;
		if (obj.layer == PhysicsLayer.PLAYER || obj.layer == PhysicsLayer.NPC)
		{
			flag = true;
		}
		foreach (PlayerController value in GameSceneController.Instance.Player_Set.Values)
		{
			if ((value.centroid - centroid).sqrMagnitude < explode_radius * explode_radius && !GameSceneController.CheckBlockBetween(centroid, value.centroid))
			{
				value.OnHit(damage, null, object_controller, value.centroid, Vector3.up);
			}
		}
		foreach (NPCController value2 in GameSceneController.Instance.NPC_Set.Values)
		{
			if ((value2.centroid - centroid).sqrMagnitude < explode_radius * explode_radius && !GameSceneController.CheckBlockBetween(centroid, value2.centroid))
			{
				value2.OnHit(damage, null, object_controller, value2.centroid, Vector3.up);
			}
		}
		if (flag)
		{
			GameSceneController.Instance.stone_boom_pool.GetComponent<ObjectPool>().CreateObject(obj.transform.position, Quaternion.identity);
		}
		else
		{
			GameSceneController.Instance.stone_boom_g_pool.GetComponent<ObjectPool>().CreateObject(centroid, Quaternion.identity);
		}
		GameSceneController.Instance.main_camera.StartCommonShake();
		Object.DestroyObject(base.gameObject);
	}

	public override void OnProjectileCollideStay(GameObject obj)
	{
	}

	public override void Update()
	{
		UpdateTransform(Time.deltaTime);
	}

	public override void UpdateTransform(float deltaTime)
	{
		if (is_ready)
		{
			launch_speed += Physics.gravity.y * Vector3.up * deltaTime;
			base.transform.Translate(launch_speed * deltaTime, Space.World);
			base.transform.Rotate(Vector3.right, -1000f * deltaTime, Space.Self);
		}
	}
}
