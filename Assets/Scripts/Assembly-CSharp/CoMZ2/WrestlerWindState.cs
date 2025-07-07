using UnityEngine;

namespace CoMZ2
{
	public class WrestlerWindState : EnemyState
	{
		public int skill_stpe;

		protected WrestlerController wrestler;

		private float cur_wind_time;

		private float max_wind_time = 2f;

		public override void DoStateLogic(float deltaTime)
		{
			if (!wrestler.nav_pather.GetCatchingState())
			{
				wrestler.AlwaysLookAtTarget();
			}
			if (skill_stpe == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(wrestler.gameObject, wrestler.ANI_WIND_01, 0.9f))
				{
					skill_stpe++;
					wrestler.ANI_CUR_ATTACK = wrestler.ANI_WIND_02;
					m_enemy.nav_pather.PlayNavMeshAgent();
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_WIND_02, WrapMode.Loop);
					wrestler.OnWrestlerWindReady();
					cur_wind_time = 0f;
				}
			}
			else if (skill_stpe == 1)
			{
				wrestler.OnWindUpdate(deltaTime);
				cur_wind_time += deltaTime;
				if (cur_wind_time >= max_wind_time)
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_WIND_03, WrapMode.ClampForever);
					wrestler.OnWrestlerWindOver();
				}
			}
			else if (skill_stpe == 2 && AnimationUtil.IsAnimationPlayedPercentage(wrestler.gameObject, wrestler.ANI_WIND_03, 1f))
			{
				skill_stpe++;
				wrestler.SetState(wrestler.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (wrestler == null)
			{
				wrestler = m_enemy as WrestlerController;
			}
			max_wind_time = wrestler.wind_time;
			skill_stpe = 0;
			wrestler.SetPathCatchState(false);
			wrestler.wind_enable = false;
			AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_WIND_01, WrapMode.ClampForever);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			wrestler.OnWrestlerWindOver();
		}
	}
}
