using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NegotiationManager : MonoBehaviour
{
    // ── UI References ───────────────────────────────────────
    public TMP_Text speakerNameText;      // delegate speaker name
    public TMP_Text speakerLineText;      // delegate line
    public TMP_Text responseNameText;     // stakeholder name
    public TMP_Text responseLineText;     // stakeholder response
    public Image panelBackground;         // changes color with momentum

    // Stakeholder portraits — assign in Inspector
    // For Oware/Dame: just index 0 is used
    // For Ludo: all three are used
    public Image[] stakeholderPortraits = new Image[3];

    // ── State ───────────────────────────────────────────────
    private NegotiationData negData;
    private int currentMomentum = 1; // 0=losing 1=even 2=winning
    private int[] momentumLineIndex = new int[3]; // tracks which line to show per state
    private HashSet<string> consumedMilestones = new HashSet<string>();
    private float lastDialogueTime = 0f;
    private float fallbackInterval = 12f;
    private bool isDisplaying = false;

    // ── Start ───────────────────────────────────────────────
    void Start()
    {
        // Guard — StoryLoader may not be ready in early testing
        if (StoryLoader.instance == null || StoryLoader.instance.currentStory == null)
        {
            Debug.LogWarning("NegotiationManager: No story loaded. Skipping negotiation.");
            return;
        }

        negData = StoryLoader.instance.currentStory.negotiation;
        lastDialogueTime = Time.time;

        // Clear display on start
        ClearDisplay();
    }

    // ── Update — Time Fallback ───────────────────────────────
    void Update()
    {
        if (negData == null || isDisplaying) return;
        if (Time.time - lastDialogueTime > fallbackInterval)
            ShowMomentumLine(currentMomentum);
    }

    // ── Called by game managers after every move ─────────────
    // playerScore = player's current score, aiScore = AI's current score
    public void OnMoveCompleted(int playerScore, int aiScore)
    {
        if (negData == null) return;

        int newMomentum = CalculateMomentum(playerScore, aiScore);
        if (newMomentum != currentMomentum)
        {
            currentMomentum = newMomentum;
            ShowMomentumLine(currentMomentum);
            UpdatePanelColor(currentMomentum);
        }
    }

    // ── Called by game managers on specific events ───────────
    public void OnMilestone(string triggerKey)
    {
        if (negData == null) return;
        if (consumedMilestones.Contains(triggerKey)) return;
        consumedMilestones.Add(triggerKey);

        foreach (NegotiationExchange exchange in negData.milestones)
        {
            if (exchange.trigger == triggerKey)
            {
                StartCoroutine(DisplayExchange(exchange));
                return;
            }
        }
    }

    // ── Show Momentum Line ───────────────────────────────────
    void ShowMomentumLine(int momentum)
    {
        NegotiationExchange[] lines = momentum == 0 ? negData.momentum.losing.lines
                                    : momentum == 2 ? negData.momentum.winning.lines
                                    : negData.momentum.even.lines;

        if (lines == null || lines.Length == 0) return;
        int index = momentumLineIndex[momentum] % lines.Length;
        momentumLineIndex[momentum]++;
        StartCoroutine(DisplayExchange(lines[index]));
    }

    // ── Display Exchange ─────────────────────────────────────
    IEnumerator DisplayExchange(NegotiationExchange exchange)
    {
        isDisplaying = true;
        lastDialogueTime = Time.time;

        // Highlight active stakeholder portrait for Ludo
        HighlightStakeholder(exchange.active_stakeholder_index);

        // Show delegate line
        speakerNameText.text = exchange.speaker;
        speakerLineText.text = exchange.text;
        responseNameText.text = "";
        responseLineText.text = "";

        yield return new WaitForSeconds(3f);

        // Show stakeholder response
        responseNameText.text = exchange.response_speaker;
        responseLineText.text = exchange.response_text;

        yield return new WaitForSeconds(3f);

        isDisplaying = false;
    }

    // ── Highlight Stakeholder ────────────────────────────────
    void HighlightStakeholder(int index)
    {
        for (int i = 0; i < stakeholderPortraits.Length; i++)
        {
            if (stakeholderPortraits[i] == null) continue;
            // Active stakeholder is full brightness, others dimmed
            stakeholderPortraits[i].color = (i == index)
                ? Color.white
                : new Color(0.5f, 0.5f, 0.5f);
        }
    }

    // ── Update Panel Color ───────────────────────────────────
    void UpdatePanelColor(int momentum)
    {
        if (panelBackground == null) return;
        if (momentum == 0) panelBackground.color = new Color(1.00f, 0.92f, 0.92f); // losing — light red
        else if (momentum == 2) panelBackground.color = new Color(0.92f, 1.00f,0.94f); // winning — light green
        else panelBackground.color = Color.white; // even
    }

    // ── Calculate Momentum ───────────────────────────────────
    int CalculateMomentum(int playerScore, int aiScore)
    {
        if (StoryLoader.instance == null) return 1;
        string game = StoryLoader.instance.currentStory.mini_game;
        int diff = playerScore - aiScore;
        int threshold = game == "oware" ? 4 : game == "dame" ? 2 : 2;
        if (diff >= threshold) return 2;  // winning
        if (diff <= -threshold) return 0; // losing
        return 1; // even
    }

    // ── Clear Display ────────────────────────────────────────
    void ClearDisplay()
    {
        speakerNameText.text = "";
        speakerLineText.text = "";
        responseNameText.text = "";
        responseLineText.text = "";
    }
}