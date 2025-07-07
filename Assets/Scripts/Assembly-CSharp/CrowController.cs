using CoMZ2;
using UnityEngine;

public class CrowController : EnemyController
{
	protected const float ReCheckFindPath_time = 5f;

	protected float rush_cd;

	protected float lastPathFindingTime;

	protected float lastRushPathTime;

	protected SimplePathFinding path_find;

	protected Vector3 move_dir = Vector3.zero;

	protected Transform move_target;

	protected Collider attackCollider;

	protected Vector3 lastTarget = Vector3.zero;

	protected Ray ray;

	protected RaycastHit rayhit;

	protected Ray ray1;

	protected RaycastHit rayhit1;

	protected float lastReCheckPathTime;

	public EnemyState RUSH_STATE;

	protected bool could_rush = true;

	public string ANI_RUSH = "Attack01_01";

	protected float catching_height;

	protected float rush_speed;

	protected float rush_time;

	protected float rush_cur_time;

	public override void Init()
	{
		ANI_IDLE = "Fly01";
		ANI_ATTACK = "Attack02";
		ANI_INJURED = "Damage01";
		ANI_DEAD = "Death01";
		ANI_RUN = "Fly01";
		base.Init();
		RUSH_STATE = EnemyState.Create(EnemyStateType.Crow_Rush, this);
		path_find = new SimplePathFinding();
		path_find.InitPath(GameSceneController.Instance.way_points);
		catching_height = GameSceneController.Instance.way_points[0].transform.position.y;
		attackCollider = base.transform.Find("Root/Dummy_Head/Bone_Mouth01").gameObject.GetComponent<Collider>();
		if (attackCollider == null)
		{
			Debug.LogError("attack collider not founded!");
		}
	}

	public override void SetEnemyData(EnemyData data)
	{
		enemy_data = data;
		rush_speed = (float)enemy_data.config.Ex_conf["rushSpeed"];
	}

	public override void CheckHit()
	{
		foreach (PlayerController value in GameSceneController.Instance.Player_Set.Values)
		{
			Collider collider = value.gameObject.GetComponent<Collider>();
			if (collider != null && attackCollider.bounds.Intersects(collider.bounds))
			{
				value.OnHit(enemy_data.damage_val, null, this, value.centroid, Vector3.zero);
			}
		}
		foreach (NPCController value2 in GameSceneController.Instance.NPC_Set.Values)
		{
			Collider collider2 = value2.gameObject.GetComponent<Collider>();
			if (collider2 != null && attackCollider.bounds.Intersects(collider2.bounds))
			{
				value2.OnHit(enemy_data.damage_val, null, this, value2.centroid, Vector3.zero);
			}
		}
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		base.OnHit(damage, weapon, player, hit_point, hit_normal);
		CreateInjuredBloodEff(hit_point, hit_normal);
	}

	public override void FireUpdate(float deltaTime)
	{
		if (target_player != null)
		{
			base.transform.LookAt(target_player.transform.position + Vector3.up);
		}
		fire_interval += deltaTime;
		if (fire_interval < enemy_data.frequency_val)
		{
			OnAttackInterval();
			return;
		}
		fire_interval = 0f;
		Fire();
	}

	public override void Fire()
	{
		AnimationUtil.Stop(base.gameObject);
		AnimationUtil.PlayAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
		CheckHit();
	}

	public override void OnDead(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		base.OnDead(damage, weapon, player, hit_point, hit_normal);
		base.GetComponent<Rigidbody>().useGravity = true;
		base.GetComponent<Collider>().isTrigger = false;
		StartCoroutine(RemoveOnTime(2f));
	}

