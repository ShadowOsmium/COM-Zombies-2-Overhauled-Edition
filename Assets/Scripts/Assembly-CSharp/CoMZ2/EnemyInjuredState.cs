namespace CoMZ2
{
	public class EnemyInjuredState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(m_enemy.gameObject, m_enemy.ANI_INJURED, 0.9f))
			{
				m_enemy.SetState(m_enemy.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			m_enemy.SetPathCatchState(false);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			m_enemy.nav_pather.PlayNavMeshAgent();
		}
	}
}
