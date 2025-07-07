using UnityEngine;
using UnityEditor;
using System.IO;

public class BatchLegacyPrefabFixer : EditorWindow
{
    private static readonly string targetFolder = "Assets/PrefabInstance";
    private static readonly string backupFolderName = "Backup_OldPrefabs";
    private static readonly string tempSceneName = "TempPrefabFixScene";

    [MenuItem("Tools/Batch Legacy Prefab Fixer")]
    public static void ShowWindow()
    {
        GetWindow<BatchLegacyPrefabFixer>("Batch Legacy Prefab Fixer");
    }

    void OnGUI()
    {
        GUILayout.Label("Fix all legacy prefabs in PrefabInstance folder", EditorStyles.boldLabel);

        if (GUILayout.Button("Fix All Prefabs"))
        {
            if (!CheckScene())
            {
                if (EditorUtility.DisplayDialog("Warning", "It's recommended to use a temporary empty scene (TempPrefabFixScene) before running this fixer.", "Switch to Temp Scene", "Run Anyway"))
                {
                    if (!CreateOrLoadTempScene())
                        return;
                }
            }
            FixAllPrefabsInFolder(targetFolder);
        }
    }

    private static bool CheckScene()
    {
        return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name == tempSceneName;
    }

    private static bool CreateOrLoadTempScene()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene);
        return UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/" + tempSceneName + ".unity");
    }

    private static void FixAllPrefabsInFolder(string folderPath)
    {
        string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab", SearchOption.TopDirectoryOnly);
        string backupFolder = System.IO.Path.Combine(folderPath, backupFolderName).Replace("\\", "/");

        if (!Directory.Exists(backupFolder))
        {
            Directory.CreateDirectory(backupFolder);
            Debug.Log("Created backup folder: " + backupFolder);
        }

        foreach (string path in prefabPaths)
        {
            string assetPath = path.Replace("\\", "/");
            string prefabName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            string backupPath = System.IO.Path.Combine(backupFolder, prefabName + ".prefab").Replace("\\", "/");

            Debug.Log("\n<color=cyan>Processing prefab: " + prefabName + "</color>");

            // 1. Move original prefab to backup folder
            string moveError = AssetDatabase.MoveAsset(assetPath, backupPath);
            if (!string.IsNullOrEmpty(moveError))
            {
                Debug.LogError("Failed to move prefab to backup: " + moveError);
                continue;
            }
            AssetDatabase.Refresh();

            // 2. Create empty dummy prefab (white box) at original path
            GameObject ghost = new GameObject(prefabName);
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(ghost, assetPath);
#else
            PrefabUtility.CreatePrefab(assetPath, ghost);
#endif
            Object.DestroyImmediate(ghost);

            // 3. Instantiate dummy prefab into the scene hierarchy
            GameObject dummyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (dummyPrefab == null)
            {
                Debug.LogError("Failed to load dummy prefab.");
                continue;
            }

            GameObject dummyInstance = (GameObject)PrefabUtility.InstantiatePrefab(dummyPrefab);
            if (dummyInstance == null)
            {
                Debug.LogError("Failed to instantiate dummy prefab.");
                continue;
            }

            // 4. Instantiate original prefab from backup into the scene
            GameObject backupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(backupPath);
            if (backupPrefab == null)
            {
                Debug.LogError("Failed to load backup prefab.");
                Object.DestroyImmediate(dummyInstance);
                continue;
            }

            GameObject originalInstance = (GameObject)PrefabUtility.InstantiatePrefab(backupPrefab);
            if (originalInstance == null)
            {
                Debug.LogError("Failed to instantiate backup prefab.");
                Object.DestroyImmediate(dummyInstance);
                continue;
            }

            // 5. Replace dummy prefab asset with the instantiated original prefab
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(originalInstance, assetPath);
#else
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            PrefabUtility.ReplacePrefab(originalInstance, existingPrefab, ReplacePrefabOptions.Default);
#endif
            Debug.Log("<color=green>Fixed prefab: " + prefabName + "</color>");

            // 6. Clean up scene instances
            Object.DestroyImmediate(dummyInstance);
            Object.DestroyImmediate(originalInstance);
        }

        AssetDatabase.Refresh();
        Debug.Log("\n<color=lime>Batch legacy prefab fix complete.</color>");
    }
}
