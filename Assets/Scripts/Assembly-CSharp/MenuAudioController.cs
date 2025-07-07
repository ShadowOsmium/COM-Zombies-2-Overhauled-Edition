using System.Collections;
using UnityEngine;

public class MenuAudioController : MonoBehaviour
{
	private static MenuAudioController instance;

	private TAudioController audio_controller;

	private AudioSource audio_source;

	private bool is_bypassEffects;

	public static MenuAudioController Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private IEnumerator Start()
	{
		CheckAudioBypassStatus();
		audio_controller = base.gameObject.AddComponent<TAudioController>();
		audio_controller.PlayAudio("MusicMenu");
		Object.DontDestroyOnLoad(base.gameObject);
		audio_source = base.transform.Find("Audio/MusicMenu").GetComponent<AudioSource>();
		yield return 1;
		SetAudioBypassStatus();
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public static void CheckGameMenuAudio()
	{
		if (!GameObject.Find("MenuAudioObj"))
		{
			GameObject gameObject = new GameObject("MenuAudioObj");
			gameObject.transform.parent = null;
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.AddComponent<MenuAudioController>();
		}
		else
		{
			Instance.CheckAudioBypassStatus();
			Instance.SetAudioBypassStatus();
		}
	}

	public static void DestroyGameMenuAudio()
	{
		if (Instance != null)
		{
			Object.Destroy(Instance.gameObject);
		}
	}

	public void SetAudioBypassStatus()
	{
		audio_source.bypassEffects = is_bypassEffects;
	}

	private void CheckAudioBypassStatus()
	{
		if (Application.loadedLevelName == "UIMap")
		{
			is_bypassEffects = true;
		}
		else if (Application.loadedLevelName == "UIShop")
		{
			is_bypassEffects = false;
		}
		else if (Application.loadedLevelName == "GameCover")
		{
			is_bypassEffects = true;
		}
	}
}
