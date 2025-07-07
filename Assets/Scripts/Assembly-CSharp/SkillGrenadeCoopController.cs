using UnityEngine;

public class SkillGrenadeCoopController : SkillCoopController
{
	protected SwatCoopController swat;

	public override void Init(SkillData data, ObjectController owner)
	{
		base.Init(data, owner);
		swat = owner as SwatCoopController;
		if (owner == null)
		{
			Debug.LogError("SkillGrenadeCoopController owner error.");
		}
	}

	public override void Dologic(float deltaTime)
	{
		base.Dologic(deltaTime);
	}

	public override void OnSkillConjure()
	{
		base.OnSkillConjure();
		swat.SetFireState(swat.GRENADE_SKILL_STATE);
		Debug.Log("swat OnSkillConjure");
	}
}
