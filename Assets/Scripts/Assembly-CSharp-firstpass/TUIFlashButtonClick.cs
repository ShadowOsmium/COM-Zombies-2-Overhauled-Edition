using UnityEngine;

public class TUIFlashButtonClick : TUIButtonClick
{
	public string[] m_FlashTextures;

	public float m_fFlashTime = 0.1f;

	public float m_fFlashGap;

	private float m_fStartTime;

	private int m_iTextureIndex;

	private bool m_bFlashing = true;

	private void Update()
	{
		if (m_bDisable || m_FlashTextures.Length == 0)
		{
			return;
		}
		m_fStartTime += Time.deltaTime;
		if (m_bFlashing)
		{
			if (m_fStartTime > m_fFlashTime)
			{
				m_iTextureIndex++;
				m_iTextureIndex %= m_FlashTextures.Length;
				if (m_fFlashGap > 0f && m_iTextureIndex == 0)
				{
					m_bFlashing = false;
				}
				m_fStartTime = 0f;
				if (!m_bPressed)
				{
					Show();
				}
			}
		}
		else if (m_fStartTime >= m_fFlashGap)
		{
			m_fStartTime = 0f;
			m_bFlashing = true;
		}
	}
}