	public void FindPath()
	{
		if (move_target == null)
		{
			Debug.Log("find path warning.");
		}
		else
		{
			if (!(Time.time - lastPathFindingTime > 0.25f))
			{
				return;
			}
			lastPathFindingTime = Time.time;
			if (lastTarget == Vector3.zero)
			{
				lastTarget = target_player.transform.position + Vector3.up;
			}
			ray = new Ray(base.transform.position, target_player.transform.position + Vector3.up - base.transform.position);
			float magnitude = (target_player.transform.position + Vector3.up - base.transform.position).magnitude;
			LayerMask layerMask = (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD);
			if (Physics.Raycast(ray, out rayhit, magnitude, layerMask))
			{
				ray1 = new Ray(base.transform.position, lastTarget - base.transform.position);
				float magnitude2 = (lastTarget - base.transform.position).magnitude;
				if (Physics.Raycast(ray1, out rayhit1, magnitude2, layerMask))
				{
					Transform nextWayPoint = path_find.GetNextWayPoint(base.transform.position, target_player.transform.position);
					if (nextWayPoint != null)
					{
						lastTarget = nextWayPoint.position;
					}
				}
				if ((base.transform.position - lastTarget).sqrMagnitude < 1f)
				{
					path_find.PopNode();
					Transform nextWayPoint2 = path_find.GetNextWayPoint(base.transform.position, target_player.transform.position);
					if (nextWayPoint2 != null)
					{
						lastTarget = nextWayPoint2.position;
					}
				}
			}
			else
			{
				lastTarget = target_player.transform.position + Vector3.up;
				if (could_rush)
				{
					SetState(RUSH_STATE);
					rush_time = (base.transform.position - target_player.transform.position).magnitude / rush_speed;
					rush_cur_time = 0f;
					could_rush = false;
				}
			}
			base.transform.LookAt(lastTarget);
			move_dir = (lastTarget - base.transform.position).normalized;
		}
	}

	public void FindRushPath()
	{
		if (move_target == null)
		{
			Debug.Log("find path warning.");
		}
		else if (Time.time - lastRushPathTime > 0.25f)
		{
			lastRushPathTime = Time.time;
			ray = new Ray(base.transform.position, target_player.transform.position + Vector3.up - base.transform.position);
			float magnitude = (target_player.transform.position + Vector3.up - base.transform.position).magnitude;
			LayerMask layerMask = (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD);
			if (Physics.Raycast(ray, out rayhit, magnitude, layerMask))
			{
				SetState(CATCHING_STATE);
			}
			else if (CouldEnterAttackState())
			{
				SetState(SHOOT_STATE);
			}
			lastTarget = target_player.transform.position + Vector3.up;
			base.transform.LookAt(lastTarget);
			move_dir = (lastTarget - base.transform.position).normalized;
		}
	}

	public override void DoSpecialCatching(float deltaTime)
	{
		if (!could_rush && Mathf.Abs(catching_height - base.transform.position.y) < 3f)
		{
			rush_cd += deltaTime;
			if (rush_cd >= 2f)
			{
				could_rush = true;
				rush_cd = 0f;
			}
		}
		FindPath();
		base.transform.Translate(move_dir * enemy_data.move_speed * deltaTime, Space.World);
	}

	public void DoRush(float deltaTime)
	{
		rush_cur_time += deltaTime;
		if (rush_cur_time >= rush_time)
		{
			if (CouldEnterAttackState())
			{
				SetState(SHOOT_STATE);
			}
			else
			{
				SetState(CATCHING_STATE);
			}
		}
		FindRushPath();
		base.transform.Translate(move_dir * rush_speed * deltaTime, Space.World);
	}

	public override void CheckTargetPlayer()
	{
		if (target_player == null && Time.time - check_target_time >= check_target_rate)
		{
			if (base.WithoutBlockCheck)
			{
				FindTargetPlayerWithoutBlock();
			}
			else
			{
				FindTargetPlayer();
			}
			check_target_time = Time.time;
			move_target = target_player.transform;
		}
	}

	public override void OnAttackInterval()
	{
		if (AnimationUtil.AnimationEnds(base.gameObject, ANI_ATTACK))
		{
			SetState(IDLE_STATE);
		}
	}
}
