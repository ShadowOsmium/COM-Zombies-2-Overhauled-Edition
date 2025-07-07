using UnityEngine;

namespace CoMZ2
{
	public class NPCState : ObjectState
	{
		protected NPCStateType state_type;

		protected NPCController m_npc;

		public NPCStateType GetStateType()
		{
			return state_type;
		}

		public static NPCState Create(NPCStateType type, NPCController npc)
		{
			NPCState nPCState = null;
			switch (type)
			{
			case NPCStateType.Idle:
				nPCState = new NPCIdleState();
				break;
			case NPCStateType.Run:
				nPCState = new NPCRunState();
				break;
			case NPCStateType.Injured:
				nPCState = new NPCInjuredState();
				break;
			case NPCStateType.TakeUp:
				nPCState = new NPCTakeResState();
				break;
			case NPCStateType.PutDown:
				nPCState = new NPCPutDownResState();
				break;
			case NPCStateType.Fear:
				nPCState = new NPCFearState();
				break;
			case NPCStateType.Dead:
				nPCState = new NPCDeadState();
				break;
			case NPCStateType.Finish:
				nPCState = new NPCFinishState();
				break;
			case NPCStateType.Rescue:
				nPCState = new NPCRescueState();
				break;
			default:
				nPCState = new NPCIdleState();
				Debug.LogError("NPCState Create Error! Type:" + type);
				break;
			}
			nPCState.state_type = type;
			nPCState.m_npc = npc;
			return nPCState;
		}

		public override void DoStateLogic(float deltaTime)
		{
		}
	}
}
