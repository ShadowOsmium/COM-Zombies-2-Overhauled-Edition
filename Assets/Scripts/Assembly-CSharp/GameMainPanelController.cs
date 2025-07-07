using System.Collections.Generic;
using UnityEngine;

public class GameMainPanelController : UIPanelController
{
	public TUIMeshSprite avatar_icon;

	public TUIMeshSprite Hp_bar;

	public TUIRect Hp_rect;

	public TUIMeshSprite Exp_bar;

	public TUIRect Exp_rect;

	public TUIMeshSprite Combo_bar;

	public TUIRect Combo_rect;

	private List<TUIMeshSprite> combo_stars = new List<TUIMeshSprite>();

	private List<ParticleSystem> combo_stars_eff = new List<ParticleSystem>();

	public GameObject npc_hp_bar_obj;

	public TUIMeshSprite npc_hp_bar;

	public TUIRect npc_hp_rect;

	public TUIMeshSprite npc_hp_bar_icon;

	public TUILabel bullet_label;

	public TUILabel combo_label;

	public TUIButtonClick add_bullet_button;

	public GameObject add_bullet_button_label;

	public TUIButtonClick weapon_button;

	private TUIMeshSprite weapon_frame_n;

	private TUIMeshSprite weapon_frame_p;

	public List<SkillUIController> skill_button_list = new List<SkillUIController>();

	protected float hp_width = 162f;

	private float last_hp_update_time;

	private bool is_hp_update;

	private float hp_target_width;

	private float hp_ori_width;

	public float hp_lerp_speed = 2f;

	protected float exp_width = 151f;

	private float last_exp_update_time;

	private bool is_exp_update;

	private float exp_target_width;

	private float exp_ori_width;

	public float exp_lerp_speed = 2f;

	protected float combo_width = 160f;

	private float last_combo_update_time;

	private bool is_combo_update;

	private float combo_target_width;

	private float combo_ori_width;

	public float combo_lerp_speed = 2f;

	protected float npc_hp_width = 85f;

	private float npc_last_hp_update_time;

	private bool npc_is_hp_update;

	private float npc_hp_target_width;

	private float npc_hp_ori_width;

	public float npc_hp_lerp_speed = 2f;

	public MissionCleanPanelController clean_panel;

	public MissionTimeAlivePanelController time_alive_panel;

	public MissionBossPanelController boss_panel;

	public MissionNpcResPanelController npc_res_panel;

	public MissionNpcConvoyPanelController npc_convoy_panel;

	public GameRebirthPanelController rebirth_panel;

	public TUIMeshSprite buff_icon_1;

	public TUIMeshSprite buff_icon_2;

	public TUILabel buff_label_1;

	public TUILabel buff_label_2;

	public TUIMeshSprite combo_eff;

	public TUIMeshSprite help_eff;

	private List<UIPanelController> panel_list = new List<UIPanelController>();

	public TUILabel game_tip_label;

	public TUILabel mission_day_label;

	public TUILabel skill_cd_label_1;

	private void Start()
	{
		if (weapon_button != null)
		{
			weapon_frame_n = weapon_button.transform.Find("button_n").GetComponent<TUIMeshSprite>();
			weapon_frame_p = weapon_button.transform.Find("button_p").GetComponent<TUIMeshSprite>();
		}
		if (Combo_bar != null)
		{
			TUIMeshSprite component = Combo_bar.transform.Find("star_1").GetComponent<TUIMeshSprite>();
			TUIMeshSprite component2 = Combo_bar.transform.Find("star_2").GetComponent<TUIMeshSprite>();
			TUIMeshSprite component3 = Combo_bar.transform.Find("star_3").GetComponent<TUIMeshSprite>();
			ParticleSystem item = component.transform.Find("star_eff").GetComponent<ParticleSystem>();
			ParticleSystem item2 = component2.transform.Find("star_eff").GetComponent<ParticleSystem>();
			ParticleSystem item3 = component3.transform.Find("star_eff").GetComponent<ParticleSystem>();
			combo_stars.Add(component);
			combo_stars.Add(component2);
			combo_stars.Add(component3);
			combo_stars_eff.Add(item);
			combo_stars_eff.Add(item2);
			combo_stars_eff.Add(item3);
		}
		buff_icon_1.gameObject.SetActive(false);
		buff_icon_2.gameObject.SetActive(false);
		panel_list.Add(clean_panel);
		panel_list.Add(time_alive_panel);
		panel_list.Add(boss_panel);
		panel_list.Add(npc_res_panel);
		panel_list.Add(npc_convoy_panel);
	}

	private void Update()
	{
		UpdateHpBar();
		UpdateExpBar();
		UpdateComboBar();
		UpdateNpcHpBar();
	}

