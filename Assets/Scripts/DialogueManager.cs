using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : SceneBase
{
    // ── UI References ──────────────────────────────────────
    public TMP_Text speakerNameText;   // who is speaking
    public TMP_Text dialogueText;      // the line of dialogue
    public TMP_Text tapHintText;       // 'Tap to continue...'
    public Button continueButton;      // appears when dialogue is done
    public Image delegateImage;        // delegate portrait
    public Button dialoguePanel;       // the tappable area

    // ── Typewriter Settings ────────────────────────────────
    public float typewriterSpeed = 0.03f; // seconds per character

    // ── State ──────────────────────────────────────────────
    private DialogueLine[] lines;
    private int currentIndex = 0;
    private bool isTyping = false;
    private bool dialogueDone = false;
    private Coroutine typewriterCoroutine;

    // ── Start ──────────────────────────────────────────────
    void Start()
    {
        continueButton.gameObject.SetActive(false);
        continueButton.transition = Selectable.Transition.None;
        continueButton.onClick.AddListener(() => StartCoroutine(LoadScene("Decision")));

        // Wire tap panel
        dialoguePanel.onClick.AddListener(OnTap);
        dialoguePanel.transition = Selectable.Transition.None;

        // Load dialogue from StoryLoader
        if (StoryLoader.instance != null && StoryLoader.instance.currentStory != null)
        {
            lines = StoryLoader.instance.currentStory.dialogue;
        }
        else
        {
            // Fallback — create one placeholder line
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = "Narrator", text = "Story loading..." }
            };
        }

        ShowLine(0);
    }

    // ── On Tap ─────────────────────────────────────────────
    void OnTap()
    {
        if (dialogueDone) return;

        if (isTyping)
        {
            // Skip typewriter — show full line immediately
            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
            dialogueText.text = lines[currentIndex].text;
            isTyping = false;
            return;
        }

        // Advance to next line
        currentIndex++;
        if (currentIndex >= lines.Length)
        {
            // All lines done
            dialogueDone = true;
            tapHintText.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
            return;
        }

        ShowLine(currentIndex);
    }

    // ── Show Line ──────────────────────────────────────────
    void ShowLine(int index)
    {
        speakerNameText.text = lines[index].speaker;
        dialogueText.text = "";
        typewriterCoroutine = StartCoroutine(TypewriterEffect(lines[index].text));

        // Change delegate sprite if specified
        // if (!string.IsNullOrEmpty(lines[index].sprite))
        //     delegateImage.sprite = Resources.Load<Sprite>("Sprites/" + lines[index].sprite);
    }

    // ── Typewriter Effect ──────────────────────────────────
    IEnumerator TypewriterEffect(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        isTyping = false;
    }
}