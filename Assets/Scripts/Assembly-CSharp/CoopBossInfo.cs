using UnityEngine;

public class CoopBossInfo : MonoBehaviour
{
	public CoopBossType cur_boss_type = CoopBossType.E_NONE;

	protected UICoopBossItem boss_item;

	public UICoopBossItem BossItem
	{
		get
		{
			return boss_item;
		}
	}

	private void Start()
	{
		boss_item = base.transform.parent.GetComponent<UICoopBossItem>();
		boss_item.SetBossName(GameConfig.Instance.Coop_Boss_Cfg_Set[GameConfig.GetEnemyTypeFromBossType(cur_boss_type)].boss_show_name);
	}

	private void Update()
	{
	}
}
