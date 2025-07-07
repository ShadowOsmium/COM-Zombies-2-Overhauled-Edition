using CoMZ2;

public class QuestInfo
{
	public string scene_name = string.Empty;

	public MissionType mission_type;

	public MissionDayType mission_day_type;

	public string mission_tag = string.Empty;

	public AvatarType avatar = AvatarType.None;

	public string reward_prob = string.Empty;

	public string reward_weapon = string.Empty;

	public EnemyType boss_type = EnemyType.E_FATCOOK;

	public bool camera_roam_enable;

	public void SetQuestComment()
	{
		switch (mission_type)
		{
		case MissionType.Cleaner:
			mission_tag = GameConfig.Instance.clean_mission_comment;
			break;
		case MissionType.Time_ALive:
			mission_tag = GameConfig.Instance.time_mission_comment;
			break;
		case MissionType.Npc_Resources:
			mission_tag = GameConfig.Instance.res_mission_comment;
			break;
		case MissionType.Npc_Convoy:
			mission_tag = GameConfig.Instance.convoy_mission_comment;
			break;
		case MissionType.Boss:
			mission_tag = GameConfig.Instance.boss_mission_comment;
			break;
        case MissionType.Endless:
			mission_tag = GameConfig.Instance.endless_mission_comment;
			break;
		}
	}
}
