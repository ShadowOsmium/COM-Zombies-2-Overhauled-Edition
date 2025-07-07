using CoMZ2;
using UnityEngine;

public class BaseballRobotController : GuardianForceController
{
	protected float check_target_rate = 0.1f;

	protected float check_target_time;

	public SkillUseState skill_use_state;

	public float cur_cd_time;

	private float hp;

	private float damage_val;

	protected ObjectController target_player;

	public GuardianState SHOOT_STATE;

	protected Transform fire_ori;

	public GameObject baseball_ref;

	protected bool is_active;

	public float SqrDistanceFromPlayer
	{
		get
		{
			if (target_player != null)
			{
				return (target_player.transform.position - base.transform.position).sqrMagnitude;
			}
			return 9999f;
		}
	}

	public override void Init()
	{
		ANI_IDLE = "Idle01";
		ANI_FIRE = "Skill_Mike_01_01_Attack01";
		ANI_SHOW = "Skill_Mike_01_01_Place01";
		IDLE_STATE = GuardianState.Create(GuardianStateType.Idle, this);
		SHOOT_STATE = GuardianState.Create(GuardianStateType.Fire, this);
		SHOW_STATE = GuardianState.Create(GuardianStateType.Show, this);
		controller_type = ControllerType.GuardianForce;
		SetState(SHOW_STATE);
		guardian_id = GameSceneController.Instance.GuardianIndex;
		GameSceneController.Instance.GuardianForce_Set.Add(guardian_id, this);
		fire_ori = base.transform.Find("fire_ori");
	}

	protected override void Update()
	{
		if (is_life_over || m_skill == null || !is_active)
		{
			return;
		}
		base.Update();
		if (skill_use_state == SkillUseState.Saving)
		{
			cur_cd_time += Time.deltaTime;
			if (cur_cd_time >= m_skill.skill_data.frequency_val)
			{
				cur_cd_time = 0f;
				skill_use_state = SkillUseState.Ready;
			}
		}
		cur_life_time += Time.deltaTime;
		if (cur_life_time >= m_skill.skill_data.life_time)
		{
			is_life_over = true;
			Debug.LogWarning("life over.");
			StartCoroutine(RemoveOnTime(0.1f));
		}
	}

	public override void DetermineState()
	{
		if (is_active)
		{
			CheckTargetPlayer();
			if (target_player == null)
			{
				SetState(IDLE_STATE);
			}
			else if (CouldEnterAttackState())
			{
				SetState(SHOOT_STATE);
			}
			else
			{
				SetState(IDLE_STATE);
			}
		}
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
	}

	public override void OnGuardianBirth()
	{
		Debug.Log("BaseballRobotController OnGuardianBirth...");
	}

	public void CheckTargetPlayer()
	{
		if (!(Time.time - check_target_time >= check_target_rate))
		{
			return;
		}
		check_target_time = Time.time;
		if (target_player == null)
		{
			FindTargetPlayerWithoutBlock();
			return;
		}
		EnemyController enemyController = target_player as EnemyController;
		if (enemyController != null && (enemyController.IsEnchant || enemyController.Enemy_State.GetStateType() == EnemyStateType.Dead))
		{
			target_player = null;
			FindTargetPlayerWithoutBlock();
		}
	}

	public bool IsBlocked(Transform target)
	{
		return Physics.Raycast(base.transform.position, (target.position - base.transform.position).normalized, (target.position - base.transform.position).magnitude, (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD));
	}

	public virtual void FindTargetPlayerWithoutBlock()
	{
		float num = 9999f;
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			if (Vector3.Distance(value.transform.position, base.transform.position) < num && !IsBlocked(value.transform))
			{
				target_player = value;
				num = Vector3.Distance(value.transform.position, base.transform.position);
			}
		}
	}

	public bool CouldEnterAttackState()
	{
		if (SqrDistanceFromPlayer <= m_skill.skill_data.range_val * m_skill.skill_data.range_val)
		{
			if (GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public override void FireUpdate(float deltaTime)
	{
		if (target_player != null)
		{
			base.transform.LookAt(target_player.transform);
			base.transform.Rotate(Vector3.up, 90f);
			if (skill_use_state == SkillUseState.Ready)
			{
				skill_use_state = SkillUseState.Saving;
				Fire();
			}
		}
	}

	public void Fire()
	{
		AnimationUtil.Stop(base.gameObject);
		AnimationUtil.PlayAnimate(base.gameObject, ANI_FIRE, WrapMode.Once);
		GameObject gameObject = Object.Instantiate(baseball_ref, fire_ori.position, Quaternion.identity) as GameObject;
		BaseballProjectile component = gameObject.GetComponent<BaseballProjectile>();
		component.launch_dir = (target_player.centroid - gameObject.transform.position).normalized;
		component.fly_speed = 35;
		component.life = 5f;
		component.damage = damage_val;
		component.object_controller = this;
		component.weapon_controller = null;
	}

	public void ActiveRobot()
	{
		is_active = true;
	}

	public override void SetSkillController(SkillController skill, PlayerController owner)
	{
		base.SetSkillController(skill, owner);
		hp = m_skill.skill_data.hp_capcity;
		damage_val = m_skill.skill_data.damage_val / m_skill.skill_data.life_time * m_skill.skill_data.frequency_val;
		Debug.Log("BaseballRobotController damage_val:" + damage_val);
	}
}
