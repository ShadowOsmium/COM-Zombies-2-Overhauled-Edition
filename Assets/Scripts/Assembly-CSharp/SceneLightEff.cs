using UnityEngine;

public class SceneLightEff : MonoBehaviour
{
	public float max_val = 3f;

	public float mim_val;

	public float speed = 1f;

	private bool is_bright;

	private void Start()
	{
		if (!GameData.IsHighEffect() && base.GetComponent<Light>() == null)
		{
			base.GetComponent<Light>().enabled = false;
		}
	}

	private void Update()
	{
		if (!GameData.IsHighEffect() || base.GetComponent<Light>() == null)
		{
			return;
		}
		if (is_bright)
		{
			base.GetComponent<Light>().intensity += speed * Time.deltaTime;
			if (base.GetComponent<Light>().intensity >= max_val)
			{
				base.GetComponent<Light>().intensity = max_val;
				is_bright = false;
			}
		}
		else
		{
			base.GetComponent<Light>().intensity -= speed * Time.deltaTime;
			if (base.GetComponent<Light>().intensity <= mim_val)
			{
				base.GetComponent<Light>().intensity = mim_val;
				is_bright = true;
			}
		}
	}
}
