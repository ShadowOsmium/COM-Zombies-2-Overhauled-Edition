using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class TUIMeshSector : TUINeedUpdateBase
{
	[SerializeField]
	protected bool onlyUseRetinaTexture = true;

	public string m_texture;

	public Color m_color = Color.white;

	public bool m_flipX;

	public bool m_flipY;

	[SerializeField]
	protected bool useCustomize;

	[SerializeField]
	protected Texture customizeTexture;

	[SerializeField]
	protected Rect customizeRect;

	protected Material customizeMaterial;

	private string baseShader = string.Empty;

	private static string grayShader = "Triniti/TUI/TUIGrayStyle";

	[SerializeField]
	protected bool grayStyle;

	public float m_beginAngle;

	public float m_rotateAngle;

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

	public bool flipX
	{
		get
		{
			return m_flipX;
		}
		set
		{
			if (m_flipX != value)
			{
				base.NeedUpdate = true;
				m_flipX = value;
			}
		}
	}

	public bool flipY
	{
		get
		{
			return m_flipY;
		}
		set
		{
			if (m_flipY != value)
			{
				base.NeedUpdate = true;
				m_flipY = value;
			}
		}
	}

	public float BeginAngle
	{
		get
		{
			return m_beginAngle;
		}
		set
		{
			if (m_beginAngle != value)
			{
				base.NeedUpdate = true;
				m_beginAngle = value;
			}
		}
	}

	public float RotateAngle
	{
		get
		{
			return m_rotateAngle;
		}
		set
		{
			if (m_rotateAngle != value)
			{
				base.NeedUpdate = true;
				m_rotateAngle = value;
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
		baseShader = material.shader.name;
		SetGray(GrayStyle);
		float num = (float)material.mainTexture.width * 1f;
		float num2 = (float)material.mainTexture.height * 1f;
		float num3 = rect.width;
		float num4 = rect.height;
		if (useCustomize || onlyUseRetinaTexture || TUI.IsRetina())
		{
			num3 /= 2f;
			num4 /= 2f;
		}
		float num5 = num3;
		float num6 = num4;
		Vector4 texUV = new Vector4(rect.xMin / num, 1f - rect.yMin / num2, rect.xMax / num, 1f - rect.yMax / num2);
		if (flipX)
		{
			float x = texUV.x;
			texUV.x = texUV.z;
			texUV.z = x;
		}
		if (flipY)
		{
			float y = texUV.y;
			texUV.y = texUV.w;
			texUV.w = y;
		}
		while (m_beginAngle < 0f)
		{
			m_beginAngle += 360f;
		}
		while (m_beginAngle > 360f)
		{
			m_beginAngle -= 360f;
		}
		float num7 = m_beginAngle - m_rotateAngle;
		bool flag = true;
		if (m_rotateAngle == 0f)
		{
			flag = false;
		}
		for (; num7 < 0f; num7 += 360f)
		{
		}
		while (num7 > 360f)
		{
			num7 -= 360f;
		}
		Vector3[] array = new Vector3[4]
		{
			new Vector3(num5 * -0.5f, num6 * 0.5f, 0f),
			new Vector3(num5 * 0.5f, num6 * 0.5f, 0f),
			new Vector3(num5 * 0.5f, num6 * -0.5f, 0f),
			new Vector3(num5 * -0.5f, num6 * -0.5f, 0f)
		};
		Vector2[] array2 = new Vector2[4]
		{
			new Vector2(texUV.x, texUV.y),
			new Vector2(texUV.z, texUV.y),
			new Vector2(texUV.z, texUV.w),
			new Vector2(texUV.x, texUV.w)
		};
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<Color> list3 = new List<Color>();
		List<int> list4 = new List<int>();
		float fAngleFactor = Mathf.Atan2(num6, num5) * 57.29578f;
		Vector3 point = Vector3.zero;
		Vector2 uv = Vector2.zero;
		int anglePoint = GetAnglePoint(m_beginAngle, fAngleFactor, num5, num6, texUV, ref point, ref uv);
		Vector3 point2 = Vector3.zero;
		Vector2 uv2 = Vector2.zero;
		int anglePoint2 = GetAnglePoint(num7, fAngleFactor, num5, num6, texUV, ref point2, ref uv2);
		list.Add(Vector3.zero);
		list2.Add(new Vector2((texUV.x + texUV.z) * 0.5f, (texUV.y + texUV.w) * 0.5f));
		list3.Add(m_color);
		list.Add(array[0]);
		list2.Add(array2[0]);
		list3.Add(m_color);
		list.Add(array[1]);
		list2.Add(array2[1]);
		list3.Add(m_color);
		list.Add(array[2]);
		list2.Add(array2[2]);
		list3.Add(m_color);
		list.Add(array[3]);
		list2.Add(array2[3]);
		list3.Add(m_color);
		list.Add(point);
		list2.Add(uv);
		list3.Add(m_color);
		list.Add(point2);
		list2.Add(uv2);
		list3.Add(m_color);
		switch (anglePoint2)
		{
		case 0:
			switch (anglePoint)
			{
			case 0:
				if (flag && point2.y >= point.y)
				{
					list4.Add(0);
					list4.Add(5);
					list4.Add(6);
					break;
				}
				list4.Add(0);
				list4.Add(3);
				list4.Add(6);
				list4.Add(0);
				list4.Add(4);
				list4.Add(3);
				list4.Add(0);
				list4.Add(1);
				list4.Add(4);
				list4.Add(0);
				list4.Add(2);
				list4.Add(1);
				list4.Add(0);
				list4.Add(5);
				list4.Add(2);
				break;
			case 1:
				list4.Add(0);
				list4.Add(3);
				list4.Add(6);
				list4.Add(0);
				list4.Add(4);
				list4.Add(3);
				list4.Add(0);
				list4.Add(1);
				list4.Add(4);
				list4.Add(0);
				list4.Add(5);
				list4.Add(1);
				break;
			case 2:
				list4.Add(0);
				list4.Add(3);
				list4.Add(6);
				list4.Add(0);
				list4.Add(4);
				list4.Add(3);
				list4.Add(0);
				list4.Add(5);
				list4.Add(4);
				break;
			case 3:
				list4.Add(0);
				list4.Add(3);
				list4.Add(6);
				list4.Add(0);
				list4.Add(5);
				list4.Add(3);
				break;
			}
			break;
		case 1:
			switch (anglePoint)
			{
			case 0:
				list4.Add(0);
				list4.Add(2);
				list4.Add(6);
				list4.Add(0);
				list4.Add(5);
				list4.Add(2);
				break;
			case 1:
				if (flag && point2.x <= point.x)
				{
					list4.Add(0);
					list4.Add(5);
					list4.Add(6);
					break;
				}
				list4.Add(0);
				list4.Add(2);
				list4.Add(6);
				list4.Add(0);
				list4.Add(3);
				list4.Add(2);
				list4.Add(0);
				list4.Add(4);
				list4.Add(3);
				list4.Add(0);
				list4.Add(1);
				list4.Add(4);
				list4.Add(0);
				list4.Add(1);
				list4.Add(5);
				break;
			case 2:
				list4.Add(0);
				list4.Add(2);
				list4.Add(6);
				list4.Add(0);
				list4.Add(3);
				list4.Add(2);
				list4.Add(0);
				list4.Add(4);
				list4.Add(3);
				list4.Add(0);
				list4.Add(5);
				list4.Add(4);
				break;
			case 3:
				list4.Add(0);
				list4.Add(2);
				list4.Add(6);
				list4.Add(0);
				list4.Add(3);
				list4.Add(2);
				list4.Add(0);
				list4.Add(5);
				list4.Add(3);
				break;
			}
			break;
		case 2:
			switch (anglePoint)
			{
			case 0:
				list4.Add(0);
				list4.Add(1);
				list4.Add(6);
				list4.Add(0);
				list4.Add(2);
				list4.Add(1);
				list4.Add(0);
				list4.Add(5);
				list4.Add(2);
				break;
			case 1:
				list4.Add(0);
				list4.Add(1);
				list4.Add(6);
				list4.Add(0);
				list4.Add(5);
				list4.Add(1);
				break;
			case 2:
				if (flag && point2.y <= point.y)
				{
					list4.Add(0);
					list4.Add(5);
					list4.Add(6);
					break;
				}
				list4.Add(0);
				list4.Add(1);
				list4.Add(6);
				list4.Add(0);
				list4.Add(2);
				list4.Add(1);
				list4.Add(0);
				list4.Add(3);
				list4.Add(2);
				list4.Add(0);
				list4.Add(4);
				list4.Add(3);
				list4.Add(0);
				list4.Add(5);
				list4.Add(4);
				break;
			case 3:
				list4.Add(0);
				list4.Add(1);
				list4.Add(6);
				list4.Add(0);
				list4.Add(2);
				list4.Add(1);
				list4.Add(0);
				list4.Add(3);
				list4.Add(2);
				list4.Add(0);
				list4.Add(5);
				list4.Add(3);
				break;
			}
			break;
		case 3:
			switch (anglePoint)
			{
			case 0:
				list4.Add(0);
				list4.Add(4);
				list4.Add(6);
				list4.Add(0);
				list4.Add(1);
				list4.Add(4);
				list4.Add(0);
				list4.Add(2);
				list4.Add(1);
				list4.Add(0);
				list4.Add(5);
				list4.Add(2);
				break;
			case 1:
				list4.Add(0);
				list4.Add(4);
				list4.Add(6);
				list4.Add(0);
				list4.Add(1);
				list4.Add(4);
				list4.Add(0);
				list4.Add(5);
				list4.Add(1);
				break;
			case 2:
				list4.Add(0);
				list4.Add(4);
				list4.Add(6);
				list4.Add(0);
				list4.Add(5);
				list4.Add(4);
				break;
			case 3:
				if (flag && point2.x >= point.x)
				{
					list4.Add(0);
					list4.Add(5);
					list4.Add(6);
					break;
				}
				list4.Add(0);
				list4.Add(4);
				list4.Add(6);
				list4.Add(0);
				list4.Add(1);
				list4.Add(4);
				list4.Add(0);
				list4.Add(2);
				list4.Add(1);
				list4.Add(0);
				list4.Add(3);
				list4.Add(2);
				list4.Add(0);
				list4.Add(5);
				list4.Add(3);
				break;
			}
			break;
		}
		meshFilter.sharedMesh.Clear();
		meshFilter.sharedMesh.vertices = list.ToArray();
		meshFilter.sharedMesh.uv = list2.ToArray();
		meshFilter.sharedMesh.colors = list3.ToArray();
		meshFilter.sharedMesh.triangles = list4.ToArray();
	}

	private int GetAnglePoint(float angle, float fAngleFactor, float controlWidth, float controlHeight, Vector4 texUV, ref Vector3 point, ref Vector2 uv)
	{
		int num = 0;
		float[] array = new float[4]
		{
			fAngleFactor,
			180f - fAngleFactor,
			180f + fAngleFactor,
			360f - fAngleFactor
		};
		if (array[0] <= angle && angle < array[1])
		{
			point.y = controlHeight * 0.5f;
			point.x = point.y / Mathf.Tan(angle * ((float)Math.PI / 180f));
			uv.y = texUV.y;
			uv.x = Mathf.Lerp(texUV.x, texUV.z, (point.x + controlWidth * 0.5f) / controlWidth);
			return 1;
		}
		if (array[1] <= angle && angle < array[2])
		{
			point.x = (0f - controlWidth) * 0.5f;
			point.y = Mathf.Tan(angle * ((float)Math.PI / 180f)) * point.x;
			uv.x = texUV.x;
			uv.y = Mathf.Lerp(texUV.w, texUV.y, (point.y + controlHeight * 0.5f) / controlHeight);
			return 2;
		}
		if (array[2] <= angle && angle < array[3])
		{
			point.y = (0f - controlHeight) * 0.5f;
			point.x = point.y / Mathf.Tan(angle * ((float)Math.PI / 180f));
			uv.y = texUV.w;
			uv.x = Mathf.Lerp(texUV.x, texUV.z, (point.x + controlWidth * 0.5f) / controlWidth);
			return 3;
		}
		point.x = controlWidth * 0.5f;
		point.y = Mathf.Tan(angle * ((float)Math.PI / 180f)) * point.x;
		uv.x = texUV.z;
		uv.y = Mathf.Lerp(texUV.w, texUV.y, (point.y + controlHeight * 0.5f) / controlHeight);
		return 0;
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
}
