using UnityEngine;

public class GameUIAutoSize : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(960f, 640f);

    private void Start()
    {
        float screenWidth = Screen.width;
        float scaleX = screenWidth / referenceResolution.x;

        transform.localScale = new Vector3(
            transform.localScale.x * scaleX,
            transform.localScale.y * scaleX,
            transform.localScale.z);
    }
}
