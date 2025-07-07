using UnityEngine;

public class UIShopPanelController : MonoBehaviour
{
	public virtual void Show()
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
		if (UISceneController.Instance.BackButton == null)
		{
			Debug.LogWarning("no full screen back button.");
		}
		else
		{
			UISceneController.Instance.BackButton.transform.parent = base.transform;
			UISceneController.Instance.BackButton.transform.localPosition = new Vector3(0f, 0f, 0.5f);
			UISceneController.Instance.BackButton.gameObject.SetActive(true);
			UISceneController.Instance.BackButton.m_NormalObj.transform.parent.gameObject.SetActive(true);
		}
		if (UISceneController.Instance is UIMapSceneController)
		{
			UIMapSceneController.Instance.BackToShopButton.SetActive(false);
		}
		if (UISceneController.Instance is UIShopSceneController)
		{
			UIShopSceneController.Instance.PromotionPanel.HideButton();
		}
	}

	public virtual void Hide(bool isPopFromStack)
	{
		base.gameObject.SetActive(false);
		if (UISceneController.Instance.BackButton == null)
		{
			Debug.LogWarning("no full screen back button.");
		}
		else
		{
			UISceneController.Instance.BackButton.gameObject.SetActive(false);
			UISceneController.Instance.BackButton.m_NormalObj.transform.parent.gameObject.SetActive(false);
		}
		if (isPopFromStack)
		{
			UISceneController.Instance.PanelStack.Pop();
			if (UISceneController.Instance.PanelStack.Count > 0)
			{
				UISceneController.Instance.PanelStack.Peek().Show();
			}
		}
		if (UISceneController.Instance is UIMapSceneController && UIMapSceneController.Instance.PanelStack.Count == 0)
		{
			UIMapSceneController.Instance.BackToShopButton.SetActive(true);
		}
		if (UISceneController.Instance is UIShopSceneController && UIShopSceneController.Instance.PanelStack.Count == 0)
		{
			UIShopSceneController.Instance.PromotionPanel.ShowButton();
		}
	}

	protected void ChangeMask(string maskName)
	{
		if (!(UISceneController.Instance.BackButton == null))
		{
			TUIMeshSprite component = UISceneController.Instance.BackButton.transform.Find("Mask").GetComponent<TUIMeshSprite>();
			if (component.m_texture != maskName)
			{
				component.texture = maskName;
			}
		}
	}
}
