using System;
using System.Collections.Generic;
using UnityEngine;

public class TUIInputHandler : MonoBehaviour
{
	[Serializable]
	public class TUIInputHandle
	{
		public TUIInputHandleModel tui;

		public int priority;

		public bool penetrate;
	}

	public TUIInputHandle[] handles;

	private List<TUIInputHandle> mHandles = new List<TUIInputHandle>();

	public void AddHandle(TUIInputHandle handle)
	{
		List<TUIInputHandle> list = ((handles != null) ? new List<TUIInputHandle>(handles) : new List<TUIInputHandle>());
		list.Add(handle);
		handles = list.ToArray();
		mHandles.Add(handle);
		Sort();
	}

	private void Sort()
	{
		mHandles.RemoveAll((TUIInputHandle handle) => handle.tui == null);
		mHandles.Sort(delegate(TUIInputHandle handle1, TUIInputHandle handle2)
		{
			if (handle1.priority < handle2.priority)
			{
				return 1;
			}
			return (handle1.priority != handle2.priority) ? (-1) : 0;
		});
	}

	private void Awake()
	{
		if (handles != null)
		{
			mHandles.AddRange(handles);
		}
		Sort();
	}

	private void Update()
	{
		TUIInput[] input = TUIInputManager.GetInput();
		TUIInput[] array = input;
		foreach (TUIInput input2 in array)
		{
			for (int j = 0; j < mHandles.Count && (!mHandles[j].tui.HandleInput(input2) || mHandles[j].penetrate); j++)
			{
			}
		}
	}
}
