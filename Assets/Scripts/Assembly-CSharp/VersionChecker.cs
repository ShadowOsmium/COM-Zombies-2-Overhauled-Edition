using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

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
        UnityWebRequest www = UnityWebRequest.Get(remoteVersionUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogWarning("Failed to fetch remote version. Allowing play.");
            yield break;
        }

        string remoteVersion = www.downloadHandler.text.Trim();
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
