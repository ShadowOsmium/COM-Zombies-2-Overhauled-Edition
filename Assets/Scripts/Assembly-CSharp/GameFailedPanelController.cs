using CoMZ2;
using UnityEngine;

public class GameFailedPanelController : UIPanelController
{
	public TUIButtonClick retry_button;

	public TUIButtonClick quit_button;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void Show()
	{
		base.Show();
		if (GameData.Instance.cur_quest_info.mission_type == MissionType.Tutorial)
		{
			retry_button.gameObject.SetActive(true);
			quit_button.gameObject.SetActive(false);
			retry_button.gameObject.transform.localPosition = new Vector3(0f, -40f, -2f);
		}
		else
		{
			retry_button.gameObject.SetActive(false);
			quit_button.gameObject.SetActive(false);
		}
	}

	private void ToShopScene()
	{
		GameSceneController.Instance.GoShopScene();
	}
}
