using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float timeSinceLastUpdate = 0.0f;
    private float updateInterval = 0.2f;

    void Start()
    {
        Application.targetFrameRate = 75;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        timeSinceLastUpdate += Time.unscaledDeltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            // Reset the time counter after the interval
            timeSinceLastUpdate = 0.0f;

            // Update deltaTime based on unscaled time
            deltaTime = Time.unscaledDeltaTime;
        }
    }

    void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
        guiStyle.fontSize = 40;  // Adjust font size as needed

        // Position on the top-left corner with some padding
        float xPos = 40; // 10 pixels from the left edge
        float yPos = 30; // 10 pixels from the top edge

        // Calculate the size of the label to prevent cutoff
        Vector2 size = guiStyle.CalcSize(new GUIContent("FPS: " + Mathf.CeilToInt(1.0f / deltaTime)));

        GUI.Label(new Rect(xPos, yPos, size.x + 10, size.y + 10), "FPS: " + Mathf.CeilToInt(1.0f / deltaTime), guiStyle);
    }
}
