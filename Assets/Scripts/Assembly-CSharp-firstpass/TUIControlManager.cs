using UnityEngine;

public class TUIControlManager : TUIControl
{
	private TUIHandler handler;

	public void Initialize(int layer)
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		SetControlsLayer(base.gameObject, layer);
	}

	public void SetHandler(TUIHandler handler)
	{
		this.handler = handler;
	}

	public override void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (handler != null)
		{
			handler.HandleEvent(control, eventType, wparam, lparam, data);
		}
	}

	private void SetControlsLayer(GameObject obj, int iLayer)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			GameObject gameObject = obj.transform.GetChild(i).gameObject;
			gameObject.layer = iLayer;
			SetControlsLayer(gameObject, iLayer);
		}
	}
}
