using CoMZ2;
using UnityEngine;

public class MiantuanProjectile : ProjectileController
{
	public float freezeCD = 180f;

	public GameObject EffectFreeze;

	public GameObject EffectOnPlayer;

	public GameObject EffectOnWall;

	public override void OnProjectileCollideEnter(GameObject obj)
	{
		Object.Destroy(base.gameObject);
		Ray ray = new Ray(base.transform.position + Vector3.up, Vector3.down);
		Quaternion rotation = Quaternion.identity;
		float num = 10000f;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD)))
		{
			num = hitInfo.point.y;
			rotation = Quaternion.LookRotation(Vector3.forward, hitInfo.normal);
		}
		if (obj.layer == PhysicsLayer.PLAYER)
		{
			PlayerController component = obj.GetComponent<PlayerController>();
			if (component != null)
			{
				component.StartLimitMove(freezeCD);
				Object.Instantiate(EffectFreeze, component.transform.position, rotation);
				Object.Instantiate(EffectOnPlayer, new Vector3(component.transform.position.x, num + 0.05f, component.transform.position.z), rotation);
			}
		}
		else
		{
			Object.Instantiate(EffectOnWall, new Vector3(base.transform.position.x, num + 0.05f, base.transform.position.z), rotation);
		}
	}

	public override void UpdateTransform(float deltaTime)
	{
		base.transform.Translate(fly_speed * launch_dir * deltaTime, Space.World);
	}
}
