using UnityEngine;

namespace CoMZ2
{
	public class WrestlerBellowState : EnemyState
	{
		public int skill_stpe;

		protected WrestlerController wrestler;

		private float cur_bellow_time;

		private float max_bellow_time = 2f;

		public override void DoStateLogic(float deltaTime)
		{
			if (!wrestler.nav_pather.GetCatchingState())
			{
				wrestler.AlwaysLookAtTarget();
			}
			if (skill_stpe == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(wrestler.gameObject, wrestler.ANI_BELLOW_01, 0.9f))
				{
					skill_stpe++;
					wrestler.ANI_CUR_ATTACK = wrestler.ANI_BELLOW_02;
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_BELLOW_02, WrapMode.Loop);
					cur_bellow_time = 0f;
				}
			}
			else if (skill_stpe == 1)
			{
				wrestler.OnBellowUpdate(deltaTime);
				cur_bellow_time += deltaTime;
				if (cur_bellow_time >= max_bellow_time)
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_BELLOW_03, WrapMode.ClampForever);
					wrestler.OnWrestlerBellowOver();
				}
			}
			else if (skill_stpe == 2 && AnimationUtil.IsAnimationPlayedPercentage(wrestler.gameObject, wrestler.ANI_BELLOW_03, 1f))
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
			max_bellow_time = wrestler.bellow_time;
			skill_stpe = 0;
			wrestler.SetPathCatchState(false);
			wrestler.bellow_enable = false;
			wrestler.OnWrestlerBellowReady();
			AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_BELLOW_01, WrapMode.ClampForever);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			wrestler.OnWrestlerBellowOver();
			m_enemy.nav_pather.PlayNavMeshAgent();
		}
	}
}
