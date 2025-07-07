using UnityEngine;
using CoMZ2;

public class PickupDropManager : MonoBehaviour
{
    private static PickupDropManager _instance;
    public static PickupDropManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PickupDropManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PickupDropManager");
                    _instance = go.AddComponent<PickupDropManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    public float hpDropChance = 0.5f;
    public float bulletDropChance = 0.75f;
    public float crystalDropChance = 1f;
    public float goldDropChance = 1.25f;

    private const float defaultCrystalDropChance = 1f;
    private const float defaultGoldDropChance = 1f;

    public float maxCrystalDropChance = 7.5f;
    public float maxGoldDropChance = 10f;

    public float crystalChanceIncreaseAmount = 0.5f;
    public float goldChanceIncreaseAmount = 1f;

    private float lastCrystalChanceIncreaseTime = 0f;
    private float lastGoldChanceIncreaseTime = 0f;

    public bool TrySpawnHpPickup(Vector3 position)
    {
        float dropChance = hpDropChance;

        float roll = Random.Range(0f, 100f);
        if (roll <= dropChance)
        {
            GameObject hpPrefab = GameSceneController.Instance.Eff_Accessory[26];
            if (hpPrefab != null)
            {
                GameObject hpObj = Instantiate(hpPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
                GameItemController itemCtrl = hpObj.GetComponent<GameItemController>();
                if (itemCtrl != null)
                    itemCtrl.item_type = ItemType.Hp;

                return true;
            }
        }
        return false;
    }

    public bool TrySpawnBulletPickup(Vector3 position)
    {
        float dropChance = bulletDropChance;

        float roll = Random.Range(0f, 100f);
        if (roll <= dropChance)
        {
            GameObject bulletPrefab = GameSceneController.Instance.Eff_Accessory[25];
            if (bulletPrefab != null)
            {
                GameObject bulletObj = Instantiate(bulletPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
                GameItemController itemCtrl = bulletObj.GetComponent<GameItemController>();
                if (itemCtrl != null)
                    itemCtrl.item_type = ItemType.Bullet_PrimaryWeapon;

                return true;
            }
        }
        return false;
    }

    public bool TrySpawnCrystal(Vector3 position)
    {
        bool isEndless = GameData.Instance.cur_quest_info != null &&
                         GameData.Instance.cur_quest_info.mission_type == MissionType.Endless &&
                         GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Endless;

        float dropChance = isEndless ? crystalDropChance : defaultCrystalDropChance;

        float roll = Random.Range(0f, 100f);
        if (roll <= dropChance)
        {
            GameObject crystalPrefab = GameSceneController.Instance.Eff_Accessory[23];
            GameObject crystalObj = Instantiate(crystalPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
            GameItemController itemCtrl = crystalObj.GetComponent<GameItemController>();
            if (itemCtrl != null)
                itemCtrl.item_type = ItemType.Crystal;
        }
        return false;
    }

    public bool TrySpawnGold(Vector3 position)
    {
        bool isEndless = GameData.Instance.cur_quest_info != null &&
                         GameData.Instance.cur_quest_info.mission_type == MissionType.Endless &&
                         GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Endless;

        float dropChance = isEndless ? goldDropChance : defaultGoldDropChance;

        float roll = Random.Range(0f, 100f);
        if (roll <= dropChance)
        {
            GameObject goldPrefab = GameSceneController.Instance.Eff_Accessory[24];
            if (goldPrefab != null)
            {
                GameObject goldObj = Instantiate(goldPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
                GameItemController itemCtrl = goldObj.GetComponent<GameItemController>();
                if (itemCtrl != null)
                {
                    itemCtrl.item_type = ItemType.Gold;
                    itemCtrl.item_val = 1500;
                }
                return true;
            }
        }
        return false;
    }

    public void IncreaseCrystalDropChance()
    {
        if (Time.time - lastCrystalChanceIncreaseTime >= 120f)
        {
            lastCrystalChanceIncreaseTime = Time.time;
            crystalDropChance += crystalChanceIncreaseAmount;
            if (crystalDropChance > maxCrystalDropChance)
                crystalDropChance = maxCrystalDropChance;

            Debug.Log("[DropManager] Crystal drop chance increased to " + crystalDropChance + "%");
        }
    }

    public void IncreaseGoldDropChance()
    {
        if (Time.time - lastGoldChanceIncreaseTime >= 45f)
        {
            lastGoldChanceIncreaseTime = Time.time;
            goldDropChance += goldChanceIncreaseAmount;
            if (goldDropChance > maxGoldDropChance)
                goldDropChance = maxGoldDropChance;

            Debug.Log("[DropManager] Gold drop chance increased to " + goldDropChance + "%");
        }
    }
}