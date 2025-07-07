using System;
using System.Collections.Generic;
using UnityEngine;

public class TUIControl : MonoBehaviour
{
	public const string ResetMethod = "OnReset";

	public bool invokeOnEvent;

	public GameObject invokeObject;

	public string componentName;

	public string invokeFunctionName;

	public object data;

	public virtual void Reset()
	{
		ResetChild();
		PostMessage("OnReset", null, SendMessageOptions.DontRequireReceiver);
	}

	public void ResetChild()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			TUIControl component = base.transform.GetChild(i).gameObject.GetComponent<TUIControl>();
			if ((bool)component && component.gameObject.active && component.enabled)
			{
				component.Reset();
			}
		}
	}

	public virtual void Activate(bool bActive)
	{
	}

	public virtual bool HandleInput(TUIInput input)
	{
		List<TUIControl> list = new List<TUIControl>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			TUIControl component = base.transform.GetChild(i).gameObject.GetComponent<TUIControl>();
			if ((bool)component && component.gameObject.active && component.enabled)
			{
				list.Add(component);
			}
		}
		TUIControl[] array = list.ToArray();
		Array.Sort(array, CompareControl);
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].HandleInput(input))
			{
				return true;
			}
		}
		return false;
	}

	public virtual void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		PostEvent(control, eventType, wparam, lparam, data);
	}

	public virtual void PostEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		TUIControl component = base.transform.parent.gameObject.GetComponent<TUIControl>();
		if ((bool)component)
		{
			component.HandleEvent(control, eventType, wparam, lparam, data);
		}
	}

	protected T GetControl<T>(string name) where T : TUIControl
	{
		Transform transform = base.transform.Find(name);
		if (!transform)
		{
			return (T)null;
		}
		return transform.gameObject.GetComponent<T>();
	}

	protected static int CompareControl(TUIControl l, TUIControl r)
	{
		if (l.transform.position.z < r.transform.position.z)
		{
			return -1;
		}
		if (l.transform.position.z > r.transform.position.z)
		{
			return 1;
		}
		return 0;
	}

	protected void PostMessage(string methodName, object param, SendMessageOptions option)
	{
		if (param == null)
		{
			SendMessage(methodName, option);
		}
		else
		{
			SendMessage(methodName, param, option);
		}
	}

	protected List<TUIControl> GetChildren(bool includeInactive)
	{
		List<TUIControl> list = new List<TUIControl>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			TUIControl component = base.transform.GetChild(i).gameObject.GetComponent<TUIControl>();
			if (null != component && (includeInactive || (component.gameObject.active && component.enabled)))
			{
				list.Add(component);
			}
		}
		list.Sort(CompareControl);
		return list;
	}
}
