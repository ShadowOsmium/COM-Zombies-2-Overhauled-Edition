using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
	protected PlayerController player;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnAttack()
	{
		if (player != null)
		{
			player.CheckHit();
		}
	}

	private void OnSpecialAttack()
	{
		if (player != null)
		{
			player.ReleaseChaisawTarget();
		}
	}

	public void SetController(PlayerController pController)
	{
		player = pController;
	}

	private void OnScarecrowBefore()
	{
		if (!(player != null))
		{
		}
	}

	private void OnScarecrowPut()
	{
		if (!(player != null))
		{
		}
	}

	private void OnGrenadeThrow()
	{
		if (!(player != null))
		{
			return;
		}
		SwatController swatController = player as SwatController;
		if (swatController != null)
		{
			swatController.SpawnGrenade();
			return;
		}
		SwatCoopController swatCoopController = player as SwatCoopController;
		if (swatCoopController != null)
		{
			swatCoopController.SpawnGrenade();
		}
	}

	private void OnEnchantTriger()
	{
		if (player != null)
		{
			DoctorController doctorController = player as DoctorController;
			if (doctorController != null)
			{
				doctorController.EnchantFire();
			}
		}
	}
}
