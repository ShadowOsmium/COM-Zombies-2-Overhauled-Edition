using UnityEngine;

namespace CoMZ2
{
	public class PlayerGrenadeState : PlayerState
	{
		protected SwatController swat;

		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(swat.gameObject, "UpperBody_Avatar_Swat_Skill01", 0.85f))
			{
				swat.CalculateSetFireState();
			}
		}

		public override void OnEnterState()
		{
			if (swat == null)
			{
				swat = m_player as SwatController;
			}
			swat.ShowCurWeapon(false);
			AnimationUtil.Stop(swat.gameObject);
			AnimationUtil.PlayAnimate(swat.gameObject, "UpperBody_Avatar_Swat_Skill01", WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
			swat.ShowCurWeapon(true);
		}
	}
}
