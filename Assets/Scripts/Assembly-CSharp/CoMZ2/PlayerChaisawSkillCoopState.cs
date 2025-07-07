using UnityEngine;

namespace CoMZ2
{
	public class PlayerChaisawSkillCoopState : PlayerState
	{
		public override void DoStateLogic(float deltaTime)
		{
			if (GameSceneController.Instance.main_camera != null)
			{
				GameSceneController.Instance.main_camera.ZoomChaisawSpecial(deltaTime);
			}
			if (AnimationUtil.IsAnimationPlayedPercentage(m_player.gameObject, m_player.ANI_CHAISAW_SKILL, 1f))
			{
				m_player.CalculateSetFireState();
			}
		}

		public override void OnEnterState()
		{
			AnimationUtil.Stop(m_player.gameObject);
			AnimationUtil.PlayAnimate(m_player.gameObject, m_player.ANI_CHAISAW_SKILL, WrapMode.ClampForever);
			Camera.main.transform.parent = m_player.chaisaw_view;
			Camera.main.transform.localPosition = Vector3.zero;
			Camera.main.transform.localRotation = Quaternion.identity;
			GameSceneController.Instance.main_camera.camera_pause = true;
			AnimationUtil.PlayAnimate(m_player.chaisaw_view.gameObject, "OT_Saw_Camera00", WrapMode.Once);
		}

		public override void OnExitState()
		{
			GameSceneController.Instance.main_camera.camera_pause = false;
			Camera.main.transform.parent = null;
		}
	}
}
