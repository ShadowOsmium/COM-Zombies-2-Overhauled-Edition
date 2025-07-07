using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Path))]
public class PathEvent : MonoBehaviour
{
	public int nodeId;

	public List<string> events;

	public void Send()
	{
		if (events == null)
		{
			return;
		}
		foreach (string @event in events)
		{
			Debug.Log("SendMessage:" + @event);
			base.gameObject.SendMessage(@event, SendMessageOptions.DontRequireReceiver);
		}
	}
}
