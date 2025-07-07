using UnityEngine;

public class IndicatorBlockController : MonoBehaviour
{
	protected static IndicatorBlockController instance;

	public TUILabel content;

	public static IndicatorBlockController Instance
	{
		get
		{
			return instance;
		}
		set
		{
			instance = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public void SetContent(string str)
	{
		content.Text = str;
	}

	public static void DestroyIndicatorBlock()
	{
		if (Instance != null)
		{
			Object.Destroy(Instance.gameObject);
		}
	}

	public static IndicatorBlockController ShowIndicator(GameObject root_obj, string str)
	{
		if (Instance == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("TUI/IndicatorBlock")) as GameObject;
			Instance = gameObject.GetComponent<IndicatorBlockController>();
			gameObject.transform.parent = root_obj.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, -80f);
			gameObject.transform.localRotation = Quaternion.identity;
		}
		Instance.gameObject.SetActive(true);
		Instance.SetContent(str);
		return Instance;
	}

	public static void Hide()
	{
		if (Instance != null)
		{
			Instance.gameObject.SetActive(false);
		}
	}
}
