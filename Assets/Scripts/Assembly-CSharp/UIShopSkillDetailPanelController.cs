using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class UIShopSkillDetailPanelController : UIShopPanelController
{
	public SkillData cur_skill;

	public TUILabel skill_name;

	public TUILabel skill_content;

	public TUIMeshSprite skill_img;

	public List<GameObject> skill_info_list = new List<GameObject>();

	public List<GameObject> skill_info_list_next = new List<GameObject>();

	public TUIButtonClick skill_upgrade_button;

	public TUILabel up_price_n;

	public TUILabel up_price_p;

	public GameObject upgrade_arrow;

	private float diff_val = 0.0001f;

	private GameMsgBoxController msgBox;

	public void ResetSkill(SkillData skill)
	{
		cur_skill = skill;
		UpdateSkillInfo();
	}

	public void UpdateSkillInfo()
	{
		if (cur_skill == null)
		{
			Debug.LogError("UpdateSkillInfo error!");
			return;
		}
		skill_name.Text = cur_skill.config.show_name;
		skill_content.Text = cur_skill.config.skill_content;
		skill_img.texture = ((!(cur_skill.skill_name == "Scarecrow")) ? ("skill_" + cur_skill.skill_name) : ("skill_" + cur_skill.skill_name + "_1"));
		for (int i = 0; i < skill_info_list.Count; i++)
		{
			skill_info_list[i].SetActive(true);
		}
		for (int j = 0; j < skill_info_list_next.Count; j++)
		{
			skill_info_list_next[j].SetActive(true);
		}
		int num = 0;
		int num2 = 0;
		GameObject gameObject = skill_info_list[num++];
		gameObject.GetComponent<TUILabel>().Text = "Level: " + cur_skill.level;
		if (cur_skill.level < cur_skill.config.max_level)
		{
			gameObject = skill_info_list_next[num2++];
			gameObject.GetComponent<TUILabel>().Text = "Level: " + (cur_skill.level + 1);
			skill_upgrade_button.Disable(false);
			up_price_n.Text = cur_skill.UpgradePrice.ToString();
			up_price_p.Text = cur_skill.UpgradePrice.ToString();
			upgrade_arrow.SetActive(true);
		}
		else
		{
			skill_upgrade_button.Disable(true);
			upgrade_arrow.SetActive(false);
		}
		if (Mathf.Abs(cur_skill.config.hp_cfg.base_data - cur_skill.config.hp_cfg.max_data) > diff_val)
		{
			gameObject = skill_info_list[num++];
			gameObject.GetComponent<TUILabel>().Text = "HP: " + cur_skill.hp_capcity.ToString("f1");
			if (cur_skill.level < cur_skill.config.max_level)
			{
				gameObject = skill_info_list_next[num2++];
				gameObject.GetComponent<TUILabel>().Text = "HP: " + cur_skill.hp_capcity_next.ToString("f1");
			}
		}
		if (Mathf.Abs(cur_skill.config.damage_cfg.base_data - cur_skill.config.damage_cfg.max_data) > diff_val)
		{
			gameObject = skill_info_list[num++];
			gameObject.GetComponent<TUILabel>().Text = "Damage: " + cur_skill.damage_val.ToString("f1");
			if (cur_skill.level < cur_skill.config.max_level)
			{
				gameObject = skill_info_list_next[num2++];
				gameObject.GetComponent<TUILabel>().Text = "Damage: " + cur_skill.damage_val_next.ToString("f1");
			}
		}
		if (Mathf.Abs(cur_skill.config.range_cfg.base_data - cur_skill.config.range_cfg.max_data) > diff_val)
		{
			gameObject = skill_info_list[num++];
			gameObject.GetComponent<TUILabel>().Text = "Range: " + cur_skill.range_val.ToString("f1");
			if (cur_skill.level < cur_skill.config.max_level)
			{
				gameObject = skill_info_list_next[num2++];
				gameObject.GetComponent<TUILabel>().Text = "Range: " + cur_skill.range_val_next.ToString("f1");
			}
		}
		if (Mathf.Abs(cur_skill.config.life_time_cfg.base_data - cur_skill.config.life_time_cfg.max_data) > diff_val)
		{
			gameObject = skill_info_list[num++];
			gameObject.GetComponent<TUILabel>().Text = "Life Span: " + cur_skill.life_time.ToString("f1");
			if (cur_skill.level < cur_skill.config.max_level)
			{
				gameObject = skill_info_list_next[num2++];
				gameObject.GetComponent<TUILabel>().Text = "Life Span: " + cur_skill.life_time_next.ToString("f1");
			}
		}
		if (Mathf.Abs(cur_skill.config.cd_time_cfg.base_data - cur_skill.config.cd_time_cfg.max_data) > diff_val)
		{
			gameObject = skill_info_list[num++];
			gameObject.GetComponent<TUILabel>().Text = "CD Time: " + cur_skill.cd_interval.ToString("f1");
			if (cur_skill.level < cur_skill.config.max_level)
			{
				gameObject = skill_info_list_next[num2++];
				gameObject.GetComponent<TUILabel>().Text = "CD Time: " + cur_skill.cd_interval_next.ToString("f1");
			}
		}
		if (Mathf.Abs(cur_skill.config.frequency_cfg.base_data - cur_skill.config.frequency_cfg.max_data) > diff_val)
		{
			gameObject = skill_info_list[num++];
			gameObject.GetComponent<TUILabel>().Text = "Fire Rate:" + cur_skill.frequency_val.ToString("f1");
			if (cur_skill.level < cur_skill.config.max_level)
			{
				gameObject = skill_info_list_next[num2++];
				gameObject.GetComponent<TUILabel>().Text = "Fire Rate:" + cur_skill.frequency_val_next.ToString("f1");
			}
		}
		for (int k = num; k < skill_info_list.Count; k++)
		{
			skill_info_list[k].SetActive(false);
		}
		for (int l = num2; l < skill_info_list_next.Count; l++)
		{
			skill_info_list_next[l].SetActive(false);
		}
	}

	private void OnSkillUpgrade(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType != 3)
		{
			return;
		}
		UIShopSceneController.Instance.SceneAudio.PlayAudio("UI_click");
		if (cur_skill.Upgrade())
		{
			UpdateSkillInfo();
			UISceneController.Instance.MoneyController.UpdateInfo();
			if (cur_skill.skill_name == "Enchant")
			{
				ImageMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, GameConfig.Instance.Skill_Enchant_Monster_List[cur_skill.level].ToString(), base.gameObject, "Now charm a zombie of this kind as your sidekick!", null, null);
			}
		}
		else
		{
			int num = cur_skill.UpgradePrice - GameData.Instance.total_voucher.GetIntVal();
			int crystal = Mathf.CeilToInt((float)num / GameConfig.Instance.crystal_to_voucher);
			string content = "You need " + num + " more vouchers to buy this.";
			CrystalExchangeCash crystalExchangeCash = new CrystalExchangeCash();
			crystalExchangeCash.Crystal = crystal;
			crystalExchangeCash.Voucher = num;
			CrystalExchangeCash exchange = crystalExchangeCash;
			msgBox = CrystalMsgBoxController.ShowMsgBox(exchange, base.gameObject, content, OnExchangeMoneyOk, OnMsgboxCancel);
		}
	}

	private void OnExchangeMoneyOk()
	{
		Object.Destroy(msgBox.gameObject);
		if (((CrystalMsgBoxController)msgBox).Exchange.Crystal <= GameData.Instance.total_crystal.GetIntVal())
		{
			GameData.Instance.OnExchgCurrcy(GameCurrencyType.Crystal, GameCurrencyType.Voucher, ((CrystalMsgBoxController)msgBox).Exchange.Crystal, ((CrystalMsgBoxController)msgBox).Exchange.Voucher);
			if (cur_skill.Upgrade())
			{
				UpdateSkillInfo();
				UISceneController.Instance.MoneyController.UpdateInfo();
				if (cur_skill.skill_name == "Enchant")
				{
					ImageMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.SingleButton, GameConfig.Instance.Skill_Enchant_Monster_List[cur_skill.level].ToString(), base.gameObject, "Now charm a zombie of this kind as your sidekick!", null, null);
				}
			}
		}
		else
		{
			//UISceneController.Instance.MoneyController.IapPanel.Show();
		}
	}

	private void OnMsgboxCancel()
	{
		Object.Destroy(msgBox.gameObject);
	}
}
