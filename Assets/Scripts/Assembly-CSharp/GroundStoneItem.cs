using UnityEngine;

public class GroundStoneItem : PoolItem
{
	private GameObject stone_1;

	private GameObject stone_2;

	protected override void Awake()
	{
		base.Awake();
		stone_1 = base.transform.Find("Groud_Cranny").gameObject;
		stone_2 = base.transform.Find("Groud_Stone").gameObject;
	}

	public override void OnItemCreate()
	{
		base.OnItemCreate();
		AnimationUtil.Stop(stone_1);
		AnimationUtil.Stop(stone_2);
	}

	public override void OnItemActive()
	{
		base.OnItemActive();
		AnimationUtil.PlayAnimate(stone_1, "01", WrapMode.Once);
		AnimationUtil.PlayAnimate(stone_2, "01", WrapMode.Once);
	}

	public override void OnItemDeactive()
	{
		base.OnItemDeactive();
		AnimationUtil.Stop(stone_1);
		AnimationUtil.Stop(stone_2);
	}
}
