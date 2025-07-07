using UnityEngine;

namespace CoMZ2
{
	public class GuardianState : ObjectState
	{
		protected GuardianStateType state_type;

		protected GuardianForceController m_guardian;

		public GuardianStateType GetStateType()
		{
			return state_type;
		}

		public static GuardianState Create(GuardianStateType type, GuardianForceController guardian, bool is_coop = false)
		{
			GuardianState guardianState = null;
			switch (type)
			{
			case GuardianStateType.Idle:
				guardianState = ((!is_coop) ? ((GuardianState)new GuardianIdleState()) : ((GuardianState)new GuardianIdleCoopState()));
				break;
			case GuardianStateType.Show:
				guardianState = ((!is_coop) ? ((GuardianState)new GuardianShowState()) : ((GuardianState)new GuardianShowCoopState()));
				break;
			case GuardianStateType.Fire:
				guardianState = ((!is_coop) ? ((GuardianState)new GuardianFireState()) : ((GuardianState)new GuardianFireCoopState()));
				break;
			case GuardianStateType.Injured:
				guardianState = ((!is_coop) ? ((GuardianState)new GuardianInjuredState()) : ((GuardianState)new GuardianInjuredCoopState()));
				break;
			case GuardianStateType.Dead:
				guardianState = ((!is_coop) ? ((GuardianState)new GuardianDeadState()) : ((GuardianState)new GuardianDeadCoopState()));
				break;
			default:
				guardianState = ((!is_coop) ? ((GuardianState)new GuardianIdleState()) : ((GuardianState)new GuardianIdleCoopState()));
				Debug.LogError("Guardian state Create Error! Type:" + type);
				break;
			}
			guardianState.state_type = type;
			guardianState.m_guardian = guardian;
			return guardianState;
		}

		public override void DoStateLogic(float deltaTime)
		{
		}
	}
}
