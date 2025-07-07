using UnityEngine;

public class GameUIAutoScaledLayout : MonoBehaviour
{
	public bool left;

	public bool right;

	public bool top;

	public bool bottom;

	public Vector2 OriginSize;

	private float deltaX;

	private float deltaY;

	private void Start()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		if (Mathf.Max(Screen.width, Screen.height) == 2048)
		{
			num /= 2f;
			num2 /= 2f;
		}
		float num3 = num / 960f;
		float num4 = num2 / 640f;
		base.transform.localScale = new Vector3(base.transform.localScale.x * num3, base.transform.localScale.y * num4, base.transform.localScale.z);
		deltaX = (num - 960f) / 4f - OriginSize.x * (num3 - 1f) / 4f;
		deltaY = (num2 - 640f) / 4f - OriginSize.y * (num4 - 1f) / 4f;
		UpdateLayout();
	}

	public void SetLocalPosition(Vector3 pos, bool mLeft, bool mRight, bool mTop, bool mBottom)
	{
		base.transform.localPosition = pos;
		left = mLeft;
		right = mRight;
		top = mTop;
		bottom = mBottom;
		UpdateLayout();
	}

	public void SetPositon(Vector3 pos, bool mLeft, bool mRight, bool mTop, bool mBottom)
	{
		base.transform.position = pos;
		left = mLeft;
		right = mRight;
		top = mTop;
		bottom = mBottom;
		UpdateLayout();
	}

	private void UpdateLayout()
	{
		float x = 0f;
		float y = 0f;
		if (left)
		{
			x = base.transform.position.x - deltaX;
		}
		if (right)
		{
			x = base.transform.position.x + deltaX;
		}
		if (top)
		{
			y = base.transform.position.y + deltaY;
		}
		if (bottom)
		{
			y = base.transform.position.y - deltaY;
		}
		base.transform.position = new Vector3(x, y, base.transform.position.z);
	}
}
