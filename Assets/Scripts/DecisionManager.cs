using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DecisionManager : SceneBase
{
    // ── UI References ─────────────────────────────────────
    public TMP_Text StoryTitle;
    public TMP_Text QuestionText;
    public Image    DelegatePlaceholder;
    public Button   CardA;
    public Button   CardB;
    public TMP_Text CardALabel;       // text on Card A button
    public TMP_Text CardBLabel;       // text on Card B button
    public TMP_Text PreambleText;
    public Button   ConfirmBtn;
    public Button   CancelButton;

    // ── State ─────────────────────────────────────────────
    public int decisionValue = 0;
    private string preambleA = "";
    private string preambleB = "";

    // ─────────────────────────────────────────────────────
    void Start()
    {
        // Fix color tinting
        CardA.transition       = Selectable.Transition.None;
        CardB.transition       = Selectable.Transition.None;
        ConfirmBtn.transition  = Selectable.Transition.None;
        CancelButton.transition = Selectable.Transition.None;

        // Hide preamble UI
        PreambleText.gameObject.SetActive(false);
        ConfirmBtn.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);

        // Wire buttons
        CardA.onClick.AddListener(OnCardAClick);
        CardB.onClick.AddListener(OnCardBClick);
        ConfirmBtn.onClick.AddListener(OnConfirmClick);
        CancelButton.onClick.AddListener(OnCancelClick);

        // Load content based on which story was picked
        LoadStoryContent(GameState.instance.currentStory);
    }

    // ─────────────────────────────────────────────────────
    void LoadStoryContent(int story)
    {
        if (story == 0) // Education
        {
            StoryTitle.text  = "The Empty Page";
            QuestionText.text = "The district education committee is meeting. Two proposals are on the table.";
            if (CardALabel) CardALabel.text = "The Expansion — Build 200 new schools";
            if (CardBLabel) CardBLabel.text = "The Foundation Fix — Train teachers, reduce class sizes";
            preambleA = "More schools means more children reached. Infrastructure is the foundation of everything that follows.";
            preambleB = "Freezing construction feels like going backwards. Can you really ask communities to wait longer?";
        }
        else if (story == 1) // Galamsey
        {
            StoryTitle.text  = "The River Doesn't Lie";
            QuestionText.text = "A coalition has been called. The river is orange. Two proposals are on the table.";
            if (CardALabel) CardALabel.text = "The Crackdown — Deploy enforcement, seize equipment";
            if (CardBLabel) CardBLabel.text = "The Alternative — Fund real livelihoods, training, legal licences";
            preambleA = "Immediate enforcement sends a message. The river cannot wait for slow programmes to take effect.";
            preambleB = "Alternative programmes take years. Is there really time while the river keeps running orange?";
        }
    }

    // ─────────────────────────────────────────────────────
    void OnCardSelected(Button card, string preamble)
    {
        card.image.color = Color.green;
        Button other = (card == CardA) ? CardB : CardA;
        other.image.color = Color.white;
        PreambleText.text = preamble;
        PreambleText.gameObject.SetActive(true);
        ConfirmBtn.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(true);
    }

    public void OnCardAClick()
    {
        decisionValue = 1;
        OnCardSelected(CardA, preambleA);
    }

    public void OnCardBClick()
    {
        decisionValue = 2;
        OnCardSelected(CardB, preambleB);
    }

    public void OnConfirmClick()
    {
        GameState.instance.decisionMade = decisionValue;
        StartCoroutine(LoadScene("MiniGame"));
    }

    public void OnCancelClick()
    {
        PreambleText.gameObject.SetActive(false);
        ConfirmBtn.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
        decisionValue = 0;
        CardA.image.color = Color.white;
        CardB.image.color = Color.white;
    }
}