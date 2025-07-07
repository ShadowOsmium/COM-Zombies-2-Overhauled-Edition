using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TUI/Control/Polygon")]
[RequireComponent(typeof(TUIDrawSprite))]
public class TUIPolygon : TUIControl
{
	[Serializable]
	public class VertexData
	{
		public float angle;

		public float length;

		public Color color;
	}

	public bool around;

	[SerializeField]
	protected Shader shader;

	public Vector3 center = Vector3.zero;

	public Color centerColor = Color.white;

	public Vector3 datumLine = Vector3.up;

	public Vector3 normalLine = Vector3.forward;

	[SerializeField]
	protected VertexData[] vertices;

	private List<float> lengths = new List<float>();

	private TUIDrawSprite drawSprite;

	private TUIGeometry geometry = new TUIGeometry();

	private Vector3 DatumLine
	{
		get
		{
			return base.transform.rotation * datumLine;
		}
	}

	private Vector3 NormalLine
	{
		get
		{
			return base.transform.rotation * normalLine;
		}
	}

	private void Awake()
	{
		if (vertices != null)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				lengths.Add(1f);
			}
		}
		drawSprite = GetComponent<TUIDrawSprite>();
		drawSprite.material = new Material(shader);
	}

	private void Start()
	{
		Draw();
	}

	public bool SetLength(int index, float length, bool immediately = true)
	{
		if (index >= lengths.Count || index < 0)
		{
			return false;
		}
		float num = Mathf.Clamp01(length);
		if (lengths[index] != num)
		{
			lengths[index] = num;
			if (immediately)
			{
				UpdateDraw(index);
			}
		}
		return true;
	}

	public bool GetLength(int index, out float length)
	{
		if (index >= lengths.Count || index < 0)
		{
			length = 0f;
			return false;
		}
		length = lengths[index];
		return true;
	}

	public void OnDrawGizmos()
	{
		if (vertices != null)
		{
			VertexData[] array = vertices;
			foreach (VertexData vertexData in array)
			{
				Gizmos.color = vertexData.color;
				Vector3 vector = center + base.transform.position;
				Gizmos.DrawLine(vector, VertexData2Vector(vertexData, DatumLine, NormalLine).normalized * vertexData.length + vector);
			}
		}
		Gizmos.color = Color.red;
		Gizmos.DrawLine(center + base.transform.position, DatumLine + base.transform.position);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(center + base.transform.position, NormalLine + base.transform.position);
	}

	private Vector3 VertexData2Vector(VertexData vd, Vector3 datumLine, Vector3 normalLine)
	{
		return Quaternion.AngleAxis(vd.angle, normalLine) * datumLine;
	}

	private void Draw()
	{
		geometry.Clear();
		geometry.Vertices.Add(center);
		geometry.Colors.Add(centerColor);
		if (vertices != null)
		{
			int num = 0;
			VertexData[] array = vertices;
			foreach (VertexData vertexData in array)
			{
				Vector3 vector = VertexData2Vector(vertexData, datumLine, normalLine);
				geometry.Vertices.Add(vector.normalized * vertexData.length * lengths[num++]);
				geometry.Colors.Add(vertexData.color);
			}
			List<VertexData> list = new List<VertexData>(vertices);
			List<VertexData> list2 = new List<VertexData>();
			foreach (VertexData item in list)
			{
				list2.Add(item);
			}
			list.Sort(delegate(VertexData v1, VertexData v2)
			{
				float num2 = v1.angle % 360f;
				float num3 = v2.angle % 360f;
				if (num2 < 0f)
				{
					num2 += 360f;
				}
				if (num3 < 0f)
				{
					num3 += 360f;
				}
				if (num2 > num3)
				{
					return 1;
				}
				return (num2 < num3) ? (-1) : 0;
			});
			if (around)
			{
				list.Add(list[0]);
			}
			List<int> list3 = new List<int>();
			foreach (VertexData item2 in list)
			{
				list3.Add(list2.IndexOf(item2));
			}
			for (int j = 0; j < list3.Count - 1; j++)
			{
				geometry.Triangles.Add(0);
				geometry.Triangles.Add(list3[j] + 1);
				geometry.Triangles.Add(list3[j + 1] + 1);
			}
		}
		drawSprite.Draw(geometry.Vertices, geometry.Triangles, geometry.Uv, geometry.Colors);
	}

	private void UpdateDraw(int index)
	{
		if (index < geometry.Vertices.Count)
		{
			Vector3 vector = VertexData2Vector(vertices[index], datumLine, normalLine);
			geometry.Vertices[index + 1] = vector.normalized * vertices[index].length * lengths[index];
			drawSprite.Draw(geometry.Vertices, geometry.Triangles, geometry.Uv, geometry.Colors);
		}
	}

	public void UpdateDraw()
	{
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = VertexData2Vector(vertices[i], datumLine, normalLine);
			geometry.Vertices[i + 1] = vector.normalized * vertices[i].length * lengths[i];
		}
		drawSprite.Draw(geometry.Vertices, geometry.Triangles, geometry.Uv, geometry.Colors);
	}
}
