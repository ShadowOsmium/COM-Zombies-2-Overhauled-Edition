using UnityEngine;

[AddComponentMenu("TUI/Control/Click Button")]
public class TUIButtonClick : TUIButton
{
	public const int CommandDown = 1;

	public const int CommandUp = 2;

	public const int CommandClick = 3;

	public const string DownMethod = "OnDown";

	public const string UpMethod = "OnUp";

	public const string ClickMethod = "OnClick";

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
				return true;
			}
			return false;
		}
		if (input.fingerId == m_iFingerId)
		{
			if (input.inputType == TUIInputType.Moved)
			{
				if (PtInControl(input.position))
				{
					if (!m_bPressed)
					{
						m_bPressed = true;
						Show();
						PostEvent(this, 1, 0f, 0f, null);
					}
				}
				else if (m_bPressed)
				{
					m_bPressed = false;
					Show();
					PostEvent(this, 2, 0f, 0f, null);
				}
			}
			else if (input.inputType == TUIInputType.Ended)
			{
				m_bPressed = false;
				m_iFingerId = -1;
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

 /*   public void SetGrayscale(bool gray)
    {
        Transform stateNormalTransform = transform.Find("StateNormal");
        Transform statePressTransform = transform.Find("StatePress");

        TUIMeshSprite normalSprite = null;
        TUIMeshSprite pressSprite = null;

        if (stateNormalTransform != null)
            normalSprite = stateNormalTransform.GetComponent<TUIMeshSprite>();

        if (statePressTransform != null)
            pressSprite = statePressTransform.GetComponent<TUIMeshSprite>();

        if (normalSprite == null)
            normalSprite = FindSpriteByTextureName("button_1_n");

        if (pressSprite == null)
            pressSprite = FindSpriteByTextureName("button_1_n");

        if (normalSprite != null)
            normalSprite.GrayStyle = gray;

        if (pressSprite != null)
            pressSprite.GrayStyle = gray;
    }

    private TUIMeshSprite FindSpriteByTextureName(string textureName)
    {
        TUIMeshSprite[] sprites = GetComponentsInChildren<TUIMeshSprite>(true);
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].texture == textureName)
                return sprites[i];
        }
        return null;
    }*/
}
