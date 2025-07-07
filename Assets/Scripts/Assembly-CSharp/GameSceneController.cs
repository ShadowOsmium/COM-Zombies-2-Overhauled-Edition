using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
	public const string frame_cash = "jinbi";

	public const string frame_voucher = "daibi";

	public const string frame_crystal = "shuijing";

	protected static GameSceneController instance;

    public Transform dropParent;

    private bool isCutsceneActive = false;

    public PlayingState GamePlayingState = PlayingState.Gaming;

	public MissionController mission_controller;

    public Dictionary<GameObject, float> PlayerDamageToBoss = new Dictionary<GameObject, float>();

    public ShooterGameCamera main_camera;

	public PlatformInputController input_controller;

	public PlayerController player_controller;

	public SightBeadScript SightBead;

	public Dictionary<TNetUser, PlayerController> Player_Set = new Dictionary<TNetUser, PlayerController>();

	public Dictionary<int, EnemyController> Enemy_Set = new Dictionary<int, EnemyController>();

	public Dictionary<int, NPCController> NPC_Set = new Dictionary<int, NPCController>();

	public Dictionary<int, GuardianForceController> GuardianForce_Set = new Dictionary<int, GuardianForceController>();

	public Dictionary<int, EnemyController> Enemy_Enchant_Set = new Dictionary<int, EnemyController>();

	public Dictionary<EnemyType, int> enemy_death_info_set = new Dictionary<EnemyType, int>();

    public Dictionary<string, float> boss_damage_record = new Dictionary<string, float>();

    public EnemyMap enemy_ref_map;

	public List<GameObject> wood_box_list;

	public GameObject[] way_points;

	public Dictionary<PlayerID, float> Player_damage_Set = new Dictionary<PlayerID, float>();

	public List<MineProjectile> mine_area = new List<MineProjectile>();

	public bool is_boss_dead;

	protected bool is_inited;

	protected int woodbox_index;

	protected int game_item_index;

	protected int enenmy_index;

	protected int npc_index;

	protected int guardian_index;

	public ScreenBloodController screen_blood;

	public ScreenBloodController screen_blood_shield;

	public ScreenBloodController boss_screen_blood1;

	public GameMainPanelController game_main_panel;

	public UIPanelController game_reward_panel;

	public UIPanelController game_failed_panel;

	public UIPanelController game_pause_panel;

	public UIPanelController cg_panel;

	public UIPanelController unlock_panel;

	public GameTutotialPanelController tutorial_panel;

	public GameObject pause_button_obj;

	protected List<UIPanelController> panel_set = new List<UIPanelController>();

	protected bool mission_controller_finished;

	protected bool mission_check_finished;

	protected float last_check_mission_finished;

	protected float check_mission_rate = 1f;

	protected UIPanelController cur_game_info_panel;

	public GameObject boom_l_pool;

	public GameObject boom_m_pool;

	public GameObject hp_add_pool;

	public GameObject blood_pool;

	public GameObject fire_line_pool;

	public GameObject level_up_pool;

	public GameObject combo_get_pool;

	public GameObject ground_stone_pool;

	public GameObject assault_cartridge_pool;

	public GameObject shotgun_cartridge_pool;

	public GameObject add_item_pool;

	public GameObject bullet_add_item_pool;

	public GameObject money_add_item_pool;

	public GameObject stone_boom_pool;

	public GameObject stone_boom_g_pool;

	public GameObject hp_add_ring_pool;

	public GameObject boom_s_pool;

	public GameObject boom_blood_pool;

	public GameObject blood_ground;

	public GameObject boom_ion_pool;

	public GameObject combo_get_pool1;

	public GameObject combo_get_pool2;

	public GameObject combo_get_pool3;

	public TUICamera tui_camera;

	public TUIControlImpl Auto_Lock_Rect;

	public TUIControlImpl PGM_Lock_Rect;

	public List<GameObject> Eff_Accessory = new List<GameObject>();

	public MissionType mission_type = MissionType.Cleaner;

	public NearestTargetInfo auto_lock_target;

	protected float cur_auto_lock_time;

	protected float auto_lock_time_interval = 0.5f;

	public GameObject pgm_sight;

    private int delayedCashReward = 0;

    private bool isGrantingDelayedRewards = false;

    public GameObject pgm_sight_pool;

	public float enemy_standard_hp;

	public float enemy_standard_dps;

	public float weapon_standard_dps;

	public float enemy_standard_reward;

	public float enemy_standard_reward_total;

	public bool enable_boss_spawn;

	public string enable_spawn_ani = string.Empty;

	protected bool unlock_new_weapon;

	protected bool unlock_new_avatar;

	protected string unlock_new_weapon_name = string.Empty;

	protected string unlock_new_avatar_name = string.Empty;

	protected bool is_skip_cg;

	public bool tutorial_ui_over;

	public int player_death_count;

	public int cur_rebirth_cost;

	public bool is_logic_paused;

	public ObjectController cur_rebirth_man;

	public MissionDayType mission_day_type;

	protected string music_name = string.Empty;

	public int mission_total_cash;

	public bool is_play_cg;

	public GameObject hp_bar_ref;

	protected RoamOrder roam_order;

    public static HashSet<EnemyType> BossEnemyTypes = new HashSet<EnemyType>
{
    EnemyType.E_FATCOOK,
    EnemyType.E_FATCOOK_E,
    EnemyType.E_HAOKE_A,
    EnemyType.E_HAOKE_B,
    EnemyType.E_WRESTLER,
    EnemyType.E_WRESTLER_E,
    EnemyType.E_HALLOWEEN,
    EnemyType.E_HALLOWEEN_E,
    EnemyType.E_HALLOWEEN_SUB,
    EnemyType.E_HALLOWEEN_SUB_E,
    EnemyType.E_SHARK,
    EnemyType.E_SHARK_E
};


    public static GameSceneController Instance
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

	public int WoodBoxIndex
	{
		get
		{
			return woodbox_index++;
		}
		set
		{
			woodbox_index = value;
		}
	}

	public int GameItemIndex
	{
		get
		{
			return game_item_index++;
		}
		set
		{
			game_item_index = value;
		}
	}

	public int EnemyIndex
	{
		get
		{
			return enenmy_index++;
		}
		set
		{
			enenmy_index = value;
		}
	}

	public int NPCIndex
	{
		get
		{
			return npc_index++;
		}
	}

	public int GuardianIndex
	{
		get
		{
			return guardian_index++;
		}
	}

	public bool IsMissionFinished
	{
		get
		{
			return mission_controller_finished;
		}
	}

	public int DayLevel
	{
		get
		{
			return GameData.Instance.day_level;
		}
	}

    public void UpdateEndlessEnemyStandardStats(int dayLevel)
    {
        float hpA = 0f;
        float hpB = 0f;

        foreach (var key in GameConfig.Instance.Endless_Quest_Hp_Difficulty_Set.Keys)
        {
            if (key <= dayLevel)
            {
                EndlessQuestHpDifficultyInfo info = GameConfig.Instance.Endless_Quest_Hp_Difficulty_Set[key];
                hpA = info.hpParaA;
                hpB = info.hpParaB;
            }
        }

        float dmgA = 0f;
        float dmgB = 0f;

        foreach (var key in GameConfig.Instance.Endless_Quest_Dmg_Difficulty_Set.Keys)
        {
            if (key <= dayLevel)
            {
                EndlessQuestDmgDifficultyInfo info = GameConfig.Instance.Endless_Quest_Dmg_Difficulty_Set[key];
                dmgA = info.dmgParaA;
                dmgB = info.dmgParaB;
            }
        }

        enemy_standard_hp = hpA + dayLevel * hpB;
        enemy_standard_dps = dmgA + dayLevel * dmgB;

        Debug.Log(string.Format("[Endless] Scaled stats for DayLevel {0}: HP={1}, DPS={2}", dayLevel, enemy_standard_hp, enemy_standard_dps));
    }


    public bool IsSkipCg
	{
		get
		{
			return is_skip_cg;
		}
	}

	public int GetCurEnemyIndex()
	{
		return enenmy_index;
	}

	private void Awake()
	{
        LockCursor();
        instance = this;
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
	}

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && GamePlayingState == PlayingState.Gaming)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public virtual void OnSceneColorReset()
	{
		if (!(Application.loadedLevelName != "Depot") || !(Application.loadedLevelName != "Church") || !(Application.loadedLevelName != "GameTutorial") || !(Application.loadedLevelName != "Junkyard"))
		{
			return;
		}
		if (Application.loadedLevelName.StartsWith("COM2_"))
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/ChannelMaterialSet")) as GameObject;
			if (GameConfig.Instance.Channel_Scene_Color_Set.ContainsKey(Application.loadedLevelName))
			{
				foreach (Material item in gameObject.GetComponent<MaterialPrefabReference>().material_set)
				{
					item.SetColor("_Color", GameConfig.Instance.Channel_Scene_Color_Set[Application.loadedLevelName]);
				}
				return;
			}
			{
				foreach (Material item2 in gameObject.GetComponent<MaterialPrefabReference>().material_set)
				{
					item2.SetColor("_Color", Color.white);
				}
				return;
			}
		}
		if (!Application.loadedLevelName.StartsWith("Lab_"))
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/LabMaterialSet")) as GameObject;
		if (GameConfig.Instance.Channel_Scene_Color_Set.ContainsKey(Application.loadedLevelName))
		{
			foreach (Material item3 in gameObject2.GetComponent<MaterialPrefabReference>().material_set)
			{
				item3.SetColor("_Color", GameConfig.Instance.Channel_Scene_Color_Set[Application.loadedLevelName]);
			}
			return;
		}
		foreach (Material item4 in gameObject2.GetComponent<MaterialPrefabReference>().material_set)
		{
			item4.SetColor("_Color", Color.white);
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
        DisableFireStickOnPC();
        input_controller.Motor = player_controller.GetComponent<CharacterMotor>();
		main_camera.player = player_controller.transform;
		way_points = GameObject.FindGameObjectsWithTag("WayPoint");
		GameObject[] tem_array = GameObject.FindGameObjectsWithTag("Wood_Box");
		wood_box_list = new List<GameObject>(tem_array);
		yield return 1;
        is_skip_cg = false;
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    public bool is_game_paused = false;
    public bool canPressEscape;
    private void Update()
	{
		if (Time.deltaTime != 0f && Time.timeScale != 0f && Time.time - last_check_mission_finished >= check_mission_rate)
		{
			last_check_mission_finished = Time.time;
			CheckMissionFinished();
		}
        if (can_buy_ammo && Input.GetKeyDown(KeyCode.R))
        {
            OnAddBulletButton();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && GameSceneController.Instance.canPressEscape)
        {
            Debug.Log("Escape pressed and GameSceneController.canPressEscape = true");
            if (is_game_paused)
            {
                GameSceneController.Instance.OnGameResume();
                is_game_paused = false;
            }
            else
            {
                GameSceneController.Instance.OnGamePause();
                is_game_paused = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed but GameSceneController.canPressEscape = false");
        }
    }

    public void DisableFireStickOnPC()
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
		instance = null;
	}

	public void SetHpBar(float percent)
	{
		game_main_panel.SetHpBar(percent);
	}

	public void SetExpBar(float percent)
	{
		game_main_panel.SetExpBar(percent);
	}

	public void SetComboBar(float percent)
	{
		game_main_panel.SetComboBar(percent);
	}

	public void SetLevelLabel(int level)
	{
		game_main_panel.SetLevelLabel(level);
	}

	public void SetComboLabel(string combo)
	{
		game_main_panel.SetComboLabel(combo);
	}

	public void UpdateBulletLabel(string content)
	{
		game_main_panel.UpdateBulletLabel(content);
	}

	public void UpdateWeaponFrame(string frame, bool status)
	{
		game_main_panel.UpdateWeaponFrame(frame, status);
	}

	public void HidePanels()
	{
		foreach (UIPanelController item in panel_set)
		{
			item.Hide();
		}
	}

	public virtual void InitMissionController()
	{
		game_main_panel.HidePanels();
		switch (GameData.Instance.cur_quest_info.mission_type)
		{
		case MissionType.Cleaner:
			mission_controller = base.gameObject.AddComponent<CleanerMissionController>();
			cur_game_info_panel = game_main_panel.clean_panel;
			game_main_panel.EnableNpcHpBar(false);
			break;
		case MissionType.Time_ALive:
			mission_controller = base.gameObject.AddComponent<TimeAliveMissionController>();
			cur_game_info_panel = game_main_panel.time_alive_panel;
			game_main_panel.EnableNpcHpBar(false);
			break;
        case MissionType.Endless:
			mission_controller = base.gameObject.AddComponent<EndlessMissionController>();
            cur_game_info_panel = game_main_panel.time_alive_panel;
			game_main_panel.EnableNpcHpBar(false);
			break;
		case MissionType.Npc_Resources:
			mission_controller = base.gameObject.AddComponent<NPCResourcesMissionController>();
			cur_game_info_panel = game_main_panel.npc_res_panel;
			GameData.Instance.cur_quest_info.camera_roam_enable = true;
			game_main_panel.EnableNpcHpBar(true);
			break;
		case MissionType.Npc_Convoy:
			mission_controller = base.gameObject.AddComponent<NPCConvoyMissionController>();
			cur_game_info_panel = game_main_panel.npc_convoy_panel;
			game_main_panel.EnableNpcHpBar(true);
			break;
		case MissionType.Boss:
			mission_controller = base.gameObject.AddComponent<BossMissionController>();
			cur_game_info_panel = game_main_panel.boss_panel;
			GameData.Instance.cur_quest_info.camera_roam_enable = true;
			game_main_panel.EnableNpcHpBar(false);
			break;
		case MissionType.Tutorial:
			mission_controller = base.gameObject.AddComponent<TutorialMissionController>();
			game_main_panel.EnableNpcHpBar(false);
			break;
		}
        List<EnemyType> missionEnemyTypeList = mission_controller.GetMissionEnemyTypeList();

        if (missionEnemyTypeList == null)
        {
            Debug.LogError("[InitMissionController] missionEnemyTypeList is null. Assigning empty list.");
            missionEnemyTypeList = new List<EnemyType>();
        }

        MissionType missionType = GameData.Instance.cur_quest_info.mission_type;
        enemy_ref_map.ResetEnemyMapInfo(missionEnemyTypeList);

        ResetMissionDifficulty();
		CheckMissionDayType();
		if (GameData.Instance.cur_quest_info.camera_roam_enable)
		{
			GamePlayingState = PlayingState.CG;
			StartCameraRoam();
			return;
		}
		GamePlayingState = PlayingState.Gaming;
		if (cur_game_info_panel != null)
		{
			cur_game_info_panel.Show();
		}
		Invoke("ShowMissionLabel", 0.5f);
	}

	public void CheckMineArea()
	{
	}

	public static bool CheckBlockBetween(Vector3 pos1, Vector3 pos2)
	{
		return Physics.Linecast(pos1, pos2, (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.DYNAMIC_SCENE) | (1 << PhysicsLayer.ANIMATION_SCENE) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD));
	}

	public void MissionTimeOver()
	{
		Debug.Log("Mission Time Over.");
	}

	public virtual void OnCancelRebirth()
	{
		MissionFinished();
	}

    public virtual void MissionFinished()
    {
        Debug.Log("Mission Finished:" + GamePlayingState);
        OpenClikPlugin.Show(false);
        HidePanels();

        TAudioController tAudioController = main_camera.gameObject.AddComponent<TAudioController>();
        tAudioController.StopAudio(music_name);

        if (GamePlayingState == PlayingState.Lose)
        {
            game_failed_panel.Show();
            music_name = "MusicLose";
            if (GameData.Instance.cur_quest_info.mission_type != MissionType.Tutorial)
            {
                Invoke("GoShopScene", 5f);
            }
        }
        else if (GamePlayingState == PlayingState.Win)
        {
            game_reward_panel.Show();
            music_name = "MusicWin";
        }
        tAudioController.PlayAudio(music_name);

        MissionStatistics();

        if (GamePlayingState == PlayingState.Win && (mission_day_type == MissionDayType.Main || mission_day_type == MissionDayType.Side))
        {
            GameData.Instance.day_level++;
        }

        if (GameData.Instance != null && GameData.Instance.blackname)
        {
            delayedCashReward += mission_total_cash;

            mission_total_cash = 0;

            if (!isGrantingDelayedRewards)
            {
                StartCoroutine(GrantDelayedRewardsCoroutine());
            }
        }
        else
        {
            GameData.Instance.total_cash.SetIntVal(
                GameData.Instance.total_cash.GetIntVal() + mission_total_cash,
                GameDataIntPurpose.Cash
            );
            mission_total_cash = 0;
        }

        GameData.Instance.SaveData();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator GrantDelayedRewardsCoroutine()
    {
        isGrantingDelayedRewards = true;

        yield return new WaitForSeconds(600f);

        Debug.Log("[GameSceneController] Granting delayed cash reward: " + delayedCashReward);

        GameData.Instance.total_cash.SetIntVal(
            GameData.Instance.total_cash.GetIntVal() + delayedCashReward,
            GameDataIntPurpose.Cash
        );

        delayedCashReward = 0;
        isGrantingDelayedRewards = false;
    }

    private void MissionStatistics()
	{
		Hashtable hashtable = new Hashtable();
		int num = 0;
		if (GamePlayingState == PlayingState.Lose)
		{
			num = 0;
		}
		else if (GamePlayingState == PlayingState.Win)
		{
			num = 1;
		}
		if (mission_day_type == MissionDayType.Side)
		{
			hashtable.Add("Level", GameData.Instance.day_level);
			hashtable.Add("Result", num);
			hashtable.Add("Income", mission_total_cash);
			GameData.Instance.UploadStatistics("Side_Day", hashtable);
		}
		else if (mission_day_type == MissionDayType.Main)
		{
			hashtable.Add("Level", GameData.Instance.day_level);
			hashtable.Add("Result", num);
			hashtable.Add("Income", mission_total_cash);
			GameData.Instance.UploadStatistics("Main_Day", hashtable);
		}
		else if (mission_day_type == MissionDayType.Daily)
		{
			hashtable.Add("Level", GameData.Instance.day_level);
			hashtable.Add("Result", num);
			hashtable.Add("Income", mission_total_cash);
			GameData.Instance.UploadStatistics("Daily_Day", hashtable);
		}
        Hashtable hashtable2 = new Hashtable();
		switch (GameData.Instance.cur_quest_info.mission_type)
		{
		case MissionType.Cleaner:
			hashtable2.Add("Time", (int)mission_controller.mission_total_time);
			GameData.Instance.UploadStatistics("Mission_Clear", hashtable2);
			break;
		case MissionType.Time_ALive:
			hashtable2.Add("Time", (int)mission_controller.mission_total_time);
			GameData.Instance.UploadStatistics("Mission_ALive", hashtable2);
			break;
		case MissionType.Npc_Resources:
			hashtable2.Add("Time", (int)mission_controller.mission_total_time);
			GameData.Instance.UploadStatistics("Mission_Resource", hashtable2);
			break;
		case MissionType.Npc_Convoy:
			hashtable2.Add("Time", (int)mission_controller.mission_total_time);
			GameData.Instance.UploadStatistics("Mission_Rescue", hashtable2);
			break;
		case MissionType.Boss:
			hashtable2.Add("Time", (int)mission_controller.mission_total_time);
			GameData.Instance.UploadStatistics("Mission_Boss", hashtable2);
			break;
		}
    }

	public void MissionControllerFinished()
	{
		if (!mission_controller_finished)
		{
			Debug.Log("Mission Controller Finished.");
			mission_controller_finished = true;
        }
	}

	public virtual void OnKeyManDead(ObjectController object_controller)
	{
		player_death_count++;
		cur_rebirth_man = object_controller;
		ChangeCameraFocus(object_controller.transform);
		mission_controller.SetMissionPaused(true);
		MissionFailed();
	}

	public virtual void MissionFailed()
	{
		is_logic_paused = true;
		mission_check_finished = true;
		Invoke("SetLoseState", 1f);
	}

	public virtual void MissionWin()
	{
		mission_check_finished = true;
		MissionReward();
		Invoke("SetWinState", 1f);
		Invoke("MissionFinished", 4f);
	}

	public virtual void SetWinState()
	{
		GamePlayingState = PlayingState.Win;
		CleanSceneEnemy();
	}

    public virtual void SetLoseState()
    {
        GamePlayingState = PlayingState.Lose;

        cur_rebirth_cost = 5 * player_death_count;

        if (cur_rebirth_cost > 25)
            cur_rebirth_cost = 25;

        if (GameData.Instance.cur_quest_info.mission_type != MissionType.Tutorial &&
            GameData.Instance.total_crystal.GetIntVal() >= cur_rebirth_cost)
        {
            game_main_panel.rebirth_panel.Show();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Invoke("MissionFinished", 4f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public virtual void MissionReward()
	{
		List<GameReward> list = new List<GameReward>();
		GameReward gameReward = null;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		num = GameData.Instance.GetMissionRewardCash(GetComponent<MissionController>().mission_type, GameData.Instance.cur_quest_info.mission_day_type);
        if (num > 0)
        {
            GameData.Instance.total_cash.SetIntVal(
                GameData.Instance.total_cash.GetIntVal() + num,
                GameDataIntPurpose.Cash
            );
            gameReward = new GameReward(GameReward.GameRewardType.CASH, "jinbi", num);
            list.Add(gameReward);
            Debug.Log("Mission reward cash:" + num);
        }

        num2 = GameData.Instance.GetMissionRewardCrystal(
            GetComponent<MissionController>().mission_type,
            GameData.Instance.cur_quest_info.mission_day_type
        );
        if (num2 > 0)
        {
            GameData.Instance.total_crystal.SetIntVal(
                GameData.Instance.total_crystal.GetIntVal() + num2,
                GameDataIntPurpose.Crystal
            );
            gameReward = new GameReward(GameReward.GameRewardType.CRYSTAL, "shuijing", num2);
            list.Add(gameReward);
            Debug.Log("Mission reward crystal:" + num2);
        }

        num3 = GameData.Instance.GetMissionRewardVoucher(
            GetComponent<MissionController>().mission_type,
            GameData.Instance.cur_quest_info.mission_day_type
        );
        if (num3 > 0)
        {
            GameData.Instance.total_voucher.SetIntVal(
                GameData.Instance.total_voucher.GetIntVal() + num3,
                GameDataIntPurpose.Voucher
            );
            gameReward = new GameReward(GameReward.GameRewardType.VOUCHER, "daibi", num3);
            list.Add(gameReward);
            Debug.Log("Mission reward voucher:" + num3);
        }
        if (mission_day_type != MissionDayType.Daily)
		{
			if (mission_day_type == MissionDayType.Tutorial)
			{
				GameData.Instance.is_enter_tutorial = false;
			}
			else if (mission_day_type == MissionDayType.Main)
			{
				string empty = string.Empty;
				if (GameConfig.Instance.Main_Quest_Order.ContainsKey(GameData.Instance.day_level) && GameConfig.Instance.Main_Quest_Order[GameData.Instance.day_level].reward_weapon != string.Empty)
				{
					string reward_weapon = GameConfig.Instance.Main_Quest_Order[GameData.Instance.day_level].reward_weapon;
					Debug.Log("unlock weapon:" + reward_weapon);
					if (GameData.Instance.WeaponData_Set[reward_weapon].Unlock())
					{
						Debug.Log("weapon:" + reward_weapon + " is unlocked, buy in the shop.");
						unlock_new_weapon_name = reward_weapon;
						unlock_new_weapon = true;
						UnlockInGame unlockInGame = new UnlockInGame();
						unlockInGame.Type = UnlockInGame.UnlockType.Weapon;
						unlockInGame.Name = reward_weapon;
						GameData.Instance.UnlockList.Add(unlockInGame);
					}
				}
				if (GameConfig.Instance.Main_Quest_Order.ContainsKey(GameData.Instance.day_level) && GameConfig.Instance.Main_Quest_Order[GameData.Instance.day_level].avatar != AvatarType.None)
				{
					AvatarType avatar = GameConfig.Instance.Main_Quest_Order[GameData.Instance.day_level].avatar;
					if (GameData.Instance.AvatarData_Set[avatar].Unlock())
					{
						Debug.Log(string.Concat(avatar, " unlocked."));
						empty = empty + " " + avatar;
						unlock_new_avatar_name = GameData.Instance.AvatarData_Set[avatar].avatar_name;
						unlock_new_avatar = true;
						gameReward = new GameReward(GameReward.GameRewardType.AVATAR, avatar.ToString() + "_01_icon", 1);
						list.Add(gameReward);
						UnlockInGame unlockInGame2 = new UnlockInGame();
						unlockInGame2.Type = UnlockInGame.UnlockType.Avatar;
						unlockInGame2.Name = GameData.Instance.AvatarData_Set[avatar].avatar_name;
						GameData.Instance.UnlockList.Add(unlockInGame2);
					}
				}
			}
			else if (mission_day_type != MissionDayType.Side)
			{
			}
		}
		mission_total_cash += num;
		((GameRewardPanelController)game_reward_panel).ResetGameReward(list);
	}

	public virtual void CheckMissionFinished()
	{
		if (!mission_controller_finished || mission_check_finished || GamePlayingState != PlayingState.Gaming)
		{
			return;
		}
		if (mission_controller.mission_type == MissionType.Cleaner || mission_controller.mission_type == MissionType.Boss || mission_controller.mission_type == MissionType.Tutorial)
		{
			if (Enemy_Set.Count == 0)
			{
				MissionWin();
			}
		}
		else if (mission_controller.mission_type == MissionType.Time_ALive)
		{
			MissionWin();
		}
        else if (mission_controller.mission_type == MissionType.Npc_Resources)
		{
			NPCResourcesMissionController nPCResourcesMissionController = mission_controller as NPCResourcesMissionController;
			if (nPCResourcesMissionController.cur_res_count >= nPCResourcesMissionController.target_res_count)
			{
				MissionWin();
			}
			else
			{
				MissionFailed();
			}
		}
		else if (mission_controller.mission_type == MissionType.Npc_Convoy)
		{
			NPCConvoyMissionController nPCConvoyMissionController = mission_controller as NPCConvoyMissionController;
			if (nPCConvoyMissionController.npc_follower.arrived_home)
			{
				MissionWin();
			}
			else
			{
				MissionFailed();
			}
		}
	}

	public void UpdateEnemyDeathInfo(EnemyType type, int count)
	{
		if (enemy_death_info_set.ContainsKey(type))
		{
			Dictionary<EnemyType, int> dictionary;
			Dictionary<EnemyType, int> dictionary2 = (dictionary = enemy_death_info_set);
			EnemyType key;
			EnemyType key2 = (key = type);
			int num = dictionary[key];
			dictionary2[key2] = num + count;
		}
		else
		{
			enemy_death_info_set[type] = count;
		}
	}

	public void LootCash(int cash)
	{
		mission_total_cash += cash;
        GameData.Instance.total_cash.SetIntVal(
            GameData.Instance.total_cash.GetIntVal() + cash,
            GameDataIntPurpose.Cash
        );
    }

	public Vector3 GetSightScreenPos()
	{
		return tui_camera.GetComponent<Camera>().WorldToScreenPoint(SightBead.transform.position);
	}

	public void SetSightBeadMaxStretch(float stretch)
	{
		SightBead.stretch_range = stretch;
	}

    public void DoAutoLock(float deltaTime, WeaponController weapon)
    {
        cur_auto_lock_time += deltaTime;

        // Handle PGM hiding sight bead
        if (weapon.weapon_type == WeaponType.PGM)
        {
            if (SightBead.gameObject.activeInHierarchy)
            {
                SightBead.gameObject.SetActive(false);
            }
        }
        else if (!SightBead.gameObject.activeInHierarchy)
        {
            SightBead.gameObject.SetActive(true);
        }

        // Auto-lock disabled
        if (!weapon.weapon_data.config.is_auto_lock)
        {
            SightBead.transform.localPosition = Vector3.zero;
            SightBead.SetUnLockColor();
            return;
        }

        // Clean up dead/invalid lock target
        if (auto_lock_target != null)
        {
            if (auto_lock_target.target_obj == null ||
                (auto_lock_target.type == NearestTargetInfo.NearestTargetType.Enemy &&
                ((EnemyController)auto_lock_target.target_obj).Enemy_State.GetStateType() == EnemyStateType.Dead))
            {
                auto_lock_target = null;
            }
        }

        // Update screen position of current lock target
        if (auto_lock_target != null && auto_lock_target.transform != null)
        {
            Vector2 screenPoint = main_camera.GetComponent<Camera>().WorldToScreenPoint(auto_lock_target.LockPosition);
            Vector2 worldPoint = tui_camera.GetComponent<Camera>().ScreenToWorldPoint(screenPoint);
            auto_lock_target.screenPos = worldPoint;
        }

        // Search for new target if needed
        if (cur_auto_lock_time >= auto_lock_time_interval)
        {
            cur_auto_lock_time = 0f;

            // Clear target if out of view or blocked
            if (auto_lock_target != null && auto_lock_target.transform != null)
            {
                Vector3 screenPos = main_camera.GetComponent<Camera>().WorldToScreenPoint(auto_lock_target.LockPosition);
                Vector3 worldUIPos = tui_camera.GetComponent<Camera>().ScreenToWorldPoint(screenPos);
                if (screenPos.z < 0f ||
                    !Auto_Lock_Rect.PtInControl(new Vector2(worldUIPos.x, worldUIPos.y)) ||
                    GameSceneController.CheckBlockBetween(auto_lock_target.LockPosition, main_camera.transform.position))
                {
                    auto_lock_target = null;
                }
            }

            // Try to find the best new target
            if (auto_lock_target == null)
            {
                float bestDistance = 99999f;

                // Look for enemies
                foreach (EnemyController enemy in Enemy_Set.Values)
                {
                    if (enemy.Enemy_State.GetStateType() == EnemyStateType.Dead)
                        continue;

                    Vector3 enemyPos = enemy.centroid;
                    Vector3 screenPos = main_camera.GetComponent<Camera>().WorldToScreenPoint(enemyPos);
                    if (screenPos.z < 0f)
                        continue;

                    Vector3 worldUIPos = tui_camera.GetComponent<Camera>().ScreenToWorldPoint(screenPos);
                    if (!Auto_Lock_Rect.PtInControl(new Vector2(worldUIPos.x, worldUIPos.y)))
                        continue;

                    if (GameSceneController.CheckBlockBetween(enemyPos, main_camera.transform.position))
                        continue;

                    float distance = (enemyPos - main_camera.transform.position).magnitude;
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        auto_lock_target = new NearestTargetInfo
                        {
                            type = NearestTargetInfo.NearestTargetType.Enemy,
                            transform = enemy.transform,
                            screenPos = worldUIPos,
                            target_obj = enemy
                        };
                    }
                }

                // Look for wood boxes
                foreach (GameObject item in wood_box_list)
                {
                    WoodBoxController box = item.GetComponent<WoodBoxController>();
                    if (box == null || box.Broken)
                        continue;

                    Vector3 boxPos = box.centroid;
                    Vector3 screenPos = main_camera.GetComponent<Camera>().WorldToScreenPoint(boxPos);
                    if (screenPos.z < 0f)
                        continue;

                    Vector3 worldUIPos = tui_camera.GetComponent<Camera>().ScreenToWorldPoint(screenPos);
                    if (!Auto_Lock_Rect.PtInControl(new Vector2(worldUIPos.x, worldUIPos.y)))
                        continue;

                    if (GameSceneController.CheckBlockBetween(boxPos, main_camera.transform.position))
                        continue;

                    float distance = (boxPos - main_camera.transform.position).magnitude;
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        auto_lock_target = new NearestTargetInfo
                        {
                            type = NearestTargetInfo.NearestTargetType.Box,
                            transform = box.transform,
                            screenPos = worldUIPos,
                            target_obj = box
                        };
                    }
                }
            }
        }

        // Update SightBead visual
        if (auto_lock_target != null)
        {
            SightBead.transform.localPosition = new Vector3(auto_lock_target.screenPos.x, auto_lock_target.screenPos.y, 0f);
            SightBead.SetLockColor();
        }
        else
        {
            SightBead.transform.localPosition = Vector3.zero;
            SightBead.SetUnLockColor();
        }
    }


    public void ResetAutoLock()
	{
		cur_auto_lock_time = 0f;
		auto_lock_target = null;
	}

    public static float GetTargetScore(EnemyController enemy)
    {
        if (enemy == null)
            return float.MinValue;

        Vector3 toTarget = enemy.transform.position - Instance.main_camera.transform.position;
        float sqrDist = Mathf.Max(toTarget.sqrMagnitude, 1f);

        float distanceScore = 1f / sqrDist;

        float bossBonus = enemy.IsBoss ? 2f : 0f;

        float damageRecency = 0f;

        return distanceScore + bossBonus + damageRecency;
    }
    public static bool CheckBlockBetweenIgnoreTagged(Vector3 startPos, Vector3 endPos, string ignoreTag, GameObject ignoreObject = null)
    {
        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;
        direction.Normalize();

        RaycastHit[] hits = Physics.RaycastAll(startPos, direction, distance);
        foreach (var hit in hits)
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.CompareTag(ignoreTag))
                continue;

            if (ignoreObject != null && hitObj == ignoreObject)
                continue;

            return true; // Something is blocking the line of sight
        }

        return false; // Clear line of sight
    }

    public virtual void OnGamePause()
    {
        OpenClikPlugin.Show(false);
        HidePanels();
        game_pause_panel.Show();
        ((GamePausePanelController)game_pause_panel).ResetButtonState();
        TAudioManager.instance.musicVolume = 0f;
        TAudioManager.instance.soundVolume = 0f;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(UnlockCursorNextFrame());
    }

    private IEnumerator UnlockCursorNextFrame()
    {
        yield return null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public virtual void OnGameResume()
    {
        OpenClikPlugin.Hide();
        HidePanels();
        game_main_panel.Show();
        TAudioManager.instance.musicVolume = 1f;
        TAudioManager.instance.soundVolume = 1f;
        Time.timeScale = 1f;
        GamePlayingState = PlayingState.Gaming;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public virtual void OnGameQuit()
	{
		OpenClikPlugin.Hide();
		Time.timeScale = 1f;
		TAudioManager.instance.musicVolume = 1f;
		TAudioManager.instance.soundVolume = 1f;
		GameData.Instance.loading_to_scene = "UIShop";
		Application.LoadLevel("Loading");
        GameData.Instance.SaveData();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

	public bool EnableControll()
	{
		if (Instance.player_controller == null)
		{
			return false;
		}
		return player_controller.EnableControll();
	}

	public void OnAddBulletButton()
	{
		if (player_controller.CurPrimaryWeapon.weapon_data.BuyBulletBattle())
		{
			player_controller.UpdateWeaponUIShow();
		}
	}


    private bool can_buy_ammo = false;
    public void UpdateAddBulletButton(bool state)
    {
        game_main_panel.ShowAddBulletButton(state);
        can_buy_ammo = state;
    }

    public void UpdateAddBulletLabel(bool state)
	{
		game_main_panel.ShowAddBulletLabel(state);
	}

	public virtual void FatCookSummon()
	{
		if (mission_controller.mission_type == MissionType.Boss)
		{
            StartCoroutine(mission_controller.SummonBossMinions(((BossMissionController)mission_controller).MissionBoss));
        }
		else
		{
			Debug.LogError("NO SPAWN.");
		}
	}

	public virtual void HalloweenSummon()
	{
		if (mission_controller.mission_type == MissionType.Boss)
		{
            var boss = ((BossMissionController)mission_controller).MissionBoss;
            StartCoroutine(mission_controller.SummonBossMinions(boss));
        }
		else
		{
            Debug.LogError("NO SPAWN.");
        }
	}

	public virtual EnemyData GetBossData()
	{
		if (mission_controller.mission_type == MissionType.Boss)
		{
			return ((BossMissionController)mission_controller).MissionBoss.enemy_data;
		}
		return null;
	}

	public void OnStartConvoyed()
	{
		NPCConvoyMissionController nPCConvoyMissionController = mission_controller as NPCConvoyMissionController;
		if (nPCConvoyMissionController != null)
		{
			nPCConvoyMissionController.StartSpawnEnemy();
			Debug.Log("On start npc convoy! Monster come out!");
		}
	}

	public void OnStartTakeRes()
	{
		NPCResourcesMissionController nPCResourcesMissionController = mission_controller as NPCResourcesMissionController;
		if (nPCResourcesMissionController != null)
		{
			nPCResourcesMissionController.StartSpawnEnemy();
		}
	}

    public virtual void ResetMissionDifficulty()
    {
        if (GameData.Instance.cur_game_type != 0)
        {
            return;
        }

        enemy_standard_reward = GameData.Instance.GetSideEnemyStandardReward();
        enemy_standard_reward_total = GameData.Instance.GetSideEnemyStandardRewardTotal();

        if (GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Main)
        {
            float killTime = GameConfig.Instance.Main_Quest_Difficulty_Set[GameData.Instance.day_level].killTime;
            float playerDps = GameConfig.Instance.Main_Quest_Difficulty_Set[GameData.Instance.day_level].playerDps;
            float weaponDps = GameConfig.Instance.Main_Quest_Difficulty_Set[GameData.Instance.day_level].weaponDps;
            float comboVal = GameConfig.Instance.Main_Quest_Difficulty_Set[GameData.Instance.day_level].comboVal;
            enemy_standard_hp = killTime * (playerDps + weaponDps) * comboVal;
            float playerHp = GameConfig.Instance.Main_Quest_Difficulty_Set[GameData.Instance.day_level].playerHp;
            float deathTime = GameConfig.Instance.Main_Quest_Difficulty_Set[GameData.Instance.day_level].deathTime;
            enemy_standard_dps = playerHp / deathTime;
            float reward = GameConfig.Instance.Main_Quest_Difficulty_Set[GameData.Instance.day_level].reward;
            enemy_standard_reward = reward / enemy_standard_reward_total * enemy_standard_reward;
            enemy_standard_reward_total = reward;
            return;
        }

        int key = 0;
        int hp_n = 0;
        int day_level = GameData.Instance.day_level;
        float hpParaA = 0f;
        float hpParaB = 0f;

        if (GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Daily)
        {
            key = FindValidKey(GameConfig.Instance.Daily_Quest_Hp_Difficulty_Set, day_level);
            hp_n = GameConfig.Instance.Daily_Quest_Hp_Difficulty_Set[key].hp_n;
            hpParaA = GameConfig.Instance.Daily_Quest_Hp_Difficulty_Set[key].hpParaA;
            hpParaB = GameConfig.Instance.Daily_Quest_Hp_Difficulty_Set[key].hpParaB;
        }
        else if (GameData.Instance.cur_quest_info.mission_type == MissionType.Endless)
        {
            key = FindValidKey(GameConfig.Instance.Endless_Quest_Hp_Difficulty_Set, day_level);

            if (key == -1 || !GameConfig.Instance.Endless_Quest_Hp_Difficulty_Set.ContainsKey(key))
            {
                Debug.LogError("[ResetMissionDifficulty] Missing Endless HP Difficulty Config for key: " + key + ", day_level: " + day_level);
                hp_n = 1;
                hpParaA = 0f;
                hpParaB = 0f;
            }
            else
            {
                hp_n = GameConfig.Instance.Endless_Quest_Hp_Difficulty_Set[key].hp_n;
                hpParaA = GameConfig.Instance.Endless_Quest_Hp_Difficulty_Set[key].hpParaA;
                hpParaB = GameConfig.Instance.Endless_Quest_Hp_Difficulty_Set[key].hpParaB;
            }
        }
        else
        {
            key = FindValidKey(GameConfig.Instance.Side_Quest_Hp_Difficulty_Set, day_level);
            hp_n = GameConfig.Instance.Side_Quest_Hp_Difficulty_Set[key].hp_n;
            hpParaA = GameConfig.Instance.Side_Quest_Hp_Difficulty_Set[key].hpParaA;
            hpParaB = GameConfig.Instance.Side_Quest_Hp_Difficulty_Set[key].hpParaB;
        }

        enemy_standard_hp = hpParaA + hpParaB * (float)(day_level - hp_n);
        key = 0;
        int dmg_n = 0;
        float num = 1f;

        if (GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Daily)
        {
            key = FindValidKey(GameConfig.Instance.Daily_Quest_Dmg_Difficulty_Set, day_level);
            hpParaA = GameConfig.Instance.Daily_Quest_Dmg_Difficulty_Set[key].dmgParaA;
            hpParaB = GameConfig.Instance.Daily_Quest_Dmg_Difficulty_Set[key].dmgParaB;
            dmg_n = GameConfig.Instance.Daily_Quest_Dmg_Difficulty_Set[key].dmg_n;
            float vatarHpWithLv = GameData.Instance.AvatarData_Set[AvatarType.Human].GetVatarHpWithLv(player_controller.avatar_data.level);
            num = 1f;
        }
        else if (GameData.Instance.cur_quest_info.mission_type == MissionType.Endless)
        {
            key = FindValidKey(GameConfig.Instance.Endless_Quest_Dmg_Difficulty_Set, day_level);

            if (!GameConfig.Instance.Endless_Quest_Dmg_Difficulty_Set.ContainsKey(key))
            {
                Debug.LogError("[ResetMissionDifficulty] Missing Endless DMG Difficulty Config for key: " + key.ToString() + ", day_level: " + day_level.ToString());
                dmg_n = 1;
                hpParaA = 0f;
                hpParaB = 0f;
            }
            else
            {
                dmg_n = GameConfig.Instance.Endless_Quest_Dmg_Difficulty_Set[key].dmg_n;
                hpParaA = GameConfig.Instance.Endless_Quest_Dmg_Difficulty_Set[key].dmgParaA;
                hpParaB = GameConfig.Instance.Endless_Quest_Dmg_Difficulty_Set[key].dmgParaB;
            }

            float vatarHpWithLv = GameData.Instance.AvatarData_Set[AvatarType.Human].GetVatarHpWithLv(player_controller.avatar_data.level);
            num = 1f;
        }
        else
        {
            key = FindValidKey(GameConfig.Instance.Side_Quest_Dmg_Difficulty_Set, day_level);
            hpParaA = GameConfig.Instance.Side_Quest_Dmg_Difficulty_Set[key].dmgParaA;
            hpParaB = GameConfig.Instance.Side_Quest_Dmg_Difficulty_Set[key].dmgParaB;
            dmg_n = GameConfig.Instance.Side_Quest_Dmg_Difficulty_Set[key].dmg_n;
            float vatarHpWithLv = player_controller.avatar_data.hp_capacity;
            num = player_controller.avatar_data.config.fix_val;
        }

        enemy_standard_dps = (hpParaA + hpParaB * (float)(day_level - dmg_n)) * num;

        if (GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Daily)
        {
            if (GameData.Instance.is_crazy_daily)
            {
                enemy_standard_reward *= GameConfig.Instance.Mission_Enemy_Reward_Info.daily_ratio_b;
            }
            else
            {
                enemy_standard_reward *= GameConfig.Instance.Mission_Enemy_Reward_Info.daily_ratio_a;
            }
        }
        else
        {
            enemy_standard_reward *= GameConfig.Instance.Mission_Enemy_Reward_Info.side_ratio;
        }
    }

    private int FindValidKey(IDictionary dict, int day_level)
    {
        int validKey = -1;

        foreach (int k in dict.Keys)
        {
            if (day_level >= k)
            {
                if (k > validKey)
                {
                    validKey = k;
                }
            }
        }

        if (validKey == -1 && dict.Count > 0)
        {
            validKey = int.MaxValue;
            foreach (int k in dict.Keys)
            {
                if (k < validKey)
                {
                    validKey = k;
                }
            }
            Debug.LogWarning("[ResetMissionDifficulty] No matching key found for day_level " + day_level + ". Using fallback key: " + validKey);
        }

        return validKey;
    }


    public float GetStandardWeaponDps(int weapon_lv)
	{
		float num = 0f;
		int num2 = 0;
		foreach (int key in GameConfig.Instance.Standard_Weapon_Dps_Set.Keys)
		{
			if (weapon_lv >= key)
			{
				num2 = key;
			}
		}
		return GameConfig.Instance.Standard_Weapon_Dps_Set[num2].base_val + GameConfig.Instance.Standard_Weapon_Dps_Set[num2].para_val * (float)(weapon_lv - num2);
	}

	public virtual void OnEnemySpawn(EnemyData enemy_data)
	{
		if (mission_controller.mission_type == MissionType.Cleaner)
		{
			((CleanerMissionController)mission_controller).mission_life -= enemy_data.exp;
		}
	}

	public virtual void GoShopScene()
	{
		OpenClikPlugin.Hide();
		GameData.Instance.loading_to_scene = "UIShop";
		Application.LoadLevel("Loading");
	}

	public void RetryMission()
	{
		GameData.Instance.loading_to_scene = Application.loadedLevelName;
		Application.LoadLevel("Loading");
	}

	public void UpdateSecondaryWeaponFrame(string frame)
	{
	}

	public void SetComboBarStar(int star_count)
	{
		game_main_panel.SetComboBarStar(star_count);
	}

    public void OnGameCgStart()
    {
        HidePanels();
        cg_panel.Show();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        canPressEscape = false;
    }

    public void OnGameCgEnd()
    {
        cg_panel.Hide();
        game_main_panel.Show();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GamePlayingState = PlayingState.Gaming;

        if (cur_game_info_panel != null)
        {
            cur_game_info_panel.Show();
        }

        Invoke("ShowMissionLabel", 0.5f);

        canPressEscape = true;
    }

    public void StartCameraRoam()
    {
        OnGameCgStart();
        GamePlayingState = PlayingState.CG;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        string text = "SceneRoamPath";
        switch (GameData.Instance.cur_quest_info.mission_type)
        {
            case MissionType.Npc_Resources:
                text += "_Res";
                break;
            case MissionType.Boss:
                text += "_Boss";
                break;
        }
        GameObject gameObject = GameObject.Find(text);
        roam_order = gameObject.transform.Find("CameraRoamOrder").gameObject.GetComponent<RoamOrder>();
        roam_order.OnShow(main_camera.GetComponent<Camera>(), true);
    }

    public void StopCameraRoam()
    {
        is_skip_cg = true;
        roam_order.OnEnd();
        Instance.enable_boss_spawn = true;
        GamePlayingState = PlayingState.Gaming;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Invoke("OnGameCgEnd", 1f);
    }

    public virtual void OnBossMissionOver()
	{
		is_boss_dead = true;
		game_main_panel.boss_panel.SetContent(1 + " / " + 1);
		CleanSceneEnemy();
		HidePanels();
		((GameCgPanelController)cg_panel).HideSkipButton();
		cg_panel.Show();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

	public void CleanSceneEnemy()
	{
		foreach (EnemyController value in Instance.Enemy_Set.Values)
		{
			value.OnMissionOverDead(player_controller, value.centroid, Vector3.down);
		}
	}

	public virtual void ShowUnlockWeaponPanel()
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

	public void ShowUnlockAvatarPanel()
	{
		Debug.Log("ShowUnlockAvatarPanel avatar:" + unlock_new_avatar_name);
		HidePanels();
		unlock_panel.Show();
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/UIAvatar/" + unlock_new_avatar_name)) as GameObject;
		GameObject render_obj = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance) as GameObject;
		GameObject eff_obj_tem = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Accessory[0]) as GameObject;
		((GameUnlockPanelController)unlock_panel).EnableCameraTexture(render_obj, eff_obj_tem);
		Invoke("GoShopScene", 5f);
	}

    public virtual void KeyManRebirth()
    {
        GameData.Instance.total_crystal.SetIntVal(
            GameData.Instance.total_crystal.GetIntVal() - cur_rebirth_cost,
            GameDataIntPurpose.Crystal
        );
        GameData.Instance.SaveData();
        foreach (NPCController value in Instance.NPC_Set.Values)
        {
            if (cur_rebirth_man == value)
            {
                value.Rebirth();
            }
            else
            {
                value.Recover(value.npc_data.hp_capacity);
            }
        }
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnKeyManRebirth()
    {
        GamePlayingState = PlayingState.Gaming;
        is_logic_paused = false;
        mission_check_finished = false;
        game_main_panel.rebirth_panel.ObRebirthOver();
        mission_controller.SetMissionPaused(false);
        Instance.ChangeCameraFocus(player_controller.transform);
        HidePanels();
        game_main_panel.Show();
        GameData.Instance.SaveData();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ChangeCameraFocus(Transform target)
	{
		main_camera.player = target;
	}

	private void CheckMissionDayType()
	{
		mission_day_type = GameData.Instance.cur_quest_info.mission_day_type;
	}

	public virtual void OnRewardOkButton()
	{
		if (unlock_new_weapon)
		{
			ShowUnlockWeaponPanel();
		}
		else if (unlock_new_avatar)
		{
			ShowUnlockAvatarPanel();
		}
		else
		{
			GoShopScene();
		}
	}

	public void CreateAudioOncePlayer(string audio_name, float life, Vector3 pos)
	{
		GameObject gameObject = new GameObject("AudioOnce");
		gameObject.transform.position = pos;
		gameObject.AddComponent<TAudioController>().PlayAudio(audio_name);
		gameObject.AddComponent<RemoveTimerScript>().life = life;
	}

	private void ShowMissionLabel()
	{
        if (mission_day_type == MissionDayType.Main)
        {
            game_main_panel.ShowMissionDayLabel("IMPORTANT MISSION");
        }
        else if (mission_day_type == MissionDayType.Daily)
        {
            game_main_panel.ShowMissionDayLabel("DAILY MISSION");
        }
        else
        {
            game_main_panel.ShowMissionDayLabel("DAY " + GameData.Instance.day_level);
        }
    }

	public void StartCgLimit(float cg_time)
	{
		is_play_cg = true;
		Invoke("StopCgLimit", cg_time);
	}

    public void StopCgLimit()
    {
        Debug.Log("Camera parent after StopCgLimit: " + (Camera.main.transform.parent ? Camera.main.transform.parent.name : "null"));
        is_play_cg = false;

    }

    public virtual void OnBossDeadOver()
	{
		if (mission_controller.mission_type == MissionType.Boss)
		{
			mission_controller.MissionFinished();
		}
		else
		{
			Debug.LogWarning("OnBossDeadOver Error.");
		}
	}
}
