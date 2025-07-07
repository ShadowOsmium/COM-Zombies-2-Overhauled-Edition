using UnityEngine;

public class LevelUpEffControll : MonoBehaviour
{
	public GameObject mesh_eff1;

	public GameObject mesh_eff2;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnMeshEffEnable()
	{
		mesh_eff1.SetActive(true);
		mesh_eff2.SetActive(true);
	}

	private void OnMeshEffDisnable()
	{
		mesh_eff1.SetActive(false);
		mesh_eff2.SetActive(false);
	}
}
