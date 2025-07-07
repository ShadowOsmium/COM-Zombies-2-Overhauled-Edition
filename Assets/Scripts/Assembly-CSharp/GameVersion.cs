using System.Collections;
using System.IO;
using CoMZ2;
using UnityEngine;

public class GameVersion : MonoBehaviour
{
    private static GameVersion instance;

    protected string content = string.Empty;

    public bool is_test_config;

    public static GameVersion Instance
    {
        get { return instance; }
    }

    public delegate void OnServerVersion(bool isUpToDate);

    public delegate void OnServerVersionError();


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
        string expectedVersion = "1.3.1";

        if (currentVersion == expectedVersion)
        {
            if (callback != null)
                callback(true);
            return;
        }
        if (GameDefine.LOAD_CONFIG_SAVE_PATH && GameConfig.IsEditorMode())
        {
            Utils.FileReadString(Utils.SavePath() + "CoMZ2_version.bytes", ref content);
            content = Encipher(content);
            Debug.Log(content);

            Configure configure = new Configure();
            configure.Load(content);

            string ver = configure.GetSingle("CoMZ2", "Ver");
            string testVer = configure.GetSingle("CoMZ2", "TestVer");
            ver = configure.GetSingle("CoMZ2", "VerAndroid");
            testVer = configure.GetSingle("CoMZ2", "TestVerAndroid");

            bool iapCheckFlag = (int.Parse(configure.GetSingle("CoMZ2", "IapCheck")) != 0);
            if (iapCheckFlag != GameData.Instance.TRINITI_IAP_CEHCK)
            {
                GameData.Instance.TRINITI_IAP_CEHCK = iapCheckFlag;
                GameData.Instance.SaveData();
            }

            int count = 0;
            if (ver == "1.3.1")
            {
                Debug.Log("to normal server.");
                is_test_config = false;
                count = int.Parse(configure.GetSingle("CoMZ2", "ConfigVersionCount"));
                for (int i = 0; i < count; i++)
                {
                    string key = configure.GetArray2("CoMZ2", "ConfigVersion", i, 0);
                    string val = configure.GetArray2("CoMZ2", "ConfigVersion", i, 1);
                    GameConfig.Instance.Remote_Config_Version_Set[key] = val;
                }
                if (callback != null)
                    callback(true);
            }
            else if (testVer == "1.3.1")
            {
                Debug.Log("to test server.");
                is_test_config = true;
                count = int.Parse(configure.GetSingle("CoMZ2", "ConfigVersionCountTest"));
                for (int i = 0; i < count; i++)
                {
                    string key = configure.GetArray2("CoMZ2", "ConfigVersionTest", i, 0);
                    string val = configure.GetArray2("CoMZ2", "ConfigVersionTest", i, 1);
                    GameConfig.Instance.Remote_Config_Version_Set[key] = val;
                }
                if (callback != null)
                    callback(true);
            }
            else
            {
                Debug.Log("game version error.");
                if (callback != null)
                    callback(false);
            }
        }
        else
        {
            StartCoroutine(CheckGameVersionCoroutine(callback, callback_error));
        }
    }

    private IEnumerator CheckGameVersionCoroutine(OnServerVersion callback, OnServerVersionError callback_error)
    {
        string url = "https://api.github.com/repos/ShadowOsmium/COM-Zombies-2/releases/latest?rand=" + Random.Range(10, 99999);

        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            if (callback_error != null)
                callback_error();
            yield break;
        }

        string json = www.text;
        string latestVersion = ExtractTagName(json);

        if (callback != null)
            callback(true);

        content = www.text;
        content = Encipher(content);
        Debug.Log(content);

        Configure cfg = new Configure();
        cfg.Load(content);

        string ver = cfg.GetSingle("CoMZ2", "Ver");
        string testVer = cfg.GetSingle("CoMZ2", "TestVer");
        ver = cfg.GetSingle("CoMZ2", "VerAndroid");
        testVer = cfg.GetSingle("CoMZ2", "TestVerAndroid");

        int count = 0;
        if (ver == "1.3.1")
        {
            Debug.Log("to normal server.");
            is_test_config = false;
            count = int.Parse(cfg.GetSingle("CoMZ2", "ConfigVersionCount"));
            for (int i = 0; i < count; i++)
            {
                string key = cfg.GetArray2("CoMZ2", "ConfigVersion", i, 0);
                string val = cfg.GetArray2("CoMZ2", "ConfigVersion", i, 1);
                GameConfig.Instance.Remote_Config_Version_Set[key] = val;
            }
            if (callback != null)
                callback(true);
        }
        else if (testVer == "1.3.1")
        {
            Debug.Log("to test server.");
            is_test_config = true;
            count = int.Parse(cfg.GetSingle("CoMZ2", "ConfigVersionCountTest"));
            for (int i = 0; i < count; i++)
            {
                string key = cfg.GetArray2("CoMZ2", "ConfigVersionTest", i, 0);
                string val = cfg.GetArray2("CoMZ2", "ConfigVersionTest", i, 1);
                GameConfig.Instance.Remote_Config_Version_Set[key] = val;
            }
            if (callback != null)
                callback(true);
        }
        else
        {
            Debug.Log("game version error.");
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
        return json.Substring(startIndex, endIndex - startIndex);
    }
}
