using UnityEngine;

/// <summary>
/// NPCInteraction — รองรับปุ่ม E (keyboard) + ปุ่ม Talk (mobile)
/// </summary>
public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName = "Knight Champion";

    [TextArea(2, 4)]
    public string message = "Hello, adventurer! Are you ready to compete?";

    [Header("Scene ปลายทาง")]
    public string sceneToLoad = "";

    [Header("Optional")]
    public GameObject interactPromptUI;

    private bool playerInRange = false;
    private bool alreadyOpen   = false;

    void Start()
    {
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || alreadyOpen) return;

        // กด E (keyboard) หรือ ปุ่ม Talk (mobile)
        bool interacted = Input.GetKeyDown(KeyCode.E)
                       || MobileButton.TalkPressed;

        if (interacted)
        {
            alreadyOpen = true;
            if (interactPromptUI != null)
                interactPromptUI.SetActive(false);
            DialogueManager.Instance.ShowDialogue(npcName, message, sceneToLoad);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        alreadyOpen   = false;
        if (interactPromptUI != null)
            interactPromptUI.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
    }
}
