using CoMZ2;
using UnityEngine;

public class PGMCoopProjectile : ProjectileController
{
    public Vector3 targetPos = Vector3.zero;

    public float initAngel = 40f;

    private float lastCheckPosTime;

    private Vector3 lastPos = Vector3.zero;

    public override void Start()
    {
        base.Start();
        lastCheckPosTime = Time.time;
        lastPos = transform.position;

        // Ignore collisions with objects tagged "IgnoreProjectile"
        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null)
        {
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
        else
        {
            Debug.LogWarning("PGMCoopProjectile missing Collider!");
        }
    }

    public override void OnProjectileCollideEnter(GameObject obj)
    {
        if (obj.CompareTag("IgnoreProjectile"))
        {
            return;
        }

        MineProjectile.ChainBoom(centroid, explode_radius);

        PlayerController playerController = object_controller as PlayerController;

        if (obj.layer == PhysicsLayer.PLAYER)
        {
            ObjectController component = obj.GetComponent<ObjectController>();
            if (component != null)
            {
                // Player hit does 0 damage? Or apply damage? Adjust if needed
                component.OnHit(0f, null, object_controller, component.centroid, centroid - component.centroid);
            }
        }
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
            obj.layer = PhysicsLayer.WALL;

            if (obj.GetComponent<RemoveTimerScript>() == null)
            {
                RemoveTimerScript timer = obj.AddComponent<RemoveTimerScript>();
                timer.life = 3f;
            }
        }
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

        bool hitAny = false;

        // Use OverlapSphere to detect enemies in explosion radius
        int enemyLayerMask = 1 << PhysicsLayer.ENEMY;
        Collider[] hitColliders = Physics.OverlapSphere(centroid, explode_radius, enemyLayerMask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            EnemyController enemy = hitColliders[i].GetComponent<EnemyController>();
            if (enemy != null)
            {
                float calculatedDamage = damage;
                if (playerController != null && playerController.avatar_data != null && weapon_controller != null)
                {
                    calculatedDamage = weapon_controller.GetDamageValWithAvatar(playerController, playerController.avatar_data);
                }
                enemy.OnHit(calculatedDamage, null, object_controller, enemy.centroid, centroid - enemy.centroid);
                hitAny = true;
            }
        }

        // Check wood boxes manually
        for (int i = 0; i < GameSceneController.Instance.wood_box_list.Count; i++)
        {
            GameObject item = GameSceneController.Instance.wood_box_list[i];
            WoodBoxController box = item.GetComponent<WoodBoxController>();
            if (box != null)
            {
                float sqrDist = (box.centroid - centroid).sqrMagnitude;
                if (sqrDist < explode_radius * explode_radius)
                {
                    box.OnHit(damage, null, object_controller, box.centroid, centroid - box.centroid);
                    hitAny = true;
                }
            }
        }

        if (hitAny && playerController != null && weapon_controller != null)
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
        transform.LookAt(targetPos);
        initAngel -= deltaTime * 80f;
        if (initAngel < 0f)
        {
            initAngel = 0f;
        }
        transform.rotation = Quaternion.AngleAxis(initAngel, -1f * transform.right) * transform.rotation;
        transform.Rotate(transform.forward, Time.time * 10f, Space.World);

        launch_dir = transform.forward;
        transform.Translate(fly_speed * launch_dir * deltaTime, Space.World);

        if (Time.time - lastCheckPosTime > 0.3f)
        {
            lastCheckPosTime = Time.time;
            if ((transform.position - lastPos).sqrMagnitude < 2f)
            {
                Object.Destroy(gameObject);
            }
            else
            {
                lastPos = transform.position;
            }
        }
    }
}