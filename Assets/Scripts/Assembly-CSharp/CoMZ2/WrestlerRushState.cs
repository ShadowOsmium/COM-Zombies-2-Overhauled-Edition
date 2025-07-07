using UnityEngine;

namespace CoMZ2
{
	public class WrestlerRushState : EnemyState
	{
		public int skill_stpe;

		protected WrestlerController wrestler;

		private float cur_rush_time;

		private float max_rush_time = 2f;

		private float cur_jump_time;

		private float max_jump_time = 0.5f;

		public override void DoStateLogic(float deltaTime)
		{
			if (!wrestler.nav_pather.GetCatchingState())
			{
				wrestler.AlwaysLookAtTarget();
			}
			if (skill_stpe == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(wrestler.gameObject, wrestler.ANI_RUSH_01, 0.9f))
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_RUSH_02, WrapMode.Loop);
					wrestler.OnWrestlerRushReady();
				}
			}
			else if (skill_stpe == 1)
			{
				m_enemy.nav_pather.PlayNavMeshAgent();
				cur_rush_time += deltaTime;
				if (wrestler.CheckEnableRushJump())
				{
					skill_stpe++;
					m_enemy.nav_pather.StopNavMeshAgent();
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_RUSH_03, WrapMode.ClampForever);
					wrestler.ResetRushJumpTarget();
					wrestler.SetPathCatchState(false);
				}
				else if (wrestler.CheckRushJumpTooClose())
				{
					wrestler.SetState(wrestler.IDLE_STATE);
				}
				else if (cur_rush_time >= max_rush_time)
				{
					wrestler.SetState(wrestler.IDLE_STATE);
				}
			}
			else if (skill_stpe == 2)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(wrestler.gameObject, wrestler.ANI_RUSH_03, 1f))
				{
					skill_stpe++;
					max_jump_time = wrestler.OnStartJump();
					m_enemy.nav_pather.PlayNavMeshAgent();
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_RUSH_04, WrapMode.Loop);
				}
			}
			else if (skill_stpe == 3)
			{
				cur_jump_time += deltaTime;
				wrestler.OnRushJumpUpdate(deltaTime);
				if (cur_jump_time >= max_jump_time)
				{
					skill_stpe = 5;
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_RUSH_05, WrapMode.ClampForever);
					wrestler.OnStopJump();
				}
				else if (wrestler.CheckJumpAlmostFinish())
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_RUSH_05, WrapMode.ClampForever);
				}
			}
			else if (skill_stpe == 4)
			{
				cur_jump_time += deltaTime;
				if (cur_jump_time >= max_jump_time)
				{
					wrestler.OnStopJump();
				}
				if (AnimationUtil.IsAnimationPlayedPercentage(wrestler.gameObject, wrestler.ANI_RUSH_05, 1f))
				{
					skill_stpe = 6;
					wrestler.SetState(wrestler.IDLE_STATE);
					wrestler.OnStopJump();
				}
			}
			else if (skill_stpe == 5 && AnimationUtil.IsAnimationPlayedPercentage(wrestler.gameObject, wrestler.ANI_RUSH_05, 1f))
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
			max_rush_time = wrestler.rush_time;
			cur_rush_time = 0f;
			cur_jump_time = 0f;
			skill_stpe = 0;
			wrestler.SetPathCatchState(false);
			wrestler.rush_enable = false;
			AnimationUtil.CrossAnimate(wrestler.gameObject, wrestler.ANI_RUSH_01, WrapMode.ClampForever);
			m_enemy.nav_pather.StopNavMeshAgent();
		}

		public override void OnExitState()
		{
			wrestler.OnWrestlerRushOver();
		}
	}
}
