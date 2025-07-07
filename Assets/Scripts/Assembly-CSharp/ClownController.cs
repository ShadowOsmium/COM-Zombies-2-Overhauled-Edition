using CoMZ2;
using UnityEngine;

public class ClownController : EnemyController
{
    private string ani_attack01 = "Attack02";

    private string ani_attack02 = "Attack01";

    private string ani_injure01 = "damage01";

    private string ani_injure02 = "damage02";

    private string ani_injure03 = "damage03";

    private string ani_dead01 = "Death01";

    private string ani_dead02 = "Death02";

    private string ani_dead03 = "Death03";

    private string ani_dead04 = "Death04";

    private float injured_ani_val = 0.1f;

    protected Collider attackCollider;

    protected Transform mouth_trans;

    private float special_attack_range = 5f;

    private float damageRatio = 1f;

    private bool is_enter_special_attack;

    private Vector3 fire_target = Vector3.zero;

    private bool is_firing;

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
        ANI_IDLE = "Walk01";
        ANI_ATTACK = "Attack02";
        ANI_INJURED = ani_injure01;
        ANI_DEAD = "Death01";
        ANI_RUN = "Forward01";
        ANI_CHAISAW_INJURED = "damage_OT01";
        base.Init();
        attackCollider = base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head").gameObject.GetComponent<Collider>();
        if (attackCollider == null)
        {
            Debug.LogError("attack collider not founded!");
        }
        mouth_trans = base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm");
        if (mouth_trans == null)
        {
            Debug.LogError("mouth_trans not found!");
        }
        head_ori = base.transform.Find("Zombie_Clown_H_Head").gameObject;
        if (head_ori == null)
        {
            Debug.LogError("head_ori not found!");
        }
        neck_ori = base.transform.Find("Zombie_Clown_H_Neck").gameObject;
        if (neck_ori == null)
        {
            Debug.LogError("neck_ori not founded!");
        }
        neck_ori.SetActive(false);
        body_ori = base.transform.Find("Zombie_Clown_H_Body").gameObject;
        if (body_ori == null)
        {
            Debug.LogError("body_ori not founded!");
        }
        head_broken = base.transform.Find("Zombie_Broken_Clown_Head01").gameObject;
        if (head_broken == null)
        {
            Debug.LogError("head_broken not founded!");
        }
        head_broken.SetActive(false);
        if (base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/blood_baotou") != null)
        {
            neck_blood_obj = base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/blood_baotou").gameObject;
        }
        body_eff_prefab = base.Accessory[1];
        head_broken_eff_prefab = base.Accessory[2];
        special_attack_range = (float)enemy_data.config.Ex_conf["specailAttackRange"];
        damageRatio = (float)enemy_data.config.Ex_conf["damageRatio"];
        SetState(SHOW_STATE);
    }

