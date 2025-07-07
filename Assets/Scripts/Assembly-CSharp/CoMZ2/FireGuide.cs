using UnityEngine;

namespace CoMZ2
{
	public class FireGuide : IGuideEvent
	{
		private GuideController controller;

		private Transform fireStick;

		private Vector3 fireStickOriginPos;

		public FireGuide(GuideController controller)
		{
			this.controller = controller;
		}

		public string GuideText()
		{
			return "Touch the firestick to fire!";
		}

        public IGuideEvent Next()
        {
            fireStick.localPosition = fireStickOriginPos;
            fireStick.GetComponent<TUIButton>().enabled = true;
            return new SkillUseGuide(controller);
        }


        public void DoSomething()
		{
			GameSceneController.Instance.game_main_panel.gameObject.SetActive(true);
			fireStick = GameSceneController.Instance.game_main_panel.transform.Find("Fire_Stick");
			controller.Arrow.transform.position = new Vector3(fireStick.position.x, fireStick.position.y + 60f, controller.Arrow.transform.position.z);
			fireStickOriginPos = fireStick.localPosition;
			fireStick.position = new Vector3(fireStick.position.x, fireStick.position.y, controller.Arrow.transform.position.z + 0.5f);
			fireStick.GetComponent<TUIButton>().enabled = false;
		}
	}
}
