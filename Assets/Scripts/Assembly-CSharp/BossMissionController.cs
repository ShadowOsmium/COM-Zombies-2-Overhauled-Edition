using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
public class BossMissionController : MissionController
{
    private float lastCheckTime;
    private const float checkRate = 1f;

    protected EnemyController boss;
    public EnemyController MissionBoss
    {
        get
        {
            return boss;
        }
    }

    private List<EnemyWaveInfo> waveList;

    public override List<EnemyType> GetMissionEnemyTypeList()
    {
        var waves = GetCurrentOrMaxWaveList();
        var enemyTypes = new List<EnemyType>();

        if (waves != null)
        {
            foreach (var wave in waves)
                foreach (var spawn in wave.spawn_info_list)
                    if (!enemyTypes.Contains(spawn.EType))
                        enemyTypes.Add(spawn.EType);
        }

        EnemyType bossType = GameData.Instance.cur_quest_info.boss_type;
        Debug.Log("[BossMissionController] Boss type before spawning: " + bossType);
        enemyTypes.Add(bossType);

        switch (bossType)
        {
            case EnemyType.E_HALLOWEEN:
                if (!enemyTypes.Contains(EnemyType.E_HALLOWEEN_SUB))
                    enemyTypes.Add(EnemyType.E_HALLOWEEN_SUB);
                break;
            case EnemyType.E_HALLOWEEN_E:
                if (!enemyTypes.Contains(EnemyType.E_HALLOWEEN_SUB_E))
                    enemyTypes.Add(EnemyType.E_HALLOWEEN_SUB_E);
                break;
            case EnemyType.E_FATCOOK:
                if (!enemyTypes.Contains(EnemyType.E_BOOMER_TIMER))
                    enemyTypes.Add(EnemyType.E_BOOMER_TIMER);
                break;
            case EnemyType.E_FATCOOK_E:
                if (!enemyTypes.Contains(EnemyType.E_BOOMER_TIMER_E))
                    enemyTypes.Add(EnemyType.E_BOOMER_TIMER_E);
                break;
                // Add other bosses + minions here when needed
        }

        return enemyTypes;
    }


    private List<EnemyWaveInfo> GetCurrentOrMaxWaveList()
    {
        foreach (var level in GameConfig.Instance.EnemyWaveInfo_Boss_Set.Keys)
            if (GameSceneController.Instance.DayLevel <= level)
                return GameConfig.Instance.EnemyWaveInfo_Boss_Set[level].wave_info_list;

        int maxKey = -1;
        foreach (var key in GameConfig.Instance.EnemyWaveInfo_Boss_Set.Keys)
            if (key > maxKey)
                maxKey = key;

        return maxKey != -1 ? GameConfig.Instance.EnemyWaveInfo_Boss_Set[maxKey].wave_info_list : null;
    }

