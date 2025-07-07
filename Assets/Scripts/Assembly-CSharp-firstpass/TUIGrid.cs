using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("TUI/Interaction/Grid")]
public class TUIGrid : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal,
		Vertical
	}

	public Arrangement arrangement;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool repositionNow;

	public bool sorted;

	public bool hideInactive = true;

	private void Start()
	{
		Reposition();
	}

	private void Update()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public void Reposition()
	{
		Transform transform = base.transform;
		int num = 0;
		int num2 = 0;
		if (sorted)
		{
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < transform.childCount; i++)
			{
				list.Add(transform.GetChild(i));
			}
			list.Sort(SortByName);
			{
				foreach (Transform item in list)
				{
					if (item.gameObject.active || !hideInactive)
					{
						item.localPosition = ((arrangement != 0) ? new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, 0f) : new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, 0f));
						if (++num >= maxPerLine && maxPerLine > 0)
						{
							num = 0;
							num2++;
						}
					}
				}
				return;
			}
		}
		for (int j = 0; j < transform.childCount; j++)
		{
			Transform child = transform.GetChild(j);
			if (child.gameObject.active || !hideInactive)
			{
				child.localPosition = ((arrangement != 0) ? new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, 0f) : new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, 0f));
				if (++num >= maxPerLine && maxPerLine > 0)
				{
					num = 0;
					num2++;
				}
			}
		}
	}
}
