using UnityEngine;

[RequireComponent(typeof(Animation))]
[AddComponentMenu("TUI/Control/Frame/Page")]
public class TUIPageEx : TUIControlImpl
{
	public enum PlayingState
	{
		IDLE,
		FORWARD,
		FORWARD_ROLLBACK,
		BACKWARD,
		BACKWARD_ROLLBACK
	}

	[SerializeField]
	private AnimationClip clipForward;

	[SerializeField]
	private AnimationClip clipBackward;

	private PlayingState playingState;

	public TUIPageFrameEx PageFrame { get; set; }

	public Animation Animation
	{
		get
		{
			return GetComponent<Animation>();
		}
	}

	public bool IsInAnimationPlaying
	{
		get
		{
			return playingState != PlayingState.IDLE;
		}
	}

	public void PlayForward(float timePercent = 0f, bool reback = false)
	{
		playingState = ((!reback) ? PlayingState.FORWARD : PlayingState.FORWARD_ROLLBACK);
		AnimationState animationState = Animation[clipForward.name];
		animationState.time = animationState.length * timePercent;
		if (PageFrame != null && PageFrame.IsManualTime)
		{
			if (reback)
			{
				TUIActiveAnimation tUIActiveAnimation = TUIActiveAnimation.Play(Animation, clipForward.name, TUIDirection.Reverse);
				tUIActiveAnimation.callWhenFinished = "OnAnimationBegin";
			}
			else if (timePercent >= 1f)
			{
				OnAnimationEnd();
			}
			else
			{
				TUIActiveAnimation tUIActiveAnimation = TUIActiveAnimation.Play(Animation, clipForward.name, TUIDirection.Forward);
				tUIActiveAnimation.callWhenFinished = "OnAnimationEnd";
			}
		}
		else
		{
			if (reback)
			{
				animationState.speed = -1f;
			}
			else
			{
				animationState.speed = 1f;
			}
			Animation.enabled = true;
			Animation.Play(clipForward.name);
		}
		if (PageFrame != null)
		{
			PageFrame.OnPagePlayBegin(this, playingState);
		}
	}

	public void PlayBackward(float timePercent = 0f, bool reback = false)
	{
		playingState = ((!reback) ? PlayingState.BACKWARD : PlayingState.BACKWARD_ROLLBACK);
		AnimationState animationState = Animation[clipBackward.name];
		animationState.time = animationState.length * timePercent;
		if (PageFrame != null && PageFrame.IsManualTime)
		{
			if (reback)
			{
				TUIActiveAnimation tUIActiveAnimation = TUIActiveAnimation.Play(Animation, clipBackward.name, TUIDirection.Reverse);
				tUIActiveAnimation.callWhenFinished = "OnAnimationBegin";
			}
			else if (timePercent >= 1f)
			{
				OnAnimationEnd();
			}
			else
			{
				TUIActiveAnimation tUIActiveAnimation = TUIActiveAnimation.Play(Animation, clipBackward.name, TUIDirection.Forward);
				tUIActiveAnimation.callWhenFinished = "OnAnimationEnd";
			}
		}
		else
		{
			if (reback)
			{
				animationState.speed = -1f;
			}
			else
			{
				animationState.speed = 1f;
			}
			Animation.enabled = true;
			Animation.Play(clipBackward.name);
		}
		if (PageFrame != null)
		{
			PageFrame.OnPagePlayBegin(this, playingState);
		}
	}

	public void TrackForward(float timePercent)
	{
		clipForward.SampleAnimation(base.gameObject, clipForward.length * timePercent);
	}

	public void TrackBackward(float timePercent)
	{
		clipBackward.SampleAnimation(base.gameObject, clipBackward.length * timePercent);
	}

	public void Init(TUIPageFrameEx frame)
	{
		PageFrame = frame;
	}

	public void Start()
	{
	}

	private void OnAnimationEnd()
	{
		if (playingState == PlayingState.FORWARD || playingState == PlayingState.BACKWARD)
		{
			if (PageFrame != null)
			{
				PageFrame.OnPagePlayEnd(this, playingState);
			}
			playingState = PlayingState.IDLE;
		}
	}

	private void OnAnimationBegin()
	{
		if (playingState == PlayingState.FORWARD_ROLLBACK || playingState == PlayingState.BACKWARD_ROLLBACK)
		{
			if (PageFrame != null)
			{
				PageFrame.OnPagePlayEnd(this, playingState);
			}
			playingState = PlayingState.IDLE;
		}
	}
}
