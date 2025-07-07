using System.Collections;
using CoMZ2;
using UnityEngine;

public class ShooterGameCamera : MonoBehaviour
{
	public GameObject shake_obj;

	private GameObject sub_shake_obj;

	public Transform player;

	public Transform target;

	public float smooth_speed = 5f;

	public Vector3 pivotOffset = new Vector3(0f, 1f, 0f);

	public Vector3 camOffset = new Vector3(1f, 0.6f, -2.5f);

	public Vector3 closeOffset = new Vector3(0f, 1.8f, 0f);

	public float cameraSwingSpeed = 100f;

	public float maxVerticalAngle = 20f;

	public float minVerticalAngle = -20f;

	private float angleH;

	private float angleV;

	private Transform camera_trans;

	private float maxCamDist = 1f;

	private LayerMask mask;

	private Vector3 smoothPlayerPos;

	private bool inited;

	protected float rx;

	protected float ry;

	public float CAMERA_CHAISAW_FOV = 55f;

	public float CAMERA_AIM_FOV = 50f;

	public float CAMERA_NORMAL_FOV = 70f;

	public float padding = 0.3f;

	public Vector3 camOffset_In = new Vector3(0.4f, 0.8f, -1.2f);

	public Vector3 camOffset_Out = new Vector3(0.6f, 1.1f, -2f);

	protected float winTime = -1f;

	public bool camera_pause;

	protected bool camera_shake;

	private Vector3 before_shake_ori_pos;

	private Quaternion before_shake_ori_rot;

	private Vector3 before_sub_shake_ori_pos;

	private Quaternion before_sub_shake_ori_rot;

	public float AngleV
	{
		get
		{
			return angleV;
		}
	}

	private IEnumerator Start()
	{
		GameSceneController.Instance.main_camera = this;
		mask = (1 << PhysicsLayer.WALL) | (1 << PhysicsLayer.WALL_METAL) | (1 << PhysicsLayer.WALL_WOOD);
		camera_trans = base.transform;
		maxCamDist = 3f;
		while (player == null)
		{
			yield return 1;
		}
		smoothPlayerPos = player.position;
		while (target == null)
		{
			yield return 1;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			cameraSwingSpeed *= 0.4f;
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			cameraSwingSpeed *= 0.6f;
		}
		inited = true;
	}

