using UnityEngine;

public class SkillBaseballRobotCoopController : SkillCoopController
{
	protected MikeCoopController mike;

	public override void Init(SkillData data, ObjectController owner)
	{
		base.Init(data, owner);
		mike = owner as MikeCoopController;
		if (mike == null)
		{
			Debug.LogError("SkillBaseballRobotController owner error.");
		}
		skill_data.damage_val = 0f;
	}

	public override void Dologic(float deltaTime)
	{
		base.Dologic(deltaTime);
	}

	public override void OnSkillConjure()
	{
		base.OnSkillConjure();
		mike.SetFireState(mike.BASEBALL_ROBOT_SKILL_STATE);
		Debug.Log("SkillBaseballRobotController");
	}
}
