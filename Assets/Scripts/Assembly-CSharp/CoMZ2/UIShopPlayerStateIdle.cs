using UnityEngine;

namespace CoMZ2
{
	public class UIShopPlayerStateIdle : UIShopPlayerState
	{
		private const float IdleTime = 3f;

		private float currentTime;

		public override void OnEnterState()
		{
			currentTime = 0f;
			AnimationUtil.CrossAnimate(player.gameObject, player.ANI_IDLE, WrapMode.Loop);
			if ((bool)player.SubAnimationObj)
			{
				AnimationUtil.CrossAnimate(player.SubAnimationObj, player.ANI_IDLE, WrapMode.Loop);
			}
		}

		public override void DoStateLogic(float deltaTime)
		{
			currentTime += deltaTime;
			if (currentTime >= 3f)
			{
				player.SetPlayerState(player.IdleAfterState);
			}
		}
	}
}
