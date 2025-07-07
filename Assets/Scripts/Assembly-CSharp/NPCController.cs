using CoMZ2;
using UnityEngine;

public class NPCController : ObjectController
{
	public string ANI_RESCUE = "Fear01";

	public string ANI_IDLE = "Idle01";

	public string ANI_RUN = "Forward01";

	public string ANI_INJURED = "Damage01";

	public string ANI_DEAD = "Death01";

	public string ANI_FEAR = "Fear01";

	public string ani_injured = "Damage01";

	public string ani_injured_res = "Take_Damage01";

	public NPCState IDLE_STATE;

	public NPCState RUN_STATE;

	public NPCState INJURED_STATE;

	public NPCState DEAD_STATE;

	public NPCState FEAR_STATE;

	public NPCState FINISH_STATE;

	protected NPCState npcState;

	public NPCData npc_data;

	public ControllerNavPath nav_pather;

	public int npc_id;

	protected float injured_time;

	public SceneTUIMeshSprite hp_bar;

	public TUIRect hp_rect;

	protected float hp_width = 49f;

	protected float last_hp_update_time;

	protected bool is_hp_update;

	protected float hp_target_width;

	protected float hp_ori_width;

	public float hp_lerp_speed = 2f;

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

    public NPCState Npc_State
	{
		get
		{
			return npcState;
		}
	}

	public float LastInjuredTime
	{
		get
		{
			return injured_time;
		}
	}

	protected override void Start()
	{
		nav_pather = GetComponent<ControllerNavPath>();
		nav_pather.SetSpeed(npc_data.move_speed);
		Init();
		hp_bar = base.transform.Find("Hp_Bar").GetComponent<SceneTUIMeshSprite>();
		hp_rect = hp_bar.m_showClipObj.GetComponent<TUIRect>();
		SetHpBar(1f);
	}

	protected override void Update()
	{
		Dologic(Time.deltaTime);
	}

	public virtual void Init()
	{
		IDLE_STATE = NPCState.Create(NPCStateType.Idle, this);
		RUN_STATE = NPCState.Create(NPCStateType.Run, this);
		INJURED_STATE = NPCState.Create(NPCStateType.Injured, this);
		DEAD_STATE = NPCState.Create(NPCStateType.Dead, this);
		FEAR_STATE = NPCState.Create(NPCStateType.Fear, this);
		FINISH_STATE = NPCState.Create(NPCStateType.Finish, this);
		npcState = IDLE_STATE;
		controller_type = ControllerType.Npc;
	}

	public virtual void Dologic(float deltaTime)
	{
		if (npcState != null)
		{
			npcState.DoStateLogic(deltaTime);
		}
		DetermineState();
		UpdateHpBar();
	}

	public virtual void DetermineState()
	{
	}

	public override void OnDead(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
	}

	public virtual void SetState(NPCState state)
	{
		if (npcState != null && npcState.GetStateType() != state.GetStateType())
		{
			npcState.OnExitState();
			npcState = state;
			npcState.OnEnterState();
		}
	}

	public virtual bool FinishedInjuredCD()
	{
		if (Time.time - LastInjuredTime >= base.GetComponent<Animation>()[ANI_INJURED].clip.length)
		{
			return true;
		}
		return false;
	}

	public void SetNpcData(NPCData data)
	{
		npc_data = data;
	}

	public virtual void SetPathCatchState(bool status)
	{
		if (status != nav_pather.GetCatchingState())
		{
			nav_pather.Catching(status);
		}
	}

	public virtual string GetAnimationWithState(NPCState state)
	{
		return string.Empty;
	}

	public void UpdateHpBar()
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

	public void SetHpBar(float percent)
	{
		last_hp_update_time = Time.time;
		is_hp_update = true;
		hp_target_width = hp_width * percent;
		hp_ori_width = hp_rect.Size.x;
		GameSceneController.Instance.game_main_panel.SetNpcHpBar(percent);
	}

	public void Recover(float hp)
	{
		npc_data.cur_hp += hp;
		npc_data.cur_hp = Mathf.Clamp(npc_data.cur_hp, 0f, npc_data.hp_capacity);
		SetHpBar(npc_data.cur_hp / npc_data.hp_capacity);
		GameObject gameObject = Object.Instantiate(GameSceneController.Instance.Eff_Accessory[5], base.transform.position, Quaternion.identity) as GameObject;
		gameObject.transform.parent = base.transform;
		RemoveTimerScript removeTimerScript = gameObject.AddComponent<RemoveTimerScript>();
		removeTimerScript.life = 3f;
	}

	public override void Rebirth()
	{
		npc_data.cur_hp += npc_data.hp_capacity;
		npc_data.cur_hp = Mathf.Clamp(npc_data.cur_hp, 0f, npc_data.hp_capacity);
		SetHpBar(npc_data.cur_hp / npc_data.hp_capacity);
		SetState(IDLE_STATE);
	}
}
