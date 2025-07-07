using UnityEngine;

public class SafeInteger
{
	private const int m_f2_param1 = 23;

	private const int m_f2_param2 = 67;

	private int m_value;

	private int m_f1_param1;

	private int m_f1_param2;

	private int m_f1;

	private int m_f2;

	public SafeInteger()
	{
		m_value = 0;
		m_f1_param1 = Random.Range(5, 55);
		m_f1_param2 = Random.Range(1, 99);
		m_f1 = 0;
		m_f2 = 0;
		Set(0);
	}

	public int Get()
	{
		return DoGet();
	}

	public void Set(int value)
	{
		DoSet(value);
	}

	private int DoGet()
	{
		int num = m_f1_param1 * m_value + m_f1_param2;
		if (num != m_f1)
		{
			DoSet(0);
			return m_value;
		}
		int num2 = 23 * m_value * m_value + 67;
		if (num2 != m_f2)
		{
			DoSet(0);
			return m_value;
		}
		return m_value;
	}

	public void DoSet(int value)
	{
		m_value = value;
		m_f1 = m_f1_param1 * m_value + m_f1_param2;
		m_f2 = 23 * m_value * m_value + 67;
	}
}