    public override IEnumerator Start()
    {
        InitMissionController();
        mission_type = MissionType.Boss;
        CaculateDifficulty();

        yield return null;

        PlayerController player = null;
        while ((player = GameSceneController.Instance.player_controller) == null)
            yield return null;

        EnemyType bossType = GameData.Instance.cur_quest_info.boss_type;
        string iconName = GameConfig.Instance.EnemyConfig_Set[bossType].enemy_name + "_icon_s";

        GameSceneController.Instance.game_main_panel.boss_panel.SetIcon(iconName);
        GameSceneController.Instance.game_main_panel.boss_panel.SetContent("0 / 1");

        waveList = GetCurrentOrMaxWaveList();

        while (!GameSceneController.Instance.enable_boss_spawn)
            yield return null;

        boss = SpawnBossFromNest(bossType, zombie_boss_array[Random.Range(0, zombie_boss_array.Length)]);

        if (boss != null)
        {
            boss.IsBoss = true;
            boss.enemyType = bossType;
            Debug.Log("[BossMissionController] Boss spawned of type: " + boss.enemyType);
        }
        else
        {
            Debug.LogError("[BossMissionController] Boss spawn failed: boss is null!");
        }

        while (GameSceneController.Instance.GamePlayingState == PlayingState.CG)
            yield return null;

        player.UpdateWeaponUIShow();

        yield return new WaitForSeconds(1f);

        while (boss != null && boss.enemy_data.cur_hp > 0f)
        {
            int index = Random.Range(0, waveList.Count);
            var wave = waveList[index];

            foreach (var spawnInfo in wave.spawn_info_list)
            {
                if (boss == null || boss.enemy_data.cur_hp <= 0f)
                    break;

                for (int i = 0; i < spawnInfo.Count; i++)
                {
                    while (GameSceneController.Instance.Enemy_Set.Count >= 8 || GameSceneController.Instance.is_play_cg)
                        yield return new WaitForSeconds(1f);

                    if (boss == null || boss.enemy_data.cur_hp <= 0f)
                        break;

                    SpawnEnemyFromSource(spawnInfo, player.transform.position);

                    yield return new WaitForSeconds(0.3f);
                }

                yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Boss.line_interval);
            }
            yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Boss.wave_interval);
        }

        MissionFinished();

        yield return new WaitForSeconds(1f);
    }

    public void SpawnEnemyFromSource(EnemySpawnInfo spawnInfo, Vector3 playerPos, bool bypassLimit = false)
    {
        switch (spawnInfo.From)
        {
            case SpawnFromType.Grave:
                SpwanZombiesFromGrave(spawnInfo.EType, FindClosedGrave(playerPos), bypassLimit);
                break;
            case SpawnFromType.Nest:
                SpwanZombiesFromNest(spawnInfo.EType, zombie_nest_array[Random.Range(0, zombie_nest_array.Length)]);
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        if (!is_mission_finished)
        {
            lastCheckTime += Time.deltaTime;
            if (lastCheckTime >= checkRate)
            {
                lastCheckTime = 0f;
            }
        }
    }

    public EnemyType BossTypeSafe
    {
        get
        {
            if (boss == null)
            {
                Debug.LogWarning("[BossMissionController] Boss is null when trying to get BossTypeSafe.");
                return EnemyType.E_NONE;
            }
            return boss.enemyType;
        }
    }

    public override void CaculateDifficulty()
    {
    }

    public EnemyController SpawnBossFromNest(EnemyType type, GameObject nest)
    {
        if (nest == null)
        {
            Debug.LogError("Spawn boss failed: nest is null.");
            return null;
        }
        return EnemyFactory.CreateEnemyGetEnemyController(type, nest.transform.position, nest.transform.rotation);
    }

    public IEnumerator FatcookSummon(EnemyController boss)
    {
        if (boss == null)
        {
            Debug.LogWarning("FatcookSummon called with null boss");
            yield break;
        }

        EnemyType minionType = (boss.enemyType == EnemyType.E_FATCOOK_E)
            ? EnemyType.E_BOOMER_TIMER_E
            : EnemyType.E_BOOMER_TIMER;

        int minionCount = (boss.enemyType == EnemyType.E_FATCOOK_E) ? 12 : 8;

        GameObject grave = FindClosedGrave(boss.transform.position);
        for (int i = 0; i < minionCount; i++)
        {
            SpwanZombiesFromGrave(minionType, grave, true); // ← bypass limit
            yield return null;
        }
    }

    public IEnumerator HalloweenSummon(EnemyController boss)
    {
        if (boss == null)
        {
            Debug.LogWarning("HalloweenSummon called with null boss");
            yield break;
        }

        EnemyType minionType = (boss.enemyType == EnemyType.E_HALLOWEEN_E)
            ? EnemyType.E_HALLOWEEN_SUB_E
            : EnemyType.E_HALLOWEEN_SUB;

        int minionCount = (boss.enemyType == EnemyType.E_HALLOWEEN_E) ? 3 : 2;

        Debug.Log("[HalloweenSummon] Boss enemy type: " + boss.enemyType);

        GameObject grave = FindClosedGrave(boss.transform.position);
        for (int i = 0; i < minionCount; i++)
        {
            SpwanZombiesFromGrave(minionType, grave, true); // ← bypass limit
            yield return null;
        }
    }

    public IEnumerator SummonBossMinions(EnemyController boss)
    {
        if (boss == null)
        {
            Debug.LogWarning("SummonBossMinions called with null boss");
            yield break;
        }

        switch (boss.enemyType)
        {
            case EnemyType.E_HALLOWEEN:
            case EnemyType.E_HALLOWEEN_E:
                yield return StartCoroutine(HalloweenSummon(boss));
                break;

            case EnemyType.E_FATCOOK:
            case EnemyType.E_FATCOOK_E:
                yield return StartCoroutine(FatcookSummon(boss));
                break;

            default:
                Debug.Log("SummonBossMinions: No summon logic defined for boss type: " + boss.enemyType);
                break;
        }
    }

    private IEnumerator SummonMinionsByType(Vector3 centerPos, int minionCount, EnemyType minionType)
    {
        Debug.LogFormat("Summoning {0} minions of type {1} near position {2}", minionCount, minionType, centerPos);

        GameObject grave = FindClosedGrave(centerPos);
        if (grave == null)
        {
            Debug.LogWarning("SummonMinionsByType: No grave found near position " + centerPos);
            yield break;
        }

        for (int i = 0; i < minionCount; i++)
        {
            if (!EnemyMap.Instance.Enemy_Set.ContainsKey(minionType))
            {
                Debug.LogError("EnemyMap missing prefab for minion type: " + minionType);
                yield break;
            }
            bool bypassLimit = GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Endless;
            // Spawn minion explicitly by type at grave position
            SpwanZombiesFromGrave(minionType, grave, bypassLimit);

            yield return null;
        }
    }

    public IEnumerator BossSummonMinions()
    {
        yield return StartCoroutine(SummonBossMinions(this.boss));
    }

    public void TriggerBossSummon()
    {
        StartCoroutine(BossSummonMinions());
    }

    public void SetBoss(EnemyController assignedBoss)
    {
        boss = assignedBoss;
    }
}