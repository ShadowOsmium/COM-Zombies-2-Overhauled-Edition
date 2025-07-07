using UnityEngine;

public class RoamEvent : MonoBehaviour, IRoamEvent
{
	public void OnRoamTrigger()
	{
		Debug.Log(base.gameObject.name + ":RoamTrigger");
	}

	public void OnRoamStop()
	{
		Debug.Log(base.gameObject.name + ":RoamStop");
	}
}
