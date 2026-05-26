using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// VolleyballController — รองรับปุ่ม Left/Right/Jump บนมือถือ
/// </summary>
public class VolleyballController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D ballRb;
    public Rigidbody2D playerRb;

    [Header("Ball")]
    public float serveForceY = 7f;
    public float serveForceX = 3f;

    [Header("Player")]
    public float playerSpeed = 5f;
    public float jumpForce   = 8f;

    [Header("AI")]
    public float aiSpeed = 3.5f;

    [Header("UI")]
    public GameObject      resultPanel;
    public TextMeshProUGUI resultText;

    private bool gameActive     = false;
    private bool playerGrounded = true;
    private bool aiOnGround     = true;

    void Start()
    {
        resultPanel?.SetActive(false);
        ScoreManager.Instance.OnGameEnd += ShowResult;
        ScoreManager.Instance.ResetScores();
        StartCoroutine(Serve());
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnGameEnd -= ShowResult;
    }

    void Update()
    {
        if (!gameActive) return;
        MovePlayer();
    }

    void MovePlayer()
{
    float h = Input.GetAxisRaw("Horizontal");
    if (MobileButton.LeftHeld)  h = -1f;
    if (MobileButton.RightHeld) h =  1f;

    float newX = playerRb.position.x + h * playerSpeed * Time.fixedDeltaTime;

    // จำกัดให้ Player อยู่ฝั่งซ้ายเท่านั้น (x < -0.5)
    newX = Mathf.Min(newX, -0.5f);

    playerRb.linearVelocity = new Vector2(
        h * playerSpeed,
        playerRb.linearVelocity.y);

    bool jump = Input.GetKeyDown(KeyCode.Space)
             || MobileButton.JumpPressed;

    if (jump && playerGrounded)
    {
        playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        playerGrounded = false;
    }
    }

    void FixedUpdate()
    {
        if (!gameActive) return;
        MoveAI();
    }

    void MoveAI()
    {
        float dir = Mathf.Sign(ballRb.position.x - aiRb.position.x);
        aiRb.linearVelocity = new Vector2(dir * aiSpeed, aiRb.linearVelocity.y);

        float dist = Vector2.Distance(ballRb.position, aiRb.position);
        if (dist < 1.6f && aiOnGround)
        {
            aiRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            aiOnGround = false;
        }
    }

    [Header("AI Rigidbody")]
    public Rigidbody2D aiRb;

    IEnumerator Serve()
    {
        gameActive = false;
        ballRb.linearVelocity = Vector2.zero;
        ballRb.transform.position = Vector3.zero;
        yield return new WaitForSeconds(1f);
        float dir = Random.value > 0.5f ? 1f : -1f;
        ballRb.AddForce(new Vector2(serveForceX * dir, serveForceY),
                        ForceMode2D.Impulse);
        gameActive = true;
    }

    public void BallHitGround(string side)
    {
        ScoreManager.Instance.AddPoint(side == "player" ? "ai" : "player");
        if (gameActive) StartCoroutine(Serve());
    }

    void ShowResult(string winner)
    {
        gameActive = false;
        ballRb.linearVelocity = Vector2.zero;
        resultPanel?.SetActive(true);
        if (resultText != null)
            resultText.text = winner == "player" ? "You Win!" : "You Lose!";
    }

    public void PlayAgain() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void BackToHub() =>
        SceneManager.LoadScene("HubWorld");

    public void SetPlayerGrounded() => playerGrounded = true;
    public void SetAIGrounded()     => aiOnGround     = true;
}
