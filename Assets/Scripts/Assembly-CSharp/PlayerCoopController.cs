using System.Collections;
using CoMZ2;
using UnityEngine;

public class PlayerCoopController : PlayerController
{
	protected NetworkTransformInterpolation net_trans_interpolation;

	public UITeammateBar ui_teammate_bar;

	protected override void Awake()
	{
		IDLE_STATE = PlayerState.Create(PlayerStateType.Idle, this, true);
		RUN_STATE = PlayerState.Create(PlayerStateType.Run, this, true);
		IDLE_LOCK_STATE = PlayerState.Create(PlayerStateType.IdleLock, this, true);
		IDLE_SHOOT_STATE = PlayerState.Create(PlayerStateType.IdleShoot, this, true);
		SHOOT_STATE = PlayerState.Create(PlayerStateType.Shoot, this, true);
		AFTER_SHOOT_STATE = PlayerState.Create(PlayerStateType.AfterShoot, this, true);
		RELOAD_STATE = PlayerState.Create(PlayerStateType.Reload, this, true);
		SWITCH_WEAPON_STATE = PlayerState.Create(PlayerStateType.SwitchWeapon, this, true);
		GOTHIT_STATE = PlayerState.Create(PlayerStateType.GotHit, this, true);
		DEAD_STATE = PlayerState.Create(PlayerStateType.Dead, this, true);
		IDLE_SHOOT_LOCK_STATE = PlayerState.Create(PlayerStateType.IdleShootLock, this, true);
		player_state_set.Add(IDLE_STATE.GetStateType(), IDLE_STATE);
		player_state_set.Add(RUN_STATE.GetStateType(), RUN_STATE);
		player_state_set.Add(IDLE_LOCK_STATE.GetStateType(), IDLE_LOCK_STATE);
		player_state_set.Add(IDLE_SHOOT_STATE.GetStateType(), IDLE_SHOOT_STATE);
		player_state_set.Add(SHOOT_STATE.GetStateType(), SHOOT_STATE);
		player_state_set.Add(AFTER_SHOOT_STATE.GetStateType(), AFTER_SHOOT_STATE);
		player_state_set.Add(RELOAD_STATE.GetStateType(), RELOAD_STATE);
		player_state_set.Add(SWITCH_WEAPON_STATE.GetStateType(), SWITCH_WEAPON_STATE);
		player_state_set.Add(GOTHIT_STATE.GetStateType(), GOTHIT_STATE);
		player_state_set.Add(DEAD_STATE.GetStateType(), DEAD_STATE);
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
		net_trans_interpolation = base.gameObject.AddComponent<NetworkTransformInterpolation>();
		base.gameObject.AddComponent<NetworkTransformReceiver>();
		is_myself = false;
	}

	protected override void Start()
	{
		Fire_Light_Wall.enabled = false;
		Fire_Light_Player.enabled = false;
		controller_type = ControllerType.Player;
		base.gameObject.GetComponent<NetworkTransformReceiver>().SetReceive(true);
	}

	protected override void Update()
	{
		if (Time.deltaTime == 0f || Time.timeScale == 0f)
		{
			return;
		}
		if (GameSceneController.Instance.GamePlayingState == PlayingState.Lose || GameSceneController.Instance.GamePlayingState == PlayingState.Win)
		{
			SetMoveState(IDLE_STATE);
			move_state.DoStateLogic(Time.deltaTime);
			fire_state.DoStateLogic(Time.deltaTime);
		}
		else if (!(spine == null))
		{
			if (fire_state != null)
			{
				fire_state.DoStateLogic(Time.deltaTime);
			}
			if (move_state != null && (fire_state == null || fire_state.GetStateType() != PlayerStateType.Whirlwind))
			{
				move_state.DoStateLogic(Time.deltaTime);
			}
			if (character_motor != null && fire_state != DEAD_STATE && is_limit_move)
			{
			}
		}
	}

	public override void Fire(PlayerStateType state_type, float deltaTime)
	{
		CheckFireWeapon();
		cur_weapon.FireUpdate(this, Time.deltaTime);
	}

	public override void StopFire()
	{
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

	public override void SetMoveState(PlayerState state)
	{
		if (move_state == null)
		{
			move_state = state;
			move_state.OnEnterState();
		}
		else if (move_state != null && move_state.GetStateType() != state.GetStateType())
		{
			move_state.OnExitState();
			move_state = state;
			move_state.OnEnterState();
		}
	}

	public override void SetFireState(PlayerState state)
	{
		if (fire_state == null)
		{
			last_fire_state = state;
			fire_state = state;
			fire_state.OnEnterState();
		}
		else if (fire_state != null && fire_state.GetStateType() != state.GetStateType())
		{
			last_fire_state = fire_state;
			fire_state.OnExitState();
			fire_state = state;
			fire_state.OnEnterState();
		}
	}

	public override void SetAvatarData(AvatarData data)
	{
		avatar_data = data;
		avatar_data.ResetData();
		StartCoroutine(ChangMotorSpeed());
		StartCoroutine(InitWeapon());
		ResetRenderSet();
	}

	public new IEnumerator InitWeapon()
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
				weapon2 = WeaponFactory.CreateWeaponCoop(avatar_data.primary_equipment);
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
	}

