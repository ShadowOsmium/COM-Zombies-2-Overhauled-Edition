using System.Text.RegularExpressions;
using UnityEngine;

public class UISearchRoomPanelController : UIShopPanelController
{
	public TUILabel label_room_id;

	private TouchScreenKeyboard keyboard;

	private bool is_input_name;

	protected Regex myRex;

	public string input_str = string.Empty;

	public override void Show()
	{
		base.Show();
		if (GameConfig.IsEditorMode())
		{
			label_room_id.Text = (input_str = "542");
			return;
		}
		input_str = string.Empty;
		is_input_name = true;
		keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.NumberPad);
	}

	public override void Hide(bool isPopFromStack)
	{
		is_input_name = false;
		base.Hide(isPopFromStack);
	}

	private void Awake()
	{
		myRex = new Regex("^[0-9]+$");
	}

	private void Update()
	{
		if (is_input_name && keyboard != null)
		{
			label_room_id.Text = (input_str = keyboard.text);
		}
		if (input_str.Length > 10)
		{
			label_room_id.Text = (input_str = input_str.Substring(0, 10));
		}
	}

	private void OnCancelButton(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			if (keyboard != null)
			{
				keyboard.active = false;
			}
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			is_input_name = false;
			Hide(true);
		}
	}

	private void OnOKButton(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			if (keyboard != null)
			{
				keyboard.active = false;
			}
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			is_input_name = false;
			Match match = myRex.Match(input_str);
			if (input_str.Length > 0 && match.Success)
			{
				int room_id = int.Parse(input_str);
				UICoopHallController.Instance.JoinRoomPrivate(room_id);
				Hide(true);
			}
			else
			{
				GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, base.gameObject, "Invalid room id. Please try again!", OnMsgOkButton, null);
			}
		}
	}

	private void OnEditButton(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			if (GameConfig.IsEditorMode())
			{
				label_room_id.Text = (input_str = "542");
				return;
			}
			label_room_id.Text = (input_str = string.Empty);
			is_input_name = true;
			keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.NumberPad);
		}
	}

	private void OnMsgOkButton()
	{
		label_room_id.Text = (input_str = string.Empty);
		is_input_name = true;
		keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.NumberPad);
	}
}
