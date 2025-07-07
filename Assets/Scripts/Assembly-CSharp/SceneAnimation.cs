using UnityEngine;

public class SceneAnimation : MonoBehaviour
{
	public string ani_name = string.Empty;

	public WrapMode mode = WrapMode.Once;

	private void Start()
	{
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>()[ani_name].clip.wrapMode = mode;
			base.GetComponent<Animation>().Play(ani_name);
		}
	}

	private void Update()
	{
	}
}
