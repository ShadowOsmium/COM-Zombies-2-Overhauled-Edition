using CoMZ2;
using TNetSdk;
using UnityEngine;

public class RPGController : WeaponController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	protected GameObject projectile;

	public GameObject fire_line_obj;

	public GameObject fire_smoke_obj;

	private float explode_radius;

	private TNetObject tnetObj;

    private float shootAnimationLockTimer = 0f;

    private const float SHOOT_ANIMATION_DURATION = 1f;

    private bool isShootingAnimationPlaying = false;

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
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		//{
		//	tnetObj = TNetConnection.Connection;
		//}
	}

    public override void Update()
    {
        base.Update();

        if (shootAnimationLockTimer > 0f)
        {
            shootAnimationLockTimer -= Time.deltaTime;
        }
    }

    public override void OnWeaponReload()
    {
        if (shootAnimationLockTimer > 0f)
        {
            // Prevent reloading if RPG shoot animation is still playing
            Debug.Log("Blocked reload because RPG shoot animation is still active.");
            return;
        }

        base.OnWeaponReload(); // Only allow reload after shoot animation
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

            if (player.avatar_data.avatar_type == AvatarType.Cowboy)
            {
                AnimationUtil.Stop(player.CowboyCap);
                AnimationUtil.PlayAnimate(player.CowboyCap, "RPG_Shooting01", WrapMode.Once);
            }

            shootAnimationLockTimer = SHOOT_ANIMATION_DURATION;
        }
    }

    public override void CheckHit(ObjectController controller)
	{
		if (fireable)
		{
			fireable = false;
            isShootingAnimationPlaying = false;
            weapon_data.OnFire();
			projectile.GetComponent<Renderer>().enabled = false;
			fire_line_obj.GetComponent<ParticleSystem>().Stop();
			fire_smoke_obj.transform.position = base.transform.position;
			fire_smoke_obj.transform.rotation = base.transform.rotation;
			fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
			fire_smoke_obj.GetComponent<ParticleSystem>().Play();
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
			RPGProjectile component = gameObject.GetComponent<RPGProjectile>();
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
			//	sFSObject.PutSFSArray("rpgFire", sFSArray);
			//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
			//}
		}
	}

    public bool IsShootAnimationLocked()
    {
        return shootAnimationLockTimer > 0f;
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
}
