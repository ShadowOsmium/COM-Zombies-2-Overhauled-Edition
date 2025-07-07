using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class HalloweenController : EnemyController
{
    private string ani_attack = "Zombie_Hook_Demon_Attack01";

    private string ani_damage_1 = "Zombie_Hook_Demon_Damage01";

    private string ani_damage_2 = "Zombie_Hook_Demon_Damage02";

    private string ani_damage_3 = "Zombie_Hook_Demon_Damage03";

    public string ANI_REPLICATION_01 = "Zombie_Hook_Demon_Attack02_01";

    public string ANI_REPLICATION_02 = "Zombie_Hook_Demon_Attack02_02";

    public string ANI_WINDSWORD = "Zombie_Hook_Demon_Attack03";

    public EnemyState SKILL_REPLICATION_STATE;

    public EnemyState SKILL_WINDSWORD_STATE;

    private float injured_val_state;

    private float injured_val_percent;

    private float cur_replication_time;

    private float replication_interval_time = 8f;

    public bool replication_enable = true;

    private float cur_windsword_time;

    private float windsword_interval_time = 8f;

    private float windsword_dmg_ratio = 1f;

    private float windsword_range = 10f;

    private float windsword_speed = 10f;

    public bool windsword_enable = true;

    private GameObject eff_bellow_obj;

    private ParticleSystem eff_bellow;

    private GameObject eff_angry_obj;

    private ParticleSystem eff_angry;

    private GameObject ground_stone1;

    private GameObject ground_stone2;

    private string stone_ani1 = string.Empty;

    private string stone_ani2 = string.Empty;

    public override Vector3 centroid
    {
        get
        {
            return base.transform.position + Vector3.up * 1.6f;
        }
    }

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public override void Init()
    {
        ANI_IDLE = "Zombie_Hook_Demon_Idle01";
        ANI_ATTACK = "Zombie_Hook_Demon_Attack01";
        ANI_INJURED = "Zombie_Hook_Demon_Damage01";
        ANI_DEAD = "Zombie_Hook_Demon_Death01";
        ANI_RUN = "Zombie_Hook_Demon_Run01";
        ANI_SHOW = "Zombie_Hook_Demon_Show01";
        ANI_HALF_HP = "Zombie_Hook_Demon_Bellow01";
        float time = 0f;
        if (base.IsBoss)
        {
            if (GameSceneController.Instance.enable_spawn_ani == "Hook_Demon_Camera_Show01")
            {
                ANI_SHOW = "Zombie_Hook_Demon_Show01";
                time = 0f;
                stone_ani1 = "Zombie_FatCook01";
                stone_ani2 = "Zombie_FatCook01";
            }
            else if (GameSceneController.Instance.enable_spawn_ani == "Hook_Demon_Camera_Show02")
            {
                ANI_SHOW = "Zombie_Hook_Demon_Show02";
                time = 1.8f;
                stone_ani1 = "01";
                stone_ani2 = "01";
            }
            CreateHpBar(Vector3.up * 3.4f);
        }
        Invoke("PlayGroundStone", time);
        base.Init();
        SKILL_REPLICATION_STATE = EnemyState.Create(EnemyStateType.HalloweenReplication, this);
        SKILL_WINDSWORD_STATE = EnemyState.Create(EnemyStateType.HalloweenWindSword, this);
        injured_val_percent = (float)enemy_data.config.Ex_conf["injuredPercent"];
        replication_interval_time = (float)enemy_data.config.Ex_conf["replicationCDTime"];
        windsword_range = (float)enemy_data.config.Ex_conf["WindSwordRange"];
        windsword_interval_time = (float)enemy_data.config.Ex_conf["WindSwordCDTime"];
        windsword_dmg_ratio = (float)enemy_data.config.Ex_conf["WindSwordDmgRatio"];
        windsword_speed = (float)enemy_data.config.Ex_conf["WindSwordSpeed"];
        eff_bellow_obj = base.transform.Find("fatcook_houjiao").gameObject;
        eff_bellow = eff_bellow_obj.GetComponent<ParticleSystem>();
        eff_bellow.Stop();
        eff_angry_obj = base.transform.Find("crazy_boss").gameObject;
        eff_angry = eff_angry_obj.GetComponent<ParticleSystem>();
        eff_angry.Stop();
        SetState(SHOW_STATE);
    }

    public override void CheckHit()
    {
        if (enemyState == SHOOT_STATE)
        {
            foreach (PlayerController value in GameSceneController.Instance.Player_Set.Values)
            {
                if ((value.centroid - centroid).sqrMagnitude <= (enemy_data.attack_range + 0.5f) * (enemy_data.attack_range + 0.5f) && !GameSceneController.CheckBlockBetween(centroid, value.centroid) && base.transform.InverseTransformPoint(value.centroid).z > 0f)
                {
                    value.OnHit(enemy_data.damage_val, null, this, value.centroid, Vector3.up);
                }
            }
            foreach (NPCController value2 in GameSceneController.Instance.NPC_Set.Values)
            {
                if ((value2.centroid - centroid).sqrMagnitude <= (enemy_data.attack_range + 0.5f) * (enemy_data.attack_range + 0.5f) && !GameSceneController.CheckBlockBetween(centroid, value2.centroid) && base.transform.InverseTransformPoint(value2.centroid).z > 0f)
                {
                    value2.OnHit(enemy_data.damage_val, null, this, value2.centroid, Vector3.up);
                }
            }
            foreach (GuardianForceController value3 in GameSceneController.Instance.GuardianForce_Set.Values)
            {
                if ((value3.centroid - centroid).sqrMagnitude <= (enemy_data.attack_range + 0.5f) * (enemy_data.attack_range + 0.5f) && !GameSceneController.CheckBlockBetween(centroid, value3.centroid) && base.transform.InverseTransformPoint(value3.centroid).z > 0f)
                {
                    value3.OnHit(enemy_data.damage_val, null, this, value3.centroid, Vector3.up);
                }
            }
            {
                foreach (EnemyController value4 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
                {
                    if ((value4.centroid - centroid).sqrMagnitude <= (enemy_data.attack_range + 0.5f) * (enemy_data.attack_range + 0.5f) && !GameSceneController.CheckBlockBetween(centroid, value4.centroid) && base.transform.InverseTransformPoint(value4.centroid).z > 0f)
                    {
                        value4.OnHit(enemy_data.damage_val, null, this, value4.centroid, Vector3.up);
                    }
                }
                return;
            }
        }
        if (enemyState == SKILL_WINDSWORD_STATE)
        {
            Vector3[] array = new Vector3[3];
            array[1] = base.transform.forward;
            array[0] = Quaternion.Euler(0f, 25f, 0f) * array[1];
            array[2] = Quaternion.Euler(0f, -25f, 0f) * array[1];
            for (int i = 0; i < array.Length; i++)
            {
                GameObject gameObject = Object.Instantiate(base.Accessory[2], base.transform.position, Quaternion.identity) as GameObject;
                HalloweenProjectile component = gameObject.GetComponent<HalloweenProjectile>();
                component.launch_dir = array[i];
                component.fly_speed = windsword_speed;
                component.life = 4f;
                component.object_controller = this;
                component.launch_speed = component.fly_speed * component.launch_dir;
                component.damage = enemy_data.damage_val * windsword_dmg_ratio;
            }
        }
    }

    public void ResetInjureAni(Vector3 hit_point)
    {
    }

    public override void FireUpdate(float deltaTime)
    {
        if (AnimationUtil.IsPlayingAnimation(base.gameObject, ani_attack))
        {
            AlwaysLookAtTarget();
        }
        if (AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ANI_ATTACK, 1f))
        {
            SetState(AFTER_SHOOT_STATE);
        }
    }

    public override void Fire()
    {
        ANI_CUR_ATTACK = (ANI_ATTACK = ani_attack);
        AnimationUtil.CrossAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
    }

    public override void EnterAttack()
    {
        Fire();
    }

    public override void OnDead(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
    {
        base.OnDead(damage, weapon, player, hit_point, hit_normal);
        Object.Destroy(base.GetComponent<Collider>());
        StartCoroutine(RemoveOnTime(3f));
        eff_angry.Stop();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Dologic(float deltaTime)
    {
        base.Dologic(deltaTime);
        if (!replication_enable)
        {
            cur_replication_time += deltaTime;
            if (cur_replication_time >= replication_interval_time)
            {
                cur_replication_time = 0f;
                replication_enable = true;
            }
        }
        if (!windsword_enable)
        {
            cur_windsword_time += deltaTime;
            if (cur_windsword_time >= windsword_interval_time)
            {
                cur_windsword_time = 0f;
                windsword_enable = true;
            }
        }
    }

    public override void DetermineNormalState()
    {
        CheckTargetPlayer();
        if (target_player == null)
        {
            SetState(IDLE_STATE);
        }
        else if (!((BossHalfHpState)HALF_HP_STATE).IsPlayed && enemy_data.cur_hp / enemy_data.hp_capacity <= 0.5f)
        {
            SetState(HALF_HP_STATE);
        }
        else if (CouldEnterWindSwordState())
        {
            SetState(SKILL_WINDSWORD_STATE);
        }
        else if (CouldEnterReplicationState())
        {
            SetState(SKILL_REPLICATION_STATE);
        }
        else if (CouldEnterAttackState())
        {
            SetState(SHOOT_STATE);
        }
        else
        {
            SetState(CATCHING_STATE);
        }
    }

    public override void OnHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
    {
        if (enemyState == SHOW_STATE || enemyState == DEAD_STATE)
        {
            return;
        }
        injured_time = Time.time;
        if (!hatred_set.ContainsKey(player))
        {
            hatred_set.Add(player, 1f);
        }
        Dictionary<ObjectController, float> dictionary;
        Dictionary<ObjectController, float> dictionary2 = (dictionary = hatred_set);
        ObjectController key;
        ObjectController key2 = (key = player);
        float num = dictionary[key];
        dictionary2[key2] = num + damage;
        //if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && damage > 0f && tnetObj != null)
        //{
        //	SFSObject sFSObject = new SFSObject();
        //	SFSArray sFSArray = new SFSArray();
        //	sFSArray.AddShort((short)enemy_id);
        //	sFSArray.AddFloat(damage);
        //	sFSArray.AddBool(false);
        //	sFSArray.AddFloat(0f);
        //	sFSObject.PutSFSArray("enemyInjured", sFSArray);
        //	tnetObj.Send(new BroadcastMessageRequest(sFSObject));
        //	Dictionary<PlayerID, float> player_damage_Set;
        //	Dictionary<PlayerID, float> dictionary3 = (player_damage_Set = GameSceneController.Instance.Player_damage_Set);
        //	PlayerID player_id;
        //	PlayerID key3 = (player_id = GameSceneController.Instance.player_controller.player_id);
        //	num = player_damage_Set[player_id];
        //	dictionary3[key3] = num + damage;
        //}
        if (enemy_data.OnInjured(damage, player.GetComponent<ObjectController>()))
        {
            GameSceneController.Instance.UpdateEnemyDeathInfo(enemy_data.enemy_type, 1);
            OnDead(damage, weapon, player, hit_point, hit_normal);
            SetState(DEAD_STATE);
            //if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && damage > 0f && tnetObj != null)
            //{
            //	SFSObject sFSObject2 = new SFSObject();
            //	SFSArray sFSArray2 = new SFSArray();
            //	sFSArray2.AddShort((short)enemy_id);
            //	sFSArray2.AddFloat(damage);
            //	sFSObject2.PutSFSArray("enemyDead", sFSArray2);
            //	tnetObj.Send(new BroadcastMessageRequest(sFSObject2));
            //}
        }
        else
        {
            injured_val_state += damage;
            if (injured_val_state / enemy_data.hp_capacity >= injured_val_percent && base.Enemy_State.GetStateType() == EnemyStateType.Catching)
            {
                switch (Random.Range(0, 100) % 3)
                {
                    case 0:
                        ANI_INJURED = ani_damage_1;
                        break;
                    case 1:
                        ANI_INJURED = ani_damage_2;
                        break;
                    default:
                        ANI_INJURED = ani_damage_3;
                        break;
                }
                AnimationUtil.Stop(base.gameObject);
                AnimationUtil.CrossAnimate(base.gameObject, ANI_INJURED, WrapMode.ClampForever);
                SetState(INJURED_STATE);
            }
        }
        CreateInjuredBloodEff(hit_point, hit_normal);
        UpdateCurHpBar();
    }

    public bool CouldEnterWindSwordState()
    {
        if (windsword_enable && base.SqrDistanceFromPlayer <= windsword_range * windsword_range && !GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
        {
            return true;
        }
        return false;
    }

    public bool CouldEnterReplicationState()
    {
        if (replication_enable)
        {
            return true;
        }
        return false;
    }

    public override void FixedUpdate()
    {
    }

    public override void SetState(EnemyState state)
    {
        if (enemyState != null && enemyState.GetStateType() != state.GetStateType())
        {
            enemyState.OnExitState();
            enemyState = state;
            enemyState.OnEnterState();
            injured_val_state = 0f;
        }
    }

    public override bool OnChaisawSpecialHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
    {
        return false;
    }

    public override void OnChaisawInjuredResease(ObjectController player)
    {
    }

    public override void OnChaisawSkillDead(ObjectController player)
    {
    }

    public override void OnRepel(Vector3 dir)
    {
    }

    public override void RepelOver()
    {
    }

    public override void OnShowUnderGround()
    {
        if (!(autio_controller != null))
        {
        }
    }

    private void HideGroundStone()
    {
        Object.Destroy(ground_stone1);
        Object.Destroy(ground_stone2);
    }

    public override void PlayHalfHpEffect()
    {
        base.PlayHalfHpEffect();
        eff_bellow.Play();
        eff_angry.Play();
        Transform parent = base.transform.Find("cg_view");
        Camera.main.transform.parent = parent;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
        GameSceneController.Instance.main_camera.camera_pause = true;
        GameSceneController.Instance.main_camera.StartShake("Hook_Demon_Camera_Bellow01");
        float length = GameSceneController.Instance.main_camera.shake_obj.GetComponent<Animation>()["Hook_Demon_Camera_Bellow01"].length;
        GameSceneController.Instance.player_controller.StartLimitMove(length);
        GameSceneController.Instance.StartCgLimit(length);
        foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
        {
            if (!value.IsBoss)
            {
                value.StartLimitMove(length);
            }
        }
    }

    public override void OnHalfHpEffOver()
    {
        base.OnHalfHpEffOver();
        eff_bellow.Stop();
    }
    public void OnReplicationCast()
    {
        EnemyController selfController = GetComponent<EnemyController>();
        MissionController missionController = GameSceneController.Instance.mission_controller;

        BossCoopMissionController coopMission = missionController as BossCoopMissionController;
        if (coopMission != null)
        {
            // Pass the boss controller to get correct elite minion spawning
            coopMission.StartCoroutine(coopMission.HalloweenSummon(selfController));
            return;
        }

        BossMissionController offlineMission = missionController as BossMissionController;
        if (offlineMission != null)
        {
            offlineMission.SetBoss(selfController);
            // Use the overload with EnemyController parameter for consistent elite minion spawning
            offlineMission.StartCoroutine(offlineMission.HalloweenSummon(selfController));
            return;
        }

        Debug.LogWarning("OnReplicationCast: mission controller type unknown, cannot summon minions.");
    }

    private void PlayGroundStone()
    {
        ground_stone1 = Object.Instantiate(base.Accessory[0], base.transform.position, base.transform.rotation) as GameObject;
        ground_stone2 = Object.Instantiate(base.Accessory[1], base.transform.position, base.transform.rotation) as GameObject;
        AnimationUtil.PlayAnimate(ground_stone1, stone_ani1, WrapMode.Once);
        AnimationUtil.PlayAnimate(ground_stone2, stone_ani2, WrapMode.Once);
        Invoke("HideGroundStone", base.GetComponent<Animation>()[ANI_SHOW].length + 2f);
    }
}
