using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class TUIMeshCircle : TUINeedUpdateBase
{
	[SerializeField]
	protected bool onlyUseRetinaTexture = true;

	public string m_texture;

	public Color m_color = Color.white;

	public float m_radius = 50f;

	public int m_sectors = 15;

	public int m_show_sectors = 15;

	public Vector2 m_textureOffset;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRender;

	public TUITextureInfo texInfo
	{
		get
		{
			TUI component = base.transform.root.gameObject.GetComponent<TUI>();
			if (null == component || m_texture == null)
			{
				return null;
			}
			if (onlyUseRetinaTexture)
			{
				return component.GetTextureInfo(m_texture, true);
			}
			return component.GetTextureInfo(m_texture);
		}
	}

	public string texture
	{
		set
		{
			if (m_texture != value)
			{
				m_texture = value;
				base.NeedUpdate = true;
			}
		}
	}

	public Color color
	{
		get
		{
			return m_color;
		}
		set
		{
			if (m_color != value)
			{
				base.NeedUpdate = true;
				m_color = value;
			}
		}
	}

	public void Start()
	{
		meshFilter = base.gameObject.GetComponent<MeshFilter>();
		meshRender = base.gameObject.GetComponent<MeshRenderer>();
		meshFilter.sharedMesh = new Mesh();
		meshFilter.sharedMesh.hideFlags = HideFlags.DontSave;
		meshRender.castShadows = false;
		meshRender.receiveShadows = false;
		UpdateMesh();
	}

	private void OnDestroy()
	{
		if ((bool)meshFilter && (bool)meshFilter.sharedMesh)
		{
			UnityEngine.Object.DestroyImmediate(meshFilter.sharedMesh);
		}
	}

	private void LateUpdate()
	{
		UpdateMesh();
	}

	public void ForceUpdate()
	{
		base.NeedUpdate = true;
		UpdateMesh();
	}

	public virtual void UpdateMesh()
	{
		if (meshFilter == null || meshRender == null || !base.NeedUpdate)
		{
			return;
		}
		base.NeedUpdate = false;
		if (null == texInfo || null == texInfo.material || null == texInfo.material.mainTexture)
		{
			meshFilter.sharedMesh.Clear();
			return;
		}
		Material material = texInfo.material;
		Rect rect = texInfo.rect;
		meshRender.sharedMaterial = material;
		float num = (float)material.mainTexture.width * 1f;
		float num2 = (float)material.mainTexture.height * 1f;
		Vector4 vector = new Vector4(rect.xMin / num, 1f - rect.yMin / num2, rect.xMax / num, 1f - rect.yMax / num2);
		Vector2 vector2 = new Vector2((vector.z - vector.x) / 2f, (vector.w - vector.y) / 2f);
		Vector2 vector3 = new Vector2((vector.z + vector.x) / 2f, (vector.w + vector.y) / 2f);
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<Color> list3 = new List<Color>();
		List<int> list4 = new List<int>();
		list.Add(new Vector3(0f, 0f, 0f));
		list2.Add(vector3);
		list3.Add(color);
		float num3 = 360f / (float)m_sectors * ((float)Math.PI / 180f);
		if (m_show_sectors > m_sectors)
		{
			m_show_sectors = m_sectors;
		}
		if (m_show_sectors < 0)
		{
			m_show_sectors = 0;
		}
		for (int i = 0; i < m_show_sectors; i++)
		{
			float f = num3 * (float)i;
			float x = Mathf.Cos(f) * m_radius;
			float y = Mathf.Sin(f) * m_radius;
			list.Add(new Vector3(x, y, 0f));
			list2.Add(vector3 + new Vector2(vector2.x * Mathf.Cos(f), vector2.y * (0f - Mathf.Sin(f))));
			list3.Add(color);
			if (i != 0)
			{
				list4.Add(0);
				list4.Add(list.Count - 1);
				list4.Add(list.Count - 2);
				if (i == m_sectors - 1)
				{
					list4.Add(0);
					list4.Add(1);
					list4.Add(list.Count - 1);
				}
			}
		}
		meshFilter.sharedMesh.Clear();
		meshFilter.sharedMesh.vertices = list.ToArray();
		meshFilter.sharedMesh.uv = list2.ToArray();
		meshFilter.sharedMesh.colors = list3.ToArray();
		meshFilter.sharedMesh.triangles = list4.ToArray();
	}
}
