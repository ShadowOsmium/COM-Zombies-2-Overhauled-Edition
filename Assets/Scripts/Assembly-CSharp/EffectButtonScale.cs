using UnityEngine;

[RequireComponent(typeof(TUIScaleAnim))]
public class EffectButtonScale : MonoBehaviour
{
	[SerializeField]
	protected float upScale;

	[SerializeField]
	protected float downScale;

	protected TUIScaleAnim scaleAnim;

	private void Awake()
	{
		scaleAnim = GetComponent<TUIScaleAnim>();
	}

	private void OnDown()
	{
		scaleAnim.m_fEndValue = downScale;
		scaleAnim.m_bFinished = false;
		scaleAnim.UpdateImmdietly();
	}

	private void OnUp()
	{
		scaleAnim.m_fEndValue = upScale;
		scaleAnim.m_bFinished = false;
		scaleAnim.UpdateImmdietly();
	}
}
