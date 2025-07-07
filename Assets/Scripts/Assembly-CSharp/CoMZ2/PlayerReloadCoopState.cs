using UnityEngine;

namespace CoMZ2
{
	public class PlayerReloadCoopState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
		}

		public override void OnEnterState()
		{
			AnimationUtil.CrossAnimate(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this), WrapMode.ClampForever);
			m_player.OnWeaponReload();
			m_player.GetMoveIdleAnimation(this);
		}

		public override void OnExitState()
		{
			m_player.ResetFireInterval();
		}
	}
}
