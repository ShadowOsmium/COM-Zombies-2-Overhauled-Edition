using UnityEngine;

public class CameraFade : MonoBehaviour
{
	protected int cameraFadeDepth = 999999;

	protected static GameObject cameraFade;

	protected static CameraFade fade;

	protected Color fadeInColor = new Color(0.5f, 0.5f, 0.5f, 0f);

	protected Color fadeOutColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	protected float m_time;

	protected bool isfadein;

	protected bool isfadeout;

	protected float lasttime;

	public void Init()
	{
		base.transform.position = new Vector3(0f, 0f, cameraFadeDepth);
		base.transform.localScale = Vector3.zero;
		base.gameObject.AddComponent<GUITexture>();
		base.GetComponent<GUITexture>().texture = CameraTexture(Color.black);
		base.GetComponent<GUITexture>().color = fadeInColor;
		base.GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
	}

	protected Texture2D CameraTexture(Color color)
	{
		Texture2D texture2D = new Texture2D(16, 16, TextureFormat.ARGB32, false);
		Color[] array = new Color[Screen.width * Screen.height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	public static void CameraFadeOut(float time)
	{
		Check();
		fade.FadeOut(time);
	}

	public static void CameraFadeIn(float time)
	{
		Check();
		fade.FadeIn(time);
	}

	public static void Clear()
	{
		if (cameraFade != null)
		{
			Object.Destroy(cameraFade);
			cameraFade = null;
			fade = null;
		}
	}

	protected static void Check()
	{
		if (cameraFade == null)
		{
			cameraFade = new GameObject("CameraFade");
			fade = cameraFade.AddComponent<CameraFade>();
			fade.Init();
		}
	}

	protected void FadeOut(float time)
	{
		m_time = time;
		isfadeout = true;
		isfadein = false;
		lasttime = 0f;
	}

	protected void FadeIn(float time)
	{
		m_time = time;
		isfadein = true;
		isfadeout = false;
		lasttime = 0f;
	}

	private void Update()
	{
		if (isfadeout || isfadein)
		{
			lasttime += Time.deltaTime;
			if (isfadeout)
			{
				base.GetComponent<GUITexture>().color = Color.Lerp(fadeInColor, fadeOutColor, Mathf.Clamp01(lasttime / m_time));
			}
			else if (isfadein)
			{
				base.GetComponent<GUITexture>().color = Color.Lerp(fadeOutColor, fadeInColor, Mathf.Clamp01(lasttime / m_time));
			}
			if (lasttime >= m_time)
			{
				isfadein = false;
				isfadeout = false;
				lasttime = 0f;
				m_time = 0f;
			}
		}
	}

	private void OnDestroy()
	{
		cameraFade = null;
		fade = null;
	}
}
