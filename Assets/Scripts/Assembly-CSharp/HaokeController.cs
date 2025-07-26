using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class HaokeController : EnemyController
{
	private string ani_summon = "Attack03";

	private string ani_attack = "Attack01";

	private string ani_damage_run = "Damage01";

	private string ani_damage_big = "Damage02";

	public string ANI_RUSH_01 = "Attack02_a";

	public string ANI_RUSH_02 = "Attack02_b";

	public string ANI_RUSH_03 = "Attack02_c1";

	public string ANI_RUSH_04 = "Attack02_c2";

	public string ANI_TANK_01 = "Attack04_a";

	public string ANI_TANK_02 = "Attack04_b";

	public string ANI_TANK_03 = "Attack04_c1";

	public string ANI_TANK_04 = "Attack04_c2";

	protected Collider attackCollider_L;

	protected Collider attackCollider_R;

	protected Collider attackCollider_C;

	public EnemyState SKILL_RUSH_STATE;

	public EnemyState SKILL_SUMMON_STATE;

	public EnemyState SKILL_TANK_STATE;

	private float injured_val_state;

	private float injured_val_percent;

	private float summon_range = 10f;

	private float cur_summon_time;

	private float summon_interval_time = 6f;

	private float summon_dmg_ratio = 1f;

	public bool summon_enable = true;

	private float rush_speed_ratio = 5f;

	private float rush_range = 10f;

	private float cur_rush_time;

	private float rush_interval_time = 8f;

	private float rush_dmg_ratio = 1f;

	public bool rush_enable = true;

	private float tank_range = 10f;

	private float cur_tank_time;

	private float tank_interval_time = 10f;

	private float tank_dmg_ratio = 1f;

	public bool tank_enable;

	private Vector3 fire_target = Vector3.zero;

	private Vector3 rush_target = Vector3.zero;

	private Vector3 tank_target = Vector3.zero;

	private Transform left_hand;

	private GameObject summon_obj;

	private bool is_summon_ready;

	private HaokeRushTriger rush_triger;

	private HaokeRushTriger tank_triger;

	private GameObject eff_attack;

	private GameObject eff_rush_ready1;

	private GameObject eff_rush_ready2;

	private GameObject eff_rush;

	private GameObject eff_summon;

	private GameObject eff_tank;

	private GameObject eff_run;

	private GameObject eff_summon_ground;

	private GameObject ground_stone1;

	private GameObject ground_stone2;

	private GameObject eff_bellow_obj;

	private ParticleSystem eff_bellow;

	private GameObject eff_angry_obj;

	private ParticleSystem eff_angry;

	private string stone_ani1 = string.Empty;

	private string stone_ani2 = string.Empty;

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
		ANI_IDLE = "Idle01";
		ANI_ATTACK = "Attack01";
		ANI_INJURED = "Damage01";
		ANI_DEAD = "Death01";
		ANI_RUN = "Run01";
		ANI_SHOW = "Show02";
		ANI_HALF_HP = "Bellow01";
		if (base.IsBoss)
		{
			if (GameSceneController.Instance.enable_spawn_ani == "Haoke_Camera_Show01")
			{
				ANI_SHOW = "Show01";
			}
			else if (GameSceneController.Instance.enable_spawn_ani == "Haoke_Camera_Show02")
			{
				ANI_SHOW = "Show02";
			}
			CreateHpBar(Vector3.up * 3.4f);
		}
		if (ANI_SHOW == "Show02")
		{
			stone_ani1 = "Zombie_Haoke02";
			stone_ani2 = "Zombie_Haoke02";
		}
		else
		{
			stone_ani1 = "01";
			stone_ani2 = "01";
		}
		PlayGroundStone();
		base.Init();
		SKILL_RUSH_STATE = EnemyState.Create(EnemyStateType.Haoke_Rush, this);
		SKILL_SUMMON_STATE = EnemyState.Create(EnemyStateType.Haoke_Summon, this);
		SKILL_TANK_STATE = EnemyState.Create(EnemyStateType.Haoke_Tank, this);
		attackCollider_L = base.transform.Find("Dummy03/Dummy02/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Dummy_Head_P/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand").gameObject.GetComponent<Collider>();
		left_hand = attackCollider_L.gameObject.transform;
		rush_triger = attackCollider_L.GetComponent<HaokeRushTriger>();
		rush_triger.on_rushed = OnHaokeRushTarget;
		if (attackCollider_L == null)
		{
			Debug.LogError("attack_l collider not founded!");
		}
		attackCollider_R = base.transform.Find("Dummy03/Dummy02/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Dummy_Head_P/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand").gameObject.GetComponent<Collider>();
		if (attackCollider_R == null)
		{
			Debug.LogError("attack_r collider not founded!");
		}
		attackCollider_C = base.transform.Find("Dummy01").gameObject.GetComponent<Collider>();
		tank_triger = attackCollider_C.GetComponent<HaokeRushTriger>();
		tank_triger.on_rushed = OnHaokeTankTarget;
		if (attackCollider_C == null)
		{
			Debug.LogError("attackCollider_C collider not founded!");
		}
		summon_range = (float)enemy_data.config.Ex_conf["summonRange"];
		summon_interval_time = (float)enemy_data.config.Ex_conf["summonCDTime"];
		summon_dmg_ratio = (float)enemy_data.config.Ex_conf["summonDmgRatio"];
		rush_speed_ratio = (float)enemy_data.config.Ex_conf["rushSpeedRatio"];
		rush_range = (float)enemy_data.config.Ex_conf["rushRange"];
		rush_interval_time = (float)enemy_data.config.Ex_conf["rushCDTime"];
		rush_dmg_ratio = (float)enemy_data.config.Ex_conf["rushDmgRatio"];
		tank_range = (float)enemy_data.config.Ex_conf["tankRange"];
		tank_interval_time = (float)enemy_data.config.Ex_conf["tankCDTime"];
		tank_dmg_ratio = (float)enemy_data.config.Ex_conf["tankDmgRatio"];
		injured_val_percent = (float)enemy_data.config.Ex_conf["injuredPercent"];
		base.GetComponent<Animation>()[ANI_TANK_02].speed = 2f;
		eff_attack = base.transform.Find("haoke_attack_01_01").gameObject;
		eff_attack.SetActive(false);
		eff_rush_ready1 = base.transform.Find("haoke_attack_02_a_01").gameObject;
		eff_rush_ready1.SetActive(false);
		eff_rush_ready2 = base.transform.Find("haoke_attack_02_a_02").gameObject;
		eff_rush_ready2.SetActive(false);
		eff_rush = base.transform.Find("haoke_attack_02_b/haoke_attack_02_b_01_01").gameObject;
		eff_rush.SetActive(false);
		eff_summon = base.transform.Find("haoke_attack_03").gameObject;
		eff_summon.SetActive(false);
		eff_tank = base.transform.Find("haoke_attack_04_a_01").gameObject;
		eff_tank.SetActive(false);
		eff_run = base.transform.Find("haoke_run").gameObject;
		eff_run.SetActive(false);
		eff_summon_ground = base.transform.Find("liehen").gameObject;
		eff_summon_ground.SetActive(false);
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
		if (enemyState == SKILL_SUMMON_STATE)
		{
			summon_obj.transform.parent = null;
			float num = 0f;
			Vector3 vector = fire_target - summon_obj.transform.position;
			float magnitude = vector.magnitude;
			float num2 = 12f;
			float num3 = magnitude / num2;
			float num4 = (num - 0.5f * Physics.gravity.y * num3 * num3) / num3;
			Vector3 vector2 = Vector3.up * num4 + vector.normalized * num2;
			HaokeProjectile component = summon_obj.GetComponent<HaokeProjectile>();
			component.launch_dir = vector2;
			component.launch_speed = vector2;
			component.explode_radius = 2f;
			component.damage = enemy_data.damage_val * summon_dmg_ratio;
			component.object_controller = this;
			component.is_ready = true;
		}
		else
		{
			if (enemyState != SHOOT_STATE)
			{
				return;
			}
			foreach (PlayerController value in GameSceneController.Instance.Player_Set.Values)
			{
				Collider collider = value.gameObject.GetComponent<Collider>();
				if (collider != null && attackCollider_R.bounds.Intersects(collider.bounds))
				{
					value.OnHit(enemy_data.damage_val, null, this, value.centroid, Vector3.up);
				}
			}
			foreach (NPCController value2 in GameSceneController.Instance.NPC_Set.Values)
			{
				Collider collider2 = value2.gameObject.GetComponent<Collider>();
				if (collider2 != null && attackCollider_R.bounds.Intersects(collider2.bounds))
				{
					value2.OnHit(enemy_data.damage_val, null, this, value2.centroid, Vector3.up);
				}
			}
			foreach (GuardianForceController value3 in GameSceneController.Instance.GuardianForce_Set.Values)
			{
				Collider collider3 = value3.gameObject.GetComponent<Collider>();
				if (collider3 != null && attackCollider_R.bounds.Intersects(collider3.bounds))
				{
					value3.OnHit(enemy_data.damage_val, null, this, value3.centroid, Vector3.up);
				}
			}
			foreach (EnemyController value4 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
			{
				Collider collider4 = value4.gameObject.GetComponent<Collider>();
				if (collider4 != null && attackCollider_R.bounds.Intersects(collider4.bounds))
				{
					value4.OnHit(enemy_data.damage_val, null, this, value4.centroid, Vector3.zero);
				}
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
		else if (AnimationUtil.IsPlayingAnimation(base.gameObject, ani_summon) && !is_summon_ready)
		{
			AlwaysLookAtTarget();
		}
		if (AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ANI_ATTACK, 1f))
		{
			is_summon_ready = false;
			SetState(AFTER_SHOOT_STATE);
		}
	}

	public override void Fire()
	{
		ANI_CUR_ATTACK = (ANI_ATTACK = ani_attack);
		AnimationUtil.CrossAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
		PlayEffAttack();
	}

	public override void EnterAttack()
	{
		Fire();
	}

	public void EnterSummonAttack()
	{
		summon_enable = false;
		ANI_CUR_ATTACK = (ANI_ATTACK = ani_summon);
		AnimationUtil.CrossAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
		PlayEffSummon();
	}

	public override void OnDead(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		base.OnDead(damage, weapon, player, hit_point, hit_normal);
		Object.Destroy(base.GetComponent<Collider>());
		StartCoroutine(RemoveOnTime(3f));
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		rush_triger.on_rushed = null;
		tank_triger.on_rushed = null;
	}

	public override void Dologic(float deltaTime)
	{
		base.Dologic(deltaTime);
		if (!summon_enable)
		{
			cur_summon_time += deltaTime;
			if (cur_summon_time >= summon_interval_time)
			{
				cur_summon_time = 0f;
				summon_enable = true;
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
		if (!tank_enable)
		{
			cur_tank_time += deltaTime;
			if (cur_tank_time >= tank_interval_time)
			{
				cur_tank_time = 0f;
				tank_enable = true;
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
		else if (CouldEnterAttackState())
		{
			SetState(SHOOT_STATE);
		}
		else if (CouldEnterRushState())
		{
			SetState(SKILL_RUSH_STATE);
		}
		else if (CouldEnterSummonState())
		{
			SetState(SKILL_SUMMON_STATE);
		}
		else if (CouldEnterTankState())
		{
			SetState(SKILL_TANK_STATE);
		}
		else
		{
			SetState(CATCHING_STATE);
		}
		if (base.Enemy_State.GetStateType() == EnemyStateType.Catching)
		{
			eff_run.SetActive(true);
		}
		else
		{
			eff_run.SetActive(false);
		}
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		if (enemyState == SHOW_STATE || enemyState == SKILL_TANK_STATE || enemyState == DEAD_STATE || base.GetComponent<Rigidbody>().useGravity)
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
				if (base.Enemy_State.GetStateType() != EnemyStateType.Injured)
				{
					if (Random.Range(0, 100) >= 50)
					{
						ANI_INJURED = ani_damage_run;
					}
					else
					{
						ANI_INJURED = ani_damage_big;
					}
					if (!AnimationUtil.IsPlayingAnimation(base.gameObject, ANI_INJURED))
					{
						AnimationUtil.CrossAnimate(base.gameObject, ANI_INJURED, WrapMode.ClampForever);
					}
				}
				SetState(INJURED_STATE);
			}
		}
		CreateInjuredBloodEff(hit_point, hit_normal);
		UpdateCurHpBar();
	}

	public bool CouldEnterSummonState()
	{
		if (summon_enable && base.SqrDistanceFromPlayer <= summon_range * summon_range && base.SqrDistanceFromPlayer > enemy_data.attack_range * enemy_data.attack_range && target_player != null && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public bool CouldEnterRushState()
	{
		if (enemy_data.cur_hp / enemy_data.hp_capacity <= 0.5f && rush_enable && base.SqrDistanceFromPlayer <= rush_range * rush_range && base.SqrDistanceFromPlayer > enemy_data.attack_range * enemy_data.attack_range && target_player != null && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public bool CouldEnterTankState()
	{
		if (tank_enable && base.SqrDistanceFromPlayer < tank_range * tank_range && base.SqrDistanceFromPlayer > enemy_data.attack_range * enemy_data.attack_range && target_player != null && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
		{
			return true;
		}
		return false;
	}

	public void OnHaokeRushReady()
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
		rush_triger.enable_triger = true;
	}

	public void OnHaokeTankReady()
	{
		if (nav_pather != null)
		{
			OnStopJump();
			if (target_player != null)
			{
				tank_target = target_player.transform.position;
			}
			else
			{
				tank_target = base.transform.position;
			}
			nav_pather.SetTargetPosition(tank_target);
			nav_pather.SetSpeed(enemy_data.move_speed * rush_speed_ratio);
		}
		JustRushTo(true);
		SetPathCatchState(true);
		tank_triger.enable_triger = true;
		PlayEffTank();
	}

	public void OnHaokeTankStart()
	{
		if (nav_pather != null)
		{
			OnStartJump();
			base.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up * 10f, base.transform.position, ForceMode.VelocityChange);
		}
	}

	public override void FixedUpdate()
	{
	}

	public void OnHaokeRushTarget(ObjectController player)
	{
		rush_triger.enable_triger = false;
		player.OnHit(enemy_data.damage_val * rush_dmg_ratio, null, this, player.centroid, Vector3.up);
	}

	public void OnHaokeTankTarget(ObjectController player)
	{
		tank_triger.enable_triger = false;
		player.OnHit(enemy_data.damage_val * tank_dmg_ratio, null, this, player.centroid, Vector3.up);
	}

	public void OnHaokeRushEnd()
	{
		if (nav_pather != null)
		{
			nav_pather.SetSpeed(enemy_data.move_speed);
		}
		JustRushTo(false);
		rush_triger.enable_triger = false;
		StopEffRush();
	}

	public void OnHaokeTankEnd()
	{
		if (nav_pather != null)
		{
			OnStopJump();
			nav_pather.SetSpeed(enemy_data.move_speed);
		}
		JustRushTo(false);
		tank_triger.enable_triger = false;
		StopEffTank();
	}

	public void OnHaokeSummonReady()
	{
		summon_obj = Object.Instantiate(base.Accessory[0], left_hand.transform.position, Quaternion.identity) as GameObject;
		summon_obj.transform.parent = left_hand;
		summon_obj.transform.localPosition = Vector3.zero;
		summon_obj.transform.localRotation = Quaternion.identity;
		if (target_player != null)
		{
			fire_target = target_player.centroid;
		}
		else
		{
			fire_target = centroid;
		}
		is_summon_ready = true;
	}

	public bool OnRushUpdate(float deltaTime)
	{
		if ((rush_target - base.transform.position).sqrMagnitude < 1f)
		{
			return true;
		}
		return false;
	}

	public bool OnTankUpdate(float deltaTime)
	{
		if ((tank_target - base.transform.position).sqrMagnitude < 1f)
		{
			return true;
		}
		return false;
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

	public void OnStartJump()
	{
		nav_pather.StopNavMeshAgent();
		base.GetComponent<Rigidbody>().useGravity = true;
		base.GetComponent<Rigidbody>().isKinematic = false;
	}

	public void OnStopJump()
	{
		nav_pather.PlayNavMeshAgent();
		base.GetComponent<Rigidbody>().useGravity = false;
		base.GetComponent<Rigidbody>().isKinematic = true;
	}

	public void OnTankSkillOver()
	{
		OnStartJump();
		SetPathCatchState(false);
		base.GetComponent<Rigidbody>().AddForceAtPosition(-base.transform.forward * 5f + Vector3.up * 7f, base.transform.position, ForceMode.VelocityChange);
		StopEffTank();
	}

	public void PlayEffAttack()
	{
		eff_attack.SetActive(true);
		eff_attack.GetComponent<ParticleSystem>().Play();
		eff_attack.GetComponent<EffectAudioBehaviour>().PlayEffect();
	}

	public void PlayEffRushReady()
	{
		eff_rush_ready1.SetActive(true);
		eff_rush_ready1.GetComponent<ParticleSystem>().Play();
		eff_rush_ready1.GetComponent<EffectAudioBehaviour>().PlayEffect();
		eff_rush_ready2.SetActive(true);
		eff_rush_ready2.GetComponent<ParticleSystem>().Play();
		eff_rush_ready2.GetComponent<EffectAudioBehaviour>().PlayEffect();
	}

	public void PlayEffRush()
	{
		eff_rush.SetActive(true);
		eff_rush.GetComponent<ParticleSystem>().Play();
		eff_rush.GetComponent<EffectAudioBehaviour>().PlayEffect();
	}

	public void StopEffRush()
	{
		eff_rush.GetComponent<ParticleSystem>().Stop();
		Invoke("CleanEffRush", 0.2f);
	}

	public void PlayEffSummon()
	{
		eff_summon.SetActive(true);
		eff_summon.GetComponent<ParticleSystem>().Play();
		eff_summon.GetComponent<EffectAudioBehaviour>().PlayEffect();
	}

	public void PlayEffSummonGround()
	{
		eff_summon_ground.SetActive(true);
		eff_summon_ground.GetComponent<ParticleSystem>().Play();
	}

	public void StopEffSummon()
	{
		eff_summon.GetComponent<ParticleSystem>().Stop();
		eff_summon.SetActive(false);
		eff_summon_ground.GetComponent<ParticleSystem>().Stop();
		eff_summon_ground.SetActive(false);
	}

	public void PlayEffTank()
	{
		eff_tank.SetActive(true);
		eff_tank.GetComponent<ParticleSystem>().Play();
		eff_tank.GetComponent<EffectAudioBehaviour>().PlayEffect();
	}

	public void StopEffTank()
	{
		eff_tank.GetComponent<ParticleSystem>().Stop();
		Invoke("CleanEffTank", 0.2f);
	}

	private void CleanEffRush()
	{
		eff_rush.GetComponent<ParticleSystem>().Clear();
		eff_rush.SetActive(false);
	}

	private void CleanEffTank()
	{
		eff_tank.GetComponent<ParticleSystem>().Clear();
		eff_tank.SetActive(false);
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
		GameSceneController.Instance.main_camera.StartShake("Haoke_Camera_Bellow01");
		float length = GameSceneController.Instance.main_camera.shake_obj.GetComponent<Animation>()["Haoke_Camera_Bellow01"].length;
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
		ground_stone1 = Object.Instantiate(base.Accessory[1], base.transform.position, base.transform.rotation) as GameObject;
		ground_stone2 = Object.Instantiate(base.Accessory[2], base.transform.position, base.transform.rotation) as GameObject;
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