	private void UpdateHpBar()
	{
		if (is_hp_update)
		{
			Hp_rect.Size = new Vector2(Mathf.Lerp(hp_ori_width, hp_target_width, (Time.time - last_hp_update_time) * hp_lerp_speed), Hp_rect.Size.y);
			Hp_rect.NeedUpdate = true;
			Hp_bar.NeedUpdate = true;
			if (Time.time - last_hp_update_time >= 1f / hp_lerp_speed)
			{
				is_hp_update = false;
			}
		}
	}

	private void UpdateExpBar()
	{
		if (is_exp_update)
		{
			Exp_rect.Size = new Vector2(Mathf.Lerp(exp_ori_width, exp_target_width, (Time.time - last_exp_update_time) * exp_lerp_speed), Exp_rect.Size.y);
			Exp_rect.NeedUpdate = true;
			Exp_bar.NeedUpdate = true;
			if (Time.time - last_exp_update_time >= 1f / exp_lerp_speed)
			{
				is_exp_update = false;
			}
		}
	}

	private void UpdateComboBar()
	{
		if (is_combo_update)
		{
			Combo_rect.Size = new Vector2(combo_target_width, Combo_rect.Size.y);
			Combo_rect.NeedUpdate = true;
			Combo_bar.NeedUpdate = true;
			is_combo_update = false;
		}
	}

	private void UpdateNpcHpBar()
	{
		if (npc_hp_bar_obj.activeSelf && npc_is_hp_update)
		{
			npc_hp_rect.Size = new Vector2(Mathf.Lerp(npc_hp_ori_width, npc_hp_target_width, (Time.time - npc_last_hp_update_time) * npc_hp_lerp_speed), npc_hp_rect.Size.y);
			npc_hp_rect.NeedUpdate = true;
			npc_hp_bar.NeedUpdate = true;
			if (Time.time - npc_last_hp_update_time >= 1f / npc_hp_lerp_speed)
			{
				npc_is_hp_update = false;
			}
		}
	}

	public void UpdateBulletLabel(string content)
	{
		if (bullet_label != null)
		{
			bullet_label.Text = content;
		}
	}

	public void UpdateWeaponFrame(string frame, bool status)
	{
		if (weapon_frame_n != null)
		{
			weapon_frame_n.GrayStyle = !status;
			weapon_frame_n.texture = frame;
		}
		if (weapon_frame_p != null)
		{
			weapon_frame_p.GrayStyle = !status;
			weapon_frame_p.texture = frame;
		}
	}

	public void SetHpBar(float percent)
	{
		last_hp_update_time = Time.time;
		is_hp_update = true;
		hp_target_width = hp_width * percent;
		hp_ori_width = Hp_rect.Size.x;
	}

	public void SetExpBar(float percent)
	{
		last_exp_update_time = Time.time;
		is_exp_update = true;
		exp_target_width = exp_width * percent;
		exp_ori_width = Exp_rect.Size.x;
	}

	public void SetComboBar(float percent)
	{
		last_combo_update_time = Time.time;
		is_combo_update = true;
		combo_target_width = combo_width * percent;
		combo_ori_width = Combo_rect.Size.x;
	}

	public void SetLevelLabel(int level)
	{
	}

	public void SetComboLabel(string combo)
	{
		if (combo_label != null)
		{
			combo_label.Text = "Combo:" + combo;
		}
	}

	public void HidePanels()
	{
		foreach (UIPanelController item in panel_list)
		{
			item.Hide();
		}
	}

	public void ShowAddBulletButton(bool state)
	{
		add_bullet_button.Disable(!state);
	}

	public void ShowAddBulletLabel(bool state)
	{
		add_bullet_button_label.SetActive(state);
	}

