using System.Collections.Generic;
using UnityEngine;

public class ImageSaveEffectManager : MonoBehaviour
{
	public List<GameObject> eff_set = new List<GameObject>();

	private ImageSaveEffect eff;

	private void Start()
	{
		eff = GetComponent<ImageSaveEffect>();
	}

	private void Update()
	{
		if (eff_set.Count > 0 && !eff.enabled)
		{
			eff.StartEff();
		}
		else if (eff_set.Count == 0 && eff.enabled)
		{
			eff.StopEff();
		}
	}
}
