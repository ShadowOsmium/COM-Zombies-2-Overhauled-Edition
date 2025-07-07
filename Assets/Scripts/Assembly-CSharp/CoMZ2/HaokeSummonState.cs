namespace CoMZ2
{
	public class HaokeSummonState : EnemyState
	{
		protected HaokeController haoke;

		public override void DoStateLogic(float deltaTime)
		{
			haoke.FireUpdate(deltaTime);
		}

		public override void OnEnterState()
		{
			if (haoke == null)
			{
				haoke = m_enemy as HaokeController;
			}
			haoke.SetPathCatchState(false);
			haoke.EnterSummonAttack();
		}

		public override void OnExitState()
		{
			haoke.StopEffSummon();
		}
	}
}
