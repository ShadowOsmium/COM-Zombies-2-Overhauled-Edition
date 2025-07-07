using UnityEngine;

namespace CoMZ2
{
	public class PlayerReloadState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this), 0.85f))
			{
				m_player.CalculateSetFireState();
			}
		}

		public override void OnEnterState()
		{
			AnimationUtil.CrossAnimate(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this), WrapMode.ClampForever);
			m_player.OnWeaponReload();
			m_player.GetMoveIdleAnimation(this);
		}

		public override void OnExitState()
		{
			m_player.UpdateWeaponUIShow();
			m_player.ResetFireInterval();
		}
	}
}
