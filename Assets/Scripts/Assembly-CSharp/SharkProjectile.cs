using CoMZ2;
using UnityEngine;

public class SharkProjectile : ProjectileController
{
    public Transform target_trans;

    public Vector3 targetPos = Vector3.zero;

    public float initAngel = 40f;

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
        if (target_trans != null)
        {
            base.transform.LookAt(targetPos);
            initAngel -= deltaTime * 80f;
            if (initAngel <= 0f)
            {
                initAngel = 0f;
            }
            base.transform.rotation = Quaternion.AngleAxis(initAngel, -1f * base.transform.right) * base.transform.rotation;
            base.transform.Rotate(base.transform.forward, Time.time * 10f, Space.World);
            launch_dir = base.transform.forward;
            base.transform.Translate(fly_speed * launch_dir * deltaTime, Space.World);
        }
        else
        {
            Object.DestroyObject(base.gameObject);
        }
    }
}