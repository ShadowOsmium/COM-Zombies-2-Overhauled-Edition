using CoMZ2;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIShopSettingsPanelController : UIShopPanelController
{
    public TUIButtonPush MusicButton;
    public TUIButtonPush SoundButton;
    public TUIButtonClick ResetDataPromptButton;
    public TUIButtonClick NicknamePromptButton;

    public override void Show()
    {
        base.Show();
        ChangeMask("Mask");

        UpdateMusicButtons();
        UpdateNicknameButtonVisual();
    }

    public void UpdateMusicButtons()
    {
        MusicButton.m_bPressed = TAudioManager.instance.isMusicOn;
        MusicButton.Show();
        SoundButton.m_bPressed = TAudioManager.instance.isSoundOn;
        SoundButton.Show();
    }

    public void UpdateNicknameButtonVisual()
    {
        if (NicknamePromptButton != null)
        {
            NicknamePromptButton.m_bPressed = GameData.Instance.showNicknamePrompt;
            NicknamePromptButton.Show();
        }
    }

    public void OnNicknamePromptToggleButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 3) return;

        Debug.Log("Nickname prompt toggle clicked");

        GameMsgBoxController.ShowMsgBox(
            GameMsgBoxController.MsgBoxType.DoubleButton,
            gameObject,
            "Would you like to change your name prompt setting?",
            delegate
            {
                GameData.Instance.showNicknamePrompt = !GameData.Instance.showNicknamePrompt;
                Debug.Log("Nickname prompt toggled: " + GameData.Instance.showNicknamePrompt);
                UpdateNicknameButtonVisual();
                // Show another box stating the new state
                string stateMessage = GameData.Instance.showNicknamePrompt
                    ? "Nickname prompt is now ENABLED."
                    : "Nickname prompt is now DISABLED.";
                GameData.Instance.SaveData();
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    gameObject,
                    stateMessage,
                    null,
                    null,
                    true
                );
            },
            delegate
            {
                Debug.Log("Cancelled nickname prompt toggle");
            },
            true
        );
    }

    public void OnResetDataButton(TUIControl control, int eventType, float wparam, float lparam, object data)
    {
        if (eventType != 3) return;

        Debug.Log("Reset data button clicked");

        GameMsgBoxController.ShowMsgBox(
            GameMsgBoxController.MsgBoxType.DoubleButton,
            gameObject,
            "Are you sure you want to reset your data? This action cannot be undone.",
            delegate
            {
                MenuAudioController.DestroyGameMenuAudio();
                GameData.Instance.didResetSave = true;
                GameData.Instance.Init();
                GameData.Instance.blackname = false;
                GameData.Instance.SaveData();
                SceneManager.LoadScene("InitScene");

                Debug.Log("Data reset.");
            },
            delegate
            {
                Debug.Log("Reset data cancelled");
            },
            true
        );
    }

    private void OnButtonSettingsEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            Show();
        }
    }

    private void OnButtonMusicEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
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
        MusicButton.m_bPressed = TAudioManager.instance.isMusicOn;
        MusicButton.Show();
    }

    private void OnButtonSoundEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
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
        SoundButton.m_bPressed = TAudioManager.instance.isSoundOn;
        SoundButton.Show();
    }

    private void OnButtonCreditEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            Transform transform = base.transform.parent.Find("CreditPanel");
            transform.GetComponent<UIShopPanelController>().Show();
            Transform transform2 = transform.Find("image");
            if (Mathf.Max(Screen.width, Screen.height) == 960)
            {
                float num = 0.8450704f;
                transform2.localScale = new UnityEngine.Vector3(num, num, 1f);
            }
            else if (Mathf.Max(Screen.width, Screen.height) == 1024)
            {
                float x = 0.901408434f;
                transform2.localScale = new UnityEngine.Vector3(x, 1f, 1f);
            }
        }
    }

    private void OnButtonReviewEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            Application.OpenURL("https://discord.gg/X2j3szhMzt");
        }
    }

    private void OnButtonSupportEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            Application.OpenURL("https://discord.gg/jthpN6g2RS");
        }
    }
}
