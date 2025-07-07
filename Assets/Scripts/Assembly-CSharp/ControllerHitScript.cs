using UnityEngine;

public class ControllerHitScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		string text = hit.collider.name;
		if (!text.StartsWith("E_"))
		{
		}
	}
}
