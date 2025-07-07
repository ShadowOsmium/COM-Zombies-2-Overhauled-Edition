public class MissionTimeKillPanelController : UIPanelController
{
	public TUILabel mission_content;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetContent(string content)
	{
		if (mission_content != null)
		{
			mission_content.Text = content;
		}
	}
}
