using UnityEngine;

public class GameUIAutoLayout : MonoBehaviour
{
    public bool left;
    public bool right;
    public bool top;
    public bool bottom;

    private void Start()
    {
        if (gameObject.CompareTag("ExcludeFromLayout"))
        {
            return;
        }

        ApplyLayout();
    }

    public void Reset()
    {
        if (gameObject.CompareTag("ExcludeFromLayout"))
        {
            return;
        }

        ApplyLayout(true);
    }

    private void ApplyLayout(bool useResetDefaults = false)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (!useResetDefaults)
        {
            if (screenWidth == 2048f || screenHeight == 2048f)
            {
            }

            float x = transform.position.x;
            float y = transform.position.y;
            float baseHeight = screenHeight;
            float aspectRatio = screenWidth / screenHeight;
            float diffRatio = 1.5f - aspectRatio;
            float adjustedWidth = screenWidth + diffRatio * 640f;

            if (left)
                x -= (screenWidth - adjustedWidth) / 4f;

            if (right)
                x += (screenWidth - adjustedWidth) / 4f;

            if (top)
                y += (screenHeight - baseHeight) / 4f;

            if (bottom)
                y -= (screenHeight - baseHeight) / 4f;

            transform.position = new Vector3(x, y, transform.position.z);
        }
        else
        {
            if (screenWidth == 2048f || screenHeight == 2048f)
            {
                screenWidth /= 2f;
                screenHeight /= 2f;
            }

            screenWidth = 1136f;
            screenHeight = 640f;

            float x = transform.position.x;
            float y = transform.position.y;

            if (left)
                x -= (screenWidth - 960f) / 4f;

            if (right)
                x += (screenWidth - 960f) / 4f;

            if (top)
                y += (screenHeight - 640f) / 4f;

            if (bottom)
                y -= (screenHeight - 640f) / 4f;

            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}