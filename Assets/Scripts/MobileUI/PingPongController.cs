using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// PingPongController — รองรับปุ่ม Up/Down บนมือถือ
/// </summary>
public class PingPongController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D ballRb;
    public Transform   playerPaddle;
    public Transform   aiPaddle;

    [Header("Ball")]
    public float ballSpeed  = 6f;
    public float serveAngle = 0.3f;

    [Header("Paddle")]
    public float paddleSpeed = 8f;
    public float paddleMinY  = -2.5f;
    public float paddleMaxY  =  2.5f;

    [Header("AI")]
    [Range(0f, 1f)]
    public float aiDifficulty = 0.6f;

    [Header("UI")]
    public GameObject      resultPanel;
    public TextMeshProUGUI resultText;

    private bool gameActive = false;

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
        MovePlayerPaddle();
        MoveAIPaddle();
    }

    void MovePlayerPaddle()
    {
        // ── Keyboard + Mobile Up/Down ─────────────────────────
        float input = Input.GetAxisRaw("Vertical");

        if (MobileButton.UpHeld)   input =  1f;
        if (MobileButton.DownHeld) input = -1f;

        // Mobile drag (touch)
        if (Input.touchCount > 0 && input == 0)
            input = Input.GetTouch(0).deltaPosition.y * 0.05f;

        float newY = Mathf.Clamp(
            playerPaddle.position.y + input * paddleSpeed * Time.deltaTime,
            paddleMinY, paddleMaxY);
        playerPaddle.position = new Vector3(
            playerPaddle.position.x, newY, 0f);
    }

    void MoveAIPaddle()
    {
        float targetY = Mathf.Lerp(
            aiPaddle.position.y,
            ballRb.position.y,
            aiDifficulty * Time.deltaTime * 5f);
        targetY = Mathf.Clamp(targetY, paddleMinY, paddleMaxY);
        aiPaddle.position = new Vector3(aiPaddle.position.x, targetY, 0f);
    }

    IEnumerator Serve()
    {
        gameActive = false;
        ballRb.linearVelocity = Vector2.zero;
        ballRb.transform.position = Vector3.zero;
        yield return new WaitForSeconds(0.8f);
        float dir = Random.value > 0.5f ? 1f : -1f;
        ballRb.linearVelocity = new Vector2(
            ballSpeed * dir, ballSpeed * serveAngle);
        gameActive = true;
    }

    public void BallOutOfBounds(string side)
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
}
