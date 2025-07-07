using UnityEngine;

public abstract class CharacterMotor : MonoBehaviour
{
	public float maxForwardSpeed = 1.5f;

	public float maxBackwardsSpeed = 1.5f;

	public float maxSidewaysSpeed = 1.5f;

	public float maxVelocityChange = 0.2f;

	public float gravity = 10f;

	public bool canJump = true;

	public float jumpHeight = 1f;

	public Vector3 forwardVector = Vector3.forward;

	protected Quaternion alignCorrection;

	protected bool m_Grounded;

	protected bool m_Jumping;

	protected Vector3 m_desiredMovementDirection;

	protected Vector3 m_desiredFacingDirection;

	public bool grounded
	{
		get
		{
			return m_Grounded;
		}
		protected set
		{
			m_Grounded = value;
		}
	}

	public bool jumping
	{
		get
		{
			return m_Jumping;
		}
		protected set
		{
			m_Jumping = value;
		}
	}

	public Vector3 desiredMovementDirection
	{
		get
		{
			return m_desiredMovementDirection;
		}
		set
		{
			m_desiredMovementDirection = value;
			if (m_desiredMovementDirection.magnitude > 1f)
			{
				m_desiredMovementDirection = m_desiredMovementDirection.normalized;
			}
		}
	}

	public Vector3 desiredFacingDirection
	{
		get
		{
			return m_desiredFacingDirection;
		}
		set
		{
			m_desiredFacingDirection = value;
			if (m_desiredFacingDirection.magnitude > 1f)
			{
				m_desiredFacingDirection = m_desiredFacingDirection.normalized;
			}
		}
	}

	public virtual Vector3 desiredVelocity
	{
		get
		{
			if (m_desiredMovementDirection == Vector3.zero)
			{
				return Vector3.zero;
			}
			float num = ((!(m_desiredMovementDirection.z > 0f)) ? maxBackwardsSpeed : maxForwardSpeed) / maxSidewaysSpeed;
			Vector3 normalized = new Vector3(m_desiredMovementDirection.x, 0f, m_desiredMovementDirection.z / num).normalized;
			float num2 = new Vector3(normalized.x, 0f, normalized.z * num).magnitude * maxSidewaysSpeed;
			Vector3 vector = m_desiredMovementDirection * num2;
			return base.transform.rotation * vector;
		}
	}

	private void Start()
	{
		alignCorrection = default(Quaternion);
		alignCorrection.SetLookRotation(forwardVector, Vector3.up);
		alignCorrection = Quaternion.Inverse(alignCorrection);
	}
}
