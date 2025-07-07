using UnityEngine;

public class ObjectController : MonoBehaviour
{
	public enum ControllerType
	{
		None,
		Player,
		Npc,
		Enemy,
		GuardianForce
	}

	public ControllerType controller_type;

	public virtual Vector3 centroid
	{
		get
		{
			return base.transform.position + Vector3.up;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	public virtual void OnHit(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
	}

	public virtual void OnDead(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
	}

	public virtual void Rebirth()
	{
	}
}
