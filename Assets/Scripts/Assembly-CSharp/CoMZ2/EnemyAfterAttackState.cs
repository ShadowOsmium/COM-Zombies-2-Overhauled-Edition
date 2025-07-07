using UnityEngine;

namespace CoMZ2
{
	public class EnemyAfterAttackState : EnemyState
	{
		public override void DoStateLogic(float deltaTime)
		{
			m_enemy.AfterFireUpdate(deltaTime);
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
