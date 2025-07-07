using UnityEngine;

[AddComponentMenu("TUI/Control/Push Button")]
public class TUIButtonPush : TUIButton
{
	public const int CommandDown = 1;

	public const int CommandUp = 2;

	public const string DownMethod = "OnDown";

	public const string UpMethod = "OnUp";

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
				m_iFingerId = input.fingerId;
				return true;
			}
			return false;
		}
		if (input.fingerId == m_iFingerId)
		{
			if (input.inputType == TUIInputType.Ended)
			{
				m_iFingerId = -1;
				if (PtInControl(input.position))
				{
					if (m_bPressed)
					{
						m_bPressed = false;
						Show();
						PostEvent(this, 2, 0f, 0f, null);
					}
					else
					{
						m_bPressed = true;
						Show();
						PostEvent(this, 1, 0f, 0f, null);
					}
				}
			}
			return true;
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
