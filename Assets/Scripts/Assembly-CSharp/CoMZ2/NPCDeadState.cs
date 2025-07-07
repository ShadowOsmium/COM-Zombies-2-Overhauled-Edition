using UnityEngine;

namespace CoMZ2
{
	public class NPCDeadState : NPCState
	{
		public override void DoStateLogic(float deltaTime)
		{
		}

		public override void OnEnterState()
		{
			if (!AnimationUtil.IsPlayingAnimation(m_npc.gameObject, m_npc.ANI_DEAD))
			{
				AnimationUtil.Stop(m_npc.gameObject);
				AnimationUtil.PlayAnimate(m_npc.gameObject, m_npc.ANI_DEAD, WrapMode.ClampForever);
			}
			m_npc.SetPathCatchState(false);
		}

		public override void OnExitState()
		{
		}
	}
}
