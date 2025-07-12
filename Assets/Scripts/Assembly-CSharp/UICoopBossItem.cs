using UnityEngine;

public class UICoopBossItem : MonoBehaviour
{
    public TUIMeshSpriteSliced ui_board;
    public TUIMeshSprite ui_bk;
    public TUILabel ui_name;
    public CoopBossInfo boss_info;
    public CoopBossType cur_boss_type;

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void SetBossName(string name)
    {
        if (ui_name != null)
        {
            ui_name.Text = name;
        }
    }

    public void SetItemChoiced(bool state)
    {
        if (ui_board == null || ui_bk == null)
        {
            //Debug.LogError("UICoopBossItem: ui_board or ui_bk is null!");
            return;
        }

        string bossName = (boss_info != null) ? boss_info.name : "Unknown";

       //Debug.Log("SetItemChoiced called with state=" + state + " for boss " + bossName);

        if (state)
        {
            ui_board.texture = null;
            ui_board.texture = "Dikuang_dianji";

            ui_bk.texture = null;
            ui_bk.texture = "touxiang_ditu_dianji";
        }
        else
        {
            ui_board.texture = null;
            ui_board.texture = "Dikuang";

            ui_bk.texture = null;
            ui_bk.texture = "touxiang_ditu";
        }
    }

/*    public void SetLocked(bool locked, string bossName, int requiredDay)
    {
        // Show/hide lock icon
        Transform lockObj = transform.Find("lock");
        if (lockObj != null)
            lockObj.gameObject.SetActive(locked);

        // Enable/disable button input
        TUIButtonClick button = GetComponent<TUIButtonClick>();
        if (button != null)
            button.enabled = !locked;

        // Apply grayscale to button states
        SetButtonGray(locked);

        // Update requirement label text
        Transform labelTransform = transform.Find("requirement_label");
        if (labelTransform != null)
        {
            TUILabel label = labelTransform.GetComponent<TUILabel>();
            if (label != null)
            {
                label.Text = locked ? "Unlocks at Day " + requiredDay.ToString() : "";
            }
        }
    }

    private void SetButtonGray(bool gray)
    {
        string[] stateNames = { "StateNormal", "StatePress" };
        foreach (string stateName in stateNames)
        {
            Transform stateTransform = transform.Find(stateName);
            if (stateTransform != null)
            {
                TUIMeshSprite sprite = stateTransform.GetComponent<TUIMeshSprite>();
                if (sprite != null)
                {
                    sprite.GrayStyle = gray;
                }
            }
        }
    }*/
}