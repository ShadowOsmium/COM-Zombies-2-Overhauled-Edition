using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class SharkController : EnemyController
{
	private string ani_attack = "Attack01";

	private string ani_damage_1 = "Zombie_Guter_Trennung_Damage01";

	private string ani_damage_2 = "Zombie_Guter_Trennung_Damage02";

	public string ANI_MISSILE = "Zombie_Guter_Trennung_Attack01";

	public string ANI_RUSH_01 = "Zombie_Guter_Trennung_Attack02_01";

	public string ANI_RUSH_02 = "Zombie_Guter_Trennung_Attack02_02";

	public string ANI_RUSH_03 = "Zombie_Guter_Trennung_Attack02_03";

	public string ANI_RUSH_04 = "Zombie_Guter_Trennung_Attack02_04";

	public string ANI_DIVE_01 = "Zombie_Guter_Trennung_Attack03_01";

	public string ANI_DIVE_02 = "Zombie_Guter_Trennung_Attack03_02";

	public EnemyState SKILL_RUSH_STATE;

	public EnemyState SKILL_MISSILE_STATE;

	public EnemyState SKILL_DIVE_STATE;

	private float injured_val_state;

	private float injured_val_percent;

	private float cur_rush_time;

	private float rush_speed_ratio = 5f;

	private float rush_range = 10f;

	private float rush_range_min = 10f;

	private float rush_interval_time = 8f;

	private float rush_dmg_ratio = 1f;

	private float rush_dmg_range = 3f;

	public float rush_rest_time = 1f;

	public bool rush_enable = true;

	private float cur_missile_time;

	private float missile_speed = 7f;

	private float missile_range = 15f;

	private float missile_interval_time = 8f;

	private float missile_dmg_ratio = 1f;

	public bool missile_enable = true;

	private float cur_dive_time;

	private float dive_speed_ratio = 5f;

	private float dive_range = 10f;

	private float dive_interval_time = 8f;

	private float dive_dmg_ratio = 1f;

	private float dive_dmg_range = 3f;

	public float dive_time = 1f;

	public bool dive_enable = true;

	private Vector3 rush_target = Vector3.zero;

	private GameObject eff_bellow_obj;

	private ParticleSystem eff_bellow;

	private GameObject eff_angry_obj;

	private ParticleSystem eff_angry;

	private GameObject ground_stone1;

	private GameObject ground_stone2;

	private string stone_ani1 = string.Empty;

	private string stone_ani2 = string.Empty;

	public GameObject shark_fish;

	public GameObject shark_fin;

	public GameObject rush_eff_obj;

	public GameObject diving_eff_obj;

	public GameObject dive_out_eff_obj;

	protected List<GameObject> rend_obj_list = new List<GameObject>();

	public override Vector3 centroid
	{
		get
		{
			return base.transform.position + Vector3.up * 1.6f;
		}
	}

	public override void Init()
	{
		ANI_IDLE = "Zombie_Guter_Trennung_Idle01";
		ANI_ATTACK = "Zombie_Guter_Trennung_Attack01";
		ANI_INJURED = "Zombie_Guter_Trennung_Damage01";
		ANI_DEAD = "Zombie_Guter_Trennung_Death01";
		ANI_RUN = "Zombie_Guter_Trennung_Forward01";
		ANI_SHOW = "Zombie_Guter_Trennung_Show01";
		ANI_HALF_HP = "Zombie_Guter_Trennung_Bellow01";
		shark_fish = base.transform.Find("Dummy_All/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 Prop1/Zombie_Guter_Trennung_Weapon_Skin").gameObject;
		shark_fin = base.transform.Find("Fin").gameObject;
		ShowSharkFin(false);
		float time = 0f;
		if (base.IsBoss)
		{
			if (GameSceneController.Instance.enable_spawn_ani == "Zombie_Guter_Tennung_Camera_Show01")
			{
				ANI_SHOW = "Zombie_Guter_Trennung_Show01";
				time = 0.4f;
			}
			else if (GameSceneController.Instance.enable_spawn_ani == "Zombie_Guter_Tennung_Camera_Show02")
			{
				ANI_SHOW = "Zombie_Guter_Trennung_Show02";
				time = 4.2f;
			}
			CreateHpBar(Vector3.up * 3f);
		}
		stone_ani1 = "01";
		stone_ani2 = "01";
		rush_eff_obj = base.transform.Find("water").gameObject;
		ShowSharkRushEff(false);
		diving_eff_obj = base.transform.Find("smoke_04").gameObject;
		ShowSharkDivingEff(false);
		dive_out_eff_obj = base.transform.Find("smoke_02").gameObject;
		HideSharkDiveOutEff();
		rend_obj_list.Add(shark_fish);
		rend_obj_list.Add(base.transform.Find("Zombie_Guter_Trennung_02").gameObject);
		rend_obj_list.Add(base.transform.Find("Zombie_Guter_Trennung_01").gameObject);
		rend_obj_list.Add(base.transform.Find("Zombie_Guter_Trennung_03").gameObject);
		Invoke("PlayGroundStone", time);
		base.Init();
		SKILL_RUSH_STATE = EnemyState.Create(EnemyStateType.SharkRush, this);
		SKILL_MISSILE_STATE = EnemyState.Create(EnemyStateType.SharkMissile, this);
		SKILL_DIVE_STATE = EnemyState.Create(EnemyStateType.SharkDive, this);
		rush_speed_ratio = (float)enemy_data.config.Ex_conf["rushSpeedRatio"];
		rush_range = (float)enemy_data.config.Ex_conf["rushRange"];
		rush_range_min = (float)enemy_data.config.Ex_conf["rushRangeMin"];
		rush_interval_time = (float)enemy_data.config.Ex_conf["rushCDTime"];
		rush_dmg_ratio = (float)enemy_data.config.Ex_conf["rushDmgRatio"];
		rush_rest_time = (float)enemy_data.config.Ex_conf["rushRestTime"];
		missile_speed = (float)enemy_data.config.Ex_conf["missileSpeed"];
		missile_range = (float)enemy_data.config.Ex_conf["missileRange"];
		missile_interval_time = (float)enemy_data.config.Ex_conf["missileCDTime"];
		missile_dmg_ratio = (float)enemy_data.config.Ex_conf["missileDmgRatio"];
		dive_speed_ratio = (float)enemy_data.config.Ex_conf["diveSpeedRatio"];
		dive_range = (float)enemy_data.config.Ex_conf["diveRange"];
		dive_interval_time = (float)enemy_data.config.Ex_conf["diveCDTime"];
		dive_dmg_ratio = (float)enemy_data.config.Ex_conf["diveDmgRatio"];
		dive_time = (float)enemy_data.config.Ex_conf["diveTime"];
		injured_val_percent = (float)enemy_data.config.Ex_conf["injuredPercent"];
		eff_bellow_obj = base.transform.Find("fatcook_houjiao").gameObject;
		eff_bellow = eff_bellow_obj.GetComponent<ParticleSystem>();
		eff_bellow.Stop();
		eff_angry_obj = base.transform.Find("crazy_boss").gameObject;
		eff_angry = eff_angry_obj.GetComponent<ParticleSystem>();
		eff_angry.Stop();
		SetState(SHOW_STATE);
	}

    public override void CheckHit()
    {
        if (enemyState == SKILL_MISSILE_STATE)
        {
            if (target_player != null)
            {
                AnimationUtil.Stop(shark_fish);
                GameObject gameObject = Object.Instantiate(base.Accessory[2], shark_fish.transform.position, shark_fish.transform.rotation) as GameObject;
                SharkProjectile component = gameObject.GetComponent<SharkProjectile>();
                component.launch_dir = target_player.centroid - gameObject.transform.position;
                component.fly_speed = 20f;
                component.life = 10f;
                component.damage = enemy_data.damage_val * missile_dmg_ratio;
                component.object_controller = this;
                component.targetPos = target_player.centroid;
                component.target_trans = target_player.transform;
                AnimationUtil.PlayAnimate(gameObject, "Zombie_Guter_Trennung_Weapon_Attack01", WrapMode.Loop);
            }
            return;
        }
        if (enemyState == SKILL_RUSH_STATE)
        {
            foreach (PlayerController value in GameSceneController.Instance.Player_Set.Values)
            {
                if ((value.centroid - centroid).sqrMagnitude < rush_dmg_range * rush_dmg_range && !GameSceneController.CheckBlockBetween(centroid, value.centroid))
                {
                    Vector3 dir = value.centroid - centroid;
                    dir.y = 0f;
                    dir.Normalize();
                    value.OnHit(enemy_data.damage_val * rush_dmg_ratio, null, this, value.centroid, Vector3.zero);
                }
            }
            foreach (NPCController value2 in GameSceneController.Instance.NPC_Set.Values)
            {
                if ((value2.centroid - centroid).sqrMagnitude < rush_dmg_range * rush_dmg_range && !GameSceneController.CheckBlockBetween(centroid, value2.centroid))
                {
                    Vector3 dir = value2.centroid - centroid;
                    dir.y = 0f;
                    dir.Normalize();
                    value2.OnHit(enemy_data.damage_val * rush_dmg_ratio, null, this, value2.centroid, Vector3.zero);
                }
            }
            foreach (GuardianForceController value3 in GameSceneController.Instance.GuardianForce_Set.Values)
            {
                if ((value3.centroid - centroid).sqrMagnitude < rush_dmg_range * rush_dmg_range && !GameSceneController.CheckBlockBetween(centroid, value3.centroid))
                {
                    Vector3 dir = value3.centroid - centroid;
                    dir.y = 0f;
                    dir.Normalize();
                    value3.OnHit(enemy_data.damage_val * rush_dmg_ratio, null, this, value3.centroid, Vector3.zero);
                }
            }
            foreach (EnemyController value4 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
            {
                if ((value4.centroid - centroid).sqrMagnitude < rush_dmg_range * rush_dmg_range && !GameSceneController.CheckBlockBetween(centroid, value4.centroid))
                {
                    Vector3 dir = value4.centroid - centroid;
                    dir.y = 0f;
                    dir.Normalize();
                    value4.OnHit(enemy_data.damage_val * rush_dmg_ratio, null, this, value4.centroid, Vector3.zero);
                }
            }
            return;
        }
        if (enemyState != SKILL_DIVE_STATE)
        {
            return;
        }
        foreach (PlayerController value5 in GameSceneController.Instance.Player_Set.Values)
        {
            if ((value5.centroid - centroid).sqrMagnitude < dive_dmg_range * dive_dmg_range && !GameSceneController.CheckBlockBetween(centroid, value5.centroid))
            {
                Vector3 dir = value5.centroid - centroid;
                dir.y = 0f;
                dir.Normalize();
                value5.OnHit(enemy_data.damage_val * dive_dmg_ratio, null, this, value5.centroid, dir);
            }
        }
        foreach (NPCController value6 in GameSceneController.Instance.NPC_Set.Values)
        {
            if ((value6.centroid - centroid).sqrMagnitude < dive_dmg_range * dive_dmg_range && !GameSceneController.CheckBlockBetween(centroid, value6.centroid))
            {
                Vector3 dir = value6.centroid - centroid;
                dir.y = 0f;
                dir.Normalize();
                value6.OnHit(enemy_data.damage_val * dive_dmg_ratio, null, this, value6.centroid, dir);
            }
        }
        foreach (GuardianForceController value7 in GameSceneController.Instance.GuardianForce_Set.Values)
        {
            if ((value7.centroid - centroid).sqrMagnitude < dive_dmg_range * dive_dmg_range && !GameSceneController.CheckBlockBetween(centroid, value7.centroid))
            {
                Vector3 dir = value7.centroid - centroid;
                dir.y = 0f;
                dir.Normalize();
                value7.OnHit(enemy_data.damage_val * dive_dmg_ratio, null, this, value7.centroid, dir);
            }
        }
        foreach (EnemyController value8 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
        {
            if ((value8.centroid - centroid).sqrMagnitude < dive_dmg_range * dive_dmg_range && !GameSceneController.CheckBlockBetween(centroid, value8.centroid))
            {
                Vector3 dir = value8.centroid - centroid;
                dir.y = 0f;
                dir.Normalize();
                value8.OnHit(enemy_data.damage_val * dive_dmg_ratio, null, this, value8.centroid, dir);
            }
        }
    }


    public void ResetInjureAni(Vector3 hit_point)
	{
	}

	public override void FireUpdate(float deltaTime)
	{
		if (AnimationUtil.IsPlayingAnimation(base.gameObject, ani_attack))
		{
			AlwaysLookAtTarget();
		}
	}

	public override void Fire()
	{
		ANI_CUR_ATTACK = (ANI_ATTACK = ani_attack);
		AnimationUtil.CrossAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
	}

	public override void EnterAttack()
	{
		Fire();
	}

	public override void OnDead(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		base.OnDead(damage, weapon, player, hit_point, hit_normal);
		Object.Destroy(base.GetComponent<Collider>());
		StartCoroutine(RemoveOnTime(3f));
		eff_angry.Stop();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public override void Dologic(float deltaTime)
	{
		base.Dologic(deltaTime);
		if (!rush_enable)
		{
			cur_rush_time += deltaTime;
			if (cur_rush_time >= rush_interval_time)
			{
				cur_rush_time = 0f;
				rush_enable = true;
			}
		}
		if (!missile_enable)
		{
			cur_missile_time += deltaTime;
			if (cur_missile_time >= missile_interval_time)
			{
				cur_missile_time = 0f;
				missile_enable = true;
			}
		}
		if (!dive_enable)
		{
			cur_dive_time += deltaTime;
			if (cur_dive_time >= dive_interval_time)
			{
				cur_dive_time = 0f;
				dive_enable = true;
			}
		}
	}

	public override void DetermineNormalState()
	{
		CheckTargetPlayer();
		if (target_player == null)
		{
			SetState(IDLE_STATE);
		}
		else if (!((BossHalfHpState)HALF_HP_STATE).IsPlayed && enemy_data.cur_hp / enemy_data.hp_capacity <= 0.5f)
		{
			SetState(HALF_HP_STATE);
		}
		else if (CouldEnterDiveState())
		{
			SetState(SKILL_DIVE_STATE);
		}
		else if (CouldEnterMissileState())
		{
			SetState(SKILL_MISSILE_STATE);
		}
		else if (CouldEnterRushState())
		{
			SetState(SKILL_RUSH_STATE);
		}
		else
		{
			SetState(CATCHING_STATE);
		}
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		if (enemyState == SHOW_STATE || enemyState == DEAD_STATE)
		{
			return;
		}
		injured_time = Time.time;
		if (!hatred_set.ContainsKey(player))
		{
			hatred_set.Add(player, 1f);
		}
		Dictionary<ObjectController, float> dictionary;
		Dictionary<ObjectController, float> dictionary2 = (dictionary = hatred_set);
		ObjectController key;
		ObjectController key2 = (key = player);
		float num = dictionary[key];
		dictionary2[key2] = num + damage;
        //if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && damage > 0f && tnetObj != null)
        //{
        //	SFSObject sFSObject = new SFSObject();
        //	SFSArray sFSArray = new SFSArray();
        //	sFSArray.AddShort((short)enemy_id);
        //	sFSArray.AddFloat(damage);
        //	sFSArray.AddBool(false);
        //	sFSArray.AddFloat(0f);
        //	sFSObject.PutSFSArray("enemyInjured", sFSArray);
        //	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
        //	Dictionary<PlayerID, float> player_damage_Set;
        //	Dictionary<PlayerID, float> dictionary3 = (player_damage_Set = GameSceneController.Instance.Player_damage_Set);
        //	PlayerID player_id;
        //	PlayerID key3 = (player_id = GameSceneController.Instance.player_controller.player_id);
        //	num = player_damage_Set[player_id];
        //	dictionary3[key3] = num + damage;
        //}
        if (enemy_data.OnInjured(damage, player.GetComponent<ObjectController>()))
        {
			GameSceneController.Instance.UpdateEnemyDeathInfo(enemy_data.enemy_type, 1);
			OnDead(damage, weapon, player, hit_point, hit_normal);
			SetState(DEAD_STATE);
			//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && damage > 0f && tnetObj != null)
			//{
			//	SFSObject sFSObject2 = new SFSObject();
			//	SFSArray sFSArray2 = new SFSArray();
			//	sFSArray2.AddShort((short)enemy_id);
			//	sFSArray2.AddFloat(damage);
			//	sFSObject2.PutSFSArray("enemyDead", sFSArray2);
			//	tnetObj.Send(new BroadcastMessageRequest(sFSObject2));
			//}
		}
		else
		{
			injured_val_state += damage;
			if (injured_val_state / enemy_data.hp_capacity >= injured_val_percent && base.Enemy_State.GetStateType() == EnemyStateType.Catching)
			{
				switch (Random.Range(0, 100) % 3)
				{
				case 0:
					ANI_INJURED = ani_damage_1;
					break;
				case 1:
					ANI_INJURED = ani_damage_2;
					break;
				}
				AnimationUtil.Stop(base.gameObject);
				AnimationUtil.CrossAnimate(base.gameObject, ANI_INJURED, WrapMode.ClampForever);
				SetState(INJURED_STATE);
			}
		}
		CreateInjuredBloodEff(hit_point, hit_normal);
		UpdateCurHpBar();
	}

	public bool CouldEnterRushState()
	{
		if (rush_enable && base.SqrDistanceFromPlayer <= rush_range * rush_range && base.SqrDistanceFromPlayer >= rush_range_min * rush_range_min && target_player != null && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public bool CouldEnterMissileState()
	{
		if (missile_enable && base.SqrDistanceFromPlayer <= missile_range * missile_range && target_player != null && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public bool CouldEnterDiveState()
	{
		if (dive_enable && base.SqrDistanceFromPlayer <= dive_range * dive_range && target_player != null && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public override void FixedUpdate()
	{
	}

	public override void SetState(EnemyState state)
	{
		if (enemyState != null && enemyState.GetStateType() != state.GetStateType())
		{
			enemyState.OnExitState();
			enemyState = state;
			enemyState.OnEnterState();
			injured_val_state = 0f;
		}
	}

	public override bool OnChaisawSpecialHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		return false;
	}

	public override void OnChaisawInjuredResease(ObjectController player)
	{
	}

	public override void OnChaisawSkillDead(ObjectController player)
	{
	}

	public override void OnRepel(Vector3 dir)
	{
	}

	public override void RepelOver()
	{
	}

	public override void OnShowUnderGround()
	{
		if (!(autio_controller != null))
		{
		}
	}

	private void HideGroundStone()
	{
		Object.Destroy(ground_stone1);
		Object.Destroy(ground_stone2);
	}

	public override void PlayHalfHpEffect()
	{
		base.PlayHalfHpEffect();
		eff_bellow.Play();
		eff_angry.Play();
        if (GameSceneController.Instance.mission_day_type == MissionDayType.Endless)
            return;
        Transform parent = base.transform.Find("cg_view");
		Camera.main.transform.parent = parent;
		Camera.main.transform.localPosition = Vector3.zero;
		Camera.main.transform.localRotation = Quaternion.identity;
		GameSceneController.Instance.main_camera.camera_pause = true;
		GameSceneController.Instance.main_camera.StartShake("Wrestler_Camera_Bellow01");
		float length = GameSceneController.Instance.main_camera.shake_obj.GetComponent<Animation>()["Wrestler_Camera_Bellow01"].length;
		GameSceneController.Instance.player_controller.StartLimitMove(length);
		GameSceneController.Instance.StartCgLimit(length);
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			if (!value.IsBoss)
			{
				value.StartLimitMove(length);
			}
		}
	}

	private void PlayGroundStone()
	{
		ground_stone1 = Object.Instantiate(base.Accessory[0], base.transform.position, base.transform.rotation) as GameObject;
		ground_stone2 = Object.Instantiate(base.Accessory[1], base.transform.position, base.transform.rotation) as GameObject;
		AnimationUtil.PlayAnimate(ground_stone1, stone_ani1, WrapMode.Once);
		AnimationUtil.PlayAnimate(ground_stone2, stone_ani2, WrapMode.Once);
		Invoke("HideGroundStone", base.GetComponent<Animation>()[ANI_SHOW].length + 2f);
	}

	public override void OnHalfHpEffOver()
	{
		base.OnHalfHpEffOver();
		eff_bellow.Stop();
	}

	public void OnSharkRushReady()
	{
		if (nav_pather != null)
		{
			if (target_player != null)
			{
				rush_target = target_player.transform.position;
			}
			else
			{
				rush_target = base.transform.position;
			}
			nav_pather.SetTargetPosition(rush_target);
			nav_pather.SetSpeed(enemy_data.move_speed * rush_speed_ratio);
		}
		JustRushTo(true);
		SetPathCatchState(true);
	}

	public void OnSharkRushEnd()
	{
		if (nav_pather != null)
		{
			nav_pather.SetSpeed(enemy_data.move_speed);
		}
		JustRushTo(false);
		ShowSharkRushEff(false);
		ShowSharkDivingEff(false);
	}

	public bool OnRushUpdate(float deltaTime)
	{
		if ((rush_target - base.transform.position).sqrMagnitude < 1f)
		{
			return true;
		}
		return false;
	}

	public void OnSharkDiveReady()
	{
		if (nav_pather != null)
		{
			nav_pather.SetSpeed(enemy_data.move_speed * rush_speed_ratio);
		}
		ShowSharkFin(true);
		ShowSharkDivingEff(true);
		ShowSharkBody(false);
	}

	public void OnSharkDiveEnd()
	{
		if (nav_pather != null)
		{
			nav_pather.SetSpeed(enemy_data.move_speed);
		}
		ShowSharkFin(false);
		ShowSharkDivingEff(false);
		ShowSharkBody(true);
	}

	public void ShowSharkFin(bool status)
	{
		shark_fin.SetActive(status);
		base.GetComponent<Collider>().enabled = !status;
	}

	public void ShowSharkRushEff(bool status)
	{
		rush_eff_obj.SetActive(status);
	}

	public void ShowSharkDivingEff(bool status)
	{
		diving_eff_obj.SetActive(status);
	}

	public void ShowSharkDiveOutEff()
	{
		dive_out_eff_obj.SetActive(true);
		dive_out_eff_obj.GetComponent<ParticleSystem>().Stop();
		dive_out_eff_obj.GetComponent<ParticleSystem>().Play();
		Invoke("HideSharkDiveOutEff", 2f);
	}

	public void HideSharkDiveOutEff()
	{
		dive_out_eff_obj.GetComponent<ParticleSystem>().Stop();
		dive_out_eff_obj.SetActive(false);
	}

	public void ShowSharkBody(bool status)
	{
		foreach (GameObject item in rend_obj_list)
		{
			item.SetActive(status);
		}
	}

	public override void OnShowOver()
	{
		shark_fish.SetActive(true);
	}

	public void OnSharkHideWeapon()
	{
		shark_fish.SetActive(false);
	}
}
