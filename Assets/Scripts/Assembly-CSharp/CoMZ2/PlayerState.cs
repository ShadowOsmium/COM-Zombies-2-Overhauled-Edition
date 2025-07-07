using UnityEngine;

namespace CoMZ2
{
	public class PlayerState : ObjectState
	{
		protected PlayerStateType state_type;

		protected PlayerController m_player;

		protected bool enable_interrupt = true;

		public bool EnableInterrupt
		{
			get
			{
				return enable_interrupt;
			}
		}

		public PlayerStateType GetStateType()
		{
			return state_type;
		}

		public static PlayerState Create(PlayerStateType type, PlayerController player, bool is_coop = false)
		{
			PlayerState playerState = null;
			switch (type)
			{
			case PlayerStateType.Idle:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerIdleState()) : ((PlayerState)new PlayerIdleCoopState()));
				playerState.enable_interrupt = true;
				break;
			case PlayerStateType.Run:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerRunState()) : ((PlayerState)new PlayerRunCoopState()));
				playerState.enable_interrupt = true;
				break;
			case PlayerStateType.IdleShoot:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerIdleShootState()) : ((PlayerState)new PlayerIdleShootCoopState()));
				playerState.enable_interrupt = true;
				break;
			case PlayerStateType.Shoot:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerShootState()) : ((PlayerState)new PlayerShootCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.AfterShoot:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerAfterShootState()) : ((PlayerState)new PlayerAfterShootCoopState()));
				playerState.enable_interrupt = true;
				break;
			case PlayerStateType.GotHit:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerGotHitState()) : ((PlayerState)new PlayerGotHitCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.Dead:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerDeadState()) : ((PlayerState)new PlayerDeadCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.Reload:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerReloadState()) : ((PlayerState)new PlayerReloadCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.SwitchWeapon:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerSwitchWeaponState()) : ((PlayerState)new PlayerSwitchWeaponCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.ShieldBreak:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerShieldBreakState()) : ((PlayerState)new PlayerShieldBreakCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.ChaisawSkill:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerChaisawSkillState()) : ((PlayerState)new PlayerChaisawSkillCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.Whirlwind:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerWhirlwindState()) : ((PlayerState)new PlayerWhirlwindCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.Scarecrow:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerScarecrowState()) : ((PlayerState)new PlayerScarecrowCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.Grenade:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerGrenadeState()) : ((PlayerState)new PlayerGrenadeCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.BaseballRobot:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerBaseballRobotState()) : ((PlayerState)new PlayerBaseballRobotCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.Enchant:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerEnchantState()) : ((PlayerState)new PlayerEnchantCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.IdleLock:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerIdleLockState()) : ((PlayerState)new PlayerIdleLockCoopState()));
				playerState.enable_interrupt = false;
				break;
			case PlayerStateType.IdleShootLock:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerIdleShootLockState()) : ((PlayerState)new PlayerIdleShootLockCoopState()));
				playerState.enable_interrupt = false;
				break;
			default:
				playerState = ((!is_coop) ? ((PlayerState)new PlayerIdleState()) : ((PlayerState)new PlayerIdleCoopState()));
				Debug.LogError("PlayerState Create Error! Type:" + type);
				break;
			}
			playerState.state_type = type;
			playerState.m_player = player;
			return playerState;
		}

		public override void DoStateLogic(float deltaTime)
		{
		}
	}
}
