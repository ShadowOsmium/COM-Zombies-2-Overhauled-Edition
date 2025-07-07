using UnityEngine;

namespace CoMZ2
{
	public class SharkDiveState : EnemyState
	{
		public int skill_stpe;

		protected SharkController shark;

		private float cur_dive_time;

		private float max_dive_time = 2f;

		public override void DoStateLogic(float deltaTime)
		{
			if (!shark.nav_pather.GetCatchingState())
			{
				shark.AlwaysLookAtTarget();
			}
			if (skill_stpe == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(shark.gameObject, shark.ANI_DIVE_01, 1f))
				{
					skill_stpe++;
					m_enemy.nav_pather.PlayNavMeshAgent();
					shark.SetPathCatchState(true);
					shark.OnSharkDiveReady();
				}
			}
			else if (skill_stpe == 1)
			{
				cur_dive_time += deltaTime;
				if (cur_dive_time >= max_dive_time)
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(shark.gameObject, shark.ANI_DIVE_02, WrapMode.ClampForever);
					shark.SetPathCatchState(false);
					shark.CheckHit();
					shark.ShowSharkFin(false);
					shark.ShowSharkDivingEff(false);
					shark.ShowSharkBody(true);
					shark.ShowSharkDiveOutEff();
				}
			}
			else if (skill_stpe == 2 && AnimationUtil.IsAnimationPlayedPercentage(shark.gameObject, shark.ANI_DIVE_02, 1f))
			{
				skill_stpe++;
				shark.SetState(shark.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (shark == null)
			{
				shark = m_enemy as SharkController;
				max_dive_time = shark.dive_time;
			}
			skill_stpe = 0;
			shark.SetPathCatchState(false);
			shark.dive_enable = false;
			cur_dive_time = 0f;
			AnimationUtil.CrossAnimate(shark.gameObject, shark.ANI_DIVE_01, WrapMode.ClampForever);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			shark.OnSharkDiveEnd();
		}
	}
}
