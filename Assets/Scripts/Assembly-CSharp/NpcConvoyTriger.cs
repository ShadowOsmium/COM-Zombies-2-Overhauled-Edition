using CoMZ2;
using UnityEngine;

public class NpcConvoyTriger : MonoBehaviour
{
	public Transform target_trans;

	public FollowerController npc_follower;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerStay(Collider other)
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			npc_follower.StartConvoyed(target_trans);
			Object.Destroy(base.gameObject);
		}
	}
}
