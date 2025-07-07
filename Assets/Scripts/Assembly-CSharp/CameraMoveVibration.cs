using UnityEngine;

public class CameraMoveVibration : ICameraMove
{
	protected DampedVibration dv = new DampedVibration();

	protected float time;

	public void OnPoisitonMove(Camera camera, Vector3 position)
	{
		time += Time.deltaTime;
		dv.SetParameter(3f, 0.1f, 30f, 0f);
		float y = dv.CalculateDistance(time);
		camera.transform.position = position + new Vector3(0f, y, 0f);
	}

	public void OnRotationMove(Camera camera, Quaternion rotation)
	{
		camera.transform.rotation = rotation;
	}
}
