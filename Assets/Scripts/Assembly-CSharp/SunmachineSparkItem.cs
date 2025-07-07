using UnityEngine;

public class SunmachineSparkItem : PoolItem
{
	public override void OnItemCreate()
	{
		base.OnItemCreate();
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
	}
}
