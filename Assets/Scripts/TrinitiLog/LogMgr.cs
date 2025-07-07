using System.Diagnostics;
using UnityEngine;

public class LogMgr
{
	private static LogMgr sInstance;

	public static LogMgr Instance()
	{
		return sInstance;
	}

	[Conditional("T_LOG")]
	public static void Log(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("T_LOG")]
	public static void Log(object message, Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	[Conditional("T_LOG")]
	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
	}

	[Conditional("T_LOG")]
	public static void LogError(object message, Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}

	[Conditional("T_LOG")]
	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	[Conditional("T_LOG")]
	public static void LogWarning(object message, Object context)
	{
		UnityEngine.Debug.LogWarning(message, context);
	}
}
