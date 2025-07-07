namespace CoMZ2
{
	public interface IGuideEvent
	{
		string GuideText();

		IGuideEvent Next();

		void DoSomething();
	}
}
