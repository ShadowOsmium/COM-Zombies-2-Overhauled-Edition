using UnityEngine;

namespace CoMZ2
{
	public class EnemyDeadState : EnemyState
	{
		private int dead_step;

		public override void DoStateLogic(float deltaTime)
		{
			m_enemy.SetPathCatchState(false);
			if (!m_enemy.IsBoss)
			{
				return;
			}
			if (dead_step == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(m_enemy.gameObject, m_enemy.ANI_DEAD, 0.3f))
				{
					dead_step++;
					if (m_enemy.IsBoss)
					{
						m_enemy.OnBossDeadHalf();
					}
				}
			}
			else if (dead_step == 1 && AnimationUtil.IsAnimationPlayedPercentage(m_enemy.gameObject, m_enemy.ANI_DEAD, 1f))
			{
				dead_step++;
				if (m_enemy.IsBoss)
				{
					m_enemy.OnBossDeadEnd();
				}
			}
		}

		public override void OnEnterState()
		{
			dead_step = 0;
			AnimationUtil.Stop(m_enemy.gameObject);
			AnimationUtil.PlayAnimate(m_enemy.gameObject, m_enemy.ANI_DEAD, WrapMode.ClampForever);
			if (m_enemy.IsBoss)
			{
				m_enemy.OnBossDead();
			}
			m_enemy.RemovePather();
		}

		public override void OnExitState()
		{
		}
	}
}
