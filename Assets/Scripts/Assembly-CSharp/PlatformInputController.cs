using CoMZ2;
using UnityEngine;

public class PlatformInputController : MonoBehaviour
{
	private CharacterMotor motor;

	public bool is_mobile;

	private float sensitivity_fire = 1f;

	public float rotationX;

	public float rotationY;

	private float moveX;

	private float moveY;

	private bool is_primary_fire;

	private bool is_move;

	public GameObject move_joy;

	public GameObject shoot_joy;

	public TUIEventRect move_event_rect;

	public TUIEventRect fire_event_rect;

	private int shootingTouchFingerId = -1;

	private Vector2 lastFireTouch = Vector2.zero;

	private bool is_primary_fire_this_time;

	public CharacterMotor Motor
	{
		get
		{
			return motor;
		}
		set
		{
			motor = value;
		}
	}

	public bool PrimaryFire
	{
		get
		{
			return is_primary_fire;
		}
	}

	public bool Fire
	{
		get
		{
			return is_primary_fire;
		}
	}

	public bool Moving
	{
		get
		{
			return is_move;
		}
	}

	public float SensitivityVal
	{
		get
		{
			if (is_mobile)
			{
				return 1f * GameData.Instance.sensitivity_ratio;
			}
			return 10f * GameData.Instance.sensitivity_ratio;
		}
	}

	private void Start()
	{
		GameSceneController.Instance.input_controller = this;
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			is_mobile = true;
		}
		fire_event_rect.on_touch_event = OnFireEvent;
	}

	private void OnDestroy()
	{
		fire_event_rect.on_touch_event = null;
	}

	private void Update()
	{
		if (motor == null || GameSceneController.Instance.GamePlayingState == PlayingState.CG)
		{
			return;
		}
		if (GameSceneController.Instance.GamePlayingState == PlayingState.Lose || GameSceneController.Instance.GamePlayingState == PlayingState.Win)
		{
			motor.desiredMovementDirection = Vector3.zero;
			is_primary_fire = false;
			GameSceneController.Instance.player_controller.CheckFireWeapon();
			return;
		}
		Vector3 zero = Vector3.zero;
		if (is_mobile)
		{
			zero = new Vector3(moveX, moveY, 0f);
			zero.Normalize();
		}
		else
		{
			zero = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
		}
		if (!GameSceneController.Instance.EnableControll())
		{
			zero = Vector3.zero;
		}
		if (zero.magnitude > 1f)
		{
			zero = zero.normalized;
		}
		zero = zero.normalized * Mathf.Pow(zero.magnitude, 2f);
		zero = Camera.main.transform.rotation * zero;
		Quaternion quaternion = Quaternion.FromToRotation(Camera.main.transform.forward * -1f, base.transform.up);
		zero = quaternion * zero;
		zero = Quaternion.Inverse(base.transform.rotation) * zero;
		if (!is_mobile)
		{
			if (Fire)
			{
				rotationX = Input.GetAxis("Mouse X") * SensitivityVal * Time.deltaTime * sensitivity_fire;
				rotationY = Input.GetAxis("Mouse Y") * SensitivityVal * Time.deltaTime * sensitivity_fire;
			}
			else
			{
				rotationX = Input.GetAxis("Mouse X") * SensitivityVal * Time.deltaTime;
				rotationY = Input.GetAxis("Mouse Y") * SensitivityVal * Time.deltaTime;
			}
		}
		if (motor != null)
		{
			motor.desiredMovementDirection = zero;
		}
		if (zero.sqrMagnitude > 0.0001f)
		{
			is_move = true;
		}
		else
		{
			is_move = false;
		}
		if (!is_mobile)
		{
			if (Input.GetButton("Fire1"))
			{
				is_primary_fire = true;
			}
			else
			{
				is_primary_fire = false;
			}
			GameSceneController.Instance.player_controller.CheckFireWeapon();
		}
		if (GameSceneController.Instance.player_controller.EnableWeaponUIProcess())
		{
			if (Input.GetButtonDown("SwitchGun"))
			{
				GameSceneController.Instance.player_controller.ChangeNextPrimaryWeapon();
			}
			if (Input.GetButtonDown("Reload") && GameSceneController.Instance.player_controller.EnableReload())
			{
				GameSceneController.Instance.player_controller.WeaponReload();
			}
			if (Input.GetButtonDown("Skill1"))
			{
				GameSceneController.Instance.player_controller.ConjureSkill(0);
			}
			if (Input.GetButtonDown("Skill2"))
			{
				GameSceneController.Instance.player_controller.ConjureSkill(1);
			}
		}
	}

	private void OnMoveButton(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (is_mobile)
		{
			switch (eventType)
			{
			case 2:
				moveX = wparam;
				moveY = lparam;
				break;
			case 3:
				moveX = 0f;
				moveY = 0f;
				move_joy.transform.localPosition = new Vector3(0f, 2000f, move_joy.transform.localPosition.z);
				move_event_rect.size = new Vector2(210f, 320f);
				break;
			}
		}
	}

	private void OnFireEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (!is_mobile)
		{
			return;
		}
		TUIInput input = (TUIInput)data;
		if (input.inputType == TUIInputType.Began)
		{
			if (shootingTouchFingerId == -1 || shootingTouchFingerId == input.fingerId)
			{
				is_primary_fire = false;
				is_primary_fire_this_time = false;
				if (shoot_joy.GetComponent<TUIControlImpl>().PtInControl(input.position))
				{
					is_primary_fire_this_time = (is_primary_fire = true);
					shoot_joy.GetComponent<TUIButtonJoystickSim>().SimPress(input);
					GameSceneController.Instance.player_controller.CheckFireWeapon();
				}
				shootingTouchFingerId = input.fingerId;
				lastFireTouch = input.position;
			}
		}
		else if (input.inputType == TUIInputType.Moved || input.inputType == TUIInputType.Stationary)
		{
			if (shootingTouchFingerId == input.fingerId)
			{
				Vector2 vector = input.position - lastFireTouch;
				if (Fire)
				{
					rotationX = vector.x * SensitivityVal * sensitivity_fire;
					rotationY = vector.y * SensitivityVal * sensitivity_fire;
				}
				else
				{
					rotationX = vector.x * SensitivityVal;
					rotationY = vector.y * SensitivityVal;
				}
				lastFireTouch = input.position;
				is_primary_fire = is_primary_fire_this_time;
			}
		}
		else if ((input.inputType == TUIInputType.Ended || input.inputType == TUIInputType.Canceled) && shootingTouchFingerId == input.fingerId)
		{
			is_primary_fire = false;
			is_primary_fire_this_time = false;
			shootingTouchFingerId = -1;
		}
	}

	private void OnMoveStart(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (!is_mobile)
		{
			return;
		}
		TUIInput input = (TUIInput)data;
		if (input.inputType == TUIInputType.Began)
		{
			if (!move_joy.GetComponent<TUIButtonJoystickSim>().m_bPressed)
			{
				move_joy.transform.localPosition = new Vector3(input.position.x, input.position.y, move_joy.transform.localPosition.z);
				move_joy.GetComponent<TUIButtonJoystickSim>().SimPress(input);
				move_event_rect.size = new Vector2(0f, 0f);
			}
		}
		else if (input.inputType != TUIInputType.Moved && input.inputType != TUIInputType.Ended)
		{
		}
	}

	public void ResetInput()
	{
		is_primary_fire = false;
		rotationX = 0f;
		rotationY = 0f;
	}
}
