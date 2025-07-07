using UnityEngine;

public class SkillBaseballRobotController : SkillController
{
	protected MikeController mike;

	public override void Init(SkillData data, ObjectController owner)
	{
		base.Init(data, owner);
		mike = owner as MikeController;
		if (mike == null)
		{
			Debug.LogError("SkillBaseballRobotController owner error.");
		}
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
