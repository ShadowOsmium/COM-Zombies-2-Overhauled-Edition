using UnityEngine;

public class TUITest : MonoBehaviour, TUIHandler
{
	public TUI m_tui;

	public void Start()
	{
		m_tui.SetHandler(this);
	}

	public void Update()
	{
		TUIInput[] input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
	}

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if ((!(control.name == "ShopBtn") || eventType != 3) && (!(control.name == "TeamBtn") || eventType != 3) && control.name == "BankBtn" && eventType != 3)
		{
		}
	}
}
