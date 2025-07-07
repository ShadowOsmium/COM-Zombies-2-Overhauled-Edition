using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleCharacterMotor : CharacterMotor
{
	private bool firstframe = true;

	private CharacterController m_controller;

	private float tem_angle;

	private float tem_speed;

	private float tem_a;

	private float tem_b;

	public override Vector3 desiredVelocity
	{
		get
		{
			if (m_desiredMovementDirection == Vector3.zero)
			{
				return Vector3.zero;
			}
			return m_desiredMovementDirection * UpdateSpeedVal();
		}
	}

	private void Start()
	{
		m_controller = GetComponent(typeof(CharacterController)) as CharacterController;
	}

	private void UpdateVelocity()
	{
		Vector3 vector = m_controller.velocity;
		if (firstframe)
		{
			vector = Vector3.zero;
			firstframe = false;
		}
		if (base.grounded)
		{
			vector = Util.ProjectOntoPlane(vector, base.transform.up);
		}
		Vector3 vector2 = vector;
		base.jumping = false;
		if (base.grounded)
		{
			Vector3 vector3 = desiredVelocity - vector;
			if (vector3.magnitude > maxVelocityChange)
			{
				vector3 = vector3.normalized * maxVelocityChange;
			}
			vector2 += vector3;
			if (canJump && Input.GetButton("Jump"))
			{
				vector2 += base.transform.up * Mathf.Sqrt(2f * jumpHeight * gravity);
				base.jumping = true;
			}
		}
		vector2 += base.transform.up * (0f - gravity) * Time.deltaTime;
		if (base.jumping)
		{
			vector2 -= base.transform.up * (0f - gravity) * Time.deltaTime / 2f;
		}
		CollisionFlags collisionFlags = m_controller.Move(vector2 * Time.deltaTime);
		base.grounded = (collisionFlags & CollisionFlags.Below) != 0;
	}

	public void DoLogic()
	{
		if (Time.deltaTime != 0f && Time.timeScale != 0f)
		{
			UpdateVelocity();
		}
	}

	public float UpdateSpeedVal()
	{
		tem_angle = Vector3.Angle(base.transform.forward, m_desiredMovementDirection);
		if (tem_angle <= 90f)
		{
			tem_a = (maxSidewaysSpeed - maxForwardSpeed) / 90f;
			tem_b = maxForwardSpeed;
		}
		else
		{
			tem_a = (maxBackwardsSpeed - maxSidewaysSpeed) / 90f;
			tem_b = 2f * maxSidewaysSpeed - maxBackwardsSpeed;
		}
		tem_speed = tem_a * tem_angle + tem_b;
		return tem_speed;
	}
}
