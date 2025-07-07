using UnityEngine;

public class TrackEvent : MonoBehaviour
{
	private Track m_track;

	private Path m_path;

	private PathEvent[] m_events;

	private Track.TrackPathType m_trackpathtype;

	private bool issmooth;

	protected float lastPercent = -1.401298E-45f;

	public static void OnTrackEvent(GameObject go, Track track)
	{
		TrackEvent trackEvent = go.AddComponent<TrackEvent>();
		trackEvent.Init(track);
	}

	protected void Init(Track track)
	{
		m_track = track;
		m_path = track.GetPath();
		m_events = m_path.gameObject.GetComponents<PathEvent>();
		m_trackpathtype = track.GetTrackPathType();
		issmooth = track.GetSmooth();
	}

	private void Update()
	{
		if (m_track != null)
		{
			float pathPercent = m_track.GetPathPercent();
			PostEvent(pathPercent);
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void PostEvent(float percent)
	{
		PathEvent[] events = m_events;
		foreach (PathEvent pathEvent in events)
		{
			float percent2 = GetPercent(pathEvent.nodeId);
			if (percent2 > lastPercent && percent2 <= percent)
			{
				pathEvent.Send();
			}
		}
		lastPercent = percent;
	}

	private float GetPercent(int nodeid)
	{
		int num = nodeid;
		if (m_trackpathtype == Track.TrackPathType.Reverse)
		{
			num = m_path.nodeCount - nodeid - 1;
		}
		if (issmooth)
		{
			return 1f / (float)m_path.nodeCount * (float)(num + 1);
		}
		return 1f / (float)(m_path.nodeCount - 1) * (float)num;
	}
}
