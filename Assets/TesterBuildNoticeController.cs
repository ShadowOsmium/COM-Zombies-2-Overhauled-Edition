using UnityEngine;
using UnityEngine.UI;

public class TesterBuildNoticeController : MonoBehaviour
{
    public GameObject testBuildNotice;

    private void Start()
    {
        if (TesterSaveManager.Instance != null && TesterSaveManager.Instance.allowTesterSaves)
        {
            if (testBuildNotice != null)
                testBuildNotice.SetActive(true);
        }
        else
        {
            if (testBuildNotice != null)
                testBuildNotice.SetActive(false);
        }
    }
}
