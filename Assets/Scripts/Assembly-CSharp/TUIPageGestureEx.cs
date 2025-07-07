using UnityEngine;

public abstract class TUIPageGestureEx : MonoBehaviour
{
	public abstract float CurrentProgress { get; }

	public abstract bool IsGesturing { get; }

	public abstract float ForwardProgress { get; }

	public abstract float BackwardProgress { get; }

	public abstract bool HandleInput(TUIInput input);

	public virtual void HandlePageFrameLock()
	{
	}
}
