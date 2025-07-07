using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class EffectAudioBehaviour : TAudioController
{
	public string m_AudioName = string.Empty;

	public float m_DeltaTime;

	public bool m_DeleteWhenEnd;

	public bool player_on_awake = true;

	public void Awake()
	{
		if (player_on_awake)
		{
			PlayEffect();
		}
	}

	public void start()
	{
	}

	public void OnDestroy()
	{
	}

	public void Update()
	{
	}

	private IEnumerator PlaySfxDelay()
	{
		yield return new WaitForSeconds(m_DeltaTime);
		PlayAudio(m_AudioName);
	}

	public void PlayEffect()
	{
		if (m_AudioName != string.Empty)
		{
			StartCoroutine(PlaySfxDelay());
		}
	}

	public void StopEffect()
	{
		GameObject gameObject = null;
		Transform transform = base.transform.Find("Audio/" + m_AudioName);
		AudioControlSelfBehaviour @object = null;
		if ((bool)transform)
		{
			gameObject = transform.gameObject;
			if ((bool)gameObject)
			{
				@object = gameObject.AddComponent<AudioControlSelfBehaviour>();
			}
		}
		if (m_DeleteWhenEnd)
		{
			TAudioAuxFade component = transform.GetComponent<TAudioAuxFade>();
			if (component != null)
			{
				component.StartCoroutine(component.FadeOut(@object.FadeOut));
			}
			else
			{
				StopAudio(m_AudioName);
			}
		}
		if ((bool)gameObject)
		{
			gameObject.transform.parent = null;
		}
	}
}
