using UnityEngine;

namespace CoMZ2
{
	public class HaokeTankState : EnemyState
	{
		public int skill_stpe;

		protected HaokeController haoke;

		private float cur_ready_time;

		private float max_ready_time = 0.2f;

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
				if (AnimationUtil.IsAnimationPlayedPercentage(haoke.gameObject, haoke.ANI_TANK_01, 0.9f))
				{
					skill_stpe++;
					haoke.ANI_CUR_ATTACK = haoke.ANI_TANK_02;
					AnimationUtil.CrossAnimate(haoke.gameObject, haoke.ANI_TANK_02, WrapMode.Loop);
					cur_rush_time = 0f;
					cur_ready_time = 0f;
				}
			}
			else if (skill_stpe == 1)
			{
				cur_ready_time += deltaTime;
				if (cur_ready_time >= max_ready_time)
				{
					haoke.OnHaokeTankReady();
					skill_stpe++;
				}
			}
			else if (skill_stpe == 2)
			{
				if (haoke.OnTankUpdate(deltaTime))
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(haoke.gameObject, haoke.ANI_TANK_04, WrapMode.ClampForever);
					haoke.OnTankSkillOver();
					return;
				}
				cur_rush_time += deltaTime;
				if (cur_rush_time >= max_rush_time)
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(haoke.gameObject, haoke.ANI_TANK_04, WrapMode.ClampForever);
					haoke.OnTankSkillOver();
				}
			}
			else if (skill_stpe == 3 && AnimationUtil.IsAnimationPlayedPercentage(haoke.gameObject, haoke.ANI_TANK_04, 1f))
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
			haoke.tank_enable = false;
			AnimationUtil.CrossAnimate(haoke.gameObject, haoke.ANI_TANK_01, WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
			haoke.OnHaokeTankEnd();
		}
	}
}
