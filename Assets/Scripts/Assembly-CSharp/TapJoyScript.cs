using System;
using UnityEngine;

public class TapJoyScript : MonoBehaviour
{
	protected const string androidAppId = "60d164ed-0894-4cdc-9c6d-4c4499623a46";

	protected const string androidSecretKey = "C7lpW6yoF3St27E1LQoY";

	protected const string tap_joy_id = "0c43b264-088e-4730-b0c1-bf36c662991b";

	protected const string tap_joy_key = "z8KX2NBXqcDaT2SRQgSQ";

	private bool openingFullScreenAd;

	public Action points_add_call_back;

	protected static TapJoyScript instance;

	public static TapJoyScript Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			//AndroidJNI.AttachCurrentThread();
		}
		TapjoyPlugin.EnableLogging(false);
		TapjoyPlugin.SetCallbackHandler(base.gameObject.name);
		if (Application.platform == RuntimePlatform.Android)
		{
			//TapjoyPlugin.RequestTapjoyConnect("60d164ed-0894-4cdc-9c6d-4c4499623a46", "C7lpW6yoF3St27E1LQoY");
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			//TapjoyPlugin.RequestTapjoyConnect("0c43b264-088e-4730-b0c1-bf36c662991b", "z8KX2NBXqcDaT2SRQgSQ");
		}
		TapjoyPlugin.GetTapPoints();
	}

	public void TapjoyConnectSuccess(string message)
	{
		Debug.Log(message);
	}

	public void TapjoyConnectFail(string message)
	{
		Debug.Log(message);
	}

	public void TapPointsLoaded(string message)
	{
		Debug.Log("TapPointsLoaded: " + message);
		int num = TapjoyPlugin.QueryTapPoints() - GameData.Instance.tapjoyPoints;
		if (num != 0)
		{
			Debug.Log("TapPoints add:" + num);
            GameData.Instance.total_crystal.SetIntVal(GameData.Instance.total_crystal.GetIntVal() + num, GameDataIntPurpose.Crystal);
            GameData.Instance.tapjoyPoints = TapjoyPlugin.QueryTapPoints();
			//GameData.Instance.SaveData();
			if (points_add_call_back != null)
			{
				points_add_call_back();
			}
		}
	}

	public void TapPointsLoadedError(string message)
	{
		Debug.Log("TapPointsLoadedError: " + message);
	}

	public void TapPointsSpent(string message)
	{
		Debug.Log("TapPointsSpent: " + message);
	}

	public void TapPointsSpendError(string message)
	{
		Debug.Log("TapPointsSpendError: " + message);
	}

	public void TapPointsAwarded(string message)
	{
		Debug.Log("TapPointsAwarded: " + message);
	}

	public void TapPointsAwardError(string message)
	{
		Debug.Log("TapPointsAwardError: " + message);
	}

	public void CurrencyEarned(string message)
	{
		Debug.Log("CurrencyEarned: " + message);
		TapjoyPlugin.ShowDefaultEarnedCurrencyAlert();
	}

	public void FullScreenAdLoaded(string message)
	{
		Debug.Log("FullScreenAdLoaded: " + message);
		TapjoyPlugin.ShowFullScreenAd();
	}

	public void FullScreenAdError(string message)
	{
		Debug.Log("FullScreenAdError: " + message);
	}

	public void DailyRewardAdLoaded(string message)
	{
		Debug.Log("DailyRewardAd: " + message);
		TapjoyPlugin.ShowDailyRewardAd();
	}

	public void DailyRewardAdError(string message)
	{
		Debug.Log("DailyRewardAd: " + message);
	}

	public void DisplayAdLoaded(string message)
	{
		Debug.Log("DisplayAdLoaded: " + message);
		if (!openingFullScreenAd)
		{
			TapjoyPlugin.ShowDisplayAd();
		}
	}

	public void DisplayAdError(string message)
	{
		Debug.Log("DisplayAdError: " + message);
	}

	public void VideoAdStart(string message)
	{
		Debug.Log("VideoAdStart: " + message);
	}

	public void VideoAdError(string message)
	{
		Debug.Log("VideoAdError: " + message);
	}

	public void VideoAdComplete(string message)
	{
		Debug.Log("VideoAdComplete: " + message);
	}

	public void ShowOffersError(string message)
	{
		Debug.Log("show offers error" + message);
	}

	public void ViewOpened(string message)
	{
		openingFullScreenAd = true;
		int num = int.Parse(message);
		Debug.Log("C#: HandleViewOpened of view type " + (TapjoyViewType)num);
	}

	public void ViewClosed(string message)
	{
		openingFullScreenAd = false;
		int num = int.Parse(message);
		Debug.Log("C#: HandleViewClosed of view type " + (TapjoyViewType)num);
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
		}
	}

	public static void CreateTapjoyObj()
	{
		if (Instance == null)
		{
			GameObject gameObject = new GameObject("TapJoyObj");
			gameObject.transform.parent = null;
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.AddComponent<TapJoyScript>();
		}
	}

	public static void DestroyTapjoyObj()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(Instance.gameObject);
		}
	}
}
