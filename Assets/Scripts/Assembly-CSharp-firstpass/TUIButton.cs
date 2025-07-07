using UnityEngine;

public class TUIButton : TUIControlImpl
{
	public GameObject m_NormalObj;

	public GameObject m_PressObj;

	public GameObject m_DisableObj;

	public GameObject m_NormalLabelObj;

	public GameObject m_PressLabelObj;

	public GameObject m_DisableLabelObj;

	public bool m_bDisable;

	public bool m_bPressed;

	protected int m_iFingerId = -1;

	public void Start()
	{
		Show();
	}

	public override void Reset()
	{
		base.Reset();
		m_bDisable = false;
		m_bPressed = false;
		m_iFingerId = -1;
		Show();
	}

	public override void Activate(bool bActive)
	{
		base.gameObject.SetActiveRecursively(bActive);
		if (bActive)
		{
			Show();
		}
	}

	public virtual void Disable(bool bValue)
	{
		m_bDisable = bValue;
		Show();
	}

	public virtual void Show()
	{
		if (m_bDisable)
		{
			if (null != m_NormalObj)
			{
				m_NormalObj.SetActiveRecursively(false);
			}
			if (null != m_PressObj)
			{
				m_PressObj.SetActiveRecursively(false);
			}
			if (null != m_DisableObj)
			{
				m_DisableObj.SetActiveRecursively(true);
			}
			if (null != m_NormalLabelObj)
			{
				m_NormalLabelObj.SetActiveRecursively(false);
			}
			if (null != m_PressLabelObj)
			{
				m_PressLabelObj.SetActiveRecursively(false);
			}
			if (null != m_DisableLabelObj)
			{
				m_DisableLabelObj.SetActiveRecursively(true);
			}
		}
		else if (m_bPressed)
		{
			if (null != m_NormalObj)
			{
				m_NormalObj.SetActiveRecursively(false);
			}
			if (null != m_PressObj)
			{
				m_PressObj.SetActiveRecursively(true);
			}
			if (null != m_DisableObj)
			{
				m_DisableObj.SetActiveRecursively(false);
			}
			if (null != m_NormalLabelObj)
			{
				m_NormalLabelObj.SetActiveRecursively(false);
			}
			if (null != m_PressLabelObj)
			{
				m_PressLabelObj.SetActiveRecursively(true);
			}
			if (null != m_DisableLabelObj)
			{
				m_DisableLabelObj.SetActiveRecursively(false);
			}
		}
		else
		{
			if (null != m_NormalObj)
			{
				m_NormalObj.SetActiveRecursively(true);
			}
			if (null != m_PressObj)
			{
				m_PressObj.SetActiveRecursively(false);
			}
			if (null != m_DisableObj)
			{
				m_DisableObj.SetActiveRecursively(false);
			}
			if (null != m_NormalLabelObj)
			{
				m_NormalLabelObj.SetActiveRecursively(true);
			}
			if (null != m_PressLabelObj)
			{
				m_PressLabelObj.SetActiveRecursively(false);
			}
			if (null != m_DisableLabelObj)
			{
				m_DisableLabelObj.SetActiveRecursively(false);
			}
		}
	}
}
