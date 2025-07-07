using CoMZ2;
using UnityEngine;

public class PistolController : WeaponController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	protected GameObject hit_spark;

	protected GameObject hit_metal;

	protected GameObject fire_spark_obj;

	protected GameObject hit_wood;

	protected GameObject obj_tem;

	private float explode_radius;

	private void Awake()
	{
		weapon_type = WeaponType.Pistol;
	}

	private void Start()
	{
		fire_ori = base.transform.Find("fire_ori");
		if (fire_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find fire_ori!"));
		}
		obj_tem = Object.Instantiate(Accessory[0]) as GameObject;
		fire_spark_obj = obj_tem.transform.Find("PistolRifleSpark").gameObject;
		fire_spark_obj.GetComponent<ParticleSystem>().Stop();
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
		explode_radius = (float)weapon_data.config.Ex_conf["explodeRange"];
	}

	public override void Update()
	{
		base.Update();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_IDLE_RUN = "44 Magnum_Upperbody_Run01";
		ANI_IDLE_SHOOT = "44 Magnum_Upperbody_Idle01";
		ANI_SHOOT = "44 Magnum_Upperbody_Shooting01";
		ANI_RELOAD = "W1200_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "44 Magnum_Upperbody_ShiftWeapon01";
		ANI_IDLE_DOWN = "44 Magnum_Lowerbody_Idle01";
		ANI_SHOOT_DOWN = "44 Magnum_Lowerbody_Shooting01";
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
		base.FireUpdate(player, deltaTime);
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (weapon_data.EnableFire())
		{
			base.Fire(player, deltaTime);
			fireable = true;
			obj_tem.transform.position = fire_ori.transform.position;
			obj_tem.transform.rotation = fire_ori.transform.rotation;
			obj_tem.transform.parent = player.transform;
			fire_spark_obj.GetComponent<ParticleSystem>().Play();
			AnimationUtil.Stop(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState));
			AnimationUtil.PlayAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.ClampForever);
			if (player.avatar_data.avatar_type == AvatarType.Cowboy)
			{
				AnimationUtil.Stop(player.CowboyCap);
				AnimationUtil.PlayAnimate(player.CowboyCap, "Magnum_Shooting01", WrapMode.Once);
			}
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
		PlayerController playerController = controller as PlayerController;
		playerController.CouldEnterAfterFire = true;
		Vector2 fireOffset = GameSceneController.Instance.SightBead.GetFireOffset();
		GameSceneController.Instance.SightBead.Stretch(weapon_data.config.recoil);
		Vector3 sightScreenPos = GameSceneController.Instance.GetSightScreenPos();
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(sightScreenPos.x + fireOffset.x, sightScreenPos.y + fireOffset.y, 100f));
		Ray ray = new Ray(Camera.main.transform.position, vector - Camera.main.transform.position);
		RaycastHit raycastHit = default(RaycastHit);
		RaycastHit[] array = Physics.RaycastAll(ray, 1000f, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.NPC) | (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.DYNAMIC_SCENE) | (1 << PhysicsLayer.ANIMATION_SCENE) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD) | (1 << PhysicsLayer.WOOD_BOX));
		float num = 1000000f;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit2 = array2[i];
			Vector3 zero = Vector3.zero;
			if (((raycastHit2.collider.gameObject.layer == PhysicsLayer.ENEMY) ? playerController.transform.InverseTransformPoint(raycastHit2.transform.position) : ((raycastHit2.collider.gameObject.layer != PhysicsLayer.PLAYER) ? playerController.transform.InverseTransformPoint(raycastHit2.point) : playerController.transform.InverseTransformPoint(raycastHit2.collider.transform.position))).z > 0f)
			{
				float sqrMagnitude = (raycastHit2.point - fire_ori.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					raycastHit = raycastHit2;
					num = sqrMagnitude;
				}
			}
		}
		if (raycastHit.collider != null)
		{
			GameSceneController.Instance.boom_s_pool.GetComponent<ObjectPool>().CreateObject(raycastHit.point, Quaternion.identity);
			bool flag = false;
			foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
			{
				if ((value.centroid - raycastHit.point).sqrMagnitude < explode_radius * explode_radius)
				{
					value.OnHit(GetDamageValWithAvatar(playerController, playerController.avatar_data), this, playerController, value.centroid, raycastHit.point - value.centroid);
					flag = true;
				}
			}
			foreach (GameObject item in GameSceneController.Instance.wood_box_list)
			{
				WoodBoxController component = item.GetComponent<WoodBoxController>();
				if (component != null && (component.centroid - raycastHit.point).sqrMagnitude < explode_radius * explode_radius)
				{
					flag = true;
					component.OnHit(GetDamageValWithAvatar(playerController, playerController.avatar_data), this, playerController, component.centroid, raycastHit.point - component.centroid);
				}
			}
			if (raycastHit.collider.gameObject.layer == PhysicsLayer.FLOOR || raycastHit.collider.gameObject.layer == PhysicsLayer.WALL)
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
				Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
				component2.mass = 5f;
				component2.drag = 1.5f;
				component2.angularDrag = 1.5f;
				component2.AddForceAtPosition(force, raycastHit.point, ForceMode.Impulse);
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
			if (flag)
			{
				playerController.AddComboValue(weapon_data.config.combo_base);
			}
		}
		if (playerController.transform.InverseTransformPoint(GameSceneController.Instance.main_camera.target.position).z > 1f)
		{
			GameObject gameObject3 = GameSceneController.Instance.fire_line_pool.GetComponent<ObjectPool>().CreateObject(fire_ori.position, Quaternion.identity);
			FireLineScript component3 = gameObject3.GetComponent<FireLineScript>();
			component3.Init(fire_ori.position, vector);
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 100f));
		Gizmos.color = Color.red;
		Gizmos.DrawRay(Camera.main.transform.position, vector - Camera.main.transform.position);
	}
}
