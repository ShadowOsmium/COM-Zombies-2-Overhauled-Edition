using CoMZ2;
using UnityEngine;

public class FollowerController : NPCController
{
	public Transform target_trans;

	public bool arrived_home;

	public NPCState RESCUE_STATE;

	public float SqrDistanceFromPlayer
	{
		get
		{
			if (target_trans != null)
			{
				return (target_trans.transform.position - base.transform.position).sqrMagnitude;
			}
			return 9999f;
		}
	}

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public override void Init()
	{
		base.Init();
		RESCUE_STATE = NPCState.Create(NPCStateType.Rescue, this);
		npcState = RESCUE_STATE;
	}

	public override void DetermineState()
	{
		if (npcState.GetStateType() == NPCStateType.Dead || npcState.GetStateType() == NPCStateType.Injured || npcState.GetStateType() == NPCStateType.Fear || npcState.GetStateType() == NPCStateType.Finish || npcState.GetStateType() == NPCStateType.Rescue)
		{
			return;
		}
		if (target_trans == null)
		{
			SetState(IDLE_STATE);
		}
		else if (ArrivedTarget())
		{
			SetState(FINISH_STATE);
			if (!arrived_home)
			{
				arrived_home = true;
				GameSceneController.Instance.game_main_panel.npc_convoy_panel.SetContent(1 + " / " + 1);
			}
		}
		else
		{
			SetState(RUN_STATE);
		}
	}

	public override void OnHit(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
		if (GameSceneController.Instance.GamePlayingState != PlayingState.Gaming || GameSceneController.Instance.is_logic_paused || npcState == DEAD_STATE)
		{
			return;
		}
		injured_time = Time.time;
		if (npc_data.OnInjured(damage))
		{
			OnDead(damage, weapon, controller, hit_point, hit_normal);
			SetState(DEAD_STATE);
		}
		else if (npcState.GetStateType() != NPCStateType.Injured && npcState.GetStateType() != NPCStateType.Fear)
		{
			if (Random.Range(0, 100) % 2 == 0)
			{
				AnimationUtil.CrossAnimate(base.gameObject, ANI_INJURED, WrapMode.ClampForever);
				SetState(INJURED_STATE);
			}
			else
			{
				SetState(FEAR_STATE);
			}
		}
		SetHpBar(npc_data.cur_hp / npc_data.hp_capacity);
		GameSceneController.Instance.game_main_panel.ShowHelpLabelEff("HELP_eff1");
	}

	public override void OnDead(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
		base.OnDead(damage, weapon, controller, hit_point, hit_normal);
	}

	public virtual bool ArrivedTarget()
	{
		if (SqrDistanceFromPlayer < 2f)
		{
			return true;
		}
		return false;
	}

	public void StartConvoyed(Transform trans)
	{
		SetState(IDLE_STATE);
		target_trans = trans;
		nav_pather.SetTarget(target_trans);
		GameSceneController.Instance.OnStartConvoyed();
	}
}
