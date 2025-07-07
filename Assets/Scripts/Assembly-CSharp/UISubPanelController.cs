using UnityEngine;

public class UISubPanelController : UIShopPanelController
{
	public override void Show()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		base.gameObject.SetActive(true);
		if (UISceneController.Instance.PanelStack.Count == 0)
		{
			UISceneController.Instance.PanelStack.Push(this);
		}
		else if (UISceneController.Instance.PanelStack.Peek() != this)
		{
			UISceneController.Instance.PanelStack.Peek().Hide(false);
			UISceneController.Instance.PanelStack.Push(this);
		}
	}

	public override void Hide(bool isPopFromStack)
	{
		base.gameObject.SetActive(false);
		UISceneController.Instance.BackButton.gameObject.SetActive(false);
		UISceneController.Instance.BackButton.m_NormalObj.transform.parent.gameObject.SetActive(false);
		if (isPopFromStack)
		{
			UISceneController.Instance.PanelStack.Pop();
			if (UISceneController.Instance.PanelStack.Count > 0)
			{
				UISceneController.Instance.PanelStack.Peek().Show();
			}
		}
	}
}
