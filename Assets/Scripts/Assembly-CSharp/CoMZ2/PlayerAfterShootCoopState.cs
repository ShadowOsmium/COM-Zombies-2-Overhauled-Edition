namespace CoMZ2
{
	public class PlayerAfterShootCoopState : PlayerState
	{
		private float cur_time;

		public float interval_time = 0.8f;

		public override void DoStateLogic(float deltaTime)
		{
			m_player.ResetSpineRotFire();
		}

		public override void OnEnterState()
		{
			m_player.GetMoveIdleAnimation(this);
			m_player.OnEnterAfterFire();
		}

		public override void OnExitState()
		{
		}
	}
}
