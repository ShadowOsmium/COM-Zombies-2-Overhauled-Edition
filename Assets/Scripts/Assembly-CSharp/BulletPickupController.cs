using UnityEngine;

public class BulletPickupController : MonoBehaviour
{
    private GameItemController itemController;

    private void Awake()
    {
        itemController = GetComponent<GameItemController>();
        if (itemController == null)
        {
            Debug.LogError("GameItemController missing on BulletPickup prefab.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && itemController != null)
        {
            player.OnGetItem(itemController);
            Destroy(gameObject);
        }
    }
}
