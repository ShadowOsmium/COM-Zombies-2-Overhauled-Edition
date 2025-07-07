using UnityEngine;

namespace CoMZ2
{
	public class PlayerScarecrowState : PlayerState
	{
		protected CowboyController cowboy;

		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(cowboy.gameObject, "Avatar_Cowboy_Skill01", 0.85f))
			{
				cowboy.CalculateSetFireState();
			}
		}

		public override void OnEnterState()
		{
			if (cowboy == null)
			{
				cowboy = m_player as CowboyController;
			}
			cowboy.ShowCurWeapon(false);
			AnimationUtil.Stop(cowboy.gameObject);
			AnimationUtil.PlayAnimate(cowboy.gameObject, "Avatar_Cowboy_Skill01", WrapMode.ClampForever);
			cowboy.OnScarecrowSkillStart();
		}

		public override void OnExitState()
		{
			cowboy.OnScarecrowSkillEnd();
			cowboy.ShowCurWeapon(true);
		}
	}
}
