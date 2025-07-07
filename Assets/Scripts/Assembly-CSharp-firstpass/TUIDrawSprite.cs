using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TUIDrawSprite : MonoBehaviour
{
	public enum Clipping
	{
		None,
		HardClip
	}

	public Material material;

	public Clipping clippingType;

	public TUIRect clippingRect;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRenderer;

	private Material sharedMat;

	private Material clipedMat;

	private Clipping lastClippingType;

	private Bounds bounds = default(Bounds);

	private bool isDirty = true;

	public Bounds Bounds
	{
		get
		{
			RecalculateBounds();
			return bounds;
		}
	}

	public void Draw(List<Vector3> vertices, List<int> triangles, List<Vector2> uv)
	{
		Initialize();
		meshFilter.sharedMesh.Clear();
		meshFilter.sharedMesh.vertices = vertices.ToArray();
		meshFilter.sharedMesh.triangles = triangles.ToArray();
		meshFilter.sharedMesh.uv = uv.ToArray();
		isDirty = true;
	}

	public void Draw(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, List<Color> colors)
	{
		Draw(vertices, triangles, uv);
		meshFilter.sharedMesh.colors = colors.ToArray();
	}

	public void SetColorBK(Color color)
	{
		if (Application.isPlaying && null != meshRenderer)
		{
			meshRenderer.sharedMaterial.SetColor("_ColorB", color);
		}
	}

	private void Initialize()
	{
		if (null == meshFilter)
		{
			meshFilter = GetComponent<MeshFilter>();
			meshFilter.sharedMesh = new Mesh();
		}
		if (null == meshRenderer)
		{
			meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.castShadows = false;
			meshRenderer.receiveShadows = false;
		}
		if (sharedMat != material)
		{
			sharedMat = material;
		}
		SwitchMaterial();
	}

	private void RecalculateBounds()
	{
		if (null != meshFilter && null != meshFilter.sharedMesh && isDirty)
		{
			meshFilter.sharedMesh.RecalculateBounds();
			bounds = meshFilter.sharedMesh.bounds;
			isDirty = false;
		}
	}

	private void OnDestroy()
	{
		if (null != meshFilter && null != meshFilter.sharedMesh)
		{
			Object.DestroyImmediate(meshFilter.sharedMesh);
		}
		if (null != clipedMat)
		{
			Object.DestroyImmediate(clipedMat);
		}
	}

	private void Update()
	{
		SetClippingRect();
	}

	private void SwitchMaterial()
	{
		if (!(null != meshRenderer))
		{
			return;
		}
		Clipping clipping = clippingType;
		if (clipping == Clipping.HardClip)
		{
			if (null == clipedMat)
			{
				clipedMat = new Material(sharedMat);
				clipedMat.shader = Shader.Find("Unlit/Transparent Colored TwoTexture Vertex Color(HardClip)");
			}
			meshRenderer.sharedMaterial = clipedMat;
		}
		else
		{
			meshRenderer.sharedMaterial = sharedMat;
		}
	}

	public void SetClippingRect()
	{
		if (clippingType != lastClippingType)
		{
			SwitchMaterial();
			lastClippingType = clippingType;
		}
		if (clippingType != 0 && null != meshRenderer && null != clippingRect && null != clipedMat)
		{
			clipedMat.SetVector("_Rect", clippingRect.GetRectViewPort());
		}
	}
}
