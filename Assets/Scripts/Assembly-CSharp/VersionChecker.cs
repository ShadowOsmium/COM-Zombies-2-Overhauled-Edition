using UnityEngine;
using System.Collections;

public class VersionChecker : MonoBehaviour
{
    public string remoteVersionUrl = "https://raw.githubusercontent.com/YourUser/YourRepo/main/version.txt";
    public string updateUrl = "https://github.com/YourUser/YourRepo/releases";

    void Start()
    {
        StartCoroutine(CheckVersionRoutine());
    }

    IEnumerator CheckVersionRoutine()
    {
        WWW www = new WWW(remoteVersionUrl);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogWarning("Failed to fetch remote version. Allowing play.");
            yield break;
        }

        string remoteVersion = www.text.Trim();
        string localVersion = GameVersion.GetVersion();

        Debug.Log("Local Version: " + localVersion + " | Remote Version: " + remoteVersion);

        if (localVersion != remoteVersion)
        {
            Debug.LogError("Outdated game version detected! Forcing update.");
            Application.OpenURL(updateUrl);
            Application.Quit();
        }
        else
        {
            Debug.Log("Game version is up to date.");
        }
    }
}
