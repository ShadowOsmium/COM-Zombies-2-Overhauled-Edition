public class WeaponIntensifierProbsCfg : GameProbsCfg
{
	public string weapon_name = string.Empty;

	public int weight;

	public string image_name = string.Empty;

	public WeaponIntensifierProbsCfg()
	{
		prob_type = ProbType.WeaponIntensifier;
	}
}