	private void LateUpdate()
	{
		if (Time.deltaTime == 0f || Time.timeScale == 0f || player == null || !inited)
		{
			return;
		}
		if (camera_pause)
		{
			if (camera_shake)
			{
				camera_trans.position = before_shake_ori_pos;
				camera_trans.rotation = before_shake_ori_rot;
				camera_trans.Translate(sub_shake_obj.transform.position - before_sub_shake_ori_pos);
				camera_trans.Rotate(sub_shake_obj.transform.rotation.eulerAngles - before_sub_shake_ori_rot.eulerAngles);
			}
		}
		else
		{
			if (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
			{
				return;
			}
			if (GameSceneController.Instance.GamePlayingState == PlayingState.Gaming)
			{
				if (base.GetComponent<Camera>().nearClipPlane != 0.1f)
				{
					base.GetComponent<Camera>().nearClipPlane = 0.1f;
				}
				rx = GameSceneController.Instance.input_controller.rotationX;
				ry = GameSceneController.Instance.input_controller.rotationY;
				angleH += rx * cameraSwingSpeed * 0.03f;
				angleV += ry * cameraSwingSpeed * 0.03f;
				angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
				float magnitude = (target.position - camera_trans.position).magnitude;
				Quaternion quaternion = Quaternion.Euler(0f - angleV, angleH, 0f);
				Quaternion quaternion2 = Quaternion.Euler(0f, angleH, 0f);
				camera_trans.rotation = quaternion;
				if (player == null)
				{
					player = GameSceneController.Instance.player_controller.transform;
				}
				player.rotation = Quaternion.Euler(0f, angleH, 0f);
				smoothPlayerPos = Vector3.MoveTowards(smoothPlayerPos, player.position, smooth_speed * Time.deltaTime);
				smoothPlayerPos.x = player.position.x;
				smoothPlayerPos.z = player.position.z;
				Vector3 vector = smoothPlayerPos + quaternion2 * pivotOffset + quaternion * camOffset;
				Vector3 vector2 = player.position + quaternion2 * closeOffset;
				float num = Vector3.Distance(vector, vector2);
				maxCamDist = Mathf.Lerp(maxCamDist, num, 5f * Time.deltaTime);
				Vector3 vector3 = (vector - vector2) / num;
				RaycastHit hitInfo;
				if (Physics.Raycast(vector2, vector3, out hitInfo, maxCamDist + padding, mask))
				{
					maxCamDist = hitInfo.distance - padding;
				}
				camera_trans.position = vector2 + vector3 * maxCamDist;
				float num2 = ((!Physics.Raycast(camera_trans.position, camera_trans.forward, out hitInfo, 100f, mask)) ? Mathf.Max(5f, magnitude) : (hitInfo.distance + 0.05f));
				target.position = camera_trans.position + camera_trans.forward * num2;
				if (camera_shake)
				{
					camera_trans.Translate(sub_shake_obj.transform.position - before_sub_shake_ori_pos);
					camera_trans.Rotate(sub_shake_obj.transform.rotation.eulerAngles - before_sub_shake_ori_rot.eulerAngles);
				}
			}
			else if (GameSceneController.Instance.GamePlayingState == PlayingState.Lose)
			{
				if (base.GetComponent<Camera>().nearClipPlane != 0.3f)
				{
					base.GetComponent<Camera>().nearClipPlane = 0.3f;
				}
				camera_trans.position = player.TransformPoint(3f * Mathf.Sin(Time.time * 0.3f), 4f, 3f * Mathf.Cos(Time.time * 0.3f));
				camera_trans.LookAt(player);
			}
			else if (GameSceneController.Instance.GamePlayingState == PlayingState.Win)
			{
				if (winTime == -1f)
				{
					winTime = Time.time;
				}
				float num3 = Time.time - winTime;
				camera_trans.position = player.TransformPoint(3f * Mathf.Sin((num3 - 1.7f) * 0.3f), 2f, 3f * Mathf.Cos((num3 - 1.7f) * 0.3f));
				camera_trans.LookAt(player);
			}
			GameSceneController.Instance.input_controller.ResetInput();
		}
	}

	public virtual void ZoomIn(float deltaTime)
	{
		base.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(base.GetComponent<Camera>().fieldOfView, CAMERA_AIM_FOV, deltaTime * 120f);
	}

	public virtual void ZoomOut(float deltaTime)
	{
		base.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(base.GetComponent<Camera>().fieldOfView, CAMERA_NORMAL_FOV, deltaTime * 120f);
	}

	public virtual void ZoomChaisawSpecial(float deltaTime)
	{
		base.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(base.GetComponent<Camera>().fieldOfView, CAMERA_CHAISAW_FOV, deltaTime * 120f);
	}

	public void SetAngleH(float h)
	{
		angleH = h;
	}

	public void StartShake(string shake_ani)
	{
		before_shake_ori_pos = base.transform.position;
		before_shake_ori_rot = base.transform.rotation;
		camera_shake = true;
		switch (shake_ani)
		{
		case "Camera_Shake01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Shake01/Dummy_Camera_Shake01/Camera01").gameObject;
			break;
		case "Boss_FatCook_Camera_Death01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_FatCook_Death01/Dummy_Camera_FatCook_Death01/Camera01").gameObject;
			break;
		case "Boss_FatCook_Camera_Bellow01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_FatCook_Bellow01/Dummy_Camera_FatCook_Bellow01/Camera01").gameObject;
			break;
		case "Boss_FatCook_Camera_Show01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_FatCook_Show01/Dummy_Camera_FatCook_Show01/Camera01").gameObject;
			break;
		case "Boss_FatCook_Camera_Show02":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_FatCook_Show02/Dummy_Camera_FatCook_Show02/Camera01").gameObject;
			break;
		case "Haoke_Camera_Death01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_HaoKe_Death01/Dummy_Camera_HaoKe_Death01/Camera01").gameObject;
			break;
		case "Haoke_Camera_Bellow01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_HaoKe_Bellow01/Dummy_Camera_HaoKe_Bellow01/Camera01").gameObject;
			break;
		case "Haoke_Camera_Show01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_HaoKe_Show01/Dummy_Camera_HaoKe_Show01/Camera01").gameObject;
			break;
		case "Haoke_Camera_Show02":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_HaoKe_Show02/Dummy_Camera_HaoKe_Show02/Camera01").gameObject;
			break;
		case "Wrestler_Camera_Bellow01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Wrestler_Bellow01/Dummy_Camera_Wrestler_Bellow01/Camera01").gameObject;
			break;
		case "Wrestler_Camera_Show01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Wrestler_Show01/Dummy_Camera_Wrestler_Show01/Camera01").gameObject;
			break;
		case "Wrestler_Camera_Show02":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Wrestler_Show02/Dummy_Camera_Boss_Wrestler_Show02/Camera01").gameObject;
			break;
		case "Hook_Demon_Camera_Bellow01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Hook_Demon_Bellow01/Dummy_Camera_Hook_Demon_Bellow01/Camera01").gameObject;
			break;
		case "Hook_Demon_Camera_Show01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Hook_Demon_Show01/Dummy_Camera_Hook_Demon_Show01/Camera01").gameObject;
			break;
		case "Hook_Demon_Camera_Show02":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Hook_Demon_Show02/Dummy_Camera_Hook_Demon_Show02/Camera01").gameObject;
			break;
		case "Zombie_Guter_Tennung_Camera_Bellow01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Zombie_Guter_Trennung_Bellow01/Dummy_Camera_Zombie_Guter_Trennung_Bellow01/Camera01").gameObject;
			break;
		case "Zombie_Guter_Tennung_Camera_Show01":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Zombie_Guter_Tennung_Show01/Dummy_Camera_Zombie_Guter_Tennung_Show01/Camera01").gameObject;
			break;
		case "Zombie_Guter_Tennung_Camera_Show02":
			sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Zombie_Guter_Tennung_Show02/Dummy_Camera_Zombie_Guter_Tennung_Show02/Camera01").gameObject;
			break;
		}
		AnimationUtil.Stop(shake_obj);
		CancelInvoke("StopShake");
		before_sub_shake_ori_pos = sub_shake_obj.transform.position;
		before_sub_shake_ori_rot = sub_shake_obj.transform.rotation;
		AnimationUtil.PlayAnimate(shake_obj, shake_ani, WrapMode.Once);
        var animClip = shake_obj.GetComponent<Animation>()[shake_ani];

        if (shake_ani == "Haoke_Camera_Show02")
        {
            float forcedDuration = 1.5f;
            if (animClip.length > forcedDuration)
            {
                animClip.speed = animClip.length / forcedDuration;
                Invoke("StopShake", forcedDuration);
            }
            else
            {
                Invoke("StopShake", animClip.length);
            }
        }
        else
        {
            Invoke("StopShake", animClip.length);
        }
    }

	public void StopShake()
	{
		camera_shake = false;
		camera_pause = false;
		base.transform.parent = null;
		GameSceneController.Instance.HidePanels();
		GameSceneController.Instance.game_main_panel.Show();
	}

	public void StartCommonShake()
	{
		before_shake_ori_pos = base.transform.position;
		before_shake_ori_rot = base.transform.rotation;
		camera_shake = true;
		string animationName = "Camera_Shake01";
		sub_shake_obj = shake_obj.transform.Find("Dummy_Root_Camera_Shake01/Dummy_Camera_Shake01/Camera01").gameObject;
		AnimationUtil.Stop(shake_obj);
		CancelInvoke("StopCommonShake");
		before_sub_shake_ori_pos = sub_shake_obj.transform.position;
		before_sub_shake_ori_rot = sub_shake_obj.transform.rotation;
		AnimationUtil.PlayAnimate(shake_obj, animationName, WrapMode.Once);
		Invoke("StopCommonShake", shake_obj.GetComponent<Animation>()[animationName].length);
	}

	public void StopCommonShake()
	{
		camera_shake = false;
		camera_pause = false;
		base.transform.parent = null;
	}
}
