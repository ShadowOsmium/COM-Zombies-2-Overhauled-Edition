using UnityEngine;

public class UIMoneyController : MonoBehaviour
{
	public TUILabel CashLabel;

	public TUILabel CrystalLabel;

	public TUILabel VoucherLabel;

	public UIShopPanelController IapPanel;

	private void Start()
	{
		UpdateInfo();
	}

    public void UpdateInfo()
    {
        /*if (GameData.Instance.blackname)
        {
            CashLabel.Text = "9999999";
            CrystalLabel.Text = "9999999";
            VoucherLabel.Text = "9999999";
        }
        else*/
        {
            CashLabel.Text = GameData.Instance.total_cash.ToString();
            CrystalLabel.Text = GameData.Instance.total_crystal.ToString();
            VoucherLabel.Text = GameData.Instance.total_voucher.ToString();
        }
    }

    private void OnShowIapEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			//IapPanel.Show();
		}
	}

	private void OnHideIapEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			//IapPanel.Hide(false);
		}
	}
}
