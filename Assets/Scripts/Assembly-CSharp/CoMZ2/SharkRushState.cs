using UnityEngine;

namespace CoMZ2
{
	public class SharkRushState : EnemyState
	{
		public int skill_stpe;

		protected SharkController shark;

		private float cur_rush_time;

		private float max_rush_time = 2f;

		private float cur_rest_time;

		private float max_rest_time = 2f;

		public override void DoStateLogic(float deltaTime)
		{
			if (!shark.nav_pather.GetCatchingState())
			{
				shark.AlwaysLookAtTarget();
			}
			if (skill_stpe == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(shark.gameObject, shark.ANI_RUSH_01, 0.95f))
				{
					skill_stpe++;
					m_enemy.nav_pather.PlayNavMeshAgent();
					shark.ANI_CUR_ATTACK = shark.ANI_RUSH_02;
					AnimationUtil.CrossAnimate(shark.gameObject, shark.ANI_RUSH_02, WrapMode.Loop);
					shark.OnSharkRushReady();
					shark.ShowSharkRushEff(true);
					shark.ShowSharkDivingEff(true);
				}
			}
			else if (skill_stpe == 1)
			{
				if (shark.OnRushUpdate(deltaTime))
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(shark.gameObject, shark.ANI_RUSH_03, WrapMode.ClampForever);
					shark.SetPathCatchState(false);
					shark.CheckHit();
					shark.ShowSharkRushEff(false);
					shark.ShowSharkDivingEff(false);
					return;
				}
				cur_rush_time += deltaTime;
				if (cur_rush_time >= max_rush_time)
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(shark.gameObject, shark.ANI_RUSH_03, WrapMode.ClampForever);
					shark.SetPathCatchState(false);
					shark.CheckHit();
					shark.ShowSharkRushEff(false);
					shark.ShowSharkDivingEff(false);
				}
			}
			else if (skill_stpe == 2)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(shark.gameObject, shark.ANI_RUSH_03, 0.95f))
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(shark.gameObject, shark.ANI_RUSH_04, WrapMode.Loop);
				}
			}
			else if (skill_stpe == 3)
			{
				cur_rest_time += deltaTime;
				if (cur_rest_time >= max_rest_time)
				{
					skill_stpe++;
					shark.SetState(shark.IDLE_STATE);
				}
			}
		}

		public override void OnEnterState()
		{
			if (shark == null)
			{
				shark = m_enemy as SharkController;
				max_rest_time = shark.rush_rest_time;
			}
			skill_stpe = 0;
			shark.SetPathCatchState(false);
			shark.rush_enable = false;
			cur_rush_time = 0f;
			cur_rest_time = 0f;
			AnimationUtil.CrossAnimate(shark.gameObject, shark.ANI_RUSH_01, WrapMode.ClampForever);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			shark.OnSharkRushEnd();
		}
	}
}
