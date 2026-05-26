using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// ═══════════════════════════════════════════════════════════════
// StoryManager — สไลด์รูป 3-4 หน้า ก่อนเข้า MainMenu
// ═══════════════════════════════════════════════════════════════
public class StoryManager : MonoBehaviour
{
    [Header("UI References")]
    public Image           storyImage;      // รูปใหญ่
    public TextMeshProUGUI storyText;       // ข้อความใต้รูป
    public TextMeshProUGUI pageIndicator;   // "1 / 4"
    public GameObject      nextButton;
    public GameObject      skipButton;

    [Header("Story Pages")]
    public Sprite[] pageImages;             // รูป 3-4 รูป — ลาก Sprite มาใส่
    [TextArea(2, 5)]
    public string[] pageTexts;             // ข้อความแต่ละหน้า

    [Header("Next Scene")]
    public string mainMenuScene = "MainMenuScene";

    private int currentPage = 0;

    void Start()
    {
        currentPage = 0;
        ShowPage(0);
    }

    public void OnNextPressed()
    {
        currentPage++;
        if (currentPage >= pageImages.Length)
            SceneManager.LoadScene(mainMenuScene);
        else
            ShowPage(currentPage);
    }

    public void OnSkipPressed()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    void ShowPage(int index)
    {
        if (index < pageImages.Length && pageImages[index] != null)
            storyImage.sprite = pageImages[index];

        if (index < pageTexts.Length)
            storyText.text = pageTexts[index];

        if (pageIndicator)
            pageIndicator.text = $"{index + 1} / {pageImages.Length}";

        // หน้าสุดท้าย เปลี่ยน Next → "เริ่มเกม!"
        if (nextButton != null)
        {
            var label = nextButton.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = (index == pageImages.Length - 1)
                    ? "play!" : "Next >";
        }
    }
}