using UnityEngine;

public class SkillWhirlwinController : SkillController
{
	protected HumanController human;

	protected float cur_whirlwind_time;

	protected float damage_val;

	public override void Init(SkillData data, ObjectController owner)
	{
		base.Init(data, owner);
		human = owner as HumanController;
		if (owner == null)
		{
			Debug.LogError("SkillWhirlwinController owner error.");
		}
		damage_val = skill_data.damage_val / skill_data.life_time * skill_data.frequency_val;
		Debug.Log("SkillWhirlwinController damage_val:" + damage_val);
	}

	public override void Dologic(float deltaTime)
	{
		base.Dologic(deltaTime);
	}

	public override void OnSkilllogic(float deltaTime)
	{
		cur_whirlwind_time += deltaTime;
		if (cur_whirlwind_time >= skill_data.frequency_val)
		{
			cur_whirlwind_time = 0f;
			CheckSkillHit();
		}
	}

	public override void OnSkillConjure()
	{
		base.OnSkillConjure();
		human.SetFireState(human.WHIRLWIND_SKILL_STATE);
	}

	public void CheckSkillHit()
	{
		Vector3 down = Vector3.down;
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			if (Vector3.Distance(human.centroid, value.centroid) < skill_data.range_val)
			{
				value.OnHit(damage_val, null, human, value.centroid, down);
			}
		}
		foreach (GameObject item in GameSceneController.Instance.wood_box_list)
		{
			WoodBoxController component = item.GetComponent<WoodBoxController>();
			if (component != null && (component.centroid - human.centroid).sqrMagnitude < skill_data.range_val * skill_data.range_val && !GameSceneController.CheckBlockBetween(human.centroid, component.centroid))
			{
				component.OnHit(damage_val, null, human, component.centroid, human.centroid - component.centroid);
			}
		}
	}
}
