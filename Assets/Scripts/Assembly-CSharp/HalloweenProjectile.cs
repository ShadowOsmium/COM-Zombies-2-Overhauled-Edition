using CoMZ2;
using UnityEngine;

public class HalloweenProjectile : ProjectileController
{
	public override void Start()
	{
		base.Start();
		GetComponent<EffectAudioBehaviour>().PlayEffect();
        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider == null)
        {
            Debug.LogWarning("HalloweenProjectile missing Collider!");
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
		if (obj.layer == PhysicsLayer.PLAYER || obj.layer == PhysicsLayer.NPC || obj.layer == PhysicsLayer.ENEMY)
		{
			ObjectController component = obj.GetComponent<ObjectController>();
			if (component != null)
			{
				component.OnHit(damage, null, object_controller, component.centroid, Vector3.zero);
			}
		}
		Object.DestroyObject(base.gameObject);
	}

	public override void OnProjectileCollideStay(GameObject obj)
	{
	}

	public override void UpdateTransform(float deltaTime)
	{
		base.transform.Translate(launch_speed * deltaTime, Space.World);
	}
}
