using UnityEngine;

public class ButtonHider : MonoBehaviour
{
    void Start()
    {
        if (GameData.Instance != null && GameData.Instance.blackname)
        {
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

            GameObject endless = GameObject.Find("EndlessTrans");
            if (endless != null)
            {
                endless.SetActive(false);
            }
        }
    }

    private void DisableMissionIcon(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

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
