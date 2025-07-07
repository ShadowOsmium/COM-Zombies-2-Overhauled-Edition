using UnityEngine;

public class NearestTargetInfo
{
	public enum NearestTargetType
	{
		None,
		Enemy,
		Box
	}

	public NearestTargetType type;

	public Transform transform;

	public Vector2 screenPos = Vector2.zero;

	public ObjectController target_obj;

	public bool enabel;

	public Vector3 LockPosition
	{
		get
		{
			if (type == NearestTargetType.Enemy || type == NearestTargetType.Box)
			{
				return target_obj.centroid;
			}
			return transform.position;
		}
	}
}
