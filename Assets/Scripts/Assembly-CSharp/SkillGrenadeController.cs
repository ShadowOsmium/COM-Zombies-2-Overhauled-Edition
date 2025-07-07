using UnityEngine;

public class SkillGrenadeController : SkillController
{
	protected SwatController swat;

	public override void Init(SkillData data, ObjectController owner)
	{
		base.Init(data, owner);
		swat = owner as SwatController;
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
		swat.SetFireState(swat.GRENADE_SKILL_STATE);
		Debug.Log("swat OnSkillConjure");
	}
}
