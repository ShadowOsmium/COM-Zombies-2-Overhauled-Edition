namespace CoMZ2
{
	public class PlayerAfterShootState : PlayerState
	{
		private float cur_time;

		public float interval_time = 0.8f;

		public override void DoStateLogic(float deltaTime)
		{
			m_player.ResetSpineRotFire();
			cur_time += deltaTime;
			if (cur_time >= interval_time)
			{
				m_player.ResetFireIdle();
			}
		}

		public override void OnEnterState()
		{
			cur_time = 0f;
			m_player.GetMoveIdleAnimation(this);
			m_player.OnEnterAfterFire();
		}

		public override void OnExitState()
		{
		}
	}
}
