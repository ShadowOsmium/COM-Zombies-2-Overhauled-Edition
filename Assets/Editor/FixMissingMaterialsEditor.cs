#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FixMissingMaterialsEditor : MonoBehaviour
{
    [MenuItem("Tools/Fix Missing Materials in PrefabInstances")]
    public static void FixMaterials()
    {
        string prefabFolder = "Assets/PrefabInstance";
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { prefabFolder });

        for (int i = 0; i < prefabGUIDs.Length; i++)
        {
            string guid = prefabGUIDs[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (prefab == null) continue;

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null) continue;

            bool modified = false;
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);

            for (int r = 0; r < renderers.Length; r++)
            {
                Renderer renderer = renderers[r];
                Material[] materials = renderer.sharedMaterials;

                for (int m = 0; m < materials.Length; m++)
                {
                    if (materials[m] == null)
                    {
                        Material fallback = FindMaterialByName(renderer.name);
                        if (fallback != null)
                        {
                            Debug.Log("Assigning material '" + fallback.name + "' to '" + renderer.name + "' in '" + path + "'");
                            materials[m] = fallback;
                            modified = true;
                        }
                        else
                        {
                            Debug.LogWarning("Missing material on '" + renderer.name + "' in '" + path + "', no fallback found.");
                        }
                    }
                }

                if (modified)
                {
                    renderer.sharedMaterials = materials;
                }
            }

            if (modified)
            {
                // Apply changes to prefab
                PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
                Debug.Log("Updated prefab: " + path);
            }

            GameObject.DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Finished fixing missing materials.");
    }

    static Material FindMaterialByName(string name)
    {
        string[] matGUIDs = AssetDatabase.FindAssets("t:Material");

        for (int i = 0; i < matGUIDs.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(matGUIDs[i]);
            Material mat = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
            if (mat != null && mat.name == name)
            {
                return mat;
            }
        }

        // fallback (modify if needed)
        return AssetDatabase.LoadAssetAtPath("Assets/Materials/Default.mat", typeof(Material)) as Material;
    }
}
#endif