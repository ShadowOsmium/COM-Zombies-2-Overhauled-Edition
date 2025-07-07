using CoMZ2;
using UnityEngine;

public class ShieldController : WeaponController
{
	private string ani_shield_run = "Shield_UpperBody_Run01";

	private string ani_shield_stay = "Shidle_Idle01";

	private void Awake()
	{
		weapon_type = WeaponType.Shield;
	}

	private void Start()
	{
	}

	public override void Update()
	{
		base.Update();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_IDLE_RUN = ani_shield_stay;
		ANI_IDLE_SHOOT = ani_shield_stay;
		ANI_SHOOT = ani_shield_run;
		ANI_RELOAD = ani_shield_stay;
		ANI_SHIFT_WEAPON = "Shield_UpperBody_ShiftWeapon01";
		ANI_IDLE_DOWN = "Shidle_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "Shidle_LowerBody_Idle01";
		ANI_RELOAD_DOWN = "Shidle_LowerBody_Idle01";
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
		Fire(player, deltaTime);
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (player.MoveState == player.IDLE_STATE)
		{
			ANI_SHOOT = ani_shield_stay;
		}
		else
		{
			ANI_SHOOT = ani_shield_run;
		}
		if (!AnimationUtil.IsPlayingAnimation(player.gameObject, ANI_SHOOT))
		{
			AnimationUtil.CrossAnimate(player.gameObject, ANI_SHOOT, WrapMode.Loop);
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 100f));
		Gizmos.color = Color.red;
		Gizmos.DrawRay(Camera.main.transform.position, vector - Camera.main.transform.position);
	}

	public override void OnWeaponReload()
	{
	}

	public float OnDefense(float damage)
	{
		float result = 0f;
		if (weapon_data.clip_bullet_count - (int)damage > 0)
		{
			weapon_data.clip_bullet_count -= (int)damage;
			weapon_data.total_bullet_count -= (int)damage;
		}
		else
		{
			result = Mathf.Abs((float)weapon_data.clip_bullet_count - damage);
			weapon_data.clip_bullet_count = 0;
			weapon_data.total_bullet_count = 0;
			GameSceneController.Instance.player_controller.OnSwatShieldBreak();
		}
		GameSceneController.Instance.player_controller.UpdateWeaponUIShow();
		return result;
	}

	public bool EnableDefense()
	{
		if (weapon_data.clip_bullet_count > 0)
		{
			return true;
		}
		return false;
	}

	public override string GetFireStateAnimation(PlayerStateType run_type, PlayerStateType fire_type)
	{
		string result = string.Empty;
		switch (fire_type)
		{
		case PlayerStateType.IdleShoot:
			result = ((run_type != PlayerStateType.Run) ? ANI_IDLE_SHOOT : ANI_SHOOT);
			break;
		case PlayerStateType.Shoot:
			result = ANI_SHOOT;
			break;
		case PlayerStateType.Reload:
			result = ANI_RELOAD;
			break;
		case PlayerStateType.GotHit:
			result = ANI_INJURED;
			break;
		case PlayerStateType.Dead:
			result = ANI_DEATH;
			break;
		case PlayerStateType.SwitchWeapon:
			result = ANI_SHIFT_WEAPON;
			break;
		}
		return result;
	}
}
