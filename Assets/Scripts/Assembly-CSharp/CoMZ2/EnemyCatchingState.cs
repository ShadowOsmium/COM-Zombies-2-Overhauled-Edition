using UnityEngine;

namespace CoMZ2
{
	public class EnemyCatchingState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(m_enemy.gameObject, m_enemy.ANI_RUN))
			{
				AnimationUtil.CrossAnimate(m_enemy.gameObject, m_enemy.ANI_RUN, WrapMode.Loop);
			}
			m_enemy.DoSpecialCatching(deltaTime);
		}

		public override void OnEnterState()
		{
			m_enemy.SetPathCatchState(true);
		}

		public override void OnExitState()
		{
		}
	}
}
