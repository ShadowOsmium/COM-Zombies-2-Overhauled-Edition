using CoMZ2;
using UnityEngine;

public class HaokeRushTriger : MonoBehaviour
{
	public bool enable_triger;

	public OnHaokeRushed on_rushed;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (enable_triger && (other.gameObject.layer == PhysicsLayer.PLAYER || other.gameObject.layer == PhysicsLayer.NPC))
		{
			ObjectController component = other.gameObject.GetComponent<ObjectController>();
			if (on_rushed != null && component != null)
			{
				on_rushed(component);
			}
		}
	}
}
