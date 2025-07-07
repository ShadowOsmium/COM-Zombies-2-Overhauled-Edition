using UnityEngine;

public class TUISelfAdaptiveAnchor : MonoBehaviour
{
	public bool lock568x384 = true;

	private void Start()
	{
		Anchor();
	}

	public void Anchor()
	{
		CastAnchor(base.transform.root.GetComponent<TUI>().Camera, base.transform, lock568x384);
	}

	public static void CastAnchor(TUICamera tCamera, Transform transform, bool lock568x384 = true)
	{
		Camera component = tCamera.GetComponent<Camera>();
		Rect rect = new Rect(0f, 0f, component.orthographicSize * component.aspect * 2f, component.orthographicSize * 2f);
		Vector3 one = Vector3.one;
		if (lock568x384)
		{
			if (rect.width > 568f)
			{
				rect.width = 568f;
			}
			if (rect.height > 384f)
			{
				rect.height = 384f;
			}
		}
		one.x = rect.width / 480f;
		one.y = rect.height / 320f;
		if (one.x < 1f)
		{
			one.x = 1f;
		}
		if (one.y < 1f)
		{
			one.y = 1f;
		}
		transform.localPosition = new Vector3(transform.localPosition.x * one.x, transform.localPosition.y * one.y, transform.localPosition.z * one.z);
	}
}
