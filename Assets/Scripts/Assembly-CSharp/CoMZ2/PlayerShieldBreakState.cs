using UnityEngine;

namespace CoMZ2
{
	public class PlayerShieldBreakState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(m_player.gameObject, m_player.ANI_SHIELD_BREAK, 1f))
			{
				m_player.ResetFireIdle();
			}
		}

		public override void OnEnterState()
		{
			m_player.ShowSwatBreakShieldEff();
			AnimationUtil.PlayAnimate(m_player.gameObject, m_player.ANI_SHIELD_BREAK, WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
		}
	}
}
