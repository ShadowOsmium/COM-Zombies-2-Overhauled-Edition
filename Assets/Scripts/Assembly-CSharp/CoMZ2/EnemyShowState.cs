using UnityEngine;

namespace CoMZ2
{
	public class EnemyShowState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.AnimationEnds(m_enemy.gameObject, m_enemy.ANI_SHOW))
			{
				AnimationUtil.Stop(m_enemy.gameObject);
				m_enemy.SetState(m_enemy.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			AnimationUtil.Stop(m_enemy.gameObject);
			AnimationUtil.PlayAnimate(m_enemy.gameObject, m_enemy.ANI_SHOW, WrapMode.ClampForever);
			m_enemy.SetPathCatchState(false);
			m_enemy.OnShowUnderGround();
			m_enemy.HideHpbar();
		}

		public override void OnExitState()
		{
			m_enemy.ShowHpbar();
			m_enemy.OnShowOver();
		}
	}
}
