using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
[AddComponentMenu("TUI/Control/Mesh Sprite Sliced")]
[RequireComponent(typeof(MeshFilter))]
public class TUIMeshSpriteSliced : TUINeedUpdateBase
{
	[SerializeField]
	protected bool onlyUseRetinaTexture = true;

	public string m_texture;

	public Color m_color = Color.white;

	private static string grayShader = "Triniti/TUI/TUIGrayStyle";

	[SerializeField]
	protected bool grayStyle;

	[SerializeField]
	protected bool useCustomize;

	[SerializeField]
	protected Texture customizeTexture;

	[SerializeField]
	protected Rect customizeRect;

	[SerializeField]
	protected Vector2 size = Vector2.one;

	[SerializeField]
	protected int borderLeft;

	[SerializeField]
	protected int borderRight;

	[SerializeField]
	protected int borderTop;

	[SerializeField]
	protected int borderBottom;

	[SerializeField]
	private bool fillCenter = true;

	protected Material customizeMaterial;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRender;

	private Material sharedMat;

	private Material grayMat;

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

	public float alpha
	{
		get
		{
			return m_color.a;
		}
		set
		{
			if (m_color.a != value)
			{
				base.NeedUpdate = true;
				m_color.a = value;
			}
		}
	}

	public bool UseCustomize
	{
		get
		{
			return useCustomize;
		}
		set
		{
			if (value != useCustomize)
			{
				useCustomize = value;
				base.NeedUpdate = true;
			}
		}
	}

	public bool GrayStyle
	{
		get
		{
			return grayStyle;
		}
		set
		{
			if (grayStyle != value)
			{
				grayStyle = value;
				SetGray(grayStyle);
			}
		}
	}

	public Texture CustomizeTexture
	{
		get
		{
			return customizeTexture;
		}
		set
		{
			if (value != customizeTexture)
			{
				customizeTexture = value;
				base.NeedUpdate = true;
			}
		}
	}

