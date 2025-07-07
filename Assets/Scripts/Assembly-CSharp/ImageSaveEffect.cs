using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
public class ImageSaveEffect : ImageEffectBase
{
	public RenderTexture targetTexture;

	public GameObject eff_camera;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateBuffers();
		Graphics.Blit(source, targetTexture, base.material);
		Graphics.Blit(source, destination, base.material);
		Shader.SetGlobalTexture("_ScreenImage", targetTexture);
	}

	private void CreateBuffers()
	{
		if (!targetTexture)
		{
			targetTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 0);
			targetTexture.hideFlags = HideFlags.DontSave;
		}
	}

	protected override void OnDisable()
	{
	}

	public void StopEff()
	{
		eff_camera.SetActive(false);
		base.enabled = false;
	}

	public void StartEff()
	{
		eff_camera.SetActive(true);
		base.enabled = true;
	}
}
