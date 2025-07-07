using System;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class IceGunController : WeaponController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	protected GameObject fire_line;

	protected List<GameObject> rend_obj_list = new List<GameObject>();

	public float frozenTime;

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
		fire_line = UnityEngine.Object.Instantiate(Accessory[0]) as GameObject;
		fire_line.transform.localPosition = Vector3.zero;
		fire_line.transform.localRotation = Quaternion.identity;
		fire_line.GetComponent<ParticleSystem>().Stop();
		eff_audio = fire_line.GetComponent<EffectAudioBehaviour>();
		frozenTime = (float)weapon_data.config.Ex_conf["frozenTime"];
		ready_fire_time = 0.3f;
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
		fire_line.transform.position = fire_ori.transform.position + fire_ori.forward * 1.2f;
		fire_line.transform.rotation = fire_ori.transform.rotation;
		fire_line.GetComponent<ParticleSystem>().Play();
		eff_audio.PlayEffect();
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
