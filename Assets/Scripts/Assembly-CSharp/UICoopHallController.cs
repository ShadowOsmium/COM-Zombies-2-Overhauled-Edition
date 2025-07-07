using System;
using System.Collections;
using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

public class UICoopHallController : UISceneController
{
	protected const string room_password = "triniti";

	private TNetObject tNetObject;

	private GameMsgBoxController msgBox;

	private List<int> coop_group_list = new List<int>();

	public UIShopPanelController auto_match_panel;

	public UIShopPanelController search_room_panel;

	public GameObject boss_info_panel;

    public List<UICoopBossItem> coop_boss_item_list = new List<UICoopBossItem>();

	public GameObject reward_root;

	private List<GameObject> reward_list = new List<GameObject>();

	protected string coop_ip = string.Empty;

	protected int coop_port;

	protected bool is_private_room;

	public new static UICoopHallController Instance
	{
		get
		{
			return (UICoopHallController)UISceneController.instance;
		}
	}

	private void Awake()
	{
		UISceneController.instance = this;
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		MenuAudioController.CheckGameMenuAudio();
	}

	private IEnumerator Start()
	{
		for (int i = 0; i < 5; i++)
		{
			coop_group_list.Add(i + 8001 + GameConfig.Instance.server_group_index);
		}
		if (TNetConnection.IsInitialized)
		{
			tNetObject = TNetConnection.Connection;
		}
		else
		{
			tNetObject = new TNetObject();
		}
		TNetConnection.IsServer = false;
		yield return 0;
		tNetObject.AddEventListener(TNetEventSystem.CONNECTION, OnConnection);
		tNetObject.AddEventListener(TNetEventSystem.LOGIN, OnLogin);
		tNetObject.AddEventListener(TNetEventRoom.ROOM_CREATION, OnRoomCreation);
		tNetObject.AddEventListener(TNetEventRoom.ROOM_JOIN, OnRoomJoin);
		tNetObject.AddEventListener(TNetEventRoom.GET_ROOM_LIST, OnRoomList);
		tNetObject.AddEventListener(TNetEventSystem.DISCONNECT, OnDisConnection);
		tNetObject.AddEventListener(TNetEventSystem.CONNECTION_KILLED, OnConnectionLost);
		tNetObject.AddEventListener(TNetEventSystem.CONNECTION_TIMEOUT, OnConnectionTimeOut);
		tNetObject.AddEventListener(TNetEventSystem.REVERSE_HEART_RENEW, OnReverseHearRenew);
		tNetObject.AddEventListener(TNetEventSystem.REVERSE_HEART_TIMEOUT, OnReverseHearTimeout);
		tNetObject.AddEventListener(TNetEventSystem.REVERSE_HEART_WAITING, OnReverseHearWaiting);
		StartCoroutine(GetCoopIp());
        UpdateBossLocks();
    }

	private void FixedUpdate()
	{
		tNetObject.Update(Time.fixedDeltaTime);
	}

	protected override void OnDestroy()
	{
		IndicatorBlockController.DestroyIndicatorBlock();
		TNetConnection.UnregisterSceneCallbacks();
		base.OnDestroy();
	}

	private void ConnectServer(string ip, int port)
	{
		if (!tNetObject.IsContected())
		{
			//Debug.Log("Connecting to server: " + ip + " " + port);
			//tNetObject.Connect(ip, port);
		}
		else
		{
			//TNetConnection.Connection = tNetObject;
			//tNetObject.Send(new LoginRequest(GameData.Instance.NickName));
		}
	}

    private void OnCreateButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            CoopBossCfg bossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[
                GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];

            int requiredDay = Mathf.CeilToInt(bossCfg.day_level / 2f);
            int playerDayLevel = GameData.Instance.day_level;

