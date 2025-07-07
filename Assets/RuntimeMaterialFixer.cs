using UnityEngine;

public class RuntimeMaterialFixer : MonoBehaviour
{
    public GameObject levelUpRoot; // Assign in inspector, or it finds it

    void Start()
    {
        if (levelUpRoot == null)
        {
            levelUpRoot = GameObject.Find("level_up_n");
            if (levelUpRoot == null)
            {
                Debug.LogWarning("Fixer: Could not find 'level_up_n' in scene.");
                return;
            }
        }

        FixParticleMaterial(levelUpRoot, "level_up_n_01_03");
    }

    void FixParticleMaterial(GameObject root, string childName)
    {
        Transform child = FindChildByName(root.transform, childName);
        if (child == null)
        {
            Debug.LogWarning("Fixer: Could not find child '" + childName + "'");
            return;
        }

        ParticleSystemRenderer psRenderer = child.GetComponent<ParticleSystemRenderer>();
        if (psRenderer == null)
        {
            Debug.LogWarning("Fixer: No ParticleSystemRenderer on '" + childName + "'");
            return;
        }

        if (psRenderer.sharedMaterial == null)
        {
            Material fallback = FindMaterialByName(childName);
            if (fallback != null)
            {
                psRenderer.sharedMaterial = fallback;
                Debug.Log("Fixer: Assigned material '" + fallback.name + "' to particle system '" + childName + "'");
            }
            else
            {
                Debug.LogWarning("Fixer: No fallback material found for '" + childName + "'");
            }
        }
    }

    Transform FindChildByName(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
                return child;

            Transform found = FindChildByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    Material FindMaterialByName(string name)
    {
        // Assumes a material with the same name exists in Resources/Materials/
        return Resources.Load("Material/" + name, typeof(Material)) as Material;
    }
}