using UnityEngine;

public class SkillUIController : MonoBehaviour
{
	public SkillController avatar_skill;

	protected TUILabel cd_label;

	protected bool is_inited;

	protected SkillUseState last_skill_state;

	protected TUIMeshSprite frame_n;

	protected TUIMeshSprite frame_p;

	protected GameObject ready_eff_obj;

	private string str = string.Empty;

	public void Init(SkillController skill)
	{
		avatar_skill = skill;
		is_inited = true;
		cd_label.Text = "0";
		frame_n = base.transform.Find("button_n").GetComponent<TUIMeshSprite>();
		frame_p = base.transform.Find("button_p").GetComponent<TUIMeshSprite>();
		ready_eff_obj = base.transform.Find("UI_01_pfb").gameObject;
	}

	private void Start()
	{
		cd_label = base.transform.Find("Label_CD").GetComponent<TUILabel>();
	}

	private void Update()
	{
		if (is_inited)
		{
			UpdateCdLabel();
		}
	}

	private void UpdateCdLabel()
	{
		if (avatar_skill.skill_use_state == SkillUseState.Ready)
		{
			str = string.Empty;
		}
		else
		{
			str = Mathf.RoundToInt(avatar_skill.SkillTimeForReady()).ToString();
		}
		if (cd_label.Text != str)
		{
			cd_label.Text = str;
		}
	}

	public void ResetSkillFrame(string frame_name)
	{
		if (frame_n != null)
		{
			frame_n.texture = frame_name;
		}
		if (frame_p != null)
		{
			frame_p.texture = frame_name;
		}
	}

	public void PlayReadyEff()
	{
		if (ready_eff_obj != null)
		{
			ready_eff_obj.GetComponent<ParticleSystem>().Play();
		}
	}

	public void StopReadyEff()
	{
		if (ready_eff_obj != null)
		{
			ready_eff_obj.GetComponent<ParticleSystem>().Stop();
		}
	}
}
