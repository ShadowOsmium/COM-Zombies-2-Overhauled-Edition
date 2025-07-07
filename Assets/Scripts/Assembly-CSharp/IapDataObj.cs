using CoMZ2;
using UnityEngine;

public class IapDataObj : MonoBehaviour
{
	public TUILabel Name;

	public IapCenter.IapIdName IapId;

	public CrystalExchangeCash ExchangeId;

	public TUIMeshSprite Image;

	public TUILabel Price;

	public GameObject PriceCrystal;

	public void Init(IapCenter.IapIdName id)
	{
		IapId = id;
		ExchangeId = null;
		//Name.Text = IapCenter.GetIapCrystalCount(id).ToString("N0");
		//Image.texture = "tCrystal_" + id;
		//Price.Text = "$" + GetDollarAmount();
		//PriceCrystal.SetActive(false);
	}

	public void Init(CrystalExchangeCash id)
	{
		ExchangeId = id;
		IapId = IapCenter.IapIdName.None;
		Name.Text = ((!id.CashOrVoucher) ? id.Voucher.ToString("N0") : id.Cash.ToString("N0"));
		Image.texture = "exchange_" + id.Crystal + ((!id.CashOrVoucher) ? "Voucher" : "Gold");
		Price.Text = id.Crystal.ToString("N0");
		PriceCrystal.SetActive(true);
	}

	//private string GetDollarAmount()
	//{
		//switch (IapId)
		//{
		//case IapCenter.IapIdName.Cent99:
		//	return "0.99";
		//case IapCenter.IapIdName.Cent499:
		//	return "4.99";
		//case IapCenter.IapIdName.Cent999:
		//	return "9.99";
		//case IapCenter.IapIdName.Cent1999:
		//	return "19.99";
		//case IapCenter.IapIdName.Cent4999:
		//	return "49.99";
		//case IapCenter.IapIdName.Cent9999:
		//	return "99.99";
		//default:
		//	return string.Empty;
		//}
	//}

	private void OnBuyIapEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType != 3)
		{
			return;
		}
		UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
		if (IapId != IapCenter.IapIdName.None)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, base.transform.parent.parent.gameObject, "Unable to connect to the server! Please try again later.", OnMsgBoxHide, null);
			}
			else
			{
				IapCenter.Instance.NowPurchaseProduct(IapId, "1", OnIapBuySuccess, OnIapBuyError, OnIapBuyCancel);
				IndicatorBlockController.ShowIndicator(base.transform.parent.parent.gameObject, string.Empty);
				GameObject gameObject = Object.Instantiate(Resources.Load("Prefab/BlockMask")) as GameObject;
				gameObject.transform.parent = base.transform.parent.parent;
				gameObject.name = "BlockMask";
				gameObject.transform.localPosition = new Vector3(0f, 0f, -10f);
			}
		}
		if (ExchangeId != null)
		{
			if (GameData.Instance.total_crystal >= ExchangeId.Crystal)
			{
				string text = "Do you want to buy ";
				text += ((!ExchangeId.CashOrVoucher) ? (ExchangeId.Voucher + " vouchers for ") : (ExchangeId.Cash + " gold for "));
				text = text + ExchangeId.Crystal + " tCrystals?";
				GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.DoubleButton, base.transform.parent.parent.gameObject, text, OnDoExchange, OnMsgBoxHide);
			}
			else
			{
				GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, base.transform.parent.parent.gameObject, "Not enough tCrystals!", OnMsgBoxHide, null);
			}
		}
	}

	private void OnIapBuySuccess(IapCenter.IapIdName id)
	{
		IndicatorBlockController.Hide();
		Object.Destroy(base.transform.parent.parent.Find("BlockMask").gameObject);
		UISceneController.Instance.MoneyController.UpdateInfo();
	}

	private void OnIapBuyError(IapCenter.IapIdName id)
	{
		IndicatorBlockController.Hide();
		Object.Destroy(base.transform.parent.parent.Find("BlockMask").gameObject);
		GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, base.transform.parent.parent.gameObject, "Purchase failed!", OnMsgBoxHide, null);
	}

	private void OnIapBuyCancel(IapCenter.IapIdName id)
	{
		IndicatorBlockController.Hide();
		Object.Destroy(base.transform.parent.parent.Find("BlockMask").gameObject);
	}

	private void OnMsgBoxHide()
	{
	}

	private void OnDoExchange()
	{
		if (ExchangeId.CashOrVoucher)
		{
			GameData.Instance.OnExchgCurrcy(GameCurrencyType.Crystal, GameCurrencyType.Cash, ExchangeId.Crystal, ExchangeId.Cash);
			UISceneController.Instance.MoneyController.UpdateInfo();
		}
		else
		{
			GameData.Instance.OnExchgCurrcy(GameCurrencyType.Crystal, GameCurrencyType.Voucher, ExchangeId.Crystal, ExchangeId.Voucher);
			UISceneController.Instance.MoneyController.UpdateInfo();
		}
		GameData.Instance.SaveData();
	}
}
