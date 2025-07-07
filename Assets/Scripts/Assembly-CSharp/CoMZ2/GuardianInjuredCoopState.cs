using UnityEngine;

namespace CoMZ2
{
	public class GuardianInjuredCoopState : GuardianState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.AnimationEnds(m_guardian.gameObject, m_guardian.ANI_INJURED))
			{
				AnimationUtil.Stop(m_guardian.gameObject);
				m_guardian.SetState(m_guardian.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			AnimationUtil.Stop(m_guardian.gameObject);
			AnimationUtil.PlayAnimate(m_guardian.gameObject, m_guardian.ANI_INJURED, WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
		}
	}
}
