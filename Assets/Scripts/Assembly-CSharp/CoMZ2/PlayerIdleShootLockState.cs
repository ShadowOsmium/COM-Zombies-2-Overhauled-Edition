using UnityEngine;

namespace CoMZ2
{
	public class PlayerIdleShootLockState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (GameSceneController.Instance.main_camera != null)
			{
				GameSceneController.Instance.main_camera.ZoomOut(deltaTime);
			}
			if (!AnimationUtil.IsPlayingAnimation(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this)))
			{
				AnimationUtil.CrossAnimate(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this), WrapMode.Loop);
				if (m_player.MoveState.GetStateType() == PlayerStateType.Run && m_player.avatar_data.avatar_type == AvatarType.Cowboy && (m_player.CurWeapon.weapon_type == WeaponType.PGM || m_player.CurWeapon.weapon_type == WeaponType.RocketLauncher))
				{
					AnimationUtil.CrossAnimate(m_player.CowboyCap, "Run01", WrapMode.Loop);
				}
				else if (m_player.avatar_data.avatar_type == AvatarType.Cowboy && AnimationUtil.IsPlayingAnimation(m_player.CowboyCap, "Run01"))
				{
					AnimationUtil.Stop(m_player.CowboyCap, "Run01");
				}
				else if (m_player.avatar_data.avatar_type == AvatarType.Swat)
				{
					m_player.OnSwatIdleShoot();
				}
			}
		}

		public override void OnEnterState()
		{
		}

		public override void OnExitState()
		{
			if (m_player.avatar_data.avatar_type == AvatarType.Cowboy)
			{
				AnimationUtil.Stop(m_player.CowboyCap);
			}
		}
	}
}
