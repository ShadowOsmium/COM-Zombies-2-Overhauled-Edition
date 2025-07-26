using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class BossCoopMissionController : MissionController
{
    private float last_check_time;
    private float check_rate = 1f;

    public EnemyController boss;

    public EnemyController MissionBoss
    {
        get
        {
            return boss;
        }
    }

    public void StartMinionSummon()
    {
        StartCoroutine(SummonBossMinions(boss));
    }

    public override List<EnemyType> GetMissionEnemyTypeList()
    {
        CoopBossCfg coopBossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[
            GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];

        List<EnemyWaveInfo> waveList = null;
        // Get highest level wave list less or equal to boss day level
        int maxKey = -1;
        foreach (int key in GameConfig.Instance.EnemyWaveInfo_Boss_Coop_Set.Keys)
            if (key > maxKey) maxKey = key;

        if (maxKey != -1)
            waveList = GameConfig.Instance.EnemyWaveInfo_Boss_Coop_Set[maxKey].wave_info_list;

        List<EnemyType> result = new List<EnemyType>();
        if (waveList != null)
        {
            foreach (EnemyWaveInfo wave in waveList)
                foreach (EnemySpawnInfo spawnInfo in wave.spawn_info_list)
                    if (!result.Contains(spawnInfo.EType))
                        result.Add(spawnInfo.EType);
        }
        result.Add(coopBossCfg.boss_type);

        // Add minions explicitly if missing
        CoopBossType bossType = GameData.Instance.cur_coop_boss;
        if (bossType == CoopBossType.E_HALLOWEEN && !result.Contains(EnemyType.E_HALLOWEEN_SUB))
            result.Add(EnemyType.E_HALLOWEEN_SUB);
        else if (bossType == CoopBossType.E_FATCOOK && !result.Contains(EnemyType.E_BOOMER_TIMER))
            result.Add(EnemyType.E_BOOMER_TIMER);
        else if (bossType == CoopBossType.E_HALLOWEEN_E && !result.Contains(EnemyType.E_HALLOWEEN_SUB_E))
            result.Add(EnemyType.E_HALLOWEEN_SUB_E);
        else if (bossType == CoopBossType.E_FATCOOK_E && !result.Contains(EnemyType.E_BOOMER_TIMER_E))
            result.Add(EnemyType.E_BOOMER_TIMER_E);

        return result;
    }

    public override IEnumerator Start()
    {
        InitMissionController();
        mission_type = MissionType.Coop;
        GameData.Instance.cur_quest_info.mission_type = MissionType.Coop;
        CaculateDifficulty();

        var types = GetMissionEnemyTypeList();
        GameSceneController.Instance.enemy_ref_map.ResetEnemyMapInfo(types);

        yield return null;

        PlayerController player = null;
        while ((player = GameSceneController.Instance.player_controller) == null)
            yield return null;

        CoopBossCfg bossCfg = GameConfig.Instance.Coop_Boss_Cfg_Set[
            GameConfig.GetEnemyTypeFromBossType(GameData.Instance.cur_coop_boss)];
        GameSceneController.Instance.game_main_panel.boss_panel.SetIcon(bossCfg.boss_name + "_icon_s");
        GameSceneController.Instance.game_main_panel.boss_panel.SetContent("0 / 1");

        List<EnemyWaveInfo> waveList = null;
        foreach (int level in GameConfig.Instance.EnemyWaveInfo_Boss_Coop_Set.Keys)
        {
            if (bossCfg.day_level <= level)
            {
                waveList = GameConfig.Instance.EnemyWaveInfo_Boss_Coop_Set[level].wave_info_list;
                break;
            }
        }

        if (waveList == null)
        {
            Debug.LogWarning("[BossCoop] No waves found for boss level.");
            yield break;
        }

        yield return new WaitForSeconds(1f);

        int spawnCountBeforeBoss = 4;
        int spawned = 0;
        while (spawned < spawnCountBeforeBoss)
        {
            EnemyWaveInfo wave = waveList[Random.Range(0, waveList.Count)];

            foreach (EnemySpawnInfo spawnInfo in wave.spawn_info_list)
            {
                if (spawned >= spawnCountBeforeBoss)
                    break;

                if (GameSceneController.Instance.Enemy_Set.Count >= 8)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                if (spawnInfo.From == SpawnFromType.Grave)
                {
                    GameObject grave = FindClosedGrave(player.transform.position);
                    if (grave != null)
                        SpwanZombiesFromGrave(spawnInfo.EType, grave, false);
                }
                else if (spawnInfo.From == SpawnFromType.Nest)
                {
                    GameObject nest = zombie_nest_array[Random.Range(0, zombie_nest_array.Length)];
                    SpwanZombiesFromNest(spawnInfo.EType, nest);
                }
                spawned++;
                yield return new WaitForSeconds(1f);
            }
        }

        yield return new WaitForSeconds(5f);

        if (boss == null)
        {
            GameObject bossNest = zombie_boss_array[Random.Range(0, zombie_boss_array.Length)];
            boss = SpawnBossFromNest(bossCfg.boss_type, bossNest);
            boss.SetEnemyBeCoopBoss(bossCfg);
            GameSceneCoopController.Instance.OnBossBirthCameraShow(boss);
        }

        // Continue spawning regular waves while boss is alive
        while (boss != null && boss.enemy_data.cur_hp > 0f)
        {
            EnemyWaveInfo wave = waveList[Random.Range(0, waveList.Count)];
            foreach (EnemySpawnInfo spawnInfo in wave.spawn_info_list)
            {
                if (boss == null || boss.enemy_data.cur_hp <= 0f)
                    break;

                if (GameSceneController.Instance.Enemy_Set.Count >= 8)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                if (spawnInfo.From == SpawnFromType.Grave)
                {
                    GameObject grave = FindClosedGrave(player.transform.position);
                    if (grave != null)
                        SpwanZombiesFromGrave(spawnInfo.EType, grave, false);
                }
                else if (spawnInfo.From == SpawnFromType.Nest)
                {
                    GameObject nest = zombie_nest_array[Random.Range(0, zombie_nest_array.Length)];
                    SpwanZombiesFromNest(spawnInfo.EType, nest);
                }

                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(GameConfig.Instance.EnemyWave_Interval_Boss_Coop.wave_interval);
        }

        FinishMissionWithDelay(2f);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void CaculateDifficulty()
    {
    }

    public EnemyController SpawnBossFromNest(EnemyType type, GameObject nest)
    {
        if (nest == null)
        {
            Debug.LogError("[BossCoop] Boss nest is null.");
            return null;
        }
        EnemyController enemy = EnemyFactory.CreateEnemyGetEnemyController(type, nest.transform.position, nest.transform.rotation);
        if (enemy == null)
        {
            Debug.LogError("[BossCoop] Failed to spawn boss: " + type);
            return null;
        }
        enemy.gameObject.SetActive(true);
        if (!GameSceneController.Instance.Enemy_Set.ContainsKey(enemy.EnemyID))
            GameSceneController.Instance.Enemy_Set.Add(enemy.EnemyID, enemy);
        return enemy;
    }

    public override EnemyController SpwanZombiesFromGrave(EnemyType type, GameObject grave, bool bypassSpawnLimit)
    {
        if (grave == null)
        {
            Debug.LogError("Spawn zombie from grave failed: grave is null.");
            return null;
        }

        // Random position in grave bounds
        float x = Random.Range(-grave.transform.localScale.x / 2f, grave.transform.localScale.x / 2f);
        float z = Random.Range(-grave.transform.localScale.z / 2f, grave.transform.localScale.z / 2f);
        Vector3 spawnPos = grave.transform.position + new Vector3(x, 0f, z);

        EnemyController enemyController = EnemyFactory.CreateEnemyGetEnemyController(type, spawnPos, Quaternion.identity);
        if (enemyController == null)
        {
            Debug.LogError("Failed to spawn enemy: " + type);
            return null;
        }

        enemyController.gameObject.SetActive(true);

        if (!GameSceneController.Instance.Enemy_Set.ContainsKey(enemyController.EnemyID))
        {
            GameSceneController.Instance.Enemy_Set.Add(enemyController.EnemyID, enemyController);
        }

        // If bypassSpawnLimit is true, mark enemy to ignore spawn cap
        enemyController.ignoreSpawnLimit = bypassSpawnLimit;

        // Assign missionWeight property
        enemyController.MissionWeight = GameConfig.Instance.EnemyConfig_Set[type].missionWeight;

        if (type != EnemyType.E_FATCOOK && type != EnemyType.E_HAOKE_A && type != EnemyType.E_HAOKE_B
            && GameData.Instance.cur_quest_info.mission_type != MissionType.Tutorial)
        {
            GameSceneController.Instance.ground_stone_pool.GetComponent<ObjectPool>().CreateObject(spawnPos, Quaternion.identity);
        }

        return enemyController;
    }

    public float SpwanZombiesFromNest(EnemyType type, GameObject nest)
    {
        if (nest == null)
            return 0f;

        EnemyController enemy = EnemyFactory.CreateEnemyGetEnemyController(type, nest.transform.position, nest.transform.rotation);
        if (enemy == null)
            return 0f;

        enemy.gameObject.SetActive(true);
        if (!GameSceneController.Instance.Enemy_Set.ContainsKey(enemy.EnemyID))
            GameSceneController.Instance.Enemy_Set.Add(enemy.EnemyID, enemy);

        return GameConfig.Instance.EnemyConfig_Set[type].missionWeight;
    }

    public IEnumerator FatcookSummon(EnemyController boss)
    {
        if (boss == null)
        {
            Debug.LogWarning("FatcookSummon called with null boss");
            yield break;
        }
        EnemyType minionType = (boss.enemy_data.enemy_type == EnemyType.E_FATCOOK_E)
            ? EnemyType.E_BOOMER_TIMER_E
            : EnemyType.E_BOOMER_TIMER;
        int minionCount = (boss.enemy_data.enemy_type == EnemyType.E_FATCOOK_E) ? 12 : 8;

        GameObject grave = FindClosedGrave(boss.transform.position);
        for (int i = 0; i < minionCount; i++)
        {
            SpwanZombiesFromGrave(minionType, grave, false);
            yield return null;
        }
    }

    public IEnumerator HalloweenSummon(EnemyController summoningBoss)
    {
        if (summoningBoss == null)
        {
            Debug.LogWarning("HalloweenSummon called with null boss");
            yield break;
        }
        EnemyType minionType = (summoningBoss.enemy_data.enemy_type == EnemyType.E_HALLOWEEN_E)
            ? EnemyType.E_HALLOWEEN_SUB_E
            : EnemyType.E_HALLOWEEN_SUB;
        int minionCount = (summoningBoss.enemy_data.enemy_type == EnemyType.E_HALLOWEEN_E) ? 3 : 2;

        GameObject grave = FindClosedGrave(summoningBoss.transform.position);
        for (int i = 0; i < minionCount; i++)
        {
            SpwanZombiesFromGrave(minionType, grave, false);
            yield return null;
        }
    }

    public void SetBoss(EnemyController assignedBoss)
    {
        if (assignedBoss == null)
            return;
        // Prevent overwriting Halloween boss with something else
        if (boss != null && (boss.enemyType == EnemyType.E_HALLOWEEN || boss.enemyType == EnemyType.E_HALLOWEEN_E))
            return;

        boss = assignedBoss;
        Debug.Log("Boss set to: " + (boss != null ? boss.enemyType.ToString() : "null"));
    }

    public void FinishMissionWithDelay(float delaySeconds)
    {
        StartCoroutine(DelayedMissionFinish(delaySeconds));
    }

    private IEnumerator DelayedMissionFinish(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        MissionFinished();
    }
}