using UnityEngine;

namespace CoMZ2
{
	public class SharkMissileState : EnemyState
	{
		protected SharkController shark;

		public override void DoStateLogic(float deltaTime)
		{
			if (!shark.nav_pather.GetCatchingState())
			{
				shark.AlwaysLookAtTarget();
			}
			if (AnimationUtil.IsAnimationPlayedPercentage(shark.gameObject, shark.ANI_MISSILE, 1f))
			{
				shark.SetState(shark.IDLE_STATE);
			}
		}

		public override void OnEnterState()
		{
			if (shark == null)
			{
				shark = m_enemy as SharkController;
			}
			shark.SetPathCatchState(false);
			shark.missile_enable = false;
			AnimationUtil.CrossAnimate(shark.gameObject, shark.ANI_MISSILE, WrapMode.ClampForever);
			AnimationUtil.Stop(shark.shark_fish);
			AnimationUtil.PlayAnimate(shark.shark_fish, "Zombie_Guter_Trennung_Weapon_Attack01", WrapMode.Loop);
		}

		public override void OnExitState()
		{
			AnimationUtil.Stop(shark.shark_fish);
		}
	}
}
