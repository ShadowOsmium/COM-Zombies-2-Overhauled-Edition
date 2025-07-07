using System.Collections;
using System.Collections.Generic;
using System.Text;
using Boomlagoon.JSON;
using LitJson;
using Prime31;
using UnityEngine;

public class IapCenter : MonoBehaviour
{
	public enum IapIdName
	{
		None = -1,
		Cent99,
		Cent499,
		Cent999,
		Cent1999,
		Cent4999,
		Cent9999,
		Count
	}

	public enum IapProcessState
	{
		None = -1,
		PrePurchase,
		PurchaseToApple,
		PreCheck,
		PurchaseCheck
	}

	public const string action_test = "test";

	public const string action_purchase = "purchase";

	public const string action_check = "check";

	private string amazon_userId = string.Empty;

	private bool itemOwned;

	private static IapCenter instance;

	private IapIdName cur_iap_id = IapIdName.None;

	private IapProcessState iap_process_state = IapProcessState.None;

	private OnPurchaseSucess on_purchase_sucess;

	private OnPurchaseError on_purchase_error;

	private OnPurchaseCancel on_purchase_cancel;

	protected string m_sRandom = string.Empty;

	protected int m_nRat;

	protected int m_nRatA;

	protected int m_nRatB;

	protected string cur_tid = string.Empty;

	protected string cur_receipt = string.Empty;

	protected float check_rate_interval = 2f;

	protected float cur_check_time;

	protected bool is_restore;

	public static IapCenter Instance
	{
		get
		{
			return instance;
		}
	}

	public IapIdName CurBuyingId
	{
		get
		{
			return cur_iap_id;
		}
	}

	private void OnEnable()
	{
		GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
		string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAn2tUEDd3js105Uis7gn8bCV+mpShC9vHwyxhJ9BOEjOnsfBVyfZw2gzU4Cgh+4d84bALwaUd8dkMk7+KT8V/SRdX8pgzAlNchIIKErDOU+R7boCzMOE7+xWVggR0yVhWRQryeuoI6WAdTicaQy+7vAhYwNqxlPQyDesgD/N7WaWhZ2tCIjqBzQnii3rjjNj02LoYAWmjRpdxu7GmCZGk86GKzOx7Hd7yTW+QnY25kyLH2PK2PrLKTmJUQFhWp9e4pX8DYZ6puuXASk5iUHfG2hqa39nmIjDBs7kIUMgd/Ix62DEN53DzwgoB0wvN7qqs1XCcX02eR983y/ScYzvwIQIDAQAB";
		GoogleIAB.init(publicKey);
		GoogleIAB.setAutoVerifySignatures(true);
		string[] skus = new string[6] { "com.trinitigame.callofminizombies2.099centsv101", "com.trinitigame.callofminizombies2.499centsv10", "com.trinitigame.callofminizombies2.999centsv10", "com.trinitigame.callofminizombies2.1999centsv10", "com.trinitigame.callofminizombies2.4999centsv10", "com.trinitigame.callofminizombies2.9999centsv10" };
		GoogleIAB.queryInventory(skus);
	}

	private void OnDisable()
	{
		GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent -= purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
		GoogleIAB.unbindService();
	}

	private void billingSupportedEvent()
	{
		Debug.Log("billingSupportedEvent");
	}

	private void billingNotSupportedEvent(string error)
	{
	}

