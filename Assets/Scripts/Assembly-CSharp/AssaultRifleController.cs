using CoMZ2;
using UnityEngine;

public class AssaultRifleController : WeaponController
{
	private Ray ray = default(Ray);

	protected Transform fire_ori;

	protected GameObject fire_spark;

	protected GameObject hit_spark;

	protected GameObject hit_metal;

	protected GameObject hit_wood;

	protected Transform bullet_eff_ori;

	private void Awake()
	{
		weapon_type = WeaponType.AssaultRifle;
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
		fire_spark = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
		fire_spark.transform.parent = fire_ori;
		fire_spark.transform.localPosition = Vector3.zero;
		fire_spark.transform.localRotation = Quaternion.identity;
		fire_spark.GetComponent<ObjectPool>().Init("FireSpark", Accessory[0], 3, 0.1f);
		hit_spark = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
		hit_spark.transform.position = Vector3.zero;
		hit_spark.transform.rotation = Quaternion.identity;
		hit_spark.GetComponent<ObjectPool>().Init("HitSpark", Accessory[1], 3, 0.9f);
		hit_metal = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
		hit_metal.transform.position = Vector3.zero;
		hit_metal.transform.rotation = Quaternion.identity;
		hit_metal.GetComponent<ObjectPool>().Init("HitMetal", Accessory[2], 3, 0.9f);
		hit_wood = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
		hit_wood.transform.position = Vector3.zero;
		hit_wood.transform.rotation = Quaternion.identity;
		hit_wood.GetComponent<ObjectPool>().Init("HitWood", Accessory[3], 3, 0.9f);
	}

