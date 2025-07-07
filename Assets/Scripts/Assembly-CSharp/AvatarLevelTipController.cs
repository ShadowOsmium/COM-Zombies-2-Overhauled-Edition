using UnityEngine;

public class AvatarLevelTipController : MonoBehaviour
{
	protected AvatarData avatar_data;

	public TUIMeshSprite tip_icon;

	public TUILabel tip_label;

	public TUIMeshSprite tip_bk;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Init(AvatarData avatar)
	{
		avatar_data = avatar;
		ResetTip();
	}

    public void ResetTip()
    {
        if (avatar_data.exist_state == AvatarExistState.Owned)
        {
            if (avatar_data.avatar_worth >= avatar_data.config.avatar_worth_2)
            {
                tip_bk.gameObject.SetActive(false);
                return;
            }

            tip_bk.gameObject.SetActive(true);

            if (avatar_data.avatar_state == AvatarData.AvatarState.Super)
            {
                tip_label.Text = avatar_data.avatar_worth + "/" + avatar_data.config.avatar_worth_2;
                tip_icon.texture = avatar_data.config.avatar_name + "_0" + (int)avatar_data.avatar_state + "_icon";
            }
            else
            {
                tip_label.Text = avatar_data.avatar_worth + "/" + NextLevelPrice();
                tip_icon.texture = avatar_data.config.avatar_name + "_0" + (int)(avatar_data.avatar_state + 1) + "_icon";
            }
        }
        else
        {
            tip_bk.gameObject.SetActive(false);
        }
    }

    private int NextLevelPrice()
	{
		int result = 0;
		if (avatar_data.avatar_state == AvatarData.AvatarState.Normal)
		{
			result = avatar_data.config.avatar_worth_1;
		}
		else if (avatar_data.avatar_state == AvatarData.AvatarState.Strong)
		{
			result = avatar_data.config.avatar_worth_2;
		}
		return result;
	}
}
