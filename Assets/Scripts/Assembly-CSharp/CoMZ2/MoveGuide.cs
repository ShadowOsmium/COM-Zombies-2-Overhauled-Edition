using UnityEngine;

namespace CoMZ2
{
	public class MoveGuide : IGuideEvent
	{
		private GuideController controller;

		public MoveGuide(GuideController controller)
		{
			this.controller = controller;
		}

		public string GuideText()
		{
			return "Touch this area to move your avatar.";
		}

        public IGuideEvent Next()
        {
            controller.Area.SetActive(false);
            controller.Mask.GetComponent<TUIMeshSprite>().m_hideClipObj = null;
            controller.Mask.GetComponent<TUIMeshSprite>().ForceUpdate();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            return new RotateGuide(controller);
        }

        public void DoSomething()
        {
            controller.Area.SetActive(true);
            controller.Area.transform.localPosition = new Vector3(-120f, 0f, -5f);
            controller.Area.GetComponent<GameUIAutoScaledLayout>().SetLocalPosition(new Vector3(-120f, 0f, -5f), true, false, false, false);
            controller.Mask.GetComponent<TUIMeshSprite>().m_hideClipObj = controller.Area;
            controller.Mask.GetComponent<TUIMeshSprite>().ForceUpdate();
            controller.Event_button.transform.localPosition = new Vector3(-240f, 120f, -6f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
