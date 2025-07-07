using UnityEngine;

namespace CoMZ2
{
	public class FatCookAfterAttackState : EnemyState
	{
		public bool IsAfterMiantuan;

		private int random;

		protected FatCookController fatCook;

		private int step = 1;

		public override void DoStateLogic(float deltaTime)
		{
			fatCook.AlwaysLookAtTarget();
			if (IsAfterMiantuan)
			{
				if (AnimationUtil.IsAnimationPlayedPercentage(fatCook.gameObject, fatCook.ANI_IDLE_02, 1f))
				{
					fatCook.SetState(fatCook.IDLE_STATE);
				}
			}
			else if (random == 0)
			{
				if (step == 1)
				{
					if (AnimationUtil.IsAnimationPlayedPercentage(fatCook.gameObject, fatCook.ANI_IDLE_01, 0.95f))
					{
						step++;
						AnimationUtil.CrossAnimate(fatCook.gameObject, fatCook.ANI_IDLE_01, WrapMode.ClampForever);
					}
				}
				else if (step == 2)
				{
					if (AnimationUtil.IsAnimationPlayedPercentage(fatCook.gameObject, fatCook.ANI_IDLE_01, 0.95f))
					{
						step++;
						AnimationUtil.CrossAnimate(fatCook.gameObject, fatCook.ANI_IDLE_03, WrapMode.ClampForever);
					}
				}
				else if (step == 3)
				{
					fatCook.MoveBack(deltaTime);
					if (AnimationUtil.IsAnimationPlayedPercentage(fatCook.gameObject, fatCook.ANI_IDLE_03, 1f))
					{
						fatCook.SetState(fatCook.IDLE_STATE);
					}
				}
			}
			else
			{
				if (random != 1)
				{
					return;
				}
				if (step == 1)
				{
					if (AnimationUtil.IsAnimationPlayedPercentage(fatCook.gameObject, fatCook.ANI_IDLE_02, 0.95f))
					{
						step++;
						AnimationUtil.CrossAnimate(fatCook.gameObject, fatCook.ANI_IDLE_01, WrapMode.ClampForever);
					}
				}
				else if (step == 2 && AnimationUtil.IsAnimationPlayedPercentage(fatCook.gameObject, fatCook.ANI_IDLE_01, 1f))
				{
					fatCook.SetState(fatCook.IDLE_STATE);
				}
			}
		}

		public override void OnEnterState()
		{
			if (fatCook == null)
			{
				fatCook = m_enemy as FatCookController;
			}
			m_enemy.SetPathCatchState(false);
			step = 1;
			if (IsAfterMiantuan)
			{
				AnimationUtil.CrossAnimate(fatCook.gameObject, fatCook.ANI_IDLE_02, WrapMode.ClampForever);
				return;
			}
			random = Random.Range(0, 2);
			if (random == 0)
			{
				AnimationUtil.CrossAnimate(fatCook.gameObject, fatCook.ANI_IDLE_01, WrapMode.ClampForever);
			}
			else if (random == 1)
			{
				AnimationUtil.CrossAnimate(fatCook.gameObject, fatCook.ANI_IDLE_02, WrapMode.ClampForever);
			}
		}

		public override void OnExitState()
		{
		}
	}
}
