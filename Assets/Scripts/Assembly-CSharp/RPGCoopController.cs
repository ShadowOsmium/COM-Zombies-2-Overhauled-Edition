using UnityEngine;

public class RPGCoopController : WeaponCoopController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	protected GameObject projectile;

	public GameObject fire_line_obj;

	public GameObject fire_smoke_obj;

	private float explode_radius;

	protected bool is_inited;

    private float shootAnimationLockTimer = 0f;

    private const float SHOOT_ANIMATION_DURATION = 1f;

    private Vector3 fire_target = Vector3.zero;

	private void Awake()
	{
		weapon_type = WeaponType.RocketLauncher;
		projectile = base.transform.Find("Projectile_RPG").gameObject;
		if (projectile == null)
		{
			Debug.LogError(string.Concat("weapon:", weapon_type, " can't find Projectile_RPG!"));
		}
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
		is_inited = true;
	}

    public override void Update()
    {
        base.Update();

        if (shootAnimationLockTimer > 0f)
            shootAnimationLockTimer -= Time.deltaTime;
    }

    public override void ResetFireInterval()
	{
		base.ResetFireInterval();
		projectile.GetComponent<Renderer>().enabled = render_obj.GetComponent<Renderer>().enabled;
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		if (weapon_data.weapon_name == "RPG")
		{
			ANI_IDLE_RUN = "RPG_UpperBody_Run01";
			ANI_IDLE_SHOOT = "RPG_UpperBody_Idle01";
			ANI_SHOOT = "RPG_UpperBody_Shooting01";
			ANI_RELOAD = "RPG_UpperBody_Reload01";
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
		render_obj = base.gameObject;
		base.SetWeaponAni(player, spine);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		render_obj = base.gameObject;
	}

	public override void FireUpdate(PlayerController player, float deltaTime)
	{
		base.FireUpdate(player, deltaTime);
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
	}

	public override void CheckHit(ObjectController controller)
	{
		if (is_inited && fireable)
		{
			fireable = false;
			projectile.GetComponent<Renderer>().enabled = false;
			fire_line_obj.GetComponent<ParticleSystem>().Stop();
			fire_smoke_obj.transform.position = base.transform.position;
			fire_smoke_obj.transform.rotation = base.transform.rotation;
			fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
			fire_smoke_obj.GetComponent<ParticleSystem>().Play();
			Vector3 normalized = (fire_target - fire_ori.position).normalized;
			GameObject gameObject = Object.Instantiate(Accessory[0], base.transform.position, Quaternion.LookRotation(normalized)) as GameObject;
			RPGProjectile component = gameObject.GetComponent<RPGProjectile>();
			component.launch_dir = normalized;
			component.fly_speed = 30f;
			component.explode_radius = explode_radius;
			component.life = 3f;
			component.damage = 0f;
			component.object_controller = controller;
			component.weapon_controller = this;
		}
	}

	public override void GunOn()
	{
		base.GunOn();
		if (HaveBullets())
		{
			projectile.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			projectile.GetComponent<Renderer>().enabled = false;
		}
	}

	public override void GunOff()
	{
		base.GunOff();
		projectile.GetComponent<Renderer>().enabled = false;
	}

    public bool IsShootAnimationLocked()
    {
        return shootAnimationLockTimer > 0f;
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
			if (playerController.avatar_data.avatar_type == AvatarType.Cowboy)
			{
				AnimationUtil.Stop(playerController.CowboyCap);
				AnimationUtil.PlayAnimate(playerController.CowboyCap, "RPG_Shooting01", WrapMode.Once);
			}
            fire_target = target;
            shootAnimationLockTimer = SHOOT_ANIMATION_DURATION;
        }
	}
}
