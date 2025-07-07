using UnityEngine;

public class UIPanelController : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public virtual void Show()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		base.gameObject.SetActive(true);
	}

	public virtual void Hide()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		base.gameObject.SetActive(false);
	}
}
