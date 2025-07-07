using UnityEngine;

public class TUITransXAnim : TUIValueAnim
{
	private TUINeedUpdateBase m_NeedUpdateBase;

	private void Start()
	{
		m_fBeginValue = base.transform.position.x;
		m_NeedUpdateBase = base.gameObject.GetComponent<TUINeedUpdateBase>();
	}

	protected override void OnValueUpdate(float value)
	{
		Vector3 position = base.transform.position;
		position.x = value;
		base.transform.position = position;
		if (null != m_NeedUpdateBase)
		{
			m_NeedUpdateBase.NeedUpdate = true;
		}
	}
}