	private void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		if (itemOwned)
		{
			GoogleIAB.consumeProduct(GetIapId(cur_iap_id));
			itemOwned = false;
		}
		Prime31.Utils.logObject(purchases);
		Prime31.Utils.logObject(skus);
	}

	private void queryInventoryFailedEvent(string error)
	{
		if (itemOwned)
		{
			itemOwned = false;
		}
	}

	private void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
	{
		GoogleIAB.consumeProduct(GetIapId(cur_iap_id));
	}

	private void purchaseSucceededEvent(GooglePurchase purchase)
	{
		GoogleIAB.consumeProduct(GetIapId(cur_iap_id));
	}

	private void consumePurchaseSucceededEvent(GooglePurchase purchase)
	{
		string productId = purchase.productId;
		string orderId = purchase.orderId;
		string originalJson = purchase.originalJson;
		string signature = purchase.signature;
		if (GameConfig.IsEditorMode() || !GameData.Instance.TRINITI_IAP_CEHCK)
		{
			PurchaseSucess();
			RemoveIapFailedState();
			return;
		}
		iap_process_state = IapProcessState.PreCheck;
		cur_tid = orderId;
		cur_receipt = originalJson;
		Iap_Resend.Set_IAP_android_list(GetIapId(cur_iap_id) + "|" + cur_tid + "|" + cur_receipt + "|purchase|" + signature);
		//GameData.Instance.SaveData();
		SendIAPVerifyRequest(productId, orderId, originalJson, "purchase", signature);
	}

	private void consumePurchaseFailedEvent(string error)
	{
		Debug.Log("consumePurchaseFailedEvent: " + error);
		if (error.Substring(0, 14) == "Item not owned" || error.Substring(0, 14) == "Unable to buy ")
		{
			GoogleIAB.consumeProduct(GetIapId(cur_iap_id));
		}
	}

	private void purchaseFailedEvent(string reason)
	{
		int num = reason.IndexOf("response: ");
		string text = reason.Substring(num + 10);
		num = text.IndexOf(':');
		text = text.Substring(0, num);
		switch (text)
		{
		case "7":
			itemOwned = true;
			GoogleIAB.queryInventory(new string[1] { GetIapId(cur_iap_id) });
			break;
		case "1":
		case "2":
		case "3":
		case "4":
		case "5":
		case "6":
		case "8":
		case "-1000":
		case "-1001":
		case "-1002":
		case "-1003":
		case "-1004":
		case "-1005":
		case "-1006":
		case "-1007":
		case "-1008":
		case "-1009":
		case "-1010":
			PurchaseCancel();
			break;
		}
		Debug.LogWarning("reason id : " + text);
	}

	public static void CheckIapCenter()
	{
		if (!GameObject.Find("IapCenter"))
		{
			GameObject gameObject = new GameObject("IapCenter");
			gameObject.transform.parent = null;
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.AddComponent<IapCenter>();
		}
	}

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private IEnumerator Start()
	{
		yield return 0;
	}

	private void Update()
	{
		if (iap_process_state == IapProcessState.PrePurchase)
		{
			return;
		}
		if (iap_process_state == IapProcessState.PurchaseToApple)
		{
			int purchaseStatus = IAPPlugin.GetPurchaseStatus();
			if (purchaseStatus == 0)
			{
				return;
			}
			if (purchaseStatus == 1)
			{
				if (GameConfig.IsEditorMode() || !GameData.Instance.TRINITI_IAP_CEHCK)
				{
					PurchaseSucess();
					RemoveIapFailedState();
					return;
				}
				iap_process_state = IapProcessState.PreCheck;
				cur_tid = IAPPlugin.GetTransactionIdentifier();
				cur_receipt = IAPPlugin.GetTransactionReceipt();
				SendIAPVerifyRequest(GetIapId(cur_iap_id), cur_tid, cur_receipt, "purchase", string.Empty);
			}
			else if (purchaseStatus == -2)
			{
				PurchaseCancel();
			}
			else if (purchaseStatus < 0 && purchaseStatus != -2)
			{
				PurchaseError();
			}
		}
		else if (iap_process_state != IapProcessState.PreCheck && iap_process_state == IapProcessState.PurchaseCheck)
		{
			cur_check_time += Time.deltaTime;
			if (cur_check_time >= check_rate_interval)
			{
				cur_check_time = 0f;
				SendIAPVerifyResultRequest();
			}
		}
	}

	public string GetIapId(IapIdName id_name)
	{
		string result = string.Empty;
		switch (id_name)
		{
		case IapIdName.Cent99:
			result = "com.trinitigame.callofminizombies2.099centsv101";
			break;
		case IapIdName.Cent499:
			result = "com.trinitigame.callofminizombies2.499centsv10";
			break;
		case IapIdName.Cent999:
			result = "com.trinitigame.callofminizombies2.999centsv10";
			break;
		case IapIdName.Cent1999:
			result = "com.trinitigame.callofminizombies2.1999centsv10";
			break;
		case IapIdName.Cent4999:
			result = "com.trinitigame.callofminizombies2.4999centsv10";
			break;
		case IapIdName.Cent9999:
			result = "com.trinitigame.callofminizombies2.9999centsv10";
			break;
		}
		return result;
	}

	public IapIdName GetIapIdNameType(string id_name)
	{
		IapIdName result = IapIdName.None;
		switch (id_name)
		{
		case "com.trinitigame.callofminizombies2.099centsv101":
			result = IapIdName.Cent99;
			break;
		case "com.trinitigame.callofminizombies2.499centsv10":
			result = IapIdName.Cent499;
			break;
		case "com.trinitigame.callofminizombies2.999centsv10":
			result = IapIdName.Cent999;
			break;
		case "com.trinitigame.callofminizombies2.1999centsv10":
			result = IapIdName.Cent1999;
			break;
		case "com.trinitigame.callofminizombies2.4999centsv10":
			result = IapIdName.Cent4999;
			break;
		case "com.trinitigame.callofminizombies2.9999centsv10":
			result = IapIdName.Cent9999;
			break;
		}
		return result;
	}

	//public static int GetIapCrystalCount(IapIdName id_name)
	//{
	//	int result = 0;
	//	switch (id_name)
	//	{
	//	case IapIdName.Cent99:
	//		result = 0;
	//		break;
	//	case IapIdName.Cent499:
	//		result = 0;
	//		break;
	//	case IapIdName.Cent999:
	//		result = 0;
	//		break;
	//	case IapIdName.Cent1999:
	//		result = 0;
	//		break;
	//	case IapIdName.Cent4999:
	//		result = 0;
	//		break;
	//	case IapIdName.Cent9999:
	//		result = 0;
	//		break;
	//	}
	//	return result;
	//}

	public void NowPurchaseProduct(IapIdName id_name, string productCount, OnPurchaseSucess on_sucess, OnPurchaseError on_error, OnPurchaseCancel on_cancel)
	{
		string iapId = GetIapId(id_name);
		is_restore = false;
		if (iapId == string.Empty)
		{
			Debug.LogError("Iap Purchase Id Error id_name:" + id_name);
		}
		cur_iap_id = id_name;
		on_purchase_sucess = on_sucess;
		on_purchase_error = on_error;
		on_purchase_cancel = on_cancel;
		if (GameConfig.IsEditorMode() || !GameData.Instance.TRINITI_IAP_CEHCK)
		{
			iap_process_state = IapProcessState.PurchaseToApple;
			IAPPlugin.NowPurchaseProduct(iapId, productCount);
		}
		else
		{
			iap_process_state = IapProcessState.PrePurchase;
			IAPPlugin.NowPurchaseProduct(iapId, productCount);
		}
	}

	private IEnumerator UnInstallCallback()
	{
		yield return 1;
		on_purchase_sucess = null;
		on_purchase_error = null;
	}

	private void PurchaseSucess()
	{
		iap_process_state = IapProcessState.None;
		OnPurchaseSucess();
		if (on_purchase_sucess != null)
		{
			on_purchase_sucess(cur_iap_id);
		}
		StartCoroutine(UnInstallCallback());
	}

	private void PurchaseError()
	{
		iap_process_state = IapProcessState.None;
		OnPurchaseError();
		if (on_purchase_error != null)
		{
			on_purchase_error(cur_iap_id);
		}
		StartCoroutine(UnInstallCallback());
	}

	private void PurchaseCancel()
	{
		iap_process_state = IapProcessState.None;
		OnPurchaseCancel();
		if (on_purchase_cancel != null)
		{
			on_purchase_cancel(cur_iap_id);
		}
		StartCoroutine(UnInstallCallback());
	}

	private void OnPurchaseSucess()
	{
		//int iapCrystalCount = GetIapCrystalCount(cur_iap_id);
		//GameData.Instance.total_crystal += iapCrystalCount;
		Hashtable hashtable = new Hashtable();
		//hashtable.Add("tCrystalNum", iapCrystalCount);
		GameData.Instance.UploadStatistics("IAP_Buy_" + cur_iap_id, hashtable);
	}

	private void OnPurchaseError()
	{
	}

	private void OnPurchaseCancel()
	{
	}

	public void SendIAPVerifyRequest_for_Android(string product_Id, string tid, string receipt, string action, string signature, bool restore = false)
	{
		cur_iap_id = GetIapIdNameType(product_Id);
		cur_tid = tid;
		Hashtable hashtable = new Hashtable();
		hashtable["cmd"] = "purchase/android/UserPurchaseBuy";
		hashtable["aid"] = "com.trinitigame.android.callofminizombies2";
		hashtable["uuid"] = GameData.Instance.UserId;
		hashtable["pid"] = product_Id;
		hashtable["tid"] = tid;
		hashtable["info"] = receipt;
		hashtable["signature"] = signature;
		if (!restore)
		{
			m_sRandom = Random.Range(1, 10).ToString();
		}
		hashtable["rand"] = m_sRandom;
		if (!restore)
		{
			m_nRat = Random.Range(1, 10);
		}
		hashtable["rat"] = m_nRat;
		string s = JsonMapper.ToJson(hashtable);
		byte[] post_data = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		//wwwClient.Instance.SendHttpRequest(GameData.Instance.iap_check_url, post_data, OnTrinitiIapServerCheckFinish, OnTrinitiIapServerCheckError, action);
	}

	private void SendIAPVerifyRequest(string product_Id, string tid, string receipt, string action, string signature, bool restore = false)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["cmd"] = "purchase/android/UserPurchaseBuy";
		hashtable["aid"] = "com.trinitigame.android.callofminizombies2";
		hashtable["uuid"] = GameData.Instance.UserId;
		hashtable["pid"] = product_Id;
		hashtable["tid"] = tid;
		hashtable["info"] = receipt;
		hashtable["signature"] = signature;
		if (!restore)
		{
			m_sRandom = Random.Range(1, 10).ToString();
		}
		hashtable["rand"] = m_sRandom;
		if (!restore)
		{
			m_nRat = Random.Range(1, 10);
		}
		hashtable["rat"] = m_nRat;
		string s = JsonMapper.ToJson(hashtable);
		byte[] post_data = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		//wwwClient.Instance.SendHttpRequest(GameData.Instance.iap_check_url, post_data, OnTrinitiIapServerCheckFinish, OnTrinitiIapServerCheckError, action);
	}

	protected void SendIAPVerifyResultRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["cmd"] = "purchase/android/GetPurchaseVerify";
		hashtable["transactionId"] = cur_tid;
		hashtable["randPara"] = m_sRandom;
		string s = JsonMapper.ToJson(hashtable);
		byte[] post_data = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		//wwwClient.Instance.SendHttpRequest(GameData.Instance.iap_check_url, post_data, OnTrinitiIapServerCheckFinish, OnTrinitiIapServerCheckError, "check");
	}

	private void OnTrinitiIapServerCheckFinish(string action, byte[] response_data)
	{
		string jsonString = string.Empty;
		if (response_data == null)
		{
			return;
		}
		byte[] array = XXTEAUtils.Decrypt(response_data, Encoding.UTF8.GetBytes("abcd@@##980[]L>."));
		if (array != null)
		{
			jsonString = Encoding.UTF8.GetString(array);
		}
		JSONObject jSONObject = JSONObject.Parse(jsonString);
		string text = jSONObject["code"].ToString();
		switch (action)
		{
		case "test":
			iap_process_state = IapProcessState.PurchaseToApple;
			IAPPlugin.NowPurchaseProduct(GetIapId(cur_iap_id), "1");
			break;
		case "purchase":
			Debug.LogError("action == action_purchase  code :" + text);
			if (text == "0")
			{
				iap_process_state = IapProcessState.PurchaseCheck;
				m_nRatA = int.Parse(jSONObject["rata"].ToString());
				m_nRatB = int.Parse(jSONObject["ratb"].ToString());
			}
			else
			{
				SaveIapFailedState();
				PurchaseError();
			}
			break;
		case "check":
			if (text == "0")
			{
				string text2 = jSONObject["sta"].ToString();
				if (text2 == "0")
				{
					string text3 = jSONObject["aid"].ToString();
					string text4 = jSONObject["pid"].ToString();
					string s = jSONObject["ratresult"].ToString();
					int num = m_nRat * m_nRatA / 9 + m_nRatB - 3;
					int num2 = int.Parse(s);
					text3 = text3.Replace("\"", string.Empty);
					text4 = text4.Replace("\"", string.Empty);
					if (num == num2 && text4 == GetIapId(cur_iap_id))
					{
						PurchaseSucess();
						Iap_Resend.Set_IAP_android_list("false");
						GameData.Instance.SaveData();
						RemoveIapFailedState();
					}
					else
					{
						RemoveIapFailedState();
						PurchaseError();
					}
				}
				else if (!(text2 == "-1"))
				{
					RemoveIapFailedState();
					PurchaseError();
				}
			}
			else
			{
				SaveIapFailedState();
				PurchaseError();
			}
			break;
		}
	}

	private void OnTrinitiIapServerCheckError(string action, byte[] response_data)
	{
		switch (action)
		{
		case "test":
			PurchaseError();
			break;
		case "purchase":
			SaveIapFailedState();
			PurchaseError();
			break;
		case "check":
			SaveIapFailedState();
			PurchaseError();
			break;
		}
	}

	private void SaveIapFailedState()
	{
		GameData.Instance.SaveIapFailedInfo(m_sRandom, m_nRat, GetIapId(cur_iap_id), cur_tid, cur_receipt);
	}

	private void RemoveIapFailedState()
	{
		GameData.Instance.RemoveIapFailedInfo(m_sRandom, m_nRat, GetIapId(cur_iap_id), cur_tid, cur_receipt);
	}

	private void RestoreIapFailedReceipt(string Random, int Rat, string product_Id, string tid, string receipt, OnPurchaseSucess on_sucess, OnPurchaseError on_error)
	{
		m_sRandom = Random;
		m_nRat = Rat;
		cur_iap_id = GetIapIdNameType(product_Id);
		cur_tid = tid;
		cur_receipt = receipt;
		iap_process_state = IapProcessState.PreCheck;
		SendIAPVerifyRequest(product_Id, cur_tid, cur_receipt, "purchase", string.Empty, true);
	}

	public int CheckIapFailedReceipt(OnPurchaseSucess on_sucess, OnPurchaseError on_error)
	{
		int result = 0;
		if (GameData.Instance.Iap_failed_info.Count > 0)
		{
			is_restore = true;
			using (Dictionary<string, string>.KeyCollection.Enumerator enumerator = GameData.Instance.Iap_failed_info.Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					string[] array = current.Split('|');
					string random = array[0];
					int rat = int.Parse(array[1]);
					string product_Id = array[2];
					string tid = array[3];
					string receipt = GameData.Instance.Iap_failed_info[current];
					RestoreIapFailedReceipt(random, rat, product_Id, tid, receipt, on_sucess, on_error);
					return 1;
				}
				return result;
			}
		}
		return result;
	}
}
