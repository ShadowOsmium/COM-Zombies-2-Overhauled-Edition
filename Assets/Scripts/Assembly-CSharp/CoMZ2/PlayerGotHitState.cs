namespace CoMZ2
{
	public class PlayerGotHitState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this)))
			{
				m_player.ResetFireIdle(false);
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
