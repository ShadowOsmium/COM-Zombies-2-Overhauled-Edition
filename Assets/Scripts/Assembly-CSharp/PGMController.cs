using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class PGMController : WeaponController
{
	protected Transform fire_ori;

	protected string ani_interval = string.Empty;

	protected bool fireable;

	public GameObject fire_line_obj;

	public GameObject fire_smoke_obj;

	private float cur_auto_lock_time;

    private const string ignoreProjectileTag = "IgnoreProjectile";

    private float auto_lock_time_interval = 0.4f;

	protected GameObject pgm_sight;

	public Dictionary<NearestTargetInfo, GameObject> auto_lock_target_dir = new Dictionary<NearestTargetInfo, GameObject>();

	private float explode_radius;

	private TNetObject tnetObj;

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
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		//{
		//	tnetObj = TNetConnection.Connection;
		//}
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
		if (GameSceneController.Instance.main_camera != null)
		{
			GameSceneController.Instance.main_camera.ZoomIn(deltaTime);
		}
		if (CouldMakeNextShoot())
		{
			AutoLock(player, deltaTime);
		}
	}

    public void AutoLock(PlayerController player, float deltaTime)
    {
        if (!weapon_data.EnableFire())
            return;

        cur_auto_lock_time += deltaTime;

        // Clean up invalid or dead targets
        List<NearestTargetInfo> removeList = new List<NearestTargetInfo>();
        foreach (var pair in auto_lock_target_dir)
        {
            var key = pair.Key;
            if (key == null || key.target_obj == null)
            {
                if (key != null) key.enabel = false;
                removeList.Add(key);
            }
            else if (key.type == NearestTargetInfo.NearestTargetType.Enemy)
            {
                EnemyController ec = key.target_obj as EnemyController;
                if (ec != null && ec.Enemy_State.GetStateType() == EnemyStateType.Dead)
                {
                    key.enabel = false;
                    removeList.Add(key);
                }
            }
        }

        foreach (var key in removeList)
        {
            if (auto_lock_target_dir.ContainsKey(key))
            {
                GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().DeleteObject(auto_lock_target_dir[key]);
                auto_lock_target_dir.Remove(key);
            }
        }

        // Update screen positions of existing targets
        foreach (var key in auto_lock_target_dir.Keys)
        {
            if (key != null && key.enabel)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(key.LockPosition);
                key.screenPos = GameSceneController.Instance.tui_camera.GetComponent<Camera>().ScreenToWorldPoint(screen);
            }
        }

        if (cur_auto_lock_time < auto_lock_time_interval)
            return;

        cur_auto_lock_time = 0f;

        NearestTargetInfo bestTarget = null;
        float bestScore = float.MinValue;

        // Collect enemies for selection
        List<EnemyController> enemies = new List<EnemyController>(GameSceneController.Instance.Enemy_Set.Values);

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyController enemy = enemies[i];
            if (enemy == null) continue;
            if (enemy.Enemy_State.GetStateType() == EnemyStateType.Dead) continue;
            if (ContainTarget(enemy)) continue; // skip already targeted

            Vector3 screen = Camera.main.WorldToScreenPoint(enemy.centroid);
            if (screen.z < 0f) continue;

            Vector3 uiPoint = GameSceneController.Instance.tui_camera.GetComponent<Camera>().ScreenToWorldPoint(screen);
            if (!GameSceneController.Instance.PGM_Lock_Rect.PtInControl(new Vector2(uiPoint.x, uiPoint.y))) continue;

            if (GameSceneController.CheckBlockBetween(enemy.centroid, Camera.main.transform.position)) continue;

            float score = GetTargetScore(enemy);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = new NearestTargetInfo()
                {
                    type = NearestTargetInfo.NearestTargetType.Enemy,
                    transform = enemy.transform,
                    screenPos = uiPoint,
                    target_obj = enemy,
                    enabel = true
                };
            }
        }

        // If found a best target, create its sight UI and add to dictionary
        if (bestTarget != null)
        {
            GameObject sight = GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().CreateObject();
            sight.transform.parent = GameSceneController.Instance.game_main_panel.transform;
            sight.transform.localPosition = bestTarget.screenPos;
            auto_lock_target_dir.Add(bestTarget, sight);
            player.PlayPlayerAudio("AutoLock");
        }

        // Update sight positions
        foreach (var pair in auto_lock_target_dir)
        {
            if (pair.Key != null && pair.Key.enabel && pair.Value != null)
            {
                pair.Value.transform.localPosition = new Vector3(pair.Key.screenPos.x, pair.Key.screenPos.y, 0f);
            }
        }
    }

    private float GetTargetScore(EnemyController enemy)
    {
        if (enemy == null)
            return float.MinValue;

        Vector3 toTarget = enemy.transform.position - fire_ori.position;
        float sqrDist = toTarget.sqrMagnitude;
        if (sqrDist < 1f) sqrDist = 1f;

        float distanceScore = 1f / sqrDist;
        float bossBonus = enemy.IsBoss ? 2f : 0f;

        float timeSinceLastHit = Time.time - enemy.LastInjuredTime;
        float damageRecency = 0f;
        if (timeSinceLastHit >= 0f && timeSinceLastHit < 5f)
        {
            damageRecency = (5f - timeSinceLastHit) * 0.1f;
        }

        return distanceScore + bossBonus + damageRecency;
    }


    public override void Fire(PlayerController player, float deltaTime)
	{
		if (weapon_data.EnableFire() && AimedTarget())
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
		}
	}

    public override void CheckHit(ObjectController controller)
	{
		if (!fireable)
		{
			return;
		}
		fireable = false;
		fire_line_obj.GetComponent<ParticleSystem>().Stop();
		fire_line_obj.GetComponent<ParticleSystem>().Play();
		fire_smoke_obj.transform.position = base.transform.position;
		fire_smoke_obj.transform.rotation = base.transform.rotation;
		fire_smoke_obj.GetComponent<ParticleSystem>().Stop();
		fire_smoke_obj.GetComponent<ParticleSystem>().Play();
		GameSceneController.Instance.SightBead.Stretch(weapon_data.config.recoil);
		SFSArray sFSArray = new SFSArray();
		foreach (NearestTargetInfo key in auto_lock_target_dir.Keys)
		{
			if (key != null && key.enabel)
			{
				if (!weapon_data.OnFire())
				{
					break;
				}
				Vector3 normalized = (key.LockPosition - fire_ori.position).normalized;
				GameObject gameObject = Object.Instantiate(Accessory[0], base.transform.position, Quaternion.LookRotation(normalized)) as GameObject;
				PGMProjectile component = gameObject.GetComponent<PGMProjectile>();
				component.launch_dir = normalized;
				component.fly_speed = 20f;
				component.explode_radius = explode_radius;
				component.life = 10f;
				component.damage = weapon_data.damage_val;
				component.object_controller = controller;
				component.weapon_controller = this;
				component.targetPos = key.LockPosition;
				component.target_trans = key.transform;
				component.target_info = key;
				//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
				//{
				//	SFSArray sFSArray2 = new SFSArray();
				//	sFSArray2.AddFloat(component.targetPos.x);
				//	sFSArray2.AddFloat(component.targetPos.y);
				//	sFSArray2.AddFloat(component.targetPos.z);
				//	sFSArray.AddSFSArray(sFSArray2);
				//}
			}
		}
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null && sFSArray.Size() > 0)
		//{
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutSFSArray("pgmFire", sFSArray);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
		CleanLockTargets();
	}

	public override void GunOn()
	{
		base.GunOn();
	}

	public override void GunOff()
	{
		base.GunOff();
		CleanLockTargets();
	}

	protected bool ContainTarget(ObjectController target_obj)
	{
		bool result = false;
		foreach (NearestTargetInfo key in auto_lock_target_dir.Keys)
		{
			if (key != null && key.target_obj != null && target_obj == key.target_obj)
			{
				return true;
			}
		}
		return result;
	}

	protected bool AimedTarget()
	{
		bool result = false;
		foreach (NearestTargetInfo key in auto_lock_target_dir.Keys)
		{
			if (key != null && key.enabel)
			{
				return true;
			}
		}
		return result;
	}

	public override void OnFireRelease(PlayerController player)
	{
		Fire(player, Time.deltaTime);
	}

	public void CleanLockTargets()
	{
		auto_lock_target_dir.Clear();
		cur_auto_lock_time = auto_lock_time_interval;
		if (GameSceneController.Instance != null && GameSceneController.Instance.pgm_sight_pool != null)
		{
			GameSceneController.Instance.pgm_sight_pool.GetComponent<ObjectPool>().AutoDestructAll();
		}
	}
}
