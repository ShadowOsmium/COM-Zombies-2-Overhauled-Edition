using System.Collections;
using System.Collections.Generic;
using System.Text;
using CoMZ2;
using UnityEngine;

public class GameCoverUIController : MonoBehaviour
{
	protected static GameCoverUIController instance;

	public TUIFade fade;

	public GameObject mask_ui;

	private int ready_update_file_count;

	private float cur_check_time;

	private float total_check_time = 15f;

	private bool check_time_out;

	public TUIMeshSprite bg_img;

	public GameObject tui_root;

	public TUILabel redeem_award_cout_label;

	public UIRedeemAwardPanelController redeem_award_panel;

	public UIRedeemPanelController redeem_panel;

	public static GameCoverUIController Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		MenuAudioController.CheckGameMenuAudio();
	}

	private IEnumerator Start()
	{
		yield return 1;
		if (GameDefine.IS_CONFIG_OUTPUT)
		{
			GameVersion.Instance.OutputVersionCheckFile();
		}
		yield return 1;
		ShowMask(true);
		yield return 1;
		ready_update_file_count = 0;
		GameVersion.Instance.CheckRemoteGameVersion(OnServerVersion, OnServerVersionError);
		yield return 1;
		OpenClikPlugin.Show(true);
		if (TrinitiAdAndroidPlugin.Instance().CanChartboost())
		{
			ChartBoostAndroid.showInterstitial(null);
		}
		yield return 1;
		IapCenter.Instance.CheckIapFailedReceipt(null, null);
		float width4 = Screen.width;
		float height4 = Screen.height;
		if (Mathf.Max(width4, height4) == 2048f)
		{
			width4 /= 2f;
			height4 /= 2f;
		}
//		Debug.LogError("------------ " + width4 + " " + height4);
		string tex_name5 = string.Empty;
		if (Mathf.Max(width4, height4) >= 1136f)
		{
			tex_name5 = "GameCoverUI_1136";
			width4 = 1136f;
			height4 = 640f;
		}
		else if (Mathf.Max(width4, height4) < 1136f && Mathf.Max(width4, height4) >= 960f)
		{
			tex_name5 = "GameCoverUI_1024";
			width4 = 1136f;
			height4 = 640f;
		}
		else
		{
			tex_name5 = "GameCoverUI_960";
		}
		tex_name5 = "GameCoverUI_1136";
		width4 = 1136f;
		height4 = 640f;
		bg_img.CustomizeTexture = Resources.Load("TUI/Textures/" + tex_name5) as Texture2D;
		bg_img.CustomizeRect = new Rect(0f, 0f, width4, height4);
		Hashtable data_tem = new Hashtable { { "count", 1 } };
		GameData.Instance.UploadStatistics("Login", data_tem);
	}

	private void Update()
	{
		if (!check_time_out)
		{
			cur_check_time += Time.deltaTime;
			if (cur_check_time >= total_check_time)
			{
				Debug.Log("Version check time out!");
				check_time_out = true;
				ShowMask(false);
			}
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void OnStartButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			//Debug.Log("Game start!");
			control.gameObject.SetActive(false);
			GameData.Instance.loading_to_scene = "UIShop";
			fade.FadeOut("Loading");
		}
	}

    private void OnServerVersion(bool status)
    {
        if (status)
        {
            CheckConfigVersion();
            return;
        }

        ShowMask(true);

        GameMsgBoxController.ShowMsgBox(
            GameMsgBoxController.MsgBoxType.SingleButton,
            tui_root,
            "You have to update the game to continue playing the game.",
            OnVersionUpdate,
            null,
            false
        );
    }

    private void OnVersionUpdate()
    {
        Application.OpenURL("https://github.com/ShadowOsmium/COM-Zombies-2-Overhauled-Edition/releases");
        Application.Quit();
    }

    private void OnServerVersionError()
	{
		//Debug.Log("OnServerVersionError");
		ShowMask(false);
	}

	public void ShowMask(bool state)
	{
		if (state)
		{
			if (mask_ui != null)
			{
				mask_ui.SetActive(state);
			}
			IndicatorBlockController.ShowIndicator(tui_root.gameObject, string.Empty);
		}
		else
		{
			if (mask_ui != null)
			{
				mask_ui.SetActive(state);
			}
			IndicatorBlockController.Hide();
		}
	}

	private void CheckConfigVersion()
	{
		if (check_time_out)
		{
			return;
		}
		List<string> list = new List<string>();
		bool flag = false;
		foreach (string key in GameConfig.Instance.Remote_Config_Version_Set.Keys)
		{
			if (GameConfig.Instance.Remote_Config_Version_Set[key] != GameConfig.Instance.Config_Version_Set[key])
			{
				ready_update_file_count++;
				list.Add(key);
				flag = true;
			}
		}
		foreach (string item in list)
		{
			string md5String = MD5Sample.GetMd5String(item);
			string empty = string.Empty;
			empty = ((!GameVersion.Instance.is_test_config) ? "Config/" : "ConfigTest/");
			string url = "https://github.com/ShadowOsmium/COM-Zombies-2/releases/latest" + empty + md5String + ".bytes?rand=" + Random.Range(10, 99999);
			wwwClient.Instance.SendHttpRequest(url, null, OnConfigUpdateFinish, OnConfigUpdateError, item);
		}
		if (!flag)
		{
			check_time_out = true;
		}
		CheckUpdateFileFinish(flag);
	}

	private void OnConfigUpdateFinish(string action, byte[] response_data)
	{
		if (!check_time_out)
		{
			string @string = Encoding.UTF8.GetString(response_data);
			string md5String = MD5Sample.GetMd5String(action);
			Utils.FileWriteString(Utils.SavePath() + md5String + ".bytes", @string);
			Debug.Log("Config update finish:" + action + " ver:" + GameConfig.Instance.Remote_Config_Version_Set[action]);
			GameConfig.Instance.Config_Version_Set[action] = GameConfig.Instance.Remote_Config_Version_Set[action];
			ready_update_file_count--;
			CheckUpdateFileFinish();
		}
	}

	private void OnConfigUpdateError(string action, byte[] post_data)
	{
		if (!check_time_out)
		{
			Debug.Log("OnConfigUpdateError:" + action);
			ready_update_file_count--;
			CheckUpdateFileFinish();
		}
	}

	private void CheckUpdateFileFinish(bool is_update_file = true)
	{
		if (is_update_file && ready_update_file_count == 0)
		{
			GameConfig.Instance.Init();
			GameData.Instance.SaveData();
			ShowMask(false);
		}
		else
		{
			ShowMask(false);
		}
	}

	private void OnRedeemButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			redeem_panel.Show();
		}
	}
}
