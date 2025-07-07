using UnityEngine;

public class NetworkTransformReceiver : MonoBehaviour
{
	private Transform thisTransform;

	private NetworkTransformInterpolation interpolator;

	private void Awake()
	{
		thisTransform = base.transform;
		interpolator = GetComponent<NetworkTransformInterpolation>();
		if (interpolator != null)
		{
			interpolator.StartReceiving();
		}
	}

	public void ReceiveTransform(NetworkTransform ntransform)
	{
		if (interpolator != null)
		{
			interpolator.ReceivedTransform(ntransform);
			return;
		}
		thisTransform.position = ntransform.Position;
		thisTransform.localEulerAngles = ntransform.AngleRotationFPS;
	}

	public void SetReceive(bool bReceive)
	{
		if (bReceive)
		{
			interpolator.StartReceiving();
		}
		else
		{
			interpolator.StopReceiving();
		}
	}

	public void SetInterpolatorMode(NetworkTransformInterpolation.InterpolationMode mode)
	{
		interpolator.mode = mode;
	}
}
