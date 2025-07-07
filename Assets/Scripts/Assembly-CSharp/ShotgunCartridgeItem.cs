using UnityEngine;

public class ShotgunCartridgeItem : PoolItem
{
	private GameObject cartridge;

	protected override void Awake()
	{
		base.Awake();
		cartridge = base.transform.Find("Cartridge_case_01").gameObject;
	}

	public override void OnItemCreate()
	{
		base.OnItemCreate();
		AnimationUtil.Stop(cartridge);
	}

	public override void OnItemActive()
	{
		base.OnItemActive();
		switch (Random.Range(0, 100) % 3)
		{
		case 0:
			AnimationUtil.PlayAnimate(cartridge, "01", WrapMode.Once);
			break;
		case 1:
			AnimationUtil.PlayAnimate(cartridge, "02", WrapMode.Once);
			break;
		default:
			AnimationUtil.PlayAnimate(cartridge, "03", WrapMode.Once);
			break;
		}
	}

	public override void OnItemDeactive()
	{
		base.OnItemDeactive();
		AnimationUtil.Stop(cartridge);
	}
}
