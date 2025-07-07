using UnityEngine;

public class DevicePlugin
{
	public static AndroidJavaClass androidplatform;

	public static void InitAndroidPlatform()
	{
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	androidplatform = new AndroidJavaClass("com.trinitigame.androidplatform.AndroidPlatformActivity");
		//}
	}

	public static void AndroidQuit()
	{
		//androidplatform.CallStatic("AndroidQuit");
	}

	public static string GetDeviceModel()
	{
		return string.Empty;
	}

	public static void OpenclikShow(string url, string appid)
	{
		//androidplatform.CallStatic("ShowAd", url, appid);
	}

	public static string GetDeviceModelDetail()
	{
		return string.Empty;
	}

	public static string GetUUID()
	{
		return "";
//		return androidplatform.CallStatic<string>("GetUUID", new object[0]);
	}

	public static string GetCountryCode()
	{
		return "";
		//return androidplatform.CallStatic<string>("GetCountry", new object[0]);
	}

	public static string GetLanguageCode()
	{
		return "";
		//return string.Empty;
	}

	public static string GetSysVersion()
	{
		return "";
		//return androidplatform.CallStatic<string>("GetAndroidVersion", new object[0]);
	}

	public static string GetAppVersion()
	{
		return string.Empty;
	}

	public static string GetAppBundleId()
	{
		return string.Empty;
	}
}
