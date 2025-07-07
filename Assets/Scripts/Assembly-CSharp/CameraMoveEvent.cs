using UnityEngine;

public class CameraMoveEvent : MonoBehaviour, IRoamEvent
{
	public CameraMoveType type;

	protected CameraRoam roam;

	public void OnRoamTrigger()
	{
		roam = base.gameObject.GetComponent<CameraRoam>();
		if (roam != null)
		{
			switch (type)
			{
			case CameraMoveType.Normal:
				roam.SetCameraMove(new CameraMove());
				break;
			case CameraMoveType.Vibration:
				roam.SetCameraMove(new CameraMoveVibration());
				break;
			case CameraMoveType.Focalize:
				roam.SetCameraMove(new CameraMoveFocalize());
				break;
			}
		}
	}

	public void OnRoamStop()
	{
		if (roam != null)
		{
			roam.SetCameraMove(null);
			roam = null;
		}
	}
}
