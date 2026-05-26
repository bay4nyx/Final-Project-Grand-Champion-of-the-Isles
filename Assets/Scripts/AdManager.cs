using UnityEngine;

/// <summary>
/// AdManager — จัดการ Interstitial Ad ด้วย PlayerPrefs
/// Flow ⑥: GetInt → Ad cooldown → SetInt
/// Data Management: PlayerPrefs เก็บ adCount ข้าม session
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [Header("Config")]
    [Tooltip("แสดง Ad ทุกกี่รอบ")]
    public int adEveryNRounds = 2;

    // PlayerPrefs key
    private const string AD_COUNT_KEY = "adCount";

    void Awake()
    {
        if (Instance != null && Instance != this)
        { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// เรียกหลังจบแต่ละ mini-game
    /// ถ้าครบ N รอบ จะแสดง Ad
    /// </summary>
    public void OnGameFinished()
    {
        // ── PlayerPrefs: GetInt ───────────────────────────────
        int count = PlayerPrefs.GetInt(AD_COUNT_KEY, 0);
        count++;

        // ── PlayerPrefs: SetInt ───────────────────────────────
        PlayerPrefs.SetInt(AD_COUNT_KEY, count);
        PlayerPrefs.Save();

        Debug.Log($"[AdManager] Round count: {count}");

        // ── Ad Cooldown: ทุก N รอบ แสดง Ad ──────────────────
        if (count % adEveryNRounds == 0)
        {
            ShowInterstitialAd();
        }
    }

    void ShowInterstitialAd()
    {
        // TODO: ใส่ Unity Ads SDK หรือ AdMob ตรงนี้
        // ตัวอย่าง: Advertisement.Show("Interstitial");
        Debug.Log("[AdManager] Showing Interstitial Ad!");
    }

    /// <summary>Reset counter (ใช้สำหรับ test)</summary>
    public void ResetCount()
    {
        PlayerPrefs.SetInt(AD_COUNT_KEY, 0);
        PlayerPrefs.Save();
    }

    /// <summary>ดูค่าปัจจุบัน</summary>
    public int GetCurrentCount() =>
        PlayerPrefs.GetInt(AD_COUNT_KEY, 0);
}
