using UnityEngine;

namespace CoMZ2
{
	public class FatCookSkillAttackState : EnemyState
	{
		public int skill_stpe;

		protected FatCookController fat_cook;

		public override void DoStateLogic(float deltaTime)
		{
			fat_cook.AlwaysLookAtTarget();
			if (skill_stpe == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(fat_cook.gameObject, fat_cook.ANI_SKILL_FOCUS, 0.9f))
				{
					skill_stpe++;
					m_enemy.nav_pather.PlayNavMeshAgent();
					AnimationUtil.CrossAnimate(fat_cook.gameObject, fat_cook.ANI_SKILL_ATTACK, WrapMode.ClampForever);
				}
			}
			else if (skill_stpe == 1 && AnimationUtil.IsAnimationPlayedPercentage(fat_cook.gameObject, fat_cook.ANI_SKILL_ATTACK, 0.9f))
			{
				skill_stpe++;
				fat_cook.SetState(fat_cook.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (fat_cook == null)
			{
				fat_cook = m_enemy as FatCookController;
			}
			skill_stpe = 0;
			m_enemy.SetPathCatchState(false);
			AnimationUtil.CrossAnimate(fat_cook.gameObject, fat_cook.ANI_SKILL_FOCUS, WrapMode.ClampForever);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
		}
	}
}
