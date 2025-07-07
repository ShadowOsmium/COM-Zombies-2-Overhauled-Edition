public interface ITapjoyEvent
{
	void SendEventSucceeded(TapjoyEvent tapjoyEvent, bool contentIsAvailable);

	void SendEventFailed(TapjoyEvent tapjoyEvent, string error);
}
