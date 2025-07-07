using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Vector3 axis;

	private void Update()
	{
		transform.Rotate(axis * Time.deltaTime);
	}
}
