using UnityEngine;
using UnityEngine.UI;

public class UICurrencyController : MonoBehaviour
{
    public static UICurrencyController Instance;

    public Text crystalLabel;
    public Text cashLabel;
    public Text voucherLabel;

    private void Awake()
    {
        Instance = this;
    }

    public void Refresh()
    {
        if (GameData.Instance == null) return;

        crystalLabel.text = GameData.Instance.total_crystal.GetIntVal().ToString();
        cashLabel.text = GameData.Instance.total_cash.GetIntVal().ToString();
        voucherLabel.text = GameData.Instance.total_voucher.GetIntVal().ToString();
    }
}
