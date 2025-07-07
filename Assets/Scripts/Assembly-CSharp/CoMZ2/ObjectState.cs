namespace CoMZ2
{
	public abstract class ObjectState
	{
		public virtual void DoStateLogic(float deltaTime)
		{
		}

		public virtual void OnEnterState()
		{
		}

		public virtual void OnExitState()
		{
		}
	}
}
