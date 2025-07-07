using UnityEngine;

public class RenderToTextureSetting : MonoBehaviour
{
	private Camera m_camera;

	public Vector2 TextureSize = new Vector2(256f, 256f);

	public RenderTexture rt;

	private void Start()
	{
	}

	public void StartRender()
	{
		m_camera = base.gameObject.GetComponent<Camera>();
		if (m_camera == null)
		{
			Debug.LogError("No Camera!");
		}
		RenderTextureFormat format = RenderTextureFormat.ARGB1555;
		if (!SystemInfo.SupportsRenderTextureFormat(format))
		{
			format = RenderTextureFormat.Default;
		}
		rt = new RenderTexture(Mathf.FloorToInt(TextureSize.x), Mathf.FloorToInt(TextureSize.y), 24, format);
		m_camera.targetTexture = rt;
		m_camera.aspect = (float)rt.width / (float)rt.height;
	}
}
