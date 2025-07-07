using UnityEngine;

namespace CoMZ2
{
	public class FatCookSummonState : EnemyState
	{
		protected FatCookController fatCook;

		public override void DoStateLogic(float deltaTime)
		{
			fatCook.AlwaysLookAtTarget();
			if (AnimationUtil.IsAnimationPlayedPercentage(fatCook.gameObject, fatCook.ANI_SKILL_SUMMON, 1f))
			{
				fatCook.SetState(fatCook.SKILL_MIANTUAN_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (fatCook == null)
			{
				fatCook = m_enemy as FatCookController;
			}
			m_enemy.SetPathCatchState(false);
			AnimationUtil.CrossAnimate(fatCook.gameObject, fatCook.ANI_SKILL_SUMMON, WrapMode.ClampForever);
			fatCook.PlayEffSummon();
		}

		public override void OnExitState()
		{
		}
	}
}
