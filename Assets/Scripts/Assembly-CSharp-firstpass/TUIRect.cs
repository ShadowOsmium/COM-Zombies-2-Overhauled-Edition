using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("TUI/Interaction/Rect")]
public class TUIRect : TUINeedUpdateBase
{
	[SerializeField]
	protected Vector2 size = Vector2.zero;

	public Camera currentCamera;

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

	public new void Awake()
	{
		base.Awake();
		UpdateRect();
	}

	public void Update()
	{
		if (base.NeedUpdate)
		{
			base.NeedUpdate = false;
			UpdateRect();
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

	public Vector4 GetRectViewPort()
	{
		if (null == currentCamera)
		{
			TUI component = base.transform.root.GetComponent<TUI>();
			if (null != component)
			{
				currentCamera = component.Camera.GetComponent<Camera>();
			}
		}
		Vector3 vector = currentCamera.WorldToViewportPoint(new Vector3(rectWorld.xMin, rectWorld.yMax));
		UpdateRect();
		Vector3 vector2 = currentCamera.WorldToViewportPoint(new Vector3(rectWorld.xMax, rectWorld.yMin));
		return new Vector4(vector.x, vector2.y, vector2.x, vector.y) * 2f - Vector4.one;
	}
}
