using UnityEngine;

[AddComponentMenu("TUI/Control/TUISliderEx")]
public class TUISliderEx : TUIControlImpl
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	public static int OnSliderChange = 1;

	public Transform foreground;

	public Transform thumb;

	[SerializeField]
	protected Direction direction;

	[SerializeField]
	public float rawValue = 1f;

	private float _rawValue = 1f;

	private float mStepValue = -1f;

	private TUIControl thumbControl;

	private TUIMeshSprite mSprite;

	private TUIRect showClip;

	private TUIScaleThumb thumbRect;

	private Vector3 lastPostion;

	private int fingerId = -1;

	public float sliderValue
	{
		get
		{
			return mStepValue;
		}
		set
		{
			if (null != thumbRect)
			{
				Set(value, thumbRect);
			}
			else
			{
				Set(value);
			}
		}
	}

	public Direction sliderDirection
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;
		}
	}

	private void Start()
	{
		GameObject gameObject = new GameObject("ShowClip");
		gameObject.transform.parent = base.transform;
		if (direction == Direction.Horizontal)
		{
			gameObject.transform.localPosition = new Vector3(-0.5f * size.x, 0f, 0f);
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(0f, -0.5f * size.y, 0f);
		}
		sliderDirection = direction;
		showClip = gameObject.AddComponent<TUIRect>();
		showClip.Size = size * 2f;
		if (foreground != null)
		{
			mSprite = foreground.GetComponent<TUIMeshSprite>();
			if (null != mSprite)
			{
				mSprite.m_showClipObj = showClip.gameObject;
				mSprite.NeedUpdate = true;
				mSprite.ForceUpdate();
			}
		}
		if (null != thumb)
		{
			thumbControl = thumb.GetComponent<TUIControl>();
			thumbRect = thumb.GetComponent<TUIScaleThumb>();
			_rawValue = rawValue;
			if (null != thumbRect)
			{
				thumbRect.initThumbData();
				Set(rawValue, thumbRect);
			}
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				UpdateDrag(input.position);
				fingerId = input.fingerId;
				if (null != thumbControl)
				{
					thumbControl.HandleInput(input);
				}
			}
			return false;
		}
		if (input.fingerId == fingerId)
		{
			if (input.inputType == TUIInputType.Moved)
			{
				UpdateDrag(input.position);
				if (null != thumbControl)
				{
					thumbControl.HandleInput(input);
				}
				return true;
			}
			if (null != thumbControl)
			{
				thumbControl.HandleInput(input);
			}
			fingerId = -1;
			return false;
		}
		return false;
	}

	private void Update()
	{
		if (null != thumbRect && _rawValue != rawValue)
		{
			_rawValue = rawValue;
			Set(rawValue, thumbRect, true);
		}
	}

	private void UpdateDrag(Vector3 lastTouchPosition)
	{
		Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint3x4(lastTouchPosition);
		if (thumbRect != null)
		{
			Set((direction != 0) ? ((vector.y - (base.transform.localPosition.y - size.y * 0.5f + thumbRect.Size.y * 0.5f)) / (size.y - thumbRect.Size.y)) : ((vector.x - (base.transform.localPosition.x - size.x * 0.5f + thumbRect.Size.x * 0.5f)) / (size.x - thumbRect.Size.x)), thumbRect, true);
		}
		else
		{
			Set((direction != 0) ? (vector.y / size.y + 0.5f) : (vector.x / size.x + 0.5f), true);
		}
	}

	public void Set(float input, TUIScaleThumb rect, bool postEvent = false)
	{
		float num = (rawValue = Mathf.Clamp01(input));
		Vector2 vector = showClip.Size;
		float num2 = vector.x;
		float num3 = vector.y;
		float num4 = size.x - rect.Size.x;
		float num5 = size.y - rect.Size.y;
		if (mStepValue != num)
		{
			mStepValue = num;
		}
		Vector3 one = Vector3.one;
		if (direction == Direction.Horizontal)
		{
			one.x *= mStepValue;
			num2 = (rect.Size.x * 0.5f + rawValue * num4) * 2f;
			float x = rect.Size.x;
			float num6 = (size.x - rect.Size.x * 0.5f) * 2f;
			if (num2 <= x)
			{
				num2 = x;
			}
			else if (num2 > num6)
			{
				num2 = num6;
			}
		}
		else
		{
			one.y *= mStepValue;
			num3 = (rect.Size.y * 0.5f + rawValue * num5) * 2f;
			float y = rect.Size.y;
			float num7 = (size.y - rect.Size.y * 0.5f) * 2f;
			if (num3 < y)
			{
				num3 = y;
			}
			else if (num3 > num7)
			{
				num3 = num7;
			}
		}
		vector.x = num2;
		vector.y = num3;
		showClip.Size = vector;
		if (null != mSprite)
		{
			mSprite.NeedUpdate = true;
			mSprite.ForceUpdate();
		}
		if (thumb != null)
		{
			Vector3 localPosition = thumb.localPosition;
			if (direction == Direction.Horizontal)
			{
				localPosition.x = showClip.transform.localPosition.x + showClip.Size.x * 0.5f;
			}
			else
			{
				localPosition.y = showClip.transform.localPosition.y + showClip.Size.y * 0.5f;
			}
			thumb.localPosition = localPosition;
		}
		if (postEvent)
		{
			PostEvent(this, OnSliderChange, rawValue, 0f, null);
		}
	}

	private void Set(float input, bool postEvent = false)
	{
		float num = (rawValue = Mathf.Clamp01(input));
		if (mStepValue == num)
		{
			return;
		}
		mStepValue = num;
		Vector3 one = Vector3.one;
		if (direction == Direction.Horizontal)
		{
			one.x *= mStepValue;
		}
		else
		{
			one.y *= mStepValue;
		}
		Vector2 vector = showClip.Size;
		vector.x = one.x * size.x * 2f;
		vector.y = one.y * size.y * 2f;
		showClip.Size = vector;
		if (null != mSprite)
		{
			mSprite.NeedUpdate = true;
			mSprite.ForceUpdate();
		}
		if (thumb != null)
		{
			Vector3 localPosition = thumb.localPosition;
			if (direction == Direction.Horizontal)
			{
				localPosition.x = showClip.transform.localPosition.x + showClip.Size.x * 0.5f;
			}
			else
			{
				localPosition.y = showClip.transform.localPosition.y + showClip.Size.y * 0.5f;
			}
			thumb.localPosition = localPosition;
		}
		if (postEvent)
		{
			PostEvent(this, OnSliderChange, rawValue, 0f, null);
		}
	}
}
