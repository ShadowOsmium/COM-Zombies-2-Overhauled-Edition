public class TUIValueAnim : TUIAnimation
{
	protected float m_fBeginValue;

	public float m_fEndValue;

	private float m_fDeltaValue;

	private void Awake()
	{
		m_fDeltaValue = m_fEndValue - m_fBeginValue;
	}

	protected override void DoUpdate(float deltaTime)
	{
		float value = Algorithm.Lerp(m_method, m_fBeginValue, m_fEndValue, m_fDeltaValue, m_fTime, deltaTime, m_fTimeLength);
		if (m_bFinished)
		{
			value = ((!m_bPingPong) ? m_fEndValue : m_fBeginValue);
		}
		OnValueUpdate(value);
	}

	protected virtual void OnValueUpdate(float value)
	{
	}
}
