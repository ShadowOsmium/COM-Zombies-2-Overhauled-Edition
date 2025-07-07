using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class WrestlerController : EnemyController
{
	private string ani_attack = "Attack01";

	private string ani_damage_1 = "Zombie_Wrestler_Damage01";

	private string ani_damage_2 = "Zombie_Wrestler_Damage02";

	private string ani_damage_3 = "Zombie_Wrestler_Damage03";

	public string ANI_WIND_01 = "Zombie_Wrestler_Attack02_01";

	public string ANI_WIND_02 = "Zombie_Wrestler_Attack02_02";

	public string ANI_WIND_03 = "Zombie_Wrestler_Attack02_03";

	public string ANI_RUSH_01 = "Zombie_Wrestler_Attack01_01";

	public string ANI_RUSH_02 = "Zombie_Wrestler_Attack01_02";

	public string ANI_RUSH_03 = "Zombie_Wrestler_Attack01_03";

	public string ANI_RUSH_04 = "Zombie_Wrestler_Attack01_04";

	public string ANI_RUSH_05 = "Zombie_Wrestler_Attack01_05";

	public string ANI_BELLOW_01 = "Zombie_Wrestler_Attack03_01";

	public string ANI_BELLOW_02 = "Zombie_Wrestler_Attack03_02";

	public string ANI_BELLOW_03 = "Zombie_Wrestler_Attack03_03";

	public EnemyState SKILL_WIND_STATE;

	public EnemyState SKILL_RUSH_STATE;

	public EnemyState SKILL_BELLOW_STATE;

	private float injured_val_state;

	private float injured_val_percent;

	private float rush_speed_ratio = 7f;

	private float rush_range = 10f;

	private float rush_range_min = 10f;

	private float cur_rush_time;

	private float rush_interval_time = 8f;

	private float rush_dmg_ratio = 1f;

	public float rush_time = 1f;

	private float rush_jump_range = 13f;

	private float rush_jump_range_min = 13f;

	public bool rush_enable = true;

	private float wind_speed_ratio = 6f;

	private float wind_range = 10f;

	private float cur_wind_time;

	private float wind_interval_time = 8f;

	private float wind_dmg_ratio = 1f;

	private float wind_dgm_range = 5f;

	public float wind_time;

	public bool wind_enable = true;

	private float cur_bellow_time;

	private float bellow_interval_time = 8f;

	private float bellow_dmg_ratio = 1f;

	private float bellow_dgm_range = 5f;

	public float bellow_time;

	public bool bellow_enable = true;

	private GameObject eff_wind;

	private GameObject eff_bellow_obj;

	private ParticleSystem eff_bellow;

	private GameObject eff_angry_obj;

	private ParticleSystem eff_angry;

	private GameObject eff_jump;

	private HaokeRushTriger rush_triger;

	private GameObject ground_stone1;

	private GameObject ground_stone2;

	private string stone_ani1 = string.Empty;

	private string stone_ani2 = string.Empty;

	private float cur_wind_logic_time;

	private float wind_logic_interval = 0.3f;

	private float cur_bellow_logic_time;

	private float bellow_logic_interval = 0.3f;

	private Vector3 rush_jump_target = Vector3.zero;

	private Vector3 launch_speed;

	public override Vector3 centroid
	{
		get
		{
			return base.transform.position + Vector3.up * 1.6f;
		}
	}

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public override void Init()
	{
		ANI_IDLE = "Zombie_Wrestler_Idle01";
		ANI_ATTACK = "Zombie_Wrestler_Attack01_01";
		ANI_INJURED = "Zombie_Wrestler_Damage01";
		ANI_DEAD = "Zombie_Wrestler_Death01";
		ANI_RUN = "Zombie_Wrestler_Run01";
		ANI_SHOW = "Zombie_Wrestler_Show01";
		ANI_HALF_HP = "Zombie_Wrestler_Bellow01";
		stone_ani1 = "01";
		stone_ani2 = "01";
		float time = 0f;
		if (base.IsBoss)
		{
			if (GameSceneController.Instance.enable_spawn_ani == "Wrestler_Camera_Show01")
			{
				ANI_SHOW = "Zombie_Wrestler_Show01";
				time = 0.4f;
				stone_ani1 = "Zombie_FatCook01";
				stone_ani2 = "Zombie_FatCook01";
			}
			else if (GameSceneController.Instance.enable_spawn_ani == "Wrestler_Camera_Show02")
			{
				ANI_SHOW = "Zombie_Wrestler_Show02";
				time = 1.8f;
				stone_ani1 = "01";
				stone_ani2 = "01";
			}
			CreateHpBar(Vector3.up * 3f);
		}
		Invoke("PlayGroundStone", time);
		base.Init();
		SKILL_WIND_STATE = EnemyState.Create(EnemyStateType.WrestlerWind, this);
		SKILL_RUSH_STATE = EnemyState.Create(EnemyStateType.WrestlerRush, this);
		SKILL_BELLOW_STATE = EnemyState.Create(EnemyStateType.WrestlerBellow, this);
		rush_speed_ratio = (float)enemy_data.config.Ex_conf["rushSpeedRatio"];
		rush_range = (float)enemy_data.config.Ex_conf["rushRange"];
		rush_range_min = (float)enemy_data.config.Ex_conf["rushRangeMin"];
		rush_interval_time = (float)enemy_data.config.Ex_conf["rushCDTime"];
		rush_dmg_ratio = (float)enemy_data.config.Ex_conf["rushDmgRatio"];
		rush_time = (float)enemy_data.config.Ex_conf["rushTime"];
		rush_jump_range = (float)enemy_data.config.Ex_conf["rushJumpRange"];
		rush_jump_range_min = (float)enemy_data.config.Ex_conf["rushJumpRangeMin"];
		wind_speed_ratio = (float)enemy_data.config.Ex_conf["windSpeedRatio"];
		wind_range = (float)enemy_data.config.Ex_conf["windRange"];
		wind_interval_time = (float)enemy_data.config.Ex_conf["windCDTime"];
		wind_dmg_ratio = (float)enemy_data.config.Ex_conf["windDmgRatio"];
		wind_time = (float)enemy_data.config.Ex_conf["windTime"];
		bellow_interval_time = (float)enemy_data.config.Ex_conf["bellowCDTime"];
		bellow_dmg_ratio = (float)enemy_data.config.Ex_conf["bellowDmgRatio"];
		bellow_dgm_range = (float)enemy_data.config.Ex_conf["bellowDmgRange"];
		bellow_time = (float)enemy_data.config.Ex_conf["bellowTime"];
		injured_val_percent = (float)enemy_data.config.Ex_conf["injuredPercent"];
		GameObject gameObject = base.transform.Find("Dummy01").gameObject;
		rush_triger = gameObject.GetComponent<HaokeRushTriger>();
		rush_triger.on_rushed = OnRushTarget;
		eff_wind = base.transform.Find("xuanzhuan_01").gameObject;
		eff_wind.SetActive(false);
		eff_jump = base.transform.Find("haoke_attack_04_a_01").gameObject;
		eff_jump.SetActive(false);
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
		if (enemyState == SKILL_WIND_STATE)
		{
			foreach (PlayerController value in GameSceneController.Instance.Player_Set.Values)
			{
				if ((value.centroid - centroid).sqrMagnitude < wind_dgm_range * wind_dgm_range && !GameSceneController.CheckBlockBetween(centroid, value.centroid))
				{
					value.OnHit(enemy_data.damage_val * wind_dmg_ratio, null, this, value.centroid, Vector3.up);
				}
			}
			foreach (NPCController value2 in GameSceneController.Instance.NPC_Set.Values)
			{
				if ((value2.centroid - centroid).sqrMagnitude < wind_dgm_range * wind_dgm_range && !GameSceneController.CheckBlockBetween(centroid, value2.centroid))
				{
					value2.OnHit(enemy_data.damage_val * wind_dmg_ratio, null, this, value2.centroid, Vector3.up);
				}
			}
			foreach (GuardianForceController value3 in GameSceneController.Instance.GuardianForce_Set.Values)
			{
				if ((value3.centroid - centroid).sqrMagnitude < wind_dgm_range * wind_dgm_range && !GameSceneController.CheckBlockBetween(centroid, value3.centroid))
				{
					value3.OnHit(enemy_data.damage_val, null, this, value3.centroid, Vector3.up);
				}
			}
			{
				foreach (EnemyController value4 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
				{
					if ((value4.centroid - centroid).sqrMagnitude < wind_dgm_range * wind_dgm_range && !GameSceneController.CheckBlockBetween(centroid, value4.centroid))
					{
						value4.OnHit(enemy_data.damage_val, null, this, value4.centroid, Vector3.zero);
					}
				}
				return;
			}
		}
		if (enemyState == SKILL_BELLOW_STATE)
		{
			foreach (PlayerController value5 in GameSceneController.Instance.Player_Set.Values)
			{
				if ((value5.centroid - centroid).sqrMagnitude < bellow_dgm_range * bellow_dgm_range && !GameSceneController.CheckBlockBetween(centroid, value5.centroid))
				{
					value5.OnHit(enemy_data.damage_val * bellow_dmg_ratio, null, this, value5.centroid, Vector3.up);
				}
			}
			foreach (NPCController value6 in GameSceneController.Instance.NPC_Set.Values)
			{
				if ((value6.centroid - centroid).sqrMagnitude < bellow_dgm_range * bellow_dgm_range && !GameSceneController.CheckBlockBetween(centroid, value6.centroid))
				{
					value6.OnHit(enemy_data.damage_val * bellow_dmg_ratio, null, this, value6.centroid, Vector3.up);
				}
			}
			foreach (GuardianForceController value7 in GameSceneController.Instance.GuardianForce_Set.Values)
			{
				if ((value7.centroid - centroid).sqrMagnitude < bellow_dgm_range * bellow_dgm_range && !GameSceneController.CheckBlockBetween(centroid, value7.centroid))
				{
					value7.OnHit(enemy_data.damage_val * bellow_dmg_ratio, null, this, value7.centroid, Vector3.up);
				}
			}
			{
				foreach (EnemyController value8 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
				{
					if ((value8.centroid - centroid).sqrMagnitude < bellow_dgm_range * bellow_dgm_range && !GameSceneController.CheckBlockBetween(centroid, value8.centroid))
					{
						value8.OnHit(enemy_data.damage_val * bellow_dmg_ratio, null, this, value8.centroid, Vector3.up);
					}
				}
				return;
			}
		}
		if (enemyState != SHOOT_STATE)
		{
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
		if (!wind_enable)
		{
			cur_wind_time += deltaTime;
			if (cur_wind_time >= wind_interval_time)
			{
				cur_wind_time = 0f;
				wind_enable = true;
			}
		}
		if (!rush_enable)
		{
			cur_rush_time += deltaTime;
			if (cur_rush_time >= rush_interval_time)
			{
				cur_rush_time = 0f;
				rush_enable = true;
			}
		}
		if (!bellow_enable)
		{
			cur_bellow_time += deltaTime;
			if (cur_bellow_time >= bellow_interval_time)
			{
				cur_bellow_time = 0f;
				bellow_enable = true;
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
		else if (CouldEnterBellowState())
		{
			SetState(SKILL_BELLOW_STATE);
		}
		else if (CouldEnterRushState())
		{
			SetState(SKILL_RUSH_STATE);
		}
		else if (CouldEnterWindState())
		{
			SetState(SKILL_WIND_STATE);
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
				default:
					ANI_INJURED = ani_damage_3;
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

	public bool CouldEnterWindState()
	{
		if (wind_enable && base.SqrDistanceFromPlayer <= wind_range * wind_range && target_player != null && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public bool CouldEnterRushState()
	{
		if (rush_enable && base.SqrDistanceFromPlayer <= rush_range * rush_range && base.SqrDistanceFromPlayer >= rush_range_min * rush_range_min && target_player != null && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public bool CouldEnterBellowState()
	{
		if (bellow_enable)
		{
			return true;
		}
		return false;
	}

	public void OnWindUpdate(float deltaTime)
	{
		cur_wind_logic_time += deltaTime;
		if (cur_wind_logic_time >= wind_logic_interval)
		{
			cur_wind_logic_time = 0f;
			CheckHit();
		}
	}

	public void OnBellowUpdate(float deltaTime)
	{
		cur_bellow_logic_time += deltaTime;
		if (cur_bellow_logic_time >= bellow_logic_interval)
		{
			cur_bellow_logic_time = 0f;
			CheckHit();
		}
	}

	public void OnWrestlerBellowReady()
	{
		cur_bellow_logic_time = bellow_logic_interval;
		eff_bellow.Play();
	}

	public void OnWrestlerBellowOver()
	{
		eff_bellow.Stop();
	}

	public void OnWrestlerWindReady()
	{
		if (nav_pather != null)
		{
			nav_pather.SetSpeed(enemy_data.move_speed * wind_speed_ratio);
		}
		SetPathCatchState(true);
		cur_wind_logic_time = wind_logic_interval;
		eff_wind.SetActive(true);
		eff_wind.GetComponent<EffectAudioBehaviour>().PlayEffect();
	}

	public void OnWrestlerWindOver()
	{
		if (nav_pather != null)
		{
			nav_pather.SetSpeed(enemy_data.move_speed);
		}
		SetPathCatchState(false);
		eff_wind.SetActive(false);
	}

	public void OnWrestlerRushReady()
	{
		if (nav_pather != null)
		{
			nav_pather.SetSpeed(enemy_data.move_speed * rush_speed_ratio);
		}
		SetPathCatchState(true);
		rush_triger.enable_triger = true;
	}

	public void OnWrestlerRushOver()
	{
		if (nav_pather != null)
		{
			nav_pather.SetSpeed(enemy_data.move_speed);
		}
		SetPathCatchState(false);
		rush_triger.enable_triger = false;
	}

	public bool CheckEnableRushJump()
	{
		if (base.SqrDistanceFromPlayer <= rush_jump_range * rush_jump_range && base.SqrDistanceFromPlayer >= rush_jump_range_min * rush_jump_range_min && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public bool CheckRushJumpTooClose()
	{
		if (base.SqrDistanceFromPlayer < rush_jump_range_min * rush_jump_range_min && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public void ResetRushJumpTarget()
	{
		rush_jump_target = target_player.transform.position;
	}

	public float OnStartJump()
	{
		eff_jump.SetActive(true);
		nav_pather.StopNavMeshAgent();
		base.GetComponent<Rigidbody>().useGravity = false;
		base.GetComponent<Rigidbody>().isKinematic = true;
		float num = 0f;
		Vector3 vector = rush_jump_target - base.transform.position;
		float num2 = vector.magnitude - 1f;
		float num3 = 10f;
		float num4 = num2 / num3;
		float num5 = (num - 0.5f * Physics.gravity.y * num4 * num4) / num4;
		launch_speed = Vector3.up * num5 + vector.normalized * num3;
		return num4;
	}

	public void OnStopJump()
	{
		eff_jump.SetActive(false);
		nav_pather.PlayNavMeshAgent();
		base.GetComponent<Rigidbody>().useGravity = false;
		base.GetComponent<Rigidbody>().isKinematic = true;
	}

	public void OnRushJumpUpdate(float deltaTime)
	{
		launch_speed += Physics.gravity.y * Vector3.up * deltaTime;
		base.transform.Translate(launch_speed * deltaTime, Space.World);
	}

	public bool CheckJumpAlmostFinish()
	{
		if ((base.transform.position - rush_jump_target).magnitude <= 1f)
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

	public void OnRushTarget(ObjectController player)
	{
		player.OnHit(enemy_data.damage_val * rush_dmg_ratio, null, this, player.centroid, Vector3.up);
	}

	public override void PlayHalfHpEffect()
	{
		base.PlayHalfHpEffect();
		eff_bellow.Play();
		eff_angry.Play();
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
}
