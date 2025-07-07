using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ControllerNavPath : MonoBehaviour
{
	public Transform target;

	protected NavMeshAgent nav_agent;

	protected float repath_interval;

	public float drop_speed = 2f;

	public float jump_height = 2f;

	public bool just_rush_to;

	private IEnumerator Start()
	{
		nav_agent = GetComponent<NavMeshAgent>();
		nav_agent.updatePosition = false;
		nav_agent.updateRotation = false;
		nav_agent.autoTraverseOffMeshLink = false;
		while (Application.isPlaying)
		{
			yield return StartCoroutine(LocomotionJumpLogic());
		}
	}

    private void Update()
    {
        repath_interval += Time.deltaTime;
        if (repath_interval >= 0.25f)
        {
            if (IsTargetValid() && !just_rush_to && CanUseNavAgent())
            {
                nav_agent.SetDestination(target.position);
            }
            repath_interval = 0f;
        }
    }

    private IEnumerator LocomotionJumpLogic()
	{
		if (IsOnOffMeshLink() && nav_agent.updatePosition)
		{
			nav_agent.Stop();
			Vector3 linkEnd = nav_agent.currentOffMeshLinkData.endPos;
			Vector3 linkStart = nav_agent.currentOffMeshLinkData.startPos;
			Vector3 posStart = base.transform.position;
			float t = 0f;
			while (Mathf.Abs(base.transform.position.y - linkEnd.y) > 0.3f)
			{
				t += Time.deltaTime;
				Vector3 newPos = Vector3.Lerp(posStart, linkEnd, t);
				newPos.y += jump_height * Mathf.Sin(3.14159f * t);
				base.transform.position = newPos;
				yield return 1;
			}
			base.transform.position = linkEnd;
			nav_agent.CompleteOffMeshLink();
			nav_agent.Resume();
		}
	}

    public bool IsOnOffMeshLink()
    {
        if (IsTargetValid())
        {
            return nav_agent.isOnOffMeshLink;
        }
        return false;
    }

    private bool IsTargetValid()
    {
        return target != null && target.gameObject != null && target.gameObject.activeInHierarchy;
    }

    public void Catching(bool status)
	{
		nav_agent.updatePosition = status;
		nav_agent.updateRotation = status;
	}

	public bool GetCatchingState()
	{
		return nav_agent.updatePosition;
	}

    public void SetTarget(Transform trans)
    {
        target = trans;
        if (IsTargetValid() && CanUseNavAgent())
        {
            nav_agent.SetDestination(target.position);
        }
    }

    private bool CanUseNavAgent()
    {
        return nav_agent != null && nav_agent.enabled && nav_agent.isOnNavMesh;
    }

    public void SetTargetPosition(Vector3 pos)
    {
        if (CanUseNavAgent())
        {
            nav_agent.SetDestination(pos);
        }
    }

    public void SetSpeed(float speed)
	{
		nav_agent.speed = speed;
	}

	public float GetSpeed()
	{
		return nav_agent.speed;
	}

	public void StopNavMeshAgent()
	{
		nav_agent.enabled = false;
	}

	public void PlayNavMeshAgent()
	{
		nav_agent.enabled = true;
	}
}
