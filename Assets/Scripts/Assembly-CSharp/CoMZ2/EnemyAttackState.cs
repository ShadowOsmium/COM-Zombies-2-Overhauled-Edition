namespace CoMZ2
{
	public class EnemyAttackState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			m_enemy.FireUpdate(deltaTime);
		}

		public override void OnEnterState()
		{
			m_enemy.SetPathCatchState(false);
			m_enemy.EnterAttack();
		}

		public override void OnExitState()
		{
		}
	}
}
