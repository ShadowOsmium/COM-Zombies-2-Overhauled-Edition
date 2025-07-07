using UnityEngine;

namespace CoMZ2
{
	public class GuardianDeadState : GuardianState
	{
		public override void DoStateLogic(float deltaTime)
		{
		}

		public override void OnEnterState()
		{
			AnimationUtil.Stop(m_guardian.gameObject);
			AnimationUtil.PlayAnimate(m_guardian.gameObject, m_guardian.ANI_DEAD, WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
		}
	}
}
