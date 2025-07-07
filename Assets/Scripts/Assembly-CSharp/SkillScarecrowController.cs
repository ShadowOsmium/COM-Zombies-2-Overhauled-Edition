using UnityEngine;

public class SkillScarecrowController : SkillController
{
	protected CowboyController cowboy;

	public override void Init(SkillData data, ObjectController owner)
	{
		base.Init(data, owner);
		cowboy = owner as CowboyController;
		if (owner == null)
		{
			Debug.LogError("SkillWhirlwinController owner error.");
		}
	}

	public override void Dologic(float deltaTime)
	{
		base.Dologic(deltaTime);
	}

	public override void OnSkillConjure()
	{
		base.OnSkillConjure();
		cowboy.SetFireState(cowboy.SCARECROW_SKILL_STATE);
	}
}
