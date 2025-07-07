using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TUI/Control/ScrollList")]
public class TUIScrollList : TUIControlImpl
{
	public enum Arrangement
	{
		Horizontal,
		Vertical
	}

	protected const float reboundSpeed = 1f;

	protected const float overscrollAllowance = 0.5f;

	protected const float scrollDecelCoef = 0.4f;

	protected const float lowPassKernelWidthInSeconds = 0.03f;

	protected const float scrollDeltaUpdateInterval = 0.0166f;

	protected const float lowPassFilterFactor = 83f / 150f;

	protected const float backgroundColliderOffset = 0.01f;

	public Arrangement arrangement;

	public float spacing;

	public float threshold;

	public TUIControl[] sencesControls;

	private GameObject mover;

	private List<TUIScrollListObject> list = new List<TUIScrollListObject>();

	[SerializeField]
	protected bool resetCurrentControlWhenMove;

	private TUIControl currentControl;

	public static int CommandDown;

	public static int CommandMove = 1;

	public static int CommandUp = 2;

	protected int fingerId = -1;

	protected Vector2 fingerPosition = Vector2.zero;

	protected bool touch;

	protected bool move;

	protected bool scroll;

	protected Vector2 lastPosition = Vector2.zero;

	protected float contentExtents;

	protected float scrollPos;

	protected float scrollDelta;

	protected float scrollMax;

	protected float scrollInertia;

	protected Vector2 moveSpeed;

	private float lastTime;

	private float timeDelta;

	public override bool HandleInput(TUIInput input)
	{
		bool result = true;
		switch (input.inputType)
		{
		case TUIInputType.Began:
			result = HandleInputBegan(input);
			break;
		case TUIInputType.Moved:
			base.HandleInput(input);
			result = HandleInputMoved(input);
			if (move && resetCurrentControlWhenMove && null != currentControl)
			{
				currentControl.Reset();
			}
			break;
		case TUIInputType.Ended:
			base.HandleInput(input);
			result = HandleInputEnded(input);
			break;
		}
		return result;
	}

	private bool HandleInputBegan(TUIInput input)
	{
		if (PtInControl(input.position))
		{
			bool flag = false;
			foreach (TUIControl tUIControlONListObj in GetTUIControlONListObjs(false))
			{
				if (tUIControlONListObj.HandleInput(input) && !flag)
				{
					currentControl = tUIControlONListObj;
					flag = true;
				}
			}
			fingerId = input.fingerId;
			fingerPosition = input.position;
			touch = true;
			move = false;
			scroll = false;
			lastPosition = fingerPosition;
			moveSpeed = Vector2.zero;
			PostEvent(this, CommandDown, 0f, 0f, input);
		}
		return false;
	}

	private bool HandleInputMoved(TUIInput input)
	{
		if (input.fingerId != fingerId)
		{
			return false;
		}
		bool flag = false;
		float f = input.position.x - fingerPosition.x;
		float f2 = input.position.y - fingerPosition.y;
		if (!move)
		{
			if (!PtInControl(input.position))
			{
				return false;
			}
			switch (arrangement)
			{
			case Arrangement.Horizontal:
				flag = Mathf.Abs(f) > threshold;
				break;
			case Arrangement.Vertical:
				flag = Mathf.Abs(f2) > threshold;
				break;
			}
			move = flag;
		}
		if (move)
		{
			BaseScrollListTo(InputToScrollDelta(input.position, fingerPosition));
			scroll = true;
			fingerPosition = input.position;
		}
		PostEvent(this, CommandMove, 0f, 0f, input);
		return true;
	}

	private bool HandleInputEnded(TUIInput input)
	{
		fingerId = -1;
		fingerPosition = Vector2.zero;
		touch = false;
		move = false;
		PostEvent(this, CommandUp, 0f, 0f, input);
		return false;
	}

	public void Add(TUIControl control)
	{
		Insert(list.Count, control);
	}

