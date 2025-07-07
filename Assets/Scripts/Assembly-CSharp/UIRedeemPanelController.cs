using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Boomlagoon.JSON;
using LitJson;
using UnityEngine;

public class UIRedeemPanelController : MonoBehaviour
{
	public TUILabel label_code;

	private TouchScreenKeyboard keyboard;

	private bool is_input;

	protected Regex myRex;

	public string input_str = string.Empty;

	protected string cur_redeem_code;

	protected GameDataInt cur_redeem_val = new GameDataInt(0);

	public void Show()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		base.gameObject.SetActive(true);
		if (GameConfig.IsEditorMode())
		{
			label_code.Text = (input_str = "F3120-C32925-I8020");
			return;
		}
		input_str = string.Empty;
		is_input = true;
		keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.ASCIICapable);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		is_input = false;
	}

	private void Awake()
	{
		myRex = new Regex("^[A-Za-z0-9-]+$");
	}

	private void Update()
	{
		if (is_input && keyboard != null)
		{
			label_code.Text = (input_str = keyboard.text);
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
			is_input = false;
			Hide();
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
			is_input = false;
			Match match = myRex.Match(input_str);
			if (input_str.Length > 0 && match.Success)
			{
				Hide();
				GameCoverUIController.Instance.ShowMask(true);
				SendRedeemCode(input_str);
			}
			else
			{
				GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, GameCoverUIController.Instance.tui_root, "Invalid code. Please try again!", OnMsgOkButton, null);
			}
		}
	}

	private void OnEditButton(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			if (GameConfig.IsEditorMode())
			{
				label_code.Text = (input_str = "542");
				return;
			}
			label_code.Text = (input_str = string.Empty);
			is_input = true;
			keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.ASCIICapable);
		}
	}

	private void OnMsgOkButton()
	{
		label_code.Text = (input_str = string.Empty);
		is_input = true;
		keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.ASCIICapable);
	}

	private void SendRedeemCode(string code)
	{
		cur_redeem_code = code;
		Hashtable hashtable = new Hashtable();
		hashtable["gcode"] = cur_redeem_code;
		string s = JsonMapper.ToJson(hashtable);
		byte[] post_data = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		//wwwClient.Instance.SendHttpRequest(GameData.Instance.redeem_get_url, post_data, OnRedeemCodeFinish, OnRedeemCodeError, "sendCode");
	}

	private void OnRedeemCodeFinish(string action, byte[] result_data)
	{
		string text = string.Empty;
		if (result_data == null)
		{
			Debug.LogError("OnTestFinish:" + action + " result_data error");
			GameCoverUIController.Instance.ShowMask(false);
			return;
		}
		byte[] array = XXTEAUtils.Decrypt(result_data, Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		if (array != null)
		{
			text = Encoding.UTF8.GetString(array);
		}
		if (text == string.Empty)
		{
			Debug.Log("gcode error!");
			GameCoverUIController.Instance.ShowMask(false);
			GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, GameCoverUIController.Instance.tui_root, "Invalid code.", null, null);
			return;
		}
		JSONObject jSONObject = JSONObject.Parse(text);
		string text2 = jSONObject["code"].ToString();
		string text3 = jSONObject["data"].ToString();
		text3 = text3.Replace("\"", string.Empty);
		if (text2 == "0")
		{
			cur_redeem_val = new GameDataInt(int.Parse(text3));
			SendAcceptRedeemCode();
		}
		else
		{
			GameCoverUIController.Instance.ShowMask(false);
		}
	}

	private void OnRedeemCodeError(string action, byte[] result_data)
	{
		GameCoverUIController.Instance.ShowMask(false);
		GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, GameCoverUIController.Instance.tui_root, "Unable to connect to the server! Please try again later.", null, null);
	}

	private void SendAcceptRedeemCode()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["gcode"] = cur_redeem_code;
		string s = JsonMapper.ToJson(hashtable);
		byte[] post_data = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		//wwwClient.Instance.SendHttpRequest(GameData.Instance.redeem_accept_url, post_data, OnAcceptRedeemCodeFinish, OnAccpetRedeemCodeError, "acceptCode");
	}

	private void OnAcceptRedeemCodeFinish(string action, byte[] result_data)
	{
		GameCoverUIController.Instance.ShowMask(false);
		string text = string.Empty;
		if (result_data == null)
		{
			Debug.LogError("OnTestFinish:" + action + " result_data error");
			return;
		}
		byte[] array = XXTEAUtils.Decrypt(result_data, Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		if (array != null)
		{
			text = Encoding.UTF8.GetString(array);
		}
		if (text == string.Empty)
		{
			Debug.Log("gcode error!");
			return;
		}
		JSONObject jSONObject = JSONObject.Parse(text);
		string text2 = jSONObject["code"].ToString();
		if (text2 == "0")
		{
			int intVal = cur_redeem_val.GetIntVal();
			//GameData.Instance.total_crystal += (int)((float)intVal * GameData.Instance.redeem_change_ratio);
			GameData.Instance.SaveData();
			GameCoverUIController.Instance.ShowMask(false);
			GameCoverUIController.Instance.redeem_award_panel.Show();
			GameCoverUIController.Instance.redeem_award_cout_label.Text = intVal.ToString();
		}
		else
		{
			GameCoverUIController.Instance.ShowMask(false);
		}
	}

	private void OnAccpetRedeemCodeError(string action, byte[] result_data)
	{
		GameCoverUIController.Instance.ShowMask(false);
		GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, GameCoverUIController.Instance.tui_root, "Unable to connect to the server! Please try again later.", null, null);
	}
}
