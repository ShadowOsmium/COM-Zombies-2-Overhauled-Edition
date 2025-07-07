using UnityEngine;

public class onlyones : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			//Debug.Log("-------------quit game!!");
			DevicePlugin.AndroidQuit();
		}
	}
}
