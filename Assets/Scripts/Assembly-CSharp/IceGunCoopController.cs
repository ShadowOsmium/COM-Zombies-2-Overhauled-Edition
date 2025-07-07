using System.Collections.Generic;
using UnityEngine;

public class IceGunCoopController : WeaponCoopController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	protected GameObject fire_line;

	protected List<GameObject> rend_obj_list = new List<GameObject>();

	public float frozenTime;

	protected bool is_inited;

	protected EffectAudioBehaviour eff_audio;

	private void Awake()
	{
		weapon_type = WeaponType.IceGun;
	}

	private void Start()
	{
		fire_ori = base.transform.Find("fire_ori");
		if (fire_ori == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find fire_ori!"));
		}
		fire_line = Object.Instantiate(Accessory[0]) as GameObject;
		fire_line.transform.localPosition = Vector3.zero;
		fire_line.transform.localRotation = Quaternion.identity;
		fire_line.GetComponent<ParticleSystem>().Stop();
		eff_audio = fire_line.GetComponent<EffectAudioBehaviour>();
		frozenTime = (float)weapon_data.config.Ex_conf["frozenTime"];
		ready_fire_time = 0.3f;
		is_inited = true;
	}

	public override void Update()
	{
		if (is_inited)
		{
			base.Update();
		}
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
		ANI_FIRE_READY = "GyoGun_UpperBody_FireReady";
		ANI_IDLE_RUN = "GyoGun_UpperBody_Run01";
		ANI_IDLE_SHOOT = "GyoGun_UpperBody_Idle01";
		ANI_SHOOT = "GyoGun_UpperBody_Shooting01";
		ANI_RELOAD = "GyoGun_UpperBody_Reload01";
		ANI_SHIFT_WEAPON = "GyoGun_UpperBody_Shiftweapon01";
		ANI_IDLE_DOWN = "W1200_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "GyoGun_LowerBody_Shooting01";
		ANI_RELOAD_DOWN = "GyoGun_LowerBody_Reload01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		ResetReloadAniSpeed(player);
		ResetFireAniSpeed(player);
		base.SetWeaponAni(player, spine);
		rend_obj_list.Add(base.transform.Find("Weapon_CryoGun_01").gameObject);
		rend_obj_list.Add(base.transform.Find("Weapon_CryoGun_02").gameObject);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		rend_obj_list.Add(base.transform.Find("Weapon_CryoGun_01").gameObject);
		rend_obj_list.Add(base.transform.Find("Weapon_CryoGun_02").gameObject);
	}

	public override void FireUpdate(PlayerController player, float deltaTime)
	{
		if (is_inited)
		{
			base.FireUpdate(player, deltaTime);
		}
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		if (is_inited && OnReadyFire(player, deltaTime))
		{
			base.Fire(player, deltaTime);
			fireable = true;
			AnimationUtil.Stop(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState));
			AnimationUtil.PlayAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.ClampForever);
		}
	}

	public override void CheckHit(ObjectController controller)
	{
		if (is_inited && fireable)
		{
			fireable = false;
			fire_line.transform.position = fire_ori.transform.position + fire_ori.forward * 1.2f;
			fire_line.transform.rotation = fire_ori.transform.rotation;
			fire_line.GetComponent<ParticleSystem>().Play();
			eff_audio.PlayEffect();
			PlayerController playerController = controller as PlayerController;
			playerController.CouldEnterAfterFire = true;
		}
	}

	public override bool OnReadyFire(PlayerController player, float deltaTime)
	{
		if (ready_fire == FireReady.None)
		{
			fireable = false;
		}
		return base.OnReadyFire(player, deltaTime);
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
