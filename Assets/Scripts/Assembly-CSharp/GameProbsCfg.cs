public class GameProbsCfg
{
	public enum ProbType
	{
		None,
		WeaponIntensifier,
		WeaponFragment,
		GameStory
	}

    public ProbType prob_type;
    public int weight = 1;

    public string prob_name = string.Empty;
}
