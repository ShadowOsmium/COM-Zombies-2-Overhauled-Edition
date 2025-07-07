using System.Collections;
using CoMZ2;
using UnityEngine;

public class GuardianForceController : ObjectController
{
	public string ANI_IDLE = "Idle01";

	public string ANI_FIRE = "Attack01";

	public string ANI_INJURED = "Damage01";

	public string ANI_DEAD = "Death01";

	public string ANI_SHOW = "Show01";

	public GuardianState IDLE_STATE;

	public GuardianState SHOW_STATE;

	public GuardianState INJURED_STATE;

	public GuardianState DEAD_STATE;

	protected GuardianState guardianState;

	public int guardian_id;

	protected float injured_time;

	protected float cur_life_time;

	protected bool is_life_over;

	protected SkillController m_skill;

	public PlayerController owner_controller;

	public GuardianState Guardian_State
	{
		get
		{
			return guardianState;
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
		Init();
	}

	protected override void Update()
	{
		Dologic(Time.deltaTime);
	}

	protected override void OnDestroy()
	{
	}

	public virtual void Init()
	{
		IDLE_STATE = GuardianState.Create(GuardianStateType.Idle, this);
		INJURED_STATE = GuardianState.Create(GuardianStateType.Injured, this);
		SHOW_STATE = GuardianState.Create(GuardianStateType.Show, this);
		DEAD_STATE = GuardianState.Create(GuardianStateType.Dead, this);
		controller_type = ControllerType.GuardianForce;
		SetState(SHOW_STATE);
		guardian_id = GameSceneController.Instance.GuardianIndex;
		GameSceneController.Instance.GuardianForce_Set.Add(guardian_id, this);
	}

	public virtual void Dologic(float deltaTime)
	{
		if (guardianState != null)
		{
			guardianState.DoStateLogic(deltaTime);
		}
		DetermineState();
	}

	public virtual void DetermineState()
	{
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
	}

	public override void OnDead(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
	}

	public virtual void OnGuardianRemove()
	{
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			value.RemoveTargetFromHateSet(this);
		}
		GameSceneController.Instance.GuardianForce_Set.Remove(guardian_id);
	}

	public virtual void SetState(GuardianState state)
	{
		if (guardianState == null)
		{
			guardianState = state;
			guardianState.OnEnterState();
		}
		else if (guardianState != null && guardianState.GetStateType() != state.GetStateType())
		{
			guardianState.OnExitState();
			guardianState = state;
			guardianState.OnEnterState();
		}
	}

	public virtual void OnGuardianBirth()
	{
	}

	public virtual void FireUpdate(float deltaTime)
	{
	}

	public IEnumerator RemoveOnTime(float time)
	{
		yield return 1;
		OnGuardianRemove();
		yield return new WaitForSeconds(time);
		Object.Destroy(base.gameObject);
	}

	public virtual void SetSkillController(SkillController skill, PlayerController owner)
	{
		m_skill = skill;
		owner_controller = owner;
	}
}
