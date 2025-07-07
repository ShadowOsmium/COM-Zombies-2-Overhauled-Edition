using UnityEngine;

namespace CoMZ2
{
	public class EnemyPatrolState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(m_enemy.gameObject, m_enemy.ANI_RUN))
			{
				AnimationUtil.CrossAnimate(m_enemy.gameObject, m_enemy.ANI_RUN, WrapMode.Loop);
			}
			m_enemy.DoPatrol(deltaTime);
		}

		public override void OnEnterState()
		{
			m_enemy.EnterPatrol();
			m_enemy.SetPathCatchState(true);
		}

		public override void OnExitState()
		{
			m_enemy.IsPartol = false;
			m_enemy.SetTargetPlayer();
		}
	}
}
