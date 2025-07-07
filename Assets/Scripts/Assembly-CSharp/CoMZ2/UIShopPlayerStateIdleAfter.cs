using UnityEngine;

namespace CoMZ2
{
	public class UIShopPlayerStateIdleAfter : UIShopPlayerState
	{
		public override void OnEnterState()
		{
			AnimationUtil.CrossAnimate(player.gameObject, player.ANI_IDLE_AFTER, WrapMode.ClampForever, 0.1f);
			if ((bool)player.SubAnimationObj)
			{
				AnimationUtil.CrossAnimate(player.SubAnimationObj, player.ANI_IDLE_AFTER, WrapMode.ClampForever, 0.1f);
			}
		}

		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(player.gameObject, player.ANI_IDLE_AFTER, 0.9f))
			{
				player.SetPlayerState(player.IdleState);
			}
		}
	}
}
