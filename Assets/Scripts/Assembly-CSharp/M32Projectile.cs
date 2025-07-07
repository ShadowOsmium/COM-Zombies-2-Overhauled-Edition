using CoMZ2;
using UnityEngine;

public class M32Projectile : ProjectileController
{
    public Vector3 targetPos = Vector3.zero;

    public override void Start()
    {
        base.Start();

        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider == null)
        {
            Debug.LogWarning("M32Projectile missing collider!");
            return;
        }

        GameObject[] ignoreObjs = GameObject.FindGameObjectsWithTag("IgnoreProjectile");
        foreach (GameObject obj in ignoreObjs)
        {
            Collider c = obj.GetComponent<Collider>();
            if (c != null)
            {
                Physics.IgnoreCollision(projectileCollider, c);
            }
        }
    }

    public override void OnProjectileCollideEnter(GameObject obj)
    {
        MineProjectile.ChainBoom(centroid, explode_radius);

        PlayerController playerController = object_controller as PlayerController;

        // Hit player with calculated damage
        if (obj.layer == PhysicsLayer.PLAYER)
        {
            ObjectController component = obj.GetComponent<ObjectController>();
            if (component != null)
            {
                float calculatedDamage = damage;
                if (playerController != null && playerController.avatar_data != null && weapon_controller != null)
                {
                    calculatedDamage = weapon_controller.GetDamageValWithAvatar(playerController, playerController.avatar_data);
                }
                component.OnHit(calculatedDamage, null, object_controller, component.centroid, centroid - component.centroid);
            }
        }
        // Push DYNAMIC_SCENE objects with physics
        else if (obj.layer == PhysicsLayer.DYNAMIC_SCENE)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = obj.AddComponent<Rigidbody>();
            }

            Vector3 force = (obj.transform.position - transform.position).normalized * 100f;

            rb.mass = 5f;
            rb.drag = 1.5f;
            rb.angularDrag = 1.5f;
            rb.AddForceAtPosition(force, obj.transform.position, ForceMode.Impulse);

            if (obj.GetComponent<RemoveTimerScript>() == null)
            {
                RemoveTimerScript timer = obj.AddComponent<RemoveTimerScript>();
                timer.life = 3f;
            }
        }
        // Animation trigger
        else if (obj.layer == PhysicsLayer.ANIMATION_SCENE)
        {
            Transform parentTrans = obj.transform.parent;
            if (parentTrans != null)
            {
                GameObject parent = parentTrans.gameObject;
                Animation anim = parent.GetComponent<Animation>();
                if (anim != null)
                {
                    AnimationState animState = anim["Take 001"];
                    if (animState != null)
                    {
                        animState.clip.wrapMode = WrapMode.Once;
                        anim.Play("Take 001");
                    }
                }
            }
        }

        // Damage enemies with calculated damage
        bool hitSomething = false;
        foreach (EnemyController enemy in GameSceneController.Instance.Enemy_Set.Values)
        {
            if ((enemy.centroid - centroid).sqrMagnitude < explode_radius * explode_radius &&
                !GameSceneController.CheckBlockBetween(centroid, enemy.centroid))
            {
                float calculatedDamage = damage;
                if (playerController != null && playerController.avatar_data != null && weapon_controller != null)
                {
                    calculatedDamage = weapon_controller.GetDamageValWithAvatar(playerController, playerController.avatar_data);
                }
                enemy.OnHit(calculatedDamage, null, object_controller, enemy.centroid, centroid - enemy.centroid);
                hitSomething = true;
            }
        }

        // Damage wooden boxes with calculated damage
        foreach (GameObject item in GameSceneController.Instance.wood_box_list)
        {
            WoodBoxController box = item.GetComponent<WoodBoxController>();
            if (box != null &&
                (box.centroid - centroid).sqrMagnitude < explode_radius * explode_radius &&
                !GameSceneController.CheckBlockBetween(centroid, box.centroid))
            {
                float calculatedDamage = damage;
                if (playerController != null && playerController.avatar_data != null && weapon_controller != null)
                {
                    calculatedDamage = weapon_controller.GetDamageValWithAvatar(playerController, playerController.avatar_data);
                }
                box.OnHit(calculatedDamage, null, object_controller, box.centroid, centroid - box.centroid);
                hitSomething = true;
            }
        }

        if (hitSomething && playerController != null && weapon_controller != null)
        {
            playerController.AddComboValue(weapon_controller.weapon_data.config.combo_base);
        }

        GameSceneController.Instance.boom_m_pool.GetComponent<ObjectPool>().CreateObject(centroid, Quaternion.identity);
        Object.DestroyObject(gameObject);
    }

    public override void OnProjectileCollideStay(GameObject obj)
    {
    }

    public override void UpdateTransform(float deltaTime)
    {
        transform.Rotate(Vector3.right, deltaTime * 80f, Space.Self);
        launch_dir = transform.forward;
        transform.Translate(fly_speed * launch_dir * deltaTime, Space.World);
    }

    public void InitLaunchAngel(float angle)
    {
        transform.Rotate(Vector3.right, -angle, Space.Self);
    }
}