    public override void CheckHit()
    {
        if (AnimationUtil.IsPlayingAnimation(base.gameObject, ani_attack01))
        {
            if (base.IsEnchant)
            {
                foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
                {
                    Collider collider = value.gameObject.GetComponent<Collider>();
                    if (collider != null && attackCollider.bounds.Intersects(collider.bounds))
                    {
                        value.OnHit(enemy_data.damage_val, null, this, value.centroid, Vector3.zero);
                    }
                }
                return;
            }
            foreach (PlayerController value2 in GameSceneController.Instance.Player_Set.Values)
            {
                Collider collider2 = value2.gameObject.GetComponent<Collider>();
                if (collider2 != null && attackCollider.bounds.Intersects(collider2.bounds))
                {
                    value2.OnHit(enemy_data.damage_val, null, this, value2.centroid, Vector3.zero);
                }
            }
            foreach (NPCController value3 in GameSceneController.Instance.NPC_Set.Values)
            {
                Collider collider3 = value3.gameObject.GetComponent<Collider>();
                if (collider3 != null && attackCollider.bounds.Intersects(collider3.bounds))
                {
                    value3.OnHit(enemy_data.damage_val, null, this, value3.centroid, Vector3.zero);
                }
            }
            foreach (GuardianForceController value4 in GameSceneController.Instance.GuardianForce_Set.Values)
            {
                Collider collider4 = value4.gameObject.GetComponent<Collider>();
                if (collider4 != null && attackCollider.bounds.Intersects(collider4.bounds))
                {
                    value4.OnHit(enemy_data.damage_val, null, this, value4.centroid, Vector3.zero);
                }
            }
            {
                foreach (EnemyController value5 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
                {
                    Collider collider5 = value5.gameObject.GetComponent<Collider>();
                    if (collider5 != null && attackCollider.bounds.Intersects(collider5.bounds))
                    {
                        value5.OnHit(enemy_data.damage_val, null, this, value5.centroid, Vector3.zero);
                    }
                }
                return;
            }
        }
        if (AnimationUtil.IsPlayingAnimation(base.gameObject, ani_attack02))
        {
            float num = 0f;
            Vector3 vector = fire_target - mouth_trans.transform.position;
            float magnitude = vector.magnitude;
            float num2 = 12f;
            float num3 = magnitude / num2;
            float num4 = (num - 0.5f * Physics.gravity.y * num3 * num3) / num3;
            Vector3 vector2 = Vector3.up * num4 + vector.normalized * num2;
            GameObject gameObject = Object.Instantiate(base.Accessory[0], mouth_trans.transform.position, base.transform.rotation) as GameObject;
            if (base.IsEnchant)
            {
                gameObject.layer = PhysicsLayer.PLAYER_PROJECTILE;
            }
            ProjectileController component = gameObject.GetComponent<ProjectileController>();
            component.launch_dir = vector2;
            component.launch_speed = vector2;
            component.explode_radius = 2f;
            component.damage = enemy_data.damage_val * damageRatio;
            component.object_controller = this;
        }
    }

    public override void OnHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
    {
        ResetInjureAni(hit_point);
        base.OnHit(damage, weapon, player, hit_point, hit_normal);
    }

    public void ResetInjureAni(Vector3 hit_point)
    {
        switch (Random.Range(0, 100) % 3)
        {
            case 0:
                ANI_INJURED = ani_injure02;
                break;
            case 1:
                ANI_INJURED = ani_injure01;
                break;
            case 2:
                ANI_INJURED = ani_injure03;
                break;
        }
    }

    public override void FireUpdate(float deltaTime)
    {
        if (AnimationUtil.IsPlayingAnimation(base.gameObject, ani_attack01))
        {
            if (target_player != null)
            {
                base.transform.LookAt(target_player.transform);
            }
        }
        else if (AnimationUtil.IsPlayingAnimation(base.gameObject, ani_attack02) && target_player != null && !is_firing)
        {
            Quaternion to = Quaternion.LookRotation(target_player.transform.position - base.transform.position, Vector3.up);
            base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 360f * deltaTime);
        }
        if (AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ANI_ATTACK, 1f))
        {
            SetState(AFTER_SHOOT_STATE);
        }
    }

    public override void Fire()
    {
        ANI_CUR_ATTACK = ANI_ATTACK;
        AnimationUtil.CrossAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
        is_firing = true;
        if (target_player != null)
        {
            base.transform.LookAt(target_player.transform);
        }
        if (target_player != null)
        {
            fire_target = target_player.centroid;
        }
        else
        {
            fire_target = centroid;
        }
    }

    public override void OnDead(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
    {
        base.OnDead(damage, weapon, player, hit_point, hit_normal);
        switch (Random.Range(0, 100) % 4)
        {
            case 0:
                ANI_DEAD = ani_dead01;
                break;
            case 1:
                ANI_DEAD = ani_dead02;
                break;
            case 2:
                ANI_DEAD = ani_dead03;
                break;
            case 3:
                ANI_DEAD = ani_dead04;
                break;
        }
        if (is_ice_dead)
        {
            OnIceBodyCrash();
        }
        else
        {
            switch (RandomHeadBrokenFall(hit_point, hit_normal))
            {
                case 1:
                    if (!RandomBodyBrokenCrash())
                    {
                        ShowNeckBloodEff();
                    }
                    break;
                case 2:
                    RandomBodyBrokenCrash();
                    break;
            }
        }
        Object.Destroy(base.GetComponent<Collider>());
        StartCoroutine(RemoveOnTime(3f));
    }

    public override void DetermineNormalState()
    {
        CheckTargetPlayer();
        if (target_player == null)
        {
            SetState(IDLE_STATE);
        }
        else if (is_enter_special_attack)
        {
            if (CouldEnterAttackState())
            {
                ANI_ATTACK = ani_attack01;
                SetState(SHOOT_STATE);
            }
            else if (CouldEnterSpecialAttackState())
            {
                ANI_ATTACK = ani_attack02;
                SetState(SHOOT_STATE);
            }
            else
            {
                SetState(CATCHING_STATE);
                is_enter_special_attack = false;
            }
        }
        else if (CouldEnterAttackState())
        {
            ANI_ATTACK = ani_attack01;
            SetState(SHOOT_STATE);
            is_enter_special_attack = true;
        }
        else
        {
            SetState(CATCHING_STATE);
            is_enter_special_attack = false;
        }
    }

    public bool CouldEnterSpecialAttackState()
    {
        if (base.SqrDistanceFromPlayer < special_attack_range * special_attack_range)
        {
            return true;
        }
        return false;
    }

    public override void CheckHitAfter()
    {
        is_firing = false;
    }
}