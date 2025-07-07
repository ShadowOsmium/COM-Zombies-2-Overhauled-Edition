using UnityEngine;

public class MinesController : WeaponController
{
	protected bool fireable;

	private void Awake()
	{
		weapon_type = WeaponType.Mines;
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
		ANI_IDLE_RUN = "W1200_UpperBody_Run01";
		ANI_IDLE_SHOOT = "W1200_UpperBody_Idle01";
		ANI_SHOOT = "Mines_UpperBody_Attack01";
		ANI_RELOAD = "W1200_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "W1200_UpperBody_Shiftweapon01";
		ANI_SHOOT_DOWN = "Mines_LowerBody_Attack01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		ResetFireAniSpeed(player);
		render_obj = base.transform.Find("Weapon_Mine").gameObject;
		base.SetWeaponAni(player, spine);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		render_obj = base.transform.Find("Weapon_Mine").gameObject;
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
			base.Fire(player, deltaTime);
			fireable = true;
			render_obj.GetComponent<Renderer>().enabled = true;
			AnimationUtil.Stop(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState));
			AnimationUtil.PlayAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.Once);
			AnimationUtil.PlayAnimate(base.gameObject, "Attack01", WrapMode.Once);
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
			MineProjectile component = gameObject.GetComponent<MineProjectile>();
			component.explode_radius = 5f;
			component.damage = weapon_data.damage_val;
			component.object_controller = controller;
			Vector3 force = (new Vector3(base.transform.position.x, playerController.transform.position.y, base.transform.position.z) - playerController.transform.position).normalized * 3f;
			gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
			GameSceneController.Instance.mine_area.Add(component);
		}
	}
}
