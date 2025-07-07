using CoMZ2;
using UnityEngine;

public class GuideController : MonoBehaviour
{
	public TUILabel Content;

	public GameObject Arrow;

	public GameObject Area;

	public GameObject Mask;

	public GameObject Event_button;

	private IGuideEvent currentGuide;

	public void Show(IGuideEvent guide)
	{
		Time.timeScale = 0f;
		TAudioManager.instance.musicVolume = 0f;
		currentGuide = guide;
		base.gameObject.SetActive(true);
		Content.Text = currentGuide.GuideText();
		currentGuide.DoSomething();
	}

	private void OnSkipEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			IGuideEvent guideEvent = currentGuide.Next();
			if (guideEvent != null)
			{
				Show(guideEvent);
				return;
			}
			Time.timeScale = 1f;
            TAudioManager.instance.musicVolume = 0.5f;
            base.gameObject.SetActive(false);
		}
	}

	public static void TriggerGuide(string type)
	{
		if (GameSceneController.Instance.mission_controller is TutorialMissionController)
		{
			GuideController component = GameSceneController.Instance.game_main_panel.transform.parent.Find("GuideUI(Clone)").GetComponent<GuideController>();
			if (type == "Tutorial Over")
			{
				component.Show(new InGameTutorialOverGuide(component));
			}
		}
	}
}
