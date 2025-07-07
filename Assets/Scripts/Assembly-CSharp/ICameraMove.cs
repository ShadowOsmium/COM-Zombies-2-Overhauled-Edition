using UnityEngine;

public interface ICameraMove
{
	void OnPoisitonMove(Camera camera, Vector3 position);

	void OnRotationMove(Camera camera, Quaternion rotation);
}
