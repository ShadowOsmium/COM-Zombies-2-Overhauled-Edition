using UnityEngine;

namespace CoMZ2
{
	public class GuardianIdleCoopState : GuardianState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!(m_guardian.GetComponent<Animation>() == null) && !AnimationUtil.IsPlayingAnimation(m_guardian.gameObject, m_guardian.ANI_IDLE))
			{
				AnimationUtil.CrossAnimate(m_guardian.gameObject, m_guardian.ANI_IDLE, WrapMode.Loop);
			}
		}

		public override void OnEnterState()
		{
		}

		public override void OnExitState()
		{
		}
	}
}
