using UnityEngine;

public class TUIIgnoreTimeScale : MonoBehaviour
{
	private float mTime;

	private float mDelta;

	public float realTimeDelta
	{
		get
		{
			return mDelta;
		}
	}

	protected float UpdateRealTimeDelta()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mDelta = Mathf.Max(0f, realtimeSinceStartup - mTime);
		mTime = realtimeSinceStartup;
		return mDelta;
	}

	private void OnEnable()
	{
		mTime = Time.realtimeSinceStartup;
	}

	private void Start()
	{
		mTime = Time.realtimeSinceStartup;
	}
}
