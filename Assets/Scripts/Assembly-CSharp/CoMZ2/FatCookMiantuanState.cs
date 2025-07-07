using UnityEngine;

namespace CoMZ2
{
	public class FatCookMiantuanState : EnemyState
	{
		protected FatCookController fatCook;

		public override void DoStateLogic(float deltaTime)
		{
			fatCook.AlwaysLookAtTarget();
			if (AnimationUtil.IsAnimationPlayedPercentage(fatCook.gameObject, fatCook.ANI_SKILL_MIANTUAN, 1f))
			{
				((FatCookAfterAttackState)fatCook.AFTER_ATTACK_STATE).IsAfterMiantuan = true;
				fatCook.SetState(fatCook.AFTER_ATTACK_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (fatCook == null)
			{
				fatCook = m_enemy as FatCookController;
			}
			m_enemy.SetPathCatchState(false);
			AnimationUtil.CrossAnimate(fatCook.gameObject, fatCook.ANI_SKILL_MIANTUAN, WrapMode.ClampForever);
			fatCook.PlayEffMiantuan();
		}

		public override void OnExitState()
		{
		}
	}
}
