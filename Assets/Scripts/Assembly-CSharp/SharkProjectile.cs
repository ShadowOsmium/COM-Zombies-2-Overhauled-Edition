using CoMZ2;
using UnityEngine;

public class SharkProjectile : ProjectileController
{
    private float lifeTimer = 0f;

    public float maxLifetime = 3f;

    public Transform target_trans;

    public Vector3 targetPos = Vector3.zero;

    public float initAngel = 40f;

    public override void OnProjectileCollideEnter(GameObject obj)
    {
        Debug.Log("Hit Layer: " + obj.layer + " (" + LayerMask.LayerToName(obj.layer) + ")");
        if (obj.layer == PhysicsLayer.PLAYER || obj.layer == PhysicsLayer.NPC)
        {
            ObjectController component = obj.GetComponent<ObjectController>();
            if (component != null)
            {
                component.OnHit(damage, null, object_controller, component.centroid, Vector3.zero);
            }
        }

        // Destroy regardless of what we hit
        Object.DestroyObject(base.gameObject);
    }

    public override void OnProjectileCollideStay(GameObject obj)
    {
    }

    public override void UpdateTransform(float deltaTime)
    {
        lifeTimer += deltaTime;
        if (lifeTimer >= maxLifetime)
        {
            Object.DestroyObject(gameObject);
            return;
        }

        // Raycast down a bit to check for ground collision (adjust layer mask to your floor/wall layers)
        float groundCheckDistance = 0.3f;
        int groundLayerMask = LayerMask.GetMask("FLOOR", "WALL");

        if (Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayerMask))
        {
            // Ground or wall detected just below, destroy projectile to prevent jitter
            Object.DestroyObject(gameObject);
            return;
        }

        if (target_trans != null)
        {
            // Make sure targetPos updates every frame from target_trans (so it doesn't get stale)
            targetPos = target_trans.position;

            // Optionally clamp Y so we don't LookAt below ground level (optional, tweak groundLevelY)
            float groundLevelY = 0f;
            if (targetPos.y < groundLevelY) targetPos.y = groundLevelY;

            Vector3 currentDir = transform.forward;
            Vector3 desiredDir = (targetPos - transform.position).normalized;

            float turnSpeed = 360f; // degrees/sec
            Quaternion targetRot = Quaternion.LookRotation(desiredDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * deltaTime);

            // Move forward manually (assuming Rigidbody is kinematic)
            Vector3 launchDir = transform.forward;
            transform.Translate(fly_speed * launchDir * deltaTime, Space.World);
        }
        else
        {
            Object.DestroyObject(gameObject);
        }
    }
}