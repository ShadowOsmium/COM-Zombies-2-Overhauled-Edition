using TNetSdk;
using UnityEngine;

public class TNetConnection : MonoBehaviour
{
	private static TNetConnection mInstance;

	private static TNetObject tnetObj;

	public static bool IsServer;

	public static TNetObject Connection
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("TNetConnection").AddComponent(typeof(TNetConnection)) as TNetConnection;
			}
			return tnetObj;
		}
		set
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("TNetConnection").AddComponent(typeof(TNetConnection)) as TNetConnection;
			}
			tnetObj = value;
		}
	}

	public static bool IsInitialized
	{
		get
		{
			return tnetObj != null;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void UnregisterSceneCallbacks()
	{
		if (tnetObj != null)
		{
			tnetObj.RemoveAllEventListeners();
		}
	}

	public static void Disconnect()
	{
		tnetObj = null;
	}
}
