using UnityEngine;

public class ImageSaveAniEvent : MonoBehaviour
{
	public void OnAniEnd()
	{
		Camera.main.GetComponent<ImageSaveEffectManager>().eff_set.Remove(base.gameObject);
	}
}
