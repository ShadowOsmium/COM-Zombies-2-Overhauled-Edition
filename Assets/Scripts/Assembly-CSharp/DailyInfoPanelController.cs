using System;
using UnityEngine;

public class DailyInfoPanelController : UIPanelController
{
	public TUILabel mission_title;

	public TUIButtonClick ok_button;

	public TUIButtonClick cancel_button;

	private double total_cd_second;

	private double last_total_cd_second;

	private bool update_time_show;

	public void OnConnect()
	{
		//ok_button.gameObject.SetActive(false);
		//cancel_button.gameObject.SetActive(false);
		//mission_title.Text = "CONNECTING...";
	}

	public void OnConnectFinish()
	{
		ok_button.gameObject.SetActive(false);
		cancel_button.gameObject.SetActive(false);
		update_time_show = true;
		last_total_cd_second = (total_cd_second = (GameData.Instance.next_cd_date - GameData.Instance.last_checked_date_now).TotalSeconds);
		TimeSpan timeSpan = TimeSpan.FromSeconds(total_cd_second);
		mission_title.Text = timeSpan.ToString();
	}

	public void OnConnectError()
	{
		//ok_button.gameObject.SetActive(false);
		//cancel_button.gameObject.SetActive(false);
		//mission_title.Text = "CONNECTING FAILED";
	}

	private void Update()
	{
		if (update_time_show)
		{
			total_cd_second -= Time.deltaTime;
			if (last_total_cd_second - total_cd_second >= 1.0)
			{
				last_total_cd_second = total_cd_second;
				TimeSpan timeSpan = TimeSpan.FromSeconds((int)total_cd_second);
				mission_title.Text = timeSpan.ToString();
			}
		}
	}

	public override void Show()
	{
		base.Show();
		update_time_show = false;
	}

	public override void Hide()
	{
		base.Hide();
		update_time_show = false;
	}
}
