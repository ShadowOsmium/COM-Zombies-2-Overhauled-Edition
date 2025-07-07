using UnityEngine;

namespace CoMZ2
{
	public class PlayerSwitchWeaponState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this), 1f))
			{
				m_player.FinishSwitchWeaponFire();
				m_player.SetFireState(m_player.SHOOT_STATE);
			}
		}

		public override void OnEnterState()
		{
			AnimationUtil.PlayAnimate(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this), WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
		}
	}
}
