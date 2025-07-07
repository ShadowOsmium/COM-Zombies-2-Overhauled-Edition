using UnityEngine;

public class BoomFireLight : MonoBehaviour
{
	public float fire_light_decay = 5f;

	public float fire_light_limit = 3f;

	public float fire_light_range = 20f;

	private void Start()
	{
		base.GetComponent<Light>().intensity = fire_light_limit;
		base.GetComponent<Light>().range = fire_light_range;
	}

	private void Update()
	{
		if (base.GetComponent<Light>().enabled && !(base.GetComponent<Light>().intensity <= 0.1f))
		{
			base.GetComponent<Light>().intensity -= Time.deltaTime * fire_light_decay;
			if (base.GetComponent<Light>().intensity <= 0.1f)
			{
				base.GetComponent<Light>().enabled = false;
			}
		}
	}

	public void Reset()
	{
		base.GetComponent<Light>().enabled = true;
		base.GetComponent<Light>().intensity = fire_light_limit;
		base.GetComponent<Light>().range = fire_light_range;
	}
}
