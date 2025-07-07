using UnityEngine;

public class SkillController
{
	public SkillData skill_data;

	public SkillUseState skill_use_state;

	public float cur_cd_time;

	public ObjectController owner_controller;

	protected GameObject skill_ui_obj;

	protected SkillUIController ui_controller;

	public void SetGameUIObj(SkillUIController uicontroller)
	{
		ui_controller = uicontroller;
		ui_controller.PlayReadyEff();
	}

	public virtual void Init(SkillData data, ObjectController owner)
	{
		skill_data = data;
		skill_use_state = SkillUseState.Ready;
		cur_cd_time = 0f;
		owner_controller = owner;
	}

	public virtual void Dologic(float deltaTime)
	{
		if (skill_use_state == SkillUseState.Saving)
		{
			cur_cd_time += deltaTime;
			if (cur_cd_time >= skill_data.cd_interval)
			{
				cur_cd_time = 0f;
				skill_use_state = SkillUseState.Ready;
				ui_controller.PlayReadyEff();
			}
		}
	}

	public virtual void OnSkilllogic(float deltaTime)
	{
	}

	public float SkillTimeForReady()
	{
		if (skill_use_state == SkillUseState.Saving)
		{
			return skill_data.cd_interval - cur_cd_time;
		}
		return 0f;
	}

	public virtual bool ConjureSkill()
	{
		if (skill_use_state == SkillUseState.Ready)
		{
			skill_use_state = SkillUseState.Saving;
			OnSkillConjure();
			return true;
		}
		return false;
	}

	public virtual void OnSkillConjure()
	{
		Debug.Log("OnSkillConjure skill name:" + skill_data.skill_name);
		ui_controller.StopReadyEff();
	}

	public static SkillController CreateSkillController(string skill_name)
	{
		SkillController skillController = null;
		switch (skill_name)
		{
		case "Whirlwind":
			skillController = new SkillWhirlwinController();
			break;
		case "Scarecrow":
			skillController = new SkillScarecrowController();
			break;
		case "Grenade":
			skillController = new SkillGrenadeController();
			break;
		case "BaseballRobot":
			skillController = new SkillBaseballRobotController();
			break;
		case "Enchant":
			skillController = new SkillEnchantController();
			break;
		default:
			skillController = new SkillController();
			Debug.LogError("SkillController create error, type:" + skill_name);
			break;
		}
		return skillController;
	}

	public static SkillCoopController CreateSkillCoopController(string skill_name)
	{
		SkillCoopController result = null;
		switch (skill_name)
		{
		case "Whirlwind":
			result = new SkillWhirlwinCoopController();
			break;
		case "Scarecrow":
			result = new SkillScarecrowCoopController();
			break;
		case "Grenade":
			result = new SkillGrenadeCoopController();
			break;
		case "BaseballRobot":
			result = new SkillBaseballRobotCoopController();
			break;
		case "Enchant":
			result = new SkillEnchantCoopController();
			break;
		default:
			Debug.LogError("SkillCoopController create error, type:" + skill_name);
			break;
		}
		return result;
	}
}
