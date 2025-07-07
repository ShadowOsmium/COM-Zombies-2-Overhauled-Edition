using System;
using System.Collections;
using UnityEngine;

public class CameraFadeEvent : MonoBehaviour, IRoamEvent
{
	public bool isFadeIn;

	public float fadeInTime;

	public bool isFadeOut;

	public float fadeOutTime;

	public Action on_fadeout_end;

	public void OnRoamTrigger()
	{
		if (isFadeIn)
		{
			CameraFade.CameraFadeIn(fadeInTime);
		}
	}

	public void OnRoamStop()
	{
		if (isFadeOut)
		{
			CameraFade.CameraFadeOut(fadeOutTime);
			StartCoroutine("OnFadeOutEnd");
		}
	}

	private IEnumerator OnFadeOutEnd()
	{
		yield return new WaitForSeconds(fadeOutTime);
		if (on_fadeout_end != null)
		{
			on_fadeout_end();
		}
		yield return 1;
		CameraFade.Clear();
	}

	private void OnDestroy()
	{
		on_fadeout_end = null;
	}
}
