using UnityEngine;

/// <summary>
/// GameConfig — ScriptableObject เก็บ config ทุก mini-game
/// สร้าง asset: Right-click → Create → BeachChampion → GameConfig
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "BeachChampion/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Game Info")]
    public string   gameName;
    public GameType gameType;

    [Header("Score")]
    public int winScore  = 5;
    public int maxSwings = 5;   // Bat & Ball เท่านั้น

    [Header("Ball")]
    public float ballSpeed = 5f;

    [Header("AI")]
    [Range(0f, 1f)]
    public float aiDifficulty    = 0.5f;
    public float aiMoveSpeed     = 4f;
    public float aiJumpForce     = 8f;
    public float aiHitForce      = 6f;
    public float aiReactionDelay = 0.2f;

    [Header("Ads")]
    public int adEveryNRounds = 2;
}

public enum GameType { Volleyball, PingPong, BatBall }
