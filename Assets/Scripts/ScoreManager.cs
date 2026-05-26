using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ScoreManager — เก็บคะแนนด้วย Dictionary&lt;string, int&gt;
/// Data Structure: Dictionary
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI aiScoreText;

    [Header("Config")]
    public int winScore = 5;

    // ── DATA STRUCTURE: Dictionary<string, int> ──────────────
    // key = "player" / "ai"   value = คะแนน
    private Dictionary<string, int> scores = new Dictionary<string, int>();

    public delegate void GameEndDelegate(string winnerKey);
    public event GameEndDelegate OnGameEnd;

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

    void Start() => ResetScores();

    public void ResetScores()
    {
        scores.Clear();
        scores["player"] = 0;
        scores["ai"]     = 0;
        UpdateUI();
    }

    public void AddPoint(string key)
    {
        if (!scores.ContainsKey(key)) return;
        scores[key]++;
        UpdateUI();
        CheckWin();
    }

    public int GetScore(string key) =>
        scores.ContainsKey(key) ? scores[key] : 0;

    void UpdateUI()
    {
        if (playerScoreText) playerScoreText.text = scores["player"].ToString();
        if (aiScoreText)     aiScoreText.text     = scores["ai"].ToString();
    }

    void CheckWin()
    {
        foreach (var kvp in scores)
            if (kvp.Value >= winScore) { OnGameEnd?.Invoke(kvp.Key); return; }
    }
}
