using UnityEngine;

namespace CoMZ2
{
	public class UIShopPlayerStateOnClickAfter : UIShopPlayerState
	{
		public override void OnEnterState()
		{
			AnimationUtil.CrossAnimate(player.gameObject, player.ANI_ONCLICK_AFTER, WrapMode.Loop);
			if ((bool)player.SubAnimationObj)
			{
				AnimationUtil.CrossAnimate(player.SubAnimationObj, player.ANI_ONCLICK_AFTER, WrapMode.Loop);
			}
		}
	}
}
