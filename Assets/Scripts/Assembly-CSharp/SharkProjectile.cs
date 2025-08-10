using CoMZ2;
using UnityEngine;

public class SharkProjectile : ProjectileController
{
    private float lifeTimer = 0f;
    public float maxLifetime = 5f;

    public Transform target_trans;
    public Vector3 targetPos = Vector3.zero;

    private Vector3 lastPosition = Vector3.zero;
    private float stuckTimer = 0f;
    private const float stuckTimeout = 0.5f;

    private bool hasHit = false;

    public override void Start()
    {
        base.Start();

        gameObject.layer = PhysicsLayer.ENEMY_PROJECTILE;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    public override void OnProjectileCollideEnter(GameObject obj)
    {
        if (hasHit) return;

        if (obj == gameObject ||
            (object_controller != null && obj == object_controller.gameObject) ||
            obj.layer == PhysicsLayer.ENEMY_PROJECTILE ||
            obj.CompareTag("IgnoreProjectile") ||
            obj.CompareTag("Zombie_Grave"))
        {
            return;
        }

        if (obj.layer == PhysicsLayer.PLAYER || obj.layer == PhysicsLayer.NPC)
        {
            hasHit = true;
            ObjectController component = obj.GetComponent<ObjectController>();
            if (component != null)
            {
                component.OnHit(damage, null, object_controller, component.centroid, Vector3.zero);
            }
            Object.DestroyObject(gameObject);
            return;
        }

        if (obj.layer == PhysicsLayer.WALL || obj.layer == PhysicsLayer.FLOOR)
        {
            Object.DestroyObject(gameObject);
        }
    }

    public override void UpdateTransform(float deltaTime)
    {
        lifeTimer += deltaTime;
        if (lifeTimer >= maxLifetime)
        {
            Object.DestroyObject(gameObject);
            return;
        }

        if (target_trans == null)
        {
            Object.DestroyObject(gameObject);
            return;
        }

        targetPos = target_trans.position;
        float dist = Vector3.Distance(transform.position, targetPos);

        // Homing rotation
        Vector3 offset = targetPos - transform.position;
        if (offset.sqrMagnitude < 0.01f)
            offset = transform.forward;

        Vector3 desiredDir = offset.normalized;
        Quaternion targetRot = Quaternion.LookRotation(desiredDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 720f * deltaTime);

        float stepDistance = fly_speed * deltaTime;

        // --- Priority Player Detection ---
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.4f);
        foreach (Collider c in hits)
        {
            if (c != null && c.gameObject == target_trans.gameObject)
            {
                OnProjectileCollideEnter(c.gameObject);
                return;
            }
        }

        // --- SphereCast for other objects ---
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, stepDistance))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                GameObject obj = hit.collider.gameObject;

                if (obj.layer == PhysicsLayer.ENEMY ||
                    obj.CompareTag("Zombie_Grave") ||
                    obj.CompareTag("IgnoreProjectile"))
                {
                    // Ignore enemies & graves
                }
                else
                {
                    OnProjectileCollideEnter(obj);
                    return;
                }
            }
        }

        // Move forward
        transform.position += transform.forward * stepDistance;

        // Detect if stuck (not moving forward)
        if ((transform.position - lastPosition).sqrMagnitude < 0.0001f)
        {
            stuckTimer += deltaTime;
            if (stuckTimer > stuckTimeout)
            {
                Object.DestroyObject(gameObject);
                return;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }
}