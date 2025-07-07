using System;
using CoMZ2;
using UnityEngine;

public class CrystalMsgBoxController : GameMsgBoxController
{
	public CrystalExchangeCash Exchange;

	private void OnOKButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3 && on_button_ok != null)
		{
			on_button_ok();
		}
	}

	private void OnCancelButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3 && on_button_cancel != null)
		{
			on_button_cancel();
		}
	}

	public static CrystalMsgBoxController ShowMsgBox(CrystalExchangeCash exchange, GameObject root_obj, string content, Action OnButtonOk, Action OnButtonCancel)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/Message_Box_Crystal")) as GameObject;
		CrystalMsgBoxController component = gameObject.GetComponent<CrystalMsgBoxController>();
		gameObject.transform.parent = root_obj.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, -20f);
		gameObject.transform.localRotation = Quaternion.identity;
		component.SetContent(content);
		component.on_button_ok = OnButtonOk;
		component.on_button_cancel = OnButtonCancel;
		component.ok_button.m_NormalLabelObj.GetComponent<TUILabel>().Text = exchange.Crystal.ToString("G");
		component.ok_button.m_PressLabelObj.GetComponent<TUILabel>().Text = exchange.Crystal.ToString("G");
		component.Exchange = exchange;
		return component;
	}
}
