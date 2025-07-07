using UnityEngine;

public class StaticObjUI : MonoBehaviour
{
	protected string log_text = string.Empty;

	protected int log_line_count;

	protected float last_time;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnEnable()
	{
		Application.RegisterLogCallback(HandleLog);
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	private void HandleLog(string condition, string stackTrace, LogType type)
	{
		log_line_count++;
		if (log_line_count > 10)
		{
			log_line_count = 0;
			log_text = string.Empty;
		}
		log_text = log_text + "\n" + condition;
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 480f, 320f), log_text);
	}
}
