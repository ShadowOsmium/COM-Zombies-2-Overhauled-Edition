using UnityEngine;

public class GameCgPanelController : UIPanelController
{
	public TUIButtonClick skip_button;

	private void Start()
	{
	}

	private void Update()
	{
	}

    public void HideSkipButton()
    {
        skip_button.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowSkipButton()
    {
        skip_button.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
