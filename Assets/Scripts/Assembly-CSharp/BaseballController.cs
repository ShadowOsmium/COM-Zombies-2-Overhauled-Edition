using CoMZ2;
using UnityEngine;

public class BaseballController : WeaponController
{
	private PlayerController m_player;

	private bool fire_consume;

	private float repel_force;

	private void Awake()
	{
		weapon_type = WeaponType.Baseball;
	}

	private void Start()
	{
		repel_force = (float)weapon_data.config.Ex_conf["repelForce"];
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Reset(Transform trans)
	{
		base.transform.position = trans.position + ori_pos;
		base.transform.parent = trans;
		GunOff();
	}

	public override void SetWeaponAni(GameObject player, Transform spine)
	{
		ANI_IDLE_RUN = "Baseball_bat_UpperBody_Idle01";
		ANI_IDLE_SHOOT = "Baseball_bat_UpperBody_Idle01";
		ANI_SHOOT = "Baseball_bat_UpperBody_Attack01";
		ANI_RELOAD = "Baseball_bat_UpperBody_Idle01";
		ANI_SHIFT_WEAPON = "Baseball_bat_UpperBody_ShiftWeapon01";
		ANI_IDLE_DOWN = "Baseball_bat_LowerBody_Idle01";
		ANI_SHOOT_DOWN = "Baseball_bat_LowerBody_Attack01";
		ANI_SWITCH_DOWN = "Baseball_bat_LowerBody_ShiftWeapon01";
		player.GetComponent<Animation>()[ANI_IDLE_RUN].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_IDLE_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHOOT].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_RELOAD].AddMixingTransform(spine);
		player.GetComponent<Animation>()[ANI_SHIFT_WEAPON].AddMixingTransform(spine);
		render_obj = base.gameObject;
		ResetFireAniSpeed(player);
		base.SetWeaponAni(player, spine);
	}

	public override void SetShopWeaponAni(GameObject player, Transform spine)
	{
		render_obj = base.gameObject;
	}

	public override void FireUpdate(PlayerController player, float deltaTime)
	{
		if (GameSceneController.Instance.main_camera != null)
		{
			GameSceneController.Instance.main_camera.ZoomOut(deltaTime);
		}
		if (weapon_data.EnableFire() && (!AnimationUtil.IsPlayingAnimation(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState)) || AnimationUtil.AnimationEnds(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState))))
		{
			m_player = player;
			fire_consume = false;
			player.GetMoveIdleAnimation(player.FireState);
			AnimationUtil.CrossAnimate(player.gameObject, player.GetFireStateAnimation(player.MoveState, player.FireState), WrapMode.Once);
			player.PlayBaseBallEff();
		}
	}

	public override void CheckHit(ObjectController controller)
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_player == null || !AnimationUtil.IsPlayingAnimation(m_player.gameObject, "Baseball_bat_UpperBody_Attack01") || AnimationUtil.IsAnimationPlayedPercentage(m_player.gameObject, "Baseball_bat_UpperBody_Attack01", 0.6f) || !AnimationUtil.IsAnimationPlayedPercentage(m_player.gameObject, "Baseball_bat_UpperBody_Attack01", 0.1f))
		{
			return;
		}
		bool flag = false;
		Vector3 down = Vector3.down;
		if (other.gameObject.layer == PhysicsLayer.ENEMY)
		{
			EnemyController component = other.gameObject.GetComponent<EnemyController>();
			if (component != null && component.Enemy_State.GetStateType() != EnemyStateType.Dead)
			{
				flag = true;
				down = m_player.centroid - component.centroid;
				component.OnHit(GetDamageValWithAvatar(m_player, m_player.avatar_data), this, m_player, component.centroid, down);
				Vector3 vector = component.centroid - m_player.centroid;
				component.OnRepel(new Vector3(vector.x, 0f, vector.z).normalized * repel_force);
			}
		}
		if (other.gameObject.layer == PhysicsLayer.WOOD_BOX)
		{
			WoodBoxController component2 = other.gameObject.GetComponent<WoodBoxController>();
			if (component2 != null)
			{
				flag = true;
				component2.OnHit(GetDamageValWithAvatar(m_player, m_player.avatar_data), this, m_player, component2.centroid, Vector3.up);
			}
		}
		if (flag && !fire_consume)
		{
			fire_consume = true;
			weapon_data.OnFire();
			m_player.AddComboValue(weapon_data.config.combo_base);
		}
	}

	public override void OnEnterAfterFire(PlayerController player)
	{
		player.StopBaseBallEff();
	}
}
