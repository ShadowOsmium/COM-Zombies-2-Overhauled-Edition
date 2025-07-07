using System.Collections.Generic;
using UnityEngine;

public class SinglePrefabReference : MonoBehaviour
{
    // This is the prefab or GameObject instance you want to instantiate at runtime
    public GameObject Instance;

    // List of accessory GameObjects that this prefab might have
    public List<GameObject> Accessory;
}