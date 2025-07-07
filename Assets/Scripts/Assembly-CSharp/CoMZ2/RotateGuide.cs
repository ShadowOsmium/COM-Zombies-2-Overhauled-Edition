using UnityEngine;

namespace CoMZ2
{
	public class RotateGuide : IGuideEvent
	{
		private GuideController controller;

		public RotateGuide(GuideController controller)
		{
			this.controller = controller;
		}

		public string GuideText()
		{
			return "Touch this area to rotate your avatar.";
		}

        public IGuideEvent Next()
        {
            controller.Area.SetActive(false);
            controller.Mask.GetComponent<TUIMeshSprite>().m_showClipObj = null;
            controller.Mask.GetComponent<TUIMeshSprite>().ForceUpdate();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            return null;
        }

        public void DoSomething()
        {
            controller.Area.SetActive(true);
            controller.Area.GetComponent<GameUIAutoScaledLayout>().SetLocalPosition(new Vector3(120f, 0f, -5f), false, true, false, false);
            controller.Mask.GetComponent<TUIMeshSprite>().m_showClipObj = controller.Area;
            controller.Mask.GetComponent<TUIMeshSprite>().ForceUpdate();
            controller.Event_button.transform.localPosition = new Vector3(240f, 120f, -6f);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