	public override void Update()
	{
		base.Update();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		if (weapon_data.weapon_name == "AK47" || weapon_data.weapon_name == "M16" || weapon_data.weapon_name == "FN2000")
		{
			ANI_IDLE_RUN = "AK_UpperBody_Run01";
			ANI_IDLE_SHOOT = "AK_UpperBody_Idle01";
			ANI_SHOOT = "AK_UpperBody_Shooting01";
			ANI_RELOAD = "AK_UpperBody_Reload01";
			ANI_SHIFT_WEAPON = "AK_UpperBody_Shiftweapon01";
			ANI_IDLE_DOWN = "AK_LowerBody_Idle01";
			ANI_SHOOT_DOWN = "AK_LowerBody_Shooting01";
			ANI_RELOAD_DOWN = "AK_LowerBody_Reload01";
			ANI_FIRE_READY = "AK_UpperBody_FireReady";
		}
		else if (weapon_data.weapon_name == "MP5" || weapon_data.weapon_name == "Uzi" || weapon_data.weapon_name == "P90")
		{
			ANI_IDLE_RUN = "MP5_UpperBody_Run01";
			ANI_IDLE_SHOOT = "MP5_UpperBody_Idle01";
			ANI_SHOOT = "MP5_UpperBody_Shooting01";
			ANI_RELOAD = "MP5_UpperBody_Reload01";
			ANI_SHIFT_WEAPON = "MP5_UpperBody_Shiftweapon01";
			ANI_IDLE_DOWN = "MP5_LowerBody_Idle01";
			ANI_SHOOT_DOWN = "MP5_LowerBody_Shooting01";
			ANI_RELOAD_DOWN = "MP5_LowerBody_Reload01";
			ANI_FIRE_READY = "MP5_UpperBody_FireReady";
		}
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		ResetReloadAniSpeed(player);
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
			GameSceneController.Instance.main_camera.ZoomIn(deltaTime);
		}
		base.FireUpdate(player, deltaTime);
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (!OnReadyFire(player, deltaTime) || !weapon_data.OnFire())
		{
			return;
		}
		base.Fire(player, deltaTime);
		AnimationUtil.Stop(player.gameObject, ANI_SHOOT);
		AnimationUtil.PlayAnimate(player.gameObject, ANI_SHOOT, WrapMode.ClampForever);
		player.CouldEnterAfterFire = true;
		fire_spark.GetComponent<ObjectPool>().CreateObject();
		Vector2 fireOffset = GameSceneController.Instance.SightBead.GetFireOffset();
		GameSceneController.Instance.SightBead.Stretch(weapon_data.config.recoil);
		Vector3 sightScreenPos = GameSceneController.Instance.GetSightScreenPos();
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(sightScreenPos.x + fireOffset.x, sightScreenPos.y + fireOffset.y, 100f));
		ray = new Ray(Camera.main.transform.position, vector - Camera.main.transform.position);
		RaycastHit raycastHit = default(RaycastHit);
		RaycastHit[] array = Physics.RaycastAll(ray, 1000f, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.DYNAMIC_SCENE) | (1 << PhysicsLayer.ANIMATION_SCENE) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD) | (1 << PhysicsLayer.WOOD_BOX));
		float num = 1000000f;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit2 = array2[i];
			Vector3 zero = Vector3.zero;
			if (((raycastHit2.collider.gameObject.layer == PhysicsLayer.ENEMY) ? player.transform.InverseTransformPoint(raycastHit2.transform.position) : ((raycastHit2.collider.gameObject.layer != PhysicsLayer.PLAYER) ? player.transform.InverseTransformPoint(raycastHit2.point) : player.transform.InverseTransformPoint(raycastHit2.collider.transform.position))).z > 0f)
			{
				float sqrMagnitude = (raycastHit2.point - player.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					raycastHit = raycastHit2;
					num = sqrMagnitude;
				}
			}
		}
		if (raycastHit.collider != null)
		{
			if (raycastHit.collider.gameObject.layer == PhysicsLayer.ENEMY)
			{
				EnemyController component = raycastHit.collider.GetComponent<EnemyController>();
				if (component != null)
				{
					component.OnHit(GetDamageValWithAvatar(player, player.avatar_data), this, player, raycastHit.point, raycastHit.normal);
					player.AddComboValue(weapon_data.config.combo_base);
				}
			}
			else if (raycastHit.collider.gameObject.layer == PhysicsLayer.WOOD_BOX)
			{
				hit_wood.GetComponent<ObjectPool>().CreateObject(raycastHit.point, Quaternion.identity);
				WoodBoxController component2 = raycastHit.collider.GetComponent<WoodBoxController>();
				component2.OnHit(GetDamageValWithAvatar(player, player.avatar_data), this, player, raycastHit.point, raycastHit.normal);
			}
			else if (raycastHit.collider.gameObject.layer == PhysicsLayer.FLOOR || raycastHit.collider.gameObject.layer == PhysicsLayer.WALL)
			{
				hit_spark.GetComponent<ObjectPool>().CreateObject(raycastHit.point, Quaternion.identity);
			}
			else if (raycastHit.collider.gameObject.layer == PhysicsLayer.WALL_METAL)
			{
				hit_metal.GetComponent<ObjectPool>().CreateObject(raycastHit.point, Quaternion.identity);
			}
			else if (raycastHit.collider.gameObject.layer == PhysicsLayer.WALL_WOOD)
			{
				hit_wood.GetComponent<ObjectPool>().CreateObject(raycastHit.point, Quaternion.identity);
			}
			else if (raycastHit.collider.gameObject.layer == PhysicsLayer.DYNAMIC_SCENE)
			{
				GameObject gameObject = raycastHit.collider.gameObject;
				if (gameObject.GetComponent<Rigidbody>() == null)
				{
					gameObject.AddComponent<Rigidbody>();
				}
				Vector3 force = (vector - Camera.main.transform.position).normalized * 50f;
				Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
				component3.mass = 5f;
				component3.drag = 1.5f;
				component3.angularDrag = 1.5f;
				component3.AddForceAtPosition(force, raycastHit.point, ForceMode.Impulse);
				gameObject.layer = PhysicsLayer.WALL;
				gameObject.AddComponent<RemoveTimerScript>().life = 3f;
			}
			else if (raycastHit.collider.gameObject.layer == PhysicsLayer.ANIMATION_SCENE)
			{
				GameObject gameObject2 = raycastHit.collider.transform.parent.gameObject;
				if (gameObject2.GetComponent<Animation>() != null)
				{
					gameObject2.GetComponent<Animation>()["Take 001"].clip.wrapMode = WrapMode.Once;
					gameObject2.GetComponent<Animation>().Play("Take 001");
				}
			}
		}
		if (fire_ori.transform.InverseTransformPoint(GameSceneController.Instance.main_camera.target.position).z > 1f)
		{
			GameObject gameObject3 = GameSceneController.Instance.fire_line_pool.GetComponent<ObjectPool>().CreateObject(fire_ori.position, Quaternion.identity);
			FireLineScript component4 = gameObject3.GetComponent<FireLineScript>();
			component4.Init(fire_ori.position, vector);
		}
		GameSceneController.Instance.assault_cartridge_pool.GetComponent<ObjectPool>().CreateObject(bullet_eff_ori.position, bullet_eff_ori.rotation);
	}

	public override void OnWeaponReload()
	{
		fire_spark.GetComponent<ObjectPool>().AutoDestructAll();
	}
}
