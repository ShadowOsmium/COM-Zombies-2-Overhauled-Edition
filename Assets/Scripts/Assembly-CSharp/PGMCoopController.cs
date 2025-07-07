using System.Collections.Generic;
using UnityEngine;

public class PGMCoopController : WeaponCoopController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	public GameObject fire_line_obj;

	public GameObject fire_smoke_obj;

	private float cur_auto_lock_time;

	private float auto_lock_time_interval = 0.4f;

	protected GameObject pgm_sight;

	public Dictionary<NearestTargetInfo, GameObject> auto_lock_target_dir = new Dictionary<NearestTargetInfo, GameObject>();

	protected bool is_inited;

	private float explode_radius;

	private Vector3 fire_target = Vector3.zero;

	private void Awake()
	{
		weapon_type = WeaponType.PGM;
	}

	private void Start()
	{
		fire_ori = base.transform.Find("fire_ori");
		if (fire_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find fire_ori!"));
		}
		fire_line_obj = Object.Instantiate(Accessory[1]) as GameObject;
		fire_line_obj.transform.parent = fire_ori;
		fire_line_obj.transform.localPosition = Vector3.zero;
		fire_line_obj.transform.localRotation = Quaternion.identity;
		fire_line_obj.GetComponent<ParticleSystem>().Stop();
		fire_smoke_obj = Object.Instantiate(Accessory[2]) as GameObject;
		fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
		explode_radius = (float)weapon_data.config.Ex_conf["explodeRange"];
		pgm_sight = Accessory[3];
		is_inited = true;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void ResetFireInterval()
	{
		base.ResetFireInterval();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_IDLE_RUN = "RPG_UpperBody_Run01";
		ANI_IDLE_SHOOT = "RPG_UpperBody_Idle01";
		ANI_SHOOT = "RPG_UpperBody_Shooting01";
		ANI_RELOAD = "RPG_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "RPG_UpperBody_Shiftweapon01";
		ANI_IDLE_DOWN = "RPG_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "RPG_LowerBody_Shooting01";
		ANI_RELOAD_DOWN = "RPG_LowerBody_Reload01";
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
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (is_inited)
		{
			base.Fire(player, deltaTime);
			AnimationUtil.Stop(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState));
			AnimationUtil.PlayAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.ClampForever);
			/*if (player.avatar_data.avatar_type == AvatarType.Cowboy)
			{
				AnimationUtil.Stop(player.CowboyCap);
				AnimationUtil.PlayAnimate(player.CowboyCap, "RPG_Shooting01", WrapMode.Once);
			}*/
		}
	}

	public override void CheckHit(ObjectController controller)
	{
	}

	public override void GunOn()
	{
		base.GunOn();
	}

	public override void GunOff()
	{
		base.GunOff();
	}

	public override void OnFireRelease(PlayerController player)
	{
	}

	public void OnRemoteFire(ObjectController controller, List<Vector3> fire_data)
	{
		if (!is_inited)
		{
			return;
		}
		PlayerController player = controller as PlayerController;
		Fire(player, 0.03f);
		fire_line_obj.GetComponent<ParticleSystem>().Stop();
		fire_line_obj.GetComponent<ParticleSystem>().Play();
		fire_smoke_obj.transform.position = base.transform.position;
		fire_smoke_obj.transform.rotation = base.transform.rotation;
		fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
		fire_smoke_obj.GetComponent<ParticleSystem>().Play();
		foreach (Vector3 fire_datum in fire_data)
		{
			Vector3 normalized = (fire_datum - fire_ori.position).normalized;
			GameObject gameObject = Object.Instantiate(Accessory[4], base.transform.position, Quaternion.LookRotation(normalized)) as GameObject;
			PGMCoopProjectile component = gameObject.GetComponent<PGMCoopProjectile>();
			component.launch_dir = normalized;
			component.fly_speed = 30f;
			component.explode_radius = explode_radius;
			component.life = 10f;
			component.damage = 0f;
			component.object_controller = controller;
			component.weapon_controller = this;
			component.targetPos = fire_datum;
		}
	}
}
