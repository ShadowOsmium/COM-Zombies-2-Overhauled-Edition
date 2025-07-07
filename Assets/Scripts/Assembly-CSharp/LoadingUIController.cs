using System.Collections;
using UnityEngine;

public class LoadingUIController : MonoBehaviour
{
	private static LoadingUIController instance;

	public GameObject scene_tui_obj;

	public TUILabel enemy_content;

	public TUIMeshSprite enemy_img;

	public bool loading_finished;

	public static LoadingUIController Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Object.DontDestroyOnLoad(scene_tui_obj);
		instance = this;
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		ResetLoadingEnemyModel();
	}

	private IEnumerator Start()
	{
		while (!GameConfig.Instance.Load_finished)
		{
			yield return 1;
		}
		yield return 3;
		Application.LoadLevel(GameData.Instance.loading_to_scene);
		yield return 1;
		while (!loading_finished)
		{
			yield return 1;
		}
		Object.Destroy(scene_tui_obj);
		yield return 1;
		yield return 1;
		Object.Destroy(base.gameObject);
		yield return 1;
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public static void FinishedLoading()
	{
		if (Instance != null)
		{
			Instance.loading_finished = true;
		}
	}

	public void ResetLoadingEnemyModel()
	{
		string text = string.Empty;
		foreach (string key in GameData.Instance.Enemy_Loading_Set.Keys)
		{
			if (GameData.Instance.Enemy_Loading_Set[key] == 0)
			{
				text = key;
				break;
			}
		}
		if (text == string.Empty)
		{
			int num = Random.Range(0, GameData.Instance.Enemy_Loading_Set.Keys.Count);
			int num2 = 0;
			foreach (string key2 in GameData.Instance.Enemy_Loading_Set.Keys)
			{
				if (num2 == num)
				{
					text = key2;
					break;
				}
				num2++;
			}
		}
		enemy_img.CustomizeTexture = Resources.Load("TUI/Textures/Loading_" + text) as Texture2D;
		enemy_img.CustomizeRect = new Rect(0f, 0f, 263f, 401f);
		enemy_content.Text = GameConfig.Instance.EnemyConfig_Set[GameConfig.Instance.GetEnemyTypeFromCfg(text)].load_content;
	}
}
