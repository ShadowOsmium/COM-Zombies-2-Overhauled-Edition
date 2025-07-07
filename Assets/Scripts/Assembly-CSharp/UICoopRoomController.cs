using System.Collections;
using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

public class UICoopRoomController : UISceneController
{
	private TNetObject tnetObj;

	public List<CoopRoomUserItem> user_item_list = new List<CoopRoomUserItem>();

	public TUIButtonClick start_button;

	public GameObject reward_root;

	public TUIMeshSprite reward_weapon;

	public TUILabel reward_weapon_name;

	public TUILabel room_id_label;

	public TUIMeshSprite boss_icon;

	public new static UICoopRoomController Instance
	{
		get
		{
			return (UICoopRoomController)UISceneController.instance;
		}
	}

	private void Awake()
	{
		UISceneController.instance = this;
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		MenuAudioController.CheckGameMenuAudio();
		//start_button.gameObject.SetActive(false);
	}

	private IEnumerator Start()
	{
		/*if (TNetConnection.IsInitialized)
		{
			tnetObj = TNetConnection.Connection;
			tnetObj.AddEventListener(TNetEventSystem.CONNECTION_KILLED, OnConnectionLost);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_RENEW, OnReverseHearRenew);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_TIMEOUT, OnReverseHearTimeout);
			tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_WAITING, OnReverseHearWaiting);
			tnetObj.AddEventListener(TNetEventRoom.USER_ENTER_ROOM, OnUserEnterRoom);
			tnetObj.AddEventListener(TNetEventRoom.USER_EXIT_ROOM, OnUserExitRoom);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_REMOVE, OnDestroyRoom);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_REMOVE_RES, OnDestroyRoomRes);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_VARIABLES_UPDATE, OnRoomVarsUpdate);
			tnetObj.AddEventListener(TNetEventRoom.USER_VARIABLES_UPDATE, OnUserVarsUpdate);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_NAME_CHANGE, OnRoomNameChange);
			tnetObj.AddEventListener(TNetEventRoom.USER_BE_KICKED, OnUserKicked);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_START, OnRoomStart);
			tnetObj.AddEventListener(TNetEventRoom.ROOM_MASTER_CHANGE, OnMasterChange);
			room_id_label.Text = "ROOM:" + tnetObj.CurRoom.Id;
		}
		else
		{
			Debug.LogError("tnetObj init error!");
		}*/
		//yield return 1;
		SFSObject dataObj = new SFSObject();
		dataObj.PutInt("avatarType", (int)GameData.Instance.cur_avatar);
		dataObj.PutUtfString("nickname", GameData.Instance.NickName);
		dataObj.PutInt("day", GameData.Instance.day_level);
		dataObj.PutInt("avatarLv", (int)GameData.Instance.AvatarData_Set[GameData.Instance.cur_avatar].avatar_state);
		dataObj.PutBool("InRoom", true);
		SFSObject userVars = new SFSObject();
		userVars.PutSFSObject("data", dataObj);
		//tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.RoomState, userVars));
		yield return 1;
		GameData.Instance.cur_coop_boss = (CoopBossType)PlayerPrefs.GetInt("curboss");//(CoopBossType)int.Parse(tnetObj.CurRoom.Commnet);
		RefreshClientsShow();
		StartCoroutine(CheckStartButtonState());
		GameData.Instance.cur_game_type = GameData.GamePlayType.Coop;
		CoopBossCfg boss_cfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
		int index = 0;
		CreateMoneyReward(boss_cfg.reward_crystal, "money_crystal", index++);
		CreateMoneyReward(boss_cfg.reward_gold, "money_cash", index++);
		foreach (string fragment in boss_cfg.rewards_weapon_fragments)
		{
			CreateDebrisReward(fragment, index++);
		}
		reward_weapon.texture = "Gameui_" + boss_cfg.reward_weapon;
		reward_weapon_name.Text = GameConfig.Instance.WeaponConfig_Set[boss_cfg.reward_weapon].show_name;
		boss_icon.texture = "big_boss_icon_" + (int)GameData.Instance.cur_coop_boss;
		yield return 3;
		SFSObject data_tem = new SFSObject();
		data_tem.PutBool("data", true);
//		tnetObj.Send(new SetUserVariableRequest(TNetUserVarType.RoomReady, userVars));
	}

