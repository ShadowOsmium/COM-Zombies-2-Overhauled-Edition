using CoMZ2;
using UnityEngine;

public class ChaisawController : WeaponController
{
	protected string ani_shoot1 = "Electric_saw_UpperBody_Attack01";

	protected string ani_shoot2 = "Electric_saw_UpperBody_Attack02";

	protected string ani_shoot1_down = "Electric_saw_LowerBody_Attack01";

	protected string ani_shoot2_down = "Electric_saw_LowerBody_Attack02";

	protected GameObject chaisaw_blood;

	private void Awake()
	{
		weapon_type = WeaponType.Saw;
		chaisaw_blood = base.transform.Find("ChainsawBlood").gameObject;
		chaisaw_blood.SetActive(false);
	}

	private void Start()
	{
		AnimationUtil.PlayAnimate(base.gameObject, "cycle", WrapMode.Loop);
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Reset(Transform trans)
	{
		base.transform.position = trans.position + ori_pos;
		base.transform.parent = trans;
		base.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		GunOff();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_IDLE_RUN = "Electric_saw_UpperBody_Run01";
		ANI_IDLE_SHOOT = "Electric_saw_UpperBody_Idle01";
		ANI_SHOOT = "Electric_saw_UpperBody_Attack02";
		ANI_RELOAD = "AK_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "Electric_saw_UpperBody_ShiftWeapon01";
		ANI_IDLE_DOWN = "Electric_saw_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "Electric_saw_LowerBody_Attack01";
		ANI_SWITCH_DOWN = "Electric_saw_LowerBody_ShiftWeapon01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ani_shoot1].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ani_shoot2].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ani_shoot1_down].layer = -1;
		player.GetComponent<Animation>()[ani_shoot2_down].layer = -1;
		if (weapon_data.weapon_name == "Chainsaw")
		{
			render_obj = base.transform.Find("Electric_saw_Large").gameObject;
		}
		else
		{
			render_obj = base.gameObject;
		}
		ResetFireAniSpeed(player);
		base.SetWeaponAni(player, spine);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		if (weapon_data.weapon_name == "Chainsaw")
		{
			render_obj = base.transform.Find("Saw_Root").gameObject;
		}
		else
		{
			render_obj = base.gameObject;
		}
	}

	public override void FireUpdate(PlayerController player, float deltaTime)
	{
		if (GameSceneController.Instance.main_camera != null)
		{
			GameSceneController.Instance.main_camera.ZoomOut(deltaTime);
		}
		if (weapon_data.EnableFire() && (!AnimationUtil.IsPlayingAnimation(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState)) || AnimationUtil.AnimationEnds(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState))))
		{
			RandomAttackAnimation();
			player.GetMoveIdleAnimation(player.FireState);
			AnimationUtil.CrossAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.Once);
		}
	}

	public void RandomAttackAnimation()
	{
		if (ANI_SHOOT == ani_shoot1)
		{
			ANI_SHOOT = ani_shoot2;
			ANI_SHOOT_DOWN = ani_shoot2_down;
		}
		else
		{
			ANI_SHOOT = ani_shoot1;
			ANI_SHOOT_DOWN = ani_shoot1_down;
		}
	}

	public override void ResetFireAniSpeed(GameObject player)
	{
		if (base.FireRate < player.GetComponent<Animation>()[ani_shoot1].length)
		{
			player.GetComponent<Animation>()[ani_shoot1].speed = player.GetComponent<Animation>()[ani_shoot1].length / base.FireRate * 1f;
		}
		if (base.FireRate < player.GetComponent<Animation>()[ani_shoot2].length)
		{
			player.GetComponent<Animation>()[ani_shoot2].speed = player.GetComponent<Animation>()[ani_shoot2].length / base.FireRate * 1f;
		}
	}

	public override void CheckHit(ObjectController controller)
	{
		PlayerController playerController = controller as PlayerController;
		playerController.chaisaw_target_enemy = null;
		bool flag = false;
		Vector3 down = Vector3.down;
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			Collider collider = value.gameObject.GetComponent<Collider>();
			if (collider != null && base.GetComponent<Collider>().bounds.Intersects(collider.bounds))
			{
				down = playerController.centroid - value.centroid;
				if (value.OnChaisawSpecialHit(value.enemy_data.hp_capacity, this, controller, value.centroid, down))
				{
					flag = true;
					playerController.chaisaw_target_enemy = value;
					break;
				}
				flag = true;
				value.OnHit(GetDamageValWithAvatar(playerController, playerController.avatar_data), this, controller, value.centroid, down);
			}
		}
		foreach (GameObject item in GameSceneController.Instance.wood_box_list)
		{
			WoodBoxController component = item.GetComponent<WoodBoxController>();
			Collider collider2 = component.GetComponent<Collider>();
			if (component != null && collider2 != null && base.GetComponent<Collider>().bounds.Intersects(collider2.bounds))
			{
				flag = true;
				component.OnHit(GetDamageValWithAvatar(playerController, playerController.avatar_data), this, controller, component.centroid, Vector3.up);
			}
		}
		if (!flag)
		{
			return;
		}
		weapon_data.OnFire();
		playerController.AddComboValue(weapon_data.config.combo_base);
		if (playerController.chaisaw_target_enemy != null)
		{
			EnableChaisawBlood(true);
			playerController.SetFireState(playerController.CHAISAW_SKILL_STATE);
			Ray ray = new Ray(playerController.chaisaw_pos.position, Vector3.down);
			float num = 10000f;
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 100f, (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD)))
			{
				num = hitInfo.point.y;
				playerController.chaisaw_target_enemy.transform.position = new Vector3(playerController.chaisaw_pos.position.x, num + 0.1f, playerController.chaisaw_pos.position.z);
				playerController.chaisaw_target_enemy.transform.LookAt(playerController.transform);
			}
		}
	}

	public void EnableChaisawBlood(bool status)
	{
		chaisaw_blood.SetActive(status);
	}
}
