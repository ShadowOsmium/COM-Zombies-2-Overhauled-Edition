using System.Collections.Generic;
using CoMZ2;
using TNetSdk;
using UnityEngine;

public class BoomerController : EnemyController
{
    private string ani_run01 = "Forward01";

    private string ani_run02 = "Forward_SpeedUP01";

    protected float speed_up_ratio = 1f;

    protected float explodeRange;

    public override void Init()
    {
        ANI_SHOW = "Show01";
        ANI_IDLE = "Forward01";
        ANI_ATTACK = "Explode01";
        ANI_INJURED = "Damage01";
        ANI_DEAD = "Death01";
        ANI_RUN = "Forward01";
        base.Init();
        speed_up_ratio = (float)enemy_data.config.Ex_conf["speedUpRatio"];
        explodeRange = (float)enemy_data.config.Ex_conf["explodeRange"];
        RandomRunAnimation();
        head_ori = base.transform.Find("Zombie_Self_H_Head").gameObject;
        if (head_ori == null)
        {
            Debug.LogError("head_ori not found!");
        }
        body_ori = base.transform.Find("Zombie_Self_H_Body").gameObject;
        if (body_ori == null)
        {
            Debug.LogError("body_ori not founded!");
        }
        neck_ori = base.transform.Find("Zombie_Self_H_Neck").gameObject;
        if (neck_ori == null)
        {
            Debug.LogError("neck_ori not founded!");
        }
        neck_ori.SetActive(false);
        head_broken = base.transform.Find("Zombie_Broken_Self_Head01").gameObject;
        if (head_broken == null)
        {
            Debug.LogError("head_broken not founded!");
        }
        head_broken.SetActive(false);
        body_eff_prefab = base.Accessory[0];
        head_broken_eff_prefab = base.Accessory[1];
        SetState(SHOW_STATE);
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

    public override void CheckHit()
    {
        if (base.IsEnchant)
        {
            foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
            {
                if ((value.centroid - centroid).sqrMagnitude < explodeRange * explodeRange && !GameSceneController.CheckBlockBetween(centroid, value.centroid))
                {
                    value.OnHit(enemy_data.damage_val, null, this, value.centroid, Vector3.zero);
                }
            }
        }
        else
        {
            foreach (PlayerController value2 in GameSceneController.Instance.Player_Set.Values)
            {
                if ((value2.centroid - centroid).sqrMagnitude < explodeRange * explodeRange && !GameSceneController.CheckBlockBetween(centroid, value2.centroid))
                {
                    value2.OnHit(enemy_data.damage_val, null, this, value2.centroid, Vector3.zero);
                }
            }
            foreach (NPCController value3 in GameSceneController.Instance.NPC_Set.Values)
            {
                if ((value3.centroid - centroid).sqrMagnitude < explodeRange * explodeRange && !GameSceneController.CheckBlockBetween(centroid, value3.centroid))
                {
                    value3.OnHit(enemy_data.damage_val, null, this, value3.centroid, Vector3.zero);
                }
            }
            foreach (GuardianForceController value4 in GameSceneController.Instance.GuardianForce_Set.Values)
            {
                if ((value4.centroid - centroid).sqrMagnitude < explodeRange * explodeRange && !GameSceneController.CheckBlockBetween(centroid, value4.centroid))
                {
                    value4.OnHit(enemy_data.damage_val, null, this, value4.centroid, Vector3.zero);
                }
            }
            foreach (EnemyController value5 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
            {
                if ((value5.centroid - centroid).sqrMagnitude < explodeRange * explodeRange && !GameSceneController.CheckBlockBetween(centroid, value5.centroid))
                {
                    value5.OnHit(enemy_data.damage_val, null, this, value5.centroid, Vector3.zero);
                }
            }
        }
        OnBoom();
        SetState(DEAD_STATE);
        GameSceneController.Instance.boom_m_pool.GetComponent<ObjectPool>().CreateObject(centroid, Quaternion.identity);
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
        OnHitSound(weapon);
        //if (GameData.Instance.cur_game_type == GameData.GamePlayType.Coop && damage > 0f && tnetObj != null)
        //{
        //	SFSObject sFSObject = new SFSObject();
        //	SFSArray sFSArray = new SFSArray();
        //	sFSArray.AddShort((short)enemy_id);
        //	sFSArray.AddFloat(damage);
        //	if (weapon != null && weapon.weapon_type == WeaponType.IceGun)
        //	{
        //		tem_frozenTime = ((IceGunController)weapon).frozenTime;
        //		sFSArray.AddBool(true);
        //		sFSArray.AddFloat(tem_frozenTime);
        //	}
        //	else
        //	{
        //		sFSArray.AddBool(false);
        //		sFSArray.AddFloat(0f);
        //	}
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
            if (weapon != null && weapon.weapon_type == WeaponType.IceGun)
            {
                is_ice_dead = true;
                PlayPlayerAudio("FreezeBurst");
            }
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
        else if (weapon != null && weapon.weapon_type == WeaponType.IceGun)
        {
            frozenTime = ((IceGunController)weapon).frozenTime;
            AnimationUtil.Stop(base.gameObject);
            SetState(FROZEN_STATE);
        }
        else
        {
            if (base.Enemy_State.GetStateType() != EnemyStateType.Shoot)
            {
                AnimationUtil.Stop(base.gameObject);
                AnimationUtil.PlayAnimate(base.gameObject, ANI_INJURED, WrapMode.ClampForever);
                SetState(INJURED_STATE);
            }
            CreateInjuredBloodEff(hit_point, hit_normal);
        }
    }

    public override void FireUpdate(float deltaTime)
    {
        if (target_player != null)
        {
            base.transform.LookAt(target_player.transform);
        }
        if (AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ANI_ATTACK, 0.95f))
        {
            CheckHit();
        }
    }

    public override void Fire()
    {
        ANI_CUR_ATTACK = ANI_ATTACK;
        AnimationUtil.CrossAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
    }

    public override void OnDead(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
    {
        base.OnDead(damage, weapon, player, hit_point, hit_normal);
        if (is_ice_dead)
        {
            OnIceBodyCrash();
        }
        else
        {
            RandomHeadBrokenFall(hit_point, hit_normal);
        }
        Object.Destroy(base.GetComponent<Collider>());
        StartCoroutine(RemoveOnTime(3f));
    }

    public void OnBoom()
    {
        Object.Destroy(base.GetComponent<Collider>());
        StartCoroutine(RemoveOnTime(0.1f));
    }

    public override bool FinishAttackAni()
    {
        if (AnimationUtil.IsPlayingAnimation(base.gameObject, ANI_ATTACK))
        {
            return false;
        }
        return true;
    }

    protected void RandomRunAnimation()
    {
        switch (Random.Range(0, 100) % 2)
        {
            case 0:
                ANI_RUN = ani_run01;
                break;
            case 1:
                ANI_RUN = ani_run02;
                nav_pather.SetSpeed(enemy_data.move_speed * speed_up_ratio);
                break;
        }
    }
}