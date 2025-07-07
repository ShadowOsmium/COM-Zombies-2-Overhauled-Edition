using UnityEngine;

public class TUIClipBinder : TUIControl
{
	[SerializeField]
	protected TUIRect clipRect;

	private void Awake()
	{
		if (null != clipRect)
		{
			UpdateClipRect(clipRect);
		}
	}

	public void SetClipRect(TUIRect rect)
	{
		clipRect = rect;
		UpdateClipRect(clipRect);
	}

	private void UpdateClipRect(TUIRect rect)
	{
		TUIMeshSprite[] componentsInChildren = GetComponentsInChildren<TUIMeshSprite>(true);
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			TUIMeshSprite[] array = componentsInChildren;
			foreach (TUIMeshSprite tUIMeshSprite in array)
			{
				if (null == rect)
				{
					tUIMeshSprite.updateForever = false;
					tUIMeshSprite.m_showClipObj = null;
				}
				else
				{
					tUIMeshSprite.updateForever = true;
					tUIMeshSprite.m_showClipObj = rect.gameObject;
				}
			}
		}
		TUIDrawSprite[] componentsInChildren2 = GetComponentsInChildren<TUIDrawSprite>(true);
		if (componentsInChildren2 == null || componentsInChildren2.Length <= 0)
		{
			return;
		}
		TUIDrawSprite[] array2 = componentsInChildren2;
		foreach (TUIDrawSprite tUIDrawSprite in array2)
		{
			if (null == rect)
			{
				tUIDrawSprite.clippingType = TUIDrawSprite.Clipping.None;
				tUIDrawSprite.clippingRect = null;
			}
			else
			{
				tUIDrawSprite.clippingType = TUIDrawSprite.Clipping.HardClip;
				tUIDrawSprite.clippingRect = rect;
			}
		}
	}
}
