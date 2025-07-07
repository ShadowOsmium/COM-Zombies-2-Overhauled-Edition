using TNetSdk;
using UnityEngine;

public class NetworkTransformSender : MonoBehaviour
{
	public static readonly float sendingPeriod = 0.2f;

	private readonly float accuracy = 0.002f;

	private float timeLastSending;

	private bool send;

	private NetworkTransform lastState;

	private Transform thisTransform;

	private bool m_bPlayerDataChanged;

	private TNetObject tnetObj;

	public void SetPlayerDataChanged(bool bChanged)
	{
		m_bPlayerDataChanged = bChanged;
	}

	private void Awake()
	{
		tnetObj = TNetConnection.Connection;
	}

	private void Start()
	{
		thisTransform = base.transform;
		lastState = NetworkTransform.FromTransform(thisTransform);
	}

	public void StartSendTransform()
	{
		send = true;
	}

	public void StopSendTransform()
	{
		send = false;
	}

	private void FixedUpdate()
	{
		if (tnetObj != null && send)
		{
			SendTransform();
		}
	}

	private void SendTransform()
	{
		timeLastSending += Time.deltaTime;
		if (!(timeLastSending >= sendingPeriod))
		{
			return;
		}
		if (!m_bPlayerDataChanged)
		{
			if (NetworkTransform.FromTransform(thisTransform).Position == lastState.Position && NetworkTransform.FromTransform(thisTransform).Rotation == lastState.Rotation)
			{
				return;
			}
		}
		else
		{
			SetPlayerDataChanged(false);
		}
		lastState = NetworkTransform.FromTransform(thisTransform);
		lastState.TimeStamp = tnetObj.TimeManager.NetworkTime;
		SFSArray val = lastState.ToSFSArray();
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutSFSArray("trans", val);
		tnetObj.Send(new BroadcastMessageRequest(sFSObject));
		timeLastSending = 0f;
	}
}
