using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class CowboyController : PlayerController
{
	public const string ANI_SCARECROW = "Avatar_Cowboy_Skill01";

	public GameObject scarecrow_obj;

	public PlayerState SCARECROW_SKILL_STATE;

	protected override void Awake()
	{
		base.Awake();
		SCARECROW_SKILL_STATE = PlayerState.Create(PlayerStateType.Scarecrow, this);
	}

	public override void SetAvatarData(AvatarData data)
	{
		cowboy_cap = base.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Head_0/Bip01 Head/Cowboy01_Low").gameObject;
		base.SetAvatarData(data);
	}

	protected override void ResetRenderSet()
	{
		List<Transform> list = new List<Transform>();
		ShaderColorFlash shaderColorFlash = null;
		list.Add(base.transform.Find("Avatar_Cowboy_H_Body"));
		list.Add(base.transform.Find("Avatar_Cowboy_H_Head"));
		list.Add(base.transform.Find("Avatar_Cowboy_H_Neck"));
		list.Add(base.transform.Find("Avatar_Cowboy_H_02"));
		list.Add(base.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Head_0/Bip01 Head/Cowboy01_Low/Cowboy_01_High"));
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

	public void OnScarecrowSkillStart()
	{
		if (cur_guardian_obj != null)
		{
			GuardianForceController component = cur_guardian_obj.GetComponent<GuardianForceController>();
			component.StartCoroutine(component.RemoveOnTime(0.1f));
		}
		cur_guardian_obj = Object.Instantiate(scarecrow_obj) as GameObject;
		cur_guardian_obj.transform.parent = base.transform;
		cur_guardian_obj.transform.localPosition = new Vector3(0f, 0f, 0f);
		cur_guardian_obj.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		cur_guardian_obj.GetComponent<GuardianForceController>().SetSkillController(skill_set["Scarecrow"], this);
		SetLimitMove(true);
	}

	public void OnScarecrowSkillEnd()
	{
		cur_guardian_obj.transform.parent = null;
		SetLimitMove(false);
	}
}
