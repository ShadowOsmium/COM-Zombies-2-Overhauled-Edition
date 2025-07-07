using System.Collections;
using UnityEngine;

public class TUICameraTexture : MonoBehaviour
{
	public Camera texture_camera;

	private void Start()
	{
	}

	public void ResetCameraTexture()
	{
		StartCoroutine(SetCameraTexture());
	}

	public IEnumerator SetCameraTexture()
	{
		TUIMeshSprite sprite = base.gameObject.GetComponent<TUIMeshSprite>();
		while (texture_camera == null)
		{
			yield return 1;
		}
		while (texture_camera.targetTexture == null)
		{
			yield return 1;
		}
		RenderTexture renderTexture = FindTexture();
		sprite.CustomizeRect = new Rect(0f, 0f, renderTexture.width, renderTexture.height);
		sprite.CustomizeTexture = renderTexture;
		sprite.ForceUpdate();
	}

	public RenderTexture FindTexture()
	{
		if (texture_camera == null && texture_camera.targetTexture == null)
		{
			return null;
		}
		return texture_camera.targetTexture;
	}
}
