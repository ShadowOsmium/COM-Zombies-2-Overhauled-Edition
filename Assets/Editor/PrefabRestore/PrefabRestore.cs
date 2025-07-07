using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabRestore : EditorWindow
{
    [MenuItem("Tools/Restore Prefabs From Backup")]
    public static void RestorePrefabs()
    {
        string selectedPath = EditorUtility.OpenFolderPanel("Select Folder with Backup_OldPrefabs", "Assets", "");

        if (string.IsNullOrEmpty(selectedPath))
        {
            Debug.LogWarning("No folder selected.");
            return;
        }

        // Convert absolute path to relative project path starting with "Assets"
        string relativePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);

        string backupFolder = System.IO.Path.Combine(relativePath, "Backup_OldPrefabs");
        if (!Directory.Exists(backupFolder))
        {
            Debug.LogError("No 'Backup_OldPrefabs' folder found in selected directory.");
            return;
        }

        string[] backupPrefabs = Directory.GetFiles(backupFolder, "*.prefab", SearchOption.TopDirectoryOnly);
        foreach (string backupFullPath in backupPrefabs)
        {
            string fileName = System.IO.Path.GetFileName(backupFullPath);
            string backupAssetPath = backupFullPath.Replace("\\", "/");
            string originalPrefabPath = System.IO.Path.Combine(relativePath, fileName).Replace("\\", "/");

            GameObject backupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(backupAssetPath);
            GameObject originalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(originalPrefabPath);

            if (backupPrefab == null)
            {
                Debug.LogError("Backup prefab not found or invalid: " + backupAssetPath);
                continue;
            }

            if (originalPrefab == null)
            {
                Debug.LogWarning("Original prefab not found (empty prefab missing?): " + originalPrefabPath);
                // You can optionally move the backup prefab back here or skip
                continue;
            }

            // Overwrite original prefab contents with backup prefab
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAssetAndConnect(backupPrefab, originalPrefabPath, InteractionMode.AutomatedAction);
#else
            PrefabUtility.ReplacePrefab(backupPrefab, originalPrefab, ReplacePrefabOptions.Default);
#endif
            Debug.Log("Restored contents to prefab: " + originalPrefabPath);

            bool deleted = AssetDatabase.DeleteAsset(backupAssetPath);
            if (!deleted)
            {
                Debug.LogWarning("Failed to delete backup prefab: " + backupAssetPath);
            }
            else
            {
                Debug.Log("Deleted backup prefab: " + backupAssetPath);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Prefab restoration completed.");
    }
}