	public void AddRange(params TUIControl[] controls)
	{
		InsertRange(list.Count, controls);
	}

	public void Insert(int position, TUIControl control)
	{
		Insert(position, ControlToTUIScrollListObject(control));
	}

	public void InsertRange(int position, params TUIControl[] controls)
	{
		if (controls != null)
		{
			TUIScrollListObject[] array = new TUIScrollListObject[controls.Length];
			for (int i = 0; i < controls.Length; i++)
			{
				array[i] = ControlToTUIScrollListObject(controls[i]);
			}
			InsertRange(position, array);
		}
	}

	public void Remove(int position, bool deleteObj)
	{
		if (position >= 0 && position < list.Count)
		{
			TUIScrollListObject tUIScrollListObject = list[position];
			list.RemoveAt(position);
			if (deleteObj)
			{
				Object.Destroy(tUIScrollListObject.gameObject);
			}
		}
	}

	public void Remove(TUIControl control, bool deleteObj)
	{
		int position = list.FindIndex((TUIScrollListObject @object) => @object == control.GetComponent<TUIScrollListObject>());
		Remove(position, deleteObj);
	}

	public void Clear(bool deleteObj)
	{
		if (deleteObj)
		{
			foreach (TUIScrollListObject item in list)
			{
				Object.Destroy(item.gameObject);
			}
		}
		list.Clear();
		PositionItems();
	}

	private void Insert(int position, TUIScrollListObject obj)
	{
		BaseInsert(position, obj);
		PositionItems();
	}

	private void InsertRange(int position, params TUIScrollListObject[] objs)
	{
		for (int num = objs.Length - 1; num > -1; num--)
		{
			BaseInsert(position, objs[num]);
		}
		PositionItems();
	}

	private void BaseInsert(int position, TUIScrollListObject obj)
	{
		list.Insert(Mathf.Clamp(position, 0, list.Count), obj);
		if (null != mover)
		{
			obj.transform.parent = mover.transform;
		}
	}

	private TUIScrollListObject ControlToTUIScrollListObject(TUIControl control)
	{
		TUIScrollListObject tUIScrollListObject = control.GetComponent<TUIScrollListObject>();
		if (null == tUIScrollListObject)
		{
			tUIScrollListObject = control.gameObject.AddComponent<TUIScrollListObject>();
		}
		return tUIScrollListObject;
	}

	private void PositionItems()
	{
		switch (arrangement)
		{
		case Arrangement.Horizontal:
			PositionHorizontally(false);
			break;
		case Arrangement.Vertical:
			PositionVertically(false);
			break;
		}
		UpdateContentExtents(0f);
	}

