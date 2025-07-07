using UnityEngine;

namespace CoMZ2
{
	public class HalloweenWindSwordState : EnemyState
	{
		protected HalloweenController halloween;

		public override void DoStateLogic(float deltaTime)
		{
			if (!halloween.nav_pather.GetCatchingState())
			{
				halloween.AlwaysLookAtTarget();
			}
			if (AnimationUtil.IsAnimationPlayedPercentage(halloween.gameObject, halloween.ANI_WINDSWORD, 1f))
			{
				halloween.SetState(halloween.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (halloween == null)
			{
				halloween = m_enemy as HalloweenController;
			}
			halloween.SetPathCatchState(false);
			AnimationUtil.CrossAnimate(halloween.gameObject, halloween.ANI_WINDSWORD, WrapMode.ClampForever);
			halloween.windsword_enable = false;
		}

		public override void OnExitState()
		{
		}
	}
}
