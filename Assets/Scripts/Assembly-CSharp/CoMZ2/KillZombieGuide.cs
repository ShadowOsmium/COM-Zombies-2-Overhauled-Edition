using UnityEngine;

namespace CoMZ2
{
	public class KillZombieGuide : IGuideEvent
	{
		private GuideController controller;

		private Vector3 arrowPosition;

		public KillZombieGuide(GuideController controller, Vector3 arrowPosition)
		{
			this.controller = controller;
			this.arrowPosition = arrowPosition;
		}

		public string GuideText()
		{
			return "Kill these zombies to gain cash!";
		}

		public IGuideEvent Next()
		{
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return new FireGuide(controller);
		}

        public void DoSomething()
        {
            controller.Arrow.SetActive(true);
            controller.Arrow.transform.localPosition = arrowPosition;
        }
    }
}
