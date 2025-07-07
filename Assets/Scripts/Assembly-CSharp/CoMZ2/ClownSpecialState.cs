namespace CoMZ2
{
	public class ClownSpecialState : EnemyState
	{
		protected ClownController clown;

		public override void DoStateLogic(float deltaTime)
		{
		}

		public override void OnEnterState()
		{
			if (clown == null)
			{
				clown = m_enemy as ClownController;
			}
			m_enemy.SetPathCatchState(false);
		}

		public override void OnExitState()
		{
		}
	}
}
