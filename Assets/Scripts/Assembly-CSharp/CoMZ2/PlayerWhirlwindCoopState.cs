using UnityEngine;

namespace CoMZ2
{
	public class PlayerWhirlwindCoopState : PlayerState
	{
		protected HumanCoopController human;

		protected float cur_skill_time;

		protected float skill_life_time;

		public override void DoStateLogic(float deltaTime)
		{
			human.OnWhirlwindLogic(deltaTime);
		}

		public override void OnEnterState()
		{
			if (human == null)
			{
				human = m_player as HumanCoopController;
				skill_life_time = human.skill_set["Whirlwind"].skill_data.life_time;
			}
			human.ShowCurWeapon(false);
			cur_skill_time = 0f;
			AnimationUtil.Stop(human.gameObject);
			AnimationUtil.PlayAnimate(human.gameObject, "Electric_saw_Skill01", WrapMode.Loop);
			human.ShowWhirlwindEff();
		}

		public override void OnExitState()
		{
			human.HideWhirlwindEff();
			human.ShowCurWeapon(true);
		}
	}
}
