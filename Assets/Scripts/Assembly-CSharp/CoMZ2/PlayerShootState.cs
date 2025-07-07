namespace CoMZ2
{
	public class PlayerShootState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			m_player.ResetSpineRotFire();
			if (GameSceneController.Instance.input_controller.Fire)
			{
				m_player.Fire(state_type, deltaTime);
				if (m_player.FireState.GetStateType() == GetStateType() && m_player.FinishFireAni())
				{
					m_player.CheckFireEndState();
				}
			}
			else if (m_player.CurFireInput != m_player.LastFireInput && m_player.CurWeapon != null && m_player.CurWeapon.weapon_type == WeaponType.PGM)
			{
				m_player.OnFireRelease();
			}
			else if (m_player.FinishFireAni())
			{
				m_player.EnterAfterFire();
			}
		}

		public override void OnEnterState()
		{
			m_player.CheckFireWeapon();
			m_player.GetMoveIdleAnimation(this);
			m_player.SetAfterFireTime();
		}

		public override void OnExitState()
		{
			m_player.StopFire();
			m_player.FireLight(false, 100f);
		}
	}
}
