using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            GameData.Instance.total_cash.SetIntVal(
                GameData.Instance.total_cash.GetIntVal() + 2500,
                GameDataIntPurpose.Cash
            );

            if (EndlessMissionController.Instance != null)
            {
                EndlessMissionController.Instance.goldCollected++;
            }

            Destroy(gameObject);
        }
    }
}