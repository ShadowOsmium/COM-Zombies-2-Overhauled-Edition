using CoMZ2;
using UnityEngine;

public class WorkerController : NPCController
{
	public NPCState TAKEUP_STATE;

	public NPCState PUTDOWN_STATE;

	public Transform home_trans;

	public Transform res_trans;

	public string ANI_TAKEUP = "TakeUp01";

	public string ANI_PUTDOWM = "PutDown01";

	public string ani_run = "Forward01";

	public string ani_carry = "Carry01";

	protected GameObject res_box;

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

    public NPCResourcesMissionController mission_controller;

	public bool is_Res_taked;

	public float SqrDistanceToHome
	{
		get
		{
			if (home_trans != null)
			{
				return (home_trans.transform.position - base.transform.position).sqrMagnitude;
			}
			return 9999f;
		}
	}

	public float SqrDistanceToRes
	{
		get
		{
			if (res_trans != null)
			{
				return (res_trans.transform.position - base.transform.position).sqrMagnitude;
			}
			return 9999f;
		}
	}

	public override void Init()
	{
		base.Init();
		TAKEUP_STATE = NPCState.Create(NPCStateType.TakeUp, this);
		PUTDOWN_STATE = NPCState.Create(NPCStateType.PutDown, this);
	}

	public override void DetermineState()
	{
		if (GameSceneController.Instance.GamePlayingState == PlayingState.CG || npcState.GetStateType() == NPCStateType.Dead || npcState.GetStateType() == NPCStateType.Injured || npcState.GetStateType() == NPCStateType.TakeUp || npcState.GetStateType() == NPCStateType.PutDown || npcState.GetStateType() == NPCStateType.Fear || npcState.GetStateType() == NPCStateType.Finish || home_trans == null || res_trans == null)
		{
			return;
		}
		if (is_Res_taked)
		{
			if (ArrivedHome())
			{
				SetState(PUTDOWN_STATE);
				return;
			}
			if (nav_pather.target != home_trans)
			{
				nav_pather.SetTarget(home_trans);
			}
			SetState(RUN_STATE);
		}
		else if (ArrivedRes())
		{
			SetState(TAKEUP_STATE);
		}
		else
		{
			if (nav_pather.target != res_trans)
			{
				nav_pather.SetTarget(res_trans);
			}
			SetState(RUN_STATE);
		}
	}

	public void FinishTakeUp()
	{
		is_Res_taked = true;
		nav_pather.SetTarget(home_trans);
		nav_pather.SetSpeed(npc_data.move_speed / 2f);
		ANI_RUN = ani_carry;
		SetState(RUN_STATE);
		GameSceneController.Instance.OnStartTakeRes();
	}

	public void TakeUpRes()
	{
		if (res_box == null)
		{
			res_box = Object.Instantiate(Resources.Load("Prefabs/Cube"), base.transform.position, Quaternion.identity) as GameObject;
			res_box.transform.parent = base.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 Prop1");
			res_box.transform.localPosition = Vector3.zero;
			res_box.transform.localRotation = Quaternion.identity;
		}
	}

	public void FinishPutDown()
	{
		is_Res_taked = false;
		nav_pather.SetTarget(res_trans);
		nav_pather.SetSpeed(npc_data.move_speed);
		ANI_RUN = ani_run;
		if (res_box != null)
		{
			Object.Destroy(res_box);
			res_box = null;
		}
		if (mission_controller.AddResources(1))
		{
			SetState(FINISH_STATE);
		}
		else
		{
			SetState(RUN_STATE);
		}
	}

	public override void OnDead(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
		base.OnDead(damage, weapon, controller, hit_point, hit_normal);
	}

	public bool ArrivedHome()
	{
		if (SqrDistanceToHome < 2f)
		{
			return true;
		}
		return false;
	}

	public bool ArrivedRes()
	{
		if (SqrDistanceToRes < 2f)
		{
			return true;
		}
		return false;
	}

	public override string GetAnimationWithState(NPCState state)
	{
		if (state == TAKEUP_STATE)
		{
			return ANI_TAKEUP;
		}
		if (state == PUTDOWN_STATE)
		{
			return ANI_PUTDOWM;
		}
		return ANI_IDLE;
	}

	public void InitRescourceTrans(Transform res, Transform home)
	{
		home_trans = home;
		res_trans = res;
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
		else if (npcState.GetStateType() != NPCStateType.Injured && npcState.GetStateType() != NPCStateType.TakeUp && npcState.GetStateType() != NPCStateType.PutDown)
		{
			if (is_Res_taked)
			{
				ANI_INJURED = ani_injured_res;
			}
			else
			{
				ANI_INJURED = ani_injured;
			}
			AnimationUtil.CrossAnimate(base.gameObject, ANI_INJURED, WrapMode.ClampForever);
			SetState(INJURED_STATE);
		}
		SetHpBar(npc_data.cur_hp / npc_data.hp_capacity);
		GameSceneController.Instance.game_main_panel.ShowHelpLabelEff("HELP_eff1");
	}
}
