using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class SwatCoopController : PlayerCoopController
{
	public const string ANI_GRENADE = "Avatar_Swat_Skill01";

	public PlayerState GRENADE_SKILL_STATE;

	public GameObject grenade_obj;

	private Transform right_hand;

	protected override void Awake()
	{
		base.Awake();
		GRENADE_SKILL_STATE = PlayerState.Create(PlayerStateType.Grenade, this, true);
		right_hand = base.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand");
	}

	public override void SetAvatarData(AvatarData data)
	{
		base.SetAvatarData(data);
		Invoke("ShowSwatShieldEff", 1f);
	}

	protected override void ResetRenderSet()
	{
		List<Transform> list = new List<Transform>();
		ShaderColorFlash shaderColorFlash = null;
		list.Add(base.transform.Find("Avatar_Swat_H_01_Body"));
		list.Add(base.transform.Find("Avatar_Swat_H_01_Head"));
		list.Add(base.transform.Find("Avatar_Swat_H_01_Neck"));
		list.Add(base.transform.Find("Avatar_Swat_H_01"));
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

	public void SpawnGrenade()
	{
		SkillGrenadeCoopController skillGrenadeCoopController = skill_set["Grenade"] as SkillGrenadeCoopController;
		float num = 0f;
		float num2 = 10f;
		float num3 = 12f;
		float num4 = num2 / num3;
		float num5 = (num - 0.5f * Physics.gravity.y * num4 * num4) / num4;
		Vector3 vector = Vector3.up * num5 + base.transform.forward * num3;
		GameObject gameObject = Object.Instantiate(grenade_obj, right_hand.position, Quaternion.LookRotation(vector)) as GameObject;
		ProjectileController component = gameObject.GetComponent<ProjectileController>();
		component.launch_dir = vector;
		component.launch_speed = vector;
		component.explode_radius = skillGrenadeCoopController.skill_data.range_val;
		component.damage = 0f;
		component.object_controller = this;
	}
}
