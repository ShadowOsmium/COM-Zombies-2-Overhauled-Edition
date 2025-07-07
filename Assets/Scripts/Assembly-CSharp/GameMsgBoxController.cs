using System;
using UnityEngine;

public class GameMsgBoxController : UIPanelController
{
	public enum MsgBoxType
	{
		SingleButton,
		DoubleButton
	}

	protected static GameMsgBoxController instance;

	public TUIButtonClick cancel_button;

	public TUIButtonClick ok_button;

	public TUILabel msg_content;

	public Action on_button_ok;

	public Action on_button_cancel;

	public bool m_auto_hide = true;

	public static GameMsgBoxController Instance
	{
		get
		{
			return instance;
		}
		set
		{
			instance = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		on_button_ok = null;
		on_button_cancel = null;
		instance = null;
	}

	private void OnOKButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			if (m_auto_hide)
			{
				Hide();
			}
			if (on_button_ok != null)
			{
				on_button_ok();
			}
		}
	}

	private void OnCancelButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			if (m_auto_hide)
			{
				Hide();
			}
			if (on_button_cancel != null)
			{
				on_button_cancel();
			}
		}
	}

	public void SetContent(string str)
	{
		msg_content.Text = str;
	}

	public static GameMsgBoxController ShowMsgBox(MsgBoxType type, GameObject root_obj, string content, Action OnButtonOk, Action OnButtonCancel, bool auto_hide = true)
	{
		if (Instance == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("TUI/Message_Box")) as GameObject;
			Instance = gameObject.GetComponent<GameMsgBoxController>();
		}
		Instance.transform.parent = root_obj.transform;
		Instance.transform.localPosition = new Vector3(0f, 0f, -60f);
		Instance.transform.localRotation = Quaternion.identity;
		Instance.m_auto_hide = auto_hide;
		Instance.Show();
		Instance.SetContent(content);
		Instance.on_button_ok = OnButtonOk;
		Instance.on_button_cancel = OnButtonCancel;
		switch (type)
		{
		case MsgBoxType.SingleButton:
			Instance.ok_button.gameObject.SetActive(true);
			Instance.cancel_button.gameObject.SetActive(false);
			Instance.ok_button.transform.localPosition = new Vector3(0f, Instance.ok_button.transform.localPosition.y, Instance.ok_button.transform.localPosition.z);
			break;
		case MsgBoxType.DoubleButton:
			Instance.ok_button.gameObject.SetActive(true);
			Instance.cancel_button.gameObject.SetActive(true);
			Instance.ok_button.transform.localPosition = new Vector3(60f, Instance.ok_button.transform.localPosition.y, Instance.ok_button.transform.localPosition.z);
			Instance.cancel_button.transform.localPosition = new Vector3(-60f, Instance.ok_button.transform.localPosition.y, Instance.ok_button.transform.localPosition.z);
			break;
		}
		return Instance;
	}

	public static void DestroyMsgBox()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(Instance.gameObject);
		}
	}
}
