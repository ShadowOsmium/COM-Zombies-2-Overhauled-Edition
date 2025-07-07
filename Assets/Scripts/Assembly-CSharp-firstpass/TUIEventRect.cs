using UnityEngine;

public class TUIEventRect : TUIControlImpl
{
	public const int CommandBegin = 1;

	public const int CommandMove = 2;

	public const int CommandEnd = 3;

	public float minX;

	public float minY;

	protected int fingerId = -1;

	protected Vector2 position = Vector2.zero;

	public OnInputHandle on_touch_event;

	public override bool HandleInput(TUIInput input)
	{
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				PostEvent(this, 1, 0f, 0f, input);
				if (on_touch_event != null)
				{
					on_touch_event(this, 1, 0f, 0f, input);
				}
				fingerId = input.fingerId;
				position = input.position;
				return true;
			}
			return false;
		}
		if (input.inputType == TUIInputType.Moved || input.inputType == TUIInputType.Stationary)
		{
			float wparam = input.position.x - position.x;
			float lparam = input.position.y - position.y;
			position = input.position;
			PostEvent(this, 2, wparam, lparam, input);
			if (on_touch_event != null)
			{
				on_touch_event(this, 2, wparam, lparam, input);
			}
		}
		else if (input.inputType == TUIInputType.Ended || input.inputType == TUIInputType.Canceled)
		{
			fingerId = -1;
			position = Vector2.zero;
			PostEvent(this, 3, 0f, 0f, input);
			if (on_touch_event != null)
			{
				on_touch_event(this, 3, 0f, 0f, input);
			}
		}
		return false;
	}

	private void Start()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		if (num == 2048f || num2 == 2048f)
		{
			num /= 2f;
			num2 /= 2f;
		}
		num = 1136f;
		num2 = 640f;
		num = (num - 960f) / 2f;
		num2 = (num2 - 640f) / 2f;
		size = new Vector2(size.x + num, size.y + num2);
	}
}
