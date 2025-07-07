using UnityEngine;

namespace CoMZ2
{
	public class EnemyIdleState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(m_enemy.gameObject, m_enemy.ANI_IDLE))
			{
				AnimationUtil.CrossAnimate(m_enemy.gameObject, m_enemy.ANI_IDLE, WrapMode.Loop);
			}
		}

		public override void OnEnterState()
		{
			AnimationUtil.CrossAnimate(m_enemy.gameObject, m_enemy.ANI_IDLE, WrapMode.Loop);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			m_enemy.nav_pather.PlayNavMeshAgent();
		}
	}
}
