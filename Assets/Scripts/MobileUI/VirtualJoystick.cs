using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// VirtualJoystick — Joystick เดินบน HubWorld
/// Attach กับ JoystickBackground (Image วงกลมนอก)
/// ข้างในต้องมี JoystickKnob (Image วงกลมใน)
/// </summary>
public class VirtualJoystick : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    public RectTransform joystickBg;    // วงกลมนอก
    public RectTransform joystickKnob;  // วงกลมใน

    [Header("Settings")]
    public float maxRadius = 60f;

    // PlayerController อ่านค่านี้
    public static Vector2 Direction = Vector2.zero;

    public void OnPointerDown(PointerEventData e) => MoveKnob(e);
    public void OnDrag(PointerEventData e)        => MoveKnob(e);

    public void OnPointerUp(PointerEventData e)
    {
        Direction = Vector2.zero;
        joystickKnob.anchoredPosition = Vector2.zero;
    }

    void MoveKnob(PointerEventData e)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBg, e.position, e.pressEventCamera, out localPos);

        localPos  = Vector2.ClampMagnitude(localPos, maxRadius);
        joystickKnob.anchoredPosition = localPos;
        Direction = localPos / maxRadius;
    }
}
