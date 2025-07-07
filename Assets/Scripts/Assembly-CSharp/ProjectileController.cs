using UnityEngine;

public class ProjectileController : MonoBehaviour
{
	public Vector3 launch_dir;

	public float explode_radius;

	public Vector3 launch_speed;

	public float fly_speed;

	public float life = 2f;

	public float damage;

	protected float createdTime;

	public ObjectController object_controller;

	public WeaponController weapon_controller;

	public virtual Vector3 centroid
	{
		get
		{
			return base.transform.position;
		}
	}

	public virtual void Start()
	{
		createdTime = Time.time;
        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider == null)
        {
            Debug.LogWarning("ProjectileController has no Collider component!");
            return;
        }

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

	public virtual void Update()
	{
		if (Time.time - createdTime > life && base.gameObject != null)
		{
			Object.DestroyObject(base.gameObject);
		}
		UpdateTransform(Time.deltaTime);
	}

	private void OnTriggerStay(Collider other)
	{
		OnProjectileCollideStay(other.gameObject);
	}

    public float CalculateDamage()
    {
        PlayerController playerController = object_controller as PlayerController;
        if (playerController != null && playerController.avatar_data != null && weapon_controller != null)
        {
            return weapon_controller.GetDamageValWithAvatar(playerController, playerController.avatar_data);
        }
        return damage;
    }   

    private void OnTriggerEnter(Collider other)
	{
		OnProjectileCollideEnter(other.gameObject);
	}

	public virtual void OnProjectileCollideEnter(GameObject obj)
	{
	}

	public virtual void OnProjectileCollideStay(GameObject obj)
	{
	}

	public virtual void UpdateTransform(float deltaTime)
	{
	}
}
