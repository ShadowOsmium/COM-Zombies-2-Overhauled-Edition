using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class HumanCoopController : PlayerCoopController
{
	public const string ANI_WHIRLWIND = "Electric_saw_Skill01";

	public PlayerState WHIRLWIND_SKILL_STATE;

	protected GameObject whirlwind_obj;

	public GameObject chaisaw_ref;

	protected GameObject chaisaw_obj;

	protected override void Awake()
	{
		base.Awake();
		WHIRLWIND_SKILL_STATE = PlayerState.Create(PlayerStateType.Whirlwind, this, true);
		whirlwind_obj = base.transform.Find("xuanfengzhan").gameObject;
		if (chaisaw_ref != null)
		{
			chaisaw_obj = Object.Instantiate(chaisaw_ref) as GameObject;
			Transform transform = base.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 Prop1");
			chaisaw_obj.transform.position = transform.position;
			chaisaw_obj.transform.parent = transform;
			chaisaw_obj.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
		}
		HideWhirlwindEff();
	}

	protected override void ResetRenderSet()
	{
		List<Transform> list = new List<Transform>();
		ShaderColorFlash shaderColorFlash = null;
		list.Add(base.transform.Find("Avatar_Joe_H_Body"));
		list.Add(base.transform.Find("Avatar_Joe_H_Head"));
		list.Add(base.transform.Find("Avatar_Joe_H_Neck"));
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

	public void OnWhirlwindLogic(float deltaTime)
	{
		if (skill_set.ContainsKey("Whirlwind"))
		{
			skill_set["Whirlwind"].OnSkilllogic(deltaTime);
		}
	}

	public void ShowWhirlwindEff()
	{
		whirlwind_obj.SetActive(true);
		whirlwind_obj.GetComponent<ParticleSystem>().Play();
		chaisaw_obj.SetActive(true);
	}

	public void HideWhirlwindEff()
	{
		whirlwind_obj.GetComponent<ParticleSystem>().Stop();
		whirlwind_obj.GetComponent<ParticleSystem>().Clear();
		whirlwind_obj.SetActive(false);
		chaisaw_obj.SetActive(false);
	}
}
