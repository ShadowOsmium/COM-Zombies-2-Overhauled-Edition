using System;
using UnityEngine;

public class ShaderColorFlash : MonoBehaviour
{
	public float flash_interval = 1f;

	private float cur_time;

	private bool enable_flash;

	private bool reverse = true;

	public string color_name = "_Color";

	public Color start_color = Color.white;

	public Color end_color = Color.black;

	private Color cur_color = Color.black;

	public Action on_flash_end;

	private void Start()
	{
	}

	private void Update()
	{
		if (!enable_flash)
		{
			return;
		}
		cur_time += Time.deltaTime;
		cur_color = base.GetComponent<Renderer>().material.GetColor(color_name);
		if (reverse)
		{
			cur_color.r = Mathf.Lerp(end_color.r, start_color.r, cur_time / flash_interval);
			cur_color.g = Mathf.Lerp(end_color.g, start_color.g, cur_time / flash_interval);
			cur_color.b = Mathf.Lerp(end_color.b, start_color.b, cur_time / flash_interval);
			cur_color.a = Mathf.Lerp(end_color.a, start_color.a, cur_time / flash_interval);
			if (cur_time >= flash_interval)
			{
				reverse = false;
				cur_time = 0f;
			}
		}
		else
		{
			cur_color.r = Mathf.Lerp(start_color.r, end_color.r, cur_time / flash_interval);
			cur_color.g = Mathf.Lerp(start_color.g, end_color.g, cur_time / flash_interval);
			cur_color.b = Mathf.Lerp(start_color.b, end_color.b, cur_time / flash_interval);
			cur_color.a = Mathf.Lerp(start_color.a, end_color.a, cur_time / flash_interval);
			if (cur_time >= flash_interval)
			{
				reverse = true;
				cur_time = 0f;
			}
		}
		base.GetComponent<Renderer>().material.SetColor(color_name, cur_color);
	}

	private void OnDestroy()
	{
		on_flash_end = null;
	}

	public void StartFlash(float flash_time, Action flash_end)
	{
		enable_flash = true;
		cur_time = 0f;
		on_flash_end = flash_end;
		base.GetComponent<Renderer>().material.SetColor(color_name, start_color);
		Invoke("StopFlash", flash_time);
	}

	public void StopFlash()
	{
		enable_flash = false;
		if (on_flash_end != null)
		{
			on_flash_end();
		}
	}
}
