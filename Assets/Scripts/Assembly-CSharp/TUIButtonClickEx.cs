using UnityEngine;

public class TUIButtonClickEx : TUIButton
{
	public const int CommandDown = 1;

	public const int CommandUp = 2;

	public const int CommandClick = 3;

	public const string DownMethod = "OnDown";

	public const string UpMethod = "OnUp";

	public const string ClickMethod = "OnClick";

	public float MoveThreshold = 3f;

	private Vector2 lastInput = Vector2.zero;

	private bool closed;

	public override bool HandleInput(TUIInput input)
	{
		if (m_bDisable)
		{
			return false;
		}
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				m_bPressed = true;
				m_iFingerId = input.fingerId;
				Show();
				PostEvent(this, 1, 0f, 0f, null);
				lastInput = input.position;
			}
			return false;
		}
		if (input.fingerId == m_iFingerId)
		{
			if (input.inputType == TUIInputType.Moved)
			{
				if (Vector2.Distance(input.position, lastInput) < MoveThreshold)
				{
					if (!m_bPressed)
					{
						m_bPressed = true;
						Show();
						PostEvent(this, 1, 0f, 0f, null);
					}
				}
				else
				{
					if (m_bPressed)
					{
						m_bPressed = false;
						Show();
						PostEvent(this, 2, 0f, 0f, null);
					}
					closed = true;
				}
			}
			else if (input.inputType == TUIInputType.Ended)
			{
				m_bPressed = false;
				m_iFingerId = -1;
				if (!closed)
				{
					if (PtInControl(input.position))
					{
						Show();
						PostEvent(this, 2, 0f, 0f, null);
						PostEvent(this, 3, 0f, 0f, null);
					}
					else
					{
						Show();
						PostEvent(this, 2, 0f, 0f, null);
					}
				}
				closed = false;
			}
		}
		return false;
	}

	public override void PostEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		string text;
		switch (eventType)
		{
		case 2:
			text = "OnUp";
			break;
		case 1:
			text = "OnDown";
			break;
		case 3:
			text = "OnClick";
			break;
		default:
			text = null;
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			PostMessage(text, null, SendMessageOptions.DontRequireReceiver);
		}
		base.PostEvent(control, eventType, wparam, lparam, data);
	}
}
