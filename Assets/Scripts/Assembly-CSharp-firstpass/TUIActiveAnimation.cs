using UnityEngine;

[RequireComponent(typeof(Animation))]
public class TUIActiveAnimation : TUIIgnoreTimeScale
{
	public string callWhenFinished;

	private Animation mAnim;

	private TUIDirection mLastDirection;

	private TUIDirection mDisableDirection;

	private bool mNotify;

	public void Reset()
	{
		if (!(mAnim != null))
		{
			return;
		}
		foreach (AnimationState item in mAnim)
		{
			if (mLastDirection == TUIDirection.Reverse)
			{
				item.time = item.length;
			}
			else if (mLastDirection == TUIDirection.Forward)
			{
				item.time = 0f;
			}
		}
	}

	private void Update()
	{
		float num = UpdateRealTimeDelta();
		if (mAnim != null)
		{
			bool flag = false;
			foreach (AnimationState item in mAnim)
			{
				float num2 = item.speed * num;
				item.time += num2;
				if (num2 < 0f)
				{
					if (item.time > 0f)
					{
						flag = true;
					}
					else
					{
						item.time = 0f;
					}
				}
				else if (item.time < item.length)
				{
					flag = true;
				}
				else
				{
					item.time = item.length;
				}
			}
			mAnim.enabled = true;
			mAnim.Sample();
			mAnim.enabled = false;
			if (flag)
			{
				return;
			}
			if (mNotify)
			{
				mNotify = false;
				if (!string.IsNullOrEmpty(callWhenFinished))
				{
					SendMessage(callWhenFinished, SendMessageOptions.DontRequireReceiver);
				}
				if (mDisableDirection != 0 && mLastDirection == mDisableDirection)
				{
					base.gameObject.SetActiveRecursively(false);
				}
			}
		}
		base.enabled = false;
	}

	private void Play(string clipName, TUIDirection playDirection)
	{
		if (!(mAnim != null))
		{
			return;
		}
		mAnim.enabled = false;
		if (playDirection == TUIDirection.Toggle)
		{
			playDirection = ((mLastDirection != TUIDirection.Forward) ? TUIDirection.Forward : TUIDirection.Reverse);
		}
		if (string.IsNullOrEmpty(clipName))
		{
			if (!mAnim.isPlaying)
			{
				mAnim.Play();
			}
		}
		else if (!mAnim.IsPlaying(clipName))
		{
			mAnim.Play(clipName);
		}
		foreach (AnimationState item in mAnim)
		{
			if (string.IsNullOrEmpty(clipName) || item.name == clipName)
			{
				float num = Mathf.Abs(item.speed);
				item.speed = num * (float)playDirection;
				if (playDirection == TUIDirection.Reverse && item.time == 0f)
				{
					item.time = item.length;
				}
				else if (playDirection == TUIDirection.Forward && item.time == item.length)
				{
					item.time = 0f;
				}
			}
		}
		mLastDirection = playDirection;
		mNotify = true;
	}

	public static TUIActiveAnimation Play(Animation anim, string clipName, TUIDirection playDirection, TUIEnableCondition enableBeforePlay, TUIDisableCondition disableCondition)
	{
		if (!anim.gameObject.active)
		{
			if (enableBeforePlay != TUIEnableCondition.EnableThenPlay)
			{
				return null;
			}
			anim.gameObject.SetActiveRecursively(true);
		}
		TUIActiveAnimation tUIActiveAnimation = anim.GetComponent<TUIActiveAnimation>();
		if (tUIActiveAnimation != null)
		{
			tUIActiveAnimation.enabled = true;
		}
		else
		{
			tUIActiveAnimation = anim.gameObject.AddComponent<TUIActiveAnimation>();
		}
		tUIActiveAnimation.mAnim = anim;
		tUIActiveAnimation.mDisableDirection = (TUIDirection)disableCondition;
		tUIActiveAnimation.Play(clipName, playDirection);
		return tUIActiveAnimation;
	}

	public static TUIActiveAnimation Play(Animation anim, string clipName, TUIDirection playDirection)
	{
		return Play(anim, clipName, playDirection, TUIEnableCondition.DoNothing, TUIDisableCondition.DoNotDisable);
	}

	public static TUIActiveAnimation Play(Animation anim, TUIDirection playDirection)
	{
		return Play(anim, null, playDirection, TUIEnableCondition.DoNothing, TUIDisableCondition.DoNotDisable);
	}
}
