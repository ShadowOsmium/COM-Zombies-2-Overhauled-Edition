using UnityEngine;

public class GameTUIEvent : MonoBehaviour
{

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnPauseButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.OnGamePause();
		}
	}

	private void OnWeaponButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3 && GameSceneController.Instance.player_controller.EnableReload())
		{
			GameSceneController.Instance.player_controller.WeaponReload();
		}
	}

	private void OnFinishButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameData.Instance.loading_to_scene = "UIShop";
			Application.LoadLevel("Loading");
		}
	}

	private void OnQuitButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.OnGameQuit();
		}
	}

	private void OnResumeButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.OnGameResume();
		}
	}

	private void OnAddBulletButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.OnAddBulletButton();
		}
	}

	private void OnMusicButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		switch (eventType)
		{
		case 1:
			TAudioManager.instance.isMusicOn = true;
			break;
		case 2:
			TAudioManager.instance.isMusicOn = false;
			break;
		}
	}

    private void OnNicknamePromptToggleButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            GameData.Instance.showNicknamePrompt = !GameData.Instance.showNicknamePrompt;
        }
    }   

    private void OnSoudnButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		switch (eventType)
		{
		case 1:
			TAudioManager.instance.isSoundOn = true;
			break;
		case 2:
			TAudioManager.instance.isSoundOn = false;
			break;
		}
	}

	private void OnSliderChange(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 1)
		{
			GameData.Instance.sensitivity_ratio = wparam;
			GameData.Instance.sensitivity_ratio = Mathf.Clamp(GameData.Instance.sensitivity_ratio, 0.01f, 1f);
		}
	}

    private void OnCgSkip(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType == 3)
        {
            control.gameObject.SetActive(false);

            IRoamEvent roamEvent = FindObjectOfType(typeof(MonoBehaviour)) as IRoamEvent;

            if (roamEvent != null)
            {
                Debug.Log("[GameTUIEvent] RoamEvent found, calling SkipCutsceneManually()");
                roamEvent.SkipCutsceneManually();
            }
            else
            {
                Debug.LogWarning("[GameTUIEvent] No IRoamEvent found, calling StopCameraRoam()");
                GameSceneController.Instance.StopCameraRoam();
            }
        }
    }

    private void OnTutorialSkip(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			control.gameObject.SetActive(false);
		}
	}

	private void OnRetryQuitButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.GoShopScene();
		}
	}

	private void OnRetryButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.RetryMission();
		}
	}

	private void OnRebirthButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.KeyManRebirth();
		}
	}

	private void OnRebirthSkipButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.game_main_panel.rebirth_panel.CancelRebirth();
		}
	}

	private void OnRewardOK(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.OnRewardOkButton();
		}
	}

	private void OnAvatarSkillButton1(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.player_controller.ConjureSkill(0);
		}
	}

	private void OnAvatarSkillButton2(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			GameSceneController.Instance.player_controller.ConjureSkill(1);
		}
	}
}
