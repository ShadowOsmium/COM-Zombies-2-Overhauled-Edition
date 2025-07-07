using UnityEngine;

public class ScreenBloodController : MonoBehaviour
{
	public float speed = 1f;

	private bool is_injure;

	private TUIMeshSprite m_sprite;

	private void Start()
	{
		m_sprite = GetComponent<TUIMeshSprite>();
	}

	private void Update()
	{
		if (!(m_sprite == null) && is_injure)
		{
			m_sprite.color = new Color(1f, 1f, 1f, m_sprite.color.a - speed * Time.deltaTime);
			if (m_sprite.color.a <= 0.1f)
			{
				m_sprite.color = new Color(1f, 1f, 1f, 0f);
				is_injure = false;
			}
		}
	}

	public void OnInjured(bool random_rot = false)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		if (m_sprite == null)
		{
			m_sprite = GetComponent<TUIMeshSprite>();
		}
		m_sprite.color = Color.white;
		is_injure = true;
		if (random_rot)
		{
			base.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0, 360)));
		}
	}
}
