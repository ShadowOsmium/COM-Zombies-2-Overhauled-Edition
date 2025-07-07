using UnityEngine;

public class TUIRotateAnim : TUIValueAnim
{
	private void Start()
	{
		m_fBeginValue = base.transform.eulerAngles.z;
	}

	protected override void OnValueUpdate(float value)
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		base.transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, value);
	}
}
