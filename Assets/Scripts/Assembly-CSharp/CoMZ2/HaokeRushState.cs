using UnityEngine;

namespace CoMZ2
{
	public class HaokeRushState : EnemyState
	{
		public int skill_stpe;

		protected HaokeController haoke;

		private float cur_rush_time;

		private float max_rush_time = 2f;

		public override void DoStateLogic(float deltaTime)
		{
			if (!haoke.nav_pather.GetCatchingState())
			{
				haoke.AlwaysLookAtTarget();
			}
			if (skill_stpe == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(haoke.gameObject, haoke.ANI_RUSH_01, 0.9f))
				{
					skill_stpe++;
					haoke.ANI_CUR_ATTACK = haoke.ANI_RUSH_02;
					AnimationUtil.CrossAnimate(haoke.gameObject, haoke.ANI_RUSH_02, WrapMode.Loop);
					cur_rush_time = 0f;
					haoke.PlayEffRush();
				}
			}
			else if (skill_stpe == 1)
			{
				if (haoke.OnRushUpdate(deltaTime))
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(haoke.gameObject, haoke.ANI_RUSH_03, WrapMode.ClampForever);
					haoke.StopEffRush();
					return;
				}
				cur_rush_time += deltaTime;
				if (cur_rush_time >= max_rush_time)
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(haoke.gameObject, haoke.ANI_RUSH_03, WrapMode.ClampForever);
					haoke.StopEffRush();
				}
			}
			else if (skill_stpe == 2 && AnimationUtil.IsAnimationPlayedPercentage(haoke.gameObject, haoke.ANI_RUSH_03, 1f))
			{
				skill_stpe++;
				haoke.SetState(haoke.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (haoke == null)
			{
				haoke = m_enemy as HaokeController;
			}
			skill_stpe = 0;
			haoke.SetPathCatchState(false);
			haoke.rush_enable = false;
			AnimationUtil.CrossAnimate(haoke.gameObject, haoke.ANI_RUSH_01, WrapMode.ClampForever);
			haoke.PlayEffRushReady();
		}

		public override void OnExitState()
		{
			haoke.OnHaokeRushEnd();
		}
	}
}
