using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class FatCookController : EnemyController
{
	private string ani_injure00 = "FatDamage";

	private string ani_injure01 = "Damage01";

	private string ani_injure02 = "Damage02";

	private string ani_injure03 = "Damage03";

	private float injured_ani_val = 0.1f;

	public string ANI_SKILL_ATTACK = "SkillAttack01";

	public string ANI_SKILL_FOCUS = "Skill01_01";

	public string ANI_SKILL_COMBO = "Skill01_02";

	public string ANI_SKILL_COMBO_END = "Skill01_03";

	public string ANI_SKILL_REST = "Skill01_04";

	public string ANI_SKILL_SUMMON = "Skill02";

	public string ANI_SKILL_MIANTUAN = "Skill03";

	public string ANI_IDLE_01 = "Idle01";

	public string ANI_IDLE_02 = "Idle02";

	public string ANI_IDLE_03 = "Fall_Back01";

    private BossCoopMissionController coopMission;

    private BossMissionController mission;

    protected Collider attackCollider;

	protected Transform miantuanHand;

	private Transform spine;

	private GameObject scart;

	public EnemyState SKILL_ATTACK_COMBO_STATE;

	public EnemyState SKILL_MIANTUAN_STATE;

	public EnemyState SKILL_SUMMON_STATE;

	public EnemyState AFTER_ATTACK_STATE;

	private float damage_ratio_combo = 1f;

	private float combo_viewRange = 5f;

	private float miantuan_viewRange = 7f;

	private float miantuan_flySpeed = 10f;

	private float avatarFreezeDuration = 180f;

	private float CD_normalAttack = 1f;

	private float CD_comboAttack = 1f;

	private float CD_miantuanAttack = 1f;

	private float curTime_normalAttack;

	private float curTime_comboAttack;

	private float curTime_miantuanAttack;

	private bool enable_normalAttack;

	private bool enable_comboAttack;

	private bool enable_miantuanAttack;

	private GameObject eff_ring;

	private GameObject eff_combo1;

	private GameObject eff_combo2;

	private GameObject eff_summon;

	private GameObject eff_miantuan;

	private GameObject ground_stone1;

	private GameObject ground_stone2;

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
        var mc = GameSceneController.Instance.mission_controller;

        coopMission = mc as BossCoopMissionController;
        mission = mc as BossMissionController;

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
		ANI_HALF_HP = "Bellow01";
		if (base.IsBoss)
		{
			if (GameSceneController.Instance.enable_spawn_ani == "Boss_FatCook_Camera_Show01")
			{
				ANI_SHOW = "Show01";
			}
			else if (GameSceneController.Instance.enable_spawn_ani == "Boss_FatCook_Camera_Show02")
			{
				ANI_SHOW = "Show02";
			}
			CreateHpBar(Vector3.up * 3.4f);
		}
		if (ANI_SHOW == "Show01")
		{
			stone_ani1 = "Zombie_FatCook01";
			stone_ani2 = "Zombie_FatCook01";
		}
        coopMission = GameSceneController.Instance.mission_controller as BossCoopMissionController;
        PlayGroundStone();
		base.Init();
		SKILL_ATTACK_COMBO_STATE = EnemyState.Create(EnemyStateType.FatCook_Combo, this);
		SKILL_MIANTUAN_STATE = EnemyState.Create(EnemyStateType.FatCook_Miantuan, this);
		SKILL_SUMMON_STATE = EnemyState.Create(EnemyStateType.FatCook_Summon, this);
		AFTER_ATTACK_STATE = EnemyState.Create(EnemyStateType.FatCook_AfterAttack, this);
		attackCollider = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 Prop1").gameObject.GetComponent<Collider>();
		if (attackCollider == null)
		{
			Debug.LogError("attack collider not founded!");
		}
		spine = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Dummy Head00");
		if (spine == null)
		{
			Debug.LogError("fatcook spine not founded!");
		}
		miantuanHand = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand");
		if (miantuanHand == null)
		{
			Debug.LogError("fatcook left hand for miantuan not found");
		}
		scart = spine.Find("Zombie_FatCook_H_Scart_Skin").gameObject;
		if (scart == null)
		{
			Debug.LogError("scart not found");
		}
		((FatCookComboState)SKILL_ATTACK_COMBO_STATE).combo_time = (float)enemy_data.config.Ex_conf["comboAttackTime"];
		((FatCookComboState)SKILL_ATTACK_COMBO_STATE).combo_rest_time = (float)enemy_data.config.Ex_conf["comboRestTime"];
		damage_ratio_combo = (float)enemy_data.config.Ex_conf["damageRatioCombo"];
		combo_viewRange = (float)enemy_data.config.Ex_conf["comboViewRange"];
		miantuan_viewRange = (float)enemy_data.config.Ex_conf["miantuanViewRange"];
		miantuan_flySpeed = (float)enemy_data.config.Ex_conf["miantuanSpeed"];
		avatarFreezeDuration = (float)enemy_data.config.Ex_conf["CDAvatarFreeze"];
		CD_normalAttack = (float)enemy_data.config.Ex_conf["CDNormalAttack"];
		CD_comboAttack = (float)enemy_data.config.Ex_conf["CDComboAttack"];
		CD_miantuanAttack = (float)enemy_data.config.Ex_conf["CDMiantuan"];
		base.GetComponent<Animation>()[ani_injure00].layer = 1;
		base.GetComponent<Animation>()[ani_injure00].AddMixingTransform(spine);
		eff_ring = base.transform.Find("fatcook_skill01_ring_01").gameObject;
		eff_ring.GetComponent<ParticleSystem>().Stop();
		eff_combo1 = base.transform.Find("light_01").gameObject;
		eff_combo1.SetActive(false);
		eff_combo2 = base.transform.Find("light_04").gameObject;
		eff_combo2.SetActive(false);
		eff_summon = base.transform.Find("fatcook_skill02_04").gameObject;
		eff_summon.SetActive(false);
		eff_miantuan = base.transform.Find("miantuan_intro_01").gameObject;
		eff_miantuan.SetActive(false);
		eff_angry_obj = base.transform.Find("crazy_boss").gameObject;
		eff_angry = eff_angry_obj.GetComponent<ParticleSystem>();
		eff_angry.Stop();
		SetState(SHOW_STATE);
	}

	public override void CheckHit()
	{
		float num = enemy_data.damage_val;
		if (enemyState == SKILL_ATTACK_COMBO_STATE)
		{
			num *= damage_ratio_combo;
		}
		foreach (PlayerController value in GameSceneController.Instance.Player_Set.Values)
		{
			Collider collider = value.gameObject.GetComponent<Collider>();
			if (collider != null && attackCollider.bounds.Intersects(collider.bounds))
			{
				value.OnHit(num, null, this, value.centroid, Vector3.up);
			}
		}
		foreach (NPCController value2 in GameSceneController.Instance.NPC_Set.Values)
		{
			Collider collider2 = value2.gameObject.GetComponent<Collider>();
			if (collider2 != null && attackCollider.bounds.Intersects(collider2.bounds))
			{
				value2.OnHit(num, null, this, value2.centroid, Vector3.up);
			}
		}
		foreach (GuardianForceController value3 in GameSceneController.Instance.GuardianForce_Set.Values)
		{
			Collider collider3 = value3.gameObject.GetComponent<Collider>();
			if (collider3 != null && attackCollider.bounds.Intersects(collider3.bounds))
			{
				value3.OnHit(num, null, this, value3.centroid, Vector3.up);
			}
		}
		foreach (EnemyController value4 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
		{
			Collider collider4 = value4.gameObject.GetComponent<Collider>();
			if (collider4 != null && attackCollider.bounds.Intersects(collider4.bounds))
			{
				value4.OnHit(enemy_data.damage_val, null, this, value4.centroid, Vector3.zero);
			}
		}
	}

	public void ResetInjureAni(Vector3 hit_point)
	{
		Vector3 vector = base.transform.InverseTransformPoint(hit_point);
		if (vector.x > injured_ani_val)
		{
			ANI_INJURED = ani_injure02;
		}
		else if (vector.x < 0f - injured_ani_val)
		{
			ANI_INJURED = ani_injure01;
		}
		else
		{
			ANI_INJURED = ani_injure03;
		}
	}

	public override void FireUpdate(float deltaTime)
	{
		AlwaysLookAtTarget();
		if (AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ANI_ATTACK, 0.9f))
		{
			((FatCookAfterAttackState)AFTER_ATTACK_STATE).IsAfterMiantuan = false;
			SetState(AFTER_ATTACK_STATE);
		}
	}

	public override void Fire()
	{
		ANI_CUR_ATTACK = ANI_ATTACK;
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

	public override void Dologic(float deltaTime)
	{
		base.Dologic(deltaTime);
		if (!enable_normalAttack)
		{
			curTime_normalAttack += deltaTime;
			if (curTime_normalAttack >= CD_normalAttack)
			{
				curTime_normalAttack = 0f;
				enable_normalAttack = true;
			}
		}
		if (!enable_comboAttack)
		{
			curTime_comboAttack += deltaTime;
			if (curTime_comboAttack >= CD_comboAttack)
			{
				curTime_comboAttack = 0f;
				enable_comboAttack = true;
			}
		}
		if (!enable_miantuanAttack)
		{
			curTime_miantuanAttack += deltaTime;
			if (curTime_miantuanAttack >= CD_miantuanAttack)
			{
				curTime_miantuanAttack = 0f;
				enable_miantuanAttack = true;
			}
		}
	}

	public override void SetState(EnemyState state)
	{
		if (enemyState != null && enemyState.GetStateType() != state.GetStateType())
		{
			enemyState.OnExitState();
			enemyState = state;
			enemyState.OnEnterState();
			if (state.GetStateType() == EnemyStateType.Idle || state.GetStateType() == EnemyStateType.FatCook_AfterAttack)
			{
				AnimationUtil.CrossAnimate(scart, "Slow", WrapMode.Loop);
			}
			else
			{
				AnimationUtil.CrossAnimate(scart, "Fast", WrapMode.Loop);
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
			enable_normalAttack = false;
			enable_comboAttack = false;
			enable_miantuanAttack = false;
			curTime_normalAttack = 0f;
			curTime_comboAttack = 0f;
			curTime_miantuanAttack = 0f;
		}
		else if (CouldEnterAttackState())
		{
			SetState(SHOOT_STATE);
			enable_normalAttack = false;
		}
		else if (CouldEnterComboAttackState())
		{
			SetState(SKILL_ATTACK_COMBO_STATE);
			enable_comboAttack = false;
		}
		else if (CouldEnterMiantuanState())
		{
			if (enemy_data.cur_hp / enemy_data.hp_capacity < 0.5f)
			{
				SetState(SKILL_SUMMON_STATE);
			}
			else
			{
				SetState(SKILL_MIANTUAN_STATE);
			}
			enable_miantuanAttack = false;
		}
		else
		{
			SetState(CATCHING_STATE);
		}
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		if (GameSceneController.Instance.is_play_cg || enemyState == SHOW_STATE || enemyState == DEAD_STATE)
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
		CreateInjuredBloodEff(hit_point, hit_normal);
		UpdateCurHpBar();
	}

	public override bool CouldEnterAttackState()
	{
		if (enable_normalAttack && base.CouldEnterAttackState())
		{
			return true;
		}
		return false;
	}

	private bool CouldEnterComboAttackState()
	{
		if (enable_comboAttack && base.SqrDistanceFromPlayer > enemy_data.attack_range * enemy_data.attack_range && base.SqrDistanceFromPlayer <= combo_viewRange * combo_viewRange)
		{
			return true;
		}
		return false;
	}

	private bool CouldEnterMiantuanState()
	{
		if (enable_miantuanAttack && base.SqrDistanceFromPlayer > combo_viewRange * combo_viewRange && base.SqrDistanceFromPlayer <= miantuan_viewRange * miantuan_viewRange)
		{
			return true;
		}
		return false;
	}

	public void PlayEffRing()
	{
		eff_ring.SetActive(true);
		eff_ring.GetComponent<ParticleSystem>().Play();
	}

	public void PlayEffCombo1()
	{
		eff_combo1.SetActive(true);
		AnimationUtil.PlayAnimate(eff_combo1, "light_01", WrapMode.Once);
		Invoke("StopEffCombo1", eff_combo1.GetComponent<Animation>()["light_01"].length);
	}

	public void StopEffCombo1()
	{
		eff_combo1.SetActive(false);
	}

	public void PlayEffCombo2()
	{
		eff_combo2.SetActive(true);
		AnimationUtil.PlayAnimate(eff_combo2, "light_04", WrapMode.Once);
		Invoke("StopEffCombo2", eff_combo2.GetComponent<Animation>()["light_04"].length);
	}

	public void StopEffCombo2()
	{
		eff_combo2.SetActive(false);
	}

	public void PlayEffSummon()
	{
		eff_summon.SetActive(true);
		eff_summon.GetComponent<ParticleSystem>().Play();
		eff_summon.GetComponent<EffectAudioBehaviour>().PlayEffect();
	}

	public void PlayEffMiantuan()
	{
		eff_miantuan.SetActive(true);
		eff_miantuan.GetComponent<ParticleSystem>().Play();
	}

	public override void PlayHalfHpEffect()
	{
		base.PlayHalfHpEffect();
		eff_angry.Play();
		GameObject gameObject = Object.Instantiate(base.Accessory[3]) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
        if (GameSceneController.Instance.mission_day_type == MissionDayType.Endless)
            return;
        Transform parent = base.transform.Find("cg_view");
		Camera.main.transform.parent = parent;
		Camera.main.transform.localPosition = Vector3.zero;
		Camera.main.transform.localRotation = Quaternion.identity;
		GameSceneController.Instance.main_camera.camera_pause = true;
		GameSceneController.Instance.main_camera.StartShake("Boss_FatCook_Camera_Bellow01");
		float length = GameSceneController.Instance.main_camera.shake_obj.GetComponent<Animation>()["Boss_FatCook_Camera_Bellow01"].length;
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

	public IEnumerator ThrowMiantuan()
	{
		if (target_player != null)
		{
			Vector3[] directions = new Vector3[3];
			int miantuanCount = 1;
			if (((BossHalfHpState)HALF_HP_STATE).IsPlayed)
			{
				float angle = 57.29578f * Mathf.Atan2(1.5f, Vector3.Distance(target_player.transform.position, base.transform.position));
				directions[1] = Vector3.Normalize(target_player.centroid - miantuanHand.transform.position);
				directions[0] = Quaternion.Euler(0f, angle, 0f) * directions[1];
				directions[2] = Quaternion.Euler(0f, 0f - angle, 0f) * directions[1];
				miantuanCount = 3;
			}
			else
			{
				directions[0] = Vector3.Normalize(target_player.centroid - miantuanHand.transform.position);
				miantuanCount = 1;
			}
			Vector3 startPos = miantuanHand.transform.position;
			for (int i = 0; i < miantuanCount; i++)
			{
				GameObject proObj = Object.Instantiate(base.Accessory[2], startPos, Quaternion.LookRotation(directions[i])) as GameObject;
				MiantuanProjectile projectile = proObj.GetComponent<MiantuanProjectile>();
				projectile.launch_dir = directions[i];
				projectile.fly_speed = miantuan_flySpeed;
				projectile.life = 4f;
				projectile.freezeCD = avatarFreezeDuration;
				projectile.object_controller = this;
				yield return 0;
			}
		}
	}

    public void SummonTimerBoomer()
    {
        if (!((BossHalfHpState)HALF_HP_STATE).IsPlayed)
            return;

        EnemyController selfController = GetComponent<EnemyController>();
        MissionController mission = GameSceneController.Instance.mission_controller;

        if (mission != null)
        {
            Debug.Log("[Fatcook] Summon triggered by: " + selfController.enemy_data.enemy_type);
            StartCoroutine(mission.SummonBossMinions(selfController));
        }
        else
        {
            Debug.LogWarning("FatCookController: No mission controller found for summoning!");
        }
    }

    public void MoveBack(float deltaTime)
	{
		base.transform.Translate(Vector3.back * deltaTime, Space.Self);
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

	private void PlayGroundStone()
	{
		ground_stone1 = Object.Instantiate(base.Accessory[0], base.transform.position, base.transform.rotation) as GameObject;
		ground_stone2 = Object.Instantiate(base.Accessory[1], base.transform.position, base.transform.rotation) as GameObject;
		AnimationUtil.PlayAnimate(ground_stone1, stone_ani1, WrapMode.Once);
		AnimationUtil.PlayAnimate(ground_stone2, stone_ani2, WrapMode.Once);
		Invoke("HideGroundStone", base.GetComponent<Animation>()[ANI_SHOW].length + 2f);
	}

	private void HideGroundStone()
	{
		Object.Destroy(ground_stone1);
		Object.Destroy(ground_stone2);
	}
}
