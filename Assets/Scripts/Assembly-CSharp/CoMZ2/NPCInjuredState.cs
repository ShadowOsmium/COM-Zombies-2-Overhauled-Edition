namespace CoMZ2
{
	public class NPCInjuredState : NPCState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(m_npc.gameObject, m_npc.ANI_INJURED, 0.95f))
			{
				m_npc.SetState(m_npc.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			m_npc.SetPathCatchState(false);
		}

		public override void OnExitState()
		{
		}
	}
}
