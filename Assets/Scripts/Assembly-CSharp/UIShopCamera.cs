using UnityEngine;

public class UIShopCamera : MonoBehaviour
{
	public Transform TarTrans;

	public float PosSpeed = 10f;

	public float RotSpeed = 10f;

	public float SmoothSpeed = 1f;

	private bool isSmooth;

	private float startSmoothTime;

	private Vector3 lastPos = Vector3.zero;

	private Quaternion lastRot = Quaternion.identity;

	private float lerpTime;

	private void Update()
	{
		if (isSmooth && TarTrans != null)
		{
			lerpTime = (Time.time - startSmoothTime) * SmoothSpeed;
			base.GetComponent<Camera>().transform.position = Vector3.Lerp(lastPos, TarTrans.position, lerpTime);
			base.GetComponent<Camera>().transform.rotation = Quaternion.Lerp(lastRot, TarTrans.rotation, lerpTime);
			if (lerpTime >= 1f)
			{
				isSmooth = false;
			}
		}
	}

	public void SmoothCameraTo(Transform target)
	{
		TarTrans = target;
		isSmooth = true;
		startSmoothTime = Time.time;
		lastPos = base.transform.position;
		lastRot = base.transform.rotation;
	}
}
