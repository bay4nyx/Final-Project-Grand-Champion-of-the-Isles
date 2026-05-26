using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// DialogueManager — Dialogue Box + Scene Transition
/// Singleton pattern
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject      dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public Button          okButton;

    private string pendingScene = "";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
        okButton.onClick.AddListener(OnOKPressed);
    }

    public void ShowDialogue(string npcName, string message, string sceneToLoad = "")
    {
        pendingScene      = sceneToLoad;
        npcNameText.text  = npcName;
        dialogueText.text = message;
        dialoguePanel.SetActive(true);

        var pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) pc.canMove = false;
    }

    void OnOKPressed()
    {
        dialoguePanel.SetActive(false);

        if (!string.IsNullOrEmpty(pendingScene))
            SceneManager.LoadScene(pendingScene);
        else
        {
            var pc = FindFirstObjectByType<PlayerController>();
            if (pc != null) pc.canMove = true;
        }
    }
}
