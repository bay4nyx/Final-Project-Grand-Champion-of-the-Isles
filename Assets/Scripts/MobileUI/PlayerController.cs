using UnityEngine;

/// <summary>
/// PlayerController — HubWorld
/// รองรับ Keyboard + Virtual Joystick + ปุ่ม Talk
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;

    [HideInInspector] public bool canMove = true;

    private Rigidbody2D    rb;
    private SpriteRenderer sr;
    private Animator       animator;

    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsIdle    = Animator.StringToHash("IsIdle");

    void Start()
    {
        rb       = GetComponent<Rigidbody2D>();
        sr       = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb.gravityScale   = 0f;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            SetAnim(false);
            return;
        }

        // Keyboard input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Virtual Joystick override
        if (VirtualJoystick.Direction.magnitude > 0.1f)
        {
            h = VirtualJoystick.Direction.x;
            v = VirtualJoystick.Direction.y;
        }

        Vector2 dir   = new Vector2(h, v).normalized;
        bool isMoving = dir.magnitude > 0.1f;

        rb.linearVelocity = dir * moveSpeed;
        SetAnim(isMoving);

        if      (h >  0.1f) sr.flipX = false;
        else if (h < -0.1f) sr.flipX = true;
    }

    void Update()
    {
        // ปุ่ม Talk (E หรือ Mobile button)
        // NPCInteraction จะรับ MobileButton.TalkPressed เองอยู่แล้ว
    }

    void SetAnim(bool isMoving)
    {
        animator?.SetBool(IsRunning, isMoving);
        animator?.SetBool(IsIdle,   !isMoving);
    }
}
