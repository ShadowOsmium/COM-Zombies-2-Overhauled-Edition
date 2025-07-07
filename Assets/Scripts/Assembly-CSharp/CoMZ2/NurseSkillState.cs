using UnityEngine;

namespace CoMZ2
{
	public class NurseSkillState : EnemyState
	{
		public int skill_stpe;

		protected NurseController nurse;

		public float skill_time = 1.5f;

		private float cur_skill_time;

		public override void DoStateLogic(float deltaTime)
		{
			nurse.DoSkillAttack(deltaTime);
			if (skill_stpe == 0)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(nurse.gameObject, nurse.ani_skill01, 0.95f))
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(nurse.gameObject, nurse.ani_skill02, WrapMode.Loop);
					nurse.PlayVomitEff();
				}
			}
			else if (skill_stpe == 1)
			{
				cur_skill_time += deltaTime;
				if (cur_skill_time >= skill_time)
				{
					skill_stpe++;
					AnimationUtil.CrossAnimate(nurse.gameObject, nurse.ani_skill03, WrapMode.ClampForever);
				}
			}
			else if (skill_stpe == 2 && AnimationUtil.IsAnimationPlayedPercentage(nurse.gameObject, nurse.ani_skill03, 0.95f))
			{
				skill_stpe++;
				nurse.SetState(nurse.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (nurse == null)
			{
				nurse = m_enemy as NurseController;
			}
			skill_stpe = 0;
			cur_skill_time = 0f;
			m_enemy.SetPathCatchState(false);
			AnimationUtil.CrossAnimate(nurse.gameObject, nurse.ani_skill01, WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
			nurse.StopVomitEff();
		}
	}
}
