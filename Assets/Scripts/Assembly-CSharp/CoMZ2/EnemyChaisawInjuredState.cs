using UnityEngine;

namespace CoMZ2
{
	public class EnemyChaisawInjuredState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
		}

		public override void OnEnterState()
		{
			m_enemy.SetPathCatchState(false);
			AnimationUtil.PlayAnimate(m_enemy.gameObject, m_enemy.ANI_CHAISAW_INJURED, WrapMode.Loop);
		}

		public override void OnExitState()
		{
		}
	}
}
