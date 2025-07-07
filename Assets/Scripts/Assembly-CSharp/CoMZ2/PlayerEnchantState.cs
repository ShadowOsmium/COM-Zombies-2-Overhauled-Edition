using UnityEngine;

namespace CoMZ2
{
	public class PlayerEnchantState : PlayerState
	{
		protected DoctorController doctor;

		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(doctor.gameObject, "UpperBody_Avatar_Doctor_Skill01", 0.95f))
			{
				doctor.CalculateSetFireState();
			}
		}

		public override void OnEnterState()
		{
			if (doctor == null)
			{
				doctor = m_player as DoctorController;
			}
			doctor.ShowCurWeapon(false);
			doctor.ShowEnchantGun();
			AnimationUtil.Stop(doctor.gameObject);
			AnimationUtil.PlayAnimate(doctor.gameObject, "UpperBody_Avatar_Doctor_Skill01", WrapMode.ClampForever);
		}

		public override void OnExitState()
		{
			doctor.ShowCurWeapon(true);
			doctor.HideEnchantGun();
		}
	}
}
