using UnityEngine;

public class SightBeadScript : MonoBehaviour
{
	public enum SightBeadState
	{
		None,
		Recover
	}

	public Camera tui_camera;

	public Color m_unlock_color = Color.white;

	public Color m_lock_color = Color.white;

	private TUIMeshSprite Sight_Bead_Up;

	private TUIMeshSprite Sight_Bead_Down;

	private TUIMeshSprite Sight_Bead_Left;

	private TUIMeshSprite Sight_Bead_Right;

	private Vector3 Sight_Bead_Up_Dir;

	private Vector3 Sight_Bead_Down_Dir;

	private Vector3 Sight_Bead_Left_Dir;

	private Vector3 Sight_Bead_Right_Dir;

	private Vector3 Sight_Bead_Up_Start;

	private Vector3 Sight_Bead_Down_Start;

	private Vector3 Sight_Bead_Left_Start;

	private Vector3 Sight_Bead_Right_Start;

	private Vector3 Sight_Bead_Up_Ori;

	private Vector3 Sight_Bead_Down_Ori;

	private Vector3 Sight_Bead_Left_Ori;

	private Vector3 Sight_Bead_Right_Ori;

	public float sight_bead_speed;

	private SightBeadState sight_bead_state;

	public float recover_max_speed = 30f;

	public float stretch_range = 20f;

	public float stretchRangeOffset;

	public float stretchRangeRunOffset;

	private void Awake()
	{
		Sight_Bead_Up = base.transform.Find("Sight_Bead_Up").GetComponent<TUIMeshSprite>();
		Sight_Bead_Down = base.transform.Find("Sight_Bead_Down").GetComponent<TUIMeshSprite>();
		Sight_Bead_Left = base.transform.Find("Sight_Bead_Left").GetComponent<TUIMeshSprite>();
		Sight_Bead_Right = base.transform.Find("Sight_Bead_Right").GetComponent<TUIMeshSprite>();
	}

	private void Start()
	{
		Sight_Bead_Up.color = m_unlock_color;
		Sight_Bead_Down.color = m_unlock_color;
		Sight_Bead_Left.color = m_unlock_color;
		Sight_Bead_Right.color = m_unlock_color;
		Sight_Bead_Up_Start = (Sight_Bead_Up_Ori = Sight_Bead_Up.transform.localPosition);
		Sight_Bead_Down_Start = (Sight_Bead_Down_Ori = Sight_Bead_Down.transform.localPosition);
		Sight_Bead_Left_Start = (Sight_Bead_Left_Ori = Sight_Bead_Left.transform.localPosition);
		Sight_Bead_Right_Start = (Sight_Bead_Right_Ori = Sight_Bead_Right.transform.localPosition);
		Sight_Bead_Up_Dir = Sight_Bead_Up_Ori.normalized;
		Sight_Bead_Down_Dir = Sight_Bead_Down_Ori.normalized;
		Sight_Bead_Left_Dir = Sight_Bead_Left_Ori.normalized;
		Sight_Bead_Right_Dir = Sight_Bead_Right_Ori.normalized;
	}

	private void Update()
	{
		if (Time.deltaTime == 0f || Time.timeScale == 0f || sight_bead_state != SightBeadState.Recover)
		{
			return;
		}
		Sight_Bead_Up_Dir = (Sight_Bead_Up_Ori - Sight_Bead_Up.transform.localPosition).normalized;
		Sight_Bead_Down_Dir = (Sight_Bead_Down_Ori - Sight_Bead_Down.transform.localPosition).normalized;
		Sight_Bead_Left_Dir = (Sight_Bead_Left_Ori - Sight_Bead_Left.transform.localPosition).normalized;
		Sight_Bead_Right_Dir = (Sight_Bead_Right_Ori - Sight_Bead_Right.transform.localPosition).normalized;
		Sight_Bead_Up.transform.Translate(Sight_Bead_Up_Dir * recover_max_speed * Time.deltaTime, base.transform);
		Sight_Bead_Down.transform.Translate(Sight_Bead_Down_Dir * recover_max_speed * Time.deltaTime, base.transform);
		Sight_Bead_Left.transform.Translate(Sight_Bead_Left_Dir * recover_max_speed * Time.deltaTime, base.transform);
		Sight_Bead_Right.transform.Translate(Sight_Bead_Right_Dir * recover_max_speed * Time.deltaTime, base.transform);
		if (Vector3.Dot(Sight_Bead_Up_Dir, Sight_Bead_Up_Ori.normalized) >= 0.8f)
		{
			if ((Sight_Bead_Up.transform.localPosition - Sight_Bead_Up_Start).sqrMagnitude >= stretch_range * stretch_range)
			{
				Sight_Bead_Up.transform.localPosition = Sight_Bead_Up_Start + Sight_Bead_Up_Start.normalized * stretch_range;
				Sight_Bead_Down.transform.localPosition = Sight_Bead_Down_Start + Sight_Bead_Down_Start.normalized * stretch_range;
				Sight_Bead_Left.transform.localPosition = Sight_Bead_Left_Start + Sight_Bead_Left_Start.normalized * stretch_range;
				Sight_Bead_Right.transform.localPosition = Sight_Bead_Right_Start + Sight_Bead_Right_Start.normalized * stretch_range;
				sight_bead_state = SightBeadState.None;
				sight_bead_speed = 0f;
			}
		}
		else if (Vector3.Dot(Sight_Bead_Up_Dir, Sight_Bead_Up_Ori.normalized) <= -0.8f && Sight_Bead_Up.transform.localPosition.sqrMagnitude <= Sight_Bead_Up_Ori.sqrMagnitude)
		{
			Sight_Bead_Up.transform.localPosition = Sight_Bead_Up_Ori;
			Sight_Bead_Down.transform.localPosition = Sight_Bead_Down_Ori;
			Sight_Bead_Left.transform.localPosition = Sight_Bead_Left_Ori;
			Sight_Bead_Right.transform.localPosition = Sight_Bead_Right_Ori;
			sight_bead_state = SightBeadState.None;
			sight_bead_speed = 0f;
		}
	}

