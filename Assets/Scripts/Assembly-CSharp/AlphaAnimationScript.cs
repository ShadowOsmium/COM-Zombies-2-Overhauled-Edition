using UnityEngine;

public class AlphaAnimationScript : MonoBehaviour
{
	public float maxAlpha = 1f;

	public float minAlpha;

	public float animationSpeed = 5.5f;

	public float maxBright = 1f;

	public float minBright;

	public bool enableAlphaAnimation;

	public bool enableBrightAnimation;

	public string colorPropertyName = "_TintColor";

	protected float alpha;

	protected float startTime;

	protected bool increasing = true;

	public Color startColor = Color.yellow;

	protected float lastUpdateTime;

	protected float deltaTime;

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.02f)
		{
			return;
		}
		Color color = Color.white;
		if (enableAlphaAnimation || enableBrightAnimation)
		{
			color = base.GetComponent<Renderer>().material.GetColor(colorPropertyName);
		}
		if (enableAlphaAnimation)
		{
			if (increasing)
			{
				color.a += animationSpeed * deltaTime;
				color.a = Mathf.Clamp(color.a, minAlpha, maxAlpha);
				if (color.a == maxAlpha)
				{
					increasing = false;
				}
			}
			else
			{
				color.a -= animationSpeed * deltaTime;
				color.a = Mathf.Clamp(color.a, minAlpha, maxAlpha);
				if (color.a == minAlpha)
				{
					increasing = true;
				}
			}
		}
		if (enableBrightAnimation)
		{
			if (increasing)
			{
				color.r += animationSpeed * deltaTime;
				color.g += animationSpeed * deltaTime;
				color.b += animationSpeed * deltaTime;
				if (color.r >= maxBright || color.g >= maxBright || color.b >= maxBright)
				{
					increasing = false;
				}
			}
			else
			{
				color.r -= animationSpeed * deltaTime;
				color.g -= animationSpeed * deltaTime;
				color.b -= animationSpeed * deltaTime;
				if (color.r <= minBright || color.g <= minBright || color.b <= minBright)
				{
					increasing = true;
				}
			}
		}
		base.GetComponent<Renderer>().material.SetColor(colorPropertyName, color);
		deltaTime = 0f;
	}
}
