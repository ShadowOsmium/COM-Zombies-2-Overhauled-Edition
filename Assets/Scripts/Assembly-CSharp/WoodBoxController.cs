using System.Collections;
using System.Collections.Generic;
using TNetSdk;
using UnityEngine;

public class WoodBoxController : ObjectController
{
	public float cur_hp = 20f;

	public List<GameObject> Accessory = new List<GameObject>();

	public int[] RateTables;

	private bool is_broken;

	public GameObject box_break_eff;

	private Transform item_pos;

	public int coop_id = -1;

	private TNetObject tnetObj;

	public bool Broken
	{
		get
		{
			return is_broken;
		}
	}

	protected override void Start()
	{
		item_pos = base.transform.Find("Item_pos");
		if (item_pos == null)
		{
			Debug.LogError("woodbox item_pos can not find.");
		}
		cur_hp = 1f;
		StartCoroutine(SetCoopId());
	}

	private IEnumerator SetCoopId()
	{
		yield return 1;
		if (GameData.Instance.cur_game_type != GameData.GamePlayType.Coop)
		{
			yield break;
		}
		tnetObj = TNetConnection.Connection;
		if (TNetConnection.IsServer)
		{
			while (GameSceneCoopController.Instance == null || !GameSceneCoopController.Instance.Inited)
			{
				yield return 1;
			}
			coop_id = GameSceneCoopController.Instance.WoodBoxIndex;
			if (tnetObj != null)
			{
				SFSObject data = new SFSObject();
				SFSArray data_array = new SFSArray();
				data_array.AddShort((short)coop_id);
				data_array.AddFloat(base.transform.position.x);
				data_array.AddFloat(base.transform.position.y);
				data_array.AddFloat(base.transform.position.z);
				data.PutSFSArray("WoodBoxId", data_array);
				tnetObj.Send(new BroadcastMessageRequest(data));
				Debug.Log("Send WoodBoxId msg.");
			}
			GameSceneCoopController.Instance.woodbox_set.Add(coop_id, this);
		}
	}

	protected override void Update()
	{
	}

    public override void OnHit(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
    {
        string weaponName = weapon != null ? weapon.name : "unknown weapon";
        string controllerName = controller != null ? controller.name : "unknown controller";

        Debug.Log(string.Format("WoodBoxController.OnHit called with damage={0} by {1} from {2}", damage, weaponName, controllerName));

        if (!is_broken)
        {
            cur_hp -= damage;
            if (cur_hp <= 0f)
            {
                is_broken = true;
                Debug.Log("WoodBoxController broken! Calling OnDead.");
                Debug.Log("OnDead called\n" + new System.Diagnostics.StackTrace().ToString());
                OnDead(damage, weapon, controller, hit_point, hit_normal);
            }
        }
    }

    public override void OnDead(float damage, WeaponController weapon, ObjectController controller, Vector3 hit_point, Vector3 hit_normal)
	{
		GameSceneController.Instance.CreateAudioOncePlayer("BrokenWood01", 1f, base.transform.position);
		StartCoroutine(OnSpawnItem());
		if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && tnetObj != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutShort("WoodBoxDestory", (short)coop_id);
			tnetObj.Send(new BroadcastMessageRequest(sFSObject));
			Debug.Log("Send WoodBoxDestory: + coop_id");
		}
	}

	public void OnRemoteDead()
	{
		is_broken = true;
		cur_hp = 0f;
		GameSceneController.Instance.CreateAudioOncePlayer("BrokenWood01", 1f, base.transform.position);
		GameSceneCoopController.Instance.woodbox_set.Remove(coop_id);
		StartCoroutine(OnSpawnItem());
	}

    private IEnumerator OnSpawnItem()
    {
        yield return 1;
        GameSceneController.Instance.wood_box_list.Remove(base.gameObject);

        List<GameObject> filteredAccessories = new List<GameObject>();
        List<int> filteredRates = new List<int>();

        for (int i = 0; i < Accessory.Count; i++)
        {
            GameObject obj = Accessory[i];
            if (obj.name != "Ammo_Packet" && obj.name != "Ammo_Packet(Clone)")
            {
                filteredAccessories.Add(obj);
                if (i < RateTables.Length)
                {
                    filteredRates.Add(RateTables[i]);
                }
                else
                {
                    filteredRates.Add(0);
                }
            }
        }

        Debug.Log("Filtered spawn table:");
        for (int i = 0; i < filteredAccessories.Count; i++)
        {
            Debug.Log(string.Format("  {0} (Rate: {1})", filteredAccessories[i].name, filteredRates[i]));
        }

        int total_val = 0;
        for (int i = 0; i < filteredRates.Count; i++)
        {
            total_val += filteredRates[i];
        }

        int rate = Random.Range(0, total_val);
        int tem_total_val = 0;
        int selectedIndex = -1;

        for (int i = 0; i < filteredRates.Count; i++)
        {
            tem_total_val += filteredRates[i];
            if (rate < tem_total_val)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex >= 0 && selectedIndex < filteredAccessories.Count)
        {
            GameObject prefabToSpawn = filteredAccessories[selectedIndex];
            Debug.Log("Selected to spawn: " + prefabToSpawn.name);

            Vector3 offset = Vector3.up * 0.5f;
            GameObject spawned = Object.Instantiate(prefabToSpawn, base.transform.position + offset, Quaternion.identity);

            Debug.Log("Actually spawned: " + spawned.name);
        }
        else
        {
            Debug.Log("Spawn game item Error, selectedIndex: " + selectedIndex);
        }

        GameObject box_break_obj = Object.Instantiate(box_break_eff) as GameObject;
        box_break_obj.transform.position = base.gameObject.transform.position;
        box_break_obj.transform.rotation = Quaternion.Euler(new Vector3(0f, base.gameObject.transform.rotation.eulerAngles.y, 0f));
        box_break_obj.AddComponent<RemoveTimerScript>().life = 3f;
        Object.Destroy(base.gameObject);
    }
}
