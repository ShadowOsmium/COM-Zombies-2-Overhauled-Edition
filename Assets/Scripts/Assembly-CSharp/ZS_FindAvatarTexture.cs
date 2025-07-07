using UnityEngine;

public class ZS_FindAvatarTexture : MonoBehaviour
{
	private void Start()
	{
		TUIMeshSprite component = base.gameObject.GetComponent<TUIMeshSprite>();
		RenderTexture renderTexture = FindTexture();
		component.CustomizeRect = new Rect(0f, 0f, renderTexture.width, renderTexture.height);
		component.CustomizeTexture = renderTexture;
		component.ForceUpdate();
	}

	public RenderTexture FindTexture()
	{
		Camera component = GameObject.Find("AvatarCamera").GetComponent<Camera>();
		if (component == null && component.targetTexture == null)
		{
			return null;
		}
		return component.targetTexture;
	}
}
