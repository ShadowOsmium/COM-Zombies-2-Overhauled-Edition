using CoMZ2;
using UnityEngine;

public class NurseController : EnemyController
{
	private string ani_run01 = "Forward01";

	private string ani_run02 = "Forward02";

	private string ani_injure01 = "Damage01";

	private string ani_injure02 = "Damage02";

	private string ani_injure03 = "Damage03";

	private string ani_dead01 = "Death01";

	private string ani_dead02 = "Death02";

	private string ani_dead03 = "Death03";

	private string ani_dead04 = "Death04";

	public string ani_skill01 = "Attack02_01";

	public string ani_skill02 = "Attack02_02";

	public string ani_skill03 = "Attack02_03";

	private string ani_attack00 = "Attack00";

	private string ani_attack01 = "Attack01";

	public EnemyState NURSE_SKILL_STATE;

	protected GameObject head_eff;

	protected Transform mouth_trans;

	protected GameObject Saliva_Eff;

	private float injured_ani_val = 0.1f;

	private Vector3 fire_target = Vector3.zero;

	private GameObject eff_vomit;

	private bool is_firing;

	protected GameObject nurse_hat;

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
		ANI_IDLE = "Idle01";
		ANI_ATTACK = "Attack01";
		ANI_INJURED = "Damage01";
		ANI_DEAD = "Death01";
		RandomRunAnimation();
		base.Init();
		NURSE_SKILL_STATE = EnemyState.Create(EnemyStateType.NurseSkill, this);
		head_ori = base.transform.Find("Zombie_Nurse_H_Head").gameObject;
		if (head_ori == null)
		{
			Debug.LogError("head_ori not found!");
		}
		neck_ori = base.transform.Find("Zombie_Nurse_H_Neck").gameObject;
		if (neck_ori == null)
		{
			Debug.LogError("neck_ori not founded!");
		}
		neck_ori.SetActive(false);
		mouth_trans = base.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Dummy_Head/Bip01 Head/Bip01 Jaw/Mouth02");
		if (mouth_trans == null)
		{
			Debug.LogError("mouth_trans not found!");
		}
		body_ori = base.transform.Find("Zombie_Nurse_H_Body").gameObject;
		if (body_ori == null)
		{
			Debug.LogError("body_ori not founded!");
		}
		nurse_hat = base.transform.Find("Zombie_Nurse_H_01").gameObject;
		if (nurse_hat == null)
		{
			Debug.LogError("nurse_hat not founded!");
		}
		head_broken = base.transform.Find("Zombie_Broken_Nurse_Head01").gameObject;
		if (head_broken == null)
		{
			Debug.LogError("head_broken not founded!");
		}
		head_broken.SetActive(false);
		eff_vomit = base.transform.Find("venom_03").gameObject;
		if (eff_vomit == null)
		{
			Debug.LogError("eff_vomit not founded!");
		}
		StopVomitEff();
		body_eff_prefab = base.Accessory[3];
		head_broken_eff_prefab = base.Accessory[0];
		Saliva_Eff = Object.Instantiate(base.Accessory[2], mouth_trans.transform.position, Quaternion.identity) as GameObject;
		Saliva_Eff.transform.parent = mouth_trans;
		Saliva_Eff.transform.localPosition = Vector3.zero;
		Saliva_Eff.GetComponent<ParticleSystem>().Stop();
		SetState(SHOW_STATE);
	}

	protected void RandomRunAnimation()
	{
		int num = Random.Range(0, 10);
		if (num < 5)
		{
			ANI_RUN = ani_run01;
		}
		else
		{
			ANI_RUN = ani_run02;
		}
	}

	public override void CheckHit()
	{
		float num = 0f;
		Vector3 vector = fire_target - mouth_trans.transform.position;
		float magnitude = vector.magnitude;
		float num2 = 12f;
		float num3 = magnitude / num2;
		float num4 = (num - 0.5f * Physics.gravity.y * num3 * num3) / num3;
		Vector3 vector2 = Vector3.up * num4 + vector.normalized * num2;
		GameObject gameObject = Object.Instantiate(base.Accessory[1], mouth_trans.transform.position, Quaternion.LookRotation(vector2)) as GameObject;
		if (base.IsEnchant)
		{
			gameObject.layer = PhysicsLayer.PLAYER_PROJECTILE;
		}
		NurseProjectile component = gameObject.GetComponent<NurseProjectile>();
		component.launch_dir = vector2;
		component.launch_speed = vector2;
		component.explode_radius = 2f;
		component.damage = enemy_data.damage_val;
		component.object_controller = this;
		Saliva_Eff.GetComponent<ParticleSystem>().Play();
		Saliva_Eff.GetComponent<EffectAudioBehaviour>().PlayEffect();
	}

	public override void CheckHitAfter()
	{
		is_firing = false;
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

	public override void OnHit(float damage, WeaponController weapon, ObjectController player, Vector3 hit_point, Vector3 hit_normal)
	{
		ResetInjureAni(hit_point);
		base.OnHit(damage, weapon, player, hit_point, hit_normal);
	}

	public override void FireUpdate(float deltaTime)
	{
		if (target_player != null && !is_firing)
		{
			Quaternion to = Quaternion.LookRotation(target_player.transform.position - base.transform.position, Vector3.up);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 180f * deltaTime);
		}
		if (AnimationUtil.IsAnimationPlayedPercentage(base.gameObject, ANI_ATTACK, 0.9f))
		{
			SetState(AFTER_SHOOT_STATE);
		}
	}

	public override void Fire()
	{
		if (target_player != null)
		{
			base.transform.LookAt(target_player.transform);
		}
		is_firing = true;
		RandomAttackAnimation();
		ANI_CUR_ATTACK = ANI_ATTACK;
		AnimationUtil.CrossAnimate(base.gameObject, ANI_ATTACK, WrapMode.ClampForever);
		if (target_player != null)
		{
			fire_target = target_player.centroid;
		}
		else
		{
			fire_target = centroid;
		}
	}

	public override bool CouldEnterAttackState()
	{
		if (base.CouldEnterAttackState())
		{
			if (GameSceneController.CheckBlockBetween(centroid, target_player.centroid))
			{
				return false;
			}
			return true;
		}
		return false;
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

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (head_eff != null)
		{
			Object.Destroy(head_eff);
		}
	}

	public override void DetermineNormalState()
	{
		CheckTargetPlayer();
		if (target_player == null)
		{
			SetState(IDLE_STATE);
		}
		else if (CouldEnterAttackState())
		{
			if (Random.Range(0, 100) >= 80)
			{
				SetState(NURSE_SKILL_STATE);
			}
			else
			{
				SetState(SHOOT_STATE);
			}
		}
		else
		{
			SetState(CATCHING_STATE);
		}
	}

	public void DoSkillAttack(float deltaTime)
	{
		if (target_player != null)
		{
			base.transform.LookAt(target_player.transform);
		}
	}

	protected void RandomAttackAnimation()
	{
		switch (Random.Range(0, 100) % 2)
		{
		case 0:
			ANI_ATTACK = ani_attack00;
			break;
		case 1:
			ANI_ATTACK = ani_attack01;
			break;
		}
	}

	public void PlayVomitEff()
	{
		eff_vomit.SetActive(true);
		eff_vomit.GetComponent<ParticleSystem>().Play();
	}

	public void StopVomitEff()
	{
		eff_vomit.GetComponent<ParticleSystem>().Stop();
		eff_vomit.SetActive(false);
	}

	public override void OnHeadFall()
	{
		nurse_hat.SetActive(false);
	}

	public override void OnHeadCrash()
	{
		nurse_hat.SetActive(false);
	}
}
