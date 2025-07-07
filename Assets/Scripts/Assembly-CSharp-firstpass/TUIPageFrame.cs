using System;
using System.Collections.Generic;
using UnityEngine;

public class TUIPageFrame : TUIControlImpl
{
	public enum Orientation
	{
		Forward,
		Back
	}

	public delegate void PageFrameChangeHandler(TUIPageFrame pageFrame);

	public TUIGesture forwardGesture;

	public TUIGesture backGesture;

	public float continueThreshold = 0.5f;

	public Vector3 pageInitPos;

	public TUIPage[] scenePages;

	[SerializeField]
	private bool ignoreTimeScale;

	private int animLock;

	private int rebackAnimLock;

	private List<TUIPage> pages = new List<TUIPage>();

	public PageFrameChangeHandler handler;

	public int CurrentPage { get; set; }

	public int Count
	{
		get
		{
			return pages.Count;
		}
	}

	protected bool IsLock
	{
		get
		{
			return animLock > 0 || rebackAnimLock > 0;
		}
	}

	private int PrevPage { get; set; }

	public void AddHandler(PageFrameChangeHandler handler)
	{
		this.handler = (PageFrameChangeHandler)Delegate.Combine(this.handler, handler);
	}

	public void RemoveHandler(PageFrameChangeHandler handler)
	{
		this.handler = (PageFrameChangeHandler)Delegate.Remove(this.handler, handler);
	}

	private void HandlePageAnimEnd(TUIPage page)
	{
		if (animLock > 0)
		{
			animLock--;
		}
	}

	private void HandleRebackPageAnimEnd(TUIPage page)
	{
		if (rebackAnimLock > 0)
		{
			rebackAnimLock--;
		}
	}

	public void Unlock()
	{
		animLock = 0;
		rebackAnimLock = 0;
	}

	private void InitPage(TUIPage page)
	{
		page.transform.position = pageInitPos;
		page.AddAnimationEnd(HandlePageAnimEnd, false);
		page.AddAnimationEnd(HandleRebackPageAnimEnd, true);
	}

	public void Add(TUIPage page)
	{
		pages.Add(page);
		page.transform.position = pageInitPos;
		InitPage(page);
		Publish();
	}

	public void AddRange(params TUIPage[] pages)
	{
		this.pages.AddRange(pages);
		List<TUIPage> list = new List<TUIPage>(pages);
		list.ForEach(delegate(TUIPage page)
		{
			InitPage(page);
		});
		Publish();
	}

	public void Insert(int position, TUIPage page)
	{
		pages.Insert(position, page);
		InitPage(page);
		Publish();
	}

	public void InsertRange(int position, params TUIPage[] pages)
	{
		this.pages.InsertRange(position, pages);
		List<TUIPage> list = new List<TUIPage>(pages);
		list.ForEach(delegate(TUIPage page)
		{
			InitPage(page);
		});
		Publish();
	}

	public void Remove(int position, bool deleteObj)
	{
		if (position >= 0 && position < pages.Count)
		{
			TUIPage tUIPage = pages[position];
			pages.RemoveAt(position);
			tUIPage.RemoveAnimationEnd(HandlePageAnimEnd, false);
			tUIPage.RemoveAnimationEnd(HandleRebackPageAnimEnd, true);
			if (deleteObj)
			{
				UnityEngine.Object.Destroy(tUIPage.gameObject);
			}
			Publish();
		}
	}

	public void Remove(TUIPage page, bool deleteObj)
	{
		int position = pages.FindIndex((TUIPage @object) => @object == page);
		Remove(position, deleteObj);
	}

	public override bool HandleInput(TUIInput input)
	{
		if (!IsLock)
		{
			base.HandleInput(input);
			forwardGesture.HandleInput(input);
			backGesture.HandleInput(input);
			if (forwardGesture.Progress > 0f)
			{
				if (forwardGesture.Ended)
				{
					if (forwardGesture.Progress >= continueThreshold && CurrentPage < Count - 1)
					{
						Forward();
					}
					else
					{
						ForwardReback();
					}
				}
				else
				{
					Forward(forwardGesture.Progress);
				}
			}
			else if (backGesture.Progress > 0f)
			{
				if (backGesture.Ended)
				{
					if (backGesture.Progress >= continueThreshold && CurrentPage > 0)
					{
						Back();
					}
					else
					{
						BackReback();
					}
				}
				else
				{
					Back(backGesture.Progress);
				}
			}
			if ((!forwardGesture.Ended || !backGesture.Ended) && input.inputType != 0)
			{
				ResetChild();
			}
			return !forwardGesture.Penetrate || !backGesture.Penetrate;
		}
		return false;
	}

	public void Forward()
	{
		Forward(false);
	}

