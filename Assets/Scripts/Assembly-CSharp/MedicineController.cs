using UnityEngine;

public class MedicineController : WeaponController
{
	protected bool fireable;

	private void Awake()
	{
		weapon_type = WeaponType.Medicine;
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
		ANI_IDLE_RUN = "Prop_UpperBody_Run01";
		ANI_IDLE_SHOOT = "Prop_UpperBody_Idle01";
		ANI_SHOOT = "Prop_UpperBody_Attack01";
		ANI_RELOAD = "W1200_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "Prop_UpperBody_ShiftWeapon01";
		ANI_IDLE_DOWN = "Prop_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "Prop_LowerBody_Attack01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		ResetFireAniSpeed(player);
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
		if (CouldMakeNextShoot())
		{
			Fire(player, deltaTime);
		}
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (weapon_data.EnableFire())
		{
			fire_interval = 0f;
			fireable = true;
			render_obj.GetComponent<Renderer>().enabled = true;
			AnimationUtil.Stop(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState));
			AnimationUtil.PlayAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.Once);
		}
	}

	public override void CheckHit(ObjectController controller)
	{
		if (fireable)
		{
			fireable = false;
			weapon_data.OnFire();
			render_obj.GetComponent<Renderer>().enabled = false;
			PlayerController playerController = controller as PlayerController;
			Vector3 position = base.transform.position;
			position = playerController.transform.position + Vector3.up;
			GameObject gameObject = Object.Instantiate(Accessory[0]) as GameObject;
			gameObject.transform.position = position;
			MedicineProjectile component = gameObject.GetComponent<MedicineProjectile>();
			component.explode_radius = (float)weapon_data.config.Ex_conf["healRange"];
			component.damage = weapon_data.damage_val;
			component.object_controller = controller;
			component.MedicineBoom();
			Vector3 force = (playerController.transform.up + playerController.transform.forward + playerController.transform.right * 0.3f).normalized * 7f;
			gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 100f));
		Gizmos.color = Color.red;
		Gizmos.DrawRay(Camera.main.transform.position, vector - Camera.main.transform.position);
	}
}
