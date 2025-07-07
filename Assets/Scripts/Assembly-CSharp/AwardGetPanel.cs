using System;
using UnityEngine;

public class AwardGetPanel : UIPanelController
{
	protected static AwardGetPanel instance;

	public TUIButtonClick ok_button;

	public Action on_button_ok;

	public TUIMeshSprite award_src;

	public TUIMeshSprite award_bk;

	public TUILabel count_label;

	public static AwardGetPanel Instance
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

	public static AwardGetPanel ShowAwardGetPanel(GameObject root_obj, Action OnButtonOk, string image, int count, bool show_bk = false)
	{
		if (Instance == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("TUI/AwardGetPanel")) as GameObject;
			Instance = gameObject.GetComponent<AwardGetPanel>();
		}
		Instance.transform.parent = root_obj.transform;
		Instance.transform.localPosition = new Vector3(0f, 0f, -60f);
		Instance.transform.localRotation = Quaternion.identity;
		Instance.Show();
		Instance.on_button_ok = OnButtonOk;
		Instance.award_src.texture = image;
		Instance.count_label.Text = count.ToString();
		if (count == 1)
		{
			Instance.count_label.gameObject.SetActive(false);
		}
		else
		{
			Instance.count_label.gameObject.SetActive(true);
		}
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
