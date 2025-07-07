using System;
using CoMZ2;
using UnityEngine;

public class ShotGunController : WeaponController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	protected string ani_shoot01 = "W1200_UpperBody_Shooting01";

	protected string ani_shoot02 = "W1200_UpperBody_Shooting02";

	protected string ani_shoot01_down = "W1200_LowerBody_Shooting01";

	protected string ani_shoot02_down = "W1200_LowerBody_Shooting02";

	public GameObject fire_spark;

	protected GameObject hit_spark;

	protected GameObject fire_line;

	protected Transform bullet_eff_ori;

	private void Awake()
	{
		weapon_type = WeaponType.ShotGun;
	}

	private void Start()
	{
		fire_ori = base.transform.Find("fire_ori");
		if (fire_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find fire_ori!"));
		}
		bullet_eff_ori = base.transform.Find("bullet_eff_ori");
		if (bullet_eff_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find bullet_eff_ori!"));
		}
		fire_spark = UnityEngine.Object.Instantiate(Accessory[0]) as GameObject;
		fire_spark.transform.parent = fire_ori;
		fire_spark.transform.localPosition = Vector3.zero;
		fire_spark.transform.localRotation = Quaternion.identity;
		fire_spark.GetComponent<ParticleSystem>().Stop();
		hit_spark = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
		hit_spark.transform.position = Vector3.zero;
		hit_spark.transform.rotation = Quaternion.identity;
		hit_spark.GetComponent<ObjectPool>().Init("HitSpark", Accessory[1], 3, 0.6f);
		fire_line = UnityEngine.Object.Instantiate(Accessory[2]) as GameObject;
		fire_line.transform.localPosition = Vector3.zero;
		fire_line.transform.localRotation = Quaternion.identity;
		fire_line.GetComponent<ParticleSystem>().Stop();
		ready_fire_time = 0.3f;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_FIRE_READY = "W1200_UpperBody_FireReady";
		ANI_IDLE_RUN = "W1200_UpperBody_Run01";
		ANI_IDLE_SHOOT = "W1200_UpperBody_Idle01";
		ANI_RELOAD = "W1200_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "W1200_UpperBody_Shiftweapon01";
		ANI_IDLE_DOWN = "W1200_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "W1200_LowerBody_Shooting01";
		ANI_RELOAD_DOWN = "W1200_LowerBody_Reload01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ani_shoot01].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ani_shoot02].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		if (weapon_data.weapon_name == "Winchester1200")
		{
			render_obj = base.transform.Find("W1200").gameObject;
			ANI_SHOOT = ani_shoot01;
			ANI_SHOOT_DOWN = ani_shoot01_down;
		}
		else if (weapon_data.weapon_name == "Xm1014" || weapon_data.weapon_name == "XM12")
		{
			render_obj = base.gameObject;
			ANI_SHOOT = ani_shoot02;
			ANI_SHOOT_DOWN = ani_shoot02_down;
		}
		ResetReloadAniSpeed(player);
		ResetFireAniSpeed(player);
		base.SetWeaponAni(player, spine);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		if (weapon_data.weapon_name == "Winchester1200")
		{
			render_obj = base.transform.Find("W1200").gameObject;
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
			GameSceneController.Instance.main_camera.ZoomIn(deltaTime);
		}
		base.FireUpdate(player, deltaTime);
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (OnReadyFire(player, deltaTime) && weapon_data.EnableFire())
		{
			base.Fire(player, deltaTime);
			fireable = true;
			AnimationUtil.Stop(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState));
			AnimationUtil.PlayAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.ClampForever);
		}
	}

	public override void CheckHit(ObjectController controller)
	{
		if (!fireable)
		{
			return;
		}
		fireable = false;
		weapon_data.OnFire();
		fire_spark.GetComponent<ParticleSystem>().Play();
		fire_line.transform.position = fire_ori.transform.position;
		fire_line.transform.rotation = fire_ori.transform.rotation;
		fire_line.GetComponent<ParticleSystem>().Play();
		float num = Mathf.Tan((float)Math.PI / 3f);
		PlayerController playerController = controller as PlayerController;
		playerController.CouldEnterAfterFire = true;
		GameSceneController.Instance.SightBead.Stretch(weapon_data.config.recoil);
		bool flag = false;
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			Vector3 vector = playerController.transform.InverseTransformPoint(value.centroid);
			float magnitude = (value.centroid - playerController.centroid).magnitude;
			Vector3 normalized = (playerController.centroid - value.centroid).normalized;
			if (vector.z > 0f && Mathf.Abs(vector.z / vector.x) > num && !Physics.Raycast(playerController.centroid, (value.centroid - playerController.centroid).normalized, magnitude, (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD)) && magnitude < weapon_data.range_val)
			{
				value.OnHit(GetDamageValWithAvatar(playerController, playerController.avatar_data), this, controller, value.centroid, normalized);
				flag = true;
			}
		}
		if (flag)
		{
			playerController.AddComboValue(weapon_data.config.combo_base);
		}
		foreach (GameObject item in GameSceneController.Instance.wood_box_list)
		{
			WoodBoxController component = item.GetComponent<WoodBoxController>();
			if (!(component == null))
			{
				Vector3 vector2 = playerController.transform.InverseTransformPoint(component.centroid);
				float magnitude2 = (component.centroid - playerController.centroid).magnitude;
				Vector3 normalized2 = (playerController.centroid - component.centroid).normalized;
				if (vector2.z > 0f && Mathf.Abs(vector2.z / vector2.x) > num && !Physics.Raycast(fire_ori.position, (component.centroid - playerController.centroid).normalized, magnitude2, (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD)) && magnitude2 < weapon_data.range_val)
				{
					component.OnHit(GetDamageValWithAvatar(playerController, playerController.avatar_data), this, controller, component.centroid, normalized2);
					flag = true;
				}
			}
		}
		Vector3 sightScreenPos = GameSceneController.Instance.GetSightScreenPos();
		Vector2 fireOffset = GameSceneController.Instance.SightBead.GetFireOffset();
		Vector3 vector3 = Camera.main.ScreenToWorldPoint(new Vector3(sightScreenPos.x + fireOffset.x, sightScreenPos.y + fireOffset.y, 100f));
		Ray ray = new Ray(Camera.main.transform.position, vector3 - Camera.main.transform.position);
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(ray, out hitInfo, 10f, (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.DYNAMIC_SCENE) | (1 << PhysicsLayer.ANIMATION_SCENE) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD)) && hitInfo.collider != null)
		{
			hit_spark.GetComponent<ObjectPool>().CreateObject(hitInfo.point, Quaternion.identity);
		}
		GameSceneController.Instance.shotgun_cartridge_pool.GetComponent<ObjectPool>().CreateObject(bullet_eff_ori.position, bullet_eff_ori.rotation);
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 100f));
		Gizmos.color = Color.red;
		Gizmos.DrawRay(Camera.main.transform.position, vector - Camera.main.transform.position);
	}

	public override bool OnReadyFire(PlayerController player, float deltaTime)
	{
		if (ready_fire == FireReady.None)
		{
			fireable = false;
		}
		return base.OnReadyFire(player, deltaTime);
	}
}
