using UnityEngine;

public class GameRebirthPanelController : UIPanelController
{
	public TUIMeshSprite time_bar;

	public TUIRect time_rect;

	public float time_rect_width = 162f;

	public TUILabel crystal_label;

	private bool is_update_time = true;

	private float total_time = 10f;

	private float cur_time;

	private void Start()
	{
	}

	private void Update()
	{
		if (is_update_time)
		{
			cur_time += Time.deltaTime;
			if (cur_time >= total_time)
			{
				cur_time = total_time;
				CancelRebirth();
			}
			time_rect.Size = new Vector2((total_time - cur_time) / total_time * time_rect_width, time_rect.Size.y);
			time_rect.NeedUpdate = true;
			time_bar.NeedUpdate = true;
		}
	}

	public override void Show()
	{
		base.Show();
		is_update_time = true;
		cur_time = 0f;
		SetCrystalLabel(GameSceneController.Instance.cur_rebirth_cost.ToString());
        Screen.lockCursor = false;
	}

	public void SetCrystalLabel(string str)
	{
		crystal_label.Text = "x" + str;
	}

	public void CancelRebirth()
	{
		is_update_time = false;
		Hide();
		GameSceneController.Instance.OnCancelRebirth();
        Screen.lockCursor = false;
    }

	public void ObRebirthOver()
	{
		is_update_time = false;
		Hide();
        Screen.lockCursor = false;
    }
}
