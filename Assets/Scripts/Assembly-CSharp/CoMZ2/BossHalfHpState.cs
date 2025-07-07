using UnityEngine;

namespace CoMZ2
{
	public class BossHalfHpState : EnemyState
	{
		public bool IsPlayed;

		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(m_enemy.gameObject, m_enemy.ANI_HALF_HP, 1f))
			{
				m_enemy.SetState(m_enemy.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			m_enemy.SetPathCatchState(false);
			AnimationUtil.CrossAnimate(m_enemy.gameObject, m_enemy.ANI_HALF_HP, WrapMode.ClampForever);
			m_enemy.PlayHalfHpEffect();
		}

		public override void OnExitState()
		{
			if (!IsPlayed)
			{
				IsPlayed = true;
			}
			m_enemy.OnHalfHpEffOver();
		}
	}
}
