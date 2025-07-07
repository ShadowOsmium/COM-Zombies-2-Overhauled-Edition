using UnityEngine;

namespace CoMZ2
{
	public class NPCRunState : NPCState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(m_npc.gameObject, m_npc.ANI_RUN))
			{
				AnimationUtil.CrossAnimate(m_npc.gameObject, m_npc.ANI_RUN, WrapMode.Loop);
			}
		}

		public override void OnEnterState()
		{
			m_npc.SetPathCatchState(true);
		}

		public override void OnExitState()
		{
		}
	}
}
