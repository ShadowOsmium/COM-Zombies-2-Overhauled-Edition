using UnityEngine;

public class UIPromoCodePanelController : UIShopPanelController
{
    public static UIPromoCodePanelController Instance { get; private set; }

    public TUILabel promoCodeLabel;

    private TouchScreenKeyboard keyboard;
    private bool isInputActive;
    private string inputStr = string.Empty;

    private void Awake()
    {
        Instance = this;
    }

    public override void Show()
    {
        base.Show();
        gameObject.SetActive(true);
        inputStr = string.Empty;
        promoCodeLabel.Text = inputStr;
        isInputActive = false;
        keyboard = null;
    }

    public override void Hide(bool isPopFromStack)
    {
        isInputActive = false;
        base.Hide(isPopFromStack);
    }

    private void Update()
    {
        if (isInputActive && keyboard != null)
        {
            promoCodeLabel.Text = inputStr = keyboard.text;

            if (inputStr.Length > 20)
            {
                inputStr = inputStr.Substring(0, 20);
                promoCodeLabel.Text = inputStr;
            }
        }
    }

    private void OnButtonOpenPromoCodePanel(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            Show();
        }
    }

    private void OnEditButton(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            isInputActive = true;
            keyboard = TouchScreenKeyboard.Open(inputStr, TouchScreenKeyboardType.Default);
        }
    }

    private void OnOKButton(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType != 3) return;

        if (keyboard != null)
            keyboard.active = false;

        isInputActive = false;

        CrossPlatformPromoCodeManager promoManager = FindObjectOfType<CrossPlatformPromoCodeManager>();
        if (promoManager != null)
        {
            string result = promoManager.TryRedeemCode(inputStr);

            if (string.IsNullOrEmpty(result))
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    gameObject,
                    "Invalid or inactive promo code.",
                    null,
                    null,
                    true
                );
            }
            else if (result == "USED")
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    gameObject,
                    "This code has already been used.",
                    null,
                    null,
                    true
                );
            }
            else
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    gameObject,
                    "Code accepted!\n" + result,
                    () => { Hide(true); },
                    null,
                    true
                );
            }
        }
    }

    private void OnCloseButton(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
            Hide(false);
        }
    }

}
