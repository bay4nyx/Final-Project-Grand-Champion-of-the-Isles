using UnityEngine;

// ══════════════════════════════════════════════════════════════
// BallGroundDetector — Volleyball
// Attach กับ Ground Collider แต่ละฝั่ง
// side = "player" (ฝั่ง player)  หรือ  "ai" (ฝั่ง AI)
// ══════════════════════════════════════════════════════════════
public class BallGroundDetector : MonoBehaviour
{
    public string side = "player";

    private VolleyballController vc;

    void Start() => vc = FindFirstObjectByType<VolleyballController>();

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ball"))
            vc?.BallHitGround(side);
    }
}
