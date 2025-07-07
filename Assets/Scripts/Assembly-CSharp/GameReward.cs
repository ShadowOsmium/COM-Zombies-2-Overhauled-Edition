public class GameReward
{
	public enum GameRewardType
	{
		NONE,
		CASH,
		CRYSTAL,
		VOUCHER,
		AVATAR,
		WEAPONFRAGMENT,
		WEAPON
	}

	public GameRewardType reward_type;

	public string reward_frame = string.Empty;

	public int reward_count;

	public GameReward(GameRewardType type, string frame, int count)
	{
		reward_type = type;
		reward_frame = frame;
		reward_count = count;
	}
}
