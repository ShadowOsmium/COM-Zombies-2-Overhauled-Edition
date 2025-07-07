using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	public enum FireReady
	{
		None,
		Readying,
		Finished
	}

	public WeaponType weapon_type;

	public WeaponData weapon_data;

	public List<GameObject> Accessory;

	protected float fire_interval = 9999f;

	protected string ANI_SHIFT_WEAPON = "AK_UpperBody_Run01";

	protected string ANI_IDLE_RUN = "AK_UpperBody_Run01";

	protected string ANI_IDLE_SHOOT = "AK_UpperBody_Idle01";

	protected string ANI_SHOOT = "AK_UpperBody_Shooting01";

	protected string ANI_RELOAD = "AK_UpperBody_Reload01";

	protected string ANI_DEATH = "Death01";

	protected string ANI_INJURED = "A_UP_Idle01";

	protected string ANI_FIRE_READY = "AK_UpperBody_Shooting01";

	protected string ANI_IDLE_DOWN = "LowerBody_Idle01";

	protected string ANI_SHOOT_DOWN = "LowerBody_Idle01";

	protected string ANI_RELOAD_DOWN = "LowerBody_Idle01";

	protected string ANI_SWITCH_DOWN = "LowerBody_Idle01";

	protected GameObject render_obj;

	public Vector3 ori_pos = Vector3.zero;

	protected FireReady ready_fire;

	protected float ready_fire_time = 0.2f;

	protected float cur_ready_fire_time;

	public bool switch_ready_fire;

	protected float combo_fire_rate_ratio = 1f;

	public float ComboRateRatio
	{
		get
		{
			return combo_fire_rate_ratio;
		}
		set
		{
			combo_fire_rate_ratio = value;
		}
	}

	public float FireRate
	{
		get
		{
			return weapon_data.frequency_val / combo_fire_rate_ratio;
		}
	}

	private void Awake()
	{
		weapon_type = WeaponType.NoGun;
		render_obj = base.gameObject;
	}

	private void Start()
	{
	}

	public virtual void Update()
	{
		fire_interval += Time.deltaTime;
	}

	public virtual void Reset(Transform trans)
	{
		base.transform.position = trans.position + ori_pos;
		base.transform.parent = trans;
		base.transform.localRotation = Quaternion.identity;
		GunOff();
	}

	public virtual void OnWeaponEquip()
	{
	}

	public virtual void UpdateWeaponFrame(bool status)
	{
		GameSceneController.Instance.UpdateWeaponFrame("Gameui_" + weapon_data.weapon_name, status);
		if (weapon_data.weapon_name == GameData.Instance.WeaponData_Set["PGM"].weapon_name)
		{
			GameSceneController.Instance.PGM_Lock_Rect.gameObject.SetActive(true);
		}
		else
		{
			GameSceneController.Instance.PGM_Lock_Rect.gameObject.SetActive(false);
		}
	}

	public virtual void ResetFireInterval()
	{
		fire_interval = 9999f;
	}

	public virtual void SetWeaponData(WeaponData data)
	{
		weapon_data = data;
		weapon_data.ResetData();
		weapon_data.Reload();
	}

	public virtual void SetWeaponAni(GameObject player, Transform spine)
	{
		player.GetComponent<Animation>()[ANI_IDLE_DOWN].layer = -1;
		player.GetComponent<Animation>()[ANI_SHOOT_DOWN].layer = -1;
		player.GetComponent<Animation>()[ANI_RELOAD_DOWN].layer = -1;
		player.GetComponent<Animation>()[ANI_SWITCH_DOWN].layer = -1;
	}

	public virtual void SetShopWeaponAni(GameObject player, Transform spine)
	{
	}

	public virtual void FireUpdate(PlayerController player, float deltaTime)
	{
		if (CouldMakeNextShoot())
		{
			Fire(player, deltaTime);
		}
		else
		{
			player.FireLight(false, deltaTime);
		}
	}

	public virtual bool CouldMakeNextShoot()
	{
		if (fire_interval >= FireRate)
		{
			return true;
		}
		return false;
	}

	public virtual void Fire(PlayerController player, float deltaTime)
	{
		fire_interval = 0f;
		player.FireLight(true, deltaTime);
	}

	public virtual void StopFire(PlayerController player)
	{
	}

	public virtual void GunOn()
	{
		render_obj.GetComponent<Renderer>().enabled = true;
		OnWeaponEquip();
	}

	public virtual void GunOff()
	{
		render_obj.GetComponent<Renderer>().enabled = false;
	}

	public virtual void CheckHit(ObjectController controller)
	{
	}

	public virtual string GetFireStateAnimation(PlayerStateType run_type, PlayerStateType fire_type)
	{
		string result = string.Empty;
		switch (fire_type)
		{
		case PlayerStateType.IdleShoot:
			result = ((run_type != PlayerStateType.Run) ? ANI_IDLE_SHOOT : ANI_IDLE_RUN);
			break;
		case PlayerStateType.Shoot:
			result = ANI_SHOOT;
			break;
		case PlayerStateType.Reload:
			result = ANI_RELOAD;
			break;
		case PlayerStateType.GotHit:
			result = ANI_INJURED;
			break;
		case PlayerStateType.Dead:
			result = ANI_DEATH;
			break;
		case PlayerStateType.SwitchWeapon:
			result = ANI_SHIFT_WEAPON;
			break;
		}
		return result;
	}

	public virtual string GetMoveIdleAnimation(PlayerStateType fire_type)
	{
		string empty = string.Empty;
		switch (fire_type)
		{
		case PlayerStateType.IdleShoot:
			return ANI_IDLE_DOWN;
		case PlayerStateType.Shoot:
			return ANI_SHOOT_DOWN;
		case PlayerStateType.AfterShoot:
			return ANI_IDLE_DOWN;
		case PlayerStateType.Reload:
			return ANI_RELOAD_DOWN;
		case PlayerStateType.SwitchWeapon:
			return ANI_SWITCH_DOWN;
		default:
			return ANI_IDLE_DOWN;
		}
	}

	public virtual bool HaveBullets()
	{
		if (weapon_data.clip_bullet_count > 0)
		{
			return true;
		}
		return false;
	}

    public virtual float GetDamageValWithAvatar(PlayerController player, AvatarData avatar)
    {
        float multiplier = 1f + (avatar.damage_val / 100f);
        float damage = weapon_data.damage_val * multiplier * player.ComboDamageRatio;

        Debug.Log("GetDamageValWithAvatar: base damage = " + weapon_data.damage_val.ToString() +
                  ", avatar damage % = " + avatar.damage_val.ToString() +
                  ", combo ratio = " + player.ComboDamageRatio.ToString() +
                  ", final damage = " + damage.ToString());

        return damage;
    }

    public virtual void OnWeaponReload()
	{
	}

	public virtual void ResetFireAniSpeed(GameObject player)
	{
		if (FireRate < player.GetComponent<Animation>()[ANI_SHOOT].length)
		{
			player.GetComponent<Animation>()[ANI_SHOOT].speed = player.GetComponent<Animation>()[ANI_SHOOT].length / FireRate * 1f;
		}
	}

	public virtual void ResetReloadAniSpeed(GameObject player)
	{
		if (player.GetComponent<PlayerController>() != null)
		{
			player.GetComponent<Animation>()[ANI_RELOAD].speed = player.GetComponent<PlayerController>().avatar_data.reload_speed_val;
		}
	}

	public string GetShopIdleAni()
	{
		return ANI_IDLE_SHOOT;
	}

	public void ResetWeaponFireReadyState()
	{
		ready_fire = FireReady.None;
		cur_ready_fire_time = 0f;
	}

	public virtual bool OnReadyFire(PlayerController player, float deltaTime)
	{
		if (ready_fire == FireReady.None)
		{
			AnimationUtil.CrossAnimate(player.gameObject, ANI_FIRE_READY, WrapMode.ClampForever, ready_fire_time);
			cur_ready_fire_time = 0f;
			ready_fire = FireReady.Readying;
			return false;
		}
		if (ready_fire == FireReady.Readying)
		{
			cur_ready_fire_time += deltaTime;
			if (!(cur_ready_fire_time >= ready_fire_time))
			{
				return false;
			}
			ready_fire = FireReady.Finished;
		}
		return true;
	}

	public virtual void OnEnterAfterFire(PlayerController player)
	{
	}

	public virtual float GetAfterFireTime()
	{
		return weapon_data.config.afterFireTime;
	}

	public virtual bool FinishFireAni(PlayerController player)
	{
		if (!AnimationUtil.IsPlayingAnimation(player.gameObject, ANI_SHOOT) || AnimationUtil.AnimationEnds(player.gameObject, ANI_SHOOT))
		{
			return true;
		}
		return false;
	}

	public virtual void OnFireRelease(PlayerController player)
	{
	}
}
