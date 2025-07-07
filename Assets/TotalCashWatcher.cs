using UnityEngine;
using System.Diagnostics;

public class TotalCashWatcher : MonoBehaviour
{
    /*private int lastTotalCash = -1;

    private GameDataInt _total_cash;

    public GameDataInt total_cash
    {
        get { return _total_cash; }
        set
        {
            UnityEngine.Debug.Log("[GameData] total_cash set from " +
                (_total_cash != null ? _total_cash.GetIntVal().ToString() : "null") +
                " to " + (value != null ? value.GetIntVal().ToString() : "null") +
                " at time " + Time.time + "\nStackTrace:\n" + new System.Diagnostics.StackTrace(true).ToString());

            _total_cash = value;
        }
    }

    void Update()
    {
        int currentCash = GameData.Instance.total_cash.GetIntVal();

        if (currentCash != lastTotalCash)
        {
            lastTotalCash = currentCash;

            UnityEngine.Debug.Log("[TotalCashWatcher] total_cash changed to " + currentCash + " at time " + Time.time);
  
            StackTrace trace = new StackTrace(true);
            UnityEngine.Debug.Log("[TotalCashWatcher] Call Stack:\n" + trace.ToString());
        }
    }*/
}
