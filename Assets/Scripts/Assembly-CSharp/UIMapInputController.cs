using UnityEngine;

public class UIMapInputController : MonoBehaviour
{
    public TUICamera TUICamera;
    public Transform MapTransform;
    public Transform Left;
    public Transform Right;
    public Transform Up;
    public Transform Down;

    private void Awake()
    {
        if (MapTransform == null)
        {
            GameObject go = GameObject.Find("Map");
            if (go != null)
                MapTransform = go.transform;
        }

        if (Left == null)
        {
            GameObject go = GameObject.Find("Left");
            if (go != null)
                Left = go.transform;
        }

        if (Right == null)
        {
            GameObject go = GameObject.Find("Right");
            if (go != null)
                Right = go.transform;
        }

        if (Up == null)
        {
            GameObject go = GameObject.Find("Up");
            if (go != null)
                Up = go.transform;
        }

        if (Down == null)
        {
            GameObject go = GameObject.Find("Down");
            if (go != null)
                Down = go.transform;
        }

        if (MapTransform == null) Debug.LogWarning("UIMapInputController: MapTransform not found.");
        if (Left == null) Debug.LogWarning("UIMapInputController: Left boundary not found.");
        if (Right == null) Debug.LogWarning("UIMapInputController: Right boundary not found.");
        if (Up == null) Debug.LogWarning("UIMapInputController: Up boundary not found.");
        if (Down == null) Debug.LogWarning("UIMapInputController: Down boundary not found.");
    }

    private void OnBackToShopEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            UIMapSceneController.Instance.SceneAudio.PlayAudio("UI_back");
            UIMapSceneController.Instance.Fade.FadeOut("UIShop");
        }
    }

    private void OnBackToSceneEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            UIMapSceneController.Instance.SceneAudio.PlayAudio("UI_back");
            if (UIMapSceneController.Instance.PanelStack.Count > 0)
            {
                UIMapSceneController.Instance.PanelStack.Peek().Hide(true);
            }
        }
    }

    private void OnMoveMapEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 2)
            return;

        if (MapTransform == null || Left == null || Right == null || Up == null || Down == null)
            return;

        MapTransform.localPosition += new Vector3(wparam, 0f, lparam) * 3f;

        float x = Mathf.Clamp(MapTransform.localPosition.x, Left.localPosition.x, Right.localPosition.x);
        float z = Mathf.Clamp(MapTransform.localPosition.z, Down.localPosition.z, Up.localPosition.z);

        MapTransform.localPosition = new Vector3(x, MapTransform.localPosition.y, z);

        for (int i = 0; i < UIMapSceneController.Instance.MissionIcons.childCount; i++)
        {
            Transform icon = UIMapSceneController.Instance.MissionIcons.GetChild(i);
            MissionIconObj missionIcon = icon.GetComponent<MissionIconObj>();
            if (missionIcon != null)
                missionIcon.UpdateArrow();
        }
    }
}