using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class TUICamera : MonoBehaviour
{
    public bool lock960x640;

    public Rect m_viewRect;

    private Camera cam;

    public void Initialize(int layer, int depth)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        Camera cam = GetComponent<Camera>();

        cam.orthographic = true;
        cam.nearClipPlane = -128f;
        cam.farClipPlane = 128f;
        cam.depth = depth;
        cam.cullingMask = 1 << layer;
        cam.clearFlags = CameraClearFlags.Depth;

        bool isRetina = TUI.IsRetina();
        bool isDoubleHD = TUI.IsDoubleHD();

        float pixelScale = (isRetina ? 0.5f : 1f) / (isDoubleHD ? 2f : 1f);
        m_viewRect = new Rect(0, 0, Screen.width, Screen.height);

        if (lock960x640)
        {
            // Apply letterbox/pillarbox if needed
            if (Screen.width >= 960 && Screen.height >= 640)
            {
                float left = (Screen.width - 960f) * 0.5f;
                float top = (Screen.height - 640f) * 0.5f;
                m_viewRect = new Rect(left, top, 960f, 640f);
            }
            else if (Screen.width >= 640 && Screen.height >= 960)
            {
                float left = (Screen.width - 640f) * 0.5f;
                float top = (Screen.height - 960f) * 0.5f;
                m_viewRect = new Rect(left, top, 640f, 960f);
            }
        }

        cam.pixelRect = m_viewRect;
        cam.aspect = m_viewRect.width / m_viewRect.height;

        // Use fixed orthographic size; this determines visible area vertically
        cam.orthographicSize = 160f;
    }
}