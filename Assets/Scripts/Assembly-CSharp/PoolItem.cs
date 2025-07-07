using UnityEngine;

public class PoolItem : MonoBehaviour
{
	protected EffectAudioBehaviour effect_audio;

	protected virtual void Awake()
	{
		effect_audio = GetComponent<EffectAudioBehaviour>();
	}

	protected virtual void Start()
	{
	}

	private void Update()
	{
	}

	public virtual void OnItemCreate()
	{
	}

	public virtual void OnItemActive()
	{
		if (effect_audio != null)
		{
			effect_audio.PlayEffect();
		}
	}

	public virtual void OnItemDeactive()
	{
	}
}
