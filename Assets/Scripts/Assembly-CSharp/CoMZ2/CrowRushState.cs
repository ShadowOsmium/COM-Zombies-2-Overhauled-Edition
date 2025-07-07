using UnityEngine;

namespace CoMZ2
{
	public class CrowRushState : EnemyState
	{
		protected CrowController crow;

		public override void DoStateLogic(float deltaTime)
		{
			if (!AnimationUtil.IsPlayingAnimation(crow.gameObject, crow.ANI_RUSH))
			{
				AnimationUtil.CrossAnimate(crow.gameObject, crow.ANI_RUSH, WrapMode.Loop);
			}
			crow.DoRush(deltaTime);
		}

		public override void OnEnterState()
		{
			if (crow == null)
			{
				crow = m_enemy as CrowController;
			}
		}

		public override void OnExitState()
		{
		}
	}
}
