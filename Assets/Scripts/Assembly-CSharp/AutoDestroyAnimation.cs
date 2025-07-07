using UnityEngine;

public class AutoDestroyAnimation : MonoBehaviour
{
	public string ani_name = string.Empty;

	private void Start()
	{
		AnimationUtil.PlayAnimate(base.gameObject, ani_name, WrapMode.ClampForever);
	}

	private void Update()
	{
		if (AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ani_name, 1f))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
