using UnityEngine;

public class LookAtCameraScript : MonoBehaviour
{
	protected Transform cameraTransform;

	public bool Flip_X;

	private void Start()
	{
		if (Camera.main != null)
		{
			cameraTransform = Camera.main.transform;
		}
	}

	private void Update()
	{
		base.transform.LookAt(cameraTransform);
		if (Flip_X)
		{
			base.transform.forward = cameraTransform.forward;
		}
	}
}
