using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TUI/Control/Union Button")]
public class TUIUnionButton : TUIControl
{
	public const int CommandDown = 1;

	public const int CommandUp = 2;

	public const int CommandChangeArea = 3;

	public const string DownMethod = "OnDown";

	public const string UpMethod = "OnUp";

	public const string ChangeAreaMethod = "OnChangeArea";

	protected bool m_IsPressed;

	private int m_CurrentButtonID = -1;

	private int m_FingerId = -1;

	[SerializeField]
	protected List<TUIButton> buttons;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override bool HandleInput(TUIInput input)
	{
		bool result = false;
		if (input.inputType == TUIInputType.Began)
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				if (buttons[i].PtInControl(input.position))
				{
					m_IsPressed = true;
					m_CurrentButtonID = i;
					m_FingerId = input.fingerId;
					buttons[m_CurrentButtonID].m_bPressed = true;
					buttons[m_CurrentButtonID].Show();
					PostEvent(this, 1, m_CurrentButtonID, 0f, null);
					result = true;
				}
			}
		}
		else if (input.inputType == TUIInputType.Moved)
		{
			if (input.fingerId == m_FingerId)
			{
				for (int j = 0; j < buttons.Count; j++)
				{
					if (buttons[j].PtInControl(input.position))
					{
						if (j != m_CurrentButtonID)
						{
							buttons[m_CurrentButtonID].m_bPressed = false;
							buttons[m_CurrentButtonID].Show();
							PostEvent(this, 3, m_CurrentButtonID, j, null);
							m_CurrentButtonID = j;
							buttons[m_CurrentButtonID].m_bPressed = true;
							buttons[m_CurrentButtonID].Show();
							PostEvent(this, 1, m_CurrentButtonID, 0f, null);
						}
					}
					else
					{
						buttons[m_CurrentButtonID].m_bPressed = false;
					}
				}
				result = true;
			}
		}
		else if (input.inputType == TUIInputType.Ended && input.fingerId == m_FingerId)
		{
			if (m_CurrentButtonID != -1)
			{
				buttons[m_CurrentButtonID].m_bPressed = false;
				buttons[m_CurrentButtonID].Show();
			}
			m_IsPressed = false;
			m_FingerId = -1;
			PostEvent(this, 2, m_CurrentButtonID, 0f, null);
			result = true;
		}
		return result;
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
			text = "OnChangeArea";
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
