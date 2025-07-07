using UnityEngine;

public class BoomBloodItem : PoolItem
{
	private ParticleSystem blood;

	protected override void Awake()
	{
		base.Awake();
		blood = base.transform.Find("blood_bomb").GetComponent<ParticleSystem>();
	}

	public override void OnItemCreate()
	{
		base.OnItemCreate();
		blood.Clear();
		blood.Stop();
	}

	public override void OnItemActive()
	{
		base.OnItemActive();
		blood.Clear();
		blood.Play();
	}

	public override void OnItemDeactive()
	{
		base.OnItemDeactive();
		blood.Stop();
		blood.Clear();
	}
}
