using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class EnemyController : ObjectController
{
	public string ANI_SHOW = "Show01";

	public string ANI_IDLE = "Idle01";

	public string ANI_RUN = "Run";

	public string ANI_ATTACK = "Attack01";

	public string ANI_INJURED = "Damage01";

	public string ANI_DEAD = "Death01";

	public string ANI_RAIL = string.Empty;

	public string ANI_PERCEIVE = string.Empty;

	public string ANI_CHAISAW_INJURED = "Damage_OT01";

	public string ANI_CUR_ATTACK = "Attack01";

	public string ANI_FROZEN = "Frozen01";

	public string ANI_HALF_HP = string.Empty;

    public EnemyType type = EnemyType.E_NONE;

    public EnemyState SHOW_STATE;

	public EnemyState IDLE_STATE;

	public EnemyState PATROL_STATE;

	public EnemyState PERCEIVE_STATE;

	public EnemyState RAIL_STATE;

	public EnemyState CATCHING_STATE;

	public EnemyState SHOOT_STATE;

	public EnemyState AFTER_SHOOT_STATE;

	public EnemyState INJURED_STATE;

	public EnemyState DEAD_STATE;

	public EnemyState CHAISAW_INJURED_STATE;

    public EnemyType enemyType;

    public EnemyState HALF_HP_STATE;

	public EnemyState FROZEN_STATE;

	protected EnemyState enemyState;

    public bool IsDead
    {
        get { return enemyState == DEAD_STATE; }
    }

    public EnemyData enemy_data;

    public bool isSpawning = true;

    public ObjectController target_player;

	public ControllerNavPath nav_pather;

	protected float fire_interval = 9999f;

	protected float injured_time;

	public float check_target_rate = 2f;

	protected float check_target_time;

    public bool ignoreSpawnLimit = false;

    protected int enemy_id = -1;

    MissionDayType currentMissionType;

    protected bool check_with_block;

	protected List<GameObject> m_accessory;

	protected bool is_partol;

	protected bool is_railing;

	protected bool is_rail_enable;

	public EnemyRailController cur_rail;

	public bool is_traped;

	public SceneTrapController trap_controller;

	public Transform perceive_trans;

	protected Vector3 patrol_posion = Vector3.zero;

	protected Vector3 born_posion = Vector3.zero;

	protected bool patrol_home;

	protected Dictionary<ObjectController, float> hatred_set = new Dictionary<ObjectController, float>();

	protected TAudioController autio_controller;

    public float MissionWeight { get; set; }

    protected GameObject head_ori;

	protected GameObject neck_ori;

	protected GameObject body_ori;

	protected GameObject head_broken;

	protected GameObject body_eff_prefab;

	protected GameObject body_eff;

	protected GameObject head_broken_eff_prefab;

	protected GameObject head_broken_eff;

	protected GameObject neck_blood_obj;

	protected int head_broken_percent = 50;

	protected int body_broken_percent = 50;

	protected bool is_boss;

	protected bool is_enchant;

	protected GameObject enchant_buff_obj;

	protected GameObject ice_skin_obj;

	protected GameObject ice_skin_broken_obj;

	protected GameObject ice_body_broken_obj;

	protected TNetObject tnetObj;

	public float frozenTime;

	protected bool is_ice_dead;

	private List<ObjectController> tem_list = new List<ObjectController>();

	protected float tem_frozenTime;

	private Vector3 tem_tar_pos = Vector3.zero;

	protected bool is_repel;

	protected Vector3 repel_dir = Vector3.zero;

	protected float total_repel_time = 0.2f;

	protected float cur_repel_time;

	private float tem_speed;

	protected bool is_hp_bar_inited;

	public SceneTUIMeshSprite hp_bar;

	public TUIRect hp_rect;

	protected float hp_width = 49f;

	protected float last_hp_update_time;

	protected bool is_hp_update;

	protected float hp_target_width;

	protected float hp_ori_width;

	public float hp_lerp_speed = 2f;

	protected TAudioController Taudio_controller;

	public EnemyState Enemy_State
	{
		get
		{
			return enemyState;
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

    public float LastInjuredTime
	{
		get
		{
			return injured_time;
		}
	}

	public float LastCheckTargetTime
	{
		get
		{
			return check_target_time;
		}
	}

	public int EnemyID
	{
		get
		{
			return enemy_id;
		}
		set
		{
			enemy_id = value;
		}
	}

	public bool WithoutBlockCheck
	{
		get
		{
			return check_with_block;
		}
		set
		{
			check_with_block = value;
		}
	}

	public List<GameObject> Accessory
	{
		get
		{
			return m_accessory;
		}
		set
		{
			m_accessory = value;
		}
	}

	public bool IsPartol
	{
		get
		{
			return is_partol;
		}
		set
		{
			is_partol = value;
		}
	}

	public bool IsRail
	{
		get
		{
			return is_railing;
		}
		set
		{
			is_railing = value;
		}
	}

	public bool RailEnable
	{
		get
		{
			return is_rail_enable;
		}
	}

	public bool AutioController
	{
		get
		{
			return autio_controller;
		}
	}

	public bool IsBoss
	{
		get
		{
			return is_boss;
		}
		set
		{
			is_boss = value;
		}
	}

	public bool IsEnchant
	{
		get
		{
			return is_enchant;
		}
	}

	public float SqrDistanceFromPlayer
	{
		get
		{
			if (target_player != null)
			{
				return (target_player.transform.position - base.transform.position).sqrMagnitude;
			}
			return 9999f;
		}
	}

	public float SqrDistanceToPatrol
	{
		get
		{
			return (patrol_posion - base.transform.position).sqrMagnitude;
		}
	}

	public float SqrDistanceToBorn
	{
		get
		{
			return (born_posion - base.transform.position).sqrMagnitude;
		}
	}

	protected override void Start()
	{
		controller_type = ControllerType.Enemy;
		nav_pather = GetComponent<ControllerNavPath>();
		Init();
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		//{
		//	tnetObj = TNetConnection.Connection;
		//}
	}

	protected override void Update()
	{
		if (Time.deltaTime != 0f && Time.timeScale != 0f)
		{
			Dologic(Time.deltaTime);
		}
	}

	protected override void OnDestroy()
	{
		if (body_eff != null)
		{
			Object.Destroy(body_eff);
		}
		if (head_broken != null)
		{
			Object.Destroy(head_broken);
		}
	}

	public virtual void Init()
	{
		SHOW_STATE = EnemyState.Create(EnemyStateType.Show, this);
		IDLE_STATE = EnemyState.Create(EnemyStateType.Idle, this);
		PATROL_STATE = EnemyState.Create(EnemyStateType.Patrol, this);
		CATCHING_STATE = EnemyState.Create(EnemyStateType.Catching, this);
		SHOOT_STATE = EnemyState.Create(EnemyStateType.Shoot, this);
		PERCEIVE_STATE = EnemyState.Create(EnemyStateType.Perceive, this);
		RAIL_STATE = EnemyState.Create(EnemyStateType.Rail, this);
		INJURED_STATE = EnemyState.Create(EnemyStateType.Injured, this);
		DEAD_STATE = EnemyState.Create(EnemyStateType.Dead, this);
		AFTER_SHOOT_STATE = EnemyState.Create(EnemyStateType.AfterShoot, this);
		CHAISAW_INJURED_STATE = EnemyState.Create(EnemyStateType.ChaisawInjured, this);
		HALF_HP_STATE = EnemyState.Create(EnemyStateType.HalfHp, this);
		FROZEN_STATE = EnemyState.Create(EnemyStateType.Frozen, this);
		enemyState = IDLE_STATE;
		WithoutBlockCheck = false;
		perceive_trans = base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Dummy_Head");
		if (perceive_trans == null)
		{
		}
		if (base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Dummy_Head/Bip01 Head/blood_baotou") != null)
		{
			neck_blood_obj = base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Dummy_Head/Bip01 Head/blood_baotou").gameObject;
		}
		if (base.transform.Find("love_gun_buff") != null)
		{
			enchant_buff_obj = base.transform.Find("love_gun_buff").gameObject;
		}
		if (base.transform.Find("Ice_Skin") != null)
		{
			ice_skin_obj = base.transform.Find("Ice_Skin").gameObject;
			ice_skin_obj.SetActive(false);
		}
		if (base.transform.Find("Ice_Skin_Broken") != null)
		{
			ice_skin_broken_obj = base.transform.Find("Ice_Skin_Broken").gameObject;
			ice_skin_broken_obj.SetActive(false);
		}
		if (base.transform.Find("Ice_Body_Broken") != null)
		{
			ice_body_broken_obj = base.transform.Find("Ice_Body_Broken").gameObject;
			ice_body_broken_obj.SetActive(false);
		}
		autio_controller = GetComponent<TAudioController>();
		born_posion = base.transform.position;
		patrol_posion = base.transform.position + base.transform.forward * 10f;
		StartCoroutine(ChangMotorSpeed());
		ShowEnchantBuff(false);
	}

	public virtual void Dologic(float deltaTime)
	{
		if (!GameSceneController.Instance.is_play_cg || IsBoss)
		{
			UpdataHateSet(deltaTime);
			if (enemyState != null)
			{
				enemyState.DoStateLogic(deltaTime);
				DetermineState();
			}
			if (is_hp_bar_inited)
			{
				UpdateHpBar();
			}
		}
	}

	public void UpdataHateSet(float deltaTime)
	{
		if (hatred_set.Count <= 0 || !(target_player != null))
		{
			return;
		}
		tem_list.Clear();
		foreach (ObjectController key3 in hatred_set.Keys)
		{
			if (key3 != null && key3.controller_type == ControllerType.Player)
			{
				tem_list.Add(key3);
			}
		}
		foreach (ObjectController item in tem_list)
		{
			Dictionary<ObjectController, float> dictionary;
			Dictionary<ObjectController, float> dictionary2 = (dictionary = hatred_set);
			ObjectController key;
			ObjectController key2 = (key = item);
			float num = dictionary[key];
			dictionary2[key2] = num - deltaTime * enemy_data.hp_capacity * GameConfig.Instance.monster_hatred_ratio;
			if (hatred_set[item] < 0f)
			{
				hatred_set[item] = 0f;
			}
		}
	}

	public void RemoveTargetFromHateSet(ObjectController obj)
	{
		if (target_player == obj)
		{
			target_player = null;
		}
		if (hatred_set.ContainsKey(obj))
		{
			hatred_set.Remove(obj);
		}
	}

	public virtual void DetermineState()
	{
		if (enemyState.IsOpen())
		{
			if (is_railing)
			{
				SetState(RAIL_STATE);
			}
			else if (is_partol)
			{
				DeterminePatrolState();
			}
			else
			{
				DetermineNormalState();
			}
		}
	}

	public virtual void DeterminePatrolState()
	{
		CheckTargetPlayer();
		if (target_player == null)
		{
			SetState(IDLE_STATE);
		}
		else if (is_partol)
		{
			if (CouldEnterAttackState())
			{
				SetState(SHOOT_STATE);
			}
			else if (CouldEnterCatchState())
			{
				SetState(CATCHING_STATE);
			}
			else
			{
				SetState(PATROL_STATE);
			}
		}
	}

	public virtual void DetermineNormalState()
	{
		CheckTargetPlayer();
		if (target_player == null)
		{
			SetState(IDLE_STATE);
		}
		else if (CouldEnterAttackState())
		{
			SetState(SHOOT_STATE);
		}
		else
		{
			SetState(CATCHING_STATE);
		}
	}

	public virtual void FindTargetPlayer()
	{
		float num = 9999f;
		if (IsEnchant)
		{
			foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
			{
				if (Vector3.Distance(value.transform.position, base.transform.position) < num)
				{
					target_player = value;
					num = Vector3.Distance(value.transform.position, base.transform.position);
				}
			}
			SetTargetPlayer();
		}
		else if (enemy_data.attack_priority == EnemyAttackPriority.Player)
		{
			foreach (PlayerController value2 in GameSceneController.Instance.Player_Set.Values)
			{
				if (value2.FireState.GetStateType() != value2.DEAD_STATE.GetStateType() && Vector3.Distance(value2.transform.position, base.transform.position) < num)
				{
					target_player = value2;
					num = Vector3.Distance(value2.transform.position, base.transform.position);
				}
			}
			if (target_player == null)
			{
				foreach (NPCController value3 in GameSceneController.Instance.NPC_Set.Values)
				{
					if (Vector3.Distance(value3.transform.position, base.transform.position) < num)
					{
						target_player = value3;
						num = Vector3.Distance(value3.transform.position, base.transform.position);
					}
				}
			}
			SetTargetPlayer();
		}
		else
		{
			if (enemy_data.attack_priority != EnemyAttackPriority.Hostage)
			{
				return;
			}
			foreach (NPCController value4 in GameSceneController.Instance.NPC_Set.Values)
			{
				if (Vector3.Distance(value4.transform.position, base.transform.position) < num)
				{
					target_player = value4;
					num = Vector3.Distance(value4.transform.position, base.transform.position);
				}
			}
			if (target_player == null)
			{
				foreach (PlayerController value5 in GameSceneController.Instance.Player_Set.Values)
				{
					if (value5.FireState.GetStateType() != value5.DEAD_STATE.GetStateType() && Vector3.Distance(value5.transform.position, base.transform.position) < num)
					{
						target_player = value5;
						num = Vector3.Distance(value5.transform.position, base.transform.position);
					}
				}
			}
			SetTargetPlayer();
		}
	}

	public virtual void FindTargetPlayerWithoutBlock()
	{
		float num = 9999f;
		if (IsEnchant)
		{
			foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
			{
				if (Vector3.Distance(value.transform.position, base.transform.position) < num && !IsBlocked(value.transform))
				{
					target_player = value;
					num = Vector3.Distance(value.transform.position, base.transform.position);
				}
			}
			SetTargetPlayer();
		}
		else if (enemy_data.attack_priority == EnemyAttackPriority.Player)
		{
			foreach (PlayerController value2 in GameSceneController.Instance.Player_Set.Values)
			{
				if (Vector3.Distance(value2.transform.position, base.transform.position) < num && !IsBlocked(value2.transform))
				{
					target_player = value2;
					num = Vector3.Distance(value2.transform.position, base.transform.position);
				}
			}
			if (target_player == null)
			{
				foreach (NPCController value3 in GameSceneController.Instance.NPC_Set.Values)
				{
					if (Vector3.Distance(value3.transform.position, base.transform.position) < num && !IsBlocked(value3.transform))
					{
						target_player = value3;
						num = Vector3.Distance(value3.transform.position, base.transform.position);
					}
				}
			}
			SetTargetPlayer();
		}
		else
		{
			if (enemy_data.attack_priority != EnemyAttackPriority.Hostage)
			{
				return;
			}
			foreach (NPCController value4 in GameSceneController.Instance.NPC_Set.Values)
			{
				if (Vector3.Distance(value4.transform.position, base.transform.position) < num && !IsBlocked(value4.transform))
				{
					target_player = value4;
					num = Vector3.Distance(value4.transform.position, base.transform.position);
				}
			}
			if (target_player == null)
			{
				foreach (PlayerController value5 in GameSceneController.Instance.Player_Set.Values)
				{
					if (Vector3.Distance(value5.transform.position, base.transform.position) < num && !IsBlocked(value5.transform))
					{
						target_player = value5;
						num = Vector3.Distance(value5.transform.position, base.transform.position);
					}
				}
			}
			SetTargetPlayer();
		}
	}

	public virtual void CheckTargetPlayer()
	{
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && !TNetConnection.IsServer)
		//{
		//	return;
		//}
		if (target_player == null || hatred_set.Count == 0)
		{
			if (Time.time - check_target_time >= check_target_rate)
			{
				if (WithoutBlockCheck)
				{
					FindTargetPlayerWithoutBlock();
				}
				else
				{
					FindTargetPlayer();
				}
				check_target_time = Time.time;
			}
		}
		else
		{
			if (!(Time.time - check_target_time >= check_target_rate))
			{
				return;
			}
			check_target_time = Time.time;
			ObjectController objectController = target_player;
			foreach (ObjectController key in hatred_set.Keys)
			{
				if (objectController == null)
				{
					objectController = key;
				}
				if (objectController != null && key != null && hatred_set.ContainsKey(key) && hatred_set.ContainsKey(objectController) && hatred_set[key] > hatred_set[objectController])
				{
					objectController = key;
				}
			}
			if (objectController != target_player)
			{
				target_player = objectController;
				SetTargetPlayer();
			}
		}
	}

	public virtual void SetTargetPlayer()
	{
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		//{
		//	if (!TNetConnection.IsServer || IsPartol || !(target_player != null) || !(nav_pather != null))
		//	{
		//		return;
		//	}
		//	nav_pather.SetTarget(target_player.transform);
		//	if (!hatred_set.ContainsKey(target_player))
		//	{
		//		hatred_set.Add(target_player, 1f);
		//	}
		//	if (tnetObj != null)
		//	{
		//		SFSObject sFSObject = new SFSObject();
		//		SFSArray sFSArray = new SFSArray();
		//		sFSArray.AddShort((short)enemy_id);
		//		sFSArray.AddShort((short)target_player.controller_type);
		//		int val = 0;
		//		if (target_player.controller_type == ControllerType.Player)
		//		{
		//			PlayerController playerController = target_player as PlayerController;
		//			val = playerController.tnet_user.Id;
		//		}
		//		else if (target_player.controller_type == ControllerType.GuardianForce)
		//		{
		//			GuardianForceController guardianForceController = target_player as GuardianForceController;
		//			val = guardianForceController.owner_controller.tnet_user.Id;
		//		}
		//		else if (target_player.controller_type == ControllerType.Enemy)
		//		{
		//			EnemyController enemyController = target_player as EnemyController;
		//			val = enemyController.enemy_id;
		//		}
		//		sFSArray.AddInt(val);
		//		sFSObject.PutSFSArray("enemyChangeTar", sFSArray);
		//		tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//	}
		//}
		/*else*/ if (!IsPartol && target_player != null && nav_pather != null)
		{
			nav_pather.SetTarget(target_player.transform);
			if (!hatred_set.ContainsKey(target_player))
			{
				hatred_set.Add(target_player, 1f);
			}
		}
	}

	public virtual void JustRushTo(bool status)
	{
		if (nav_pather != null)
		{
			nav_pather.just_rush_to = status;
		}
	}

	public virtual void SetState(EnemyState state)
	{
		if (enemyState != null && enemyState.GetStateType() != state.GetStateType())
		{
			enemyState.OnExitState();
			enemyState = state;
			enemyState.OnEnterState();
		}
	}

	public void SetStateWithType(EnemyStateType type)
	{
		switch (type)
		{
		case EnemyStateType.Dead:
			SetState(DEAD_STATE);
			break;
		case EnemyStateType.Injured:
			SetState(INJURED_STATE);
			break;
		case EnemyStateType.Idle:
			SetState(IDLE_STATE);
			break;
		case EnemyStateType.Patrol:
			SetState(PATROL_STATE);
			break;
		case EnemyStateType.Catching:
			SetState(CATCHING_STATE);
			break;
		case EnemyStateType.Shoot:
			SetState(SHOOT_STATE);
			break;
		case EnemyStateType.Frozen:
			SetState(FROZEN_STATE);
			break;
		}
	}

	public virtual void SetEnemyData(EnemyData data)
	{
		enemy_data = data;
		GameData.Instance.CheckEnemyFirstShow(enemy_data.config.enemy_name);
	}

	public IEnumerator ChangMotorSpeed()
	{
		while (nav_pather == null)
		{
			yield return 1;
		}
		nav_pather.SetSpeed(enemy_data.move_speed);
	}

	public virtual bool CouldEnterAttackState()
	{
		if (SqrDistanceFromPlayer <= enemy_data.attack_range * enemy_data.attack_range)
		{
			return true;
		}
		return false;
	}

	public virtual bool CouldEnterCatchState()
	{
		if (SqrDistanceFromPlayer < enemy_data.view_range * enemy_data.view_range)
		{
			return true;
		}
		return false;
	}

	public virtual void FireUpdate(float deltaTime)
	{
		if (target_player != null)
		{
			base.transform.LookAt(target_player.transform);
		}
		if (AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ANI_ATTACK, 0.9f))
		{
			SetState(AFTER_SHOOT_STATE);
		}
	}

	public virtual void AfterFireUpdate(float deltaTime)
	{
		if (target_player != null)
		{
			base.transform.LookAt(target_player.transform);
		}
		if (!AnimationUtil.IsPlayingAnimation(base.gameObject, ANI_CUR_ATTACK))
		{
			SetState(IDLE_STATE);
		}
	}

	public virtual void Fire()
	{
	}

	public virtual void CheckHit()
	{
	}

	public virtual void CheckHitAfter()
	{
	}

    public override void OnHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
    {
        if (enemyState == SHOW_STATE || enemyState == DEAD_STATE || isSpawning)
            return;

        injured_time = Time.time;

        if (!hatred_set.ContainsKey(player))
            hatred_set.Add(player, 1f);
        hatred_set[player] += damage;

        OnHitSound(weapon);

        bool isBoss = GameSceneController.BossEnemyTypes.Contains(enemy_data.enemy_type);
        if (isBoss && GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && damage > 0f)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                PlayerID playerID = pc.player_id;

                float existingDamage = 0f;
                if (GameSceneController.Instance.Player_damage_Set.TryGetValue(playerID, out existingDamage))
                {
                    GameSceneController.Instance.Player_damage_Set[playerID] = existingDamage + damage;
                }
                else
                {
                    GameSceneController.Instance.Player_damage_Set[playerID] = damage;
                }

                float bossMaxHp = enemy_data.hp_capacity;  // boss max HP
                float totalDamage = 0f;

                foreach (KeyValuePair<PlayerID, float> kvp in GameSceneController.Instance.Player_damage_Set)
                {
                    totalDamage += kvp.Value;
                }

                float damagePercent = 0f;
                if (bossMaxHp > 0f)
                {
                    damagePercent = totalDamage / bossMaxHp;
                }

                Debug.Log("[OnHit] Player " + playerID.ToString() + " damage: " + GameSceneController.Instance.Player_damage_Set[playerID] +
                          ", Boss Max HP: " + bossMaxHp +
                          ", Total Damage: " + totalDamage +
                          ", Percent: " + (damagePercent * 100f).ToString("F2") + "%");
            }
        }

        if (enemy_data.OnInjured(damage, player.GetComponent<ObjectController>()))
        {
            if (weapon != null && weapon.weapon_type == WeaponType.IceGun)
            {
                is_ice_dead = true;
                PlayPlayerAudio("FreezeBurst");
            }
            GameSceneController.Instance.UpdateEnemyDeathInfo(enemy_data.enemy_type, 1);
            OnDead(damage, weapon, player, hit_point, hit_normal);
            SetState(DEAD_STATE);
        }
        else if (weapon != null && weapon.weapon_type == WeaponType.IceGun)
        {
            frozenTime = ((IceGunController)weapon).frozenTime;
            AnimationUtil.Stop(base.gameObject);
            SetState(FROZEN_STATE);
        }
        else
        {
            AnimationUtil.Stop(base.gameObject);
            AnimationUtil.PlayAnimate(base.gameObject, ANI_INJURED, WrapMode.ClampForever);
            SetState(INJURED_STATE);
            CreateInjuredBloodEff(hit_point, hit_normal);
        }
        LogPlayerDamagePercentages();
        UpdateCurHpBar();
    }

    public void LogPlayerDamagePercentages()
    {
        if (enemy_data == null)
        {
            Debug.LogWarning("Enemy data is null");
            return;
        }

        float totalBossHp = enemy_data.hp_capacity;
        if (totalBossHp <= 0f)
        {
            Debug.LogWarning("Boss HP capacity is zero or less");
            return;
        }

        float totalDamage = 0f;

        // Sum total damage from all players
        foreach (KeyValuePair<PlayerID, float> kvp in GameSceneController.Instance.Player_damage_Set)
        {
            totalDamage += kvp.Value;
        }

        // For each player, calculate and log percent damage
        foreach (KeyValuePair<PlayerID, float> kvp in GameSceneController.Instance.Player_damage_Set)
        {
            PlayerID playerID = kvp.Key;
            float playerDamage = kvp.Value;
        }
    }

    public virtual void OnHitRemote(float damage, ObjectController player, bool is_frozen, float ice_time)
    {
        if (enemyState != DEAD_STATE)
        {
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

            if (is_frozen)
            {
                frozenTime = ice_time;
                AnimationUtil.Stop(base.gameObject);
                SetState(FROZEN_STATE);
            }

            // Track boss damage for coop reward (UI Still Buggy)
            bool isBoss = GameSceneController.BossEnemyTypes.Contains(enemy_data.enemy_type);
            if (isBoss && GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && damage > 0f)
            {
                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null)
                {
                    PlayerID playerID = pc.player_id;
                    if (!GameSceneController.Instance.Player_damage_Set.ContainsKey(playerID))
                    {
                        GameSceneController.Instance.Player_damage_Set[playerID] = 0f;
                    }
                    GameSceneController.Instance.Player_damage_Set[playerID] += damage;
                }
            }

            enemy_data.OnInjured(damage, player.GetComponent<ObjectController>());
            UpdateCurHpBar();
        }
    }

    public virtual void OnDeadRemote(float damage, ObjectController player)
	{
		if (enemyState != DEAD_STATE)
		{
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
            enemy_data.OnInjured(damage, player.GetComponent<ObjectController>());
            GameSceneController.Instance.UpdateEnemyDeathInfo(enemy_data.enemy_type, 1);
			OnDead(damage, null, player, base.transform.transform.position, Vector3.up);
			SetState(DEAD_STATE);
		}
	}

    private bool hasSpawnedItem = false;

    public override void OnDead(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
    {
        if (hasSpawnedItem) return;

        PlayerController playerController = (player != null) ? player.GetComponent<PlayerController>() : null;
        if (playerController == null) return;

        MissionController mission = GameSceneController.Instance.mission_controller;

        TimeAliveMissionController timeMission = mission as TimeAliveMissionController;
        if (timeMission != null)
        {
            timeMission.RegisterEnemyKill();
        }

        DailyMissionController dailyMission = mission as DailyMissionController;
        if (dailyMission != null)
        {
            dailyMission.OnEnemyKilled();
        }

        if (GameData.Instance != null && GameData.Instance.blackname)
        {
            Debug.Log("Enemy died — no drops due to blacklist.");
            hasSpawnedItem = true;
            if (cur_rail != null) cur_rail.RemoveEnemy(this);
            CancelInvoke("RepelOver");
            RepelOver();
            return;
        }

        QuestInfo questInfo = GameData.Instance.cur_quest_info;
        EndlessMissionController endlessController = EndlessMissionController.Instance;

        Vector3 crystalPos = transform.position;
        Vector3 goldPos = transform.position;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            crystalPos = col.bounds.center;
            goldPos = new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z) + Vector3.up * 0.1f;
        }

        if (endlessController != null && questInfo != null &&
            questInfo.mission_type == MissionType.Endless &&
            questInfo.mission_day_type == MissionDayType.Endless)
        {
            PickupDropManager.Instance.TrySpawnCrystal(crystalPos);
            PickupDropManager.Instance.TrySpawnGold(goldPos);
        }

        PickupDropManager.Instance.TrySpawnBulletPickup(transform.position);
        PickupDropManager.Instance.TrySpawnHpPickup(transform.position);

        hasSpawnedItem = true;

        GameSceneController.Instance.LootCash(enemy_data.loot_cash);

        if (cur_rail != null)
        {
            cur_rail.RemoveEnemy(this);
        }

        CancelInvoke("RepelOver");
        RepelOver();
    }

    public virtual bool FinishedInjuredCD()
	{
		if (Time.time - LastInjuredTime >= base.GetComponent<Animation>()[ANI_INJURED].clip.length)
		{
			return true;
		}
		return false;
	}

	public IEnumerator RemoveOnTime(float time)
	{
		yield return 1;
		if (GameSceneController.Instance.Enemy_Set.ContainsKey(enemy_id))
		{
			GameSceneController.Instance.Enemy_Set.Remove(enemy_id);
		}
		if (GameSceneController.Instance.Enemy_Enchant_Set.ContainsKey(enemy_id))
		{
			GameSceneController.Instance.Enemy_Enchant_Set.Remove(enemy_id);
		}
		if (is_traped)
		{
			trap_controller.RemoveEnemy(this);
		}
		yield return new WaitForSeconds(time);
		Object.Destroy(base.gameObject);
	}

	public virtual void EnterAttack()
	{
		Fire();
	}

	public virtual void SetPathCatchState(bool status)
	{
		if (!(nav_pather == null) && status != nav_pather.GetCatchingState())
		{
			nav_pather.Catching(status);
		}
	}

	public bool IsBlocked(Transform target)
	{
		return Physics.Raycast(base.transform.position, (target.position - base.transform.position).normalized, (target.position - base.transform.position).magnitude, (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD));
	}

	public void CreateInjuredBloodEff(Vector3 pos, Vector3 normal)
	{
		Quaternion rotation = Quaternion.LookRotation(normal);
		GameSceneController.Instance.blood_pool.GetComponent<ObjectPool>().CreateObject(pos, rotation);
	}

	public virtual void DoSpecialCatching(float deltaTime)
	{
	}

	public virtual void OnAttackInterval()
	{
	}

	public virtual bool FinishAttackAni()
	{
		if (AnimationUtil.IsPlayingAnimation(base.gameObject, ANI_ATTACK) && AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ANI_ATTACK, 0.85f))
		{
			return true;
		}
		return false;
	}

	public virtual void StartRailing()
	{
		is_railing = true;
		SetState(RAIL_STATE);
	}

	public virtual void EndRailing()
	{
		is_railing = false;
		SetState(IDLE_STATE);
	}

	public virtual void DoPatrol(float deltaTime)
	{
		if (patrol_home)
		{
			if (SqrDistanceToBorn < 1.44f)
			{
				nav_pather.SetTargetPosition(patrol_posion);
				patrol_home = false;
			}
		}
		else if (SqrDistanceToPatrol < 1.44f)
		{
			nav_pather.SetTargetPosition(born_posion);
			patrol_home = true;
		}
		SetPathCatchState(true);
	}

	public virtual void EnterPatrol()
	{
		if (patrol_home)
		{
			nav_pather.SetTargetPosition(born_posion);
		}
		else
		{
			nav_pather.SetTargetPosition(patrol_posion);
		}
	}

	public virtual bool OnChaisawSpecialHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		if (enemyState == DEAD_STATE)
		{
			return false;
		}
		if (Random.Range(0, 100) <= GameConfig.Instance.chaisaw_skill_percent)
		{
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
            enemy_data.OnInjured(damage, player.GetComponent<ObjectController>());
            SetState(CHAISAW_INJURED_STATE);
			return true;
		}
		return false;
	}

	public virtual void OnChaisawInjuredResease(ObjectController player)
	{
		if (enemy_data.cur_hp <= 0f)
		{
			GameSceneController.Instance.UpdateEnemyDeathInfo(enemy_data.enemy_type, 1);
			OnChaisawSkillDead(player);
			SetState(DEAD_STATE);
		}
		else
		{
			SetState(IDLE_STATE);
		}
	}

	public virtual void OnChaisawSkillDead(ObjectController player)
	{
		OnDead(0f, null, player, Vector3.up, Vector3.up);
		HeadBrokenFall(base.transform.forward * 0.3f, base.transform.forward);
		BodyBrokenCrash();
		Object.Destroy(base.GetComponent<Collider>());
		StartCoroutine(RemoveOnTime(3f));
	}

	public virtual void OnBodyCrashed()
	{
		GameSceneController.Instance.boom_blood_pool.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
		GameSceneController.Instance.CreateAudioOncePlayer("BulletBustBody01", 1f, base.transform.position);
		GameSceneController.Instance.blood_ground.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
	}

	public virtual void OnHeadFall()
	{
	}

	public virtual void OnHeadCrash()
	{
	}

	public virtual void OnShowUnderGround()
	{
		if (autio_controller != null)
		{
			autio_controller.PlayAudio("BrokenRock01");
		}
	}

	public virtual void OnHitSound(WeaponController weapon)
	{
		if (autio_controller != null)
		{
			if (weapon != null && (weapon.weapon_type == WeaponType.AssaultRifle || weapon.weapon_type == WeaponType.Pistol || weapon.weapon_type == WeaponType.ShotGun || weapon.weapon_type == WeaponType.MachineGun))
			{
				autio_controller.PlayAudio("BulletHitBody01");
			}
			else if (weapon != null && weapon.weapon_type == WeaponType.Baseball)
			{
				autio_controller.PlayAudio("BluntHitBody01");
			}
		}
	}

	public virtual void AlwaysLookAtTarget()
	{
		if (target_player != null)
		{
			tem_tar_pos = new Vector3(target_player.centroid.x, base.transform.position.y, target_player.centroid.z);
			base.transform.LookAt(tem_tar_pos, Vector3.up);
		}
	}

	public virtual void HeadBrokenFall(Vector3 hit_point, Vector3 hit_normal)
	{
		if (head_broken != null)
		{
			head_broken.SetActive(true);
			head_broken.transform.parent = null;
			if (neck_ori != null)
			{
				neck_ori.SetActive(true);
				neck_ori.transform.parent = head_broken.transform;
			}
			head_broken.GetComponent<Rigidbody>().mass = 1f;
			head_broken.GetComponent<Rigidbody>().drag = 1.5f;
			head_broken.GetComponent<Rigidbody>().angularDrag = 0f;
			head_broken.GetComponent<Rigidbody>().useGravity = true;
			head_broken.GetComponent<Rigidbody>().isKinematic = false;
			head_broken.GetComponent<Rigidbody>().AddForceAtPosition((-hit_normal + Vector3.up).normalized * 25f, hit_point, ForceMode.Impulse);
			AdditionalGravity additionalGravity = head_broken.AddComponent<AdditionalGravity>();
			additionalGravity.force_val = 100f;
		}
		if (head_ori != null)
		{
			head_ori.GetComponent<Renderer>().enabled = false;
		}
		OnHeadFall();
	}

	public virtual int RandomHeadBrokenFall(Vector3 hit_point, Vector3 hit_normal)
	{
		if (Random.Range(0, 100) > head_broken_percent)
		{
			HeadBrokenFall(hit_point, hit_normal);
			return 1;
		}
		if (Random.Range(0, 100) > head_broken_percent)
		{
			HeadBrokenCrash();
			return 2;
		}
		return 0;
	}

	public virtual void HeadBrokenCrash()
	{
		if (head_ori != null)
		{
			head_ori.GetComponent<Renderer>().enabled = false;
			head_broken_eff = Object.Instantiate(head_broken_eff_prefab) as GameObject;
			if (enemy_data.enemy_type == EnemyType.E_BOOMER || enemy_data.enemy_type == EnemyType.E_BOOMER_E)
			{
				head_broken_eff.transform.position = base.transform.position - Vector3.down * 0.3f;
			}
			else
			{
				head_broken_eff.transform.position = base.transform.position;
			}
			head_broken_eff.transform.rotation = base.transform.rotation;
			if (Random.Range(0, 100) % 2 == 0)
			{
				AnimationUtil.PlayAnimate(head_broken_eff, "Zombie_Broken_Head02_01", WrapMode.Once);
			}
			else
			{
				AnimationUtil.PlayAnimate(head_broken_eff, "Zombie_Broken_Head02_02", WrapMode.Once);
			}
			OnHeadCrash();
		}
		else
		{
			Debug.LogError("HeadBrokenCrash error!");
		}
	}

	public virtual bool RandomHeadBrokenCrash()
	{
		if (Random.Range(0, 100) > head_broken_percent)
		{
			HeadBrokenCrash();
			return true;
		}
		return false;
	}

	public virtual void BodyBrokenCrash()
	{
		if (body_ori != null)
		{
			body_ori.GetComponent<Renderer>().enabled = false;
			body_eff = Object.Instantiate(body_eff_prefab) as GameObject;
			body_eff.transform.position = base.transform.position;
			body_eff.transform.rotation = base.transform.rotation;
			HideShadow();
			OnBodyCrashed();
		}
		else
		{
			Debug.LogError("BodyBrokenCrash error!");
		}
	}

	public virtual bool RandomBodyBrokenCrash()
	{
		if (Random.Range(0, 100) > body_broken_percent)
		{
			BodyBrokenCrash();
			return true;
		}
		return false;
	}

	public virtual void OnMissionOverDead(ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		if (enemyState != DEAD_STATE)
		{
			OnDead(enemy_data.hp_capacity, null, player, hit_point, hit_normal);
			SetState(DEAD_STATE);
		}
	}

	public virtual void OnBossDead()
	{
        if (GameData.Instance.cur_quest_info != null &&
        GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Endless)
        {
            return;
        }
        GameSceneController.Instance.canPressEscape = false;
        Transform parent = base.transform.Find("cg_view");
		Camera.main.transform.parent = parent;
		Camera.main.transform.localPosition = Vector3.zero;
		Camera.main.transform.localRotation = Quaternion.identity;
		GameSceneController.Instance.main_camera.camera_pause = true;
		if (enemy_data.enemy_type == EnemyType.E_FATCOOK || enemy_data.enemy_type == EnemyType.E_FATCOOK_E)
		{
			GameSceneController.Instance.main_camera.StartShake("Boss_FatCook_Camera_Death01");
		}
		else if (enemy_data.enemy_type == EnemyType.E_HAOKE_A || enemy_data.enemy_type == EnemyType.E_HAOKE_B)
		{
			GameSceneController.Instance.main_camera.StartShake("Haoke_Camera_Death01");
		}
		else if (enemy_data.enemy_type == EnemyType.E_WRESTLER || enemy_data.enemy_type == EnemyType.E_WRESTLER_E || enemy_data.enemy_type == EnemyType.E_HALLOWEEN || enemy_data.enemy_type == EnemyType.E_HALLOWEEN_E || enemy_data.enemy_type == EnemyType.E_SHARK || enemy_data.enemy_type == EnemyType.E_SHARK_E)
		{
			GameSceneController.Instance.main_camera.StartShake("Boss_FatCook_Camera_Death01");
		}
		Time.timeScale = 0.3f;
		GameSceneController.Instance.OnBossMissionOver();
        GameSceneController.Instance.canPressEscape = false;
    }

	public virtual void OnBossDeadHalf()
	{
		Time.timeScale = 1f;
	}

	public virtual void OnBossDeadEnd()
	{
        GameSceneController.Instance.canPressEscape = false;
        GameSceneController.Instance.OnBossDeadOver();
	}

	public void HideShadow()
	{
		Transform transform = base.transform.Find("shadow");
		if (transform != null)
		{
			transform.GetComponent<Renderer>().enabled = false;
		}
	}

	public void RemovePather()
	{
		if (nav_pather == null)
		{
			Object.Destroy(nav_pather);
		}
	}

	public virtual void OnRepel(Vector3 dir)
	{
		CancelInvoke("RepelOver");
		is_repel = true;
		repel_dir = dir;
		base.GetComponent<Rigidbody>().useGravity = true;
		base.GetComponent<Rigidbody>().isKinematic = false;
		Invoke("RepelOver", total_repel_time);
	}

	public virtual void RepelOver()
	{
		is_repel = false;
		base.GetComponent<Rigidbody>().useGravity = false;
		base.GetComponent<Rigidbody>().isKinematic = true;
	}

	public virtual void FixedUpdate()
	{
		if (is_repel && !base.GetComponent<Rigidbody>().isKinematic)
		{
			cur_repel_time += Time.fixedDeltaTime;
			if (cur_repel_time >= total_repel_time)
			{
				cur_repel_time = total_repel_time;
			}
			Mathf.Lerp(repel_dir.magnitude, 0f, cur_repel_time / total_repel_time);
			base.transform.Translate(repel_dir * Time.deltaTime, Space.World);
		}
	}

	public virtual void ShowNeckBloodEff()
	{
		if (neck_blood_obj != null)
		{
			neck_blood_obj.GetComponent<ParticleSystem>().Play();
		}
	}

    public virtual void PlayHalfHpEffect()
    {
        if (EndlessMissionController.Instance != null && GameData.Instance.cur_quest_info != null &&
        GameData.Instance.cur_quest_info.mission_type == MissionType.Endless &&
        GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Endless)
        {
            return; // Early exit if mission is endless
        }
        GameSceneController.Instance.player_controller.SetIdleLockState(true);
    }

    public virtual void OnHalfHpEffOver()
	{
		GameSceneController.Instance.player_controller.SetIdleLockState(false);
	}

	public void AddHateTarget(ObjectController obj, float hate_val)
	{
		if (!hatred_set.ContainsKey(obj))
		{
			hatred_set.Add(obj, 0f);
		}
		Dictionary<ObjectController, float> dictionary;
		Dictionary<ObjectController, float> dictionary2 = (dictionary = hatred_set);
		ObjectController key;
		ObjectController key2 = (key = obj);
		float num = dictionary[key];
		dictionary2[key2] = num + hate_val;
	}

	public virtual bool EnchantMonster()
	{
		if (IsBoss)
		{
			return false;
		}
		if (!IsEnchant)
		{
			//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && !TNetConnection.IsServer)
			//{
			//	if (tnetObj != null)
			//	{
			//		SFSObject sFSObject = new SFSObject();
			//		sFSObject.PutShort("askEnmeyEnchant", (short)enemy_id);
			//		tnetObj.Send(new BroadcastMessageRequest(sFSObject));
			//	}
			//}
			//else
			//{
				is_enchant = true;
				base.gameObject.layer = PhysicsLayer.NPC;
				hatred_set.Clear();
				target_player = null;
				if (head_broken != null)
				{
					head_broken.layer = PhysicsLayer.NPC;
				}
				GameSceneController.Instance.Enemy_Set.Remove(enemy_id);
				foreach (EnemyController value in GameSceneController.Instance.Enemy_Enchant_Set.Values)
				{
					value.RemoveTargetFromHateSet(this);
				}
				if (is_traped && trap_controller != null)
				{
					trap_controller.RemoveEnemy(this);
				}
				GameSceneController.Instance.Enemy_Enchant_Set.Add(enemy_id, this);
				ShowEnchantBuff(true);
				//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && TNetConnection.IsServer && tnetObj != null)
				//{
				//	SFSObject sFSObject2 = new SFSObject();
				//	sFSObject2.PutShort("EnmeyEnchant", (short)enemy_id);
				//	tnetObj.Send(new BroadcastMessageRequest(sFSObject2));
				//	Debug.Log("Send EnmeyEnchant msg.");
				//}
			//}
			return true;
		}
		return false;
	}

	public virtual bool RemoteEnchantMonster()
	{
		if (!IsEnchant)
		{
			//Debug.Log("RemoteEnchantMonster");
			is_enchant = true;
			base.gameObject.layer = PhysicsLayer.NPC;
			hatred_set.Clear();
			target_player = null;
			if (head_broken != null)
			{
				head_broken.layer = PhysicsLayer.NPC;
			}
			GameSceneController.Instance.Enemy_Set.Remove(enemy_id);
			foreach (EnemyController value in GameSceneController.Instance.Enemy_Enchant_Set.Values)
			{
				value.RemoveTargetFromHateSet(this);
			}
			if (is_traped && trap_controller != null)
			{
				trap_controller.RemoveEnemy(this);
			}
			GameSceneController.Instance.Enemy_Enchant_Set.Add(enemy_id, this);
			ShowEnchantBuff(true);
			return true;
		}
		return false;
	}

	public virtual void ShowEnchantBuff(bool show_state)
	{
		if (enchant_buff_obj != null)
		{
			enchant_buff_obj.SetActive(show_state);
		}
	}

    public void StartLimitMove(float limit_time)
    {
        if (nav_pather != null)
        {
            tem_speed = nav_pather.GetSpeed();
            nav_pather.SetSpeed(0f);
            Invoke("CancelLimitMove", limit_time);
        }
    }

    public void CancelLimitMove()
	{
		nav_pather.SetSpeed(tem_speed);
	}

	public IEnumerator SetTargetPlayerRemote(ControllerType type, int tar_id)
	{
		yield return 1;
		if (GameData.Instance.cur_game_type != GameData.GamePlayType.Coop || TNetConnection.IsServer)
		{
			yield break;
		}
		ObjectController tar_controller = null;
		while (tar_controller == null)
		{
			yield return 3;
			switch (type)
			{
			case ControllerType.Player:
			{
				TNetUser user2 = tnetObj.CurRoom.GetUserById(tar_id);
				if (user2 != null && GameSceneController.Instance.Player_Set.ContainsKey(user2))
				{
					tar_controller = GameSceneController.Instance.Player_Set[user2];
				}
				break;
			}
			case ControllerType.GuardianForce:
			{
				TNetUser user = tnetObj.CurRoom.GetUserById(tar_id);
				if (user != null && GameSceneController.Instance.Player_Set.ContainsKey(user))
				{
					PlayerController tar_player = GameSceneController.Instance.Player_Set[user];
					if (tar_player != null && tar_player.cur_guardian_obj != null)
					{
						tar_controller = tar_player.cur_guardian_obj.GetComponent<GuardianForceController>();
					}
				}
				break;
			}
			case ControllerType.Enemy:
				if (GameSceneController.Instance.Enemy_Set.ContainsKey(tar_id))
				{
					tar_controller = GameSceneController.Instance.Enemy_Set[tar_id];
				}
				break;
			}
		}
		target_player = tar_controller;
		if (!IsPartol && target_player != null)
		{
			while (nav_pather == null)
			{
				yield return 1;
			}
			nav_pather.SetTarget(target_player.transform);
			if (!hatred_set.ContainsKey(target_player))
			{
				hatred_set.Add(target_player, 1f);
			}
		}
	}

	public void SetEnemyBeCoopBoss(CoopBossCfg boss_cfg)
	{
		if (boss_cfg.boss_type != enemy_data.enemy_type)
		{
			Debug.LogError("coop boss type error!");
			return;
		}
		IsBoss = true;
		enemy_data.cur_hp = (enemy_data.hp_capacity = boss_cfg.hp_capacity);
		enemy_data.damage_val = boss_cfg.damage_base;
        BossCoopMissionController missionController = GameSceneController.Instance.mission_controller as BossCoopMissionController;
        if (missionController != null)
        {
            missionController.SetBoss(this);
        }
    }

	public void CreateHpBar(Vector3 pos)
	{
		GameObject gameObject = Object.Instantiate(GameSceneController.Instance.hp_bar_ref) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = pos;
		hp_bar = gameObject.GetComponent<SceneTUIMeshSprite>();
		hp_rect = hp_bar.m_showClipObj.GetComponent<TUIRect>();
		SetHpBar(1f);
		is_hp_bar_inited = true;
	}

	public void SetHpBar(float percent)
	{
		last_hp_update_time = Time.time;
		is_hp_update = true;
		hp_target_width = hp_width * percent;
		hp_ori_width = hp_rect.Size.x;
	}

	private void UpdateHpBar()
	{
		if (is_hp_update)
		{
			hp_rect.Size = new Vector2(Mathf.Lerp(hp_ori_width, hp_target_width, (Time.time - last_hp_update_time) * hp_lerp_speed), hp_rect.Size.y);
			hp_rect.NeedUpdate = true;
			hp_bar.NeedUpdate = true;
			if (Time.time - last_hp_update_time >= 1f * hp_lerp_speed)
			{
				is_hp_update = false;
			}
		}
	}

	public void UpdateCurHpBar()
	{
		if (is_hp_bar_inited)
		{
			SetHpBar(enemy_data.cur_hp / enemy_data.hp_capacity);
		}
	}

	public void ShowHpbar()
	{
		if (is_hp_bar_inited)
		{
			hp_bar.gameObject.SetActive(true);
		}
	}

	public void HideHpbar()
	{
		if (is_hp_bar_inited)
		{
			hp_bar.gameObject.SetActive(false);
		}
	}

	public virtual void OnEnterFrozenState()
	{
		if (ice_skin_obj != null)
		{
			ice_skin_obj.SetActive(true);
			AnimationUtil.PlayAnimate(ice_skin_obj, "Shake01", WrapMode.Loop);
		}
		PlayPlayerAudio("Freeze");
	}

	public virtual void OnExitFrozenState()
	{
		if (ice_skin_obj != null)
		{
			AnimationUtil.Stop(ice_skin_obj);
			ice_skin_obj.SetActive(false);
			ShowIceSkinBroken();
		}
	}

	public void ShowIceSkinBroken()
	{
		if (ice_skin_broken_obj != null)
		{
			ice_skin_broken_obj.SetActive(true);
			AnimationUtil.Stop(ice_skin_broken_obj);
			AnimationUtil.PlayAnimate(ice_skin_broken_obj, "Broken01", WrapMode.Once);
			Invoke("HideIceSkinBroken", ice_skin_broken_obj.GetComponent<Animation>()["Broken01"].length);
		}
	}

	public void HideIceSkinBroken()
	{
		if (ice_skin_broken_obj != null)
		{
			ice_skin_broken_obj.SetActive(false);
		}
	}

	public virtual void OnIceBodyCrash()
	{
		if (ice_body_broken_obj != null)
		{
			if (head_ori != null)
			{
				head_ori.GetComponent<Renderer>().enabled = false;
				OnHeadCrash();
			}
			if (body_ori != null)
			{
				body_ori.GetComponent<Renderer>().enabled = false;
				HideShadow();
			}
			ice_body_broken_obj.SetActive(true);
			AnimationUtil.Stop(ice_body_broken_obj);
			AnimationUtil.PlayAnimate(ice_body_broken_obj, "Broken01", WrapMode.Once);
		}
	}

	public virtual void OnShowOver()
	{
	}

	public virtual void PlayPlayerAudio(string audio_name)
	{
		if (Taudio_controller == null)
		{
			Taudio_controller = GetComponent<TAudioController>();
		}
		Taudio_controller.PlayAudio(audio_name);
	}
}
