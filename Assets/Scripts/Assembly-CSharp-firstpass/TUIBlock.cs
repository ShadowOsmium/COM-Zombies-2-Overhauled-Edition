using UnityEngine;

[AddComponentMenu("TUI/Control/Block")]
public class TUIBlock : TUIControlImpl
{
	public bool m_bEnable = true;

	public override bool HandleInput(TUIInput input)
	{
		if (base.HandleInput(input))
		{
			return true;
		}
		if (!m_bEnable)
		{
			return false;
		}
		if (PtInControl(input.position))
		{
			return true;
		}
		return false;
	}
}