	public new void ChangeCurWeapon(WeaponController weapon)
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
		GetMoveIdleAnimation(base.FireState);
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
		if (!(avatar_data.cur_hp <= 0f))
		{
			avatar_data.Injured(damage);
			UpdateHpBar();
		}
	}

	public override void OnDead(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
		StopFire();
	}

	public override void CalculateSetFireState()
	{
	}

    public new void WeaponReload()
    {
        RPGCoopController rpg = cur_weapon as RPGCoopController;
        if (rpg != null && rpg.IsShootAnimationLocked())
        {
            Debug.Log("Coop RPG reload blocked: shoot animation still playing.");
            return;
        }

        cur_weapon.weapon_data.Reload();
    }

    public new bool EnableReload()
    {
        RPGCoopController rpg = cur_weapon as RPGCoopController;
        if (rpg != null && rpg.IsShootAnimationLocked())
        {
            Debug.Log("Blocked Coop RPG reload early: shoot animation still playing.");
            return false;
        }

        return cur_weapon.weapon_data.EnableReload();
    }

    public void OnRemoteRecover(float hp, float hp_total)
	{
		avatar_data.cur_hp = hp;
		avatar_data.hp_capacity = hp_total;
		GameObject gameObject = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[5], base.transform.position, Quaternion.identity) as GameObject;
		gameObject.transform.parent = base.transform;
		RemoveTimerScript removeTimerScript = gameObject.AddComponent<RemoveTimerScript>();
		removeTimerScript.life = 3f;
	}

	protected new void OnComboBuffFast()
	{
		combo_fire_rate_ratio = GameConfig.Instance.Player_Combo_Buff_Info[1].rate_ratio;
		combo_fire_damage_ratio = GameConfig.Instance.Player_Combo_Buff_Info[1].damage_ratio;
		ResetWeaponsComboRate();
		cur_combo_limit = combo_limit_strong;
		GameObject gameObject = GameSceneController.Instance.combo_get_pool1.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
		gameObject.transform.parent = base.transform;
	}

	protected new void OnComboBuffStrong()
	{
		combo_fire_rate_ratio = GameConfig.Instance.Player_Combo_Buff_Info[2].rate_ratio;
		combo_fire_damage_ratio = GameConfig.Instance.Player_Combo_Buff_Info[2].damage_ratio;
		ResetWeaponsComboRate();
		cur_combo_limit = combo_limit_super;
		GameObject gameObject = GameSceneController.Instance.combo_get_pool2.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
		gameObject.transform.parent = base.transform;
	}

	protected new void OnComboBuffSuperman()
	{
		combo_fire_rate_ratio = GameConfig.Instance.Player_Combo_Buff_Info[3].rate_ratio;
		combo_fire_damage_ratio = GameConfig.Instance.Player_Combo_Buff_Info[3].damage_ratio;
		ResetWeaponsComboRate();
		cur_combo_limit = combo_limit_max;
		GameObject gameObject = GameSceneController.Instance.combo_get_pool3.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
		gameObject.transform.parent = base.transform;
		combo_buff_obj.SetActive(true);
	}

	protected new void OnComboBuffStop()
	{
		combo_fire_rate_ratio = 1f;
		combo_fire_damage_ratio = 1f;
		ResetWeaponsComboRate();
		cur_combo_limit = combo_limit_fast;
		combo_buff_obj.SetActive(false);
	}

	public override void OnGetItem(GameItemController item)
	{
		Debug.Log("player get item:" + item.item_type);
		if (item.item_type == ItemType.Gold)
		{
			GameObject gameObject = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[12], base.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = base.transform;
			RemoveTimerScript removeTimerScript = gameObject.AddComponent<RemoveTimerScript>();
			removeTimerScript.life = 3f;
		}
		else if (item.item_type == ItemType.Hp)
		{
			GameObject gameObject2 = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[5], base.transform.position, Quaternion.identity) as GameObject;
			gameObject2.transform.parent = base.transform;
			RemoveTimerScript removeTimerScript2 = gameObject2.AddComponent<RemoveTimerScript>();
			removeTimerScript2.life = 3f;
		}
		else if (item.item_type == ItemType.Bullet_PrimaryWeapon)
		{
			GameObject gameObject3 = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[11], base.transform.position, Quaternion.identity) as GameObject;
			gameObject3.transform.parent = base.transform;
            RemoveTimerScript removeTimerScript2 = gameObject3.AddComponent<RemoveTimerScript>();
            removeTimerScript2.life = 3f;
        }
	}

	public override void Rebirth()
	{
		UpdateHpBar();
		SetMoveState(IDLE_STATE);
		ResetFireIdle();
		OnRebirthBuffStart();
	}

	public void OnRemoteRebirth(float hp, float hp_total)
	{
		avatar_data.cur_hp = hp;
		avatar_data.hp_capacity = hp_total;
		Rebirth();
	}

	public override void UpdateHpBar()
	{
		ui_teammate_bar.SetHpBar(avatar_data.cur_hp / avatar_data.hp_capacity);
	}

	public override void UpdateNetworkTrans(NetworkTransform net_trans)
	{
		net_trans_interpolation.ReceivedTransform(NetworkTransform.Clone(net_trans));
	}

	public override void SetAfterFireTime()
	{
		((PlayerAfterShootCoopState)AFTER_SHOOT_STATE).interval_time = cur_weapon.GetAfterFireTime();
	}

	public override void SetFireStateType(PlayerStateType state_type)
	{
		if (player_state_set.ContainsKey(state_type))
		{
			if (state_type == PlayerStateType.IdleShoot)
			{
				ResetFireIdle();
			}
			else
			{
				SetFireState(player_state_set[state_type]);
			}
		}
	}

	public override void InitSkill()
	{
		foreach (string item in avatar_data.skill_list)
		{
			if (item != "null")
			{
				SkillController skillController = SkillController.CreateSkillCoopController(item);
				skillController.Init(GameData.Instance.Skill_Avatar_Set[item], this);
				skill_set[item] = skillController;
			}
		}
	}

	public override void UpdateRunAni(float deltaTime)
	{
		float num = 0f;
		num = Vector3.Angle(base.transform.forward, net_trans_interpolation.MoveDir);
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
		else if (base.transform.InverseTransformDirection(net_trans_interpolation.MoveDir).x > 0f)
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
		if (base.FireState == SHOOT_STATE || base.FireState == AFTER_SHOOT_STATE)
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

	public void OnRemoteInjured(float cur_hp, float hp_total)
	{
		avatar_data.cur_hp = cur_hp;
		avatar_data.hp_capacity = hp_total;
		if (avatar_data.cur_hp < 0f)
		{
			avatar_data.cur_hp = 0f;
		}
		UpdateHpBar();
	}

	public void OnRemoteComboBuff(int val)
	{
		switch (val)
		{
		case 0:
			OnComboBuffStop();
			break;
		case 1:
			OnComboBuffFast();
			break;
		case 2:
			OnComboBuffStrong();
			break;
		case 3:
			OnComboBuffSuperman();
			break;
		}
	}

	public void OnUserExit()
	{
		Object.Destroy(ui_teammate_bar.gameObject);
	}

	public void OnRemoteGetItem(ItemType item_type)
	{
		switch (item_type)
		{
            case ItemType.Gold:
                {
                    GameObject gameObject3 = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[12], base.transform.position, Quaternion.identity) as GameObject;
                    gameObject3.transform.parent = base.transform;
                    RemoveTimerScript removeTimerScript3 = gameObject3.AddComponent<RemoveTimerScript>();
                    removeTimerScript3.life = 3f;
                    break;
                }
            case ItemType.Hp:
                {
                    GameObject gameObject2 = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[5], base.transform.position, Quaternion.identity) as GameObject;
                    gameObject2.transform.parent = base.transform;
                    RemoveTimerScript removeTimerScript2 = gameObject2.AddComponent<RemoveTimerScript>();
                    removeTimerScript2.life = 3f;
                    break;
                }

            case ItemType.Crystal:
                {
                    GameObject crystalFX = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[23], base.transform.position, Quaternion.identity) as GameObject;
                    crystalFX.transform.parent = base.transform;
                    crystalFX.transform.localPosition = Vector3.zero;

                    RemoveTimerScript timer = crystalFX.AddComponent<RemoveTimerScript>();
                    timer.life = 3f;

                    Debug.Log("Crystal prefab instantiated: " + crystalFX.name);
                    Debug.Log("Position: " + crystalFX.transform.position);
                    Debug.Log("Parent: " + crystalFX.transform.parent.name);

                    GameData.Instance.total_crystal.SetIntVal(
                        GameData.Instance.total_crystal.GetIntVal() + 1,
                        GameDataIntPurpose.Crystal
                    );
                    break;
                }
        }
	}
}
