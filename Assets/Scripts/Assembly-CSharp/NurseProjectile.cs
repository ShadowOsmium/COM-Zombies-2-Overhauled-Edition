using CoMZ2;
using UnityEngine;

public class NurseProjectile : ProjectileController
{
	public GameObject Saliva_area;

	public bool is_enchant;

	public override void OnProjectileCollideEnter(GameObject obj)
	{
		if (obj.layer == PhysicsLayer.PLAYER || obj.layer == PhysicsLayer.NPC || obj.layer == PhysicsLayer.ENEMY)
		{
			ObjectController component = obj.GetComponent<ObjectController>();
			if (component != null)
			{
				component.OnHit(damage, null, object_controller, component.centroid, Vector3.zero);
			}
		}
		Ray ray = new Ray(base.transform.position + Vector3.up * 1f, Vector3.down);
		Quaternion rotation = Quaternion.identity;
		float num = 10000f;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD)))
		{
			num = hitInfo.point.y;
			rotation = Quaternion.LookRotation(Vector3.forward, hitInfo.normal);
		}
		Object.Instantiate(Saliva_area, new Vector3(base.transform.position.x, num + 0.1f, base.transform.position.z), rotation);
		Object.DestroyObject(base.gameObject);
	}

	public override void OnProjectileCollideStay(GameObject obj)
	{
	}

	public override void UpdateTransform(float deltaTime)
	{
		launch_speed += Physics.gravity.y * Vector3.up * deltaTime;
		base.transform.Translate(launch_speed * deltaTime, Space.World);
		base.transform.LookAt(base.transform.position + launch_speed * 10f);
	}
}
