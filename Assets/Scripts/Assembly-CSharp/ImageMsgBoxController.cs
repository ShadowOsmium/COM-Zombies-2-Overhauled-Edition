using System;
using UnityEngine;

public class ImageMsgBoxController : UIPanelController
{
	public TUIMeshSprite Image;

	protected static ImageMsgBoxController instance;

	public TUIButtonClick ok_button;

	public TUILabel msg_content;

	public Action on_button_ok;

	public Action on_button_cancel;

	public bool m_auto_hide = true;

	public static ImageMsgBoxController Instance
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

	public void SetContent(string str)
	{
		msg_content.Text = str;
	}

	public static ImageMsgBoxController ShowMsgBox(GameMsgBoxController.MsgBoxType type, string imageName, GameObject root_obj, string content, Action OnButtonOk, Action OnButtonCancel, bool auto_hide = true)
	{
		if (Instance == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/Message_Box_Image")) as GameObject;
			Instance = gameObject.GetComponent<ImageMsgBoxController>();
		}
		Instance.transform.parent = root_obj.transform;
		Instance.transform.localPosition = new Vector3(0f, 0f, -60f);
		Instance.transform.localRotation = Quaternion.identity;
		Instance.m_auto_hide = auto_hide;
		Instance.Show();
		Instance.SetContent(content);
		Instance.Image.texture = imageName;
		Instance.on_button_ok = OnButtonOk;
		Instance.on_button_cancel = OnButtonCancel;
		Instance.ok_button.gameObject.SetActive(true);
		Instance.ok_button.transform.localPosition = new Vector3(0f, Instance.ok_button.transform.localPosition.y, Instance.ok_button.transform.localPosition.z);
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
