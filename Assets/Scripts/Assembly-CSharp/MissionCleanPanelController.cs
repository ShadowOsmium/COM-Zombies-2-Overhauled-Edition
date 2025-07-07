using UnityEngine;

public class MissionCleanPanelController : UIPanelController
{
	public TUILabel mission_content;

	protected float mission_width = 178f;

	private float last_mission_update_time;

	private bool is_mission_update;

	private float mission_target_width;

	private float mission_ori_width;

	public float mission_lerp_speed = 2f;

	public TUIMeshSprite mission_bar;

	public TUIRect mission_rect;

	public TUIMeshSprite mission_bar_icon;

	public GameObject mission_eff;

	private void Start()
	{
	}

	private void Update()
	{
		UpdateMissionBar();
	}

	public override void Show()
	{
		mission_eff.SetActive(true);
		base.Show();
		Invoke("ShowEff", 0.5f);
	}

	public void ShowEff()
	{
		AnimationUtil.PlayAnimate(mission_eff, "mubiao", WrapMode.Once);
		Invoke("HideEff", mission_eff.GetComponent<Animation>()["mubiao"].length);
	}

	public void HideEff()
	{
		if (mission_eff != null)
		{
			mission_eff.SetActive(false);
		}
	}

	public void SetContent(string content)
	{
		if (mission_content != null)
		{
			mission_content.Text = content;
		}
	}

	private void UpdateMissionBar()
	{
		if (is_mission_update)
		{
			mission_rect.Size = new Vector2(Mathf.Lerp(mission_ori_width, mission_target_width, (Time.time - last_mission_update_time) * mission_lerp_speed), mission_rect.Size.y);
			mission_rect.NeedUpdate = true;
			mission_bar.NeedUpdate = true;
			mission_bar_icon.transform.localPosition = new Vector3((0f - mission_rect.Size.x) / 2f, mission_bar_icon.transform.localPosition.y, mission_bar_icon.transform.localPosition.z);
			if (Time.time - last_mission_update_time >= 1f / mission_lerp_speed)
			{
				is_mission_update = false;
			}
		}
	}

	public void SetMissionBar(float percent)
	{
		last_mission_update_time = Time.time;
		is_mission_update = true;
		mission_target_width = mission_width * percent;
		mission_ori_width = mission_rect.Size.x;
	}
}
