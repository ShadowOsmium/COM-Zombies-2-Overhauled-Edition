using UnityEngine;

public class InterpAlgorithm
{
	public enum InterpType
	{
		Linear,
		Cubic
	}

	public static Vector3 Interp(InterpType type, Vector3[] pts, float t)
	{
		switch (type)
		{
		case InterpType.Linear:
			return Interp_Linear(pts, t);
		case InterpType.Cubic:
			return Interp_Cubic(pts, t);
		default:
			return Vector3.zero;
		}
	}

	private static Vector3 Interp_Cubic(Vector3[] pts, float t)
	{
		int num = pts.Length - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 vector = pts[num2];
		Vector3 vector2 = pts[num2 + 1];
		Vector3 vector3 = pts[num2 + 2];
		Vector3 vector4 = pts[num2 + 3];
		return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + vector3) * num3 + 2f * vector2);
	}

	private static Vector3 Interp_Linear(Vector3[] pts, float t)
	{
		int num = pts.Length - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 vector = pts[num2 + 1];
		Vector3 vector2 = pts[num2 + 2];
		return (vector2 - vector) * num3 + vector;
	}
}
