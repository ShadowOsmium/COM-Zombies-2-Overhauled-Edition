using System;
using System.Collections;
using System.IO;
using CoMZ2;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameVersion : MonoBehaviour
{
    private static GameVersion instance;

    protected string content = string.Empty;

    public bool is_test_config;

    public static GameVersion Instance
    {
        get { return instance; }
    }
    private string latestVersion;

    public delegate void OnServerVersion(bool isUpToDate);

    public delegate void OnServerVersionError();

    private static string fallbackVersion;

    static GameVersion()
    {
        TextAsset versionFile = Resources.Load<TextAsset>("version");
        fallbackVersion = versionFile != null ? versionFile.text.Trim() : "1.0.0";
    }

    public static string GetVersion()
    {
#if UNITY_IOS || UNITY_ANDROID
        return Application.version;
#else
        return fallbackVersion;
#endif
    }

    public static void CheckGameVersionInstance()
    {
        if (GameObject.Find("GameVersion") == null)
        {
            GameObject gameObject = new GameObject("GameVersion");
            gameObject.transform.parent = null;
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.AddComponent<GameVersion>();
        }
    }

    private void Awake()
    {
        instance = this;
        Object.DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
    }

    public void CheckRemoteGameVersion(OnServerVersion callback, OnServerVersionError callback_error)
    {
        string currentVersion = Application.version;
        string versionFilePath = Utils.SavePath() + "CoMZ2_version.bytes";

        if (File.Exists(versionFilePath))
        {
            string fileContent = string.Empty;
            Utils.FileReadString(versionFilePath, ref fileContent);
            fileContent = Encipher(fileContent);

            Configure configure = new Configure();
            configure.Load(fileContent);

            string expectedVersion = configure.GetSingle("CoMZ2", "VerAndroid");
            if (string.IsNullOrEmpty(expectedVersion))
                expectedVersion = configure.GetSingle("CoMZ2", "Ver");

            Debug.Log(string.Format("Local version: {0}, App version: {1}", expectedVersion, currentVersion));

            if (VersionsMatch(currentVersion, expectedVersion))
            {
                if (callback != null)
                    callback(true);
                return;  // local version matches, no need to check GitHub
            }
            else
            {
                Debug.Log("Local version mismatch, checking GitHub latest version...");
                StartCoroutine(CheckLatestVersionFromGitHub(callback, callback_error));
            }
        }
        else
        {
            Debug.Log("Local version file missing, checking GitHub latest version...");
            StartCoroutine(CheckLatestVersionFromGitHub(callback, callback_error));
        }
    }

    private IEnumerator CheckLatestVersionFromGitHub(OnServerVersion callback, OnServerVersionError callback_error)
    {
        string url = "https://api.github.com/repos/ShadowOsmium/COM-Zombies-2-Overhauled-Edition/releases/latest?rand=" + Random.Range(10, 99999);

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("User-Agent", "COM-Zombies-2-GameVersionCheck");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (callback_error != null)
                callback_error();
            yield break;
        }

        string jsonText = www.downloadHandler.text;
        Debug.Log("GitHub response text: " + jsonText);

        string latestTag = ExtractTagName(jsonText);

        Debug.Log("GitHub latest version tag: " + latestTag + ", App version: " + Application.version);

        if (VersionsMatch(Application.version, latestTag))
        {
            if (callback != null)
                callback(true);
        }
        else
        {
            if (callback != null)
                callback(false);
        }
    }

    private IEnumerator CheckGameVersionCoroutine(OnServerVersion callback, OnServerVersionError callback_error)
    {
        string url = "https://api.github.com/repos/ShadowOsmium/COM-Zombies-2-Overhauled-Edition/releases/latest?rand=" + Random.Range(10, 99999);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Version check failed: " + www.error);
            if (callback_error != null)
                callback_error();
            yield break;
        }

        content = www.downloadHandler.text;
        latestVersion = ExtractTagName(content);

        Debug.Log("Latest GitHub version tag: " + latestVersion);

        string currentVersion = Application.version;

        if (VersionsMatch(currentVersion, latestVersion))
        {
            Debug.Log("Version is up to date.");
            if (callback != null)
                callback(true);
        }
        else
        {
            Debug.Log("Version mismatch.");
            if (callback != null)
                callback(false);
        }
    }

    private string Encipher(string data)
    {
        int xorValue = 30;
        char[] chars = data.ToCharArray();
        char[] result = new char[chars.Length];
        char[] tmp = new char[2];

        for (int i = 0; i < chars.Length; i++)
        {
            tmp[0] = chars[i];
            string s = new string(tmp);
            int codePoint = char.ConvertToUtf32(s, 0);
            codePoint ^= xorValue;
            result[i] = char.ConvertFromUtf32(codePoint)[0];
        }

        return new string(result);
    }

    private bool VersionsMatch(string v1, string v2)
    {
        if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
        {
            Debug.LogWarning(string.Format("VersionsMatch failed due to empty input: v1='{0}', v2='{1}'", v1, v2));
            return false;
        }

        string v1Trim = v1.Trim();
        string v2Trim = v2.Trim();

        Debug.Log(string.Format("Comparing versions: '{0}' vs '{1}'", v1Trim, v2Trim));

        bool match = string.Equals(v1Trim, v2Trim, StringComparison.OrdinalIgnoreCase);

        Debug.Log("VersionsMatch result: " + match);

        return match;
    }

    public void OutputVersionCheckFile()
    {
        string data = Utils.LoadResourcesFileForText("CoMZ2_version");
        string savePath = Utils.SavePath();

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        data = Encipher(data);
        Utils.FileWriteString(savePath + "CoMZ2_version.bytes", data);
        Debug.Log("CoM2_version.bytes output is ok.");
    }

    private string ExtractTagName(string json)
    {
        string tag = "\"tag_name\":\"";
        int startIndex = json.IndexOf(tag);
        if (startIndex == -1)
            return null;
        startIndex += tag.Length;
        int endIndex = json.IndexOf("\"", startIndex);
        if (endIndex == -1)
            return null;

        string rawTag = json.Substring(startIndex, endIndex - startIndex);

        if (!string.IsNullOrEmpty(rawTag) && rawTag.StartsWith("v"))
        {
            return rawTag.Substring(1);
        }
        return rawTag;
    }

}
