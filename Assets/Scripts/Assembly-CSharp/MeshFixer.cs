using UnityEngine;
using UnityEngine.SceneManagement;

public class MeshFixer : MonoBehaviour
{
    private static MeshFixer _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Fix();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Fix();
    }

    public static void Fix()
    {
        foreach (GameObject gobj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (gobj.GetComponent<MeshFilter>() && gobj.GetComponent<MeshCollider>())
            {
                gobj.GetComponent<MeshFilter>().sharedMesh = gobj.GetComponent<MeshCollider>().sharedMesh;
            }
        }
    }
}
