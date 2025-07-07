using CoMZ2;
using UnityEngine;

public class RPGProjectile : ProjectileController
{
    private Collider projectileCollider;

    public override void Start()
    {
        base.Start();

        projectileCollider = GetComponent<Collider>();
        if (projectileCollider == null)
        {
            Debug.LogWarning("RPGProjectile missing collider!");
            return;
        }

        // Find all invisible walls and ignore collision with them
        GameObject[] invisibleWalls = GameObject.FindGameObjectsWithTag("IgnoreProjectile");
        foreach (var wall in invisibleWalls)
        {
            Collider wallCollider = wall.GetComponent<Collider>();
            if (wallCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, wallCollider);
            }
        }
    }

    public override void OnProjectileCollideEnter(GameObject obj)
    {
        MineProjectile.ChainBoom(centroid, explode_radius);

        PlayerController playerController = object_controller as PlayerController;
        float calculatedDamage = damage;

        // Calculate scaled damage if possible
        if (playerController != null && playerController.avatar_data != null && weapon_controller != null)
        {
            calculatedDamage = weapon_controller.GetDamageValWithAvatar(playerController, playerController.avatar_data);
        }

        if (obj.layer == PhysicsLayer.PLAYER)
        {
            ObjectController component = obj.GetComponent<ObjectController>();
            if (component != null)
            {
                // Usually no damage to player? Keep 0 or change if needed
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

        bool hitSomething = false;

        foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
        {
            if ((value.centroid - centroid).sqrMagnitude < explode_radius * explode_radius &&
                !GameSceneController.CheckBlockBetween(centroid, value.centroid))
            {
                value.OnHit(calculatedDamage, null, object_controller, value.centroid, centroid - value.centroid);
                hitSomething = true;
            }
        }
        foreach (GameObject item in GameSceneController.Instance.wood_box_list)
        {
            WoodBoxController component3 = item.GetComponent<WoodBoxController>();
            if (component3 != null &&
                (component3.centroid - centroid).sqrMagnitude < explode_radius * explode_radius &&
                !GameSceneController.CheckBlockBetween(centroid, component3.centroid))
            {
                component3.OnHit(calculatedDamage, null, object_controller, component3.centroid, centroid - component3.centroid);
                hitSomething = true;
            }
        }

        if (hitSomething && weapon_controller != null && playerController != null)
        {
            playerController.AddComboValue(weapon_controller.weapon_data.config.combo_base);
        }

        GameSceneController.Instance.boom_l_pool.GetComponent<ObjectPool>().CreateObject(centroid, Quaternion.identity);
        Object.DestroyObject(base.gameObject);
    }

    public override void OnProjectileCollideStay(GameObject obj)
    {
    }

    public override void UpdateTransform(float deltaTime)
    {
        base.transform.Translate(fly_speed * launch_dir * deltaTime, Space.World);
        base.transform.Rotate(Vector3.forward, deltaTime * 1000f, Space.Self);
    }
}
