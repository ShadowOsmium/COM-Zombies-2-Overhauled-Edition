using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class GameSceneCoopController : GameSceneController
{
    private TNetObject tnetObj;

    public GameObject ui_teammate_bar_ref;

    protected List<UITeammateBar> ui_teammate_bar_list = new List<UITeammateBar>();

    protected float teammate_bar_space = 40f;

    public GameRewardCoopPanelController reward_coop_panel;

    public Dictionary<int, WoodBoxController> woodbox_set = new Dictionary<int, WoodBoxController>();

    public Dictionary<int, GameItemController> game_item_set = new Dictionary<int, GameItemController>();

    public List<GameObject> game_item_ref_list = new List<GameObject>();

    public TUILabel wait_server_tip;

    public Transform team_bar_root;

    protected float coop_start_time;

    private string src_img = string.Empty;

    private string des_img = string.Empty;

    private int des_count;

    private bool award_get_changed;

    private bool show_bk;

    private int reward_cash_temp;

    public new static GameSceneCoopController Instance
    {
        get
        {
            return (GameSceneCoopController)GameSceneController.instance;
        }
    }

    private void Awake()
    {
        GameSceneController.instance = this;
        GameConfig.CheckGameConfig();
        GameData.CheckGameData();
        enemy_ref_map = base.gameObject.AddComponent<EnemyMap>();
        GameData.Instance.scene_state = GameData.GameSceneState.Gaming;
        if (GameConfig.Instance.editor_mode.is_enable)
        {
            GameData.Instance.cur_quest_info.mission_type = (MissionType)GameConfig.Instance.editor_mode.editor_mission_type;
            GameData.Instance.cur_quest_info.camera_roam_enable = GameConfig.Instance.editor_mode.editor_camera_roam;
        }
        OnSceneColorReset();
        if (TNetConnection.IsInitialized)
        {
            tnetObj = TNetConnection.Connection;
            tnetObj.AddEventListener(TNetEventSystem.CONNECTION_KILLED, OnConnectionLost);
            tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_RENEW, OnReverseHearRenew);
            tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_TIMEOUT, OnReverseHearTimeout);
            tnetObj.AddEventListener(TNetEventSystem.REVERSE_HEART_WAITING, OnReverseHearWaiting);
            tnetObj.AddEventListener(TNetEventRoom.USER_ENTER_ROOM, OnUserEnterRoom);
            tnetObj.AddEventListener(TNetEventRoom.USER_EXIT_ROOM, OnUserExitRoom);
            tnetObj.AddEventListener(TNetEventRoom.OBJECT_MESSAGE, OnObjectMessage);
            tnetObj.AddEventListener(TNetEventRoom.ROOM_VARIABLES_UPDATE, OnRoomVarsUpdate);
            tnetObj.AddEventListener(TNetEventRoom.USER_VARIABLES_UPDATE, OnUserVarsUpdate);
            tnetObj.AddEventListener(TNetEventRoom.LOCK_STH, OnLockSth);
            tnetObj.AddEventListener(TNetEventRoom.ROOM_MASTER_CHANGE, OnMasterChange);
        }
        else
        {
            //Debug.LogError("TNetConnection init error!");
        }
        EnemyType enemyTypeFromBossType = GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss);
        int num = Random.Range(0, 100) % 2;
        switch (enemyTypeFromBossType)
        {
            case EnemyType.E_FATCOOK:
            case EnemyType.E_FATCOOK_E:
                if (num == 0)
                {
                    enable_spawn_ani = "Boss_FatCook_Camera_Show01";
                }
                else
                {
                    enable_spawn_ani = "Boss_FatCook_Camera_Show02";
                }
                break;
            case EnemyType.E_HAOKE_A:
            case EnemyType.E_HAOKE_B:
                if (num == 0)
                {
                    enable_spawn_ani = "Haoke_Camera_Show01";
                }
                else
                {
                    enable_spawn_ani = "Haoke_Camera_Show02";
                }
                break;
            case EnemyType.E_WRESTLER:
            case EnemyType.E_WRESTLER_E:
                if (num == 0)
                {
                    enable_spawn_ani = "Wrestler_Camera_Show01";
                }
                else
                {
                    enable_spawn_ani = "Wrestler_Camera_Show02";
                }
                break;
            case EnemyType.E_HALLOWEEN:
            case EnemyType.E_HALLOWEEN_E:
                if (num == 0)
                {
                    enable_spawn_ani = "Hook_Demon_Camera_Show01";
                }
                else
                {
                    enable_spawn_ani = "Hook_Demon_Camera_Show02";
                }
                break;
            case EnemyType.E_SHARK:
            case EnemyType.E_SHARK_E:
                if (num == 0)
                {
                    enable_spawn_ani = "Zombie_Guter_Tennung_Camera_Show01";
                }
                else
                {
                    enable_spawn_ani = "Zombie_Guter_Tennung_Camera_Show02";
                }
                break;
        }
        wait_server_tip.gameObject.SetActive(false);
    }

    public override void OnSceneColorReset()
    {
        if (!(Application.loadedLevelName != "Depot") || !(Application.loadedLevelName != "Church") || !(Application.loadedLevelName != "GameTutorial") || !(Application.loadedLevelName != "Junkyard"))
        {
            return;
        }
        if (Application.loadedLevelName.StartsWith("COM2_"))
        {
            GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/ChannelMaterialSet")) as GameObject;
            {
                foreach (Material item in gameObject.GetComponent<MaterialPrefabReference>().material_set)
                {
                    item.SetColor("_Color", Color.white);
                }
                return;
            }
        }
        if (!Application.loadedLevelName.StartsWith("Lab_"))
        {
            return;
        }
        GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/LabMaterialSet")) as GameObject;
        foreach (Material item2 in gameObject2.GetComponent<MaterialPrefabReference>().material_set)
        {
            item2.SetColor("_Color", Color.white);
        }
    }

    private IEnumerator Start()
    {
        OpenClikPlugin.Hide();
        yield return 1;
        while (main_camera == null)
        {
            yield return 1;
        }
        while (input_controller == null)
        {
            yield return 1;
        }
        AvatarFactory.CreateAvatar(GameData.Instance.cur_avatar);
        yield return 1;
        while (player_controller == null)
        {
            yield return 1;
        }
        while (!GameConfig.Instance.Load_finished)
        {
            yield return 1;
        }
        DisableFireStickOnPCCoop();
        input_controller.Motor = player_controller.GetComponent<CharacterMotor>();
        main_camera.player = player_controller.transform;
        way_points = GameObject.FindGameObjectsWithTag("WayPoint");
        GameObject[] tem_array = GameObject.FindGameObjectsWithTag("Wood_Box");
        wood_box_list = new List<GameObject>(tem_array);
        yield return 1;
        boom_l_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        boom_l_pool.transform.position = Vector3.zero;
        boom_l_pool.transform.rotation = Quaternion.identity;
        boom_l_pool.GetComponent<ObjectPool>().Init("BoomLPool", Eff_Accessory[0], 1, 3f);
        boom_m_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        boom_m_pool.transform.position = Vector3.zero;
        boom_m_pool.transform.rotation = Quaternion.identity;
        boom_m_pool.GetComponent<ObjectPool>().Init("BoomMPool", Eff_Accessory[1], 3, 3f);
        blood_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        blood_pool.transform.position = Vector3.zero;
        blood_pool.transform.rotation = Quaternion.identity;
        blood_pool.GetComponent<ObjectPool>().Init("HitBloodPool", Eff_Accessory[2], 5, 0.6f);
        fire_line_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        fire_line_pool.transform.position = Vector3.zero;
        fire_line_pool.transform.rotation = Quaternion.identity;
        fire_line_pool.GetComponent<ObjectPool>().Init("FireLinePool", Eff_Accessory[3], 5, 1f);
        level_up_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        level_up_pool.transform.position = Vector3.zero;
        level_up_pool.transform.rotation = Quaternion.identity;
        level_up_pool.GetComponent<ObjectPool>().Init("LevelUpPool", Eff_Accessory[4], 1, 3f);
        pgm_sight_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        pgm_sight_pool.transform.position = Vector3.zero;
        pgm_sight_pool.transform.rotation = Quaternion.identity;
        pgm_sight_pool.GetComponent<ObjectPool>().Init("PgmSightPool", pgm_sight, 3, -1f);
        ground_stone_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        ground_stone_pool.transform.position = Vector3.zero;
        ground_stone_pool.transform.rotation = Quaternion.identity;
        ground_stone_pool.GetComponent<ObjectPool>().Init("GroundStonePool", Eff_Accessory[7], 3, 3f);
        assault_cartridge_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        assault_cartridge_pool.transform.position = Vector3.zero;
        assault_cartridge_pool.transform.rotation = Quaternion.identity;
        assault_cartridge_pool.GetComponent<ObjectPool>().Init("AssaultCartridgePool", Eff_Accessory[8], 3, 1f);
        shotgun_cartridge_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        shotgun_cartridge_pool.transform.position = Vector3.zero;
        shotgun_cartridge_pool.transform.rotation = Quaternion.identity;
        shotgun_cartridge_pool.GetComponent<ObjectPool>().Init("ShotgunCartridgePool", Eff_Accessory[9], 2, 1f);
        add_item_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        add_item_pool.transform.position = Vector3.zero;
        add_item_pool.transform.rotation = Quaternion.identity;
        add_item_pool.GetComponent<ObjectPool>().Init("AddItemPool", Eff_Accessory[10], 1, 3f);
        stone_boom_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        stone_boom_pool.transform.position = Vector3.zero;
        stone_boom_pool.transform.rotation = Quaternion.identity;
        stone_boom_pool.GetComponent<ObjectPool>().Init("StoneBoomPool", Eff_Accessory[13], 1, 3f);
        stone_boom_g_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        stone_boom_g_pool.transform.position = Vector3.zero;
        stone_boom_g_pool.transform.rotation = Quaternion.identity;
        stone_boom_g_pool.GetComponent<ObjectPool>().Init("StoneBoomGPool", Eff_Accessory[14], 1, 3f);
        hp_add_ring_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        hp_add_ring_pool.transform.position = Vector3.zero;
        hp_add_ring_pool.transform.rotation = Quaternion.identity;
        hp_add_ring_pool.GetComponent<ObjectPool>().Init("HpAddRingPool", Eff_Accessory[15], 1, 3f);
        boom_s_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        boom_s_pool.transform.position = Vector3.zero;
        boom_s_pool.transform.rotation = Quaternion.identity;
        boom_s_pool.GetComponent<ObjectPool>().Init("BoomSPool", Eff_Accessory[16], 3, 3f);
        combo_get_pool1 = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        combo_get_pool1.transform.position = Vector3.zero;
        combo_get_pool1.transform.rotation = Quaternion.identity;
        combo_get_pool1.GetComponent<ObjectPool>().Init("ComboGetPool1", Eff_Accessory[17], 1, 1.5f);
        combo_get_pool2 = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        combo_get_pool2.transform.position = Vector3.zero;
        combo_get_pool2.transform.rotation = Quaternion.identity;
        combo_get_pool2.GetComponent<ObjectPool>().Init("ComboGetPool2", Eff_Accessory[18], 1, 1.5f);
        combo_get_pool3 = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        combo_get_pool3.transform.position = Vector3.zero;
        combo_get_pool3.transform.rotation = Quaternion.identity;
        combo_get_pool3.GetComponent<ObjectPool>().Init("ComboGetPool3", Eff_Accessory[19], 1, 1.5f);
        boom_blood_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        boom_blood_pool.transform.position = Vector3.zero;
        boom_blood_pool.transform.rotation = Quaternion.identity;
        boom_blood_pool.GetComponent<ObjectPool>().Init("BoomBloodPool", Eff_Accessory[20], 1, 1.5f);
        blood_ground = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        blood_ground.transform.position = Vector3.zero;
        blood_ground.transform.rotation = Quaternion.identity;
        blood_ground.GetComponent<ObjectPool>().Init("BloodGroundPool", Eff_Accessory[21], 1, 6f);
        boom_ion_pool = Object.Instantiate(Resources.Load("Prefabs/ObjectPool")) as GameObject;
        boom_ion_pool.transform.position = Vector3.zero;
        boom_ion_pool.transform.rotation = Quaternion.identity;
        boom_ion_pool.GetComponent<ObjectPool>().Init("BoomIonPool", Eff_Accessory[22], 3, 3f);
        panel_set.Add(game_main_panel);
        panel_set.Add(game_reward_panel);
        panel_set.Add(game_failed_panel);
        panel_set.Add(game_pause_panel);
        panel_set.Add(cg_panel);
        panel_set.Add(unlock_panel);
        panel_set.Add(tutorial_panel);
        HidePanels();
        game_main_panel.Show();
        yield return 1;
        player_controller.InitSkill();
        string frame_name = GameData.Instance.AvatarData_Set[GameData.Instance.cur_avatar].avatar_name + "_0" + (int)GameData.Instance.AvatarData_Set[GameData.Instance.cur_avatar].avatar_state + "_icon";
        game_main_panel.SetIcon(frame_name);
        InitMissionController();
        yield return 2;
        is_inited = true;
        Screen.lockCursor = true;
        LoadingUIController.FinishedLoading();
        player_controller.UpdateWeaponUIShow();
        TAudioController audio_controller = main_camera.gameObject.AddComponent<TAudioController>();
        if (Application.loadedLevelName == "Depot")
        {
            music_name = "MusicMap01";
        }
        else if (Application.loadedLevelName == "Church")
        {
            music_name = "MusicMap02";
        }
        else
        {
            music_name = "MusicMap03";
        }
        audio_controller.PlayAudio(music_name);
        Hashtable data_tem = new Hashtable { { "Count", 1 } };
        GameData.Instance.UploadStatistics("Game_Count", data_tem);
    }

    private void FixedUpdate()
    {
        if (base.Inited && tnetObj != null)
        {
            tnetObj.Update(Time.fixedDeltaTime);
        }
    }


    private void Update()
    {
        if (Time.deltaTime != 0f && Time.timeScale != 0f && Time.time - last_check_mission_finished >= check_mission_rate)
        {
            last_check_mission_finished = Time.time;
            CheckMissionFinished();
            CheckCoopMissionOver();
        }
    }

    private void DisableFireStickOnPCCoop()
    {
        if (IsPCPlatform())
        {
            GameObject fireStickGO = GameObject.Find("TUI/TUIControls/Game_Panel/Fire_Stick");
            if (fireStickGO != null)
            {
                fireStickGO.SetActive(false);
                Debug.Log("Fire_Stick fully disabled on PC.");
            }
            else
            {
                Debug.LogWarning("Fire_Stick not found.");
            }
            GameObject moveStickGO = GameObject.Find("TUI/TUIControls/Game_Panel/Move_Stick");
            if (moveStickGO != null)
            {
                moveStickGO.SetActive(false);
                Debug.Log("Move_Stick disabled on PC.");
            }
            else
            {
                Debug.LogWarning("Move_Stick not found.");
            }

        }
    }

    private bool IsPCPlatform()
    {
        return Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.OSXPlayer
            || Application.platform == RuntimePlatform.OSXEditor
            || Application.platform == RuntimePlatform.LinuxPlayer;
    }

    private void OnDestroy()
    {
        TNetConnection.UnregisterSceneCallbacks();
        GameSceneController.instance = null;
    }

    public override void InitMissionController()
    {
        game_main_panel.HidePanels();
        mission_controller = base.gameObject.AddComponent<BossCoopMissionController>();
        cur_game_info_panel = game_main_panel.boss_panel;
        GameData.Instance.cur_quest_info.camera_roam_enable = false;
        game_main_panel.EnableNpcHpBar(false);

        List<EnemyType> missionEnemyTypeList = mission_controller.GetMissionEnemyTypeList();
        enemy_ref_map.ResetEnemyMapInfo(missionEnemyTypeList);

        ResetMissionDifficulty();
        GamePlayingState = PlayingState.Gaming;
        if (cur_game_info_panel != null)
        {
            cur_game_info_panel.Show();
        }
        Invoke("ShowMissionLabel", 0.5f);
    }

    public override void OnCancelRebirth()
	{
		Debug.Log("OnCancelRebirth");
		SetPlayerCoopDead();
	}

	public void SetPlayerCoopDead()
	{
		player_controller.is_coop_dead = true;
		if (tnetObj != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutUtfString("PlayerCoopDead", "0");
			tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		}
	}

    public override void MissionFinished()
    {
        int totalReward = 0;
        foreach (KeyValuePair<string, float> entry in GameSceneController.Instance.boss_damage_record)
        {
            int money = Mathf.FloorToInt(entry.Value / 5f);
            Debug.Log("game_reward: " + entry.Key + " damage:" + entry.Value + " money:" + money);
            totalReward += money;
        }

        GameData.Instance.total_cash.SetIntVal(
            GameData.Instance.total_cash.GetIntVal() + totalReward,
            GameDataIntPurpose.Cash
        );
        Debug.Log("Total Coop Reward Added: " + totalReward);

        GameSceneController.Instance.boss_damage_record.Clear();
        Debug.Log("Mission Finished:" + GamePlayingState);
        OpenClikPlugin.Hide();
        HidePanels();

        TAudioController tAudioController = main_camera.gameObject.AddComponent<TAudioController>();
        tAudioController.StopAudio(music_name);

        if (GamePlayingState == PlayingState.Lose)
        {
            reward_coop_panel.title_bk.texture = "BIAOTILAN-2";
            music_name = "MusicLose";
        }
        else if (GamePlayingState == PlayingState.Win)
        {
            reward_coop_panel.title_bk.texture = "BIAOTILAN";
            music_name = "MusicWin";
        }

        reward_coop_panel.Show();
        tAudioController.PlayAudio(music_name);
        MissionStatistics();
        GameData.Instance.SaveData();

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;

        if (tnetObj != null)
        {
            tnetObj.Send(new LeaveRoomRequest());
            TNetConnection.UnregisterSceneCallbacks();
        }

        Invoke("DestroyNetConnection", 0.5f);
    }

    private void MissionStatistics()
	{
	}

	public override void OnKeyManDead(ObjectController object_controller)
	{
		player_death_count++;
		cur_rebirth_man = object_controller;
		ChangeCameraFocus(object_controller.transform);
		mission_controller.SetMissionPaused(true);
		is_logic_paused = true;
		Invoke("SetLoseState", 1f);
	}

	public override void MissionFailed()
	{
		is_logic_paused = true;
		mission_check_finished = true;
		Invoke("SetLoseState", 1f);
	}

	public override void MissionWin()
	{
		mission_check_finished = true;
		mission_controller_finished = true;
		Invoke("SetWinState", 1f);
		Invoke("MissionFinished", 4f);
	}

	public override void SetWinState()
	{
		GamePlayingState = PlayingState.Win;
		CleanSceneEnemy();
		MissionReward();
		Debug.Log("SetWinState");
	}

    public override void SetLoseState()
    {
        GamePlayingState = PlayingState.Lose;

        cur_rebirth_cost = 5 * player_death_count;
        if (cur_rebirth_cost > 25)
            cur_rebirth_cost = 25;

        if (GameData.Instance.total_crystal.GetIntVal() >= cur_rebirth_cost)
        {
            game_main_panel.rebirth_panel.Show();
            Screen.lockCursor = false;
        }
        else
        {
            SetPlayerCoopDead();
            Screen.lockCursor = true;
        }
    }


    public void MissionRewardFailed()
    {
        List<GameRewardCoop> list = new List<GameRewardCoop>();
        CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
        Debug.Log("Coop boss reward:" + coopBossCfg.ToString());

        float totalDamage = 0f;
        PlayerID localPlayerID = player_controller.player_id;
        Debug.Log("[Reward Check] Local player ID: " + localPlayerID.ToString());

        foreach (KeyValuePair<PlayerID, float> pair in Player_damage_Set)
        {
            PlayerID key = pair.Key;
            float damage = pair.Value;
            totalDamage += damage;

            // Use your PlayerID equality (ignoring tnet_id) or custom comparison here:
            bool isLocalPlayer = key == localPlayerID; // Or key.Equals(localPlayerID)

            int rewardGold = Mathf.Min(Mathf.FloorToInt(damage / 5f), coopBossCfg.reward_gold_failed);

            if (isLocalPlayer)
            {
                GameData.Instance.total_cash.SetIntVal(
                    GameData.Instance.total_cash.GetIntVal() + rewardGold,
                    GameDataIntPurpose.Cash
                );
            }

            Debug.Log("game_reward: " + key.player_name + " damage: " + damage + " money: " + rewardGold);

            GameRewardCoop item = new GameRewardCoop(
                key.avatar_type,
                key.avatar_state,
                key.player_name,
                (int)damage,
                GameRewardCoop.RewardMoneyType.CASH,
                rewardGold,
                null,
                isLocalPlayer,
                false
            );

            list.Add(item);
            Screen.lockCursor = false;
        }

        Debug.Log("Total Coop Reward Added: " + Mathf.FloorToInt(totalDamage / 5f));
        reward_coop_panel.ResetGameReward(list);

        Hashtable hashtable = new Hashtable();
        hashtable.Add("Boss", coopBossCfg.boss_name);
        hashtable.Add("Time", (int)(Time.time - coop_start_time));
        GameData.Instance.UploadStatistics("Coop_Mission_Failed", hashtable);
    }

    public override void MissionReward()
	{
		List<GameRewardCoop> list = new List<GameRewardCoop>();
		CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
		Debug.Log("Coop boss reward:" + coopBossCfg.ToString());
		List<string> rewards_weapon_fragments = coopBossCfg.rewards_weapon_fragments;
		PlayerID playerID = new PlayerID(player_controller.avatar_data.avatar_type, player_controller.avatar_data.avatar_state, player_controller.avatar_data.show_name, 0);
		float num = 0f;
		foreach (PlayerID key in Player_damage_Set.Keys)
		{
			if (Player_damage_Set[key] > num)
			{
				playerID = key;
				num = Player_damage_Set[key];
			}
		}
        //if (playerID.tnet_id == player_controller.tnet_user.Id)
        {
            GameData.Instance.total_crystal.SetIntVal(
                GameData.Instance.total_crystal.GetIntVal() + coopBossCfg.reward_crystal,
                GameDataIntPurpose.Crystal
            );
        }
        //else
        {
            int num2 = (int)((float)coopBossCfg.reward_gold * (Player_damage_Set[player_controller.player_id] / coopBossCfg.hp_capacity));
            GameData.Instance.total_cash.SetIntVal(
                GameData.Instance.total_cash.GetIntVal() + num2,
                GameDataIntPurpose.Cash
            );
        }
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
		int num3 = 0;
		foreach (string item2 in rewards_weapon_fragments)
		{
			if (GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(item2))
			{
				num3++;
			}
		}
		switch (num3)
		{
		case 1:
			foreach (string item3 in rewards_weapon_fragments)
			{
				if (GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(item3))
				{
					dictionary.Add(item3, 40);
				}
				else
				{
					dictionary.Add(item3, 30);
				}
			}
			break;
		case 2:
			foreach (string item4 in rewards_weapon_fragments)
			{
				if (GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(item4))
				{
					dictionary.Add(item4, 40);
				}
				else
				{
					dictionary.Add(item4, 20);
				}
			}
			break;
		default:
			foreach (string item5 in rewards_weapon_fragments)
			{
				dictionary.Add(item5, 33);
			}
			break;
		}
		bool flag = false;
		int num4 = Random.Range(0, 101);
		if (num4 == 100)
		{
			flag = true;
		}
		int num5 = 0;
		string text = string.Empty;
		foreach (string key2 in dictionary.Keys)
		{
			num5 += dictionary[key2];
			if (num4 <= num5)
			{
				text = key2;
				break;
			}
		}
		bool flag2 = false;
		bool fragment_sell = false;
		//foreach (PlayerID key3 in Player_damage_Set.Keys)
		//{
			GameProb gameProb = null;
			GameReward gameReward = null;
			//flag2 = ((key3.tnet_id == player_controller.tnet_user.Id) ? true : false);
			flag2 = true;
			if (flag2)
			{
				if (flag)
				{
					if (GameData.Instance.WeaponData_Set[coopBossCfg.reward_weapon].LotteryReward(false))
					{
						Debug.Log("weapon:" + coopBossCfg.reward_weapon + " is unlocked, it enable combie.");
						unlock_new_weapon_name = coopBossCfg.reward_weapon;
						unlock_new_weapon = true;
						UnlockInGame unlockInGame = new UnlockInGame();
						unlockInGame.Type = UnlockInGame.UnlockType.Weapon;
						unlockInGame.Name = coopBossCfg.reward_weapon;
						GameData.Instance.UnlockList.Add(unlockInGame);
					}
					else
					{
						award_get_changed = true;
						src_img = "Gameui_" + coopBossCfg.reward_weapon;
						des_img = "Cash_s";
						des_count = GameData.Instance.WeaponData_Set[coopBossCfg.reward_weapon].config.sell_price.GetIntVal();
						show_bk = false;
					}
				}
				else if (GameData.Instance.WeaponData_Set[coopBossCfg.reward_weapon].exist_state != 0 || GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(text))
				{
					Debug.Log("drop_fragment already own.");
					fragment_sell = true;
					award_get_changed = true;
					src_img = text;
					des_img = "Cash_s";
					des_count = ((WeaponFragmentProbsCfg)GameConfig.Instance.ProbsConfig_Set[text]).sell_price.GetIntVal();
					show_bk = true;
				}
				else
				{
					gameProb = new GameProb();
					gameProb.prob_cfg = GameConfig.Instance.ProbsConfig_Set[text];
					gameProb.count = 1;
					GameData.Instance.WeaponFragmentProbs_Set.Add(text, gameProb);
					fragment_sell = false;
					string weapon_name = ((WeaponFragmentProbsCfg)gameProb.prob_cfg).weapon_name;
					Debug.Log("check weapon combine:" + weapon_name);
					if (GameData.Instance.CheckFragmentProbCombine(weapon_name) && GameData.Instance.WeaponData_Set[weapon_name].Unlock())
					{
						Debug.Log("weapon:" + weapon_name + " is now unlocked! You can now buy the weapon.");
						unlock_new_weapon_name = weapon_name;
						unlock_new_weapon = true;
						UnlockInGame unlockInGame2 = new UnlockInGame();
						unlockInGame2.Type = UnlockInGame.UnlockType.Weapon;
						unlockInGame2.Name = weapon_name;
						GameData.Instance.UnlockList.Add(unlockInGame2);
					}
				}
			}
			else
			{
				text = rewards_weapon_fragments[Random.Range(0, rewards_weapon_fragments.Count)];
				fragment_sell = false;
			}
			if (flag && flag2)
			{
				Debug.Log("Get weapon:" + coopBossCfg.reward_weapon);
				gameReward = new GameReward(GameReward.GameRewardType.WEAPON, "Gameui_" + coopBossCfg.reward_weapon, 1);
			}
			else
			{
				Debug.Log("Get fragment:" + text + " mySelf:" + flag2);
				WeaponFragmentProbsCfg weaponFragmentProbsCfg = GameConfig.Instance.ProbsConfig_Set[text] as WeaponFragmentProbsCfg;
				gameReward = new GameReward(GameReward.GameRewardType.WEAPONFRAGMENT, weaponFragmentProbsCfg.image_name, 1);
			}
			GameRewardCoop item;
			//if (key3.tnet_id == playerID.tnet_id)
			//{
				item = new GameRewardCoop(playerID.avatar_type, playerID.avatar_state, playerID.player_name, (int)coopBossCfg.hp_capacity, GameRewardCoop.RewardMoneyType.CRYSTAL, coopBossCfg.reward_crystal, gameReward, flag2, fragment_sell);
			//}
			//else
			//{
			//	reward_cash_temp = (int)((float)coopBossCfg.reward_gold * (Player_damage_Set[key3] / coopBossCfg.hp_capacity));
			//	item = new GameRewardCoop(key3.avatar_type, key3.avatar_state, key3.player_name, (int)Player_damage_Set[key3], GameRewardCoop.RewardMoneyType.CASH, reward_cash_temp, gameReward, flag2, fragment_sell);
			//}
			list.Add(item);
		//}
		reward_coop_panel.ResetGameReward(list);
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Boss", coopBossCfg.boss_name);
		hashtable.Add("Time", (int)(Time.time - coop_start_time));
		GameData.Instance.UploadStatistics("Coop_Mission_Win", hashtable);
	}

	public override void CheckMissionFinished()
	{
		if (/*TNetConnection.IsServer && */mission_controller_finished && !mission_check_finished && Enemy_Set.Count == 0)
		{
			//if (tnetObj != null && TNetConnection.IsServer)
			//{
			//	Debug.Log("send MissionWin");
			//	SFSObject sFSObject = new SFSObject();
			//	sFSObject.PutUtfString("MissionWin", "0");
			//	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
			//}
			MissionWin();
		}
	}

    public void OnRemoteMissionWin()
    {
        Debug.Log("Remote MissionWin");
        mission_check_finished = true;
        mission_controller_finished = true;

        UnlockCursor();

        MissionWin();
    }

    public void OnRemoteMissionFailed()
    {
        Debug.Log("Remote MissionFailed");
        is_logic_paused = true;
        GamePlayingState = PlayingState.Lose;
        mission_check_finished = true;
        mission_controller_finished = true;

        UnlockCursor();

        Invoke("MissionRewardFailed", 0.5f);
        Invoke("MissionFinished", 1f);
    }

    public override void OnGamePause()
    {
        HidePanels();
        game_pause_panel.Show();
        ((GamePausePanelController)game_pause_panel).ResetButtonState();

        TAudioManager.instance.musicVolume = 0f;
        TAudioManager.instance.soundVolume = 0f;

        UnlockCursor();
        Debug.Log("OnGamePause called");
        GameSceneController.Instance.canPressEscape = false;
    }

    public override void OnGameResume()
    {
        OpenClikPlugin.Hide();
        HidePanels();
        game_main_panel.Show();

        LockCursor();

        TAudioManager.instance.musicVolume = 0.5f;
        TAudioManager.instance.soundVolume = 0.5f;
        Debug.Log("OnGamePause called");
        GameSceneController.Instance.canPressEscape = true;
    }

    public override void OnGameQuit()
    {
        OpenClikPlugin.Hide();

        TAudioManager.instance.musicVolume = 0.5f;
        TAudioManager.instance.soundVolume = 0.5f;

        UnlockCursor();

        QuitGameForDisconnect(0.2f);
    }

    public override void OnRewardOkButton()
	{
		if (unlock_new_weapon)
		{
			ShowUnlockWeaponPanel();
		}
		else if (award_get_changed)
		{
            GameData.Instance.total_cash.SetIntVal(
                GameData.Instance.total_cash.GetIntVal() + des_count,
                GameDataIntPurpose.Cash
            );
            AwardChangePanel.ShowAwardChangePanel(game_main_panel.transform.parent.gameObject, OnAwardChangeButton, src_img, des_img, des_count, show_bk);
		}
		else
		{
			GoShopScene();
		}
	}

	private void OnAwardChangeButton()
	{
		GoShopScene();
	}

    private GameObject boss;

    public override void FatCookSummon()
    {
        if (mission_controller != null)
        {
            EnemyController boss = GetComponent<EnemyController>();
            StartCoroutine(mission_controller.SummonBossMinions(boss));
        }
    }

    public override void HalloweenSummon()
    {
        if (mission_controller != null)
        {
            EnemyController boss = GetComponent<EnemyController>();
            StartCoroutine(mission_controller.SummonBossMinions(boss));
        }
    }

    public override EnemyData GetBossData()
	{
		return ((BossCoopMissionController)mission_controller).MissionBoss.enemy_data;
	}

	public override void ResetMissionDifficulty()
	{
		CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
		enemy_standard_reward = GameData.Instance.GetSideEnemyStandardReward();
		enemy_standard_reward_total = GameData.Instance.GetSideEnemyStandardRewardTotal();
		int key = 0;
		int day_level = coopBossCfg.day_level;
		foreach (int key2 in GameConfig.Instance.Side_Quest_Hp_Difficulty_Set.Keys)
		{
			if (day_level >= key2)
			{
				key = key2;
			}
		}
		int hp_n = GameConfig.Instance.Side_Quest_Hp_Difficulty_Set[key].hp_n;
		float hpParaA = GameConfig.Instance.Side_Quest_Hp_Difficulty_Set[key].hpParaA;
		float hpParaB = GameConfig.Instance.Side_Quest_Hp_Difficulty_Set[key].hpParaB;
		enemy_standard_hp = hpParaA + hpParaB * (float)(day_level - hp_n);
		key = 0;
		day_level = coopBossCfg.day_level;
		foreach (int key3 in GameConfig.Instance.Side_Quest_Dmg_Difficulty_Set.Keys)
		{
			if (day_level >= key3)
			{
				key = key3;
			}
		}
		hpParaA = GameConfig.Instance.Side_Quest_Dmg_Difficulty_Set[key].dmgParaA;
		hpParaB = GameConfig.Instance.Side_Quest_Dmg_Difficulty_Set[key].dmgParaB;
		int dmg_n = GameConfig.Instance.Side_Quest_Dmg_Difficulty_Set[key].dmg_n;
		float hp_capacity = player_controller.avatar_data.hp_capacity;
		float fix_val = player_controller.avatar_data.config.fix_val;
		enemy_standard_dps = (hpParaA + hpParaB * (float)(day_level - dmg_n)) * fix_val;
		enemy_standard_reward *= GameConfig.Instance.Mission_Enemy_Reward_Info.side_ratio;
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Boss", coopBossCfg.boss_name);
		GameData.Instance.UploadStatistics("Coop_Mission", hashtable);
		coop_start_time = Time.time;
	}

	public override void OnBossMissionOver()
	{
		is_boss_dead = true;
		game_main_panel.boss_panel.SetContent(1 + " / " + 1);
		CleanSceneEnemy();
		HidePanels();
		((GameCgPanelController)cg_panel).HideSkipButton();
		cg_panel.Show();
        Screen.lockCursor = false;
    }

	public override void ShowUnlockWeaponPanel()
	{
		Debug.Log("ShowUnlockWeaponPanel weapon:" + unlock_new_weapon_name);
		HidePanels();
		unlock_panel.Show();
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/UIWeapon/" + unlock_new_weapon_name)) as GameObject;
		GameObject gameObject2 = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance) as GameObject;
		ItemRotateScript itemRotateScript = gameObject2.AddComponent<ItemRotateScript>();
		itemRotateScript.enableUpandDown = false;
		GameObject eff_obj_tem = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Accessory[0]) as GameObject;
		((GameUnlockPanelController)unlock_panel).EnableCameraTexture(gameObject2, eff_obj_tem);
		Invoke("GoShopScene", 5f);
	}

	public override void KeyManRebirth()
	{
        GameData.Instance.total_crystal.SetIntVal(
            GameData.Instance.total_crystal.GetIntVal() - cur_rebirth_cost,
            GameDataIntPurpose.Crystal
        );
        GameData.Instance.SaveData();
		if (cur_rebirth_man == player_controller)
		{
			player_controller.Rebirth();
		}
		else
		{
			player_controller.Recover(player_controller.avatar_data.hp_capacity);
		}
		OnKeyManRebirth();
		Hashtable hashtable = new Hashtable();
		hashtable.Add("tCrystalNum", cur_rebirth_cost);
		GameData.Instance.UploadStatistics("tCrystal_Revive", hashtable);
	}

	public override void GoShopScene()
	{
		OpenClikPlugin.Hide();
		GameData.Instance.loading_to_scene = "UIShop";
		Application.LoadLevel("Loading");
	}

	private void ShowMissionLabel()
	{
		game_main_panel.ShowMissionDayLabel("COOP MISSION");
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
		Debug.Log("OnConnectionLost");
		TNetConnection.UnregisterSceneCallbacks();
		TNetConnection.Disconnect();
		GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, game_main_panel.transform.parent.gameObject, "Unable to connect to the server! Please try again later.", OnOnConnectionLostButton, null);
		Time.timeScale = 0f;
	}

	private void OnReverseHearWaiting(TNetEventData evt)
	{
		wait_server_tip.gameObject.SetActive(true);
		Debug.LogWarning("OnReverseHearWaiting");
	}

	private void OnReverseHearRenew(TNetEventData evt)
	{
		wait_server_tip.gameObject.SetActive(false);
		Debug.LogWarning("OnReverseHearRenew");
	}

	private void OnReverseHearTimeout(TNetEventData evt)
	{
		wait_server_tip.gameObject.SetActive(false);
		Debug.LogWarning("OnReverseHearTimeout");
		TNetConnection.UnregisterSceneCallbacks();
		TNetConnection.Disconnect();
		GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, game_main_panel.transform.parent.gameObject, "Unable to connect to the server! Please try again later.", OnOnConnectionLostButton, null);
		Time.timeScale = 0f;
	}

	private void OnUserEnterRoom(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		Debug.Log("User: " + tNetUser.Name + " has just joined Room.");
	}

	private void OnUserExitRoom(TNetEventData evt)
	{
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		Debug.Log("User: " + tNetUser.Name + " has just left Room.");
		if (tNetUser.Id == tnetObj.Myself.Id || !Player_Set.ContainsKey(tNetUser))
		{
			return;
		}
		PlayerCoopController playerCoopController = Player_Set[tNetUser] as PlayerCoopController;
		if (ui_teammate_bar_list.Contains(playerCoopController.ui_teammate_bar))
		{
			ui_teammate_bar_list.Remove(playerCoopController.ui_teammate_bar);
		}
		Player_Set.Remove(tNetUser);
		foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
		{
			value.RemoveTargetFromHateSet(playerCoopController);
		}
		playerCoopController.OnUserExit();
		Object.Destroy(playerCoopController.gameObject);
		ResetTeammateBarPos();
	}

	private void OnDestroyRoomRes(TNetEventData evt)
	{
		if ((int)evt.data["result"] != 0)
		{
		}
	}

	private void OnDestroyRoom(TNetEventData evt)
	{
	}

	private void OnRoomVarsUpdate(TNetEventData evt)
	{
		Debug.Log("On room variable update...");
		TNetRoomVarType tNetRoomVarType = (TNetRoomVarType)(int)evt.data["key"];
	}

	private void OnLockSth(TNetEventData evt)
	{
		RoomLockResCmd.Result result = (RoomLockResCmd.Result)(int)evt.data["result"];
		string text = (string)evt.data["key"];
		if (result != 0)
		{
		}
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
			}
		}
	}

	private void OnUserVarsUpdate(TNetEventData evt)
	{
		TNetUserVarType tNetUserVarType = (TNetUserVarType)(int)evt.data["key"];
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		if (tNetUser.Id != tnetObj.Myself.Id)
		{
			if (!Player_Set.ContainsKey(tNetUser) && tNetUserVarType == TNetUserVarType.AvatarData)
			{
				OnTNetPlayerBirth(tNetUser);
			}
			else if (tNetUserVarType == TNetUserVarType.PlayerMoveState && Player_Set.ContainsKey(tNetUser))
			{
				Player_Set[tNetUser].SetMoveStateType((PlayerStateType)tNetUser.GetVariable(TNetUserVarType.PlayerMoveState).GetShort("data"));
			}
			else if (tNetUserVarType == TNetUserVarType.PlayerFireState && Player_Set.ContainsKey(tNetUser))
			{
				Player_Set[tNetUser].SetFireStateType((PlayerStateType)tNetUser.GetVariable(TNetUserVarType.PlayerFireState).GetShort("data"));
			}
		}
	}

	private void OnObjectMessage(TNetEventData evt)
	{
		SFSObject sFSObject = (SFSObject)evt.data["message"];
		TNetUser tNetUser = (TNetUser)evt.data["user"];
		if ((!Player_Set.ContainsKey(tNetUser) && tNetUser.Id != tnetObj.Myself.Id && !OnTNetPlayerBirth(tNetUser)) || tNetUser == tnetObj.Myself)
		{
			return;
		}
		if (sFSObject.ContainsKey("trans"))
		{
			SFSArray data = sFSObject.GetSFSArray("trans") as SFSArray;
			if (tNetUser != null && tNetUser.Id != tnetObj.Myself.Id)
			{
				NetworkTransform net_trans = NetworkTransform.FromSFSArray(data);
				Player_Set[tNetUser].UpdateNetworkTrans(net_trans);
			}
			return;
		}
        if (sFSObject.ContainsKey("spawnEnemy"))
        {
            SFSArray sFSArray = sFSObject.GetSFSArray("spawnEnemy") as SFSArray;
            EnemyType enemyType = (EnemyType)sFSArray.GetShort(0);
            int enemyId = sFSArray.GetShort(1);
            Vector3 position = new Vector3(sFSArray.GetFloat(2), sFSArray.GetFloat(3), sFSArray.GetFloat(4));

            EnemyFactory.CreateRemoteEnemy(enemyType, position, Quaternion.identity, enemyId, false);

            Debug.Log(string.Concat("CreateRemoteEnemy:", enemyType, " index:", enemyId, " pos:", position, " time:", Time.time));
            return;
        }
        if (sFSObject.ContainsKey("playerInjured"))
		{
			SFSArray sFSArray2 = sFSObject.GetSFSArray("playerInjured") as SFSArray;
			float @float = sFSArray2.GetFloat(0);
			float float2 = sFSArray2.GetFloat(1);
			if (Player_Set.ContainsKey(tNetUser))
			{
				((PlayerCoopController)Player_Set[tNetUser]).OnRemoteInjured(@float, float2);
			}
			return;
		}
		if (sFSObject.ContainsKey("playerComboBuff"))
		{
			int short3 = sFSObject.GetShort("playerComboBuff");
			if (Player_Set.ContainsKey(tNetUser))
			{
				((PlayerCoopController)Player_Set[tNetUser]).OnRemoteComboBuff(short3);
			}
			return;
		}
		if (sFSObject.ContainsKey("spawnBoss"))
		{
			SFSArray sFSArray3 = sFSObject.GetSFSArray("spawnBoss") as SFSArray;
			EnemyType short4 = (EnemyType)sFSArray3.GetShort(0);
			int short5 = sFSArray3.GetShort(1);
			Vector3 vector2 = new Vector3(sFSArray3.GetFloat(2), sFSArray3.GetFloat(3), sFSArray3.GetFloat(4));
			Vector3 euler = new Vector3(sFSArray3.GetFloat(5), sFSArray3.GetFloat(6), sFSArray3.GetFloat(7));
			EnemyController enemyController = EnemyFactory.CreateRemoteEnemy(short4, vector2, Quaternion.Euler(euler), short5, true);
			Debug.Log(string.Concat("CreateRemoteBoss:", short4, " index:", short5, " pos:", vector2, " time:", Time.time));
			CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
			enemyController.SetEnemyBeCoopBoss(coopBossCfg);
			Debug.Log("boss_cfg:" + coopBossCfg.ToString());
			((BossCoopMissionController)mission_controller).boss = enemyController;
			OnBossBirthCameraShow(enemyController);
			return;
		}
		if (sFSObject.ContainsKey("skill"))
		{
			string utfString = sFSObject.GetUtfString("skill");
			if (Player_Set.ContainsKey(tNetUser))
			{
				Player_Set[tNetUser].ConjureSkill(utfString);
			}
			return;
		}
		if (sFSObject.ContainsKey("skillEvent"))
		{
			string utfString2 = sFSObject.GetUtfString("skillEvent");
			Debug.Log("skillEvent:" + utfString2);
			if (utfString2 == "ScarecrowTriger" && Player_Set.ContainsKey(tNetUser))
			{
				CowboyCoopController cowboyCoopController = Player_Set[tNetUser] as CowboyCoopController;
				if (cowboyCoopController != null)
				{
					cowboyCoopController.OnRemoteScarecrowTriger();
				}
			}
			return;
		}
		if (sFSObject.ContainsKey("WoodBoxId"))
		{
			SFSArray sFSArray4 = sFSObject.GetSFSArray("WoodBoxId") as SFSArray;
			int short6 = sFSArray4.GetShort(0);
			Vector3 vector3 = new Vector3(sFSArray4.GetFloat(1), sFSArray4.GetFloat(2), sFSArray4.GetFloat(3));
			Debug.Log("wood box id:" + short6 + " pos:" + vector3);
			{
				foreach (GameObject item2 in GameSceneController.Instance.wood_box_list)
				{
					WoodBoxController component = item2.GetComponent<WoodBoxController>();
					if (component != null && (component.transform.position - vector3).magnitude <= 0.5f)
					{
						component.coop_id = short6;
						woodbox_set.Add(short6, component);
						base.WoodBoxIndex = short6 + 1;
						break;
					}
				}
				return;
			}
		}
		if (sFSObject.ContainsKey("WoodBoxDestory"))
		{
			int short7 = sFSObject.GetShort("WoodBoxDestory");
			Debug.Log("wood box id:" + short7 + " destory");
			if (woodbox_set.ContainsKey(short7))
			{
				woodbox_set[short7].OnRemoteDead();
			}
		}
		else if (sFSObject.ContainsKey("GameItemId"))
		{
			SFSArray sFSArray5 = sFSObject.GetSFSArray("GameItemId") as SFSArray;
			int short8 = sFSArray5.GetShort(0);
			ItemType short9 = (ItemType)sFSArray5.GetShort(1);
			Vector3 vector4 = new Vector3(sFSArray5.GetFloat(2), sFSArray5.GetFloat(3), sFSArray5.GetFloat(4));
			Debug.Log(string.Concat("GameItemId:", short8, " pos:", vector4, " type:", short9));
			GameObject gameObject = Object.Instantiate(game_item_ref_list[(int)short9], vector4, Quaternion.identity) as GameObject;
			GameItemController component2 = gameObject.GetComponent<GameItemController>();
			component2.coop_id = short8;
			game_item_set.Add(short8, component2);
			base.GameItemIndex = short8 + 1;
		}
		else if (sFSObject.ContainsKey("ItemGet"))
		{
			SFSArray sFSArray6 = sFSObject.GetSFSArray("ItemGet") as SFSArray;
			int short10 = sFSArray6.GetShort(0);
			ItemType short11 = (ItemType)sFSArray6.GetShort(1);
			Debug.Log("ItemGet:" + short10 + " type:" + short11);
			if (game_item_set.ContainsKey(short10))
			{
				game_item_set[short10].OnRemoteGet((PlayerCoopController)Player_Set[tNetUser]);
			}
		}
		else if (sFSObject.ContainsKey("rpgFire"))
		{
			SFSArray sFSArray7 = sFSObject.GetSFSArray("rpgFire") as SFSArray;
			Vector3 target = new Vector3(sFSArray7.GetFloat(0), sFSArray7.GetFloat(1), sFSArray7.GetFloat(2));
			RPGCoopController rPGCoopController = Player_Set[tNetUser].CurWeapon as RPGCoopController;
			if (rPGCoopController != null)
			{
				rPGCoopController.OnRemoteFire(Player_Set[tNetUser], target);
			}
		}
		else if (sFSObject.ContainsKey("ionFire"))
		{
			SFSArray sFSArray8 = sFSObject.GetSFSArray("ionFire") as SFSArray;
			Vector3 target2 = new Vector3(sFSArray8.GetFloat(0), sFSArray8.GetFloat(1), sFSArray8.GetFloat(2));
			IonCannonCoopController ionCannonCoopController = Player_Set[tNetUser].CurWeapon as IonCannonCoopController;
			if (ionCannonCoopController != null)
			{
				ionCannonCoopController.OnRemoteFire(Player_Set[tNetUser], target2);
			}
		}
		else if (sFSObject.ContainsKey("pgmFire"))
		{
			SFSArray sFSArray9 = sFSObject.GetSFSArray("pgmFire") as SFSArray;
			List<Vector3> list = new List<Vector3>();
			int num = sFSArray9.Size();
			for (int i = 0; i < num; i++)
			{
				SFSArray sFSArray10 = sFSArray9.GetSFSArray(i) as SFSArray;
				Vector3 item = new Vector3(sFSArray10.GetFloat(0), sFSArray10.GetFloat(1), sFSArray10.GetFloat(2));
				list.Add(item);
			}
			PGMCoopController pGMCoopController = Player_Set[tNetUser].CurWeapon as PGMCoopController;
			if (pGMCoopController != null)
			{
				pGMCoopController.OnRemoteFire(Player_Set[tNetUser], list);
			}
		}
		else if (sFSObject.ContainsKey("enemyChangeTar"))
		{
			SFSArray sFSArray11 = sFSObject.GetSFSArray("enemyChangeTar") as SFSArray;
			int short12 = sFSArray11.GetShort(0);
			ObjectController.ControllerType short13 = (ObjectController.ControllerType)sFSArray11.GetShort(1);
			int @int = sFSArray11.GetInt(2);
			StartCoroutine(EnemySetTargetPlayerRemote(short12, short13, @int));
		}
		else if (sFSObject.ContainsKey("enemyInjured"))
		{
			SFSArray sFSArray12 = sFSObject.GetSFSArray("enemyInjured") as SFSArray;
			int short14 = sFSArray12.GetShort(0);
			float float3 = sFSArray12.GetFloat(1);
			bool @bool = sFSArray12.GetBool(2);
			float float4 = sFSArray12.GetFloat(3);
			PlayerController playerController = Player_Set[tNetUser];
			if (Enemy_Set.ContainsKey(short14))
			{
				Enemy_Set[short14].OnHitRemote(float3, playerController, @bool, float4);
				Dictionary<PlayerID, float> player_damage_Set;
				Dictionary<PlayerID, float> dictionary = (player_damage_Set = Player_damage_Set);
				PlayerID player_id;
				PlayerID key = (player_id = playerController.player_id);
				float num2 = player_damage_Set[player_id];
				dictionary[key] = num2 + float3;
			}
			else if (Enemy_Enchant_Set.ContainsKey(short14))
			{
				Enemy_Enchant_Set[short14].OnHitRemote(float3, playerController, @bool, float4);
			}
		}
		else if (sFSObject.ContainsKey("enemyDead"))
		{
			SFSArray sFSArray13 = sFSObject.GetSFSArray("enemyDead") as SFSArray;
			int short15 = sFSArray13.GetShort(0);
			float float5 = sFSArray13.GetFloat(1);
			PlayerController player = Player_Set[tNetUser];
			if (Enemy_Set.ContainsKey(short15))
			{
				Enemy_Set[short15].OnDeadRemote(float5, player);
			}
			else if (Enemy_Enchant_Set.ContainsKey(short15))
			{
				Enemy_Enchant_Set[short15].OnDeadRemote(float5, player);
			}
		}
		else if (sFSObject.ContainsKey("MissionWin"))
		{
			OnRemoteMissionWin();
		}
		else if (sFSObject.ContainsKey("PlayerCoopDead"))
		{
			if (Player_Set.ContainsKey(tNetUser))
			{
				Debug.Log("PlayerCoopDead sender:" + tNetUser.Id);
				Player_Set[tNetUser].is_coop_dead = true;
			}
		}
		else
		{
			if (sFSObject.ContainsKey("playerDead"))
			{
				if (!Player_Set.ContainsKey(tNetUser))
				{
					return;
				}
				Debug.Log("playerDead sender:" + tNetUser.Id);
				{
					foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
					{
						value.RemoveTargetFromHateSet(Player_Set[tNetUser]);
					}
					return;
				}
			}
			if (sFSObject.ContainsKey("playerRecover"))
			{
				SFSArray sFSArray14 = sFSObject.GetSFSArray("playerRecover") as SFSArray;
				float float6 = sFSArray14.GetFloat(0);
				float float7 = sFSArray14.GetFloat(1);
				if (Player_Set.ContainsKey(tNetUser))
				{
					((PlayerCoopController)Player_Set[tNetUser]).OnRemoteRecover(float6, float7);
				}
			}
			else if (sFSObject.ContainsKey("playerRebirth"))
			{
				SFSArray sFSArray15 = sFSObject.GetSFSArray("playerRebirth") as SFSArray;
				float float8 = sFSArray15.GetFloat(0);
				float float9 = sFSArray15.GetFloat(1);
				if (Player_Set.ContainsKey(tNetUser))
				{
					((PlayerCoopController)Player_Set[tNetUser]).OnRemoteRebirth(float8, float9);
				}
			}
			else if (sFSObject.ContainsKey("MissionFailed"))
			{
				OnRemoteMissionFailed();
			}
			else if (sFSObject.ContainsKey("askEnmeyEnchant"))
			{
				if (TNetConnection.IsServer)
				{
					int short16 = sFSObject.GetShort("askEnmeyEnchant");
					if (Enemy_Set.ContainsKey(short16))
					{
						Enemy_Set[short16].EnchantMonster();
					}
				}
			}
			else if (sFSObject.ContainsKey("EnmeyEnchant"))
			{
				int short17 = sFSObject.GetShort("EnmeyEnchant");
				if (Enemy_Set.ContainsKey(short17))
				{
					Debug.Log("Get EnmeyEnchant msg.");
					Enemy_Set[short17].RemoteEnchantMonster();
				}
			}
		}
	}

    void OnEnable()
    {
        GameSceneController.Instance.canPressEscape = true;
    }

    private void OnOnConnectionLostButton()
	{
		Debug.LogWarning("OnOnConnectionLostButton");
		Time.timeScale = 1f;
		QuitGameForDisconnect(0.2f);
	}

	private bool OnTNetPlayerBirth(TNetUser user)
	{
		if (!user.ContainsVariable(TNetUserVarType.AvatarData))
		{
			return false;
		}
		Debug.Log("OnSFSPlayerBirth name:" + user.ToString());
		SFSObject variable = user.GetVariable(TNetUserVarType.AvatarData);
		string utfString = variable.GetUtfString("nickName");
		AvatarType @short = (AvatarType)variable.GetShort("avatarType");
		int short2 = variable.GetShort("hpLv");
		int short3 = variable.GetShort("armorLv");
		int short4 = variable.GetShort("dmgLv");
		int short5 = variable.GetShort("avatarLv");
		string utfString2 = variable.GetUtfString("weapon");
		string utfString3 = variable.GetUtfString("skill0");
		string utfString4 = variable.GetUtfString("skill1");
		Debug.Log("nickName:" + utfString);
		Debug.Log("avatarType:" + @short);
		Debug.Log("hpLv:" + short2);
		Debug.Log("armorLv:" + short3);
		Debug.Log("dmgLv:" + short4);
		Debug.Log("weapon:" + utfString2);
		Debug.Log("skill0:" + utfString3);
		Debug.Log("skill1:" + utfString4);
		AvatarConfig avatarConfig = GameConfig.Instance.AvatarConfig_Set[@short];
		AvatarData avatarData = new AvatarData();
		avatarData.show_name = avatarConfig.show_name;
		avatarData.avatar_name = avatarConfig.avatar_name;
		avatarData.avatar_type = avatarConfig.avatar_type;
		avatarData.config = avatarConfig;
		avatarData.level = 1;
		avatarData.cur_exp = new GameDataInt(0);
		avatarData.exist_state = AvatarExistState.Owned;
		avatarData.avatar_state = (AvatarData.AvatarState)short5;
		avatarData.hp_level = short2;
		avatarData.armor_level = short3;
		avatarData.damage_level = short4;
		avatarData.primary_equipment = utfString2;
		avatarData.skill_list.Add(utfString3);
		avatarData.skill_list.Add(utfString4);
		PlayerCoopController playerCoopController = AvatarFactory.CreateAvatarCoop(avatarData);
		playerCoopController.tnet_user = user;
		playerCoopController.player_id = new PlayerID(avatarData.avatar_type, avatarData.avatar_state, utfString, user.Id);
		playerCoopController.InitSkill();
		Player_Set.Add(user, playerCoopController);
		Player_damage_Set.Add(playerCoopController.player_id, 0f);
		StartCoroutine(CreateTeammateBar(playerCoopController, @short, utfString, (AvatarData.AvatarState)short5));
		return true;
	}

	private IEnumerator CreateTeammateBar(PlayerCoopController player_coop, AvatarType avatarType, string nickName, AvatarData.AvatarState avatar_state)
	{
		while (game_main_panel == null)
		{
			yield return 1;
		}
		GameObject ui_obj = Object.Instantiate(ui_teammate_bar_ref) as GameObject;
		ui_obj.transform.parent = team_bar_root;
		UITeammateBar teammate_bar = ui_obj.GetComponent<UITeammateBar>();
		teammate_bar.SetHpbarInfo(GameConfig.Instance.AvatarConfig_Set[avatarType].avatar_name + "_0" + (int)avatar_state + "_icon_s", nickName);
		player_coop.ui_teammate_bar = teammate_bar;
		ui_teammate_bar_list.Add(teammate_bar);
		ResetTeammateBarPos();
	}

	private void ResetTeammateBarPos()
	{
		int num = 0;
		foreach (UITeammateBar item in ui_teammate_bar_list)
		{
			item.transform.localPosition = new Vector3(0f, 0f - teammate_bar_space, 0f) * num;
			num++;
		}
	}

	public override void OnEnemySpawn(EnemyData enemy_data)
	{
	}

	public IEnumerator EnemySetTargetPlayerRemote(int enemy_id, ObjectController.ControllerType type, int tar_id)
	{
		yield return 1;
		int step = 0;
		while (!Enemy_Set.ContainsKey(enemy_id) && step < 100)
		{
			step++;
			yield return 1;
		}
		if (Enemy_Set.ContainsKey(enemy_id))
		{
			StartCoroutine(Enemy_Set[enemy_id].SetTargetPlayerRemote(type, tar_id));
		}
		else if (Enemy_Enchant_Set.ContainsKey(enemy_id))
		{
			StartCoroutine(Enemy_Enchant_Set[enemy_id].SetTargetPlayerRemote(type, tar_id));
		}
	}

    private Transform originalCamParent;
    private Vector3 originalCamLocalPos;
    private Quaternion originalCamLocalRot;

    public void OnBossBirthCameraShow(EnemyController boss)
    {
        Transform parent = boss.transform.Find("cg_view");

        // Store original camera transform
        originalCamParent = Camera.main.transform.parent;
        originalCamLocalPos = Camera.main.transform.localPosition;
        originalCamLocalRot = Camera.main.transform.localRotation;

        // Re-parent camera to boss cg_view
        Camera.main.transform.parent = parent;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;

        main_camera.camera_pause = true;
        main_camera.StartShake(enable_spawn_ani);

        float length = main_camera.shake_obj.GetComponent<Animation>()[enable_spawn_ani].length;

        player_controller.StartLimitMove(length);
        StartCgLimit(length);

        foreach (EnemyController value in Enemy_Set.Values)
        {
            if (!value.IsBoss)
            {
                value.StartLimitMove(length);
            }
        }

        StartCoroutine(RestoreCameraAfterDelay(length));
    }

    private IEnumerator RestoreCameraAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Camera.main.transform.parent = originalCamParent;
        Camera.main.transform.localPosition = originalCamLocalPos;
        Camera.main.transform.localRotation = originalCamLocalRot;

        main_camera.camera_pause = false;
    }

    public bool CheckAllPlayerCoopDead()
	{
		bool result = true;
		foreach (PlayerController value in Player_Set.Values)
		{
			if (!value.is_coop_dead)
			{
				return false;
			}
		}
		return result;
	}

	public void CheckCoopMissionOver()
	{
		if (base.Inited/* && TNetConnection.IsServer*/ && !mission_check_finished && CheckAllPlayerCoopDead())
		{
			//Debug.Log("Coop Mission Over...");
			OnRemoteMissionFailed();
            Screen.lockCursor = false;
            //SFSObject sFSObject = new SFSObject();
            //sFSObject.PutUtfString("MissionFailed", "0");
            //tnetObj.Send(new BroadcastMessageRequest(sFSObject));
        }
	}

    public void QuitGameForDisconnect(float time)
    {
        Time.timeScale = 1f;

        if (tnetObj != null)
        {
            tnetObj.Send(new LeaveRoomRequest());
            TNetConnection.UnregisterSceneCallbacks();
        }

        Invoke("CoopOverToShop", time);
        Screen.lockCursor = false;
    }


    private void DestroyNetConnection()
	{
		TNetConnection.UnregisterSceneCallbacks();
		TNetConnection.Disconnect();
		tnetObj = null;
	}

	private void CoopOverToShop()
	{
		DestroyNetConnection();
		GameData.Instance.loading_to_scene = "UIShop";
		Application.LoadLevel("Loading");
        Screen.lockCursor = false;
    }

	public override void OnBossDeadOver()
	{
		mission_controller.MissionFinished();
	}
}
