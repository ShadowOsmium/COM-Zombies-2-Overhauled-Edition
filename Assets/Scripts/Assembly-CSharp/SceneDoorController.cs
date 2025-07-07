using UnityEngine;

public class SceneDoorController : MonoBehaviour
{
	public enum DoorState
	{
		None,
		Rise,
		Drop
	}

	public float door_speed = 3f;

	public GameObject trap_door;

	protected DoorState door_state;

	public EnemyRailController enemy_rail;

	private void Start()
	{
		if (trap_door == null)
		{
			trap_door = base.transform.Find("railing").gameObject;
		}
	}

	private void Update()
	{
		if (door_state == DoorState.Rise)
		{
			trap_door.transform.localPosition += Vector3.up * Time.deltaTime * door_speed;
			if (trap_door.transform.localPosition.y >= 4f)
			{
				trap_door.transform.localPosition = new Vector3(0f, 4f, 0f);
				door_state = DoorState.None;
			}
		}
		else if (door_state == DoorState.Drop)
		{
			trap_door.transform.localPosition -= Vector3.up * Time.deltaTime * door_speed;
			if (trap_door.transform.localPosition.y <= 0f)
			{
				trap_door.transform.localPosition = new Vector3(0f, 0f, 0f);
				door_state = DoorState.None;
			}
		}
	}

	public virtual void RiseDoor()
	{
		door_state = DoorState.Rise;
		if (enemy_rail != null)
		{
			enemy_rail.ReleaseRailEnemys();
		}
	}

	public virtual void DropDoor()
	{
		door_state = DoorState.Drop;
		if (enemy_rail != null)
		{
			enemy_rail.StartRail();
		}
	}
}
