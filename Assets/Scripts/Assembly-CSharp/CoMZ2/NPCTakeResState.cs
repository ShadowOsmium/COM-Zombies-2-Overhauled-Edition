using UnityEngine;

namespace CoMZ2
{
	public class NPCTakeResState : NPCState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(m_npc.gameObject, m_npc.GetAnimationWithState(this), 0.95f))
			{
				WorkerController workerController = m_npc as WorkerController;
				workerController.FinishTakeUp();
			}
		}

		public override void OnEnterState()
		{
			m_npc.SetPathCatchState(false);
			WorkerController workerController = m_npc as WorkerController;
			workerController.TakeUpRes();
			AnimationUtil.CrossAnimate(m_npc.gameObject, m_npc.GetAnimationWithState(this), WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
		}
	}
}