	public void Stretch(float stretch_val)
	{
		sight_bead_state = SightBeadState.Recover;
		Sight_Bead_Up.transform.Translate(Sight_Bead_Up_Start.normalized * stretch_val, base.transform);
		Sight_Bead_Down.transform.Translate(Sight_Bead_Down_Start.normalized * stretch_val, base.transform);
		Sight_Bead_Left.transform.Translate(Sight_Bead_Left_Start.normalized * stretch_val, base.transform);
		Sight_Bead_Right.transform.Translate(Sight_Bead_Right_Start.normalized * stretch_val, base.transform);
		if ((Sight_Bead_Up.transform.localPosition - Sight_Bead_Up_Start).sqrMagnitude >= stretch_range * stretch_range)
		{
			Sight_Bead_Up.transform.localPosition = Sight_Bead_Up_Start + Sight_Bead_Up_Start.normalized * stretch_range;
			Sight_Bead_Down.transform.localPosition = Sight_Bead_Down_Start + Sight_Bead_Down_Start.normalized * stretch_range;
			Sight_Bead_Left.transform.localPosition = Sight_Bead_Left_Start + Sight_Bead_Left_Start.normalized * stretch_range;
			Sight_Bead_Right.transform.localPosition = Sight_Bead_Right_Start + Sight_Bead_Right_Start.normalized * stretch_range;
		}
	}

	public Vector2 GetFireOffset()
	{
		float num = (Sight_Bead_Up.transform.localPosition - Sight_Bead_Up_Ori).magnitude * (float)Screen.height / 320f;
		return Random.insideUnitCircle * num;
	}

	public void SetSightOriPos(float offset)
	{
		Sight_Bead_Up_Ori = Sight_Bead_Up_Start + Sight_Bead_Up_Start.normalized * offset;
		Sight_Bead_Down_Ori = Sight_Bead_Down_Start + Sight_Bead_Down_Start.normalized * offset;
		Sight_Bead_Left_Ori = Sight_Bead_Left_Start + Sight_Bead_Left_Start.normalized * offset;
		Sight_Bead_Right_Ori = Sight_Bead_Right_Start + Sight_Bead_Right_Start.normalized * offset;
		sight_bead_state = SightBeadState.Recover;
	}

	public void SetLockColor()
	{
		Sight_Bead_Up.color = m_lock_color;
		Sight_Bead_Down.color = m_lock_color;
		Sight_Bead_Left.color = m_lock_color;
		Sight_Bead_Right.color = m_lock_color;
	}

	public void SetUnLockColor()
	{
		Sight_Bead_Up.color = m_unlock_color;
		Sight_Bead_Down.color = m_unlock_color;
		Sight_Bead_Left.color = m_unlock_color;
		Sight_Bead_Right.color = m_unlock_color;
	}

	public void ResetSightTexture(string texture_name, Quaternion rot)
	{
		if (Sight_Bead_Up == null)
		{
			Sight_Bead_Up = base.transform.Find("Sight_Bead_Up").GetComponent<TUIMeshSprite>();
			Sight_Bead_Down = base.transform.Find("Sight_Bead_Down").GetComponent<TUIMeshSprite>();
			Sight_Bead_Left = base.transform.Find("Sight_Bead_Left").GetComponent<TUIMeshSprite>();
			Sight_Bead_Right = base.transform.Find("Sight_Bead_Right").GetComponent<TUIMeshSprite>();
		}
		Sight_Bead_Up.texture = texture_name;
		Sight_Bead_Down.texture = texture_name;
		Sight_Bead_Left.texture = texture_name;
		Sight_Bead_Right.texture = texture_name;
		base.transform.localRotation = rot;
	}
}
