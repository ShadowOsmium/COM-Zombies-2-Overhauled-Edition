using System.Collections.Generic;
using UnityEngine;

public class UISceneController : MonoBehaviour
{
	protected static UISceneController instance;

	public Stack<UIShopPanelController> PanelStack = new Stack<UIShopPanelController>();

	public TUIFade Fade;

	public Transform TUIControls;

	public TAudioController SceneAudio;

	public TUIButtonClick BackButton;

	public UIMoneyController MoneyController;

	public static UISceneController Instance
	{
		get
		{
			return instance;
		}
	}

	protected virtual void OnDestroy()
	{
		instance = null;
	}
}
