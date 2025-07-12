using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    protected PlayerController player;

    private void Start()
    {
    }

    private void Update()
    {
    }

    private bool IsReloading()
    {
        return player != null && player.IsReloading();
    }

    private void OnAttack()
    {
        if (IsReloading())
        {
            Debug.Log("OnAttack blocked during reload");
            return;
        }

        if (player != null)
        {
            player.CheckHit();
        }
    }

    private void OnSpecialAttack()
    {
        if (IsReloading())
        {
            Debug.Log("OnSpecialAttack blocked during reload");
            return;
        }

        if (player != null)
        {
            player.ReleaseChaisawTarget();
        }
    }

    public void SetController(PlayerController pController)
    {
        player = pController;
    }

    private void OnScarecrowBefore()
    {
        if (IsReloading())
        {
            Debug.Log("OnScarecrowBefore blocked during reload");
            return;
        }

        if (player != null)
        {
            // Existing logic here, if any
        }
    }

    private void OnScarecrowPut()
    {
        if (IsReloading())
        {
            Debug.Log("OnScarecrowPut blocked during reload");
            return;
        }

        if (player != null)
        {
            // Existing logic here, if any
        }
    }

    private void OnGrenadeThrow()
    {
        if (IsReloading())
        {
            Debug.Log("OnGrenadeThrow blocked during reload");
            return;
        }

        if (player == null)
            return;

        SwatController swatController = player as SwatController;
        if (swatController != null)
        {
            swatController.SpawnGrenade();
            return;
        }
        SwatCoopController swatCoopController = player as SwatCoopController;
        if (swatCoopController != null)
        {
            swatCoopController.SpawnGrenade();
        }
    }

    private void OnEnchantTriger()
    {
        if (IsReloading())
        {
            Debug.Log("OnEnchantTriger blocked during reload");
            return;
        }

        if (player != null)
        {
            DoctorController doctorController = player as DoctorController;
            if (doctorController != null)
            {
                doctorController.EnchantFire();
            }
        }
    }
}
