public class OpenClikPlugin
{
	private enum Status
	{
		kShowBottom,
		kShowFull,
		kShowTop,
		kHide
	}

	private static Status s_Status;

	public static void Initialize(string key)
	{
		AdMobAndroid.init(key);
		AdMobAndroid.requestInterstital(key);
		s_Status = Status.kHide;
	}

	public static void Request(int type)
	{
	}

	public static void Show(int type)
	{
		if (TrinitiAdAndroidPlugin.Instance().CanAdmob())
		{
			AdMobAndroid.hideBanner(false);
		}
	}

	public static void Show(bool show_full)
	{
		if (s_Status == Status.kHide)
		{
			if (show_full)
			{
				if (TrinitiAdAndroidPlugin.Instance().CanAdmob())
				{
					AdMobAndroid.displayInterstital();
				}
			}
			else if (TrinitiAdAndroidPlugin.Instance().CanAdmob())
			{
				AdMobAndroid.hideBanner(false);
			}
			if (show_full)
			{
				s_Status = Status.kShowFull;
			}
			else
			{
				s_Status = Status.kShowBottom;
			}
		}
		else if (s_Status == Status.kShowFull)
		{
			if (!show_full)
			{
				if (TrinitiAdAndroidPlugin.Instance().CanAdmob())
				{
					AdMobAndroid.hideBanner(false);
				}
				s_Status = Status.kShowBottom;
			}
		}
		else if (s_Status == Status.kShowBottom && show_full)
		{
			if (TrinitiAdAndroidPlugin.Instance().CanAdmob())
			{
				AdMobAndroid.displayInterstital();
			}
			s_Status = Status.kShowFull;
		}
	}

	public static void Hide()
	{
		s_Status = Status.kHide;
		if (TrinitiAdAndroidPlugin.Instance().CanAdmob())
		{
			AdMobAndroid.hideBanner(true);
		}
	}

	public static bool IsAdReady()
	{
		if (TrinitiAdAndroidPlugin.Instance().CanAdmob())
		{
			return AdMobAndroid.isInterstitalReady();
		}
		return false;
	}

	public static void Refresh()
	{
		AdMobAndroid.refreshAd();
	}
}
