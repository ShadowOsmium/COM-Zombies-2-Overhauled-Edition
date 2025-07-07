using UnityEngine;

namespace CoMZ2
{
	public class EnemyPerceiveState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(m_enemy.gameObject, m_enemy.ANI_PERCEIVE, 0.6f))
			{
				m_enemy.SetState(m_enemy.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			m_enemy.SetPathCatchState(false);
			if (!AnimationUtil.IsPlayingAnimation(m_enemy.gameObject, m_enemy.ANI_PERCEIVE))
			{
				AnimationUtil.CrossAnimate(m_enemy.gameObject, m_enemy.ANI_PERCEIVE, WrapMode.Once);
			}
		}

		public override void OnExitState()
		{
		}
	}
}
