using UnityEngine;

namespace CoMZ2
{
	public class NPCFearState : NPCState
	{
		private float cur_time;

		public float fear_time = 5f;

		public override void DoStateLogic(float deltaTime)
		{
			cur_time += deltaTime;
			if (cur_time >= fear_time)
			{
				m_npc.SetState(m_npc.IDLE_STATE);
			}
			if (!AnimationUtil.IsPlayingAnimation(m_npc.gameObject, m_npc.ANI_FEAR))
			{
				AnimationUtil.CrossAnimate(m_npc.gameObject, m_npc.ANI_FEAR, WrapMode.Loop);
			}
		}

		public override void OnEnterState()
		{
			m_npc.SetPathCatchState(false);
			cur_time = 0f;
			m_npc.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			m_npc.nav_pather.PlayNavMeshAgent();
		}
	}
}
