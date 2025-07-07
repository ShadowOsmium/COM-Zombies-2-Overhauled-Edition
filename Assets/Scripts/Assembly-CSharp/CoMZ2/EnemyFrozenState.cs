namespace CoMZ2
{
	public class EnemyFrozenState : EnemyState
	{
		protected float cur_frozen_time;

		protected float max_frozen_time;

		public override void DoStateLogic(float deltaTime)
		{
			cur_frozen_time += deltaTime;
			if (cur_frozen_time >= max_frozen_time)
			{
				m_enemy.SetState(m_enemy.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			m_enemy.SetPathCatchState(false);
			cur_frozen_time = 0f;
			max_frozen_time = m_enemy.frozenTime;
			m_enemy.OnEnterFrozenState();
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			m_enemy.OnExitFrozenState();
			m_enemy.nav_pather.PlayNavMeshAgent();
		}
	}
}