            if (playerDayLevel < requiredDay)
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    TUIControls.gameObject,
                    bossCfg.boss_show_name + " is Locked.\nReach Day " + requiredDay + " to fight this Boss.",
                    null,
                    null
                );
                return;
            }

            int cur_coop_boss = (int)GameData.Instance.cur_coop_boss;
            PlayerPrefs.SetInt("curboss", cur_coop_boss);
            IndicatorBlockController.ShowIndicator(TUIControls.gameObject, string.Empty);
            is_private_room = true;
            OnRoomJoin(null);
        }
    }

    public List<T> RandomSortList<T>(List<T> ListT)
	{
		System.Random random = new System.Random();
		List<T> list = new List<T>();
		foreach (T item in ListT)
		{
			list.Insert(random.Next(list.Count + 1), item);
		}
		return list;
	}

	private void OnSeachButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Debug.Log("OnSeachButton");
			search_room_panel.Show();
		}
	}

	private void OnBossButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType != 3)
		{
			return;
		}
		CoopBossInfo component = control.GetComponent<CoopBossInfo>();
		GameData.Instance.cur_coop_boss = component.cur_boss_type;
		if (!boss_info_panel.activeSelf)
		{
			boss_info_panel.SetActive(true);
		}
		foreach (UICoopBossItem item in coop_boss_item_list)
		{
			item.SetItemChoiced(item == component.BossItem);
		}
		foreach (GameObject item2 in reward_list)
		{
			UnityEngine.Object.Destroy(item2);
		}
		reward_list.Clear();
		CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
		Debug.Log("boss info:" + coopBossCfg.ToString());
		int num = 0;
		CreateMoneyReward(coopBossCfg.reward_crystal, "money_crystal", num++);
		CreateMoneyReward(coopBossCfg.reward_gold, "money_cash", num++);
		foreach (string rewards_weapon_fragment in coopBossCfg.rewards_weapon_fragments)
		{
			CreateDebrisReward(rewards_weapon_fragment, num++);
		}
	}