	private void PositionHorizontally(bool updateExtents)
	{
		float num = size.x * -0.5f;
		contentExtents = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			list[i].transform.localPosition = new Vector3(num + list[i].Borader.size.x * 0.5f, 0f);
			contentExtents += list[i].Borader.size.x + spacing;
			num += list[i].Borader.size.x + spacing;
		}
	}

	private void PositionVertically(bool updateExtents)
	{
		float num = size.y * 0.5f;
		contentExtents = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			list[i].transform.localPosition = new Vector3(0f, num - list[i].Borader.size.y * 0.5f);
			contentExtents += list[i].Borader.size.y + spacing;
			num -= list[i].Borader.size.y + spacing;
		}
	}

	public void ScrollListTo(float pos)
	{
		scrollInertia = 0f;
		scrollDelta = 0f;
		BaseScrollListTo(pos);
	}

	protected float InputToScrollDelta(Vector3 now, Vector3 prev)
	{
		Vector3 vector = now - prev;
		switch (arrangement)
		{
		case Arrangement.Horizontal:
			scrollDelta = (0f - vector.x) / (contentExtents + spacing - size.x);
			scrollDelta *= base.transform.localScale.x;
			break;
		case Arrangement.Vertical:
			scrollDelta = vector.y / (contentExtents + spacing - size.y);
			scrollDelta *= base.transform.localScale.y;
			break;
		}
		float num = scrollPos + scrollDelta;
		if (num > 1f)
		{
			scrollDelta *= Mathf.Clamp01(1f - (num - 1f) / scrollMax);
		}
		else if (num < 0f)
		{
			scrollDelta *= Mathf.Clamp01(1f + num / scrollMax);
		}
		return scrollPos + scrollDelta;
	}

	public void PointerReleased()
	{
		touch = false;
		if (scrollInertia != 0f)
		{
			scrollDelta = scrollInertia;
		}
		scrollInertia = 0f;
	}

	protected void BaseScrollListTo(float pos)
	{
		if (!float.IsNaN(pos))
		{
			switch (arrangement)
			{
			case Arrangement.Horizontal:
			{
				float num = contentExtents - spacing - size.x;
				mover.transform.localPosition = Vector3.right * Mathf.Clamp(num, 0f, num) * (0f - pos);
				break;
			}
			case Arrangement.Vertical:
			{
				float num = contentExtents - spacing - size.y;
				mover.transform.localPosition = Vector3.up * Mathf.Clamp(num, 0f, num) * pos;
				break;
			}
			}
			scrollPos = pos;
		}
	}

	protected void UpdateContentExtents(float change)
	{
		float num = 1f;
		float num2 = 1f;
		contentExtents += change;
		switch (arrangement)
		{
		case Arrangement.Horizontal:
			num = contentExtents - change + spacing - size.x;
			num2 = contentExtents + spacing - size.x;
			scrollMax = size.x / (contentExtents + spacing - size.x) * 0.5f;
			break;
		case Arrangement.Vertical:
			num = contentExtents - change + spacing - size.y;
			num2 = contentExtents + spacing - size.y;
			scrollMax = size.y / (contentExtents + spacing - size.y) * 0.5f;
			break;
		}
		BaseScrollListTo(Mathf.Clamp01(num * scrollPos / num2));
	}

	private void Awake()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Mover";
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.AddComponent<TUIControl>();
		mover = gameObject;
	}

	private void Start()
	{
		AddRange(sencesControls);
		ScrollListTo(0f);
	}

	private void Update()
	{
		timeDelta = Time.realtimeSinceStartup - lastTime;
		lastTime = Time.realtimeSinceStartup;
		if (scroll && !touch)
		{
			scrollDelta -= scrollDelta * 0.4f * (timeDelta / 0.166f);
			if (scrollPos < 0f)
			{
				scrollPos -= scrollPos * 1f * (timeDelta / 0.166f);
				scrollDelta *= Mathf.Clamp01(1f + scrollPos / scrollMax);
			}
			else if (scrollPos > 1f)
			{
				scrollPos -= (scrollPos - 1f) * 1f * (timeDelta / 0.166f);
				scrollDelta *= Mathf.Clamp01(1f - (scrollPos - 1f) / scrollMax);
			}
			if (Mathf.Abs(scrollDelta) < 0.0001f)
			{
				scrollDelta = 0f;
				if (scrollPos > -0.0001f && scrollPos < 0.0001f)
				{
					scrollPos = Mathf.Clamp01(scrollPos);
				}
			}
			BaseScrollListTo(scrollPos + scrollDelta);
			if (scrollPos >= 0f && scrollPos <= 1.001f && scrollDelta == 0f)
			{
				scroll = false;
			}
		}
		else
		{
			scrollInertia = Mathf.Lerp(scrollInertia, scrollDelta, 83f / 150f);
		}
	}

	public List<TUIControl> GetTUIControlONListObjs(bool includeInactive)
	{
		List<TUIControl> list = new List<TUIControl>();
		for (int i = 0; i < this.list.Count; i++)
		{
			TUIControl component = this.list[i].gameObject.GetComponent<TUIControl>();
			if (null != component && (includeInactive || (component.gameObject.active && component.enabled)))
			{
				list.Add(component);
			}
		}
		list.Sort(TUIControl.CompareControl);
		return list;
	}
}
