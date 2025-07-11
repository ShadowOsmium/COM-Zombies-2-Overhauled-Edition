using CoMZ2;
using UnityEngine;

public class MissionIconObj : MonoBehaviour
{
    public TUIMeshSprite Background;
    public TUIMeshSprite Image;
    public GameObject Arrow;

    private QuestInfo questInfo;
    private Vector3 lastMapPointPosition;
    private Transform mapPoint;

    private void Update()
    {
        if (mapPoint != null && mapPoint.position != lastMapPointPosition)
        {
            PositionFromWorldToTUI(mapPoint.position);
        }
    }

    public void Init(QuestInfo info)
    {
        TUIControl control = GetComponent<TUIControl>();
        questInfo = info;

        int mission_day_type = (int)info.mission_day_type;

        string textureName = "mission_";
        textureName = (info.mission_type == MissionType.Boss) ? textureName + "boss_" :
                      (info.mission_type == MissionType.Npc_Convoy) ? textureName + "help_" :
                      (info.mission_type == MissionType.Time_ALive) ? textureName + "time_" :
                      (info.mission_type == MissionType.Npc_Resources) ? textureName + "resource_" :
                      textureName + "zombie_";

        textureName += mission_day_type.ToString("G");
        //transform.localScale = Vector3.one * 1.1f;

        if (info.mission_type == MissionType.Coop)
        {
            textureName = "Boss-raid";
            mapPoint = UIMapSceneController.Instance.coop_trans;
            Background.texture = string.Empty;
        }
        else if (info.mission_type == MissionType.Endless)
        {
            textureName = "mission_endless";
            mapPoint = UIMapSceneController.Instance.endless_trans;
            Background.texture = "endless";
        }
        else
        {
            // Assign background
            Background.texture = "mission_bk_" + mission_day_type.ToString("G");

            string category = (info.mission_day_type == MissionDayType.Daily) ? "Daily" : "MainSide";

            var categoryList = UIMapSceneController.Instance.missionCategoryTransforms[category];
            if (categoryList != null && categoryList.Count > 0)
            {
                int index = Random.Range(0, categoryList.Count);
                mapPoint = categoryList[index];
                categoryList.RemoveAt(index);
            }
            else
            {
                Debug.LogWarning("No available map points in category: " + category);
                GameObject fallbackGO = new GameObject("FallbackPoint");
                fallbackGO.transform.SetParent(transform.parent, false);
                fallbackGO.transform.position = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                mapPoint = fallbackGO.transform;
            }
        }

        Image.texture = textureName;
        PositionFromWorldToTUI(mapPoint.position);
        UpdateArrow();
    }

    private void PositionFromWorldToTUI(Vector3 worldPosition)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(worldPosition);
        Camera tuiCam = UIMapSceneController.Instance.TUICamera.GetComponent<Camera>();

        float x = (viewportPoint.x - 0.5f) * tuiCam.orthographicSize * 2f * tuiCam.aspect;
        float y = (viewportPoint.y - 0.5f) * tuiCam.orthographicSize * 2f;
        float z = transform.parent.position.z;

        transform.position = new Vector3(x, y, z);
        lastMapPointPosition = worldPosition;
    }

    public void UpdateArrow()
    {
        Camera cam = UIMapSceneController.Instance.TUICamera.GetComponent<Camera>();
        Vector3 screenPoint = cam.WorldToScreenPoint(transform.position);

        float screenW = Screen.width;
        float screenH = Screen.height;

        bool isOffScreen = screenPoint.x < 0 || screenPoint.x > screenW || screenPoint.y < 0 || screenPoint.y > screenH;

        if (isOffScreen)
        {
            Arrow.SetActive(true);

            Vector2 screenCenter = new Vector2(screenW / 2f, screenH / 2f);
            Vector2 dir = new Vector2(screenPoint.x, screenPoint.y) - screenCenter;
            dir.Normalize();

            float edgeBuffer = 20f;
            Vector2 arrowPos = screenCenter + dir * ((Mathf.Min(screenW, screenH) / 2f) - edgeBuffer);
            Vector3 worldArrowPos = cam.ScreenToWorldPoint(new Vector3(arrowPos.x, arrowPos.y, cam.nearClipPlane + 1f));
            Arrow.transform.position = new Vector3(worldArrowPos.x, worldArrowPos.y, Arrow.transform.position.z);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }
        else
        {
            Arrow.SetActive(false);
        }
    }

    private void OnMissionButtonEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 3) return;

        UISceneController.Instance.SceneAudio.PlayAudio("UI_click");

        MissionType type = questInfo.mission_type;

        if (type == MissionType.Coop)
        {
            if (GameData.Instance.showNicknamePrompt)
            {
                var nicknamePanel = UIMapSceneController.Instance.NickNamePanel.GetComponent<UIMapNicknamePanelController>();
                nicknamePanel.ShowAndThenGoToScene("UICoopHall");
            }
            else
            {
                UIMapSceneController.Instance.Fade.FadeOut("UICoopHall");
            }
        }
        else if (questInfo.mission_day_type == MissionDayType.Daily)
        {
            var dailyPanel = UIMapSceneController.Instance.MissionDailyPanel as UIMapMissionDailyPanelController;
            if (dailyPanel != null)
            {
                dailyPanel.QuestInfo = questInfo;
                UIMapSceneController.Instance.ConnectServer();
            }
        }
        else
        {
            UIShopPanelController panel = null;

            if (type == MissionType.Endless) panel = UIMapSceneController.Instance.EndlessMissionPanel;
            else if (type == MissionType.Cleaner) panel = UIMapSceneController.Instance.CleanerMissionPanel;
            else if (type == MissionType.Time_ALive) panel = UIMapSceneController.Instance.TimeAliveMissionPanel;
            else if (type == MissionType.Npc_Resources) panel = UIMapSceneController.Instance.NpcResourcesMissionPanel;
            else if (type == MissionType.Npc_Convoy) panel = UIMapSceneController.Instance.NpcConvoyMissionPanel;
            else if (type == MissionType.Boss) panel = UIMapSceneController.Instance.BossMissionPanel;
            else panel = UIMapSceneController.Instance.MissionPanel;

            UIMapMissionPanelController missionPanel = panel as UIMapMissionPanelController;
            if (missionPanel != null)
            {
                missionPanel.QuestInfo = questInfo;
                missionPanel.Show();
            }
        }
    }
}
