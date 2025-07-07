using System;
using UnityEngine;

public class Algorithm
{
	public static float Lerp(TUIAnimation.Method method, float begin, float end, float delta, float time, float deltaTime, float timeLength)
	{
		float num = time / timeLength;
		float num2 = num;
		switch (method)
		{
		case TUIAnimation.Method.Damped:
		{
			float f = (float)Math.E;
			int num3 = 3;
			float num4 = timeLength / (float)(1 + (num3 - 1) * 2);
			float num5 = (0f - delta) * Mathf.Pow(f, -0.5f * time) * Mathf.Cos((float)Math.PI / 2f * time / num4);
			return num5 + end;
		}
		case TUIAnimation.Method.ExponentialOut:
			num2 = 0f - Mathf.Pow(2f, -10f * num / 1f) + 1f;
			break;
		case TUIAnimation.Method.ExponentialIn:
			num2 = Mathf.Pow(2f, 10f * (num / 1f - 1f)) - 0.001f;
			break;
		case TUIAnimation.Method.SineIn:
			num2 = -1f * Mathf.Cos(num * (float)Math.PI * 0.5f) + 1f;
			break;
		case TUIAnimation.Method.SineOut:
			num2 = Mathf.Sin(num * (float)Math.PI * 0.5f);
			break;
		case TUIAnimation.Method.SineInAndOut:
			num2 = -0.5f * (Mathf.Cos((float)Math.PI * num) - 1f);
			break;
		}
		return begin + (end - begin) * num2;
	}
}
