using System.Collections.Generic;
using UnityEngine;

public class IonCannonCoopController : WeaponCoopController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	public GameObject fire_smoke_obj;

	private float explode_radius;

	protected List<GameObject> rend_obj_list = new List<GameObject>();

	protected bool is_inited;

	private Vector3 fire_target = Vector3.zero;

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
		if (is_inited)
		{
			base.FireUpdate(player, deltaTime);
		}
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
	}

    public override void CheckHit(ObjectController controller)
    {
        if (is_inited && fireable)
        {
            fireable = false;
            Vector3 normalized = (fire_target - fire_ori.position).normalized;
            GameObject gameObject = Object.Instantiate(Accessory[0], base.transform.position, Quaternion.LookRotation(normalized)) as GameObject;
            IonProjectile component = gameObject.GetComponent<IonProjectile>();
            component.launch_dir = normalized;
            component.fly_speed = 30f;
            component.explode_radius = explode_radius;
            component.life = 3f;

            PlayerController playerController = controller as PlayerController;
            if (playerController != null)
            {
                float damage = GetDamageValWithAvatar(playerController, playerController.avatar_data);
                component.damage = damage;
            }
            else
            {
                component.damage = weapon_data.damage_val;
            }

            component.object_controller = controller;
            component.weapon_controller = this;
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

	public void OnRemoteFire(ObjectController controller, Vector3 target)
	{
		if (is_inited)
		{
			PlayerController playerController = controller as PlayerController;
			base.Fire(playerController, 0.03f);
			fireable = true;
			AnimationUtil.Stop(playerController.gameObject, playerController.GetFireStateAnimation(playerController.MoveState, playerController.FireState));
			AnimationUtil.PlayAnimate(playerController.gameObject, playerController.GetFireStateAnimation(playerController.MoveState, playerController.FireState), WrapMode.ClampForever);
			fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
			fire_smoke_obj.GetComponent<ParticleSystem>().Play();
			if (playerController.avatar_data.avatar_type == AvatarType.Cowboy)
			{
				AnimationUtil.Stop(playerController.CowboyCap);
				AnimationUtil.PlayAnimate(playerController.CowboyCap, "RPG_Shooting01", WrapMode.Once);
			}
			fire_target = target;
		}
	}
}
