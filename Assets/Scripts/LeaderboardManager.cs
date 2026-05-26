using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// LeaderboardManager — จัดการบอร์ดคะแนน
/// Flow ⑤: ScoreEntry struct → List → BubbleSort → แสดง Top 5
/// Data Structure: List<ScoreEntry>
/// Algorithm: Bubble Sort (เขียนเอง ไม่ใช้ LINQ)
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [Header("UI — Drag มาใส่")]
    public GameObject         leaderboardPanel;
    public TextMeshProUGUI[]  entryTexts;   // 5 ช่อง สำหรับ Top 5
    public int                maxEntries = 5;


    public void OnPressed()
    {
        LeaderboardManager.Instance?.ShowLeaderboard();
    }

    // ── DATA STRUCTURE: List<ScoreEntry> ─────────────────────
    private List<ScoreEntry> leaderboard = new List<ScoreEntry>();

    void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);
}

    void Start()
    {
        leaderboardPanel?.SetActive(false);
    }

    // ═══════════════════════════════════════════════════════════
    // PUBLIC API
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// เพิ่มคะแนนหลังจบแต่ละ mini-game
    /// เรียกจาก ScoreManager หรือ BatBallController
    /// </summary>
    public void AddEntry(string playerName, int score, string gameType)
{
    Debug.Log($"[Leaderboard] AddEntry called! {playerName} {score} {gameType}");
    leaderboard.Add(new ScoreEntry
    {
        playerName = playerName,
        score      = score,
        gameType   = gameType
    });
    BubbleSort(leaderboard);
    while (leaderboard.Count > maxEntries)
        leaderboard.RemoveAt(leaderboard.Count - 1);
}

public void ShowLeaderboard()
{
    Debug.Log($"[Leaderboard] Show! panel={leaderboardPanel} count={leaderboard.Count}");
    if (leaderboardPanel == null)
    {
        Debug.Log("[Leaderboard] Panel is NULL!");
        return;
    }
    leaderboardPanel.SetActive(true);
    UpdateUI();
}

    /// <summary>ปิด Leaderboard UI</summary>
    public void HideLeaderboard()
    {
        leaderboardPanel?.SetActive(false);
    }

    // ═══════════════════════════════════════════════════════════
    // BUBBLE SORT — เรียงจากมากไปน้อย
    // ═══════════════════════════════════════════════════════════
    void BubbleSort(List<ScoreEntry> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - 1 - i; j++)
            {
                if (list[j].score < list[j + 1].score)
                {
                    // swap
                    ScoreEntry temp = list[j];
                    list[j]         = list[j + 1];
                    list[j + 1]     = temp;
                }
            }
        }
    }

    // ═══════════════════════════════════════════════════════════
    // UPDATE UI
    // ═══════════════════════════════════════════════════════════
    void UpdateUI()
    {
        if (entryTexts == null) return;

        for (int i = 0; i < entryTexts.Length; i++)
        {
            if (entryTexts[i] == null) continue;

            if (i < leaderboard.Count)
            {
                ScoreEntry e = leaderboard[i];
                entryTexts[i].text =
                    $"{i + 1}.  {e.playerName}  |  {e.score}  |  {e.gameType}";
            }
            else
            {
                entryTexts[i].text = $"{i + 1}.  —";
            }
        }
    }

    // ── GETTER ────────────────────────────────────────────────
    public List<ScoreEntry> GetLeaderboard() =>
        new List<ScoreEntry>(leaderboard);
}

// ═══════════════════════════════════════════════════════════════
// ScoreEntry — Struct เก็บข้อมูลแต่ละ entry
// ใช้ struct เพราะข้อมูลเล็ก ไม่ต้องการ inheritance
// ═══════════════════════════════════════════════════════════════
[System.Serializable]
public struct ScoreEntry
{
    public string playerName;
    public int    score;
    public string gameType;    // "Volleyball" / "PingPong" / "BatBall"
}
