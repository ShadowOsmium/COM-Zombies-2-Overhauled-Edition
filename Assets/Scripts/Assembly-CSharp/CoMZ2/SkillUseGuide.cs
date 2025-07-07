using UnityEngine;

namespace CoMZ2
{
	public class SkillUseGuide : IGuideEvent
	{
		public GuideController controller;

		private Transform skillStick;

		private Vector3 skillStickOriginPos;

		public SkillUseGuide(GuideController controller)
		{
			this.controller = controller;
		}

		public string GuideText()
		{
			return "Touch the skillstick to use skills!";
		}

		public IGuideEvent Next()
		{
			controller.Arrow.SetActive(false);
			skillStick.localPosition = skillStickOriginPos;
			skillStick.GetComponent<TUIButton>().enabled = true;
			return null;
		}

		public void DoSomething()
		{
			skillStick = GameSceneController.Instance.game_main_panel.transform.Find("Skill_Button1");
			controller.Arrow.transform.position = new Vector3(skillStick.position.x, skillStick.position.y + 40f, controller.Arrow.transform.position.z);
			skillStickOriginPos = skillStick.localPosition;
			skillStick.position = new Vector3(skillStick.position.x, skillStick.position.y, controller.Arrow.transform.position.z + 0.5f);
			skillStick.GetComponent<TUIButton>().enabled = false;
		}
	}
}
