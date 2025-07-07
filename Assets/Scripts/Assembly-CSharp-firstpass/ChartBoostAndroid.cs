using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class ChartBoostAndroid
{
	private static AndroidJavaObject _plugin;

	static ChartBoostAndroid()
	{
		//if (Application.platform != RuntimePlatform.Android)
		//{
			return;
		//}
		//using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.chartboost.ChartboostPlugin"))
		//{
		//	_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		//}
	}

	public static void onStart()
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	_plugin.Call("onStart");
		//}
	}

	public static void onDestroy()
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	_plugin.Call("onDestroy");
		//}
	}

	public static void onStop()
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	_plugin.Call("onStop");
		//}
	}

	public static void onBackPressed()
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	_plugin.Call("onBackPressed");
		//}
	}

	public static void init(string appId, string appSignature, bool shouldRequestInterstitialsInFirstSession = true)
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	_plugin.Call("init", appId, appSignature, shouldRequestInterstitialsInFirstSession);
		//}
	}

	public static void cacheInterstitial(string location)
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	if (location == null)
		//	{
		//		location = string.Empty;
		//	}
		//	_plugin.Call("cacheInterstitial", location);
		//}
	}

	public static bool hasCachedInterstitial(string location)
	{
		//if (Application.platform != RuntimePlatform.Android)
		//{
			return false;
		//}
		//if (location == null)
		//{
		//	location = string.Empty;
		//}
		//return _plugin.Call<bool>("hasCachedInterstitial", new object[1] { location });
	}

	public static void showInterstitial(string location)
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	if (location == null)
		//	{
		//		location = string.Empty;
		//	}
		//	_plugin.Call("showInterstitial", location);
		//}
	}

	public static void cacheMoreApps()
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	_plugin.Call("cacheMoreApps");
		//}
	}

	public static bool hasCachedMoreApps()
	{
		//if (Application.platform != RuntimePlatform.Android)
		//{
			return false;
		//}
		//return _plugin.Call<bool>("hasCachedMoreApps", new object[0]);
	}

	public static void showMoreApps()
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	_plugin.Call("showMoreApps");
		//}
	}

	public static void trackEvent(string eventIdentifier, double value, Dictionary<string, object> metaData)
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	metaData = metaData ?? new Dictionary<string, object>();
		//	_plugin.Call("trackEvent", eventIdentifier, value, metaData.toJson());
		//}
	}
}
