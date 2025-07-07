using System.Collections;
using System.Text;
using Boomlagoon.JSON;
using LitJson;
using UnityEngine;

public class TestUIController : MonoBehaviour
{
	private void Awake()
	{
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		MenuAudioController.CheckGameMenuAudio();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void Test()
	{
		string url = "http://184.168.72.188:9218/gameapi/comzb.do?action=comzb/GetGiftPackage&json=";
		Hashtable hashtable = new Hashtable();
		hashtable["gcode"] = "M1425-V32925-S4408";
		string s = JsonMapper.ToJson(hashtable);
		byte[] post_data = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		wwwClient.Instance.SendHttpRequest(url, post_data, OnTestFinish, OnTestError, "sendCode");
	}

	private void OnTestFinish(string action, byte[] result_data)
	{
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
		string text3 = jSONObject["data"].ToString();
		text3 = text3.Replace("\"", string.Empty);
	}

	private void OnTestError(string action, byte[] result_data)
	{
	}

	private void OnTestButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Test();
		}
	}
}
