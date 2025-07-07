using UnityEngine;

public class ExploreEffect : ImageEffectBase
{
	public int screenWidth = 30;

	public int screenHeight = 25;

	public float amplitude;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CustomGraphicsBlit(source, destination);
	}

	private void CustomGraphicsBlit(RenderTexture source, RenderTexture dest)
	{
		RenderTexture.active = dest;
		source.filterMode = FilterMode.Point;
		base.material.SetTexture("_MainTex", source);
		base.material.SetFloat("amplitude", amplitude);
		float num = 1f / (float)screenWidth;
		float num2 = 1f / (float)screenHeight;
		GL.PushMatrix();
		GL.LoadOrtho();
		base.material.SetPass(0);
		GL.Begin(7);
		for (int i = 0; i < screenWidth; i++)
		{
			for (int j = 0; j < screenHeight; j++)
			{
				GL.MultiTexCoord2(0, num * (float)(i + 1), num2 * (float)j);
				GL.Vertex3(num * (float)(i + 1), num2 * (float)j, -1f);
				GL.MultiTexCoord2(0, num * (float)i, num2 * (float)j);
				GL.Vertex3(num * (float)i, num2 * (float)j, -1f);
				GL.MultiTexCoord2(0, num * (float)i, num2 * (float)(j + 1));
				GL.Vertex3(num * (float)i, num2 * (float)(j + 1), -1f);
				GL.MultiTexCoord2(0, num * (float)(i + 1), num2 * (float)(j + 1));
				GL.Vertex3(num * (float)(i + 1), num2 * (float)(j + 1), -1f);
			}
		}
		GL.End();
		GL.PopMatrix();
	}
}
