using UnityEngine;

[AddComponentMenu("TUI/Control/Wheel Button")]
public class TUIButtonWheel : TUIButton
{
	public const int CommandDown = 1;

	public const int CommandRotate = 2;

	public const int CommandUp = 3;

	public const string DownMethod = "OnDown";

	public const string UpMethod = "OnUp";

	public const string RotateMethod = "OnUp";

	private float m_fLastAngle;

	public override void Reset()
	{
		base.Reset();
		m_bPressed = false;
		Show();
	}

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
				float x = input.position.x - base.transform.position.x;
				float y = input.position.y - base.transform.position.y;
				m_fLastAngle = Mathf.Atan2(y, x);
				Show();
				PostEvent(this, 1, 0f, 0f, null);
				return true;
			}
			return false;
		}
		if (input.fingerId == m_iFingerId)
		{
			if (input.inputType == TUIInputType.Moved)
			{
				float x2 = input.position.x - base.transform.position.x;
				float y2 = input.position.y - base.transform.position.y;
				float num = Mathf.Atan2(y2, x2);
				float num2 = num - m_fLastAngle;
				m_fLastAngle = num;
				Vector3 localEulerAngles = base.transform.localEulerAngles;
				localEulerAngles += new Vector3(0f, 0f, 57.29578f * num2);
				base.transform.localEulerAngles = localEulerAngles;
				PostEvent(this, 2, num2, 0f, null);
			}
			else if (input.inputType == TUIInputType.Ended)
			{
				m_bPressed = false;
				m_iFingerId = -1;
				Show();
				PostEvent(this, 3, 0f, 0f, null);
			}
		}
		return false;
	}

	public override void PostEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		string text;
		switch (eventType)
		{
		case 3:
			text = "OnUp";
			break;
		case 1:
			text = "OnDown";
			break;
		case 2:
			text = "OnUp";
			break;
		default:
			text = null;
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			PostMessage(text, (eventType != 2) ? null : ((object)wparam), SendMessageOptions.DontRequireReceiver);
		}
		base.PostEvent(control, eventType, wparam, lparam, data);
	}
}
