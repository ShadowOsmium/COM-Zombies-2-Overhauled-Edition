using UnityEngine;

public class ButtonHider : MonoBehaviour
{
    void Start()
    {
        GameObject endless = GameObject.Find("EndlessTrans");
        GameObject endlessLocked = GameObject.Find("EndlessLocked");

        if (GameData.Instance != null && GameData.Instance.blackname)
        {
            // Hide all mission icons, including coop and endless
            GameObject[] dailyIcons = GameObject.FindGameObjectsWithTag("Daily_Mission_Tag");
            for (int i = 0; i < dailyIcons.Length; i++)
            {
                DisableMissionIcon(dailyIcons[i]);
            }

            GameObject[] missionIcons = GameObject.FindGameObjectsWithTag("Map_Mission_Tag");
            for (int i = 0; i < missionIcons.Length; i++)
            {
                GameObject icon = missionIcons[i];
                if (icon != null)
                {
                    string name = icon.name.ToLower();
                    if (name.Contains("coop") || name.Contains("endless"))
                    {
                        DisableMissionIcon(icon);
                    }
                }
            }

            GameObject coop = GameObject.Find("CoopTrans");
            if (coop != null)
            {
                coop.SetActive(false);
            }

            if (endless != null) endless.SetActive(false);
            if (endlessLocked != null) endlessLocked.SetActive(false);
        }
        else
        {
            if (endless != null)
                endless.SetActive(GameData.Instance.day_level >= 35);

            if (endlessLocked != null)
                endlessLocked.SetActive(GameData.Instance.day_level < 35);
        }
    }

    private void DisableMissionIcon(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        for (int j = 0; j < renderers.Length; j++)
        {
            renderers[j].enabled = false;
        }

        Component ctrl = obj.GetComponent("TUIControl");
        if (ctrl != null)
        {
            Behaviour behaviour = ctrl as Behaviour;
            if (behaviour != null)
            {
                behaviour.enabled = false;
            }
        }
    }
}
