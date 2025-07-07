using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabFixer : EditorWindow
{
    [MenuItem("Tools/Prefab Fixer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PrefabFixer), false, "Prefab Fixer");
    }

    private Object prefabFolderObject;

    void OnGUI()
    {
        GUILayout.Label("Select the folder containing broken prefabs", EditorStyles.boldLabel);

        prefabFolderObject = EditorGUILayout.ObjectField("Prefab Folder", prefabFolderObject, typeof(DefaultAsset), false);

        if (prefabFolderObject != null)
        {
            if (GUILayout.Button("Fix Prefabs"))
            {
                string folderPath = AssetDatabase.GetAssetPath(prefabFolderObject);

                if (Directory.Exists(folderPath))
                {
                    FixPrefabsInFolder(folderPath);
                }
                else
                {
                    Debug.LogError("Selected path is not a folder.");
                }
            }
        }
        else
        {
            GUILayout.Label("Please select a folder from your Project window.");
        }
    }

    private void FixPrefabsInFolder(string folderPath)
    {
        string backupFolder = folderPath + "/Backup_OldPrefabs";
        if (!Directory.Exists(backupFolder))
        {
            Directory.CreateDirectory(backupFolder);
            Debug.Log("Created backup folder: " + backupFolder);
        }

        string[] prefabFiles = Directory.GetFiles(folderPath, "*.prefab", SearchOption.TopDirectoryOnly);

        foreach (string prefabFilePath in prefabFiles)
        {
            string prefabName = System.IO.Path.GetFileNameWithoutExtension(prefabFilePath);
            string assetPath = prefabFilePath.Replace("\\", "/");

            Debug.Log("Processing prefab: " + assetPath);

            string backupPath = backupFolder + "/" + prefabName + ".prefab";
            string error = AssetDatabase.MoveAsset(assetPath, backupPath);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("Failed to move prefab: " + error);
                continue;
            }

            Debug.Log("Backed up prefab to: " + backupPath);

            GameObject restoredPrefab = AssetDatabase.LoadAssetAtPath(backupPath, typeof(GameObject)) as GameObject;

            if (restoredPrefab != null)
            {
                // Instantiate the backup prefab into the scene
                GameObject instantiatedCopy = PrefabUtility.InstantiatePrefab(restoredPrefab) as GameObject;
                if (instantiatedCopy != null)
                {
                    // Save the instantiated object back to the original prefab location
                    string newPrefabPath = folderPath + "/" + prefabName + ".prefab";
                    PrefabUtility.CreatePrefab(newPrefabPath, instantiatedCopy);
                    Debug.Log("Restored prefab from backup: " + newPrefabPath);

                    DestroyImmediate(instantiatedCopy);
                    continue;
                }
            }

            // Fallback: create an empty placeholder if backup failed to load or instantiate
            Debug.LogWarning("Could not instantiate backup for prefab: " + prefabName + ", creating empty placeholder.");

            GameObject emptyGO = new GameObject(prefabName);
            string fallbackPrefabPath = folderPath + "/" + prefabName + ".prefab";
            PrefabUtility.CreatePrefab(fallbackPrefabPath, emptyGO);
            DestroyImmediate(emptyGO);
        }

        AssetDatabase.Refresh();
        Debug.Log("Prefab fixing completed.");
    }
}