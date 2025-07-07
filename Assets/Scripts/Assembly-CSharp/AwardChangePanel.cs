using System;
using UnityEngine;

public class AwardChangePanel : UIPanelController
{
	protected static AwardChangePanel instance;

	public TUIButtonClick ok_button;

	public Action on_button_ok;

	public TUIMeshSprite award_src;

	public TUIMeshSprite award_des;

	public TUILabel count_label;

	public TUIMeshSprite award_bk;

	public static AwardChangePanel Instance
	{
		get
		{
			return instance;
		}
		set
		{
			instance = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		on_button_ok = null;
		instance = null;
	}

	private void OnOKButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			Hide();
			if (on_button_ok != null)
			{
				on_button_ok();
			}
		}
	}

	public static AwardChangePanel ShowAwardChangePanel(GameObject root_obj, Action OnButtonOk, string src_image, string des_img, int count_des, bool show_bk = false)
	{
		if (Instance == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("TUI/AwardChangePanel")) as GameObject;
			Instance = gameObject.GetComponent<AwardChangePanel>();
		}
		Instance.transform.parent = root_obj.transform;
		Instance.transform.localPosition = new Vector3(0f, 0f, -60f);
		Instance.transform.localRotation = Quaternion.identity;
		Instance.Show();
		Instance.on_button_ok = OnButtonOk;
		Instance.award_src.texture = src_image;
		Instance.award_des.texture = des_img;
		Instance.count_label.Text = count_des.ToString();
		Instance.award_bk.gameObject.SetActive(show_bk);
		return Instance;
	}

	public static void DestroyPanel()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(Instance.gameObject);
		}
	}
}
