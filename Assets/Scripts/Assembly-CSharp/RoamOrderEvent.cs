using UnityEngine;

public class RoamOrderEvent : MonoBehaviour, IRoamEvent
{
	public RoamOrder m_order;

	public void OnRoamTrigger()
	{
	}

	public void OnRoamStop()
	{
		m_order.Next();
		Object.Destroy(this);
	}
}
