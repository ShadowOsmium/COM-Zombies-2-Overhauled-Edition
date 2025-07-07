using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class TUIPage : TUIControlImpl
{
	public enum AnimationFlag
	{
		ForwardBringIn,
		ForwardDismiss,
		BackBringIn,
		BackDismiss
	}

	public delegate void AnimationEnd(TUIPage page);

	private Animation anim;

	private Dictionary<AnimationFlag, string> flag2name;

	private AnimationEnd del;

	private AnimationEnd rebackDel;

	public Animation Animation
	{
		get
		{
			if (null == anim)
			{
				anim = GetComponent<Animation>();
			}
			return anim;
		}
	}

	public void AddAnimationEnd(AnimationEnd del, bool isReback)
	{
		if (!isReback)
		{
			this.del = (AnimationEnd)Delegate.Combine(this.del, del);
		}
		else
		{
			rebackDel = (AnimationEnd)Delegate.Combine(rebackDel, del);
		}
	}

	public void RemoveAnimationEnd(AnimationEnd del, bool isReback)
	{
		if (!isReback)
		{
			this.del = (AnimationEnd)Delegate.Remove(this.del, del);
		}
		else
		{
			rebackDel = (AnimationEnd)Delegate.Remove(rebackDel, del);
		}
	}

	private void DoWhenAnimationEnd()
	{
		if (del != null)
		{
			del(this);
		}
	}

	private void DoWhenRebackAnimationEnd()
	{
		if (rebackDel != null)
		{
			rebackDel(this);
		}
	}

	public string GetAnimationName(AnimationFlag flag)
	{
		if (flag2name == null)
		{
			flag2name = new Dictionary<AnimationFlag, string>();
			IEnumerator enumerator = Animation.GetEnumerator();
			enumerator.Reset();
			for (int i = 0; i < 4; i++)
			{
				enumerator.MoveNext();
				AnimationFlag key = (AnimationFlag)(int)Enum.ToObject(typeof(AnimationFlag), i);
				flag2name.Add(key, ((AnimationState)enumerator.Current).name);
			}
		}
		return flag2name[flag];
	}

	private void Start()
	{
		AnimationEvent animationEvent = new AnimationEvent();
		AnimationClip clip = base.GetComponent<Animation>()[GetAnimationName(AnimationFlag.ForwardBringIn)].clip;
		animationEvent.functionName = "DoWhenAnimationEnd";
		animationEvent.time = clip.length;
		clip.AddEvent(animationEvent);
		AnimationEvent animationEvent2 = new AnimationEvent();
		animationEvent2.functionName = "DoWhenRebackAnimationEnd";
		animationEvent2.time = 0f;
		clip.AddEvent(animationEvent2);
		animationEvent = new AnimationEvent();
		clip = base.GetComponent<Animation>()[GetAnimationName(AnimationFlag.ForwardDismiss)].clip;
		animationEvent.functionName = "DoWhenAnimationEnd";
		animationEvent.time = clip.length;
		clip.AddEvent(animationEvent);
		animationEvent2 = new AnimationEvent();
		animationEvent2.functionName = "DoWhenRebackAnimationEnd";
		animationEvent2.time = 0f;
		clip.AddEvent(animationEvent2);
		animationEvent = new AnimationEvent();
		clip = base.GetComponent<Animation>()[GetAnimationName(AnimationFlag.BackBringIn)].clip;
		animationEvent.functionName = "DoWhenAnimationEnd";
		animationEvent.time = clip.length;
		clip.AddEvent(animationEvent);
		animationEvent2 = new AnimationEvent();
		animationEvent2.functionName = "DoWhenRebackAnimationEnd";
		animationEvent2.time = 0f;
		clip.AddEvent(animationEvent2);
		animationEvent = new AnimationEvent();
		clip = base.GetComponent<Animation>()[GetAnimationName(AnimationFlag.BackDismiss)].clip;
		animationEvent.functionName = "DoWhenAnimationEnd";
		animationEvent.time = clip.length;
		clip.AddEvent(animationEvent);
		animationEvent2 = new AnimationEvent();
		animationEvent2.functionName = "DoWhenRebackAnimationEnd";
		animationEvent2.time = 0f;
		clip.AddEvent(animationEvent2);
		TUIActiveAnimation tUIActiveAnimation = GetComponent<TUIActiveAnimation>();
		if (null == tUIActiveAnimation)
		{
			tUIActiveAnimation = base.gameObject.AddComponent<TUIActiveAnimation>();
		}
		tUIActiveAnimation.callWhenFinished = "DoWhenAnimationEnd";
	}

	private void Update()
	{
	}
}
