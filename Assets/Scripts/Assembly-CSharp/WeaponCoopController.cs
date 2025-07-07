using UnityEngine;

public class WeaponCoopController : WeaponController
{
	public override void Reset(Transform trans)
	{
		base.transform.position = trans.position + ori_pos;
		base.transform.parent = trans;
		base.transform.localRotation = Quaternion.identity;
		GunOff();
	}

	public override void UpdateWeaponFrame(bool status)
	{
	}

	public override void SetWeaponData(WeaponData data)
	{
		weapon_data = data;
		weapon_data.ResetData();
		weapon_data.Reload();
	}

	public override void FireUpdate(PlayerController player, float deltaTime)
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

	public override bool CouldMakeNextShoot()
	{
		if (fire_interval >= base.FireRate)
		{
			return true;
		}
		return false;
	}

	public override void Fire(PlayerController player, float deltaTime)
	{
		fire_interval = 0f;
		player.FireLight(true, deltaTime);
	}

	public override void StopFire(PlayerController player)
	{
	}

	public override void GunOn()
	{
		render_obj.GetComponent<Renderer>().enabled = true;
		OnWeaponEquip();
	}

	public override void GunOff()
	{
		render_obj.GetComponent<Renderer>().enabled = false;
	}

	public override void CheckHit(ObjectController controller)
	{
	}

	public override bool HaveBullets()
	{
		return true;
	}

    public override float GetDamageValWithAvatar(PlayerController player, AvatarData avatar)
    {
        float multiplier = 1f + (avatar.damage_val / 100f);
        float damage = weapon_data.damage_val * multiplier * player.ComboDamageRatio;

        Debug.Log("[Coop GetDamageValWithAvatar] base damage = " + weapon_data.damage_val.ToString() +
                  ", avatar damage % = " + avatar.damage_val.ToString() +
                  ", combo ratio = " + player.ComboDamageRatio.ToString() +
                  ", final damage = " + damage.ToString());

        return damage;
    }

    public override void OnWeaponReload()
	{
	}

	public override bool OnReadyFire(PlayerController player, float deltaTime)
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

	public override float GetAfterFireTime()
	{
		return weapon_data.config.afterFireTime;
	}

	public override bool FinishFireAni(PlayerController player)
	{
		if (!AnimationUtil.IsPlayingAnimation(player.gameObject, ANI_SHOOT) || AnimationUtil.AnimationEnds(player.gameObject, ANI_SHOOT))
		{
			return true;
		}
		return false;
	}

	public override void OnFireRelease(PlayerController player)
	{
	}
}