	public Rect CustomizeRect
	{
		get
		{
			return customizeRect;
		}
		set
		{
			if (value != customizeRect)
			{
				customizeRect = value;
				base.NeedUpdate = true;
			}
		}
	}

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			if (value != size)
			{
				size = value;
				base.NeedUpdate = true;
			}
		}
	}

	public bool FillCenter
	{
		get
		{
			return fillCenter;
		}
		set
		{
			if (value != fillCenter)
			{
				fillCenter = value;
				base.NeedUpdate = true;
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
			Object.DestroyImmediate(meshFilter.sharedMesh);
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

	protected virtual void UpdateMesh()
	{
		if (meshFilter == null || meshRender == null || !base.NeedUpdate)
		{
			return;
		}
		base.NeedUpdate = false;
		Material material;
		Rect rect;
		if (useCustomize)
		{
			if (null == customizeTexture)
			{
				meshFilter.sharedMesh.Clear();
				return;
			}
			if (null == customizeMaterial)
			{
				customizeMaterial = TUITool.CreateUITextureMaterial();
			}
			customizeMaterial.mainTexture = customizeTexture;
			material = customizeMaterial;
			rect = customizeRect;
		}
		else
		{
			if (null == texInfo || null == texInfo.material || null == texInfo.material.mainTexture)
			{
				meshFilter.sharedMesh.Clear();
				return;
			}
			material = texInfo.material;
			rect = texInfo.rect;
		}
		sharedMat = material;
		meshRender.sharedMaterial = sharedMat;
		SetGray(GrayStyle);
		float num = (float)material.mainTexture.width * 1f;
		float num2 = (float)material.mainTexture.height * 1f;
		Vector2[] array = new Vector2[8];
		Vector2[] array2 = new Vector2[8];
		Rect rect2 = rect;
		Rect rect3 = default(Rect);
		rect3.xMin = rect2.xMin + (float)borderLeft;
		rect3.yMin = rect2.yMin + (float)borderTop;
		rect3.xMax = rect2.xMax - (float)borderRight;
		rect3.yMax = rect2.yMax - (float)borderBottom;
		Rect rect4 = ConvertToTexCoords(rect2, (int)num, (int)num2);
		Rect rect5 = ConvertToTexCoords(rect3, (int)num, (int)num2);
		array2[0] = new Vector2(rect4.xMin, rect4.yMax);
		array2[1] = new Vector2(rect5.xMin, rect5.yMax);
		array2[2] = new Vector2(rect4.xMax, rect4.yMin);
		array2[3] = new Vector2(rect5.xMax, rect5.yMin);
		array2[4] = new Vector2(rect4.xMax, rect4.yMax);
		array2[5] = new Vector2(rect5.xMax, rect5.yMax);
		array2[6] = new Vector2(rect4.xMin, rect4.yMin);
		array2[7] = new Vector2(rect5.xMin, rect5.yMin);
		array[0] = new Vector2((0f - size.x) * 0.5f, size.y * 0.5f);
		array[1] = new Vector2(array[0].x + (float)borderLeft, array[0].y - (float)borderTop);
		array[2] = new Vector2(size.x * 0.5f, (0f - size.y) * 0.5f);
		array[3] = new Vector2(array[2].x - (float)borderRight, array[2].y + (float)borderBottom);
		array[4] = new Vector2(array[0].x + size.x, array[0].y);
		array[5] = new Vector2(array[4].x - (float)borderRight, array[4].y - (float)borderTop);
		array[6] = new Vector2(array[2].x - size.x, array[2].y);
		array[7] = new Vector2(array[6].x + (float)borderLeft, array[6].y + (float)borderBottom);
		if (useCustomize || onlyUseRetinaTexture || TUI.IsRetina())
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] *= 0.5f;
			}
		}
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<Color> list3 = new List<Color>();
		List<int> list4 = new List<int>();
		list.Add(new Vector2(array[1].x, array[0].y));
		list.Add(array[1]);
		list.Add(new Vector2(array[0].x, array[1].y));
		list.Add(array[0]);
		list2.Add(new Vector2(array2[1].x, array2[0].y));
		list2.Add(array2[1]);
		list2.Add(new Vector2(array2[0].x, array2[1].y));
		list2.Add(array2[0]);
		list.Add(array[1]);
		list.Add(array[7]);
		list.Add(new Vector2(array[6].x, array[7].y));
		list.Add(new Vector2(array[6].x, array[1].y));
		list2.Add(array2[1]);
		list2.Add(array2[7]);
		list2.Add(new Vector2(array2[6].x, array2[7].y));
		list2.Add(new Vector2(array2[6].x, array2[1].y));
		list.Add(array[7]);
		list.Add(new Vector2(array[7].x, array[6].y));
		list.Add(array[6]);
		list.Add(new Vector2(array[6].x, array[7].y));
		list2.Add(array2[7]);
		list2.Add(new Vector2(array2[7].x, array2[6].y));
		list2.Add(array2[6]);
		list2.Add(new Vector2(array2[6].x, array2[7].y));
		list.Add(array[3]);
		list.Add(new Vector2(array[3].x, array[2].y));
		list.Add(new Vector2(array[7].x, array[2].y));
		list.Add(array[7]);
		list2.Add(array2[3]);
		list2.Add(new Vector2(array2[3].x, array2[2].y));
		list2.Add(new Vector2(array2[7].x, array2[2].y));
		list2.Add(array2[7]);
		list.Add(new Vector2(array[2].x, array[3].y));
		list.Add(array[2]);
		list.Add(new Vector2(array[3].x, array[2].y));
		list.Add(array[3]);
		list2.Add(new Vector2(array2[2].x, array2[3].y));
		list2.Add(array2[2]);
		list2.Add(new Vector2(array2[3].x, array2[2].y));
		list2.Add(array2[3]);
		list.Add(new Vector2(array[4].x, array[5].y));
		list.Add(new Vector2(array[4].x, array[3].y));
		list.Add(array[3]);
		list.Add(array[5]);
		list2.Add(new Vector2(array2[4].x, array2[5].y));
		list2.Add(new Vector2(array2[4].x, array2[3].y));
		list2.Add(array2[3]);
		list2.Add(array2[5]);
		list.Add(array[4]);
		list.Add(new Vector2(array[4].x, array[5].y));
		list.Add(array[5]);
		list.Add(new Vector2(array[5].x, array[4].y));
		list2.Add(array2[4]);
		list2.Add(new Vector2(array2[4].x, array2[5].y));
		list2.Add(array2[5]);
		list2.Add(new Vector2(array2[5].x, array2[4].y));
		list.Add(new Vector2(array[5].x, array[0].y));
		list.Add(array[5]);
		list.Add(array[1]);
		list.Add(new Vector2(array[1].x, array[0].y));
		list2.Add(new Vector2(array2[5].x, array2[0].y));
		list2.Add(array2[5]);
		list2.Add(array2[1]);
		list2.Add(new Vector2(array2[1].x, array2[0].y));
		if (fillCenter)
		{
			list.Add(array[5]);
			list.Add(array[3]);
			list.Add(array[7]);
			list.Add(array[1]);
			list2.Add(array2[5]);
			list2.Add(array2[3]);
			list2.Add(array2[7]);
			list2.Add(array2[1]);
		}
		for (int j = 0; j < list.Count; j += 4)
		{
			list3.Add(m_color);
			list3.Add(m_color);
			list3.Add(m_color);
			list3.Add(m_color);
			list4.Add(j);
			list4.Add(j + 1);
			list4.Add(j + 2);
			list4.Add(j);
			list4.Add(j + 2);
			list4.Add(j + 3);
		}
		meshFilter.sharedMesh.Clear();
		meshFilter.sharedMesh.vertices = list.ToArray();
		meshFilter.sharedMesh.uv = list2.ToArray();
		meshFilter.sharedMesh.colors = list3.ToArray();
		meshFilter.sharedMesh.triangles = list4.ToArray();
	}

	private void SetGray(bool gray)
	{
		if (!(null != meshRender) || !Application.isPlaying)
		{
			return;
		}
		if (gray)
		{
			meshRender.sharedMaterial = new Material(sharedMat);
			if (null != meshRender.sharedMaterial)
			{
				meshRender.sharedMaterial.shader = Shader.Find(grayShader);
			}
		}
		else
		{
			meshRender.sharedMaterial = sharedMat;
		}
	}

	public static Rect ConvertToTexCoords(Rect rect, int width, int height)
	{
		Rect result = rect;
		if ((float)width != 0f && (float)height != 0f)
		{
			result.xMin = rect.xMin / (float)width;
			result.xMax = rect.xMax / (float)width;
			result.yMin = 1f - rect.yMax / (float)height;
			result.yMax = 1f - rect.yMin / (float)height;
		}
		return result;
	}
}
