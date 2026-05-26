using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BatBallController : MonoBehaviour
{
    [Header("Config")]
    public int   maxSwings     = 5;
    public float timingWindow  = 0.5f;
    public float pitchInterval = 2f;
    public float minDistance   = 50f;
    public float maxDistance   = 500f;

    [Header("Ball")]
    public Rigidbody2D ballRb;
    public Transform   ballSpawnPoint;

    [Header("UI")]
    public TextMeshProUGUI swingCountText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI lastDistText;
    public GameObject      resultPanel;
    public TextMeshProUGUI resultText;

    // List<int> เก็บระยะทางแต่ละ swing
    private List<int> swingDistances = new List<int>();

    private int   currentSwing  = 0;
    private float pitchTime     = 0f;
    private bool  waitingForHit = false;
    private bool  gameOver      = false;
    [Header("AI Throw")]
public float throwSpeed = 12f;

    void Start()
    {
        if (resultPanel) resultPanel.SetActive(false);
        swingDistances.Clear();
        currentSwing  = 0;
        gameOver      = false;
        waitingForHit = false;
        UpdateSwingUI();
        StartCoroutine(NextPitch());
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
        Debug.Log($"Space กด! waiting={waitingForHit} gameOver={gameOver}");

    if (gameOver)       return;
    if (!waitingForHit) return;

    if (Input.GetKeyDown(KeyCode.Space))
    {
        waitingForHit = false;
        StopAllCoroutines();
        ProcessHit();
    }
}

    IEnumerator NextPitch()
{
    if (currentSwing >= maxSwings) { EndGame(); yield break; }

    // Reset ลูกไปที่ AI ก่อน
    if (ballRb != null)
    {
        ballRb.linearVelocity  = Vector2.zero;
        ballRb.angularVelocity = 0f;
        if (ballSpawnPoint != null)
            ballRb.transform.position = ballSpawnPoint.position;
    }

    waitingForHit = false;
    SetFeedback("...", Color.gray);
    yield return new WaitForSeconds(1f);

    // AI ขว้างลูกมาหา Player
    if (ballRb != null)
    {
        float throwSpeed = 12f;
        ballRb.linearVelocity = new Vector2(-throwSpeed, 1f);
    }

    yield return new WaitForSeconds(1f);

    // พร้อมให้ตี
    currentSwing++;
    pitchTime     = Time.time;
    waitingForHit = true;
    UpdateSwingUI();
    SetFeedback("TAP!", Color.white);

    yield return new WaitForSeconds(timingWindow * 3f);
    if (waitingForHit)
    {
        waitingForHit = false;
        Miss();
    }
}
    void ProcessHit()
    {
        
        Debug.Log($"ProcessHit! ballRb={ballRb}");
    float elapsed = Time.time - pitchTime;
    float offset  = Mathf.Abs(elapsed - (pitchInterval * 0.5f));
    Debug.Log($"elapsed={elapsed} offset={offset} window={timingWindow}");
        if (offset <= timingWindow)
        {
            float  acc   = 1f - (offset / timingWindow);
            int    dist  = Mathf.RoundToInt(Mathf.Lerp(minDistance, maxDistance, acc));
            string grade = acc > 0.85f ? "PERFECT!" :
                           acc > 0.5f  ? "GOOD"     : "OK";

            swingDistances.Add(dist);

            Color col = acc > 0.85f ? Color.yellow : Color.green;
            SetFeedback($"{grade} {dist}m", col);
            if (lastDistText) lastDistText.text = $"Last: {dist}m";

            // ยิงลูก
            if (ballRb != null)
{
    float power = Mathf.Lerp(3f, 15f, acc);
    ballRb.linearVelocity = Vector2.zero;
    // เพิ่ม Y ให้สูงขึ้น
    ballRb.AddForce(new Vector2(-power * 0.5f, power * 2f), ForceMode2D.Impulse);
}
        }
        else
        {
            Miss();
            return;
        }

        StartCoroutine(NextPitch());
    }

    void Miss()
    {
        swingDistances.Add(0);
        SetFeedback("Miss!", Color.red);
        StartCoroutine(NextPitch());
    }

    void EndGame()
    {
        gameOver = true;

        // Bubble Sort
        BubbleSort(swingDistances);

        int best  = swingDistances.Count > 0 ? swingDistances[0] : 0;
        int worst = swingDistances.Count > 0 ? swingDistances[swingDistances.Count - 1] : 0;
        int total = 0;
        for (int i = 0; i < swingDistances.Count; i++)
            total += swingDistances[i];
        int avg  = swingDistances.Count > 0 ? total / swingDistances.Count : 0;
        int hits = 0;
        for (int i = 0; i < swingDistances.Count; i++)
            if (swingDistances[i] > 0) hits++;

        if (resultPanel) resultPanel.SetActive(true);
        if (resultText)
            resultText.text =
                resultText.text =
    $"Game Over!\n"                +
    $"Hit: {hits}/{maxSwings}\n"   +
    $"Best: {best}m\n"             +
    $"Avg: {avg}m\n"               +
    $"Total: {total}m";

        // ส่งไป Leaderboard
        LeaderboardManager.Instance?.AddEntry("Player", best, "BatBall");
        AdManager.Instance?.OnGameFinished();
    }

    void BubbleSort(List<int> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - 1 - i; j++)
                if (list[j] < list[j + 1])
                {
                    int tmp    = list[j];
                    list[j]    = list[j + 1];
                    list[j + 1] = tmp;
                }
    }

    void UpdateSwingUI()
    {
        if (swingCountText)
            swingCountText.text = $"Swing {currentSwing}/{maxSwings}";
    }

    void SetFeedback(string msg, Color color)
    {
        if (!feedbackText) return;
        feedbackText.text  = msg;
        feedbackText.color = color;
    }

    public void PlayAgain() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void BackToHub() =>
        SceneManager.LoadScene("HubWorld");

        // เรียกจากปุ่ม BtnTap
public void OnTapButton()
{
    if (gameOver || !waitingForHit) return;

    waitingForHit = false;
    StopAllCoroutines();
    ProcessHit();
}
}
