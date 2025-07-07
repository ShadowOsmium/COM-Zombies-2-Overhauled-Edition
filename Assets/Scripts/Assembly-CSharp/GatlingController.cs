using CoMZ2;
using UnityEngine;

public class GatlingController : WeaponController
{
	private Ray ray = default(Ray);

	protected Transform fire_ori;

	protected GameObject fire_spark;

	protected GameObject hit_spark;

	protected GameObject hit_metal;

	protected GameObject hit_wood;

	protected string ANI_SHOOT_AFTER = string.Empty;

	protected string ani_shoot_down_a = "Rifle_Gatlin_LowerBody_Shooting01_a";

	protected string ani_shoot_down_b = "Rifle_Gatlin_LowerBody_Shooting01_b";

	protected string ani_shoot_down_c = "Rifle_Gatlin_LowerBody_Shooting01_c";

	private float last_combo_ratio;

	private void Awake()
	{
		weapon_type = WeaponType.Gatling;
	}

	private void Start()
	{
		fire_ori = base.transform.Find("Root/Rotate/fire_ori");
		if (fire_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find fire_ori!"));
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

	public override void Reset(Transform trans)
	{
		base.transform.position = trans.position + ori_pos;
		base.transform.parent = trans;
		base.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		GunOff();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_IDLE_RUN = "Rifle_Gatlin_UpperBody_Run01";
		ANI_IDLE_SHOOT = "Rifle_Gatlin_UpperBody_Idle01";
		ANI_SHOOT = "Rifle_Gatlin_UpperBody_Shooting01_b";
		ANI_RELOAD = "Rifle_Gatlin_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "Rifle_Gatlin_UpperBody_Shiftweapon01";
		ANI_SHOOT_AFTER = "Rifle_Gatlin_UpperBody_Shooting01_c";
		ANI_FIRE_READY = "Rifle_Gatlin_UpperBody_Shooting01_a";
		ANI_IDLE_DOWN = "Rifle_Gatlin_LowerBody_Idle01";
		ANI_SHOOT_DOWN = ani_shoot_down_b;
		ANI_RELOAD_DOWN = "Rifle_Gatlin_LowerBody_Reload01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_FIRE_READY].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT_AFTER].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ani_shoot_down_a].layer = -1;
		player.GetComponent<Animation>()[ani_shoot_down_b].layer = -1;
		player.GetComponent<Animation>()[ani_shoot_down_c].layer = -1;
		ResetReloadAniSpeed(player);
		ResetFireAniSpeed(player);
		render_obj = base.transform.Find("Rifle_Gatlin").gameObject;
		base.SetWeaponAni(player, spine);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		render_obj = base.transform.Find("Rifle_Gatlin").gameObject;
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
		if (!OnReadyFire(player, deltaTime))
		{
			return;
		}
		if (!weapon_data.OnFire())
		{
			AnimationUtil.CrossAnimate(base.gameObject, "Attack01_c", WrapMode.ClampForever);
			player.StopPlayerAudio("ShotGatlin01ServoLoop");
			player.StopPlayerAudio("ShotGatlin01");
			player.StopPlayerAudio("ShotGatlin02");
			player.PlayPlayerAudio("ShotGatlin01Release");
			last_combo_ratio = 0f;
			return;
		}
		if (last_combo_ratio != player.ComboRateRatio)
		{
			last_combo_ratio = player.ComboRateRatio;
			if (last_combo_ratio >= 1.1f)
			{
				player.StopPlayerAudio("ShotGatlin01");
				player.PlayPlayerAudio("ShotGatlin02");
				player.PlayPlayerAudio("ShotGatlin01ServoLoop");
			}
			else
			{
				player.StopPlayerAudio("ShotGatlin02");
				player.PlayPlayerAudio("ShotGatlin01");
				player.PlayPlayerAudio("ShotGatlin01ServoLoop");
			}
		}
		base.Fire(player, deltaTime);
		AnimationUtil.Stop(player.gameObject, ANI_SHOOT);
		AnimationUtil.PlayAnimate(player.gameObject, ANI_SHOOT, WrapMode.ClampForever);
		if (!AnimationUtil.IsPlayingAnimation(base.gameObject, "Attack01_b"))
		{
			AnimationUtil.PlayAnimate(base.gameObject, "Attack01_b", WrapMode.Loop);
			ANI_SHOOT_DOWN = ani_shoot_down_b;
			player.GetMoveIdleAnimation(player.FireState);
		}
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
	}

	public override void OnWeaponReload()
	{
		fire_spark.GetComponent<ObjectPool>().AutoDestructAll();
	}

	public override bool OnReadyFire(PlayerController player, float deltaTime)
	{
		if (ready_fire == FireReady.None)
		{
			ready_fire_time = base.gameObject.GetComponent<Animation>()["Attack01_a"].clip.length;
			AnimationUtil.CrossAnimate(player.gameObject, ANI_FIRE_READY, WrapMode.ClampForever, ready_fire_time);
			AnimationUtil.CrossAnimate(base.gameObject, "Attack01_a", WrapMode.ClampForever);
			ANI_SHOOT_DOWN = ani_shoot_down_a;
			player.GetMoveIdleAnimation(player.FireState);
			cur_ready_fire_time = 0f;
			ready_fire = FireReady.Readying;
			return false;
		}
		if (ready_fire == FireReady.Readying)
		{
			cur_ready_fire_time += deltaTime;
			if (!(cur_ready_fire_time >= ready_fire_time))
			{
				return false;
			}
			ready_fire = FireReady.Finished;
		}
		return true;
	}

	public override void OnEnterAfterFire(PlayerController player)
	{
		AnimationUtil.CrossAnimate(base.gameObject, "Attack01_c", WrapMode.ClampForever);
		AnimationUtil.CrossAnimate(player.gameObject, ANI_SHOOT_AFTER, WrapMode.ClampForever);
		player.StopPlayerAudio("ShotGatlin01ServoLoop");
		player.StopPlayerAudio("ShotGatlin01");
		player.StopPlayerAudio("ShotGatlin02");
		player.PlayPlayerAudio("ShotGatlin01Release");
		last_combo_ratio = 0f;
		ANI_SHOOT_DOWN = ani_shoot_down_c;
		player.ANI_MOVE_IDLE = ANI_SHOOT_DOWN;
	}

	public override float GetAfterFireTime()
	{
		return base.gameObject.GetComponent<Animation>()["Attack01_c"].clip.length;
	}

	public override void StopFire(PlayerController player)
	{
		if (GameSceneController.Instance != null && GameSceneController.Instance.player_controller != null)
		{
			player.StopPlayerAudio("ShotGatlin01ServoLoop");
			player.StopPlayerAudio("ShotGatlin01");
			player.StopPlayerAudio("ShotGatlin02");
			player.PlayPlayerAudio("ShotGatlin01Release");
			AnimationUtil.Stop(base.gameObject);
			last_combo_ratio = 0f;
		}
	}
}
