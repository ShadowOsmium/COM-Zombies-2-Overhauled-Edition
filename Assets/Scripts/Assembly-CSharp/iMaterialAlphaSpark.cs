using UnityEngine;

public class iMaterialAlphaSpark : MonoBehaviour
{
	public string sTexName = string.Empty;

	public Color colorSrc;

	public Color colorDst;

	public float fSpeed = 1f;

	protected Renderer m_Renderer;

	protected float m_fRate;

	protected int m_nDir;

	private void Awake()
	{
		m_Renderer = GetComponent<Renderer>();
		if (m_Renderer != null)
		{
			m_fRate = 0f;
			m_nDir = 1;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!(m_Renderer == null))
		{
			Color color = Color.Lerp(colorSrc, colorDst, m_fRate);
			m_fRate += Time.deltaTime * fSpeed * (float)m_nDir;
			if (m_fRate >= 1f && m_nDir == 1)
			{
				m_nDir = -1;
				m_fRate = 1f;
			}
			else if (m_fRate <= 0f && m_nDir == -1)
			{
				m_nDir = 1;
				m_fRate = 0f;
			}
			m_Renderer.material.SetColor(sTexName, color);
		}
	}
}
