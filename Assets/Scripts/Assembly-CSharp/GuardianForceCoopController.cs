using CoMZ2;
using UnityEngine;

public class GuardianForceCoopController : GuardianForceController
{
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

	public override void Init()
	{
		IDLE_STATE = GuardianState.Create(GuardianStateType.Idle, this, true);
		INJURED_STATE = GuardianState.Create(GuardianStateType.Injured, this, true);
		SHOW_STATE = GuardianState.Create(GuardianStateType.Show, this, true);
		DEAD_STATE = GuardianState.Create(GuardianStateType.Dead, this, true);
		controller_type = ControllerType.GuardianForce;
		SetState(SHOW_STATE);
		guardian_id = GameSceneController.Instance.GuardianIndex;
		GameSceneController.Instance.GuardianForce_Set.Add(guardian_id, this);
	}

	public override void Dologic(float deltaTime)
	{
		if (guardianState != null)
		{
			guardianState.DoStateLogic(deltaTime);
		}
		DetermineState();
	}

	public override void DetermineState()
	{
	}

	public override void OnGuardianRemove()
	{
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			value.RemoveTargetFromHateSet(this);
		}
		GameSceneController.Instance.GuardianForce_Set.Remove(guardian_id);
	}

	public override void OnGuardianBirth()
	{
	}

	public override void FireUpdate(float deltaTime)
	{
	}

	public override void SetSkillController(SkillController skill, PlayerController owner)
	{
		m_skill = skill;
		owner_controller = owner;
	}
}
