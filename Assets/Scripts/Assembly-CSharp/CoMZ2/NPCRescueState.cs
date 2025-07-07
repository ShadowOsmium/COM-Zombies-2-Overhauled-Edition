using UnityEngine;

namespace CoMZ2
{
	public class NPCRescueState : NPCState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(m_npc.gameObject, m_npc.ANI_RESCUE))
			{
				AnimationUtil.CrossAnimate(m_npc.gameObject, m_npc.ANI_RESCUE, WrapMode.Loop);
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
