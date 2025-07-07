using UnityEngine;

[AddComponentMenu("TUI/Control/TUIScaleThumb")]
public class TUIScaleThumb : TUINeedUpdateBase
{
	public GameObject lThumb;

	public GameObject mThumb;

	public GameObject rThumb;

	public TUISliderEx tNSlider;

	private TUIRect lSize;

	private TUIRect mSize;

	private TUIRect rSize;

	public float factor;

	private float realFactor;

	public float space;

	private float tempSpace;

	private float drift;

	private float sliderWidth;

	private float sliderHeight;

	private float rectx;

	private float recty;

	private TUISliderEx.Direction direction;

	private Vector3 lpos;

	private Vector3 mpos;

	private Vector3 mscale;

	private Vector3 rpos;

	private Vector3 temLpos;

	private Vector3 temMpos;

	private Vector3 temMscale;

	private Vector3 temRpos;

	private Vector2 sliderRect;

	[SerializeField]
	private Vector2 size = Vector2.zero;

	private Rect rectWorld = new Rect(0f, 0f, 0f, 0f);

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			if (value != size)
			{
				size = value;
				UpdateRect();
			}
		}
	}

	public float RealFactor
	{
		get
		{
			return realFactor;
		}
		set
		{
			realFactor = value;
		}
	}

	public float TempSpace
	{
		get
		{
			return tempSpace;
		}
		set
		{
			tempSpace = value;
		}
	}

	public new void Awake()
	{
		base.Awake();
		UpdateRect();
	}

	private void Start()
	{
	}

	public void initThumbData()
	{
		realFactor = Mathf.Clamp01(factor);
		space = Mathf.Max(0f, space);
		tempSpace = space;
		direction = tNSlider.sliderDirection;
		if (null != mThumb)
		{
			mSize = mThumb.GetComponent<TUIRect>();
			temMpos = (mpos = new Vector3(0f, 0f, mThumb.transform.localPosition.z));
			mThumb.transform.localPosition = mpos;
			mscale = mThumb.transform.localScale;
			temMscale = mscale;
		}
		if (null != lThumb)
		{
			lSize = lThumb.GetComponent<TUIRect>();
			if (direction == TUISliderEx.Direction.Horizontal)
			{
				lpos.x = mpos.x - mSize.Size.x * 0.5f - space - lSize.Size.x * 0.5f;
			}
			else
			{
				lpos.y = mpos.y - mSize.Size.y * 0.5f - space - lSize.Size.y * 0.5f;
			}
			lThumb.transform.localPosition = lpos;
			temLpos = lpos;
		}
		if (null != rThumb)
		{
			rSize = rThumb.GetComponent<TUIRect>();
			if (direction == TUISliderEx.Direction.Horizontal)
			{
				rpos.x = mpos.x + mSize.Size.x * 0.5f + space + rSize.Size.x * 0.5f;
			}
			else
			{
				rpos.y = mpos.y + mSize.Size.y * 0.5f + space + rSize.Size.y * 0.5f;
			}
			rThumb.transform.localPosition = rpos;
			temRpos = rpos;
		}
		if (null != tNSlider)
		{
			sliderWidth = tNSlider.size.x;
			sliderHeight = tNSlider.size.y;
			sliderRect = new Vector2(sliderWidth, sliderHeight);
		}
		UpdataThumbShape(realFactor);
	}

	public void UpdataThumbShape(float _factor)
	{
		if (null != lThumb && null != mThumb && null != rThumb)
		{
			AllSliderExists(_factor);
		}
		else if (null != lThumb && null != mThumb && null == rThumb)
		{
			LeftThumbExists(_factor);
		}
		else if (null == lThumb && null != mThumb && null != rThumb)
		{
			RightThumbExists(_factor);
		}
		else if (null != mThumb && null == rThumb && null == lThumb)
		{
			OnlyMidThumbExists(_factor);
		}
	}

	public void Update()
	{
		factor = Mathf.Clamp01(factor);
		if (realFactor != factor)
		{
			realFactor = factor;
			DataFallBack(true);
			UpdataThumbShape(realFactor);
			tNSlider.Set(tNSlider.rawValue, this);
		}
		if (base.NeedUpdate)
		{
			base.NeedUpdate = false;
			UpdateRect();
		}
	}

	private void DataFallBack(bool flag)
	{
		sliderWidth = sliderRect.x;
		sliderHeight = sliderRect.y;
		lpos = temLpos;
		mpos = temMpos;
		rpos = temRpos;
		if (flag)
		{
			mscale = temMscale;
		}
	}

	public void UpdateRect()
	{
		float num = size.x / 2f;
		float num2 = size.y / 2f;
		Vector3[] array = new Vector3[4]
		{
			base.transform.TransformPoint(0f - num, num2, 0f),
			base.transform.TransformPoint(num, num2, 0f),
			base.transform.TransformPoint(num, 0f - num2, 0f),
			base.transform.TransformPoint(0f - num, 0f - num2, 0f)
		};
		rectWorld = default(Rect);
		rectWorld.xMin = Mathf.Min(array[0].x, array[2].x);
		rectWorld.xMax = Mathf.Max(array[0].x, array[2].x);
		rectWorld.yMin = Mathf.Min(array[0].y, array[2].y);
		rectWorld.yMax = Mathf.Max(array[0].y, array[2].y);
	}

	public Rect GetRectLocal(Transform x)
	{
		Vector3 v = new Vector3(rectWorld.xMin, rectWorld.yMin, 0f);
		Vector3 v2 = new Vector3(rectWorld.xMax, rectWorld.yMax, 0f);
		v = x.worldToLocalMatrix.MultiplyPoint3x4(v);
		v2 = x.worldToLocalMatrix.MultiplyPoint3x4(v2);
		return new Rect(v.x, v.y, v2.x - v.x, v2.y - v.y);
	}

	public static Rect RectIntersect(Rect rect1, Rect rect2)
	{
		float num = Mathf.Max(rect1.xMin, rect2.xMin);
		float num2 = Mathf.Min(rect1.xMax, rect2.xMax);
		float num3 = Mathf.Max(rect1.yMin, rect2.yMin);
		float num4 = Mathf.Min(rect1.yMax, rect2.yMax);
		float num5 = num2 - num;
		float num6 = num4 - num3;
		return new Rect(num, num3, (!(num5 < 0f)) ? num5 : 0f, (!(num6 < 0f)) ? num6 : 0f);
	}

	public void OnDrawGizmos()
	{
		float num = size.x / 2f;
		float num2 = size.y / 2f;
		Vector3[] array = new Vector3[4]
		{
			base.transform.TransformPoint(0f - num, num2, 0f),
			base.transform.TransformPoint(num, num2, 0f),
			base.transform.TransformPoint(num, 0f - num2, 0f),
			base.transform.TransformPoint(0f - num, 0f - num2, 0f)
		};
		Gizmos.color = Color.white;
		Gizmos.DrawLine(array[0], array[1]);
		Gizmos.DrawLine(array[1], array[2]);
		Gizmos.DrawLine(array[2], array[3]);
		Gizmos.DrawLine(array[3], array[0]);
		Gizmos.DrawLine(array[0], array[2]);
	}

	public void OnDrawGizmosSelected()
	{
		float num = size.x / 2f;
		float num2 = size.y / 2f;
		Vector3[] array = new Vector3[4]
		{
			base.transform.TransformPoint(0f - num, num2, 0f),
			base.transform.TransformPoint(num, num2, 0f),
			base.transform.TransformPoint(num, 0f - num2, 0f),
			base.transform.TransformPoint(0f - num, 0f - num2, 0f)
		};
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(array[0], array[1]);
		Gizmos.DrawLine(array[1], array[2]);
		Gizmos.DrawLine(array[2], array[3]);
		Gizmos.DrawLine(array[3], array[0]);
		Gizmos.DrawLine(array[0], array[2]);
	}

	private void LeftThumbExists(float _factor)
	{
		if (direction == TUISliderEx.Direction.Horizontal)
		{
			float x = lSize.Size.x;
			float x2 = mSize.Size.x;
			float num = _factor * (sliderWidth - x - space) / x2;
			mscale.x = num;
			if (num < 1f)
			{
				lpos.x += x2 * (1f - num) / 2f;
			}
			else
			{
				lpos.x -= x2 * (num - 1f) / 2f;
			}
			mThumb.transform.localScale = mscale;
			rectx = space + x + x2 * num;
			recty = Mathf.Max(lSize.Size.y, mSize.Size.y);
			drift = (rectx - x2 * num) * 0.5f;
			lpos.x += drift;
			mpos.x += drift;
			lThumb.transform.localPosition = lpos;
			mThumb.transform.localPosition = mpos;
		}
		else
		{
			float y = lSize.Size.y;
			float y2 = mSize.Size.y;
			float num2 = _factor * (sliderHeight - y - space) / y2;
			mscale.y = num2;
			if (num2 < 1f)
			{
				lpos.y += y2 * (1f - num2) / 2f;
			}
			else
			{
				lpos.y -= y2 * (num2 - 1f) / 2f;
			}
			mThumb.transform.localScale = mscale;
			rectx = Mathf.Max(lSize.Size.x, mSize.Size.x);
			recty = space + y + y2 * num2;
			drift = (recty - y2 * num2) * 0.5f;
			lpos.y += drift;
			mpos.y += drift;
			lThumb.transform.localPosition = lpos;
			mThumb.transform.localPosition = mpos;
		}
		base.transform.GetComponent<TUIScaleThumb>().Size = new Vector2(rectx, recty);
	}

	private void RightThumbExists(float _factor)
	{
		if (direction == TUISliderEx.Direction.Horizontal)
		{
			float num = 0f;
			float num2 = 1f;
			num = rSize.Size.x;
			num2 = mSize.Size.x;
			float num3 = _factor * (sliderWidth - num - space) / num2;
			mscale.x = num3;
			if (num3 < 1f)
			{
				rpos.x -= num2 * (1f - num3) / 2f;
			}
			else
			{
				rpos.x += num2 * (num3 - 1f) / 2f;
			}
			mThumb.transform.localScale = mscale;
			recty = Mathf.Max(mSize.Size.y, rSize.Size.y);
			rectx = num + space + num2 * num3;
			drift = (rectx - num2 * num3) * 0.5f;
			rpos.x -= drift;
			mpos.x -= drift;
			rThumb.transform.localPosition = rpos;
			mThumb.transform.localPosition = mpos;
		}
		else
		{
			float y = rSize.Size.y;
			float y2 = mSize.Size.y;
			float num4 = _factor * (sliderHeight - y - space) / y2;
			mscale.y = num4;
			if (num4 < 1f)
			{
				rpos.y -= y2 * (1f - num4) / 2f;
			}
			else
			{
				rpos.y += y2 * (num4 - 1f) / 2f;
			}
			mThumb.transform.localScale = mscale;
			rectx = Mathf.Max(mSize.Size.x, rSize.Size.x);
			recty = space + y + y2 * num4;
			drift = (recty - y2 * num4) * 0.5f;
			rpos.y -= drift;
			mpos.y -= drift;
			rThumb.transform.localPosition = rpos;
			mThumb.transform.localPosition = mpos;
		}
		Size = new Vector2(rectx, recty);
	}

	private void OnlyMidThumbExists(float _factor)
	{
		if (direction == TUISliderEx.Direction.Horizontal)
		{
			float x = mSize.Size.x;
			float num = _factor * sliderWidth / x;
			mscale.x = num;
			mThumb.transform.localScale = mscale;
			rectx = x * num;
			recty = mSize.Size.y;
		}
		else
		{
			float y = mSize.Size.y;
			rectx = mSize.Size.x;
			float num2 = _factor * sliderHeight / y;
			mscale.y = num2;
			mThumb.transform.localScale = mscale;
			recty = y * num2;
		}
		base.transform.GetComponent<TUIScaleThumb>().Size = new Vector2(rectx, recty);
	}

	private void AllSliderExists(float _factor)
	{
		if (direction == TUISliderEx.Direction.Horizontal)
		{
			float num = 0f;
			float num2 = 1f;
			float num3 = 0f;
			num = lSize.Size.x;
			num2 = mSize.Size.x;
			num3 = rSize.Size.x;
			float num4 = _factor * (sliderWidth - num - num3 - space * 2f) / num2;
			mscale.x = num4;
			drift = (num3 - num) * 0.5f;
			if (num4 < 1f)
			{
				lpos.x = lpos.x + tempSpace - space + num2 * (1f - num4) / 2f;
				rpos.x = rpos.x - tempSpace + space - num2 * (1f - num4) / 2f;
			}
			else
			{
				lpos.x = lpos.x + tempSpace - space - num2 * (num4 - 1f) / 2f;
				rpos.x = rpos.x - tempSpace + space + num2 * (num4 - 1f) / 2f;
			}
			mpos.x -= drift;
			lpos.x -= drift;
			rpos.x -= drift;
			mThumb.transform.localPosition = mpos;
			mThumb.transform.localScale = mscale;
			lThumb.transform.localPosition = lpos;
			rThumb.transform.localPosition = rpos;
			rectx = space * 2f + num3 + num + num2 * num4;
			recty = Mathf.Max(lSize.Size.y, mSize.Size.y, rSize.Size.y);
		}
		else
		{
			float num5 = 0f;
			float num6 = 1f;
			float num7 = 0f;
			num5 = lSize.Size.y;
			num6 = mSize.Size.y;
			num7 = rSize.Size.y;
			float num8 = _factor * (sliderHeight - num5 - num7 - space * 2f) / num6;
			mscale.y = num8;
			drift = (num7 - num5) * 0.5f;
			if (num8 < 1f)
			{
				lpos.y = lpos.y + tempSpace - space + num6 * (1f - num8) / 2f;
				rpos.y = rpos.y + tempSpace - space - num6 * (1f - num8) / 2f;
			}
			else
			{
				lpos.y = lpos.y + tempSpace - space - num6 * (num8 - 1f) / 2f;
				rpos.y = rpos.y + tempSpace - space + num6 * (num8 - 1f) / 2f;
			}
			mpos.y -= drift;
			rpos.y -= drift;
			lpos.y -= drift;
			mThumb.transform.localScale = mscale;
			mThumb.transform.localPosition = mpos;
			lThumb.transform.localPosition = lpos;
			rThumb.transform.localPosition = rpos;
			rectx = Mathf.Max(lSize.Size.x, mSize.Size.x, rSize.Size.x);
			recty = space * 2f + num5 + num7 + num6 * num8;
		}
		Size = new Vector2(rectx, recty);
	}
}
