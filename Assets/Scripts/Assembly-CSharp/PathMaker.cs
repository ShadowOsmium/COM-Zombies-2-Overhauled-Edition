using System;
using UnityEngine;

public class PathMaker
{
	public static void DrawPath(InterpAlgorithm.InterpType type, Vector3[] path, Color color)
	{
		if (path.Length > 0)
		{
			DrawPathHelper(type, path, color);
		}
	}

	private static void DrawPathHelper(InterpAlgorithm.InterpType type, Vector3[] path, Color color)
	{
		Vector3[] pts = PathControlPointGenerator(path);
		Vector3 to = InterpAlgorithm.Interp(type, pts, 0f);
		Gizmos.color = color;
		int num = path.Length * 20;
		for (int i = 1; i <= num; i++)
		{
			float t = (float)i / (float)num;
			Vector3 vector = InterpAlgorithm.Interp(type, pts, t);
			Gizmos.DrawLine(vector, to);
			to = vector;
		}
	}

	public static Vector3[] PathControlPointGenerator(Vector3[] path)
	{
		int num = 2;
		Vector3[] array = new Vector3[path.Length + num];
		Array.Copy(path, 0, array, 1, path.Length);
		array[0] = array[1] + (array[1] - array[2]);
		array[array.Length - 1] = array[array.Length - 2] + (array[array.Length - 2] - array[array.Length - 3]);
		if (array[1] == array[array.Length - 2])
		{
			Vector3[] array2 = new Vector3[array.Length];
			Array.Copy(array, array2, array.Length);
			array2[0] = array2[array2.Length - 3];
			array2[array2.Length - 1] = array2[2];
			array = new Vector3[array2.Length];
			Array.Copy(array2, array, array2.Length);
		}
		return array;
	}
}
