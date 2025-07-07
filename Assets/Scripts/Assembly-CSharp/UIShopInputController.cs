using CoMZ2;
using UnityEngine;

public class UIShopInputController : MonoBehaviour
{
	public TUICamera TUICamera;

	private void OnTouchEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		TUIInput tUIInput = (TUIInput)data;
		if (tUIInput.inputType == TUIInputType.Began)
		{
			if (!UIShopSceneController.Instance.CheckShopCameraAtOrigin())
			{
				return;
			}
			Vector3 position = TUICamera.GetComponent<Camera>().WorldToScreenPoint(new Vector3(tUIInput.position.x, tUIInput.position.y, 0.3f));
			Vector3 vector = Camera.main.ScreenToWorldPoint(position);
			Ray ray = new Ray(Camera.main.transform.position, vector - Camera.main.transform.position);
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo, 1000f, 1 << PhysicsLayer.UI_OBJECT))
			{
				return;
			}
			UIShopPlayerController component = hitInfo.collider.gameObject.GetComponent<UIShopPlayerController>();
			if (component != null)
			{
				UIShopSceneController.Instance.SceneAudio.PlayAudio("UI_click");
				UIShopSceneController.Instance.ShowAvatarPanel(component);
			}
			else if (hitInfo.collider.gameObject.name == "map")
			{
				if (UIShopSceneController.Instance.PanelStack.Count == 0)
				{
					UIShopSceneController.Instance.SceneAudio.PlayAudio("UI_click");
					UIShopSceneController.Instance.Fade.FadeOut("UIMap");
					//GameData.Instance.SaveData();
				}
			}
			else if (hitInfo.collider.gameObject.name == "Lottery Draw" || hitInfo.collider.gameObject.name == "LotteryTip")
			{
				Debug.Log("go to Lottery");
				UIShopSceneController.Instance.Fade.FadeOut("UILottery");
			}
		}
		else if (tUIInput.inputType != TUIInputType.Moved && tUIInput.inputType != TUIInputType.Ended)
		{
		}
	}

	private void OnBackToSceneEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UIShopSceneController.Instance.SceneAudio.PlayAudio("UI_back");
			if (UIShopSceneController.Instance.PanelStack.Count > 0)
			{
				UIShopSceneController.Instance.PanelStack.Peek().Hide(true);
			}
			IndicatorBlockController.Hide();
		}
	}
}
