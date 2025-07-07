using UnityEngine;

namespace CoMZ2
{
	public class UIShopPlayerStateOnClick : UIShopPlayerState
	{
		public override void OnEnterState()
		{
			AnimationUtil.CrossAnimate(player.gameObject, player.ANI_ONCLICK, WrapMode.ClampForever, 0.1f);
			if ((bool)player.SubAnimationObj && player.AvatarType != AvatarType.Swat)
			{
				AnimationUtil.CrossAnimate(player.SubAnimationObj, player.ANI_ONCLICK, WrapMode.ClampForever, 0.1f);
			}
		}

		public override void DoStateLogic(float deltaTime)
		{
			if (AnimationUtil.IsAnimationPlayedPercentage(player.gameObject, player.ANI_ONCLICK, 0.9f))
			{
				player.SetPlayerState(player.OnclickAfterState);
			}
		}
	}
}
