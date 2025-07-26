using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class MainMapUIController : MonoBehaviour
{
	private static MainMapUIController instance;

	private bool is_inited;

	private List<UIPanelController> panel_set = new List<UIPanelController>();

	public TUIFade fade_panel;

	public GameObject main_mission_button;

	public GameObject side_mission_button1;

	public GameObject side_mission_button2;

	public GameObject daily_mission_button;

	public MissionInfoPanelController mission_info_panel;

	public DailyInfoPanelController daily_info_panel;

	public GameObject[] mission_tags;

	public TUILabel cash_label;

	public static MainMapUIController Instance
	{
		get
		{
			return instance;
		}
	}

	public bool Inited
	{
		get
		{
			return is_inited;
		}
	}

	private void Awake()
	{
		instance = this;
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		GameData.Instance.reset_nist_time_finish = null;
		GameData.Instance.reset_nist_time_error = null;
		instance = null;
	}

	public void MoveCameraTo(Transform trans)
	{
	}

	public void HidePanels()
	{
		foreach (UIPanelController item in panel_set)
		{
			item.Hide();
		}
	}

	public void OnBackButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Debug.Log("Back!");
			if (fade_panel != null)
			{
				fade_panel.FadeOut("UIShop");
			}
		}
	}

	public void OnSideMissionButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameData.Instance.cur_quest_info = control.GetComponent<MapMissionButton>().quest_info;
			HidePanels();
			mission_info_panel.Show();
			mission_info_panel.mission_title.Text = "Mission type:" + GameData.Instance.cur_quest_info.mission_type;
			Debug.Log("mission comment:" + GameData.Instance.cur_quest_info.mission_tag + " scene:" + GameData.Instance.cur_quest_info.scene_name);
		}
	}

    public void OnMainMissionButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameData.Instance.cur_quest_info = control.GetComponent<MapMissionButton>().quest_info;
			HidePanels();
			mission_info_panel.Show();
			mission_info_panel.mission_title.Text = "Mission type:" + GameData.Instance.cur_quest_info.mission_type;
			Debug.Log("mission comment:" + GameData.Instance.cur_quest_info.mission_tag + " scene:" + GameData.Instance.cur_quest_info.scene_name);
		}
	}

	public void OnDailyMissionButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			IndicatorBlockController.Hide();
		}
	}

	public void OnMissionCancelButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Debug.Log("OnMissionCancelButton!");
			HidePanels();
		}
	}

    public void OnMissionGoButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 3)
        {
            return;
        }
        Debug.Log("OnMissionGoButton!");
        if (fade_panel != null)
        {
            GameData.Instance.loading_to_scene = GameData.Instance.cur_quest_info.scene_name;
            fade_panel.FadeOut("Loading");
        }
    }

    private void ResetCurNistTime()
	{
		Debug.Log("ResetCurNistTime...");
		StartCoroutine(GameData.Instance.ResetCurServerTime());
	}

	private void OnResetNistTimeFinish()
	{
		daily_info_panel.OnConnectFinish();
	}

	private void OnResetNistTimeError()
	{
		daily_info_panel.OnConnectError();
	}
}
