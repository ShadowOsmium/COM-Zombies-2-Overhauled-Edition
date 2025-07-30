using UnityEngine;
using System.IO;

public class TesterSaveManager : MonoBehaviour
{
    public static TesterSaveManager Instance { get; private set; }

    public bool allowTesterSaves = false;

    private const string regularKey = "CoMZ2";
    private const string testerKey = "CoMZ2_TESTER";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // avoid duplicates
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private string GetSaveFilePath(string key)
    {
        return Utils.SavePath() + MD5Sample.GetMd5String(key) + ".bytes";
    }

    public string CurrentSavePath
    {
        get
        {
            if (allowTesterSaves)
            {
                string testerPath = GetSaveFilePath(testerKey);
                if (File.Exists(testerPath))
                    return testerPath;

                // Tester saves allowed but none found — no fallback to regular save
                return null;
            }
            else
            {
                string regularPath = GetSaveFilePath(regularKey);
                return File.Exists(regularPath) ? regularPath : null;
            }
        }
    }

    public bool LoadSave(GameData gameData)
    {
        string path = CurrentSavePath;

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("[TesterSaveManager] No save file found.");
            return false;
        }

        Debug.Log("[TesterSaveManager] Loading save from: " + path);

        // Your LoadData method now is parameterless, so just call it directly on GameData instance
        bool loaded = gameData.LoadData();

        if (!loaded)
        {
            Debug.LogWarning("[TesterSaveManager] Failed to load save at " + path);
            return false;
        }

        if (!allowTesterSaves && gameData.isTesterSave)
        {
            Debug.LogWarning("[TesterSaveManager] Tester save was loaded in regular mode. Moving it back...");

            string realTesterPath = GetSaveFilePath(testerKey);
            string regularPath = GetSaveFilePath(regularKey);

            // Move the file back to tester path if it exists on regular path and tester path is missing
            if (path == regularPath && !File.Exists(realTesterPath))
            {
                File.Move(regularPath, realTesterPath);
            }

            // Delete the regular save if it still exists
            if (File.Exists(regularPath))
            {
                File.Delete(regularPath);
            }

            return false; // Prevent loading tester save in regular mode
        }

        return true;
    }

    public void ToggleTesterSaves(GameData gameData, bool enable)
    {
        allowTesterSaves = enable;

        if (enable)
            EnsureTesterSaveExists(gameData);

        // Always reload the save after toggling mode
        if (!LoadSave(gameData))
            Debug.LogWarning("[TesterSaveManager] Reload after toggling tester saves failed.");
    }

    public void EnsureTesterSaveExists(GameData gameData)
    {
        string testerSavePath = GetSaveFilePath(testerKey);

        if (!File.Exists(testerSavePath))
        {
            Debug.Log("[TesterSaveManager] Creating new tester save file...");
            // Save current game data into tester save with tester flag on
            gameData.SaveData();
        }
    }

    public void SaveGame(GameData gameData)
    {
        string savePath = allowTesterSaves ? GetSaveFilePath(testerKey) : GetSaveFilePath(regularKey);
        gameData.SaveData();
    }
}