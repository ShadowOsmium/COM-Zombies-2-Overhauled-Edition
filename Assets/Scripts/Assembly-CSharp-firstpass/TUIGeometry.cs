using System.Collections.Generic;
using UnityEngine;

public class TUIGeometry
{
	protected List<Vector3> vertices = new List<Vector3>();

	protected List<int> triangles = new List<int>();

	protected List<Vector2> uv = new List<Vector2>();

	protected List<Color> colors = new List<Color>();

	protected Bounds bounds;

	public Bounds Bounds
	{
		get
		{
			return bounds;
		}
	}

	public List<Vector3> Vertices
	{
		get
		{
			return vertices;
		}
	}

	public List<int> Triangles
	{
		get
		{
			return triangles;
		}
	}

	public List<Vector2> Uv
	{
		get
		{
			return uv;
		}
	}

	public List<Color> Colors
	{
		get
		{
			return colors;
		}
	}

	public void RecalculateBounds()
	{
		Vector3 min = Vector3.zero;
		Vector3 max = Vector3.zero;
		if (vertices.Count > 0)
		{
			min = vertices[0];
			max = vertices[0];
			foreach (Vector3 vertex in vertices)
			{
				if (vertex.x < min.x)
				{
					min.x = vertex.x;
				}
				else if (vertex.x > max.x)
				{
					max.x = vertex.x;
				}
				if (vertex.y < min.y)
				{
					min.y = vertex.y;
				}
				else if (vertex.y > max.y)
				{
					max.y = vertex.y;
				}
				if (vertex.z < min.z)
				{
					min.z = vertex.z;
				}
				else if (vertex.z > max.z)
				{
					max.z = vertex.z;
				}
			}
		}
		bounds.SetMinMax(min, max);
	}

	public void ReadIn(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, List<Color> colors)
	{
		this.vertices.AddRange(vertices);
		this.triangles.AddRange(triangles);
		this.uv.AddRange(uv);
		this.colors.AddRange(colors);
	}

	public void ReReadIn(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, List<Color> colors)
	{
		Clear();
		ReadIn(vertices, triangles, uv, colors);
	}

	public void WriteOut(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, List<Color> colors)
	{
		vertices.AddRange(this.vertices);
		triangles.AddRange(this.triangles);
		uv.AddRange(this.uv);
		colors.AddRange(this.colors);
	}

	public void TRS(Matrix4x4 trs)
	{
		if (!trs.isIdentity)
		{
			for (int i = 0; i < vertices.Count; i++)
			{
				vertices[i] = trs.MultiplyPoint3x4(vertices[i]);
			}
			RecalculateBounds();
		}
	}

	public void Clear()
	{
		vertices.Clear();
		triangles.Clear();
		uv.Clear();
		colors.Clear();
	}
}
