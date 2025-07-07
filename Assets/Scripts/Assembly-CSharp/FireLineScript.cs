using UnityEngine;

public class FireLineScript : MonoBehaviour
{
	public float speed;

	protected Vector3 begin_pos;

	protected Vector3 end_pos;

	protected Vector3 direct;

	protected float startTime;

	protected float life_time;

	protected bool inited;

	public bool is_destroy;

	private void Start()
	{
	}

	private void Update()
	{
		if (!inited)
		{
			return;
		}
		if (Time.time - startTime >= life_time)
		{
			if (is_destroy)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				GetComponent<FireLineItem>().OnItemDeactive();
			}
		}
		base.transform.Translate(speed * direct * Time.deltaTime, Space.World);
	}

	public void Init(Vector3 beginPos, Vector3 endPos)
	{
		begin_pos = beginPos;
		end_pos = endPos;
		direct = (end_pos - begin_pos).normalized;
		base.transform.position = begin_pos;
		startTime = Time.time;
		life_time = (end_pos - begin_pos).magnitude / speed;
		inited = true;
	}
}
