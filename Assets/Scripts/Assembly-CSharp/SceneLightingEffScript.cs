using System.Collections.Generic;
using UnityEngine;

public class SceneLightingEffScript : MonoBehaviour
{
	public GameObject lighting_bar;

	public float ani_rate = 0.1f;

	public List<SceneTUIMeshSprite> lighting_bar_list = new List<SceneTUIMeshSprite>();

	protected float unit_length;

	private float cur_ani_time;

	private void Start()
	{
		unit_length = 213f * base.transform.localScale.y;
	}

	private void Update()
	{
		cur_ani_time += Time.deltaTime;
		if (!(cur_ani_time >= ani_rate))
		{
			return;
		}
		cur_ani_time = 0f;
		foreach (SceneTUIMeshSprite item in lighting_bar_list)
		{
			item.texture = "1000" + (Random.Range(0, 100) % 4 + 1);
		}
	}
}
