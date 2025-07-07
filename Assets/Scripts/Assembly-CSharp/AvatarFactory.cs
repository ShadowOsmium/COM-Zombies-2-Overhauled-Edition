using UnityEngine;

public class AvatarFactory
{
	public static void CreateAvatar(AvatarType type)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player_Spawn");
		GameObject gameObject = array[0];
		GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/Avatar/" + GameData.Instance.AvatarData_Set[type].avatar_name + "_" + (int)GameData.Instance.AvatarData_Set[type].avatar_state)) as GameObject;
		GameObject gameObject3 = Object.Instantiate(gameObject2.GetComponent<SinglePrefabReference>().Instance, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
		Object.Destroy(gameObject2);
		gameObject3.GetComponent<PlayerController>().SetAvatarData(GameData.Instance.AvatarData_Set[type]);
		GameSceneController.Instance.main_camera.SetAngleH(gameObject.transform.rotation.eulerAngles.y);
	}

	public static PlayerCoopController CreateAvatarCoop(AvatarData avatar_data)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player_Spawn");
		GameObject gameObject = array[0];
		GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/AvatarCoop/" + avatar_data.avatar_name + "_" + (int)avatar_data.avatar_state)) as GameObject;
		GameObject gameObject3 = Object.Instantiate(gameObject2.GetComponent<SinglePrefabReference>().Instance, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
		Object.Destroy(gameObject2);
		PlayerCoopController component = gameObject3.GetComponent<PlayerCoopController>();
		component.SetAvatarData(avatar_data);
		return component;
	}
}
