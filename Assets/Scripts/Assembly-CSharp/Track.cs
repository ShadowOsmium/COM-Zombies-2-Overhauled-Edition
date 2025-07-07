using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
	public enum TrackType
	{
		Linear,
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc,
		EaseInBounce,
		EaseOutBounce,
		EaseInOutBounce,
		EaseInBack,
		EaseOutBack,
		EaseInOutBack,
		EaseInElastic,
		EaseOutElastic,
		EaseInOutElastic
	}

	public enum TrackPathType
	{
		Normal,
		Reverse
	}

	private Path m_path;

	private float m_trackTime;

	private TrackType m_type;

	private TrackPathType m_trackpathtype;

	private float percent;

	private float trackvalue;

	private bool istrack;

	protected bool issmooth;

	public Vector3 smoothposition = Vector3.zero;

	public Path GetPath()
	{
		return m_path;
	}

	public TrackPathType GetTrackPathType()
	{
		return m_trackpathtype;
	}

	public float GetPathPercent()
	{
		return trackvalue;
	}

	public bool GetSmooth()
	{
		return issmooth;
	}

	public static Track MoveTrack(Transform trans, Path path, TrackType type, float time, float delaytime = 0f, bool smooth = false, TrackPathType trackpathtype = TrackPathType.Normal)
	{
		Track track = trans.gameObject.AddComponent<Track>();
		track.Init(path, type, time, trackpathtype);
		track.issmooth = smooth;
		if (track.issmooth)
		{
			track.smoothposition = trans.position;
		}
		if (delaytime >= 0f)
		{
			track.Invoke("BeginTrack", delaytime);
		}
		else
		{
			track.BeginTrack(-1f * delaytime / time);
		}
		return track;
	}

	public static void CancelTrack(Transform trans)
	{
		Track component = trans.gameObject.GetComponent<Track>();
		if (component != null)
		{
			Object.Destroy(component);
		}
	}

	protected void PutOnPath(Transform target, Path path, float percent, TrackPathType trackpathtype)
	{
		List<Vector3> list = path.LocalModify(path.nodes);
		if (trackpathtype == TrackPathType.Reverse)
		{
			list.Reverse();
		}
		if (issmooth)
		{
			list.Insert(0, smoothposition);
		}
		target.position = InterpAlgorithm.Interp(path.type, PathMaker.PathControlPointGenerator(list.ToArray()), percent);
	}

	protected void Init(Path path, TrackType type, float tracktime, TrackPathType trackpathtype)
	{
		m_path = path;
		m_type = type;
		m_trackTime = tracktime;
		m_trackpathtype = trackpathtype;
	}

	protected void BeginTrack()
	{
		BeginTrack(0f);
	}

	protected void BeginTrack(float beginpercent)
	{
		percent = beginpercent;
		trackvalue = 0f;
		istrack = true;
	}

	private void OnTrack()
	{
		if (istrack)
		{
			percent = Mathf.Clamp01(percent + Time.deltaTime / m_trackTime);
			switch (m_type)
			{
			case TrackType.Linear:
				trackvalue = TrackAlgorithm.linear(0f, 1f, percent);
				break;
			case TrackType.EaseInQuad:
				trackvalue = TrackAlgorithm.easeInQuad(0f, 1f, percent);
				break;
			case TrackType.EaseOutQuad:
				trackvalue = TrackAlgorithm.easeOutQuad(0f, 1f, percent);
				break;
			case TrackType.EaseInOutQuad:
				trackvalue = TrackAlgorithm.easeInOutQuad(0f, 1f, percent);
				break;
			case TrackType.EaseInCubic:
				trackvalue = TrackAlgorithm.easeInCubic(0f, 1f, percent);
				break;
			case TrackType.EaseOutCubic:
				trackvalue = TrackAlgorithm.easeOutCubic(0f, 1f, percent);
				break;
			case TrackType.EaseInOutCubic:
				trackvalue = TrackAlgorithm.easeInOutCubic(0f, 1f, percent);
				break;
			case TrackType.EaseInQuart:
				trackvalue = TrackAlgorithm.easeInQuart(0f, 1f, percent);
				break;
			case TrackType.EaseOutQuart:
				trackvalue = TrackAlgorithm.easeOutQuart(0f, 1f, percent);
				break;
			case TrackType.EaseInOutQuart:
				trackvalue = TrackAlgorithm.easeInOutQuart(0f, 1f, percent);
				break;
			case TrackType.EaseInQuint:
				trackvalue = TrackAlgorithm.easeInQuint(0f, 1f, percent);
				break;
			case TrackType.EaseOutQuint:
				trackvalue = TrackAlgorithm.easeOutQuint(0f, 1f, percent);
				break;
			case TrackType.EaseInOutQuint:
				trackvalue = TrackAlgorithm.easeInOutQuint(0f, 1f, percent);
				break;
			case TrackType.EaseInSine:
				trackvalue = TrackAlgorithm.easeInSine(0f, 1f, percent);
				break;
			case TrackType.EaseOutSine:
				trackvalue = TrackAlgorithm.easeOutSine(0f, 1f, percent);
				break;
			case TrackType.EaseInOutSine:
				trackvalue = TrackAlgorithm.easeInOutSine(0f, 1f, percent);
				break;
			case TrackType.EaseInExpo:
				trackvalue = TrackAlgorithm.easeInExpo(0f, 1f, percent);
				break;
			case TrackType.EaseOutExpo:
				trackvalue = TrackAlgorithm.easeOutExpo(0f, 1f, percent);
				break;
			case TrackType.EaseInOutExpo:
				trackvalue = TrackAlgorithm.easeInOutExpo(0f, 1f, percent);
				break;
			case TrackType.EaseInCirc:
				trackvalue = TrackAlgorithm.easeInCirc(0f, 1f, percent);
				break;
			case TrackType.EaseOutCirc:
				trackvalue = TrackAlgorithm.easeOutCirc(0f, 1f, percent);
				break;
			case TrackType.EaseInOutCirc:
				trackvalue = TrackAlgorithm.easeInOutCirc(0f, 1f, percent);
				break;
			case TrackType.EaseInBounce:
				trackvalue = TrackAlgorithm.easeInBounce(0f, 1f, percent);
				break;
			case TrackType.EaseOutBounce:
				trackvalue = TrackAlgorithm.easeOutBounce(0f, 1f, percent);
				break;
			case TrackType.EaseInOutBounce:
				trackvalue = TrackAlgorithm.easeInOutBounce(0f, 1f, percent);
				break;
			case TrackType.EaseInBack:
				trackvalue = TrackAlgorithm.easeInBack(0f, 1f, percent);
				break;
			case TrackType.EaseOutBack:
				trackvalue = TrackAlgorithm.easeOutBack(0f, 1f, percent);
				break;
			case TrackType.EaseInOutBack:
				trackvalue = TrackAlgorithm.easeInOutBack(0f, 1f, percent);
				break;
			case TrackType.EaseInElastic:
				trackvalue = TrackAlgorithm.easeInElastic(0f, 1f, percent);
				break;
			case TrackType.EaseOutElastic:
				trackvalue = TrackAlgorithm.easeOutElastic(0f, 1f, percent);
				break;
			case TrackType.EaseInOutElastic:
				trackvalue = TrackAlgorithm.easeInOutElastic(0f, 1f, percent);
				break;
			}
		}
	}

	private void OnPutOnPath()
	{
		if (istrack)
		{
			PutOnPath(base.transform, m_path, trackvalue, m_trackpathtype);
		}
	}

	private void OnLater()
	{
		if (percent >= 1f)
		{
			istrack = false;
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		OnTrack();
		OnPutOnPath();
		OnLater();
	}
}
