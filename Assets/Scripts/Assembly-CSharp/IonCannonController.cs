using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class IonCannonController : WeaponController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	public GameObject fire_smoke_obj;

	private float explode_radius;

	private TNetObject tnetObj;

	protected List<GameObject> rend_obj_list = new List<GameObject>();

	private void Awake()
	{
		weapon_type = WeaponType.IonCannon;
	}

	private void Start()
	{
		fire_ori = base.transform.Find("fire_ori");
		if (fire_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find fire_ori!"));
		}
		fire_smoke_obj = Object.Instantiate(Accessory[1]) as GameObject;
		fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
		fire_smoke_obj.transform.parent = fire_ori;
		fire_smoke_obj.transform.localPosition = Vector3.zero;
		fire_smoke_obj.transform.localRotation = Quaternion.identity;
		explode_radius = (float)weapon_data.config.Ex_conf["explodeRange"];
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		//{
		//	tnetObj = TNetConnection.Connection;
		//}
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

	public override void ResetFireInterval()
	{
		base.ResetFireInterval();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		if (weapon_data.weapon_name == "IonCannon")
		{
			ANI_IDLE_RUN = "RPG_UpperBody_Run01";
			ANI_IDLE_SHOOT = "RPG_UpperBody_Idle01";
			ANI_SHOOT = "IonCannon_UpperBody_Shooting01";
			ANI_RELOAD = "IonCannon_UpperBody_Reload01";
			ANI_SHIFT_WEAPON = "RPG_UpperBody_Shiftweapon01";
			ANI_IDLE_DOWN = "RPG_LowerBody_Idle01";
			ANI_SHOOT_DOWN = "RPG_LowerBody_Shooting01";
			ANI_RELOAD_DOWN = "RPG_LowerBody_Reload01";
		}
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		ResetReloadAniSpeed(player);
		ResetFireAniSpeed(player);
		base.SetWeaponAni(player, spine);
		rend_obj_list.Add(base.transform.Find("Weapon_IonCannon_01").gameObject);
		rend_obj_list.Add(base.transform.Find("Weapon_IonCannon_02").gameObject);
		rend_obj_list.Add(base.transform.Find("Weapon_IonCannon_03").gameObject);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		rend_obj_list.Add(base.transform.Find("Weapon_IonCannon_01").gameObject);
		rend_obj_list.Add(base.transform.Find("Weapon_IonCannon_02").gameObject);
		rend_obj_list.Add(base.transform.Find("Weapon_IonCannon_03").gameObject);
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
		if (weapon_data.EnableFire())
		{
			base.Fire(player, deltaTime);
			fireable = true;
			AnimationUtil.Stop(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState));
			AnimationUtil.PlayAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.ClampForever);
			fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
			fire_smoke_obj.GetComponent<ParticleSystem>().Play();
			if (player.avatar_data.avatar_type == AvatarType.Cowboy)
			{
				AnimationUtil.Stop(player.CowboyCap);
				AnimationUtil.PlayAnimate(player.CowboyCap, "RPG_Shooting01", WrapMode.Once);
			}
		}
	}

	public override void CheckHit(ObjectController controller)
	{
		if (fireable)
		{
			fireable = false;
			weapon_data.OnFire();
			GameSceneController.Instance.SightBead.Stretch(weapon_data.config.recoil);
			Vector3 sightScreenPos = GameSceneController.Instance.GetSightScreenPos();
			Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(sightScreenPos.x, sightScreenPos.y, 100f));
			Vector3 vector2 = vector;
			RaycastHit hitInfo;
			if (Physics.Raycast(Camera.main.transform.position, (vector - Camera.main.transform.position).normalized, out hitInfo, 100f, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.NPC) | (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.DYNAMIC_SCENE) | (1 << PhysicsLayer.ANIMATION_SCENE) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD) | (1 << PhysicsLayer.WOOD_BOX)))
			{
				vector2 = hitInfo.point;
			}
			Vector3 vector3 = (vector2 - fire_ori.position).normalized;
			if (fire_ori.InverseTransformPoint(vector2).z <= 0f)
			{
				vector3 = fire_ori.forward;
			}
			GameObject gameObject = Object.Instantiate(Accessory[0], fire_ori.position, Quaternion.LookRotation(vector3)) as GameObject;
			IonProjectile component = gameObject.GetComponent<IonProjectile>();
			component.launch_dir = vector3;
			component.fly_speed = 30f;
			component.explode_radius = explode_radius;
			component.life = 3f;
			component.damage = weapon_data.damage_val;
			component.object_controller = controller;
			component.weapon_controller = this;
			//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
			//{
			//	SFSArray sFSArray = new SFSArray();
			//	sFSArray.AddFloat(vector2.x);
			//	sFSArray.AddFloat(vector2.y);
			//	sFSArray.AddFloat(vector2.z);
			//	SFSObject sFSObject = new SFSObject();
			//	sFSObject.PutSFSArray("ionFire", sFSArray);
			//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
			//}
		}
	}

	public override void GunOn()
	{
		foreach (GameObject item in rend_obj_list)
		{
			item.GetComponent<Renderer>().enabled = true;
		}
		OnWeaponEquip();
	}

	public override void GunOff()
	{
		foreach (GameObject item in rend_obj_list)
		{
			item.GetComponent<Renderer>().enabled = false;
		}
	}
}
