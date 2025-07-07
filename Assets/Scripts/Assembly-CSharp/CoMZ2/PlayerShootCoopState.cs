namespace CoMZ2
{
	public class PlayerShootCoopState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			m_player.Fire(state_type, deltaTime);
		}

		public override void OnEnterState()
		{
			m_player.GetMoveIdleAnimation(this);
			m_player.SetAfterFireTime();
		}

		public override void OnExitState()
		{
			m_player.StopFire();
			m_player.FireLight(false, 100f);
		}
	}
}