private void OnAutoMatchSelectButton(TUIControl control, int eventType, float wparam, float lparam, object data)
{
    if (eventType == 3)
    {
        CoopBossCfg bossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[
            GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];

        int requiredDay = bossCfg.day_level / 2;
        int playerDayLevel = GameData.Instance.day_level;

        if (playerDayLevel < requiredDay)
        {
            GameMsgBoxController.ShowMsgBox(
                GameMsgBoxController.MsgBoxType.SingleButton,
                TUIControls.gameObject,
                bossCfg.boss_show_name + " is Locked.\nReach Day " + requiredDay + " to fight this Boss.",
                null,
                null
            );
            return;
        }

        IndicatorBlockController.ShowIndicator(TUIControls.gameObject, "Searching room...");
        Debug.Log("OnAutoMatchSelectButton:" + GameData.Instance.cur_coop_boss);

        tNetObject.Send(new GetRoomListRequest(
            8001 + (int)GameData.Instance.cur_coop_boss + GameConfig.Instance.server_group_index,
            0, 10,
            RoomDragListCmd.ListType.not_full_not_game
        ));
    }
}

    private void OnBackButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Debug.Log("OnBackButton");
			TNetConnection.UnregisterSceneCallbacks();
			TNetConnection.Disconnect();
			Fade.FadeOut("UIMap");
		}
	}

	private void CreateMoneyReward(int amount, string image, int index)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/reward_money")) as GameObject;
		gameObject.transform.parent = reward_root.transform;
		gameObject.transform.localPosition = new Vector3(index * 45, 0f, -2f);
		gameObject.transform.Find("money").GetComponent<TUIMeshSprite>().texture = image;
		gameObject.transform.Find("value").GetComponent<TUILabel>().Text = amount.ToString("G");
		reward_list.Add(gameObject);
	}

    private void CreateDebrisReward(string image, int index)
    {
        if (string.IsNullOrEmpty(image))
            return;

        GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/reward_debris")) as GameObject;
        gameObject.transform.parent = reward_root.transform;
        gameObject.transform.localPosition = new Vector3(index * 45, 0f, -2f);
        gameObject.transform.Find("image").GetComponent<TUIMeshSprite>().texture = image;
        reward_list.Add(gameObject);
    }

    private void OnConnection(TNetEventData eventData)
	{
		Debug.Log("On connected...");
		TNetConnection.Connection = tNetObject;
		tNetObject.Send(new LoginRequest(GameData.Instance.NickName));
	}

	private void OnLogin(TNetEventData eventData)
	{
		IndicatorBlockController.Hide();
		//if ((int)eventData.data["result"] == 0)
		//{
		//	Debug.Log("OnLogin OK! id:" + tNetObject.Myself.Id);
		//	return;
		//}
		//Debug.Log("OnLogin Fail!");
		//TNetConnection.UnregisterSceneCallbacks();
		//TNetConnection.Disconnect();
		//GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Unable to connect to the server! Please try again later.", OnOnConnectionFailedButton, null);
	}

	private void OnRoomCreation(TNetEventData eventData)
	{
		if ((int)eventData.data["result"] == 0)
		{
			Debug.Log("OnRoomCreation OK!, room id:" + eventData.data["roomId"]);
			TNetConnection.IsServer = true;
		}
		else
		{
			Debug.Log("OnRoomCreation Fail!");
			IndicatorBlockController.Hide();
			GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Create room failed!", null, null);
		}
	}

	private void OnRoomJoin(TNetEventData eventData)
	{
		//if ((int)eventData.data["result"] == 0)
		//{
			IndicatorBlockController.Hide();
			Debug.Log("OnRoomJoin OK!");
			Fade.FadeOut("UICoopRoom");
		//}
		//else
		//{
		//	IndicatorBlockController.Hide();
		//	Debug.Log("OnRoomJoin Fail! result:" + (RoomJoinResCmd.Result)(int)eventData.data["result"]);
		//	GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Join room failed!", null, null);
		//}
	}

	private void OnConnectionTimeOut(TNetEventData evt)
	{
		IndicatorBlockController.Hide();
		//Debug.Log("OnConnectionTimeOut");
		//TNetConnection.UnregisterSceneCallbacks();
		//TNetConnection.Disconnect();
		//GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Unable to connect to the server! Please try again later.", OnOnConnectionFailedButton, null);
	}

	private void OnDisConnection(TNetEventData evt)
	{
		IndicatorBlockController.Hide();
		Debug.Log("OnDisConnection");
	}

	private void OnConnectionLost(TNetEventData evt)
	{
		IndicatorBlockController.Hide();
		//Debug.Log("OnConnectionLost");
		//TNetConnection.UnregisterSceneCallbacks();
		//TNetConnection.Disconnect();
		//GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Unable to connect to the server! Please try again later.", OnOnConnectionLostButton, null);
	}

	private void OnReverseHearWaiting(TNetEventData evt)
	{
		Debug.LogWarning("OnReverseHearWaiting");
		IndicatorBlockController.ShowIndicator(TUIControls.gameObject, "Waiting for server...");
	}

	private void OnReverseHearRenew(TNetEventData evt)
	{
		Debug.LogWarning("OnReverseHearRenew");
		IndicatorBlockController.Hide();
	}

	private void OnReverseHearTimeout(TNetEventData evt)
	{
		Debug.LogWarning("OnReverseHearTimeout");
		IndicatorBlockController.Hide();
		//TNetConnection.UnregisterSceneCallbacks();
		//TNetConnection.Disconnect();
		//GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Unable to connect to the server! Please try again later.", OnOnConnectionLostButton, null);
	}

	private void OnRoomList(TNetEventData evt)
	{
		List<TNetRoom> list = (List<TNetRoom>)evt.data["roomList"];
		if (list.Count > 0)
		{
			Debug.Log("OnRoomList count:" + list.Count);
			foreach (TNetRoom item in list)
			{
				if (!item.IsPasswordProtected)
				{
					Debug.Log("Randomly join room:" + item.Name + "  room Commnet: " + item.Commnet);
					tNetObject.Send(new JoinRoomRequest(item.Id, string.Empty));
					IndicatorBlockController.Hide();
					return;
				}
			}
		}
		Debug.Log("Match failed, create room by self.");
		int cur_coop_boss = (int)GameData.Instance.cur_coop_boss;
		tNetObject.Send(new CreateRoomRequest(GameData.Instance.NickName + "'s Room", string.Empty, (int)(GameData.Instance.cur_coop_boss + 8001 + GameConfig.Instance.server_group_index), 4, RoomCreateCmd.RoomType.limit, RoomCreateCmd.RoomSwitchMasterType.Auto, cur_coop_boss.ToString()));
		is_private_room = false;
	}

	private void OnOnConnectionLostButton()
	{
		Debug.LogWarning("OnOnConnectionLostButton");
		Fade.FadeOut("UIMap");
	}

	private void OnOnConnectionFailedButton()
	{
		Debug.LogWarning("OnOnConnectionLostButton");
		Fade.FadeOut("UIMap");
	}

	public void JoinRoomPrivate(int room_id)
	{
		IndicatorBlockController.ShowIndicator(TUIControls.gameObject, string.Empty);
		Debug.Log("join room:" + room_id);
		tNetObject.Send(new JoinRoomRequest(room_id, "triniti"));
	}

	private IEnumerator GetCoopIp()
	{
		//IndicatorBlockController.ShowIndicator(TUIControls.gameObject, "Connecting to server...");
		WWW www = new WWW("http://account.trinitigame.com/game/callofminizombies2/new_version/CoMZ2_version.bytes?rand=" + UnityEngine.Random.Range(10, 99999));
		yield return www;
		if (www.error != null)
		{
			//Debug.Log(www.error);
			IndicatorBlockController.Hide();
			//GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Unable to connect to the server! Please try again later.", OnOnConnectionFailedButton, null);
			yield break;
		}
		string content2 = www.text;
		content2 = Encipher(content2);
		Configure cfg = new Configure();
		cfg.Load(content2);
		string ver2 = cfg.GetSingle("CoMZ2", "Ver");
		string ver_test2 = cfg.GetSingle("CoMZ2", "TestVer");
		ver2 = cfg.GetSingle("CoMZ2", "VerAndroid");
		ver_test2 = cfg.GetSingle("CoMZ2", "TestVerAndroid");
		coop_ip = cfg.GetSingle("CoMZ2", "CoopIP");
		coop_port = int.Parse(cfg.GetSingle("CoMZ2", "CoopPort"));
		coop_ip = cfg.GetSingle("CoMZ2", "CoopIPAndroid");
		coop_port = int.Parse(cfg.GetSingle("CoMZ2", "CoopPortAndroid"));
		if (ver2 == "2.1.2" || ver_test2 == "2.1.2")
		{
			if (ver2 == "2.1.2")
			{
				GameConfig.Instance.server_group_index = 0;
			}
			else if (ver_test2 == "2.1.2")
			{
				GameConfig.Instance.server_group_index = 500;
			}
			ConnectServer(coop_ip, coop_port);
		}
		else
		{
			Debug.Log("game version error.");
			IndicatorBlockController.Hide();
			GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "An updated version is required to continue playing.", OnVersionUpdate, null, false);
		}
	}

	private string Encipher(string data_encipher)
	{
		int num = 30;
		char[] array = data_encipher.ToCharArray();
		char[] array2 = data_encipher.ToCharArray();
		char[] array3 = new char[2] { '\0', '\0' };
		for (int i = 0; i < array.Length; i++)
		{
			char c = (array3[0] = array[i]);
			string s = new string(array3);
			int num2 = char.ConvertToUtf32(s, 0);
			num2 ^= num;
			array2[i] = char.ConvertFromUtf32(num2)[0];
		}
		return new string(array2);
	}

    private void UpdateBossLocks()
    {
        int playerDayLevel = GameData.Instance.day_level;

        foreach (var item in coop_boss_item_list)
        {
            CoopBossCfg bossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[
                GameConfig.GetEnemyTypeFromBossType(item.cur_boss_type)];

            int requiredDay = bossCfg.day_level / 2;
            bool isLocked = playerDayLevel < requiredDay;
            //item.SetLocked(isLocked, bossCfg.boss_show_name, requiredDay);
        }
    }

    private void OnVersionUpdate()
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.trinitigame.android.callofminizombies2");
	}
}
