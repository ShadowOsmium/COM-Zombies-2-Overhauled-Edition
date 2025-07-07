using UnityEngine;

namespace CoMZ2
{
	public class EnemyState : ObjectState
	{
		protected EnemyStateType state_type;

		protected EnemyController m_enemy;

		protected bool enable_change_state = true;

		public static EnemyState Create(EnemyStateType type, EnemyController enemy)
		{
			EnemyState enemyState = null;
			switch (type)
			{
			case EnemyStateType.Idle:
				enemyState = new EnemyIdleState();
				enemyState.enable_change_state = true;
				break;
			case EnemyStateType.Patrol:
				enemyState = new EnemyPatrolState();
				enemyState.enable_change_state = true;
				break;
			case EnemyStateType.Catching:
				enemyState = new EnemyCatchingState();
				enemyState.enable_change_state = true;
				break;
			case EnemyStateType.Shoot:
				enemyState = new EnemyAttackState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Injured:
				enemyState = new EnemyInjuredState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Frozen:
				enemyState = new EnemyFrozenState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Dead:
				enemyState = new EnemyDeadState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Show:
				enemyState = new EnemyShowState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.AfterShoot:
				enemyState = new EnemyAfterAttackState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Perceive:
				enemyState = new EnemyPerceiveState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Rail:
				enemyState = new EnemyRailState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.NurseSkill:
				enemyState = new NurseSkillState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Crow_Rush:
				enemyState = new CrowRushState();
				enemyState.enable_change_state = true;
				break;
			case EnemyStateType.FatCook_Combo:
				enemyState = new FatCookComboState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.FatCook_Summon:
				enemyState = new FatCookSummonState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.FatCook_Miantuan:
				enemyState = new FatCookMiantuanState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.FatCook_AfterAttack:
				enemyState = new FatCookAfterAttackState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Haoke_Rush:
				enemyState = new HaokeRushState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Haoke_Summon:
				enemyState = new HaokeSummonState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.Haoke_Tank:
				enemyState = new HaokeTankState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.ChaisawInjured:
				enemyState = new EnemyChaisawInjuredState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.HalfHp:
				enemyState = new BossHalfHpState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.WrestlerWind:
				enemyState = new WrestlerWindState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.WrestlerRush:
				enemyState = new WrestlerRushState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.WrestlerBellow:
				enemyState = new WrestlerBellowState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.HalloweenReplication:
				enemyState = new HalloweenReplicationState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.HalloweenWindSword:
				enemyState = new HalloweenWindSwordState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.HalloweenWindSwordSub:
				enemyState = new HalloweenWindSwordSubState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.SharkRush:
				enemyState = new SharkRushState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.SharkMissile:
				enemyState = new SharkMissileState();
				enemyState.enable_change_state = false;
				break;
			case EnemyStateType.SharkDive:
				enemyState = new SharkDiveState();
				enemyState.enable_change_state = false;
				break;
			default:
				enemyState = new EnemyIdleState();
				enemyState.enable_change_state = true;
				Debug.LogError("EnemyState Create Error! Type:" + type);
				break;
			}
			enemyState.state_type = type;
			enemyState.m_enemy = enemy;
			return enemyState;
		}

		public EnemyStateType GetStateType()
		{
			return state_type;
		}

		public bool IsOpen()
		{
			return enable_change_state;
		}

		public override void DoStateLogic(float deltaTime)
		{
		}
	}
}
