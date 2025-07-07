using UnityEngine;

public class UIPromotionPanelController : UIShopPanelController
{
	public GameObject Promotion_Button;

	public void ShowButton()
	{
		Promotion_Button.SetActive(true);
	}

	public void HideButton()
	{
		Promotion_Button.SetActive(false);
	}

	private void OnCOMPromotionEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			UIShopSceneController.Instance.PromotionPanel.Show();
		}
	}

	private void OnCOMZombiesEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://github.com/LITTLECHOPT8/COMZ-Online-Releases");
		}
	}

	private void OnCOMInfinityEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.trinitigame.android.callofminiinfinity");
		}
	}

	private void OnCOMDinoHunterEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://github.com/ShadowOsmium/COM-Dino-Hunter-Remastered-Edition");
		}
	}

	private void OnCOMBrawlersEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://github.com/Zweronz/Call-Of-Mini-Brawlers-Source/releases/tag/redecomp");
		}
	}

	private void OnCOMDoubleShootEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://github.com/Zweronz/Call-Of-Mini-Double-Shot-Ultimate/releases/tag/DS   ");
		}
	}

	private void OnCOMSniperEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Application.OpenURL("https://github.com/Zweronz/Call-Of-Mini-Sniper/releases/tag/1.0");
		}
	}
}
