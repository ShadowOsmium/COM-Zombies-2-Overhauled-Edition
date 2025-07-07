using UnityEngine;

public class TapjoyEvent
{
	private string myGuid;

	private string myName;

	private string myParameter;

	private ITapjoyEvent myCallback;

	public TapjoyEvent(string eventName)
		: this(eventName, null)
	{
	}

	public TapjoyEvent(string eventName, string eventParameter)
	{
		myName = eventName;
		myParameter = eventParameter;
		myGuid = TapjoyPlugin.CreateEvent(this, eventName, eventParameter);
		Debug.Log(string.Format("C#: Event {0} created with GUID:{1} with Param:{2}", myName, myGuid, myParameter));
	}

	public void Send(ITapjoyEvent callback)
	{
		Debug.Log(string.Format("C#: Sending event {0} ", myName));
		myCallback = callback;
		TapjoyPlugin.SendEvent(myGuid);
	}

	public void Show()
	{
		TapjoyPlugin.ShowEvent(myGuid);
	}

	public void TriggerSendEventSucceeded(bool contentIsAvailable)
	{
		Debug.Log("C#: TriggerSendEventSucceeded");
		myCallback.SendEventSucceeded(this, contentIsAvailable);
	}

	public void TriggerSendEventFailed(string errorMsg)
	{
		Debug.Log("C#: TriggerSendEventFailed");
		myCallback.SendEventFailed(this, errorMsg);
	}
}
