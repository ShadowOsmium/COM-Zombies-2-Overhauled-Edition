using UnityEngine;

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
            Destroy(gameObject); // prevent duplicates
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private string GetSaveFilePath(string key)
    {
        return Utils.SavePath() + MD5Sample.GetMd5String(key) + ".bytes";
//      return Utils.SavePath() + SHA256Sample.GetSha256String(key).Substring(0, 32) + ".bytes";
    }

    public string CurrentSavePath
    {
        get
        {
            if (allowTesterSaves)
            {
                string testerPath = GetSaveFilePath(testerKey);
                if (System.IO.File.Exists(testerPath))
                    return testerPath;

                // Tester saves allowed but tester save missing → no fallback!
                return null;
            }
            else
            {
                string regularPath = GetSaveFilePath(regularKey);
                if (System.IO.File.Exists(regularPath))
                    return regularPath;

                return null; // no save found
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

        bool loaded = gameData.LoadData(path);

        if (!loaded)
        {
            Debug.LogWarning("[TesterSaveManager] Failed to load save at " + path);
            return false;
        }

        if (!allowTesterSaves && gameData.isTesterSave)
        {
            Debug.LogWarning("[TesterSaveManager] Tester save was loaded via renamed file. Moving it back to tester path...");

            string realTesterPath = GetSaveFilePath(testerKey);
            string regularPath = GetSaveFilePath(regularKey);

            if (path == regularPath && !System.IO.File.Exists(realTesterPath))
            {
                System.IO.File.Move(regularPath, realTesterPath);
            }

            if (System.IO.File.Exists(regularPath))
            {
                System.IO.File.Delete(regularPath);
            }

            return false;
        }

        return true;
    }

    public void ToggleTesterSaves(GameData gameData, bool enable)
    {
        allowTesterSaves = enable;

        if (enable)
        {
            EnsureTesterSaveExists(gameData);
        }

        // Always reload the save after toggling
        if (!LoadSave(gameData))
        {
            Debug.LogWarning("[TesterSaveManager] Reload after toggling tester saves failed.");
        }
    }


    public void EnsureTesterSaveExists(GameData gameData)
    {
        string testerSavePath = GetSaveFilePath(testerKey);

        if (!System.IO.File.Exists(testerSavePath))
        {
            Debug.Log("Creating new tester save file...");
            // Save current game data into tester save with tester flag on
            gameData.SaveData(testerSavePath, true);
        }
    }

    public void SaveGame(GameData gameData)
    {
        string savePath;

        if (allowTesterSaves)
            savePath = GetSaveFilePath(testerKey);
        else
            savePath = GetSaveFilePath(regularKey);

        gameData.SaveData(savePath, allowTesterSaves);
    }
}