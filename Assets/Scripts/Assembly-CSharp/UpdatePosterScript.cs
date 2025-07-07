using CoMZ2;
using UnityEngine;

public class UpdatePosterScript : MonoBehaviour
{
	public enum Resolution
	{
		Iphone4 = 480,
		Iphone5 = 568,
		Ipad = 512
	}

	public TUIScrollerEx Scroller;

	public GameObject[] Pages;

	public GameObject TapToPlay;

	private Resolution resolution = Resolution.Iphone4;

	private float resolution_val;

	private void Awake()
	{
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		resolution_val = Screen.width / 2;
		if (Mathf.Max(Screen.width, Screen.height) == 1136)
		{
			resolution = Resolution.Iphone5;
		}
		else if (Mathf.Max(Screen.width, Screen.height) == 1024 || Mathf.Max(Screen.width, Screen.height) == 2048)
		{
			resolution = Resolution.Ipad;
			resolution_val = 512f;
		}
		else if (Mathf.Max(Screen.width, Screen.height) == 960)
		{
			resolution = Resolution.Iphone4;
		}
		float num = (float)resolution / 568f;
		float num2 = Screen.width;
		float num3 = Screen.height;
		float num4 = num2 / num3;
		if ((double)num4 >= 1.77)
		{
			num = 1f;
		}
		else if ((double)num4 < 1.77 && (double)num4 >= 1.6)
		{
			num = 0.98f;
			if (Mathf.Max(num2, num3) <= 1136f && Mathf.Max(num2, num3) > 960f)
			{
				num = 0.98f;
				resolution_val = Screen.width / 2;
				if (num2 == 1024f)
				{
					resolution_val = Screen.width;
				}
			}
			else if (Mathf.Max(num2, num3) < 960f)
			{
				num = 0.98f;
				resolution_val = Screen.width;
			}
			else if (Mathf.Max(num2, num3) > 2048f)
			{
				num = 0.9f;
			}
		}
		else
		{
			num = 0.85f;
		}
		for (int i = 0; i < Pages.Length; i++)
		{
			Pages[i].transform.localPosition = new Vector3(resolution_val * (float)i, 0f, 0f);
			Pages[i].transform.Find("slide").localScale = new Vector3(num, num, 1f);
		}
		Scroller.size = new Vector2(Screen.width, Screen.height);
		Scroller.rangeXMin = (0f - resolution_val) * (float)(Pages.Length - 1);
		Scroller.borderXMin = Scroller.rangeXMin;
		Scroller.pageX = new float[Pages.Length];
		for (int j = 0; j < Pages.Length; j++)
		{
			Scroller.pageX[j] = (0f - resolution_val) * (float)j;
		}
	}

	private void Start()
	{
	}

	private void OnPlayGameEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			if (GameData.Instance.is_enter_tutorial)
			{
				GameData.Instance.cur_quest_info.mission_type = MissionType.Tutorial;
				GameData.Instance.cur_quest_info.mission_day_type = MissionDayType.Tutorial;
				GameData.Instance.loading_to_scene = "GameTutorial";
				Application.LoadLevel("Loading");
			}
			else
			{
				Application.LoadLevel("GameCover");
			}
		}
	}

	private void OnScrollerEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3 && Scroller.cur_page == Pages.Length - 1)
		{
			TapToPlay.GetComponent<MissionIconEffect>().enabled = true;
		}
	}
}
