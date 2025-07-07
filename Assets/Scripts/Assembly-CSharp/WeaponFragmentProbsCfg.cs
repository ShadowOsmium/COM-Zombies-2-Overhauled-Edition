public class WeaponFragmentProbsCfg : GameProbsCfg
{
	public enum WeaponFragmentType
	{
		none,
		part1,
		part2,
		part3,
		part4,
		part5,
		part6
	}

	public string weapon_name = string.Empty;

	public string image_name = string.Empty;

	public WeaponFragmentType type;

	public GameDataInt sell_price;

	public WeaponFragmentProbsCfg()
	{
		prob_type = ProbType.WeaponFragment;
	}
}
