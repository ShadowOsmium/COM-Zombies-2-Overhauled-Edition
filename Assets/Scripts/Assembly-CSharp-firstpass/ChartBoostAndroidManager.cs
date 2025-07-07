using System;
using Prime31;

public class ChartBoostAndroidManager : AbstractManager
{
	public static event Action didFailToLoadMoreAppsEvent;

	public static event Action<string> didCacheInterstitialEvent;

	public static event Action didCacheMoreAppsEvent;

	public static event Action<string> didFinishInterstitialEvent;

	public static event Action<string> didFinishMoreAppsEvent;

	public static event Action didCloseMoreAppsEvent;

	public static event Action<string> didFailToLoadInterstitialEvent;

	public static event Action<string> didShowInterstitialEvent;

	public static event Action didShowMoreAppsEvent;

	static ChartBoostAndroidManager()
	{
		AbstractManager.initialize(typeof(ChartBoostAndroidManager));
	}

	public void didFailToLoadMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didFailToLoadMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didFailToLoadMoreAppsEvent();
		}
	}

	public void didCacheInterstitial(string location)
	{
		if (ChartBoostAndroidManager.didCacheInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didCacheInterstitialEvent(location);
		}
	}

	public void didCacheMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didCacheMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didCacheMoreAppsEvent();
		}
	}

	public void didFinishInterstitial(string param)
	{
		if (ChartBoostAndroidManager.didFinishInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didFinishInterstitialEvent(param);
		}
	}

	public void didFinishMoreApps(string param)
	{
		if (ChartBoostAndroidManager.didFinishMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didFinishMoreAppsEvent(param);
		}
	}

	public void didCloseMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didCloseMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didCloseMoreAppsEvent();
		}
	}

	public void didFailToLoadInterstitial(string location)
	{
		if (ChartBoostAndroidManager.didFailToLoadInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didFailToLoadInterstitialEvent(location);
		}
	}

	public void didShowInterstitial(string location)
	{
		if (ChartBoostAndroidManager.didShowInterstitialEvent != null)
		{
			ChartBoostAndroidManager.didShowInterstitialEvent(location);
		}
	}

	public void didShowMoreApps(string empty)
	{
		if (ChartBoostAndroidManager.didShowMoreAppsEvent != null)
		{
			ChartBoostAndroidManager.didShowMoreAppsEvent();
		}
	}
}
