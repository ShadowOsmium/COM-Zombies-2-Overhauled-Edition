using UnityEngine;

public class TUITransYAnim : TUIValueAnim
{
	private TUIControlImpl m_Controller;

	private TUINeedUpdateBase m_NeedUpdateBase;

	private void Start()
	{
		m_fBeginValue = base.transform.position.y;
		m_Controller = base.gameObject.GetComponent<TUIControlImpl>();
		m_NeedUpdateBase = base.gameObject.GetComponent<TUINeedUpdateBase>();
	}

	protected override void OnValueUpdate(float value)
	{
		Vector3 position = base.transform.position;
		position.y = value;
		base.transform.position = position;
		if (null != m_Controller && null != m_Controller.m_showClipObj && null != m_NeedUpdateBase)
		{
			m_NeedUpdateBase.NeedUpdate = true;
		}
	}
}
