using System.Collections.Generic;
using UnityEngine;

public class TUISelfAdaptiveAnchorGroup : MonoBehaviour
{
	public bool lock568x384 = true;

	public bool anchorWhenStart = true;

	public List<Transform> trans;

	private void Start()
	{
		if (anchorWhenStart)
		{
			Anchor();
		}
	}

	public void Anchor()
	{
		if (trans == null)
		{
			return;
		}
		foreach (Transform tran in trans)
		{
			TUISelfAdaptiveAnchor.CastAnchor(base.transform.root.GetComponent<TUI>().Camera, tran, lock568x384);
		}
	}
}
