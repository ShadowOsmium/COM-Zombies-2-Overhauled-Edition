using CoMZ2;
using UnityEngine;

public class WeaponProperty : MonoBehaviour
{
	public TUILabel Name;

	public TUILabel Value;

	public TUILabel UpgradeValue;

	public TUIButtonClick UpgradeButton;

	public WeaponPropertyType Type { get; private set; }

	public void Show(WeaponPropertyType type)
	{
		base.gameObject.SetActive(true);
		Type = type;
	}

	public void Clear()
	{
		Type = WeaponPropertyType.None;
		base.gameObject.SetActive(false);
	}
}
