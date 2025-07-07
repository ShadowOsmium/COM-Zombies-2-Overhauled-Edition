using UnityEngine;

public class UITeammateBar : MonoBehaviour
{
	public TUIMeshSprite npc_hp_bar;

	public TUIRect hp_rect;

	public TUIMeshSprite hp_bar_icon;

	public TUILabel label_name;

	protected float hp_width = 85f;

	private float last_hp_update_time;

	private bool is_hp_update;

	private float hp_target_width;

	private float hp_ori_width;

	public float hp_lerp_speed = 2f;

	private void Start()
	{
		SetHpBar(1f);
	}

	private void Update()
	{
		UpdateHpBar();
	}

	private void UpdateHpBar()
	{
		if (base.gameObject.activeSelf && is_hp_update)
		{
			hp_rect.Size = new Vector2(Mathf.Lerp(hp_ori_width, hp_target_width, (Time.time - last_hp_update_time) * hp_lerp_speed), hp_rect.Size.y);
			hp_rect.NeedUpdate = true;
			npc_hp_bar.NeedUpdate = true;
			if (Time.time - last_hp_update_time >= 1f / hp_lerp_speed)
			{
				is_hp_update = false;
			}
		}
	}

	public void SetHpBar(float percent)
	{
		last_hp_update_time = Time.time;
		is_hp_update = true;
		hp_target_width = hp_width * percent;
		hp_ori_width = hp_rect.Size.x;
	}

	public void SetHpbarInfo(string icon_name, string content)
	{
		hp_bar_icon.texture = icon_name;
		label_name.Text = content;
	}
}
