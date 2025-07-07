using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
	protected EnemyController enemy;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnAttack()
	{
		if (enemy != null)
		{
			enemy.CheckHit();
		}
	}

	private void CameraShake()
	{
		GameSceneController.Instance.main_camera.StartCommonShake();
	}

	private void OnAttackAfter()
	{
		if (enemy != null)
		{
			enemy.CheckHitAfter();
		}
	}

	private void OnHaokeRush()
	{
		if (enemy != null)
		{
			HaokeController haokeController = enemy as HaokeController;
			if (haokeController != null)
			{
				haokeController.OnHaokeRushReady();
			}
		}
	}

	private void OnHaokeTank()
	{
		if (enemy != null)
		{
			HaokeController haokeController = enemy as HaokeController;
			if (haokeController != null)
			{
				haokeController.OnHaokeTankStart();
			}
		}
	}

	private void OnHaokeSummonStart()
	{
		if (enemy != null)
		{
			HaokeController haokeController = enemy as HaokeController;
			if (haokeController != null)
			{
				haokeController.PlayEffSummonGround();
			}
		}
	}

	private void OnHaokeSummon()
	{
		if (enemy != null)
		{
			HaokeController haokeController = enemy as HaokeController;
			if (haokeController != null)
			{
				haokeController.OnHaokeSummonReady();
			}
		}
	}

	private void OnFatFocus()
	{
		if (enemy != null)
		{
			FatCookController fatCookController = enemy as FatCookController;
			if (fatCookController != null)
			{
				fatCookController.PlayEffRing();
			}
		}
	}

	private void OnFatComboEff1()
	{
		if (enemy != null)
		{
			FatCookController fatCookController = enemy as FatCookController;
			if (fatCookController != null)
			{
				fatCookController.PlayEffCombo1();
			}
		}
	}

	private void OnFatComboEff2()
	{
		if (enemy != null)
		{
			FatCookController fatCookController = enemy as FatCookController;
			if (fatCookController != null)
			{
				fatCookController.PlayEffCombo2();
			}
		}
	}

	private void OnFatThrowMiantuan()
	{
		if (enemy != null)
		{
			FatCookController fatCookController = enemy as FatCookController;
			if (fatCookController != null)
			{
				StartCoroutine(fatCookController.ThrowMiantuan());
			}
		}
	}

	private void OnFatSummon()
	{
		if (enemy != null)
		{
			FatCookController fatCookController = enemy as FatCookController;
			if (fatCookController != null)
			{
				fatCookController.SummonTimerBoomer();
			}
		}
	}

	public void SetController(EnemyController eController)
	{
		enemy = eController;
	}

	private void OnSharkHideWeapon()
	{
		if (enemy != null)
		{
			SharkController sharkController = enemy as SharkController;
			if (sharkController != null)
			{
				sharkController.OnSharkHideWeapon();
			}
		}
	}
}
