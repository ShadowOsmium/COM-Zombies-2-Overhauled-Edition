using UnityEngine;

public class FlameController : WeaponController
{
	protected Transform fire_ori;

	private void Awake()
	{
		weapon_type = WeaponType.AssaultRifle;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void Start()
	{
		fire_ori = base.transform.Find("fire_ori");
		if (fire_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find fire_ori!"));
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_IDLE_RUN = "Flame_UpperBody_Run01";
		ANI_IDLE_SHOOT = "Flame_UpperBody_Idle01";
		ANI_SHOOT = "Flame_UpperBody_Shooting01";
		ANI_RELOAD = "AK_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "Flame_UpperBody_Shiftweapon01";
		ANI_IDLE_DOWN = "Flame_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "Flame_LowerBody_Shooting01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		render_obj = base.gameObject;
		base.SetWeaponAni(player, spine);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		render_obj = base.gameObject;
	}

	public override void FireUpdate(PlayerController player, float deltaTime)
	{
		if (GameSceneController.Instance.main_camera != null)
		{
			GameSceneController.Instance.main_camera.ZoomOut(deltaTime);
		}
		base.FireUpdate(player, deltaTime);
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (weapon_data.OnFire())
		{
			base.Fire(player, deltaTime);
			if (!AnimationUtil.IsPlayingAnimation(player.gameObject, ANI_SHOOT))
			{
				AnimationUtil.Stop(player.gameObject);
				AnimationUtil.PlayAnimate(player.gameObject, ANI_SHOOT, WrapMode.Loop);
			}
		}
	}
}
