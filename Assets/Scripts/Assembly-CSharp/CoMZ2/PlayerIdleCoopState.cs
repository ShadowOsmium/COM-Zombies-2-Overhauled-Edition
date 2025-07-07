using UnityEngine;

namespace CoMZ2
{
	public class PlayerIdleCoopState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			m_player.ResetSpineRot(deltaTime);
			if (!AnimationUtil.IsPlayingAnimation(m_player.gameObject, m_player.ANI_MOVE_IDLE))
			{
				AnimationUtil.CrossAnimate(m_player.gameObject, m_player.ANI_MOVE_IDLE, WrapMode.Loop);
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
