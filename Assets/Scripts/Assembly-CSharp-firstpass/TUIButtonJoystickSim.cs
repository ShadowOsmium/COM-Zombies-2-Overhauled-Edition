using System;
using UnityEngine;

public class TUIButtonJoystickSim : TUIButton
{
    public const int CommandDown = 1;
    public const int CommandMove = 2;
    public const int CommandUp = 3;

    public GameObject m_JoyStickObj;
    public GameObject joy_bk; // Define joy_bk
    public float m_MinDistance;
    public float m_MaxDistance;

    private float m_Direction;
    private float m_Distance;

    public override bool HandleInput(TUIInput input)
    {
        if (m_bDisable)
        {
            if (joy_bk != null)
            {
                joy_bk.SetActive(false); // Disable joy_bk
            }
            return false;
        }

        // Existing input handling code...
        if (input.inputType == TUIInputType.Began)
        {
            if (PtInControl(input.position))
            {
                m_bPressed = true;
                m_iFingerId = input.fingerId;
                float num = input.position.x - base.transform.position.x;
                float num2 = input.position.y - base.transform.position.y;
                m_Direction = ((!(num2 >= 0f)) ? (Mathf.Atan2(num2, num) + (float)Math.PI * 2f) : Mathf.Atan2(num2, num));
                m_Distance = Mathf.Sqrt(num * num + num2 * num2);
                if (m_Distance > m_MaxDistance)
                {
                    m_Distance = m_MaxDistance;
                }
                float num3 = (m_Distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
                Show();
                PostEvent(this, 1, num, num2, null);
                return true;
            }
        }
        else if (input.fingerId == m_iFingerId)
        {
            if (input.inputType == TUIInputType.Moved)
            {
                float num4 = input.position.x - base.transform.position.x;
                float num5 = input.position.y - base.transform.position.y;
                m_Direction = ((!(num5 >= 0f)) ? (Mathf.Atan2(num5, num4) + (float)Math.PI * 2f) : Mathf.Atan2(num5, num4));
                m_Distance = Mathf.Sqrt(num4 * num4 + num5 * num5);
                if (m_Distance > m_MaxDistance)
                {
                    m_Distance = m_MaxDistance;
                }
                float num6 = (m_Distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
                Show();
                PostEvent(this, 2, num4, num5, null);
            }
            else if (input.inputType == TUIInputType.Ended)
            {
                m_bPressed = false;
                m_iFingerId = -1;
                m_Direction = 0f;
                m_Distance = 0f;
                Show();
                PostEvent(this, 3, 0f, 0f, null);
            }
        }
        return false;
    }

    public override void Show()
    {
        base.Show();
        if (null != m_JoyStickObj && Application.isMobilePlatform) // Check if running on mobile
        {
            Vector3 localPosition = new Vector3(z: m_JoyStickObj.transform.localPosition.z, x: m_Distance * Mathf.Cos(m_Direction), y: m_Distance * Mathf.Sin(m_Direction));
            m_JoyStickObj.transform.localPosition = localPosition;
            m_JoyStickObj.SetActive(true); // Show joystick on mobile
        }
        else
        {
            m_JoyStickObj.SetActive(false); // Hide joystick on PC
        }
    }

    public void SimPress(TUIInput input)
    {
        if (input.inputType == TUIInputType.Began)
        {
            m_bPressed = true;
            m_iFingerId = input.fingerId;
            float num = input.position.x - base.transform.position.x;
            float num2 = input.position.y - base.transform.position.y;
            m_Direction = ((!(num2 >= 0f)) ? (Mathf.Atan2(num2, num) + (float)Math.PI * 2f) : Mathf.Atan2(num2, num));
            m_Distance = Mathf.Sqrt(num * num + num2 * num2);
            if (m_Distance > m_MaxDistance)
            {
                m_Distance = m_MaxDistance;
            }
            float num3 = (m_Distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
            Show();
            PostEvent(this, 1, num, num2, null);
        }
    }
}
