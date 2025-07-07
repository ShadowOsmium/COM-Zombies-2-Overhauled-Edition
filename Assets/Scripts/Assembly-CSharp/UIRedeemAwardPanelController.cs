using UnityEngine;

public class UIRedeemAwardPanelController : MonoBehaviour
{
	public void Show()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.transform.localPosition = new Vector3(0f, 2000f, base.transform.localPosition.z);
		base.gameObject.SetActive(false);
	}

	private void Awake()
	{
	}

	private void OnOKButton(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			Hide();
		}
	}
}
