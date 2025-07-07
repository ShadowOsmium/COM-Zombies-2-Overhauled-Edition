using CoMZ2;
using UnityEngine;

public class ZombieController : EnemyController
{
	private string ani_run01 = "Forward01";

	private string ani_run02 = "Forward02";

	private string ani_run03 = "Forward03";

	private string ani_run04 = "Forward_speedup01";

	private string ani_run05 = "Forward_speedup02";

	private string ani_injure01 = "Damage01";

	private string ani_injure02 = "Damage02";

	private string ani_injure03 = "Damage03";

	private string ani_dead01 = "Death01";

	private string ani_dead02 = "Death02";

	private string ani_dead03 = "Death03";

	private string ani_dead04 = "Death04";

	private string ani_attack01 = "Attack01";

	private string ani_attack02 = "Attack02";

	private string ani_attack03 = "Attack03";

	private string ani_rail01 = "weilan01";

	private string ani_rail02 = "weilan02";

	protected Collider attackCollider;

	protected float speed_up_ratio = 1f;

	public override void Init()
	{
		ANI_IDLE = "Idle01";
		ANI_ATTACK = "Attack01";
		ANI_INJURED = "Damage01";
		ANI_DEAD = "Death01";
		ANI_RAIL = "weilan01";
		ANI_PERCEIVE = "TurnRound01";
		base.Init();
		speed_up_ratio = (float)enemy_data.config.Ex_conf["speedUpRatio"];
		RandomRunAnimation();
		RandomRailAnimation();
		is_rail_enable = true;
		attackCollider = base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Dummy_Head/Bip01 Head").gameObject.GetComponent<Collider>();
		if (attackCollider == null)
		{
			Debug.LogError("attack collider not founded!");
		}
		head_ori = base.transform.Find("Zombie_Normal_H_Head").gameObject;
		if (head_ori == null)
		{
			Debug.LogError("head_ori not founded!");
		}
		neck_ori = base.transform.Find("Zombie_Normal_H_Neck").gameObject;
		if (neck_ori == null)
		{
			Debug.LogError("neck_ori not founded!");
		}
		neck_ori.SetActive(false);
		body_ori = base.transform.Find("Zombie_Normal_H_Body").gameObject;
		if (body_ori == null)
		{
			Debug.LogError("body_ori not founded!");
		}
		head_broken = base.transform.Find("Zombie_Broken_Normal_Head01").gameObject;
		if (head_broken == null)
		{
			Debug.LogError("head_broken not founded!");
		}
		head_broken.SetActive(false);
		body_eff_prefab = base.Accessory[0];
		head_broken_eff_prefab = base.Accessory[1];
		if (GameData.Instance.cur_quest_info.mission_type == MissionType.Tutorial)
		{
			SetState(IDLE_STATE);
		}
		else
		{
			SetState(SHOW_STATE);
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

    protected void RandomRunAnimation()
	{
		switch (Random.Range(0, 100) % 5)
		{
		case 0:
			ANI_RUN = ani_run01;
			break;
		case 1:
			ANI_RUN = ani_run02;
			break;
		case 2:
			ANI_RUN = ani_run03;
			break;
		case 3:
			ANI_RUN = ani_run04;
			nav_pather.SetSpeed(enemy_data.move_speed * speed_up_ratio);
			break;
		case 4:
			ANI_RUN = ani_run05;
			nav_pather.SetSpeed(enemy_data.move_speed * speed_up_ratio);
			break;
		}
	}

	protected void RandomRailAnimation()
	{
		switch (Random.Range(0, 100) % 2)
		{
		case 0:
			ANI_RAIL = ani_rail01;
			break;
		case 1:
			ANI_RAIL = ani_rail02;
			break;
		}
	}

	protected void RandomAttackAnimation()
	{
		switch (Random.Range(0, 100) % 3)
		{
		case 0:
			ANI_ATTACK = ani_attack01;
			break;
		case 1:
			ANI_ATTACK = ani_attack02;
			break;
		case 2:
			ANI_ATTACK = ani_attack03;
			break;
		}
	}

	public override void CheckHit()
	{
		if (base.IsEnchant)
		{
			foreach (EnemyController value in GameSceneController.Instance.Enemy_Set.Values)
			{
				Collider collider = value.gameObject.GetComponent<Collider>();
				if (collider != null && attackCollider.bounds.Intersects(collider.bounds))
				{
					value.OnHit(enemy_data.damage_val, null, this, value.centroid, Vector3.up);
				}
			}
			return;
		}
		foreach (PlayerController value2 in GameSceneController.Instance.Player_Set.Values)
		{
			Collider collider2 = value2.gameObject.GetComponent<Collider>();
			if (collider2 != null && attackCollider.bounds.Intersects(collider2.bounds))
			{
				value2.OnHit(enemy_data.damage_val, null, this, value2.centroid, Vector3.up);
			}
		}
		foreach (NPCController value3 in GameSceneController.Instance.NPC_Set.Values)
		{
			Collider collider3 = value3.gameObject.GetComponent<Collider>();
			if (collider3 != null && attackCollider.bounds.Intersects(collider3.bounds))
			{
				value3.OnHit(enemy_data.damage_val, null, this, value3.centroid, Vector3.up);
			}
		}
		foreach (GuardianForceController value4 in GameSceneController.Instance.GuardianForce_Set.Values)
		{
			Collider collider4 = value4.gameObject.GetComponent<Collider>();
			if (collider4 != null && attackCollider.bounds.Intersects(collider4.bounds))
			{
				value4.OnHit(enemy_data.damage_val, null, this, value4.centroid, Vector3.up);
			}
		}
		foreach (EnemyController value5 in GameSceneController.Instance.Enemy_Enchant_Set.Values)
		{
			Collider collider5 = value5.gameObject.GetComponent<Collider>();
			if (collider5 != null && attackCollider.bounds.Intersects(collider5.bounds))
			{
				value5.OnHit(enemy_data.damage_val, null, this, value5.centroid, Vector3.up);
			}
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

	public override void Fire()
	{
		RandomAttackAnimation();
		ANI_CUR_ATTACK = ANI_ATTACK;
		AnimationUtil.CrossAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
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

	public void OnIceState()
	{
	}
}
