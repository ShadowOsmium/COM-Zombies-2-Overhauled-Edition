using UnityEngine;

public class CameraMoveFocalize : ICameraMove
{
	private float focalize;

	public float modify = 10f;

	public void OnPoisitonMove(Camera camera, Vector3 position)
	{
		camera.transform.position = position;
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			focalize += axis * modify;
		}
		camera.transform.position += camera.transform.forward.normalized * focalize;
	}

	public void OnRotationMove(Camera camera, Quaternion rotation)
	{
		camera.transform.rotation = rotation;
	}
}
