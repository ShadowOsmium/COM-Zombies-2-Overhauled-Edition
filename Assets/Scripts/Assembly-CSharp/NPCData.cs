public class NPCData
{
	public string npc_name = string.Empty;

	public NpcType npc_type = NpcType.N_NONE;

	public float move_speed;

	public float hp_capacity;

	public float cur_hp;

	public NpcConfig config;

	public void InitData(NpcConfig nconfig)
	{
		config = nconfig;
		npc_type = nconfig.npc_type;
		move_speed = config.speed_val;
		cur_hp = (hp_capacity = GameSceneController.Instance.player_controller.avatar_data.hp_capacity * config.hp_ratio);
	}

	public static NPCData CreateData(NpcConfig config)
	{
		NPCData nPCData = new NPCData();
		nPCData.InitData(config);
		return nPCData;
	}

	public bool OnInjured(float damage)
	{
		cur_hp -= damage;
		if (cur_hp <= 0f)
		{
			cur_hp = 0f;
			return true;
		}
		return false;
	}
}