	public void SetComboBarStar(int star_count)
	{
		for (int i = 0; i < combo_stars.Count; i++)
		{
			if (i < star_count)
			{
				combo_stars[i].texture = "xx-2";
				if (i == star_count - 1)
				{
					combo_stars_eff[i].Play();
				}
			}
			else
			{
				combo_stars[i].texture = "xx-1";
			}
		}
		switch (star_count)
		{
		case 3:
			buff_icon_1.gameObject.SetActive(true);
			buff_icon_2.gameObject.SetActive(true);
			buff_icon_1.transform.localPosition = new Vector3(-53f, -32f, -1f);
			buff_icon_2.transform.localPosition = new Vector3(-23f, -32f, -1f);
			ShowComboLabelEff("nubelievable", "combo_eff1");
			ShowBuffLabelEff1("Damage + " + GetPersentString(GameSceneController.Instance.player_controller.ComboDamageRatio), "buff_label_eff");
			ShowBuffLabelEff2("Rate of Fire + " + GetPersentString(GameSceneController.Instance.player_controller.ComboRateRatio), "buff_label_eff");
			break;
		case 2:
			buff_icon_1.gameObject.SetActive(false);
			buff_icon_2.gameObject.SetActive(true);
			buff_icon_2.transform.localPosition = new Vector3(-53f, -32f, -1f);
			ShowComboLabelEff("IMPRESSIVE!", "combo_eff1");
			ShowBuffLabelEff1("Rate of Fire + " + GetPersentString(GameSceneController.Instance.player_controller.ComboRateRatio), "buff_label_eff");
			HideBuffLabelEff2();
			break;
		case 1:
			buff_icon_1.gameObject.SetActive(true);
			buff_icon_2.gameObject.SetActive(false);
			buff_icon_1.transform.localPosition = new Vector3(-53f, -32f, -1f);
			ShowComboLabelEff("cool", "combo_eff1");
			ShowBuffLabelEff1("Damage + " + GetPersentString(GameSceneController.Instance.player_controller.ComboDamageRatio), "buff_label_eff");
			HideBuffLabelEff2();
			break;
		default:
			buff_icon_1.gameObject.SetActive(false);
			buff_icon_2.gameObject.SetActive(false);
			HideComboLabelEff();
			HideBuffLabelEff1();
			HideBuffLabelEff2();
			break;
		}
	}

	public void SetIcon(string frame)
	{
		if (avatar_icon != null)
		{
			avatar_icon.texture = frame;
		}
	}

	public void SetNpcHpBar(float percent)
	{
		npc_last_hp_update_time = Time.time;
		npc_is_hp_update = true;
		npc_hp_target_width = npc_hp_width * percent;
		npc_hp_ori_width = npc_hp_rect.Size.x;
	}

	public void SetNpcBarIcon(string frame)
	{
		npc_hp_bar_icon.texture = frame;
	}

	public void EnableNpcHpBar(bool status)
	{
		npc_hp_bar_obj.SetActive(status);
	}

	private void HideComboLabelEff()
	{
		combo_eff.gameObject.SetActive(false);
	}

	private void ShowComboLabelEff(string frame_name, string ani_name)
	{
		combo_eff.gameObject.SetActive(true);
		combo_eff.texture = frame_name;
		AnimationUtil.PlayAnimate(combo_eff.gameObject, ani_name, WrapMode.Once);
		Invoke("HideComboLabelEff", combo_eff.GetComponent<Animation>()[ani_name].length);
	}

	private void HideHelpLabelEff()
	{
		help_eff.gameObject.SetActive(false);
	}

	public void ShowHelpLabelEff(string ani_name)
	{
		if (!help_eff.gameObject.activeSelf)
		{
			help_eff.gameObject.SetActive(true);
			AnimationUtil.PlayAnimate(help_eff.gameObject, ani_name, WrapMode.Loop);
			Invoke("HideHelpLabelEff", help_eff.GetComponent<Animation>()[ani_name].length * 3f);
		}
	}

	public void ShowGameTipLabel(string content, Color color)
	{
		game_tip_label.gameObject.SetActive(true);
		game_tip_label.color = color;
		game_tip_label.Text = content;
	}

	public void HideGameTipLabel()
	{
		game_tip_label.gameObject.SetActive(false);
	}

	private void ShowBuffLabelEff1(string content, string ani_name)
	{
		buff_label_1.gameObject.SetActive(true);
		buff_label_1.Text = content;
		AnimationUtil.PlayAnimate(buff_label_1.gameObject, ani_name, WrapMode.Once);
	}

	private void ShowBuffLabelEff2(string content, string ani_name)
	{
		buff_label_2.gameObject.SetActive(true);
		buff_label_2.Text = content;
		AnimationUtil.PlayAnimate(buff_label_2.gameObject, ani_name, WrapMode.Once);
	}

	private void HideBuffLabelEff1()
	{
		buff_label_1.gameObject.SetActive(false);
	}

	private void HideBuffLabelEff2()
	{
		buff_label_2.gameObject.SetActive(false);
	}

	private string GetPersentString(float val)
	{
		string empty = string.Empty;
		float num = val - 1f;
		return Mathf.RoundToInt(num * 100f) + "%";
	}

	public void ShowMissionDayLabel(string content)
	{
		mission_day_label.gameObject.SetActive(true);
		mission_day_label.Text = content;
		AnimationUtil.PlayAnimate(mission_day_label.gameObject, "Mission_day_ani", WrapMode.Once);
		Invoke("HideMissionDayLabel", mission_day_label.gameObject.GetComponent<Animation>()["Mission_day_ani"].length);
	}

	private void HideMissionDayLabel()
	{
		mission_day_label.gameObject.SetActive(false);
	}

	public void SetSkillCdLabel(string content)
	{
		if (skill_cd_label_1 != null)
		{
			skill_cd_label_1.Text = content;
		}
	}
}
