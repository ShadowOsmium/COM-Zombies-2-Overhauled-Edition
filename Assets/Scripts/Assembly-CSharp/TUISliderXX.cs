using UnityEngine;

[AddComponentMenu("TUI/Control/Slider")]
public class TUISliderXX : TUIControlImpl
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	[SerializeField]
	protected bool updateForever;

	public static int OnSliderChange = 1;

	public Transform foreground;

	public Transform thumb;

	[SerializeField]
	protected Direction direction;

	[SerializeField]
	protected float rawValue = 1f;

	private float mStepValue = -1f;

	private TUIControl thumbControl;

	private TUIMeshSprite mSprite;

	private TUIRect showClip;

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
			Set(value);
		}
	}

	public bool UpdateForever
	{
		get
		{
			return updateForever;
		}
		set
		{
			updateForever = value;
			if (null != showClip)
			{
				showClip.updateForever = updateForever;
			}
		}
	}

	private void Awake()
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
		showClip = gameObject.AddComponent<TUIRect>();
		showClip.Size = size * 2f;
		showClip.updateForever = updateForever;
		if (foreground != null)
		{
			mSprite = foreground.GetComponent<TUIMeshSprite>();
			if (null != mSprite)
			{
				mSprite.otherClips.Add(showClip);
				mSprite.ForceUpdate();
			}
		}
		if (null != thumb)
		{
			thumbControl = thumb.GetComponent<TUIControl>();
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
			if (input.inputType == TUIInputType.Moved || input.inputType == TUIInputType.Stationary)
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

	private void Start()
	{
		Set(rawValue);
	}

	private void UpdateDrag(Vector3 lastTouchPosition)
	{
		Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint3x4(lastTouchPosition);
		Set((direction != 0) ? (vector.y / size.y + 0.5f) : (vector.x / size.x + 0.5f), false, true);
	}

	public void Set(float input, bool forceUpdate = true, bool postEvent = false)
	{
		float num = (rawValue = Mathf.Clamp01(input));
		if (!forceUpdate && mStepValue == num)
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