	public void Forward(bool immediately)
	{
		int currentPage = CurrentPage;
		CurrentPage++;
		if (CurrentPage < Count)
		{
			if (immediately)
			{
				CurrentPage--;
				Forward(1f);
				CurrentPage++;
			}
			else
			{
				Dismiss(currentPage, Orientation.Forward);
				BringIn(CurrentPage, Orientation.Forward);
			}
			Publish();
		}
		else
		{
			CurrentPage = currentPage;
		}
	}

	private void Forward(float progress)
	{
		Play(CurrentPage, TUIPage.AnimationFlag.ForwardDismiss, progress, true);
		Play(CurrentPage + 1, TUIPage.AnimationFlag.ForwardBringIn, progress, true);
	}

	private void ForwardReback()
	{
		Reback(CurrentPage, TUIPage.AnimationFlag.ForwardDismiss);
		Reback(CurrentPage + 1, TUIPage.AnimationFlag.ForwardBringIn);
	}

	public void Back()
	{
		Back(false);
	}

	public void Back(bool immediately)
	{
		int currentPage = CurrentPage;
		CurrentPage--;
		if (0 <= CurrentPage)
		{
			if (immediately)
			{
				CurrentPage++;
				Back(1f);
				CurrentPage--;
			}
			else
			{
				Dismiss(currentPage, Orientation.Back);
				BringIn(CurrentPage, Orientation.Back);
			}
			Publish();
		}
		else
		{
			CurrentPage = currentPage;
		}
	}

	private void Back(float progress)
	{
		Play(CurrentPage, TUIPage.AnimationFlag.BackDismiss, progress, true);
		Play(CurrentPage - 1, TUIPage.AnimationFlag.BackBringIn, progress, true);
	}

	private void BackReback()
	{
		Reback(CurrentPage, TUIPage.AnimationFlag.BackDismiss);
		Reback(CurrentPage - 1, TUIPage.AnimationFlag.BackBringIn);
	}

	private void BringIn(int pageIndex, Orientation orientation)
	{
		switch (orientation)
		{
		case Orientation.Forward:
			Play(pageIndex, TUIPage.AnimationFlag.ForwardBringIn);
			break;
		case Orientation.Back:
			Play(pageIndex, TUIPage.AnimationFlag.BackBringIn);
			break;
		}
	}

	private void Dismiss(int pageIndex, Orientation orientation)
	{
		switch (orientation)
		{
		case Orientation.Forward:
			Play(pageIndex, TUIPage.AnimationFlag.ForwardDismiss);
			break;
		case Orientation.Back:
			Play(pageIndex, TUIPage.AnimationFlag.BackDismiss);
			break;
		}
	}

	private void Play(int pageIndex, TUIPage.AnimationFlag flag)
	{
		if (0 <= pageIndex && pageIndex < Count)
		{
			TUIPage tUIPage = pages[pageIndex];
			string animationName = tUIPage.GetAnimationName(flag);
			float speed = tUIPage.Animation[animationName].speed;
			if (speed < 0f)
			{
				speed *= -1f;
			}
			if (ignoreTimeScale)
			{
				TUIActiveAnimation.Play(tUIPage.Animation, animationName, TUIDirection.Forward);
			}
			else
			{
				tUIPage.Animation.Play(animationName);
			}
			animLock++;
		}
	}

	private void Play(int pageIndex, TUIPage.AnimationFlag flag, float progress, bool isSample)
	{
		if (progress >= -1f && progress <= 1f && 0 <= pageIndex && pageIndex < Count)
		{
			TUIPage tUIPage = pages[pageIndex];
			string animationName = tUIPage.GetAnimationName(flag);
			AnimationState animationState = tUIPage.Animation[animationName];
			float num = animationState.speed;
			if ((progress < 0f && num > 0f) || (progress > 0f && num < 0f))
			{
				num *= -1f;
			}
			animationState.speed = num;
			animationState.time = animationState.length * Mathf.Abs(progress);
			if (isSample)
			{
				animationState.clip.SampleAnimation(tUIPage.gameObject, animationState.time);
			}
		}
	}

	private void Reback(int pageIndex, TUIPage.AnimationFlag flag)
	{
		if (0 <= pageIndex && pageIndex < Count)
		{
			TUIPage tUIPage = pages[pageIndex];
			string animationName = tUIPage.GetAnimationName(flag);
			float num = tUIPage.Animation[animationName].speed;
			if (num > 0f)
			{
				num *= -1f;
			}
			tUIPage.Animation[animationName].speed = num;
			if (ignoreTimeScale)
			{
				TUIActiveAnimation.Play(tUIPage.Animation, animationName, TUIDirection.Reverse);
			}
			else
			{
				tUIPage.Animation.Play(animationName);
			}
			if (!ignoreTimeScale)
			{
				rebackAnimLock++;
			}
			else
			{
				animLock++;
			}
		}
	}

	private void Awake()
	{
		if (scenePages != null)
		{
			CurrentPage = -1;
			if (scenePages != null)
			{
				AddRange(scenePages);
			}
		}
	}

	private void Publish()
	{
		if (handler != null)
		{
			handler(this);
		}
	}
}
