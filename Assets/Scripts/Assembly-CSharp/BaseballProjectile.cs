using CoMZ2;
using UnityEngine;

public class BaseballProjectile : ProjectileController
{
    public override void OnProjectileCollideEnter(GameObject obj)
    {
        if (obj.layer == PhysicsLayer.ENEMY)
        {
            ObjectController component = obj.GetComponent<ObjectController>();
            if (component != null)
            {
                PlayerController playerController = object_controller as PlayerController;
                float calculatedDamage = damage;
                if (playerController != null && playerController.avatar_data != null && weapon_controller != null)
                {
                    calculatedDamage = weapon_controller.GetDamageValWithAvatar(playerController, playerController.avatar_data);
                }

                component.OnHit(calculatedDamage, null, object_controller, component.centroid, Vector3.up);
            }
        }
        Object.DestroyObject(base.gameObject);
    }

    public override void OnProjectileCollideStay(GameObject obj)
	{
	}

	public override void UpdateTransform(float deltaTime)
	{
		base.transform.Translate(fly_speed * launch_dir * deltaTime, Space.World);
	}
}
