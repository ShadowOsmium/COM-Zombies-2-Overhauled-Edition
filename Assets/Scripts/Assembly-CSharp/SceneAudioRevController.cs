using CoMZ2;
using UnityEngine;

public class SceneAudioRevController : MonoBehaviour
{
	private void Start()
	{
		GetComponent<AudioReverbZone>().enabled = false;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			GetComponent<AudioReverbZone>().enabled = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			GetComponent<AudioReverbZone>().enabled = false;
		}
	}
}
