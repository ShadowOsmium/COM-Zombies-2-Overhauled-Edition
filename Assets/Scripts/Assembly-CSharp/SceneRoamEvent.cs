using UnityEngine;

public class SceneRoamEvent : MonoBehaviour, IRoamEvent
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameSceneController.Instance.StopCameraRoam();
            OnGameCgEnd();
        }
    }

    public void OnRoamTrigger()
	{
		Debug.Log("on roam triger!");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

	public void OnRoamStop()
	{
		Debug.Log("on roam stop!");
		if (GameSceneController.Instance.IsSkipCg)
		{
			return;
		}
		if (GetComponent<CameraFadeEvent>() != null)
		{
			CameraFadeEvent component = GetComponent<CameraFadeEvent>();
			if (component.isFadeOut)
			{
				component.on_fadeout_end = OnGameCgEnd;
			}
		}
		else
		{
			OnGameCgEnd();
		}
	}

    private void OnGameCgEnd()
    {
        GameSceneController.Instance.OnGameCgEnd();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SkipCutsceneManually()
    {
        Debug.Log("[SceneRoamEvent] SkipCutsceneManually called");

        GameSceneController.Instance.is_skip_cg = true;
        GameSceneController.Instance.is_play_cg = false;

        CancelInvoke();
        StopAllCoroutines();

        OnGameCgEnd();
    }
}
