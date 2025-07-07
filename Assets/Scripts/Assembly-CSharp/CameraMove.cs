using UnityEngine;

public class CameraMove : ICameraMove
{
	public void OnPoisitonMove(Camera camera, Vector3 position)
	{
		camera.transform.position = position;
	}

	public void OnRotationMove(Camera camera, Quaternion rotation)
	{
		camera.transform.rotation = rotation;
	}
}
