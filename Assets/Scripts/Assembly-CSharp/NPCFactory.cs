using UnityEngine;

public class NPCFactory
{
	public static NPCController CreateNPC(NpcType type, Vector3 pos, Quaternion rot, AvatarType avatar_type = AvatarType.None)
	{
		GameObject gameObject = null;
		gameObject = ((avatar_type == AvatarType.None) ? (Object.Instantiate(Resources.Load("Prefabs/NPC/NPC")) as GameObject) : (Object.Instantiate(Resources.Load("Prefabs/NPC/NPC" + GameConfig.Instance.AvatarConfig_Set[avatar_type].avatar_name)) as GameObject));
		GameObject gameObject2 = Object.Instantiate(gameObject.GetComponent<SinglePrefabReference>().Instance, pos, rot) as GameObject;
		Object.Destroy(gameObject);
		NPCController nPCController = Utility.AddNPCComponent(gameObject2, GetNpcTypeControllerName(type));
		NPCData npcData = NPCData.CreateData(GameConfig.Instance.NpcConfig_Set[type]);
		nPCController.SetNpcData(npcData);
		nPCController.npc_id = GameSceneController.Instance.NPCIndex;
		gameObject2.name = "NPC_" + nPCController.npc_id;
		GameSceneController.Instance.NPC_Set.Add(nPCController.npc_id, nPCController);
		return nPCController;
	}

	public static string GetNpcTypeControllerName(NpcType type)
	{
		string result = "NPCController";
		switch (type)
		{
		case NpcType.N_FOLLOWER:
			result = "FollowerController";
			break;
		case NpcType.N_WORKER:
			result = "WorkerController";
			break;
		}
		return result;
	}
}
