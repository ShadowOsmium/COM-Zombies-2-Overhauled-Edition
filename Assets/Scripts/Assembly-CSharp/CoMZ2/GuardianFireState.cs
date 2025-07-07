namespace CoMZ2
{
	public class GuardianFireState : GuardianState
	{
		public override void DoStateLogic(float deltaTime)
		{
			m_guardian.FireUpdate(deltaTime);
		}

		public override void OnEnterState()
		{
		}

		public override void OnExitState()
		{
		}
	}
}
