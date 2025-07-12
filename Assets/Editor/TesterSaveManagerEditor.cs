using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TesterSaveManager))]
public class TesterSaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TesterSaveManager manager = (TesterSaveManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button(manager.allowTesterSaves ? "Disable Tester Saves" : "Enable Tester Saves"))
        {
            manager.allowTesterSaves = !manager.allowTesterSaves;
            EditorUtility.SetDirty(manager);
            Debug.Log("Tester Saves toggled to: " + manager.allowTesterSaves);

            if (GameData.Instance != null)
            {
                bool success = manager.LoadSave(GameData.Instance);
                Debug.Log("Reloaded save after toggling tester saves: " + success);
            }
        }
    }
}
