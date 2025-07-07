using UnityEngine;

[RequireComponent(typeof(TUIRect))]
public class TUISetTUIRectCamera : MonoBehaviour
{
	private void Awake()
	{
		TUIRect component = GetComponent<TUIRect>();
		if (null == component.currentCamera)
		{
			TUI component2 = base.transform.root.GetComponent<TUI>();
			if (null != component2)
			{
				component.currentCamera = component2.Camera.GetComponent<Camera>();
			}
		}
	}
}
