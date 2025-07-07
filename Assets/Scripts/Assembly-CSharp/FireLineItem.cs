using System.Collections;
using UnityEngine;

public class FireLineItem : PoolItem
{
	private ParticleSystem fire_line;

	private GameObject trail_obj;

	protected override void Awake()
	{
		base.Awake();
		fire_line = base.GetComponent<ParticleSystem>();
		trail_obj = base.transform.Find("bullet_02").gameObject;
	}

	public override void OnItemCreate()
	{
		base.OnItemCreate();
		fire_line.Stop();
		StartCoroutine(TrailRendererClean());
	}

	public override void OnItemActive()
	{
		base.OnItemActive();
		fire_line.Play();
		StartCoroutine(TrailRendererStart());
	}

	public override void OnItemDeactive()
	{
		base.OnItemDeactive();
		fire_line.Stop();
		StartCoroutine(TrailRendererClean());
	}

	private IEnumerator TrailRendererClean()
	{
		((TrailRenderer)trail_obj.GetComponent<Renderer>()).time = 0f;
		yield return 1;
		trail_obj.SetActive(false);
	}

	private IEnumerator TrailRendererStart()
	{
		((TrailRenderer)trail_obj.GetComponent<Renderer>()).time = 0f;
		trail_obj.SetActive(true);
		yield return 1;
		((TrailRenderer)trail_obj.GetComponent<Renderer>()).time = 1.3f;
	}
}
