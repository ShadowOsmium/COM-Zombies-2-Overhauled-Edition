using UnityEngine;

public class SceneAwake : MonoBehaviour
{
	public GameObject scene_base;

	public GameObject scene_coop_base;

	private void Awake()
	{
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		GameObject gameObject = null;
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Normal)
		{
			gameObject = Object.Instantiate(scene_base, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		{
			gameObject = Object.Instantiate(scene_coop_base, Vector3.zero, Quaternion.identity) as GameObject;
		}
		GameObject gameObject2 = gameObject.transform.Find("TUI").gameObject;
		gameObject2.transform.parent = null;
		Object.Destroy(base.gameObject);
	}
}
