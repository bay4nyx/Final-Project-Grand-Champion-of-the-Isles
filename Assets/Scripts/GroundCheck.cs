using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public string owner = "player";

    private VolleyballController vc;

    void Start()
    {
        vc = FindFirstObjectByType<VolleyballController>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Ground")) return;

        if (owner == "player")
            vc?.SetPlayerGrounded();
        else
            vc?.SetAIGrounded();
    }
}