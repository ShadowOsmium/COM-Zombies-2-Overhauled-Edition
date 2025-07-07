using UnityEngine;

public class IAPPlugin
{
	public enum Status
	{
		kUserCancel = -2,
		kError,
		kBuying,
		kSuccess
	}

	public static void AddProductId(string productId)
	{
	}

	public static void LoadProductInfo()
	{
	}

	public static string GetProductInfoCurrency(string productId)
	{
		return string.Empty;
	}

	public static float GetProductInfoPrice(string productId)
	{
		return 0f;
	}

	public static void NowPurchaseProduct(string productId, string productCount)
	{
		if (MiscPlugin.IsIAPCrack())
		{
			Debug.Log("IsIAPCrack!!!!!!");
		}
		else
		{
			GoogleIAB.purchaseProduct(productId);
		}
	}

	public static int GetPurchaseStatus()
	{
		return 1;
	}

	public static string GetTransactionIdentifier()
	{
		return string.Empty;
	}

	public static string GetTransactionReceipt()
	{
		return string.Empty;
	}

	public static void DoRestoreProduct()
	{
	}

	public static int DoRestoreStatus()
	{
		return 1;
	}

	public static string[] DoRestoreGetProductId()
	{
		string empty = string.Empty;
		return empty.Split('|');
	}
}
