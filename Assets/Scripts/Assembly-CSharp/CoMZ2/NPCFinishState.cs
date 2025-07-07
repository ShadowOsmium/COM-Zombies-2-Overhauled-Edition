using UnityEngine;

namespace CoMZ2
{
	public class NPCFinishState : NPCState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(m_npc.gameObject, m_npc.ANI_IDLE))
			{
				AnimationUtil.CrossAnimate(m_npc.gameObject, m_npc.ANI_IDLE, WrapMode.Loop);
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
