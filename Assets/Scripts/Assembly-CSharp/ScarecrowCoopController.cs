using CoMZ2;
using UnityEngine;

public class ScarecrowCoopController : GuardianForceCoopController
{
	public GameObject broken_eff_ref;

	private float cur_taunt_time;

	private float hp;

	public override void Init()
	{
		ANI_IDLE = "Idle01";
		ANI_INJURED = "Damage01";
		ANI_DEAD = "Attack01";
		ANI_SHOW = "Place01";
		base.Init();
	}

	protected override void Update()
	{
		base.Update();
		if (guardianState.GetStateType() != GuardianStateType.Show && guardianState.GetStateType() != GuardianStateType.Dead)
		{
			cur_taunt_time += Time.deltaTime;
			if (cur_taunt_time >= m_skill.skill_data.frequency_val)
			{
				cur_taunt_time = 0f;
				TauntMonster();
			}
		}
	}

	public override void DetermineState()
	{
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
		if (guardianState.GetStateType() != GuardianStateType.Show && guardianState.GetStateType() != GuardianStateType.Dead)
		{
			if (hp > 0f)
			{
				SetState(INJURED_STATE);
				return;
			}
			OnDead(damage, weapon, controller, hit_point, hit_normal);
			SetState(DEAD_STATE);
			StartCoroutine(RemoveOnTime(3f));
			Invoke("OnScarecrowTriger", 0.03f);
			Object.Instantiate(broken_eff_ref, base.transform.position, base.transform.rotation);
		}
	}

	public override void OnGuardianBirth()
	{
		Debug.Log("ScarecrowController OnGuardianBirth...");
		TauntMonster();
	}

	public void TauntMonster()
	{
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			if ((value.centroid - centroid).sqrMagnitude <= m_skill.skill_data.range_val * m_skill.skill_data.range_val)
			{
				value.AddHateTarget(this, 1000f);
			}
		}
	}

	public void OnScarecrowTriger()
	{
		GameSceneController.Instance.GuardianForce_Set.Remove(guardian_id);
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			value.RemoveTargetFromHateSet(this);
		}
		foreach (EnemyController value2 in GameSceneController.Instance.Enemy_Set.Values)
		{
			if ((value2.centroid - centroid).sqrMagnitude < m_skill.skill_data.range_val * m_skill.skill_data.range_val && !GameSceneController.CheckBlockBetween(centroid, value2.centroid))
			{
				value2.OnHit(0f, null, this, value2.centroid, centroid - value2.centroid);
			}
		}
		foreach (GameObject item in GameSceneController.Instance.wood_box_list)
		{
			WoodBoxController component = item.GetComponent<WoodBoxController>();
			if (component != null && (component.centroid - centroid).sqrMagnitude < m_skill.skill_data.range_val * m_skill.skill_data.range_val && !GameSceneController.CheckBlockBetween(centroid, component.centroid))
			{
				component.OnHit(0f, null, this, component.centroid, centroid - component.centroid);
			}
		}
	}

	public override void SetSkillController(SkillController skill, PlayerController owner)
	{
		base.SetSkillController(skill, owner);
		hp = m_skill.skill_data.hp_capcity;
	}

	public void OnRemoteScarecrowTriger()
	{
		OnDead(0f, null, null, base.transform.position, Vector3.up);
		SetState(DEAD_STATE);
		StartCoroutine(RemoveOnTime(3f));
		Invoke("OnScarecrowTriger", 0.03f);
		Object.Instantiate(broken_eff_ref, base.transform.position, base.transform.rotation);
	}
}
