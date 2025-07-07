using UnityEngine;

public abstract class TUIGesture : MonoBehaviour
{
	protected Bounds area;

	protected float progress;

	protected bool ended = true;

	protected bool penetrate = true;

	public float Progress
	{
		get
		{
			return progress;
		}
	}

	public bool Ended
	{
		get
		{
			return ended;
		}
	}

	public bool Penetrate
	{
		get
		{
			return penetrate;
		}
	}

	public virtual void Reset()
	{
		progress = 0f;
	}

	public abstract void HandleInput(TUIInput input);
}
