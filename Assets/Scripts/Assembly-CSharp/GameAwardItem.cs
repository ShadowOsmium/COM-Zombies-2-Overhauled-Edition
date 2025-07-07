public class GameAwardItem
{
	public enum AwardType
	{
		None,
		Weapon,
		WeaponFragment,
		Cash,
		Voucher,
		Crystal
	}

	public AwardType award_type;

	public int award_level;

	public string award_name = string.Empty;

	public int award_count;

	public override string ToString()
	{
		return string.Concat("type:", award_type, " level:", award_level, " name:", award_name, " count:", award_count);
	}
}