	private void FixedUpdate()
	{
		if (tnetObj != null)
		{
			tnetObj.Update(Time.fixedDeltaTime);
		}
	}

	protected override void OnDestroy()
	{
		IndicatorBlockController.DestroyIndicatorBlock();
		TNetConnection.UnregisterSceneCallbacks();
		base.OnDestroy();
	}

	private void StartGame()
	{
		OnRoomStart(null);
		CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
		//string val = coopBossCfg.scene_list[Random.Range(0, coopBossCfg.scene_list.Count)];
		//SFSObject sFSObject = new SFSObject();
		//sFSObject.PutUtfString("SceneName", val);
		//sFSObject.PutDouble("GameStartTime", tnetObj.TimeManager.NetworkTime);
		//sFSObject.PutShort("CurBoss", (short)GameData.Instance.cur_coop_boss);
		//tnetObj.Send(new SetRoomVariableRequest(TNetRoomVarType.GameStarted, sFSObject));
		//IndicatorBlockController.ShowIndicator(TUIControls.gameObject, string.Empty);
	}

	private void OnStartButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		//if (eventType == 3)
		//{
		//	Debug.Log("OnStartButton");
			StartGame();
		//}
	}

	private void OnBackButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		//if (eventType == 3)
		//{
		//	Debug.Log("OnBackButton");
		//	tnetObj.Send(new LeaveRoomRequest());
		//	IndicatorBlockController.ShowIndicator(TUIControls.gameObject, string.Empty);
		//}
		Fade.FadeOut("UICoopHall");
	}

	private void OnKickButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Debug.Log("OnKickButton");
			IndicatorBlockController.ShowIndicator(TUIControls.gameObject, string.Empty);
			tnetObj.Send(new KickUserRequest(control.transform.parent.GetComponent<CoopRoomUserItem>().cur_user.Id));
		}
	}

	private void ClearClientsData()
	{
		foreach (CoopRoomUserItem item in user_item_list)
		{
			if (item != null)
			{
				item.cur_user = null;
				item.gameObject.SetActive(false);
			}
		}
	}

	private TNetUser FindRoomMaster()
	{
		//int roomMasterID = TNetConnection.Connection.CurRoom.RoomMasterID;
		//foreach (TNetUser user in TNetConnection.Connection.CurRoom.UserList)
		//{
		//	if (user.Id == roomMasterID)
		//	{
		//		return user;
		//	}
		//}
		return null;
	}

	private void RefreshClientsShow()
	{
		ClearClientsData();
		//TNetUser tNetUser = FindRoomMaster();
		//if (tNetUser == null)
		//{
		//	return;
		//}
		//int num = 0;
		user_item_list[0].ResetUserItem(null, false);
		//foreach (TNetUser user in TNetConnection.Connection.CurRoom.UserList)
		//{
		//	if (user.Id != tNetUser.Id && user_item_list[num].ResetUserItem(user, tnetObj.CurRoom.IsPasswordProtected))
		//	{
		//		num++;
		//	}
		//}
	}

	private void CreateMoneyReward(int amount, string image, int index)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefab/reward_money")) as GameObject;
		gameObject.transform.parent = reward_root.transform;
		gameObject.transform.localPosition = new Vector3(-90 + index * 45, 0f, -2f);
		gameObject.transform.Find("money").GetComponent<TUIMeshSprite>().texture = image;
		gameObject.transform.Find("value").GetComponent<TUILabel>().Text = amount.ToString("G");
	}

	private void CreateDebrisReward(string image, int index)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefab/reward_debris")) as GameObject;
		gameObject.transform.parent = reward_root.transform;
		gameObject.transform.localPosition = new Vector3(-90 + index * 45, 0f, -2f);
		gameObject.transform.Find("image").GetComponent<TUIMeshSprite>().texture = image;
	}

	private void OnConnectionTimeOut(TNetEventData evt)
	{
		Debug.Log("OnConnectionTimeOut");
	}

	private void OnDisConnection(TNetEventData evt)
	{
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

	private void OnUserEnterRoom(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		Debug.Log("User: " + tNetUser.Name + " has just joined Room.");
		RefreshClientsShow();
	}

	private void OnUserExitRoom(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		Debug.Log("User: " + tNetUser.Name + " has just left Room.");
		if (tNetUser.Id == tnetObj.Myself.Id)
		{
			TNetConnection.UnregisterSceneCallbacks();
			Fade.FadeOut("UICoopHall");
		}
		else
		{
			RefreshClientsShow();
		}
	}

	private void OnDestroyRoomRes(TNetEventData evt)
	{
		if ((int)evt.data["result"] == 0)
		{
			Debug.Log("OnDestroyRoomRes");
			TNetConnection.UnregisterSceneCallbacks();
			Fade.FadeOut("UICoopHall");
		}
	}

	private void OnDestroyRoom(TNetEventData evt)
	{
		Debug.Log("OnDestroyRoom");
		TNetConnection.UnregisterSceneCallbacks();
		GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "Room closed, try another one!", OnRoomDestroyButton, null);
	}

	private void OnRoomVarsUpdate(TNetEventData evt)
	{
		Debug.Log("On room variable update...");
		if ((int)evt.data["key"] == 0)
		{
			TNetUser tNetUser = (TNetUser)evt.data["user"];
			if (TNetConnection.IsServer && tNetUser.Id == tnetObj.Myself.Id)
			{
				tnetObj.Send(new RoomStartRequest());
				Debug.Log("Send Game Start, Start Time:" + tnetObj.CurRoom.GetVariable(TNetRoomVarType.GameStarted).GetDouble("GameStartTime"));
			}
		}
	}

	private void OnRoomStart(TNetEventData evt)
	{
		IndicatorBlockController.Hide();
		CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
		string utfString = coopBossCfg.scene_list[Random.Range(0, coopBossCfg.scene_list.Count)];
		GameData.Instance.cur_coop_boss = (CoopBossType)PlayerPrefs.GetInt("curboss");
		Debug.Log("On Room Start! scene name:" + utfString + " cur boss:" + GameData.Instance.cur_coop_boss);
		MenuAudioController.DestroyGameMenuAudio();
		GameData.Instance.loading_to_scene = utfString;
		Fade.FadeOut("Loading");
	}

	private void OnMasterChange(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		if (tNetUser != null)
		{
			Debug.Log("OnMasterChange..");
			if (tNetUser.Id == tnetObj.Myself.Id)
			{
				Debug.Log("I become master!");
				TNetConnection.IsServer = true;
				StartCoroutine(CheckStartButtonState());
			}
			RefreshClientsShow();
		}
	}

	private IEnumerator CheckStartButtonState()
	{
		yield return 1;
		if (TNetConnection.IsServer && tnetObj.CurRoom.IsPasswordProtected)
		{
			while (!tnetObj.TimeManager.IsSynchronized())
			{
				yield return 5;
			}
			start_button.gameObject.SetActive(true);
		}
	}

	private void OnRoomNameChange(TNetEventData evt)
	{
	}

	private void OnUserKicked(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		if (tNetUser.Id == tnetObj.Myself.Id)
		{
			TNetConnection.UnregisterSceneCallbacks();
			GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, TUIControls.gameObject, "You were removed from the room.", OnKickedButton, null);
		}
		else
		{
			IndicatorBlockController.Hide();
			RefreshClientsShow();
		}
	}

	private void OnUserVarsUpdate(TNetEventData evt)
	{
		Debug.Log("OnUserVarsUpdate");
		RefreshClientsShow();
		if (!TNetConnection.IsServer || tnetObj.CurRoom.IsPasswordProtected || tnetObj.CurRoom.UserCount < 4)
		{
			return;
		}
		bool flag = true;
		foreach (TNetUser user in tnetObj.CurRoom.UserList)
		{
			if (!user.ContainsVariable(TNetUserVarType.RoomReady))
			{
				flag = false;
			}
		}
		if (flag)
		{
			StartGame();
		}
	}

	private void OnOnConnectionLostButton()
	{
		Debug.LogWarning("OnOnConnectionLostButton");
		Fade.FadeOut("UICoopHall");
	}

	private void OnKickedButton()
	{
		Debug.LogWarning("OnKickedButton");
		Fade.FadeOut("UICoopHall");
	}

	private void OnRoomDestroyButton()
	{
		Debug.LogWarning("OnRoomDestroyButton");
		Fade.FadeOut("UICoopHall");
	}
}
