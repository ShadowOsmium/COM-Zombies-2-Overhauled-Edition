using UnityEngine;

public class TUIScrollListObject : MonoBehaviour
{
	private Bounds borader;

	public Bounds Borader
	{
		get
		{
			RecalculateBorader();
			return borader;
		}
		set
		{
			borader = value;
		}
	}

	public virtual void RecalculateBorader()
	{
		borader = TUITool.CalculateRelativeControlBounds(base.transform, false);
	}

	private void Awake()
	{
		RecalculateBorader();
	}
}
