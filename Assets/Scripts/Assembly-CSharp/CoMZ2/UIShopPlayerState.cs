namespace CoMZ2
{
	public class UIShopPlayerState : ObjectState
	{
		protected UIShopPlayerStateType stateType;

		protected UIShopPlayerController player;

		public UIShopPlayerStateType GetStateType()
		{
			return stateType;
		}

		public static UIShopPlayerState Create(UIShopPlayerStateType type, UIShopPlayerController player)
		{
			UIShopPlayerState uIShopPlayerState = null;
			switch (type)
			{
			case UIShopPlayerStateType.Idle:
				uIShopPlayerState = new UIShopPlayerStateIdle();
				break;
			case UIShopPlayerStateType.IdleAfter:
				uIShopPlayerState = new UIShopPlayerStateIdleAfter();
				break;
			case UIShopPlayerStateType.OnClick:
				uIShopPlayerState = new UIShopPlayerStateOnClick();
				break;
			case UIShopPlayerStateType.OnCLickAfter:
				uIShopPlayerState = new UIShopPlayerStateOnClickAfter();
				break;
			default:
				uIShopPlayerState = new UIShopPlayerStateIdle();
				break;
			}
			uIShopPlayerState.stateType = type;
			uIShopPlayerState.player = player;
			return uIShopPlayerState;
		}
	}
}
