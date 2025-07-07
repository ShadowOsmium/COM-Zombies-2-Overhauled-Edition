using UnityEngine;

public class LevelUp3 : MonoBehaviour
{
	public ParticleSystem particle1;

	public ParticleSystem particle2;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnAnimationEvent()
	{
		particle1.GetComponent<Renderer>().enabled = false;
		particle2.GetComponent<Renderer>().enabled = false;
	}

	private void OnEnable()
	{
		particle1.GetComponent<Renderer>().enabled = true;
		particle2.GetComponent<Renderer>().enabled = true;
	}
}
