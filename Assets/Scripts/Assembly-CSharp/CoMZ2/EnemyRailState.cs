using UnityEngine;

namespace CoMZ2
{
	public class EnemyRailState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(m_enemy.gameObject, m_enemy.ANI_RAIL))
			{
				AnimationUtil.CrossAnimate(m_enemy.gameObject, m_enemy.ANI_RAIL, WrapMode.Loop);
			}
		}

		public override void OnEnterState()
		{
			Debug.Log("enemy enter rail state.enemy:" + m_enemy.name + " ani:" + m_enemy.ANI_RAIL);
			m_enemy.SetPathCatchState(false);
		}

		public override void OnExitState()
		{
		}
	}
}
