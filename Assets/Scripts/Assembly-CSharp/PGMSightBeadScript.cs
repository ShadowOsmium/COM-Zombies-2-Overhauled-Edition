using UnityEngine;

public class PGMSightBeadScript : MonoBehaviour
{
	public enum SightBeadState
	{
		None,
		Recover
	}

	public Camera tui_camera;

	public Vector3 screen_pos = Vector3.zero;

	public Color m_unlock_color = Color.white;

	public Color m_lock_color = Color.white;

	private TUIMeshSprite Sight_Bead_Up;

	private TUIMeshSprite Sight_Bead_Down;

	private TUIMeshSprite Sight_Bead_Left;

	private TUIMeshSprite Sight_Bead_Right;

	private void Start()
	{
		if (tui_camera != null)
		{
			screen_pos = tui_camera.WorldToScreenPoint(base.transform.localPosition);
		}
		Sight_Bead_Up = base.transform.Find("Sight_Bead_Up").GetComponent<TUIMeshSprite>();
		Sight_Bead_Down = base.transform.Find("Sight_Bead_Down").GetComponent<TUIMeshSprite>();
		Sight_Bead_Left = base.transform.Find("Sight_Bead_Left").GetComponent<TUIMeshSprite>();
		Sight_Bead_Right = base.transform.Find("Sight_Bead_Right").GetComponent<TUIMeshSprite>();
		Sight_Bead_Up.color = m_lock_color;
		Sight_Bead_Down.color = m_lock_color;
		Sight_Bead_Left.color = m_lock_color;
		Sight_Bead_Right.color = m_lock_color;
	}

	private void Update()
	{
	}
}
