namespace CoMZ2
{
	public class DestroyBoxGuide : IGuideEvent
	{
		private GuideController controller;

		public DestroyBoxGuide(GuideController controller)
		{
			this.controller = controller;
		}

		public string GuideText()
		{
			return "Now let's hunt treasures. Try to explore boxes in this scene and destroy them to get treasures.";
		}

		public IGuideEvent Next()
		{
			return null;
		}

		public void DoSomething()
		{
		}
	}
}
