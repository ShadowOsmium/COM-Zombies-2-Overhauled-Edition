using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TUI/Control/Frame/Frame")]
public class TUIPageFrameEx : TUIControlImpl
{
	[SerializeField]
	private List<TUIPageEx> pages = new List<TUIPageEx>();

	[SerializeField]
	private int currentPageIndex;

	[SerializeField]
	private TUIPageGestureEx gesture;

	[SerializeField]
	private bool ignoreTimeScale;

	[SerializeField]
	private int handleLock;

	public TUIPageEx CurrentPage
	{
		get
		{
			if (pages.Count > 0 && currentPageIndex >= 0 && currentPageIndex < pages.Count)
			{
				return pages[currentPageIndex];
			}
			return null;
		}
	}

	public bool IsLock
	{
		get
		{
			return handleLock > 0;
		}
	}

	public bool IsManualTime
	{
		get
		{
			return ignoreTimeScale;
		}
		set
		{
			ignoreTimeScale = value;
		}
	}

	public TUIPageEx this[int index]
	{
		get
		{
			return pages[index];
		}
	}

	private void Awake()
	{
		currentPageIndex = 0;
		foreach (TUIPageEx page in pages)
		{
			InitPage(page);
		}
	}

	public void AddPage(TUIPageEx page)
	{
		pages.Add(page);
		InitPage(page);
	}

	public void AddRange(params TUIPageEx[] pages)
	{
		this.pages.AddRange(pages);
		foreach (TUIPageEx page in pages)
		{
			InitPage(page);
		}
	}

	public void Insert(int position, TUIPageEx page)
	{
		pages.Insert(position, page);
		InitPage(page);
	}

	public void InsertRange(int position, params TUIPageEx[] pages)
	{
		this.pages.InsertRange(position, pages);
		foreach (TUIPageEx page in pages)
		{
			InitPage(page);
		}
	}

	public void Remove(TUIPageEx page, bool destroy = false)
	{
		pages.Remove(page);
		Release(page, destroy);
	}

	public void Remove(int position, bool destroy = false)
	{
		TUIPageEx page = pages[position];
		pages.RemoveAt(position);
		Release(page, destroy);
	}

	public bool Forward(float time = 0f)
	{
		if (IsLock)
		{
			return false;
		}
		if (currentPageIndex + 1 < pages.Count)
		{
			currentPageIndex++;
			foreach (TUIPageEx page in pages)
			{
				page.PlayForward(Mathf.Clamp01(time));
			}
			return true;
		}
		return RollBackFromForward(Mathf.Clamp01(time));
	}

	public bool Backward(float time = 0f)
	{
		if (IsLock)
		{
			return false;
		}
		if (currentPageIndex - 1 >= 0)
		{
			currentPageIndex--;
			foreach (TUIPageEx page in pages)
			{
				page.PlayBackward(Mathf.Clamp01(time));
			}
			return true;
		}
		return RollBackFromBackward(Mathf.Clamp01(time));
	}

	protected bool RollBackFromForward(float time)
	{
		if (IsLock)
		{
			return false;
		}
		foreach (TUIPageEx page in pages)
		{
			page.PlayForward(time, true);
		}
		return true;
	}

	protected bool RollBackFromBackward(float time)
	{
		if (IsLock)
		{
			return false;
		}
		foreach (TUIPageEx page in pages)
		{
			page.PlayBackward(time, true);
		}
		return true;
	}

	protected void ForwardIncomplete(float timePercent)
	{
		if (IsLock)
		{
			return;
		}
		foreach (TUIPageEx page in pages)
		{
			page.TrackForward(timePercent);
		}
	}

	protected void BackwardIncomplete(float timePercent)
	{
		if (IsLock)
		{
			return;
		}
		foreach (TUIPageEx page in pages)
		{
			page.TrackBackward(timePercent);
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		if (IsLock)
		{
			if (null != gesture)
			{
				gesture.HandlePageFrameLock();
			}
			return false;
		}
		if (gesture != null)
		{
			bool flag = gesture.HandleInput(input);
			float currentProgress = gesture.CurrentProgress;
			if (currentProgress > 0f)
			{
				if (gesture.IsGesturing)
				{
					ForwardIncomplete(currentProgress);
				}
				else if (currentProgress >= gesture.ForwardProgress)
				{
					Forward(currentProgress);
				}
				else
				{
					RollBackFromForward(currentProgress);
				}
			}
			else if (currentProgress < 0f)
			{
				if (gesture.IsGesturing)
				{
					BackwardIncomplete(0f - currentProgress);
				}
				else if (0f - currentProgress >= gesture.BackwardProgress)
				{
					Backward(0f - currentProgress);
				}
				else
				{
					RollBackFromBackward(0f - currentProgress);
				}
			}
			if (flag)
			{
				if (CurrentPage != null)
				{
					CurrentPage.ResetChild();
				}
				return true;
			}
		}
		if (CurrentPage != null && CurrentPage.HandleInput(input))
		{
			return true;
		}
		return false;
	}

	public void OnPagePlayBegin(TUIPageEx page, TUIPageEx.PlayingState state)
	{
		handleLock++;
	}

	public void OnPagePlayEnd(TUIPageEx page, TUIPageEx.PlayingState state)
	{
		StartCoroutine(coordinate_OnPagePlayEnd(page, state));
	}

	private GameObject GetPageHandle(TUIPageEx page)
	{
		return page.transform.parent.gameObject;
	}

	private void InitPage(TUIPageEx page)
	{
		GameObject gameObject = new GameObject("PageHandle");
		gameObject.AddComponent<TUIControl>();
		gameObject.transform.parent = base.gameObject.transform;
		page.transform.parent = gameObject.transform;
		gameObject.transform.localPosition = page.transform.localPosition;
		page.transform.localPosition = Vector3.zero;
		page.Init(this);
	}

	private void Release(TUIPageEx page, bool destroy)
	{
		GameObject pageHandle = GetPageHandle(page);
		if (destroy)
		{
			Object.Destroy(pageHandle);
			return;
		}
		page.transform.parent = null;
		Object.Destroy(pageHandle);
	}

	private void AlignPage(TUIPageEx page)
	{
		page.transform.parent.localPosition += page.transform.localPosition;
		page.transform.localPosition = Vector3.zero;
	}

	private IEnumerator coordinate_OnPagePlayEnd(TUIPageEx page, TUIPageEx.PlayingState state)
	{
		yield return 0;
		AlignPage(page);
		handleLock--;
	}
}
