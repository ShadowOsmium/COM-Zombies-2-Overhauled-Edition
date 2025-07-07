using UnityEngine;

namespace CoMZ2
{
	public class FatCookComboState : EnemyState
	{
		public int skill_stpe;

		protected FatCookController fat_cook;

		public float combo_time = 5f;

		public float combo_rest_time = 3f;

		private float cur_combo_time;

		private float cur_combo_rest_time;

		public override void DoStateLogic(float deltaTime)
		{
			fat_cook.AlwaysLookAtTarget();
			if (skill_stpe == 0)
			{
				m_enemy.SetPathCatchState(false);
				if (AnimationUtil.IsAnimationPlayedPercentage(fat_cook.gameObject, fat_cook.ANI_SKILL_FOCUS, 0.9f))
				{
					skill_stpe++;
					m_enemy.nav_pather.PlayNavMeshAgent();
					AnimationUtil.CrossAnimate(fat_cook.gameObject, fat_cook.ANI_SKILL_COMBO, WrapMode.Loop);
				}
			}
			else if (skill_stpe == 1)
			{
				cur_combo_time += deltaTime;
				m_enemy.SetPathCatchState(true);
				if (cur_combo_time >= combo_time)
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(fat_cook.gameObject, fat_cook.ANI_SKILL_COMBO_END, WrapMode.ClampForever);
				}
			}
			else if (skill_stpe == 2)
			{
				m_enemy.SetPathCatchState(false);
				if (AnimationUtil.IsAnimationPlayedPercentage(fat_cook.gameObject, fat_cook.ANI_SKILL_COMBO_END, 0.9f))
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(fat_cook.gameObject, fat_cook.ANI_SKILL_REST, WrapMode.Loop);
				}
			}
			else if (skill_stpe == 3)
			{
				cur_combo_rest_time += deltaTime;
				m_enemy.SetPathCatchState(false);
				if (cur_combo_rest_time >= combo_rest_time)
				{
					skill_stpe++;
					fat_cook.SetState(fat_cook.IDLE_STATE);
				}
			}
		}

		public override void OnEnterState()
		{
			if (fat_cook == null)
			{
				fat_cook = m_enemy as FatCookController;
			}
			skill_stpe = 0;
			cur_combo_time = 0f;
			cur_combo_rest_time = 0f;
			m_enemy.SetPathCatchState(false);
			AnimationUtil.CrossAnimate(fat_cook.gameObject, fat_cook.ANI_SKILL_FOCUS, WrapMode.ClampForever);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
		}
	}
}
