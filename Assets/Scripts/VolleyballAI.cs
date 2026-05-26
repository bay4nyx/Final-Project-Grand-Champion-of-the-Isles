using UnityEngine;

/// <summary>
/// VolleyballAI — AI อยู่แค่ในพื้นที่ GroundRight
/// State Machine: Idle → MovingToTarget → Jumping → Hitting
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class VolleyballAI : MonoBehaviour
{
    [Header("References")]
    public Transform   ballTransform;
    public Rigidbody2D ballRb;

    [Header("Ground Reference — Drag GroundRight มาใส่")]
    public Transform groundRight;

    [Header("Config")]
    public float moveSpeed = 3f;
    public float jumpForce = 8f;
    public float hitForce  = 7f;
    public float hitRange  = 2f;

    // ── Boundary คำนวณจาก GroundRight ───────────────────────
    private float minX = 0.5f;
    private float maxX = 7f;

    // ── State Machine ────────────────────────────────────────
    private enum AIState { Idle, MovingToTarget, Jumping, Hitting }
    private AIState currentState = AIState.Idle;

    private Rigidbody2D rb;
    private bool        isGrounded = true;
    private bool        aiOnGround = true;
    private Vector2     targetPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ── ดึงขอบเขตจาก GroundRight ─────────────────────────
        if (groundRight != null)
        {
            BoxCollider2D col = groundRight.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                minX = groundRight.position.x - col.size.x * 0.5f + 0.3f;
                maxX = groundRight.position.x + col.size.x * 0.5f - 0.3f;
            }
        }

        // ตั้งตำแหน่ง default ตรงกลาง GroundRight
        targetPos = new Vector2((minX + maxX) * 0.5f, transform.position.y);
    }

    void Update()
    {
        if (ballTransform == null) return;

        switch (currentState)
        {
            case AIState.Idle:           HandleIdle();    break;
            case AIState.MovingToTarget: HandleMoving();  break;
            case AIState.Jumping:        HandleJumping(); break;
            case AIState.Hitting:        HandleHitting(); break;
        }
    }

    // ── IDLE — รอลูกเข้ามาฝั่ง ───────────────────────────────
    void HandleIdle()
    {
        if (ballTransform.position.x > 0.5f)
        {
            // ลูกอยู่ฝั่ง AI → ไปหาลูก
            float tx = Mathf.Clamp(ballTransform.position.x, minX, maxX);
            targetPos    = new Vector2(tx, transform.position.y);
            currentState = AIState.MovingToTarget;
        }
        else
        {
            // ลูกอยู่ฝั่ง player → กลับตำแหน่ง default
            float defaultX = (minX + maxX) * 0.5f;
            float newX     = Mathf.MoveTowards(
                transform.position.x, defaultX, moveSpeed * Time.deltaTime);
            newX = Mathf.Clamp(newX, minX, maxX);
            rb.linearVelocity = new Vector2(
                newX - transform.position.x,
                rb.linearVelocity.y);
        }
    }

    // ── MOVING — วิ่งหาลูก ───────────────────────────────────
    void HandleMoving()
    {
        // อัปเดต target ตามลูก (clamp ให้อยู่ใน boundary)
        if (ballTransform.position.x > 0.5f)
        {
            float tx  = Mathf.Clamp(ballTransform.position.x, minX, maxX);
            targetPos = new Vector2(tx, transform.position.y);
        }

        float dist = Mathf.Abs(transform.position.x - targetPos.x);

        float newX = Mathf.MoveTowards(
            transform.position.x,
            targetPos.x,
            moveSpeed * Time.deltaTime);

        // ── Clamp อยู่แค่ใน GroundRight ───────────────────────
        newX = Mathf.Clamp(newX, minX, maxX);

        rb.linearVelocity = new Vector2(
            newX - transform.position.x,
            rb.linearVelocity.y);

        // กระโดดเฉพาะตอนถึงจุดและลูกใกล้
        bool ballNear = Vector2.Distance(
            transform.position, ballTransform.position) < 3f;
        bool ballOnAISide = ballTransform.position.x > 0.5f;

        if (dist < 0.5f && aiOnGround && ballNear && ballOnAISide)
            currentState = AIState.Jumping;

        // ลูกไปฝั่ง player แล้ว → กลับ Idle
        if (ballTransform.position.x <= 0.5f)
            currentState = AIState.Idle;
    }

    // ── JUMPING ───────────────────────────────────────────────
    void HandleJumping()
    {
        bool ballOnAISide = ballTransform.position.x > 0.5f;

        if (isGrounded && ballOnAISide)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded   = false;
            aiOnGround   = false;
            currentState = AIState.Hitting;
        }
        else if (!ballOnAISide)
        {
            currentState = AIState.Idle;
        }
    }

    // ── HITTING — ตีลูกไปฝั่ง player ────────────────────────
    void HandleHitting()
    {
        if (ballTransform == null) { currentState = AIState.Idle; return; }

        float distToBall = Vector2.Distance(
            transform.position, ballTransform.position);

        if (distToBall <= hitRange)
        {
            Vector2 hitDir = new Vector2(-1f, 0.6f).normalized;
            ballRb.linearVelocity = hitDir * hitForce;
            currentState = AIState.Idle;
            return;
        }

        if (aiOnGround)
            currentState = AIState.Idle;
    }

    // ── Ground Detection ──────────────────────────────────────
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            aiOnGround = true;
            if (currentState == AIState.Hitting ||
                currentState == AIState.Jumping)
                currentState = AIState.Idle;
        }
    }

    // ── Debug: แสดงขอบเขตใน Scene view ──────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            new Vector3(minX, transform.position.y - 2f, 0),
            new Vector3(minX, transform.position.y + 2f, 0));
        Gizmos.DrawLine(
            new Vector3(maxX, transform.position.y - 2f, 0),
            new Vector3(maxX, transform.position.y + 2f, 0));
    }
}