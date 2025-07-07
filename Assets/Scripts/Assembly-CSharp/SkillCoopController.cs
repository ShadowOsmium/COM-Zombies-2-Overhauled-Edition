public class SkillCoopController : SkillController
{
	public override void Init(SkillData data, ObjectController owner)
	{
		skill_data = data;
		skill_use_state = SkillUseState.Ready;
		cur_cd_time = 0f;
		owner_controller = owner;
	}

	public override void Dologic(float deltaTime)
	{
	}

	public override void OnSkilllogic(float deltaTime)
	{
	}

	public override bool ConjureSkill()
	{
		OnSkillConjure();
		return true;
	}

	public override void OnSkillConjure()
	{
	}
}
