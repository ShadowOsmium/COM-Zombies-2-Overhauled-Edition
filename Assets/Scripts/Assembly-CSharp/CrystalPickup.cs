using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            GameData.Instance.total_crystal.SetIntVal(
                GameData.Instance.total_crystal.GetIntVal() + 1,
                GameDataIntPurpose.Crystal
            );

            if (EndlessMissionController.Instance != null)
            {
                EndlessMissionController.Instance.crystalsCollected++;
            }

            Destroy(gameObject);
        }
    }
}