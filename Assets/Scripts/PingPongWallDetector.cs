using UnityEngine;

// ══════════════════════════════════════════════════════════════
// PingPongWallDetector — Ping Pong
// Attach กับ Wall Collider ซ้าย/ขวา (Is Trigger)
// ══════════════════════════════════════════════════════════════
public class PingPongWallDetector : MonoBehaviour
{
    public string side = "player";

    private PingPongController pp;

    void Start() => pp = FindFirstObjectByType<PingPongController>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
            pp?.BallOutOfBounds(side);
    }
}