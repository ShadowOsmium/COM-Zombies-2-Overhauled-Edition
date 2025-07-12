using UnityEngine;

namespace CoMZ2
{
    public class PlayerReloadState : PlayerState
    {
        private bool canInterrupt = false;

        public override void DoStateLogic(float deltaTime)
        {
            float animProgress = AnimationUtil.GetAnimationPlayedPercentage(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this));

            // Only after 85% of reload animation, allow interruptions
            if (!canInterrupt && animProgress >= 0.85f)
            {
                canInterrupt = true;
            }

            if (canInterrupt)
            {
                // Allow player input to change fire state normally once reload is mostly done
                m_player.CalculateSetFireState();
            }
            else
            {
                // Block input/state changes until reload almost done
                // (you can optionally ignore or queue input here)
            }
        }


        public bool CanInterrupt()
        {
            return canInterrupt;
        }

        public override void OnEnterState()
        {
            canInterrupt = false;
            AnimationUtil.CrossAnimate(m_player.gameObject, m_player.GetFireStateAnimation(m_player.MoveState, this), WrapMode.ClampForever);
            m_player.OnWeaponReload();
            m_player.GetMoveIdleAnimation(this);
        }

        public override void OnExitState()
        {
            m_player.UpdateWeaponUIShow();
            m_player.ResetFireInterval();
        }
    }
}
