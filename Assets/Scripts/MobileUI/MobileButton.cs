using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// MobileButton — ปุ่มกดบนมือถือ
/// Attach กับ Image ปุ่มแต่ละปุ่ม แล้วตั้ง ButtonType ใน Inspector
/// ใช้ได้กับ: HubWorld (Talk), Volleyball (Left/Right/Jump),
///            PingPong (Up/Down), BatBall (Tap)
/// </summary>
public class MobileButton : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType
    {
        Talk,           // HubWorld — คุยกับ NPC
        Left, Right,    // Volleyball
        Up,   Down,     // PingPong paddle
        Jump,           // Volleyball
        Tap             // BatBall
    }

    public ButtonType buttonType;

    // ── Static flags ─────────────────────────────────────────
    public static bool TalkPressed  = false;
    public static bool LeftHeld     = false;
    public static bool RightHeld    = false;
    public static bool UpHeld       = false;
    public static bool DownHeld     = false;
    public static bool JumpPressed  = false;
    public static bool TapPressed   = false;

    public void OnPointerDown(PointerEventData e)
    {
        switch (buttonType)
        {
            case ButtonType.Talk:  TalkPressed = true;  break;
            case ButtonType.Left:  LeftHeld    = true;  break;
            case ButtonType.Right: RightHeld   = true;  break;
            case ButtonType.Up:    UpHeld      = true;  break;
            case ButtonType.Down:  DownHeld    = true;  break;
            case ButtonType.Jump:  JumpPressed = true;  break;
            case ButtonType.Tap:   TapPressed  = true;  break;
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        switch (buttonType)
        {
            case ButtonType.Talk:  TalkPressed = false; break;
            case ButtonType.Left:  LeftHeld    = false; break;
            case ButtonType.Right: RightHeld   = false; break;
            case ButtonType.Up:    UpHeld      = false; break;
            case ButtonType.Down:  DownHeld    = false; break;
            case ButtonType.Jump:  JumpPressed = false; break;
            case ButtonType.Tap:   TapPressed  = false; break;
        }
    }

    void LateUpdate()
    {
        // Reset one-shot buttons ทุก frame
        TalkPressed = false;
        JumpPressed = false;
        TapPressed  = false;
    }
}
