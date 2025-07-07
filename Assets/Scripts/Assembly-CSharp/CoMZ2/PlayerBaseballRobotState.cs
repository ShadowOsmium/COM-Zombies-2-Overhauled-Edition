using UnityEngine;

namespace CoMZ2
{
	public class PlayerBaseballRobotState : PlayerState
	{
		protected MikeController mike;

		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(mike.gameObject, "Avatar_Mike_Skill01", 0.85f))
			{
				mike.CalculateSetFireState();
			}
		}

		public override void OnEnterState()
		{
			if (mike == null)
			{
				mike = m_player as MikeController;
			}
			mike.ShowCurWeapon(false);
			AnimationUtil.Stop(mike.gameObject);
			AnimationUtil.PlayAnimate(mike.gameObject, "Avatar_Mike_Skill01", WrapMode.ClampForever);
			mike.OnBaseballRobotSkill();
		}

		public override void OnExitState()
		{
			mike.ShowCurWeapon(true);
			mike.OnBaseballRobotSkillEnd();
		}
	}
}
