using UnityEngine;

public class MissionIconEffect : MonoBehaviour
{
	public bool updown = true;

	public bool brightness;

	public bool frameBrightness;

	public bool zoom;

	public float speed = 200f;

	public float minValue;

	public float maxValue = 10f;

	private float deltaTime;

	private bool positiveDirection = true;

	private float startValue;

	public OnFrameBrightnessMax onFrameBrightnessMax;

	public OnFrameBrightnessMin onFrameBrightnessMin;

	private void Start()
	{
		if (updown)
		{
			startValue = base.transform.localPosition.y;
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if ((double)deltaTime < 0.02)
		{
			return;
		}
		if (updown)
		{
			float y = base.transform.localPosition.y;
			if (positiveDirection)
			{
				y += deltaTime * speed;
				y = Mathf.Clamp(y, startValue + minValue, startValue + maxValue);
				if (y == startValue + maxValue)
				{
					positiveDirection = false;
				}
			}
			else
			{
				y -= deltaTime * speed;
				y = Mathf.Clamp(y, startValue + minValue, startValue + maxValue);
				if (y == startValue + minValue)
				{
					positiveDirection = true;
				}
			}
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, y, base.transform.localPosition.z);
		}
		if (brightness)
		{
			float a = base.GetComponent<Renderer>().material.GetColor("_Color").a;
			if (positiveDirection)
			{
				a += deltaTime * speed;
				a = Mathf.Clamp(a, minValue, maxValue);
				if (a == maxValue)
				{
					positiveDirection = false;
				}
			}
			else
			{
				a -= deltaTime * speed;
				a = Mathf.Clamp(a, minValue, maxValue);
				if (a == minValue)
				{
					positiveDirection = true;
				}
			}
			base.GetComponent<Renderer>().material.SetColor("_Color", new Color(a, a, a, a));
		}
		if (frameBrightness)
		{
			float a2 = base.gameObject.GetComponent<TUIMeshSprite>().color.a;
			if (positiveDirection)
			{
				a2 += deltaTime * speed;
				a2 = Mathf.Clamp(a2, minValue, maxValue);
				if (a2 == maxValue)
				{
					positiveDirection = false;
					if (onFrameBrightnessMax != null)
					{
						onFrameBrightnessMax();
					}
				}
			}
			else
			{
				a2 -= deltaTime * speed;
				a2 = Mathf.Clamp(a2, minValue, maxValue);
				if (a2 == minValue)
				{
					positiveDirection = true;
					if (onFrameBrightnessMin != null)
					{
						onFrameBrightnessMin();
					}
				}
			}
			base.gameObject.GetComponent<TUIMeshSprite>().color = new Color(1f, 1f, 1f, a2);
		}
		if (zoom)
		{
			float x = base.transform.localScale.x;
			float y2 = base.transform.localScale.y;
			if (positiveDirection)
			{
				x += Time.deltaTime * speed;
				y2 += Time.deltaTime * speed;
				x = Mathf.Clamp(x, minValue, maxValue);
				y2 = Mathf.Clamp(y2, minValue, maxValue);
				if (x == maxValue)
				{
					positiveDirection = false;
					if (onFrameBrightnessMax != null)
					{
						onFrameBrightnessMax();
					}
				}
			}
			else
			{
				x -= Time.deltaTime * speed;
				y2 -= Time.deltaTime * speed;
				x = Mathf.Clamp(x, minValue, maxValue);
				y2 = Mathf.Clamp(y2, minValue, maxValue);
				if (x == minValue)
				{
					positiveDirection = true;
					if (onFrameBrightnessMin != null)
					{
						onFrameBrightnessMin();
					}
				}
			}
			base.transform.localScale = new Vector3(x, y2, base.transform.localScale.z);
		}
		deltaTime = 0f;
	}
}
