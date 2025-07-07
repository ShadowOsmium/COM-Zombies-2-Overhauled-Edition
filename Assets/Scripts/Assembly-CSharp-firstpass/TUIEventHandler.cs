using UnityEngine;

public class TUIEventHandler : MonoBehaviour, TUIHandler
{
	[SerializeField]
	protected TUI[] targets;

	private void Awake()
	{
		if (targets == null)
		{
			return;
		}
		TUI[] array = targets;
		foreach (TUI tUI in array)
		{
			if (null != tUI)
			{
				tUI.SetHandler(this);
			}
		}
	}

	public virtual void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (control.invokeOnEvent)
		{
			TUITool.TUIControlEventInvoke(control, control, eventType, wparam, lparam, data);
		}
	}
}
