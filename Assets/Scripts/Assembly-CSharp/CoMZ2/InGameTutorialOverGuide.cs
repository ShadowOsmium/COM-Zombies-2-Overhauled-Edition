namespace CoMZ2
{
	public class InGameTutorialOverGuide : IGuideEvent
	{
		private GuideController controller;

		public InGameTutorialOverGuide(GuideController controller)
		{
			this.controller = controller;
		}

		public string GuideText()
		{
			return "Congratulations, you've finished this tutorial! Now begin your journey to kill zombies.";
		}

		public IGuideEvent Next()
		{
			GameSceneController.Instance.tutorial_ui_over = true;
			return null;
		}

		public void DoSomething()
		{
		}
	}
}
