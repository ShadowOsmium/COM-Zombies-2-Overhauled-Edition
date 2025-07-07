using UnityEngine;

public class ButtonHider : MonoBehaviour
{
    void Start()
    {
        if (GameData.Instance != null && GameData.Instance.blackname)
        {
            // Disable Daily mission buttons
            GameObject[] dailyIcons = GameObject.FindGameObjectsWithTag("Daily_Mission_Tag");
            for (int i = 0; i < dailyIcons.Length; i++)
            {
                DisableMissionIcon(dailyIcons[i]);
            }

            // Disable Coop and Endless mission icons
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

            // Also disable CoopTrans and EndlessTrans visuals
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
        // Disable collider so it can't be clicked
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Optional: disable visuals
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        for (int j = 0; j < renderers.Length; j++)
        {
            renderers[j].enabled = false;
        }

        // Optional: disable TUIControl if used
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
