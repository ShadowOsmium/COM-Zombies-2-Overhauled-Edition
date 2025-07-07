using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

public class NetworkTransformInterpolation : MonoBehaviour
{
	public enum InterpolationMode
	{
		INTERPOLATION,
		EXTRAPOLATION
	}

	public InterpolationMode mode;

	private double interpolationBackTime = 200.0;

	private float extrapolationForwardTime = 1000f;

	private bool running;

	private TNetObject tnetObj;

	private NetworkTransform[] bufferedStates = new NetworkTransform[20];

	private List<NetworkTransform> m_stateList = new List<NetworkTransform>();

	private int statesCount;

	private Vector3 move_dir = Vector3.zero;

	private float m_fMoveCounter;

	public Vector3 MoveDir
	{
		get
		{
			return move_dir;
		}
	}

	private void Awake()
	{
		tnetObj = TNetConnection.Connection;
	}

	public void StartReceiving()
	{
		running = true;
	}

	public void StopReceiving()
	{
		running = false;
	}

	public void ReceivedTransform(NetworkTransform ntransform)
	{
		if (!running)
		{
			return;
		}
		Vector3 position = ntransform.Position;
		Quaternion rotation = ntransform.Rotation;
		for (int num = bufferedStates.Length - 1; num >= 1; num--)
		{
			bufferedStates[num] = bufferedStates[num - 1];
		}
		bufferedStates[0] = ntransform;
		statesCount = Mathf.Min(statesCount + 1, bufferedStates.Length);
		for (int i = 0; i < statesCount - 1; i++)
		{
			if (bufferedStates[i].TimeStamp < bufferedStates[i + 1].TimeStamp)
			{
			}
		}
		m_stateList.Add(ntransform);
	}

	public void MoveToWaypoint()
	{
		if (m_stateList.Count > 0)
		{
			m_stateList.RemoveAt(0);
		}
	}

	public void ClearWaypoints()
	{
		m_stateList.Clear();
	}

	private void UpdateState_New(float delta_time)
	{
		if (m_stateList.Count <= 0)
		{
			return;
		}
		if (m_stateList.Count == 1)
		{
			move_dir = Vector3.zero;
		}
		else if (m_stateList.Count >= 2)
		{
			m_fMoveCounter += delta_time * (1f / NetworkTransformSender.sendingPeriod);
			base.transform.position = Vector3.Lerp(m_stateList[0].Position, m_stateList[1].Position, m_fMoveCounter);
			base.transform.rotation = Quaternion.Lerp(m_stateList[0].Rotation, m_stateList[1].Rotation, m_fMoveCounter);
			move_dir = (m_stateList[1].Position - m_stateList[0].Position).normalized;
			if (m_fMoveCounter >= 1f)
			{
				m_fMoveCounter = 0f;
				m_stateList.RemoveAt(0);
			}
		}
		else
		{
			move_dir = Vector3.zero;
		}
	}

	private void Update()
	{
		if (running && statesCount != 0 && tnetObj != null)
		{
			UpdateState_New(Time.deltaTime);
		}
	}

	private void UpdateValues()
	{
		if (tnetObj != null)
		{
			double averagePing = tnetObj.TimeManager.AveragePing;
			if (averagePing < 50.0)
			{
				interpolationBackTime = 50.0;
			}
			else if (averagePing < 100.0)
			{
				interpolationBackTime = 100.0;
			}
			else if (averagePing < 200.0)
			{
				interpolationBackTime = 200.0;
			}
			else if (averagePing < 400.0)
			{
				interpolationBackTime = 400.0;
			}
			else if (averagePing < 600.0)
			{
				interpolationBackTime = 600.0;
			}
			else if (averagePing < 800.0)
			{
				interpolationBackTime = 800.0;
			}
			else if (averagePing < 1000.0)
			{
				interpolationBackTime = 1000.0;
			}
			else if (averagePing < 1200.0)
			{
				interpolationBackTime = 1200.0;
			}
			else if (averagePing < 1400.0)
			{
				interpolationBackTime = 1400.0;
			}
			else if (averagePing < 1600.0)
			{
				interpolationBackTime = 1600.0;
			}
			else if (averagePing < 1800.0)
			{
				interpolationBackTime = 1800.0;
			}
			else
			{
				interpolationBackTime = 2000.0;
			}
			interpolationBackTime += 300.0;
		}
	}
}
