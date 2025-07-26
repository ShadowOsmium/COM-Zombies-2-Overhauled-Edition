using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class PlayerController : ObjectController
{
	public enum ComboBuffType
	{
		None = 1,
		Fast,
		Strong,
		SuperMan
	}

	public TNetUser tnet_user;

	public PlayerID player_id;

	public string ANI_MOVE_IDLE_ORI = "LowerBody_Idle01";

	public string ANI_MOVE_IDLE = "LowerBody_Idle01";

	public string ANI_MOVE_RUN = "Lower_body_Run01";

	public string ANI_MOVE_RUN_FORWORD = "Lower_body_Run01";

	public string ANI_MOVE_RUN_LEFT = "Lower_body_Run03";

	public string ANI_MOVE_RUN_RIGHT = "Lower_body_Run04";

	public string ANI_MOVE_RUN_BACK = "Lower_body_Run02";

	public string ANI_SHIELD_BREAK = "Shield_UpperBody_BreakRun01";

	public string ANI_CHAISAW_SKILL = "Electric_saw_OT01";

	public PlayerState IDLE_STATE;

	public PlayerState RUN_STATE;

	public PlayerState IDLE_LOCK_STATE;

	public PlayerState IDLE_SHOOT_LOCK_STATE;

	public PlayerState IDLE_SHOOT_STATE;

	public PlayerState SHOOT_STATE;

	public PlayerState AFTER_SHOOT_STATE;

	public PlayerState RELOAD_STATE;

	public PlayerState SWITCH_WEAPON_STATE;

	public PlayerState GOTHIT_STATE;

	public PlayerState DEAD_STATE;

	public PlayerState SHIELD_BREAK_STATE;

	public PlayerState CHAISAW_SKILL_STATE;

	protected PlayerState move_state;

	protected PlayerState fire_state;

	protected PlayerState last_fire_state;

	protected SimpleCharacterMotor character_motor;

	public AvatarData avatar_data;

	protected List<WeaponController> primary_weapon = new List<WeaponController>();

	protected WeaponController cur_weapon;

	protected WeaponController cur_primary_weapon;

	public Transform spine;

	public Transform head_spine;

	public Quaternion spine_ori_rot = Quaternion.identity;

	public Quaternion head_spine_ori_rot = Quaternion.identity;

	protected float angle_v_spine;

	protected float angle_h_spine;

	protected float angle_v_head_spine;

	protected float angle_h_head_spine;

	protected float spine_rot_speed = 60f;

	public float fire_light_wall_decay = 10f;

	public float fire_light_wall_limit = 1.5f;

	public float fire_light_player_decay = 10f;

	public float fire_light_player_limit = 1.5f;

	public Light Fire_Light_Wall;

	public Light Fire_Light_Player;

	protected int last_fire_input;

	protected int cur_fire_input;

	protected bool could_enter_after_fire;

	protected ComboBuffType cur_combo_type = ComboBuffType.None;

	protected float cur_combo_val;

	protected float last_combo_val = 1f;

	protected float combo_fire_damage_ratio = 1f;

	protected float combo_fire_rate_ratio = 1f;

	protected float combo_limit_fast = 10f;

	protected float combo_limit_strong = 20f;

	protected float combo_limit_super = 30f;

	protected float combo_limit_max = 40f;

	protected float cur_combo_limit = 10f;

	public EnemyController chaisaw_target_enemy;

	public Transform chaisaw_view;

	public Transform chaisaw_pos;

	protected float cur_combo_update_time;

	protected float combo_update_rate = 0.25f;

	public GameObject combo_buff_obj;

	public GameObject eff_base_ball;

	public GameObject swat_dun;

	public GameObject swat_podun;

	protected TAudioController Taudio_controller;

	protected GameObject cowboy_cap;

	protected List<Renderer> avatar_render_set = new List<Renderer>();

	protected Shader diffuse_shader;

    protected Shader alpha_shader;

	protected bool is_rebirth_buff;

	protected bool is_limit_move;

	public Dictionary<string, SkillController> skill_set = new Dictionary<string, SkillController>();

	private TNetObject tnetObj;

	protected Dictionary<PlayerStateType, PlayerState> player_state_set = new Dictionary<PlayerStateType, PlayerState>();

	public GameObject cur_guardian_obj;

	public bool is_coop_dead;

	protected bool is_myself;

	public PlayerState MoveState
	{
		get
		{
			return move_state;
		}
	}

	public PlayerState FireState
	{
		get
		{
			return fire_state;
		}
	}

	public PlayerState LastFireState
	{
		get
		{
			return last_fire_state;
		}
	}

	public WeaponController CurWeapon
	{
		get
		{
			return cur_weapon;
		}
	}

	public WeaponController CurPrimaryWeapon
	{
		get
		{
			return cur_primary_weapon;
		}
	}

	public int LastFireInput
	{
		get
		{
			return last_fire_input;
		}
	}

	public int CurFireInput
	{
		get
		{
			return cur_fire_input;
		}
	}

	public bool CouldEnterAfterFire
	{
		get
		{
			return could_enter_after_fire;
		}
		set
		{
			could_enter_after_fire = value;
		}
	}

	public ComboBuffType ComboType
	{
		get
		{
			return cur_combo_type;
		}
	}

	public float ComboDamageRatio
	{
		get
		{
			return combo_fire_damage_ratio;
		}
	}

	public float ComboRateRatio
	{
		get
		{
			return combo_fire_rate_ratio;
		}
	}

	public GameObject CowboyCap
	{
		get
		{
			return cowboy_cap;
		}
	}

	public bool IsMyself
	{
		get
		{
			return is_myself;
		}
	}

	protected virtual void Awake()
	{
		IDLE_STATE = PlayerState.Create(PlayerStateType.Idle, this);
		player_state_set.Add(IDLE_STATE.GetStateType(), IDLE_STATE);
		RUN_STATE = PlayerState.Create(PlayerStateType.Run, this);
		player_state_set.Add(RUN_STATE.GetStateType(), RUN_STATE);
		IDLE_LOCK_STATE = PlayerState.Create(PlayerStateType.IdleLock, this);
		player_state_set.Add(IDLE_LOCK_STATE.GetStateType(), IDLE_LOCK_STATE);
		IDLE_SHOOT_STATE = PlayerState.Create(PlayerStateType.IdleShoot, this);
		player_state_set.Add(IDLE_SHOOT_STATE.GetStateType(), IDLE_SHOOT_STATE);
		SHOOT_STATE = PlayerState.Create(PlayerStateType.Shoot, this);
		player_state_set.Add(SHOOT_STATE.GetStateType(), SHOOT_STATE);
		AFTER_SHOOT_STATE = PlayerState.Create(PlayerStateType.AfterShoot, this);
		player_state_set.Add(AFTER_SHOOT_STATE.GetStateType(), AFTER_SHOOT_STATE);
		RELOAD_STATE = PlayerState.Create(PlayerStateType.Reload, this);
		player_state_set.Add(RELOAD_STATE.GetStateType(), RELOAD_STATE);
		SWITCH_WEAPON_STATE = PlayerState.Create(PlayerStateType.SwitchWeapon, this);
		player_state_set.Add(SWITCH_WEAPON_STATE.GetStateType(), SWITCH_WEAPON_STATE);
		GOTHIT_STATE = PlayerState.Create(PlayerStateType.GotHit, this);
		player_state_set.Add(GOTHIT_STATE.GetStateType(), GOTHIT_STATE);
		DEAD_STATE = PlayerState.Create(PlayerStateType.Dead, this);
		player_state_set.Add(DEAD_STATE.GetStateType(), DEAD_STATE);
		IDLE_SHOOT_LOCK_STATE = PlayerState.Create(PlayerStateType.IdleShootLock, this);
		player_state_set.Add(IDLE_SHOOT_LOCK_STATE.GetStateType(), IDLE_SHOOT_LOCK_STATE);
		SetMoveState(IDLE_STATE);
		ResetFireIdle();
		ResetAnimationLayer();
		character_motor = GetComponent<SimpleCharacterMotor>();
		GetComponent<PlayerAnimationEvent>().SetController(this);
		if (spine != null)
		{
			spine_ori_rot = spine.localRotation;
		}
		if (head_spine != null)
		{
			head_spine_ori_rot = head_spine.localRotation;
		}
		if (head_spine != null)
		{
			head_spine_ori_rot = head_spine.localRotation;
		}
		combo_buff_obj.SetActive(false);
		StopBaseBallEff();
		diffuse_shader = Shader.Find("Mobile/Diffuse");
        alpha_shader = Shader.Find("Triniti/Character/COL_AB");
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		{
			tnetObj = TNetConnection.Connection;
			base.gameObject.AddComponent<NetworkTransformSender>();
		}
		is_myself = true;
	}

    protected override void Start()
    {
        GameSceneController.Instance.player_controller = this;

        tnet_user = new TNetUser(0, string.Empty);

        string nick = GameData.Instance.NickName;
        player_id = new PlayerID(avatar_data.avatar_type, avatar_data.avatar_state, nick, 0);

        gameObject.name = nick;

        if (!GameSceneController.Instance.Player_Set.ContainsKey(tnet_user))
        {
            GameSceneController.Instance.Player_Set.Add(tnet_user, this);
        }

        // Add the player to the damage dictionary
        if (!GameSceneController.Instance.Player_damage_Set.ContainsKey(player_id))
        {
            GameSceneController.Instance.Player_damage_Set.Add(player_id, 0f);
        }

        Fire_Light_Wall.enabled = false;
        Fire_Light_Player.enabled = false;
        controller_type = ControllerType.Player;
    }

    protected override void Update()
	{
		if (Time.deltaTime == 0f || Time.timeScale == 0f)
		{
			return;
		}
		ResetFireInput();
		if (GameSceneController.Instance.GamePlayingState == PlayingState.Lose || GameSceneController.Instance.GamePlayingState == PlayingState.Win)
		{
			SetMoveState(IDLE_STATE);
			move_state.DoStateLogic(Time.deltaTime);
			fire_state.DoStateLogic(Time.deltaTime);
			DetermineFireState();
		}
		else
		{
			if (spine == null)
			{
				return;
			}
			if (cur_weapon != null)
			{
				DoAutoLock(Time.deltaTime, cur_weapon);
			}
			if (fire_state != null)
			{
				fire_state.DoStateLogic(Time.deltaTime);
				DetermineFireState();
			}
			if (move_state != null && (fire_state == null || fire_state.GetStateType() != PlayerStateType.Whirlwind))
			{
				move_state.DoStateLogic(Time.deltaTime);
				DetermineMoveState();
			}
			if (character_motor != null && fire_state != DEAD_STATE && !is_limit_move)
			{
				character_motor.DoLogic();
			}
			ComboDologic(Time.deltaTime);
			foreach (SkillController value in skill_set.Values)
			{
				value.Dologic(Time.deltaTime);
			}
		}
        if (queued_fire_state != null && fire_state != null)
        {
            var reloadState = fire_state as PlayerReloadState;
            if (reloadState == null || reloadState.CanInterrupt())
            {
                SetFireState(queued_fire_state);
            }
        }
    }

	public void CheckFireWeapon()
	{
		if (EnableControll() && GameSceneController.Instance.input_controller.PrimaryFire && cur_weapon != cur_primary_weapon)
		{
			ChangeCurWeapon(cur_primary_weapon);
		}
	}

	public void ResetFireWeapon()
	{
		if (cur_weapon != cur_primary_weapon)
		{
			ChangeCurWeapon(cur_primary_weapon);
		}
	}

	public void FireLight(bool state, float time)
	{
		if (!GameData.IsHighEffect())
		{
			return;
		}
		if (state)
		{
			Fire_Light_Wall.enabled = state;
			Fire_Light_Wall.intensity = fire_light_wall_limit;
		}
		else
		{
			Fire_Light_Wall.intensity -= time * fire_light_wall_decay;
			if (Fire_Light_Wall.intensity <= 0.1f)
			{
				Fire_Light_Wall.enabled = false;
			}
		}
		if (state)
		{
			Fire_Light_Player.enabled = state;
			Fire_Light_Player.intensity = fire_light_player_limit;
			return;
		}
		Fire_Light_Player.intensity -= time * fire_light_player_decay;
		if (Fire_Light_Player.intensity <= 0.1f)
		{
			Fire_Light_Player.enabled = false;
		}
	}

	public virtual void Fire(PlayerStateType state_type, float deltaTime)
	{
		CheckFireWeapon();
		cur_weapon.FireUpdate(this, Time.deltaTime);
	}

	public virtual void StopFire()
	{
		GameSceneController.Instance.game_main_panel.HideGameTipLabel();
		if (cur_weapon != null)
		{
			cur_weapon.StopFire(this);
		}
		if (GameData.IsHighEffect())
		{
			Fire_Light_Wall.enabled = false;
			Fire_Light_Player.enabled = false;
		}
	}

	public virtual void SetMoveState(PlayerState state)
	{
		if (move_state == null)
		{
			move_state = state;
			move_state.OnEnterState();
			//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
			//{
			//	SFSObject sFSObject = new SFSObject();
			//	sFSObject.PutShort("data", (short)move_state.GetStateType());
			//	tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.PlayerMoveState, sFSObject));
			//}
		}
		else if (move_state != null && move_state.GetStateType() != state.GetStateType())
		{
			move_state.OnExitState();
			move_state = state;
			move_state.OnEnterState();
			//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
			//{
			//	SFSObject sFSObject2 = new SFSObject();
			//	sFSObject2.PutShort("data", (short)move_state.GetStateType());
			//	tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.PlayerMoveState, sFSObject2));
			//}
		}
	}

    private PlayerState queued_fire_state;

    public virtual void SetFireState(PlayerState state)
    {
        if (fire_state == null)
        {
            last_fire_state = state;
            fire_state = state;
            fire_state.OnEnterState();
        }
        else if (fire_state.GetStateType() != state.GetStateType())
        {
            var canInterrupt = true;
            var reloadState = fire_state as PlayerReloadState;
            if (reloadState != null)
                canInterrupt = reloadState.CanInterrupt();

            if (!canInterrupt)
            {
                Debug.Log("State change denied, queuing new state: " + state.GetStateType());
                queued_fire_state = state;
                return;
            }

            last_fire_state = fire_state;
            fire_state.OnExitState();
            fire_state = state;
            fire_state.OnEnterState();
            queued_fire_state = null; // clear after applying
        }
    }

    public virtual void SetAvatarData(AvatarData data)
	{
		avatar_data = data;
		avatar_data.ResetData();
		GameSceneController.Instance.SetExpBar((float)avatar_data.cur_exp.GetIntVal() / (float)avatar_data.next_level_exp.GetIntVal());
		GameSceneController.Instance.SetHpBar(1f);
		GameSceneController.Instance.SetLevelLabel(avatar_data.level);
		StartCoroutine(ChangMotorSpeed());
		StartCoroutine(InitWeapon());
		ResetRenderSet();
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
		//{
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutUtfString("nickName", GameData.Instance.NickName);
		//	sFSObject.PutShort("avatarType", (short)avatar_data.avatar_type);
		//	sFSObject.PutShort("hpLv", (short)avatar_data.hp_level);
		//	sFSObject.PutShort("armorLv", (short)avatar_data.armor_level);
		//	sFSObject.PutShort("dmgLv", (short)avatar_data.damage_level);
		//	sFSObject.PutShort("avatarLv", (short)avatar_data.avatar_state);
		//	sFSObject.PutUtfString("weapon", avatar_data.primary_equipment);
		//	for (int i = 0; i < 2; i++)
		//	{
		//		sFSObject.PutUtfString("skill" + i, avatar_data.skill_list[i]);
		//	}
		//	tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.AvatarData, sFSObject));
		//}
	}

	public IEnumerator ChangMotorSpeed()
	{
		while (character_motor == null)
		{
			yield return 1;
		}
		character_motor.maxForwardSpeed = avatar_data.move_speed;
		character_motor.maxSidewaysSpeed = avatar_data.move_speed * 0.8f;
		character_motor.maxBackwardsSpeed = avatar_data.move_speed * 0.5f;
		character_motor.maxVelocityChange = avatar_data.move_speed * 2f;
	}

	public void ResetMotorSpeed()
	{
		if (!(cur_weapon == null))
		{
			character_motor.maxForwardSpeed = avatar_data.move_speed - cur_weapon.weapon_data.config.moveSpeedDrag;
			character_motor.maxSidewaysSpeed = character_motor.maxForwardSpeed * 0.8f;
			character_motor.maxBackwardsSpeed = character_motor.maxForwardSpeed * 0.5f;
			character_motor.maxVelocityChange = character_motor.maxForwardSpeed * 2f;
		}
	}

	public IEnumerator InitWeapon()
	{
		while (spine == null)
		{
			yield return 1;
		}
		WeaponController weapon2 = null;
		for (int i = 0; i < 1; i++)
		{
			if (avatar_data.primary_equipment != "None")
			{
				weapon2 = WeaponFactory.CreateWeapon(avatar_data.primary_equipment);
				weapon2.SetWeaponAni(base.gameObject, spine);
				if (weapon2.weapon_type == WeaponType.Shield || weapon2.weapon_type == WeaponType.Medicine)
				{
					weapon2.Reset(base.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 Prop2"));
				}
				else
				{
					weapon2.Reset(base.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 Prop1"));
				}
				primary_weapon.Add(weapon2);
			}
		}
		cur_primary_weapon = primary_weapon[0];
		ChangeCurWeapon(cur_primary_weapon);
		PlayerStatistics();
	}

	public virtual void InitSkill()
	{
		int num = 0;
		foreach (string item in avatar_data.skill_list)
		{
			if (item != "null")
			{
				SkillController skillController = SkillController.CreateSkillController(item);
				skillController.Init(GameData.Instance.Skill_Avatar_Set[item], this);
				GameSceneController.Instance.game_main_panel.skill_button_list[num].Init(skillController);
				GameSceneController.Instance.game_main_panel.skill_button_list[num].ResetSkillFrame(item);
				skillController.SetGameUIObj(GameSceneController.Instance.game_main_panel.skill_button_list[num]);
				skill_set[item] = skillController;
			}
			else
			{
				GameSceneController.Instance.game_main_panel.skill_button_list[num].gameObject.SetActive(false);
			}
			num++;
		}
	}

	protected void PlayerStatistics()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("damage_level", avatar_data.damage_level);
		hashtable.Add("hp_level", avatar_data.hp_level);
		hashtable.Add("armor_level", avatar_data.armor_level);
		hashtable.Add("UseTime", 1);
		GameData.Instance.UploadStatistics("Avatar_" + avatar_data.avatar_name, hashtable);
		foreach (WeaponController item in primary_weapon)
		{
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("damage_level", item.weapon_data.damage_level);
			hashtable2.Add("frequency_level", item.weapon_data.frequency_level);
			hashtable2.Add("clip_level", item.weapon_data.clip_level);
			hashtable2.Add("stretch_level", item.weapon_data.stretch_level);
			hashtable2.Add("UseTime", 1);
			GameData.Instance.UploadStatistics(item.weapon_data.config.weapon_name + "_State", hashtable2);
		}
	}

	public bool ChangePrimaryWeapon(WeaponController weapon_desired)
	{
		foreach (WeaponController item in primary_weapon)
		{
			if (item.weapon_data.weapon_name == weapon_desired.weapon_data.weapon_name)
			{
				cur_primary_weapon = item;
				ChangeCurWeapon(cur_primary_weapon);
				return true;
			}
		}
		return false;
	}

	public void ChangeNextPrimaryWeapon()
	{
		if (primary_weapon.Count < 2 || !(cur_primary_weapon != null))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		foreach (WeaponController item in primary_weapon)
		{
			if (item.weapon_data.weapon_name == cur_primary_weapon.weapon_data.weapon_name)
			{
				num2 = num + 1;
				num2 %= primary_weapon.Count;
				ChangePrimaryWeapon(primary_weapon[num2]);
				break;
			}
			num++;
		}
	}

	public void ShowCurWeapon(bool is_show)
	{
		if (is_show)
		{
			StopFire();
			cur_weapon.GunOn();
		}
		else
		{
			StopFire();
			cur_weapon.GunOff();
		}
	}

	public void ChangeCurWeapon(WeaponController weapon)
	{
		bool flag = false;
		if (cur_weapon != null)
		{
			StopFire();
			cur_weapon.GunOff();
			flag = ((!weapon.weapon_data.is_secondary) ? true : false);
		}
		cur_weapon = weapon;
		cur_weapon.GunOn();
		if (flag)
		{
			if (fire_state != null && fire_state.GetStateType() == SWITCH_WEAPON_STATE.GetStateType())
			{
				ResetFireIdle(false);
			}
			SetFireState(SWITCH_WEAPON_STATE);
		}
		else
		{
			ResetFireIdle(false);
		}
		UpdateWeaponUIShow();
		GameSceneController.Instance.SetSightBeadMaxStretch(cur_weapon.weapon_data.stretch_max);
		GetMoveIdleAnimation(FireState);
		if (move_state.GetStateType() == PlayerStateType.Idle)
		{
			ResetSightOriPos();
		}
		else if (move_state.GetStateType() == PlayerStateType.Run)
		{
			ResetSightOriPosRun();
		}
		ResetSightTexture();
		ResetMotorSpeed();
		GameSceneController.Instance.ResetAutoLock();
		GameSceneController.Instance.SetComboLabel(cur_combo_type.ToString());
	}

	public void UpdateWeaponUIShow()
	{
		GameSceneController.Instance.UpdateBulletLabel(cur_weapon.weapon_data.clip_bullet_count + "/" + cur_weapon.weapon_data.GetRemainCount());
		if (cur_weapon.weapon_data.total_bullet_count > 0)
		{
			GameSceneController.Instance.UpdateAddBulletButton(false);
			GameSceneController.Instance.UpdateAddBulletLabel(false);
		}
		else
		{
			if (cur_weapon.weapon_data.EnableBuyButtletInBattlet())
			{
				GameSceneController.Instance.UpdateAddBulletButton(true);
			}
			else
			{
				GameSceneController.Instance.UpdateAddBulletButton(false);
			}
			GameSceneController.Instance.UpdateAddBulletLabel(true);
		}
		GameSceneController.Instance.game_main_panel.HideGameTipLabel();
		if (cur_weapon.weapon_data.config.is_infinity_ammo)
		{
			GameSceneController.Instance.UpdateBulletLabel("N/A");
		}
		cur_weapon.UpdateWeaponFrame(true);
	}

	public virtual bool CouldUseSecondaryWeapon()
	{
		return true;
	}

	public void ResetFireInterval()
	{
		cur_weapon.ResetFireInterval();
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
		if (GameSceneController.Instance.GamePlayingState != PlayingState.Gaming || GameSceneController.Instance.is_logic_paused || !EnableControll() || is_rebirth_buff || avatar_data.cur_hp <= 0f)
		{
			return;
		}
		avatar_data.Injured(damage);
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && damage > 0f && tnetObj != null)
		//{
		//	SFSArray sFSArray = new SFSArray();
		//	sFSArray.AddFloat(avatar_data.cur_hp);
		//	sFSArray.AddFloat(avatar_data.hp_capacity);
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutSFSArray("playerInjured", sFSArray);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
		if (avatar_data.cur_hp <= 0f)
		{
			avatar_data.cur_hp = 0f;
			OnDead(damage, weapon, controller, hit_point, hit_normal);
			SetFireState(DEAD_STATE);
			//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
			//{
			//	SFSObject sFSObject2 = new SFSObject();
			//	sFSObject2.PutUtfString("playerDead", "0");
			//	tnetObj.Send(new BroadcastMessageRequest(sFSObject2));
			//	foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
			//	{
			//		value.RemoveTargetFromHateSet(this);
			//	}
			//}
		}
		UpdateHpBar();
		GameSceneController.Instance.screen_blood.OnInjured();
		EnemyController enemyController = controller as EnemyController;
		if (enemyController != null && (enemyController.IsBoss || enemyController.enemy_data.enemy_type == EnemyType.E_HALLOWEEN_SUB || enemyController.enemy_data.enemy_type == EnemyType.E_HALLOWEEN_SUB_E))
		{
			GameSceneController.Instance.boss_screen_blood1.OnInjured(true);
			GameSceneController.Instance.main_camera.StartCommonShake();
			PlayPlayerAudio("BrokenGlass01");
		}
	}

    public void SetPlayerDeadState()
    {
        move_state = DEAD_STATE;
        fire_state = DEAD_STATE;
    }

    public override void OnDead(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
    {
        StopFire();
        SetPlayerDeadState();
        GameSceneController.Instance.OnKeyManDead(this);
    }

    public virtual void CheckHit()
	{
		cur_weapon.CheckHit(this);
	}

	public virtual void DetermineMoveState()
	{
		if (GameSceneController.Instance.input_controller.Moving)
		{
			SetMoveState(RUN_STATE);
		}
		else
		{
			SetMoveState(IDLE_STATE);
		}
	}

	public virtual void DetermineFireState()
	{
		if (fire_state.EnableInterrupt)
		{
			CalculateSetFireState();
		}
	}

	public virtual void ResetFireInput()
	{
		if (last_fire_input != cur_fire_input)
		{
			last_fire_input = cur_fire_input;
		}
		if (GameSceneController.Instance.input_controller.Fire)
		{
			if (GameSceneController.Instance.input_controller.PrimaryFire)
			{
				cur_fire_input = 1;
			}
		}
		else
		{
			cur_fire_input = 0;
		}
	}

    public virtual void CalculateSetFireState()
    {
        if (GameSceneController.Instance.input_controller.Fire)
        {
            if (cur_weapon.HaveBullets())
            {
                SetFireState(SHOOT_STATE);
            }
            else if (cur_weapon.weapon_data.EnableReload())
            {
                WeaponReload();
            }
            else
            {
                ResetFireIdle();
            }
        }
        else if (could_enter_after_fire && fire_state.GetStateType() == SHOOT_STATE.GetStateType())
        {
            SetFireState(AFTER_SHOOT_STATE);
        }
        else if (fire_state.GetStateType() != AFTER_SHOOT_STATE.GetStateType())
        {
            ResetFireIdle();
        }
        if (last_fire_input != cur_fire_input)
        {
            UpdateWeaponUIShow();
        }
    }

    protected void ResetAnimationLayer()
	{
		base.GetComponent<Animation>()[ANI_MOVE_IDLE].layer = -1;
		base.GetComponent<Animation>()[ANI_MOVE_RUN_FORWORD].layer = -1;
		base.GetComponent<Animation>()[ANI_MOVE_RUN_BACK].layer = -1;
		base.GetComponent<Animation>()[ANI_MOVE_RUN_LEFT].layer = -1;
		base.GetComponent<Animation>()[ANI_MOVE_RUN_RIGHT].layer = -1;
	}

	public string GetFireStateAnimation(PlayerState run_state, PlayerState fire_state)
	{
		if (cur_weapon != null)
		{
			return cur_weapon.GetFireStateAnimation(run_state.GetStateType(), fire_state.GetStateType());
		}
		return ANI_MOVE_IDLE_ORI;
	}

	public void OnSwatIdleShoot()
	{
	}

	public void OnSwatShieldBreak()
	{
		Debug.Log("Enter Shield Break State.");
		SetFireState(SHIELD_BREAK_STATE);
	}

	public void GetMoveIdleAnimation(PlayerState fire_state)
	{
		if (cur_weapon != null)
		{
			ANI_MOVE_IDLE = cur_weapon.GetMoveIdleAnimation(fire_state.GetStateType());
		}
		else
		{
			ANI_MOVE_IDLE = ANI_MOVE_IDLE_ORI;
		}
	}

    public void WeaponReload()
    {
        if (!CanStartReload())
            return;

        if (cur_weapon.weapon_data.Reload())
        {
            SetFireState(RELOAD_STATE);
        }
    }

    public bool EnableReload()
    {
        RPGController rpg = cur_weapon as RPGController;
        if (rpg != null && rpg.IsShootAnimationLocked())
        {
            Debug.Log("Reload blocked: RPG shoot animation still playing.");
            return false;
        }

        return cur_weapon.weapon_data.EnableReload();
    }

    public bool IsReloading()
    {
        if (fire_state == RELOAD_STATE)
        {
            string reloadAnim = GetFireStateAnimation(move_state, fire_state);
            float progress = AnimationUtil.GetAnimationPlayedPercentage(this.gameObject, reloadAnim);
            if (progress < 0.85f)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanStartReload()
    {
        RPGController rpg = cur_weapon as RPGController;
        if (rpg != null && rpg.IsShootAnimationLocked())
        {
            Debug.Log("Reload blocked: RPG shoot animation still playing.");
            return false;
        }

        if (IsReloading())
        {
            Debug.Log("Reload blocked: reload animation not finished.");
            return false;
        }

        return cur_weapon.weapon_data.EnableReload();
    }

    public bool EnableWeaponUIProcess()
	{
		return true;
	}

	public void ResetSpineRot(float deltaTime)
	{
		if (move_state != null && fire_state != null)
		{
			angle_h_spine = Mathf.MoveTowards(angle_h_spine, 0f, deltaTime * spine_rot_speed);
			angle_v_spine = Mathf.MoveTowards(angle_v_spine, 0f, deltaTime * spine_rot_speed);
			angle_h_head_spine = Mathf.MoveTowards(angle_h_head_spine, 0f, deltaTime * spine_rot_speed);
			angle_v_head_spine = Mathf.MoveTowards(angle_v_head_spine, 0f, deltaTime * spine_rot_speed);
			spine.transform.localRotation = spine_ori_rot;
			head_spine.transform.localRotation = head_spine_ori_rot;
			spine.transform.Rotate(base.transform.right, angle_v_spine, Space.World);
			spine.transform.Rotate(base.transform.up, angle_h_spine, Space.World);
		}
	}

	public void ResetSpineRotFire()
	{
		if (!(spine == null) && move_state != null && fire_state != null && (fire_state == SHOOT_STATE || fire_state == AFTER_SHOOT_STATE))
		{
			spine.transform.localRotation = spine_ori_rot;
			head_spine.transform.localRotation = head_spine_ori_rot;
			angle_h_spine = 0f;
			angle_v_spine = 0f - GameSceneController.Instance.main_camera.AngleV;
			angle_h_head_spine = 0f;
			angle_v_head_spine = 0f;
			spine.transform.Rotate(base.transform.right, angle_v_spine, Space.World);
			spine.transform.Rotate(base.transform.up, angle_h_spine, Space.World);
			head_spine.transform.Rotate(base.transform.right, angle_v_head_spine, Space.World);
			head_spine.transform.Rotate(base.transform.up, angle_h_head_spine, Space.World);
		}
	}

	public virtual void UpdateRunAni(float deltaTime)
	{
		float num = 0f;
		if (character_motor != null)
		{
			num = Vector3.Angle(base.transform.forward, character_motor.desiredMovementDirection);
			if (num <= 60f)
			{
				ANI_MOVE_RUN = ANI_MOVE_RUN_FORWORD;
				angle_h_spine = Mathf.MoveTowards(angle_h_spine, 0f, deltaTime * spine_rot_speed);
				angle_v_spine = Mathf.MoveTowards(angle_v_spine, 15f, deltaTime * spine_rot_speed);
				angle_h_head_spine = Mathf.MoveTowards(angle_h_head_spine, 0f, deltaTime * spine_rot_speed);
				angle_v_head_spine = Mathf.MoveTowards(angle_v_head_spine, -10f, deltaTime * spine_rot_speed);
			}
			else if (num >= 140f)
			{
				ANI_MOVE_RUN = ANI_MOVE_RUN_BACK;
				angle_h_spine = Mathf.MoveTowards(angle_h_spine, 0f, deltaTime * spine_rot_speed);
				angle_v_spine = Mathf.MoveTowards(angle_v_spine, -10f, deltaTime * spine_rot_speed);
				angle_h_head_spine = Mathf.MoveTowards(angle_h_head_spine, 0f, deltaTime * spine_rot_speed);
				angle_v_head_spine = Mathf.MoveTowards(angle_v_head_spine, 0f, deltaTime * spine_rot_speed);
			}
			else if (base.transform.InverseTransformDirection(character_motor.desiredMovementDirection).x > 0f)
			{
				ANI_MOVE_RUN = ANI_MOVE_RUN_RIGHT;
				angle_h_spine = Mathf.MoveTowards(angle_h_spine, 10f, deltaTime * spine_rot_speed);
				angle_v_spine = Mathf.MoveTowards(angle_v_spine, 10f, deltaTime * spine_rot_speed);
				angle_h_head_spine = Mathf.MoveTowards(angle_h_head_spine, 0f, deltaTime * spine_rot_speed);
				angle_v_head_spine = Mathf.MoveTowards(angle_v_head_spine, -10f, deltaTime * spine_rot_speed);
			}
			else
			{
				ANI_MOVE_RUN = ANI_MOVE_RUN_LEFT;
				angle_h_spine = Mathf.MoveTowards(angle_h_spine, -20f, deltaTime * spine_rot_speed);
				angle_v_spine = Mathf.MoveTowards(angle_v_spine, 10f, deltaTime * spine_rot_speed);
				angle_h_head_spine = Mathf.MoveTowards(angle_h_head_spine, 0f, deltaTime * spine_rot_speed);
				angle_v_head_spine = Mathf.MoveTowards(angle_v_head_spine, -10f, deltaTime * spine_rot_speed);
			}
			if (FireState == SHOOT_STATE || FireState == AFTER_SHOOT_STATE)
			{
				angle_h_spine = Mathf.MoveTowards(angle_h_spine, 0f, deltaTime * spine_rot_speed);
				angle_v_spine = Mathf.MoveTowards(angle_v_spine, 0f, deltaTime * spine_rot_speed);
				angle_h_head_spine = Mathf.MoveTowards(angle_h_head_spine, 0f, deltaTime * spine_rot_speed);
				angle_v_head_spine = Mathf.MoveTowards(angle_v_head_spine, 0f, deltaTime * spine_rot_speed);
			}
			spine.transform.localRotation = spine_ori_rot;
			head_spine.transform.localRotation = head_spine_ori_rot;
			spine.transform.Rotate(base.transform.right, angle_v_spine, Space.World);
			spine.transform.Rotate(base.transform.up, angle_h_spine, Space.World);
			head_spine.transform.Rotate(base.transform.right, angle_v_head_spine, Space.World);
			head_spine.transform.Rotate(base.transform.up, angle_h_head_spine, Space.World);
		}
	}

	public void OnWeaponReload()
	{
		cur_weapon.OnWeaponReload();
		if (avatar_data.avatar_type == AvatarType.Cowboy && (cur_weapon.weapon_type == WeaponType.RocketLauncher || cur_weapon.weapon_type == WeaponType.PGM))
		{
			AnimationUtil.Stop(CowboyCap);
			AnimationUtil.PlayAnimate(CowboyCap, "RPG_Reload01", WrapMode.Once);
		}
	}

	public void ResetWeaponFireReadyState()
	{
		if (cur_weapon != null)
		{
			cur_weapon.ResetWeaponFireReadyState();
		}
	}

	public void OnEnterAfterFire()
	{
		if (cur_weapon != null)
		{
			cur_weapon.StopFire(this);
			cur_weapon.OnEnterAfterFire(this);
		}
	}

	public virtual void SetAfterFireTime()
	{
		((PlayerAfterShootState)AFTER_SHOOT_STATE).interval_time = cur_weapon.GetAfterFireTime();
	}

	public void Recover(float hp)
	{
		avatar_data.cur_hp += hp;
		avatar_data.cur_hp = Mathf.Clamp(avatar_data.cur_hp, 0f, avatar_data.hp_capacity);
		UpdateHpBar();
		GameObject gameObject = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[5], base.transform.position, Quaternion.identity) as GameObject;
		gameObject.transform.parent = base.transform;
		RemoveTimerScript removeTimerScript = gameObject.AddComponent<RemoveTimerScript>();
		removeTimerScript.life = 3f;
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
		//{
		//	SFSArray sFSArray = new SFSArray();
		//	sFSArray.AddFloat(avatar_data.cur_hp);
		//	sFSArray.AddFloat(avatar_data.hp_capacity);
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutSFSArray("playerRecover", sFSArray);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
	}

	public virtual void UpdateHpBar()
	{
		GameSceneController.Instance.SetHpBar(avatar_data.cur_hp / avatar_data.hp_capacity);
	}

	public void AddExp(int exp)
	{
		if (avatar_data.AddExp(exp))
		{
			UpdateHpBar();
			GameSceneController.Instance.SetLevelLabel(avatar_data.level);
			GameObject gameObject = GameSceneController.Instance.level_up_pool.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
			gameObject.transform.parent = base.transform;
		}
		GameSceneController.Instance.SetExpBar((float)avatar_data.cur_exp.GetIntVal() / (float)avatar_data.next_level_exp.GetIntVal());
	}

	public void FinishSwitchWeaponFire()
	{
		if (cur_weapon != null)
		{
			cur_weapon.switch_ready_fire = true;
			CouldEnterAfterFire = true;
		}
	}

	public virtual bool FinishFireAni()
	{
		return cur_weapon.FinishFireAni(this);
	}

	public void EnterAfterFire()
	{
		if (!cur_weapon.HaveBullets() && cur_weapon.weapon_data.EnableReload())
		{
			WeaponReload();
		}
		else if (could_enter_after_fire)
		{
			SetFireState(AFTER_SHOOT_STATE);
		}
		else if (fire_state.GetStateType() != AFTER_SHOOT_STATE.GetStateType())
		{
			ResetFireIdle();
		}
	}

	public void CheckFireEndState()
	{
		if (cur_weapon.HaveBullets())
		{
			SetFireState(SHOOT_STATE);
		}
		else if (cur_weapon.weapon_data.EnableReload())
		{
			WeaponReload();
		}
		else
		{
			ResetFireIdle();
		}
	}

	public void ResetSightOriPos()
	{
		if (cur_weapon != null)
		{
			float sightOriPos = Mathf.Min(cur_weapon.weapon_data.config.stretchRangeOffset, cur_weapon.weapon_data.stretch_max);
			GameSceneController.Instance.SightBead.SetSightOriPos(sightOriPos);
		}
	}

	public void ResetSightOriPosRun()
	{
		if (cur_weapon != null)
		{
			float sightOriPos = Mathf.Min(cur_weapon.weapon_data.config.stretchRangeRunOffset, cur_weapon.weapon_data.stretch_max);
			GameSceneController.Instance.SightBead.SetSightOriPos(sightOriPos);
		}
	}

	public void ResetSightTexture()
	{
		if (cur_weapon != null)
		{
			string empty = string.Empty;
			Quaternion identity = Quaternion.identity;
			if (cur_weapon.weapon_type == WeaponType.RocketLauncher || cur_weapon.weapon_type == WeaponType.PGM || cur_weapon.weapon_type == WeaponType.Laser)
			{
				empty = "zhunxin-2";
				identity = Quaternion.identity;
			}
			else if (cur_weapon.weapon_type == WeaponType.ShotGun || cur_weapon.weapon_type == WeaponType.Gatling)
			{
				empty = "zhunxin-3";
				identity = Quaternion.Euler(new Vector3(0f, 0f, 45f));
			}
			else
			{
				empty = "zhunxin-1";
				identity = Quaternion.identity;
			}
			GameSceneController.Instance.SightBead.ResetSightTexture(empty, identity);
		}
	}

	public void DoAutoLock(float deltaTime, WeaponController weapon)
	{
		GameSceneController.Instance.DoAutoLock(deltaTime, weapon);
	}

	protected void ComboDologic(float deltaTime)
	{
		if (cur_combo_type == ComboBuffType.SuperMan)
		{
			cur_combo_val -= deltaTime * 0.75f * (float)(cur_combo_type + 1);
		}
		else
		{
			cur_combo_val -= deltaTime * 0.75f * (float)cur_combo_type;
		}
		if (cur_combo_val <= 0f)
		{
			cur_combo_val = 0f;
			if (cur_combo_type != ComboBuffType.None)
			{
				cur_combo_type = ComboBuffType.None;
				OnComboBuffStop();
			}
		}
		cur_combo_update_time += deltaTime;
		if (cur_combo_update_time >= combo_update_rate)
		{
			if (cur_combo_val != last_combo_val)
			{
				GameSceneController.Instance.SetComboBar(cur_combo_val / cur_combo_limit);
			}
			last_combo_val = cur_combo_val;
			cur_combo_update_time = 0f;
		}
	}

	public void AddComboValue(float value)
	{
		cur_combo_val += value;
		if (cur_combo_val > combo_limit_max)
		{
			cur_combo_val = combo_limit_max;
		}
		if (cur_combo_val >= combo_limit_super)
		{
			if (cur_combo_type < ComboBuffType.SuperMan)
			{
				cur_combo_type = ComboBuffType.SuperMan;
				OnComboBuffSuperman();
			}
		}
		else if (cur_combo_val >= combo_limit_strong)
		{
			if (cur_combo_type < ComboBuffType.Strong)
			{
				cur_combo_type = ComboBuffType.Strong;
				OnComboBuffStrong();
			}
		}
		else if (cur_combo_val >= combo_limit_fast && cur_combo_type < ComboBuffType.Fast)
		{
			cur_combo_type = ComboBuffType.Fast;
			OnComboBuffFast();
		}
	}

	public void ResetWeaponsComboRate()
	{
		foreach (WeaponController item in primary_weapon)
		{
			if (item != null)
			{
				item.ComboRateRatio = combo_fire_rate_ratio;
				item.ResetFireAniSpeed(base.gameObject);
			}
		}
	}

	protected void OnComboBuffFast()
	{
		combo_fire_rate_ratio = GameConfig.Instance.Player_Combo_Buff_Info[1].rate_ratio;
		combo_fire_damage_ratio = GameConfig.Instance.Player_Combo_Buff_Info[1].damage_ratio;
		ResetWeaponsComboRate();
		cur_combo_limit = combo_limit_strong;
		GameSceneController.Instance.SetComboLabel(cur_combo_type.ToString());
		GameSceneController.Instance.SetComboBarStar(1);
		GameObject gameObject = GameSceneController.Instance.combo_get_pool1.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
		gameObject.transform.parent = base.transform;
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
		//{
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutShort("playerComboBuff", 1);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
	}

	protected void OnComboBuffStrong()
	{
		combo_fire_rate_ratio = GameConfig.Instance.Player_Combo_Buff_Info[2].rate_ratio;
		combo_fire_damage_ratio = GameConfig.Instance.Player_Combo_Buff_Info[2].damage_ratio;
		ResetWeaponsComboRate();
		cur_combo_limit = combo_limit_super;
		GameSceneController.Instance.SetComboLabel(cur_combo_type.ToString());
		GameSceneController.Instance.SetComboBarStar(2);
		GameObject gameObject = GameSceneController.Instance.combo_get_pool2.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
		gameObject.transform.parent = base.transform;
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
		//{
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutShort("playerComboBuff", 2);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
	}

	protected void OnComboBuffSuperman()
	{
		combo_fire_rate_ratio = GameConfig.Instance.Player_Combo_Buff_Info[3].rate_ratio;
		combo_fire_damage_ratio = GameConfig.Instance.Player_Combo_Buff_Info[3].damage_ratio;
		ResetWeaponsComboRate();
		cur_combo_limit = combo_limit_max;
		GameSceneController.Instance.SetComboLabel(cur_combo_type.ToString());
		GameSceneController.Instance.SetComboBarStar(3);
		GameObject gameObject = GameSceneController.Instance.combo_get_pool3.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
		gameObject.transform.parent = base.transform;
		combo_buff_obj.SetActive(true);
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
		//{
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutShort("playerComboBuff", 3);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
	}

	protected void OnComboBuffStop()
	{
		combo_fire_rate_ratio = 1f;
		combo_fire_damage_ratio = 1f;
		ResetWeaponsComboRate();
		cur_combo_limit = combo_limit_fast;
		GameSceneController.Instance.SetComboLabel(cur_combo_type.ToString());
		GameSceneController.Instance.SetComboBarStar(0);
		combo_buff_obj.SetActive(false);
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
		//{
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutShort("playerComboBuff", 0);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
	}

	public void ReleaseChaisawTarget()
	{
		if (chaisaw_target_enemy != null)
		{
			chaisaw_target_enemy.OnChaisawInjuredResease(this);
		}
		ChaisawController chaisawController = cur_weapon as ChaisawController;
		if (chaisawController != null)
		{
			chaisawController.EnableChaisawBlood(false);
		}
	}

	public bool EnableControll()
	{
		if (fire_state.GetStateType() == DEAD_STATE.GetStateType() || fire_state.GetStateType() == IDLE_SHOOT_LOCK_STATE.GetStateType())
		{
			return false;
		}
		return true;
	}

	public virtual void OnFireRelease()
	{
		cur_weapon.OnFireRelease(this);
	}

    public virtual void OnGetItem(GameItemController item)
    {
        Debug.Log("player get item:" + item.item_type);
        if (item.item_type == ItemType.Gold)
        {
            GameData.Instance.total_cash.SetIntVal(
                GameData.Instance.total_cash.GetIntVal() + GameConfig.Instance.cash_package_val,
                GameDataIntPurpose.Cash
            );
            GameObject gameObject = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[12], base.transform.position, Quaternion.identity) as GameObject;
            gameObject.transform.parent = base.transform;
        }
        else if (item.item_type == ItemType.Hp)
        {
            Recover(avatar_data.hp_capacity * GameConfig.Instance.hp_package_ratio);
        }
        else if (item.item_type == ItemType.Crystal)
        {
            GameData.Instance.total_crystal.SetIntVal(
                GameData.Instance.total_crystal.GetIntVal() + GameConfig.Instance.crystal_package_val,
                GameDataIntPurpose.Crystal
            );
            GameObject gameObject = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[12], base.transform.position, Quaternion.identity) as GameObject;
            gameObject.transform.parent = base.transform;
        }
        else if (item.item_type == ItemType.Bullet_PrimaryWeapon)
        {
            int ammoToAdd = CalculateAmmoFromBulletRatio();
            Debug.Log("add bullet:" + ammoToAdd);
            CurPrimaryWeapon.weapon_data.AddBullet(ammoToAdd);
            GameObject gameObject2 = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[11], base.transform.position, Quaternion.identity) as GameObject;
            gameObject2.transform.parent = base.transform;
            RemoveTimerScript removeTimerScript2 = gameObject2.AddComponent<RemoveTimerScript>();
            removeTimerScript2.life = 3f;
            UpdateWeaponUIShow();
        }
    }


    private int CalculateAmmoFromBulletRatio()
    {
        if (CurPrimaryWeapon == null || CurPrimaryWeapon.weapon_data == null || CurPrimaryWeapon.weapon_data.config == null)
        {
            Debug.LogWarning("CalculateAmmoFromBulletRatio: CurPrimaryWeapon or config is null");
            return 0;
        }
        int baseBulletCount = CurPrimaryWeapon.weapon_data.config.buy_bullet_count;
        float ratio = GameConfig.Instance.bullet_package_ratio;
        return (int)(baseBulletCount * ratio);
    }


    public virtual void PlayBaseBallEff()
	{
		if (eff_base_ball != null)
		{
			eff_base_ball.SetActive(true);
			AnimationUtil.PlayAnimate(eff_base_ball, "klight_C", WrapMode.Once);
		}
	}

	public virtual void StopBaseBallEff()
	{
		if (eff_base_ball != null)
		{
			AnimationUtil.Stop(eff_base_ball);
			eff_base_ball.SetActive(false);
		}
	}

	public virtual void PlayPlayerAudio(string audio_name)
	{
		if (Taudio_controller == null)
		{
			Taudio_controller = GetComponent<TAudioController>();
		}
		Taudio_controller.PlayAudio(audio_name);
	}

	public virtual void StopPlayerAudio(string audio_name)
	{
		if (Taudio_controller == null)
		{
			Taudio_controller = GetComponent<TAudioController>();
		}
		Taudio_controller.StopAudio(audio_name);
	}

	public override void Rebirth()
	{
		avatar_data.cur_hp = avatar_data.hp_capacity;
		avatar_data.cur_hp = Mathf.Clamp(avatar_data.cur_hp, 0f, avatar_data.hp_capacity);
		UpdateHpBar();
		SetMoveState(IDLE_STATE);
		ResetFireIdle();
		UpdateWeaponUIShow();
		OnRebirthBuffStart();
		if (avatar_data.avatar_type == AvatarType.Swat)
		{
			ShowSwatShieldEff();
		}
		//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
		//{
		//	SFSArray sFSArray = new SFSArray();
		//	sFSArray.AddFloat(avatar_data.cur_hp);
		//	sFSArray.AddFloat(avatar_data.hp_capacity);
		//	SFSObject sFSObject = new SFSObject();
		//	sFSObject.PutSFSArray("playerRebirth", sFSArray);
		//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		//}
	}

	public void ResetFireIdle(bool reset_weapon = true)
	{
		if (fire_state == null || fire_state.GetStateType() != IDLE_SHOOT_STATE.GetStateType())
		{
			if (reset_weapon)
			{
				StopFire();
				ResetFireWeapon();
				GetMoveIdleAnimation(IDLE_SHOOT_STATE);
				ResetWeaponFireReadyState();
			}
			else
			{
				StopFire();
			}
			SetFireState(IDLE_SHOOT_STATE);
		}
	}

	protected virtual void ResetRenderSet()
	{
	}

	public void OnRebirthBuffStart()
	{
		is_rebirth_buff = true;
		foreach (Renderer item in avatar_render_set)
		{
			item.material.shader = alpha_shader;
			item.GetComponent<ShaderColorFlash>().StartFlash(GameConfig.Instance.rebirth_god_time, null);
		}
		Invoke("OnRebirthBuffEnd", GameConfig.Instance.rebirth_god_time);
	}

	public void OnRebirthBuffEnd()
	{
		is_rebirth_buff = false;
		foreach (Renderer item in avatar_render_set)
		{
			item.material.shader = diffuse_shader;
		}
	}

	public void ShowSwatShieldEff()
	{
	}

	public void HideSwatShieldEff()
	{
	}

	public void ShowSwatBreakShieldEff()
	{
	}

	public void HideSwatBreakShieldEff()
	{
	}

	public void StartLimitMove(float limit_time)
	{
        CancelInvoke("CancelLimitMove");
        is_limit_move = true;
		Invoke("CancelLimitMove", limit_time);
	}

	public void CancelLimitMove()
	{
		is_limit_move = false;
	}

	public void SetLimitMove(bool status)
	{
		is_limit_move = status;
	}

	public virtual void ConjureSkill(int index)
	{
		if (fire_state.GetStateType() == DEAD_STATE.GetStateType() || fire_state.GetStateType() == IDLE_SHOOT_LOCK_STATE.GetStateType() || index >= skill_set.Count)
		{
			return;
		}
		string text = avatar_data.skill_list[index];
		if (skill_set.ContainsKey(text) && skill_set[text].ConjureSkill())
		{
			//Debug.Log("ConjureSkill:" + text);
			//if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
			//{
			//	SFSObject sFSObject = new SFSObject();
			//	sFSObject.PutUtfString("skill", text);
			//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
			//}
		}
	}

	public virtual void ConjureSkill(string skill_name)
	{
		if (skill_set.ContainsKey(skill_name) && !skill_set[skill_name].ConjureSkill())
		{
			//Debug.LogWarning("ConjureSkill:" + skill_name + " failed!");
		}
	}

	public virtual void UpdateNetworkTrans(NetworkTransform net_trans)
	{
	}

	public virtual void SetMoveStateType(PlayerStateType state_type)
	{
		if (player_state_set.ContainsKey(state_type))
		{
			SetMoveState(player_state_set[state_type]);
		}
	}

	public virtual void SetFireStateType(PlayerStateType state_type)
	{
		if (player_state_set.ContainsKey(state_type))
		{
			SetFireState(player_state_set[state_type]);
		}
	}

	public virtual void SetIdleLockState(bool status)
	{
		if (fire_state.GetStateType() != DEAD_STATE.GetStateType())
		{
			if (status)
			{
				SetMoveState(IDLE_LOCK_STATE);
				SetFireState(IDLE_SHOOT_LOCK_STATE);
			}
			else
			{
				SetMoveState(IDLE_STATE);
				SetFireState(IDLE_SHOOT_STATE);
			}
		}
	}
}
