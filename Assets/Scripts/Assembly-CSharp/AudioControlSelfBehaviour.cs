using UnityEngine;

public class AudioControlSelfBehaviour : MonoBehaviour
{
	private float deltaTime;

	public void Awake()
	{
	}

	public void OnDestroy()
	{
	}

	public void FadeOut()
	{
		Object.Destroy(base.gameObject);
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		if (!(deltaTime >= 1f))
		{
			return;
		}
		TAudioEffectRandom component = base.gameObject.GetComponent<TAudioEffectRandom>();
		if (component != null && !component.isPlaying)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		TAudioEffectSequence component2 = base.gameObject.GetComponent<TAudioEffectSequence>();
		if (component2 != null && !component2.isPlaying)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		TAudioEffectTogether component3 = base.gameObject.GetComponent<TAudioEffectTogether>();
		if (component3 != null && !component3.isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
