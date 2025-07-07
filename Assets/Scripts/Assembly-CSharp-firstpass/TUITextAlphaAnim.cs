using UnityEngine;

public class TUITextAlphaAnim : TUIValueAnim
{
	private Material m_Material;

	private void Start()
	{
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		m_Material = component.material;
		m_fBeginValue = m_Material.color.a;
	}

	protected override void OnValueUpdate(float value)
	{
		Color color = m_Material.color;
		color.a = value;
		m_Material.color = color;
	}
}
