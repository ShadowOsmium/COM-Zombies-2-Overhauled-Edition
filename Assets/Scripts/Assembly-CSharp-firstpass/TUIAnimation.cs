using UnityEngine;

public class TUIAnimation : MonoBehaviour
{
	public enum Method
	{
		Line,
		Damped,
		ExponentialOut,
		ExponentialIn,
		SineIn,
		SineOut,
		SineInAndOut
	}

	public string m_strName;

	public bool m_bLoop;

	public bool m_bPingPong;

	public Method m_method;

	public float m_fTimeLength;

	protected float m_fTime;

	public bool m_bFinished;

	protected float m_fDirection = 1f;

	private void Update()
	{
		if (m_bFinished)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		m_fTime += deltaTime * m_fDirection;
		if (m_bLoop)
		{
			if (m_bPingPong)
			{
				if (m_fTime >= m_fTimeLength)
				{
					m_fTime = m_fTimeLength;
					m_fDirection *= -1f;
				}
				else if (m_fTime <= 0f)
				{
					m_fTime = 0f;
					m_fDirection *= -1f;
				}
			}
			else if (m_fTime >= m_fTimeLength)
			{
				m_fTime = 0f;
			}
		}
		else if (m_bPingPong)
		{
			if (m_fTime >= m_fTimeLength)
			{
				m_fTime = m_fTimeLength;
				m_fDirection *= -1f;
			}
			else if (m_fTime <= 0f)
			{
				m_bFinished = true;
			}
		}
		else if (m_fTime >= m_fTimeLength)
		{
			m_bFinished = true;
		}
		DoUpdate(deltaTime);
	}

	protected virtual void DoUpdate(float deltaTime)
	{
	}

	public virtual void UpdateImmdietly()
	{
		m_fTimeLength = m_fTime;
		DoUpdate(m_fTime);
		m_bFinished = true;
	}
}
