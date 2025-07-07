using System;

public class MiscPlugin
{
	public static int ShowMessageBox1(string title, string message, string button)
	{
		return 0;
	}

	public static int ShowMessageBox2(string title, string message, string button1, string button2)
	{
		return 0;
	}

	public static string GetMacAddr()
	{
		return "000000000000";
	}

	public static bool IsJailbreak()
	{
		return true;
	}

	public static bool IsIAPCrack()
	{
		return false;
	}

	public static void ShowIndicatorSystem(int style, bool iPad, float r, float g, float b, float a)
	{
	}

	public static void HideIndicatorSystem()
	{
	}

	public static int GetAppAcctiveTimes()
	{
		return 0;
	}

	public static int GetIOSYear()
	{
		return DateTime.Now.Year;
	}

	public static int GetIOSMonth()
	{
		return DateTime.Now.Month;
	}

	public static int GetIOSDay()
	{
		return DateTime.Now.Day;
	}

	public static int GetIOSHour()
	{
		return DateTime.Now.Hour;
	}

	public static int GetIOSMin()
	{
		return DateTime.Now.Minute;
	}

	public static int GetIOSSec()
	{
		return DateTime.Now.Second;
	}

	public static long GetSystemSecond()
	{
		TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
		return (long)timeSpan.TotalSeconds;
	}
}
