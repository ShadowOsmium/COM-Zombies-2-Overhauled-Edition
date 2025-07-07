using UnityEngine;

public class HorizontalPageGesture : TUIPageGestureEx
{
	public float xDistance = 1f;

	public float forwardDistance = 120f;

	public float backwardDistance = 120f;

	public Vector2 size;

	[SerializeField]
	private int fingerId = -1;

	private Vector3 beganPos;

	private float x_dist;

	public override float CurrentProgress
	{
		get
		{
			return (0f - x_dist) / xDistance;
		}
	}

	public override bool IsGesturing
	{
		get
		{
			return fingerId != -1;
		}
	}

	public override float ForwardProgress
	{
		get
		{
			return forwardDistance / xDistance;
		}
	}

	public override float BackwardProgress
	{
		get
		{
			return backwardDistance / xDistance;
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		switch (input.inputType)
		{
		case TUIInputType.Began:
			if (fingerId == -1 && PointtIn(input.position))
			{
				beganPos = input.position;
				fingerId = input.fingerId;
				x_dist = 0f;
				return false;
			}
			return false;
		case TUIInputType.Moved:
			if (fingerId == input.fingerId)
			{
				x_dist = input.position.x - beganPos.x;
				return true;
			}
			return false;
		case TUIInputType.Ended:
			if (fingerId == input.fingerId)
			{
				x_dist = input.position.x - beganPos.x;
				fingerId = -1;
				return false;
			}
			return false;
		default:
			return false;
		}
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
		Gizmos.color = Color.red;
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

	public virtual bool PointtIn(Vector2 point)
	{
		float num = size.x / 2f;
		float num2 = size.y / 2f;
		Vector3[] array = new Vector3[4];
		Rect rect = default(Rect);
		rect.xMin = 0f - num;
		rect.xMax = num;
		rect.yMin = 0f - num2;
		rect.yMax = num2;
		array[0] = base.transform.TransformPoint(rect.xMin, rect.yMax, 0f);
		array[1] = base.transform.TransformPoint(rect.xMax, rect.yMax, 0f);
		array[2] = base.transform.TransformPoint(rect.xMax, rect.yMin, 0f);
		array[3] = base.transform.TransformPoint(rect.xMin, rect.yMin, 0f);
		return PointInPolygon(array, point);
	}

	protected bool PointInPolygon(Vector3[] v, Vector2 point)
	{
		bool flag = false;
		int num = v.Length;
		for (int i = 0; i < num; i++)
		{
			if ((!(point.y < v[i].y) || !(point.y < v[(i + 1) % num].y)) && (!(v[i].x <= point.x) || !(v[(i + 1) % num].x <= point.x)))
			{
				float num2 = v[(i + 1) % num].x - v[i].x;
				float num3 = v[(i + 1) % num].y - v[i].y;
				float num4 = (point.x - v[i].x) / num2;
				float num5 = num4 * num3 + v[i].y;
				if (num5 <= point.y && num4 >= 0f && num4 <= 1f)
				{
					flag = !flag;
				}
			}
		}
		return flag;
	}
}
