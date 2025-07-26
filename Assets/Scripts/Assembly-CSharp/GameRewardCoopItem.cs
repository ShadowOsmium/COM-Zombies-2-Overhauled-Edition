using UnityEngine;

public class GameRewardCoopItem : MonoBehaviour
{
	public TUIMeshSprite user_icon;

	public TUIMeshSprite reward_money_icon;

	public TUIMeshSpriteSliced self_bk;

	public TUILabel user_name;

	public TUILabel user_damage;

	public TUILabel reward_money;

	public GameRewardItem weapon_fragment_item;

	public bool fragment_sell;

	public TUIMeshSprite reward_weapon;

	public TUIMeshSprite mvp_tip;

	private void Start()
	{
	}

	private void Update()
	{
	}

    public void Init(GameRewardCoop reward)
    {
        Debug.Log("Init reward UI: player=" + reward.nick_name + ", money_count=" + reward.money_count);

        user_icon.texture = GameConfig.Instance.AvatarConfig_Set[reward.avatar_type].avatar_name + "_0" + (int)reward.avatar_state + "_icon";

        user_name.Text = reward.nick_name;
        user_damage.Text = reward.damage.ToString();

        if (reward.money_type == GameRewardCoop.RewardMoneyType.CASH)
        {
            reward_money_icon.texture = "jinbi";
            mvp_tip.gameObject.SetActive(false);
        }
        else if (reward.money_type == GameRewardCoop.RewardMoneyType.CRYSTAL)
        {
            reward_money_icon.texture = "shuijing";
            mvp_tip.gameObject.SetActive(true);
        }

        reward_money.Text = reward.money_count.ToString();

        self_bk.gameObject.SetActive(reward.is_myself);
        fragment_sell = reward.fragment_sell;

        if (reward.weapon_fragment == null)
        {
            weapon_fragment_item.gameObject.SetActive(false);
        }
        else if (reward.weapon_fragment.reward_type == GameReward.GameRewardType.WEAPON)
        {
            weapon_fragment_item.gameObject.SetActive(false);
            reward_weapon.gameObject.SetActive(true);
            reward_weapon.texture = reward.weapon_fragment.reward_frame;
        }
        else
        {
            weapon_fragment_item.SetReward(reward.weapon_fragment.reward_frame, 1);
        }
    }
}
