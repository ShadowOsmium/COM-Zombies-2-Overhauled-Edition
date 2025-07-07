using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class MikeController : PlayerController
{
	public const string ANI_BASE_BALL_ROBOT = "Avatar_Mike_Skill01";

	public GameObject baseball_robot_obj;

	public PlayerState BASEBALL_ROBOT_SKILL_STATE;

	protected override void Awake()
	{
		base.Awake();
		BASEBALL_ROBOT_SKILL_STATE = PlayerState.Create(PlayerStateType.BaseballRobot, this);
	}

	protected override void ResetRenderSet()
	{
		List<Transform> list = new List<Transform>();
		ShaderColorFlash shaderColorFlash = null;
		list.Add(base.transform.Find("Avatar_Mike_H_Body"));
		list.Add(base.transform.Find("Avatar_Mike_H_Head"));
		list.Add(base.transform.Find("Avatar_Mike_H_Neck"));
		list.Add(base.transform.Find("Avatar_Mike_H_01"));
		foreach (Transform item in list)
		{
			if (item != null && item.GetComponent<Renderer>() != null)
			{
				shaderColorFlash = item.gameObject.AddComponent<ShaderColorFlash>();
				shaderColorFlash.start_color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				shaderColorFlash.end_color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 100);
				shaderColorFlash.flash_interval = 0.3f;
				avatar_render_set.Add(item.GetComponent<Renderer>());
			}
		}
	}

	public void OnBaseballRobotSkill()
	{
		if (cur_guardian_obj != null)
		{
			GuardianForceController component = cur_guardian_obj.GetComponent<GuardianForceController>();
			component.StartCoroutine(component.RemoveOnTime(0.1f));
		}
		cur_guardian_obj = Object.Instantiate(baseball_robot_obj) as GameObject;
		cur_guardian_obj.transform.parent = base.transform;
		cur_guardian_obj.transform.localPosition = new Vector3(0.2f, 0f, 0.5f);
		cur_guardian_obj.transform.localRotation = Quaternion.Euler(new Vector3(0f, 30f, 0f));
		cur_guardian_obj.transform.parent = null;
		cur_guardian_obj.GetComponent<GuardianForceController>().SetSkillController(skill_set["BaseballRobot"], this);
		SetLimitMove(true);
	}

	public void OnBaseballRobotSkillEnd()
	{
		SetLimitMove(false);
		if (cur_guardian_obj != null)
		{
			BaseballRobotController component = cur_guardian_obj.GetComponent<BaseballRobotController>();
			component.ActiveRobot();
		}
	}
}
