using UnityEngine;

public class ParticleItem : PoolItem
{
	public ParticleSystem particle;

	public override void OnItemCreate()
	{
		base.OnItemCreate();
		particle.Stop();
	}

	public override void OnItemActive()
	{
		base.OnItemActive();
		particle.Play();
	}

	public override void OnItemDeactive()
	{
		base.OnItemDeactive();
		particle.Stop();
		particle.Clear();
	}
}
