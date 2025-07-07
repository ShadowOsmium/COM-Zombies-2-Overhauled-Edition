using CoMZ2;
using UnityEngine;

public class PGMProjectile : ProjectileController
{
    public NearestTargetInfo target_info;

    public Transform target_trans;

    public Vector3 targetPos = Vector3.zero;

    public float initAngel = 40f;

    public override void Start()
    {
        base.Start();

        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider == null)
        {
            Debug.LogWarning("PGMProjectile missing Collider!");
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
        if (obj.CompareTag("IgnoreProjectile"))
        {
            return;
        }

        MineProjectile.ChainBoom(centroid, explode_radius);

        if (obj.layer == PhysicsLayer.PLAYER)
        {
            ObjectController component = obj.GetComponent<ObjectController>();
            if (component != null)
            {
                component.OnHit(0f, null, object_controller, component.centroid, centroid - component.centroid);
            }
        }
        else if (obj.layer == PhysicsLayer.DYNAMIC_SCENE)
        {
            if (obj.GetComponent<Rigidbody>() == null)
            {
                obj.AddComponent<Rigidbody>();
            }
            Vector3 force = (obj.transform.position - base.transform.position).normalized * 100f;
            Rigidbody component2 = obj.GetComponent<Rigidbody>();
            component2.mass = 5f;
            component2.drag = 1.5f;
            component2.angularDrag = 1.5f;
            component2.AddForceAtPosition(force, obj.transform.position, ForceMode.Impulse);
            obj.layer = PhysicsLayer.WALL;
            obj.AddComponent<RemoveTimerScript>().life = 3f;
        }
        else if (obj.layer == PhysicsLayer.ANIMATION_SCENE)
        {
            GameObject gameObject = obj.transform.parent.gameObject;
            if (gameObject.GetComponent<Animation>() != null)
            {
                gameObject.GetComponent<Animation>()["Take 001"].clip.wrapMode = WrapMode.Once;
                gameObject.GetComponent<Animation>().Play("Take 001");
            }
        }

        PlayerController playerController = object_controller as PlayerController;
        bool hitAny = false;

        // Use physics to detect enemies within explosion radius
        int enemyLayerMask = 1 << PhysicsLayer.ENEMY;
        Collider[] hitColliders = Physics.OverlapSphere(centroid, explode_radius, enemyLayerMask);

        foreach (Collider col in hitColliders)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
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
        foreach (GameObject item in GameSceneController.Instance.wood_box_list)
        {
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

        if (hitAny && weapon_controller != null && playerController != null)
        {
            playerController.AddComboValue(weapon_controller.weapon_data.config.combo_base);
        }

        GameSceneController.Instance.boom_m_pool.GetComponent<ObjectPool>().CreateObject(centroid, Quaternion.identity);
        Object.DestroyObject(base.gameObject);
    }

    public override void OnProjectileCollideStay(GameObject obj)
    {
    }

    public override void UpdateTransform(float deltaTime)
    {
        if (target_info != null && target_trans != null)
        {
            targetPos = target_info.LockPosition;
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

            if (target_info.type == NearestTargetInfo.NearestTargetType.Enemy)
            {
                EnemyController enemyController = target_info.target_obj as EnemyController;
                if (enemyController != null && enemyController.Enemy_State.GetStateType() == EnemyStateType.Dead)
                {
                    Object.DestroyObject(base.gameObject);
                }
            }
        }
        else
        {
            Object.DestroyObject(base.gameObject);
        }
    }
}