using CoMZ2;
using TNetSdk;
using UnityEngine;

public class GameItemController : MonoBehaviour
{
	public ItemType item_type = ItemType.None;

	public int item_val = 100;

	public int coop_id = -1;

	private TNetObject tnetObj;

	private void Start()
	{
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
		{
			tnetObj = TNetConnection.Connection;
		}
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && TNetConnection.IsServer)
		{
			coop_id = GameSceneCoopController.Instance.GameItemIndex;
			if (tnetObj != null)
			{
				SFSObject sFSObject = new SFSObject();
				SFSArray sFSArray = new SFSArray();
				sFSArray.AddShort((short)coop_id);
				sFSArray.AddShort((short)item_type);
				sFSArray.AddFloat(base.transform.position.x);
				sFSArray.AddFloat(base.transform.position.y);
				sFSArray.AddFloat(base.transform.position.z);
				sFSObject.PutSFSArray("GameItemId", sFSArray);
				tnetObj.Send(new BroadcastMessageRequest(sFSObject));
				Debug.Log("Send GameItemId msg.");
			}
			GameSceneCoopController.Instance.game_item_set.Add(coop_id, this);
		}
	}

	private void Update()
	{
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().gameObject.layer != PhysicsLayer.PLAYER)
        {
            return;
        }
        PlayerController component = other.GetComponent<Collider>().gameObject.GetComponent<PlayerController>();
        if (!(component != null) || !component.IsMyself)
        {
            return;
        }
        component.OnGetItem(this);
        GameSceneController.Instance.add_item_pool.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
        if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop)
        {
            if (tnetObj != null)
            {
                SFSObject sFSObject = new SFSObject();
                SFSArray sFSArray = new SFSArray();
                sFSArray.AddShort((short)coop_id);
                sFSArray.AddShort((short)item_type);
                sFSObject.PutSFSArray("ItemGet", sFSArray);
                tnetObj.Send(new BroadcastMessageRequest(sFSObject));
                Debug.Log("Send ItemGet msg.");
            }
            GameSceneCoopController.Instance.game_item_set.Remove(coop_id);
        }
        Object.Destroy(base.gameObject);
        GuideController.TriggerGuide("Tutorial Over");
        if (item_type == ItemType.Gold)
        {
            /*GameData.Instance.total_cash.SetIntVal(
                GameData.Instance.total_cash.GetIntVal() + 2500,
                GameDataIntPurpose.Cash
            );*/

            if (GameData.Instance.cur_quest_info != null &&
                GameData.Instance.cur_quest_info.mission_type == MissionType.Endless &&
                EndlessMissionController.Instance != null)
            {
                EndlessMissionController.Instance.goldCollected += 1;
            }
        }
        if (item_type == ItemType.Crystal)
        {
            GameData.Instance.total_crystal.SetIntVal(
                GameData.Instance.total_crystal.GetIntVal() + 1,
                GameDataIntPurpose.Crystal
            );

            if (GameData.Instance.cur_quest_info != null &&
                GameData.Instance.cur_quest_info.mission_type == MissionType.Endless &&
                EndlessMissionController.Instance != null)
            {
                EndlessMissionController.Instance.crystalsCollected += 1;
            }
        }
    }

	public void OnRemoteGet(PlayerCoopController player)
	{
		if (!(player == null))
		{
			player.OnRemoteGetItem(item_type);
			GameSceneController.Instance.add_item_pool.GetComponent<ObjectPool>().CreateObject(base.transform.position, Quaternion.identity);
			GameSceneCoopController.Instance.game_item_set.Remove(coop_id);
			Object.Destroy(base.gameObject);
		}
	}
}
