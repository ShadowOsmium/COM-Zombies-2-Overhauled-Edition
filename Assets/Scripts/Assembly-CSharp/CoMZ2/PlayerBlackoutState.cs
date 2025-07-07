namespace CoMZ2
{
	public class PlayerBlackoutState : PlayerState
	{
		private float cur_blackout_time;

		private float max_blackout_time = 3f;

		public override void DoStateLogic(float deltaTime)
		{
			cur_blackout_time += deltaTime;
			if (cur_blackout_time >= max_blackout_time)
			{
				m_player.ResetFireIdle(false);
			}
		}

		public override void OnEnterState()
		{
			cur_blackout_time = 0f;
		}

		public override void OnExitState()
		{
		}
	}
}
