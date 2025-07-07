public class TUIImageAlphaAnim : TUIValueAnim
{
	private TUIMeshSprite m_Sprite;

	private void Start()
	{
		m_Sprite = base.gameObject.GetComponent<TUIMeshSprite>();
		m_fBeginValue = m_Sprite.alpha;
	}

	protected override void OnValueUpdate(float value)
	{
		m_Sprite.alpha = value;
	}
}
