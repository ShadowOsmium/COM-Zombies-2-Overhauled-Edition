using UnityEngine;

public class TUINeedUpdateBase : MonoBehaviour
{
	public bool updateForever;

	private bool m_bNeedUpdate = true;

	public bool NeedUpdate
	{
		get
		{
			if (updateForever)
			{
				m_bNeedUpdate = true;
			}
			return m_bNeedUpdate;
		}
		set
		{
			if (updateForever)
			{
				m_bNeedUpdate = true;
			}
			else
			{
				m_bNeedUpdate = value;
			}
		}
	}

	public void Awake()
	{
		m_bNeedUpdate = true;
	}
}
