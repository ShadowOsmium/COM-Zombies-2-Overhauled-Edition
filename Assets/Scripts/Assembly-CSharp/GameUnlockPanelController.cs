using UnityEngine;

public class GameUnlockPanelController : UIPanelController
{
	public Transform render_obj_pos;

	public Transform render_eff_pos;

	public Camera render_camera;

	private GameObject eff_obj;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void EnableCameraTexture(GameObject render_obj, GameObject eff_obj_tem)
	{
		render_camera.gameObject.SetActive(true);
		render_obj.transform.parent = render_obj_pos;
		render_obj.transform.localPosition = Vector3.zero;
		render_obj.transform.localRotation = Quaternion.identity;
		eff_obj = eff_obj_tem;
		eff_obj.transform.parent = render_eff_pos;
		eff_obj.transform.localPosition = Vector3.zero;
		eff_obj.transform.localRotation = Quaternion.identity;
		Invoke("PlayLockEff", 0.2f);
	}

	private void PlayLockEff()
	{
		GameObject obj = eff_obj.transform.Find("suo").gameObject;
		AnimationUtil.PlayAnimate(obj, "suo", WrapMode.Once);
	}
}
