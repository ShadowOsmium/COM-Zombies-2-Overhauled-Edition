using CoMZ2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapSceneController : UISceneController
{
    public TUICamera TUICamera;

    public GameObject BackToShopButton;

    public UIShopPanelController MissionPanel;

    public UIShopPanelController EndlessMissionPanel;

    public UIShopPanelController MissionDailyPanel;

    public UIShopPanelController TimeAliveMissionPanel;

    public UIShopPanelController NpcResourcesMissionPanel;

    public UIShopPanelController CleanerMissionPanel;

    public UIShopPanelController BossMissionPanel;

    public UIShopPanelController NpcConvoyMissionPanel;

    public UIShopPanelController NickNamePanel;

    public GameObject PromoCodePanel;

    public Dictionary<string, List<Transform>> missionCategoryTransforms;

    public Transform MissionIcons;

    public Transform coop_trans;

    public Transform endless_trans;

    public new static UIMapSceneController Instance
    {
        get { return (UIMapSceneController)UISceneController.instance; }
    }

    public Rect ScreenRect { get; set; }

    private void Awake()
    {
        UISceneController.instance = this;
        GameConfig.CheckGameConfig();
        GameData.CheckGameData();
        MenuAudioController.CheckGameMenuAudio();

        int num = 2;
        if (!TUI.IsRetina())
            num = 1;
        else if (TUI.IsDoubleHD())
            num = 4;

        ScreenRect = new Rect(
            0f - (float)(Screen.width / num / 2),
            0f - (float)(Screen.height / num / 2),
            Screen.width / num,
            Screen.height / num
        );

        missionCategoryTransforms = new Dictionary<string, List<Transform>>();

        missionCategoryTransforms["MainSide"] = FindMissionPointsByPrefix("MainPoint");
        missionCategoryTransforms["MainSide"].AddRange(FindMissionPointsByPrefix("SidePoint"));
        missionCategoryTransforms["Daily"] = FindMissionPointsByPrefix("DailyPoint");

        AddMissionIconsByTagToCategory("Mission_Tag", "MainSide");
        AddMissionIconsByTagToCategory("Daily_Mission_Tag", "Daily");
    }

    private List<Transform> FindMissionPointsByPrefix(string prefix)
    {
        List<Transform> points = new List<Transform>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (var obj in allObjects)
        {
            if (obj.name.StartsWith(prefix))
            {
                points.Add(obj.transform);
            }
        }
        return points;
    }

    private void AddMissionIconsByTagToCategory(string tag, string categoryKey)
    {
        GameObject[] missionIcons = GameObject.FindGameObjectsWithTag(tag);
        if (!missionCategoryTransforms.ContainsKey(categoryKey))
        {
            missionCategoryTransforms[categoryKey] = new List<Transform>();
        }
        foreach (GameObject icon in missionIcons)
        {
            missionCategoryTransforms[categoryKey].Add(icon.transform);
        }
    }

    private IEnumerator Start()
    {
        var labelDayObj = GameObject.Find("LabelDay");
        if (labelDayObj != null)
        {
            var label = labelDayObj.GetComponent<TUILabel>();
            if (label != null)
                label.Text = "Day " + GameData.Instance.day_level;
        }

        while (!GameConfig.Instance.Load_finished)
        {
            yield return null;
        }

        List<QuestInfo> missionQuest = new List<QuestInfo>();
        GameData.Instance.SetMapMissionList(ref missionQuest);

        foreach (QuestInfo info in missionQuest)
        {
            if (GameData.Instance != null && GameData.Instance.blackname)
            {
                // Only skip blacklisted mission types
                bool isRestrictedType =
                    info.mission_type == MissionType.Coop ||
                    info.mission_type == MissionType.Endless ||
                    info.mission_day_type == MissionDayType.Daily;

                if (isRestrictedType)
                {
                    continue; // Don't spawn this icon
                }
            }

            GameObject prefab = Resources.Load<GameObject>("Prefab/missionIcon");
            if (prefab != null)
            {
                GameObject icon = Object.Instantiate(prefab);
                icon.transform.SetParent(MissionIcons, false);
                icon.GetComponent<MissionIconObj>().Init(info);
                yield return null;
            }
            else
            {
                Debug.LogError("Prefab missionIcon not found in Resources/Prefab");
                yield break;
            }
        }


        GameData.Instance.reset_nist_time_finish = OnResetServerTimeFinish;
        GameData.Instance.reset_nist_time_error = OnResetSeverTimeError;
        GameData.Instance.cur_game_type = GameData.GamePlayType.Normal;
        GameData.Instance.is_crazy_daily = false;

        if (TapJoyScript.Instance != null)
        {
            TapJoyScript.Instance.points_add_call_back = OnTapJoyPointsAdd;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameData.Instance.reset_nist_time_finish = null;
        GameData.Instance.reset_nist_time_error = null;

        if (TapJoyScript.Instance != null)
        {
            TapJoyScript.Instance.points_add_call_back = null;
        }
    }

    public void ConnectServer()
    {
        StartCoroutine(GameData.Instance.ResetCurServerTime());
    }

    private void OnResetServerTimeFinish()
    {
        IndicatorBlockController.Hide();
        MissionDailyPanel.Show();
    }

    private void OnResetSeverTimeError()
    {
        OnResetServerTimeFinish();
    }

    private void OnTapJoyPointsAdd()
    {
        MoneyController.UpdateInfo();
    }
}