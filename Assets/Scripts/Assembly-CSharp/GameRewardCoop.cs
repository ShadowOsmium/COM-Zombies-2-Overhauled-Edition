public class GameRewardCoop
{
	public enum RewardMoneyType
	{
		NONE,
		CASH,
		CRYSTAL
	}

	public AvatarType avatar_type;

	public AvatarData.AvatarState avatar_state;

	public string nick_name;

	public int damage;

	public RewardMoneyType money_type;

	public int money_count;

	public GameReward weapon_fragment;

	public bool is_myself;

	public bool fragment_sell;

	public GameRewardCoop(AvatarType avatar_type, AvatarData.AvatarState state, string nick_name, int damage, RewardMoneyType money_type, int money_count, GameReward weapon_fragment, bool is_myself, bool fragment_sell)
	{
		this.avatar_type = avatar_type;
		avatar_state = state;
		this.nick_name = nick_name;
		this.damage = damage;
		this.money_type = money_type;
		this.money_count = money_count;
		this.weapon_fragment = weapon_fragment;
		this.is_myself = is_myself;
		this.fragment_sell = fragment_sell;
	}
}
