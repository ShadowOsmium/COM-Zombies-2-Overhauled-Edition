using UnityEngine;

public class TUITextureInfo : MonoBehaviour
{
	public Rect rect;

	public Material material
	{
		get
		{
			TUIMaterialInfo component = base.transform.parent.GetComponent<TUIMaterialInfo>();
			if (null != component)
			{
				return component.material;
			}
			return null;
		}
	}
}
