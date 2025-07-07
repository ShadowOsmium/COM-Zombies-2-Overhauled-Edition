using UnityEngine;

public class ImageScale : MonoBehaviour
{
	public float scale_to = 5f;

	public float speed = 2f;

	public bool is_live;

	private void Start()
	{
	}

	private void Update()
	{
		if (is_live)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x + speed * Time.deltaTime, base.transform.localScale.y + speed * Time.deltaTime, base.transform.localScale.z + speed * Time.deltaTime);
			if (base.transform.localScale.x >= scale_to)
			{
				StopEff();
			}
		}
	}

	public void StartEff()
	{
		base.transform.localScale = Vector3.zero;
		is_live = true;
	}

	public void StopEff()
	{
		base.transform.localScale = Vector3.zero;
		is_live = false;
		Camera.main.GetComponent<ImageSaveEffectManager>().eff_set.Remove(base.gameObject);
	}
}
