using UnityEngine;

public class AdditionalGravity : MonoBehaviour
{
	public float force_val = 1f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		if (!base.GetComponent<Rigidbody>().isKinematic && !base.GetComponent<Rigidbody>().IsSleeping())
		{
			base.GetComponent<Rigidbody>().AddForce(Vector3.down * force_val, ForceMode.Force);
		}
	}
}
