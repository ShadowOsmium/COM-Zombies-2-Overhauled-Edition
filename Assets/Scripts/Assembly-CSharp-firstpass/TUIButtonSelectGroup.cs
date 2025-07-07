using UnityEngine;

[AddComponentMenu("TUI/Control/Select Group")]
public class TUIButtonSelectGroup : TUIControl
{
	public override void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		TUIButtonSelect[] componentsInChildren = base.gameObject.GetComponentsInChildren<TUIButtonSelect>(false);
		bool flag = false;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] == control && eventType == 1)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j] != control && componentsInChildren[j].IsSelected())
				{
					componentsInChildren[j].SetSelected(false);
				}
			}
		}
		base.HandleEvent(control, eventType, wparam, lparam, data);
	}
}
