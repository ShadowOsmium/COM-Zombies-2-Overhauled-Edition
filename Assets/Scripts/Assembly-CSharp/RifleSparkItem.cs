using UnityEngine;

public class RifleSparkItem : PoolItem
{
	public override void OnItemCreate()
	{
		base.OnItemCreate();
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
	}
}
