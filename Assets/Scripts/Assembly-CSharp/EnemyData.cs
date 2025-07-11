using CoMZ2;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
	public string enemy_name = string.Empty;

	public EnemyType enemy_type = EnemyType.E_NONE;

	public float move_speed;

    private Dictionary<GameObject, float> damageDoneByPlayer = new Dictionary<GameObject, float>();

    public float hp_capacity;

	public float cur_hp;

	public float damage_val;

	public float frequency_val;

	public float attack_range;

	public float view_range;

	public EnemyAttackPriority attack_priority;

	public int loot_cash;

	public int exp;

	public EnemyConfig config;

	public void InitData(EnemyConfig econfig)
	{
		config = econfig;
		enemy_type = econfig.enemy_type;
		move_speed = config.speed_val;
		attack_range = config.attack_range;
		view_range = config.view_range;
		frequency_val = config.attack_frequency;
		attack_priority = config.attack_priority;
		if (GameSceneController.Instance.mission_day_type == MissionDayType.Tutorial)
		{
			cur_hp = (hp_capacity = 15f);
		}
		else
		{
			cur_hp = (hp_capacity = GameSceneController.Instance.enemy_standard_hp * config.hp_ratio);
		}
		damage_val = GameSceneController.Instance.enemy_standard_dps * config.damage_ratio;
		loot_cash = (exp = (int)(GameSceneController.Instance.enemy_standard_reward * config.reward_ratio));
	}

    private static readonly HashSet<EnemyType> BossTypes = new HashSet<EnemyType>
{
    EnemyType.E_FATCOOK,
    EnemyType.E_FATCOOK_E,
    EnemyType.E_HAOKE_A,
    EnemyType.E_HAOKE_B,
    EnemyType.E_WRESTLER,
    EnemyType.E_WRESTLER_E,
    EnemyType.E_HALLOWEEN,
    EnemyType.E_HALLOWEEN_E,
    EnemyType.E_HALLOWEEN_SUB,
    EnemyType.E_HALLOWEEN_SUB_E,
    EnemyType.E_SHARK,
    EnemyType.E_SHARK_E,
};

    public static EnemyData CreateData(EnemyConfig config)
	{
		EnemyData enemyData = new EnemyData();
		enemyData.InitData(config);
		return enemyData;
	}

    public bool OnInjured(float damage, ObjectController attacker)
    {
        if (damage > 0f)
        {
            // === COOP BOSS DAMAGE TRACKING ===
            if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && attacker != null)
            {
                EnemyType type = this.enemy_type;

                if (type == EnemyType.E_FATCOOK || type == EnemyType.E_FATCOOK_E ||
                    type == EnemyType.E_HAOKE_A || type == EnemyType.E_HAOKE_B ||
                    type == EnemyType.E_WRESTLER || type == EnemyType.E_WRESTLER_E ||
                    type == EnemyType.E_HALLOWEEN || type == EnemyType.E_HALLOWEEN_E ||
                    type == EnemyType.E_HALLOWEEN_SUB || type == EnemyType.E_HALLOWEEN_SUB_E ||
                    type == EnemyType.E_SHARK || type == EnemyType.E_SHARK_E)
                {
                    string playerName = attacker.name;

                    if (GameSceneController.Instance.boss_damage_record == null)
                        GameSceneController.Instance.boss_damage_record = new Dictionary<string, float>();

                    var dict = GameSceneController.Instance.boss_damage_record;
                    if (!dict.ContainsKey(playerName))
                        dict[playerName] = 0f;

                    dict[playerName] += damage;

                    int reward = Mathf.FloorToInt(dict[playerName] / 10f);
                    Debug.Log("game_reward: " + playerName + " damage:" + dict[playerName] + " money:" + reward);
                }
            }
            // === END TRACKING ===

            cur_hp -= damage;
            return cur_hp <= 0f;
        }

        return false;
    }
}

