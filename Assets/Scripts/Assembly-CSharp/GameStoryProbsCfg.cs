public class GameStoryProbsCfg : GameProbsCfg
{
	public int level;

	public int weight;

	public string image_name = string.Empty;

	public GameStoryProbsCfg()
	{
		prob_type = ProbType.GameStory;
	}
}
