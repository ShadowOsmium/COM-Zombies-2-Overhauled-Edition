using UnityEngine;

public class BoomItem : PoolItem
{
	public ParticleSystem boom;

	public GameObject eff_test;

	public override void OnItemCreate()
	{
		base.OnItemCreate();
		boom.Stop();
	}

	public override void OnItemActive()
	{
		base.OnItemActive();
		boom.Play();
		GetComponent<BoomFireLight>().Reset();
		if (eff_test != null)
		{
			eff_test.GetComponent<ImageScale>().StartEff();
			Camera.main.GetComponent<ImageSaveEffectManager>().eff_set.Add(eff_test);
		}
	}

	public override void OnItemDeactive()
	{
		base.OnItemDeactive();
		boom.Stop();
	}
}
