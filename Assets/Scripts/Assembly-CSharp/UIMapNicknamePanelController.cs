using System.Text.RegularExpressions;
using UnityEngine;

public class UIMapNicknamePanelController : UIShopPanelController
{
    public TUILabel nick_name;

    private TouchScreenKeyboard keyboard;

    private bool is_input_name;

    protected Regex myRex;

    private string input_str = string.Empty;

    private string sceneToLoadAfterConfirm = null;

    private void Awake()
    {
        myRex = new Regex("^[A-Za-z0-9]+$");
    }

    public override void Show()
    {
        if (!GameData.Instance.showNicknamePrompt)
        {
            Hide(false);
            return;
        }

        base.Show();
        input_str = GameData.Instance.NickName ?? string.Empty;
        nick_name.Text = input_str;
        is_input_name = true;
        keyboard = TouchScreenKeyboard.Open(input_str, TouchScreenKeyboardType.ASCIICapable);
    }

    public void ShowAndThenGoToScene(string sceneName)
    {
        sceneToLoadAfterConfirm = sceneName;
        Show();
    }

    public override void Hide(bool isPopFromStack)
    {
        is_input_name = false;
        base.Hide(isPopFromStack);
    }

    private void Update()
    {
        if (is_input_name && keyboard != null)
        {
            nick_name.Text = input_str = keyboard.text;
        }

        if (input_str.Length > 8)
        {
            input_str = input_str.Substring(0, 8);
            nick_name.Text = input_str;
        }
    }

    private void OnOKButton(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            if (keyboard != null)
            {
                keyboard.active = false;
            }

            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            is_input_name = false;

            Match match = myRex.Match(input_str);
            if (input_str.Length > 0 && match.Success)
            {
                GameData.Instance.NickName = input_str;
                GameData.Instance.SaveData();

                if (!string.IsNullOrEmpty(sceneToLoadAfterConfirm))
                {
                    UIMapSceneController.Instance.Fade.FadeOut(sceneToLoadAfterConfirm);
                }
                else
                {
                    this.Hide(false);
                }
            }
            else
            {
                GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, gameObject, "Invalid name. Please try again!", OnMsgOkButton, null);
            }
        }
    }

    private void OnEditButton(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            nick_name.Text = input_str = GameData.Instance.NickName ?? string.Empty;
            is_input_name = true;
            keyboard = TouchScreenKeyboard.Open(input_str, TouchScreenKeyboardType.ASCIICapable);
        }
    }

    private void OnMsgOkButton()
    {
        nick_name.Text = input_str = GameData.Instance.NickName ?? string.Empty;
        is_input_name = true;
        keyboard = TouchScreenKeyboard.Open(input_str, TouchScreenKeyboardType.ASCIICapable);
    }
}
