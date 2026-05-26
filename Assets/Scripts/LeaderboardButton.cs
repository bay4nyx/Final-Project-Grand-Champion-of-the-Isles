using UnityEngine;

public class LeaderboardButton : MonoBehaviour
{
    public void OnPressed()
    {
        Debug.Log($"[Button] Pressed! Instance={LeaderboardManager.Instance}");
        if (LeaderboardManager.Instance == null)
        {
            Debug.Log("[Button] Instance is NULL!");
            return;
        }
        LeaderboardManager.Instance.ShowLeaderboard();
    }
}