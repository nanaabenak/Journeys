using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DecisionManagerTwo : SceneBase
{
    public TMP_Text StoryTitle;
    public TMP_Text QuestionText;
    public Image DelegatePlaceholder;
    public Button CardA;
    public TMP_Text CardALabel;
    public TMP_Text CardBLabel;
    public TMP_Text CardADescription;
    public TMP_Text CardBDescription;
    public Button CardB;
    public Image preambleBackground;
    public TMP_Text PreambleText;
    public Button ConfirmBtn;
    public Button CancelButton;
    public int decisionValue;
    public string cardText;

    // Store preambles loaded from JSON
    private string preambleA;
    private string preambleB;

    void Start()
    {
        PreambleText.gameObject.SetActive(false);
        preambleBackground.gameObject.SetActive(false);
        ConfirmBtn.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);



        // DecisionData decision = StoryLoader.instance.currentStory.decision;
        // StoryTitle.text = decision.title;
        // QuestionText.text = decision.question;
        // CardALabel.text = decision.option_a.label;
        // CardADescription.text = decision.option_a.description;
        // preambleA = decision.option_a.preamble;
        // CardBLabel.text = decision.option_b.label;
        // CardBDescription.text = decision.option_b.description;
        // preambleB = decision.option_b.preamble;
        // Try to load from StoryLoader — fall back to hardcoded if not ready
        if (StoryLoader.instance != null && StoryLoader.instance.currentStory != null)
        {
            DecisionData decision = StoryLoader.instance.currentStory.decision;
            StoryTitle.text = decision.title;
            QuestionText.text = decision.question;
            CardALabel.text = decision.option_a.label;
            CardADescription.text = decision.option_a.description;
            preambleA = decision.option_a.preamble;
            CardBLabel.text = decision.option_a.label;
            CardBDescription.text = decision.option_a.description;
            preambleB = decision.option_b.preamble;
        }
        else
        {
            // Fallback — StoryLoader not ready yet
            if (GameState.instance.currentStory == 0)
            {
                StoryTitle.text = "The Empty Page";
                QuestionText.text = "The district education committee is meeting. Two proposals are on the table.";
                preambleA = "More schools means more children reached. Infrastructure is the foundation of everything.";
                preambleB = "Freezing construction feels like going backwards. Can you really ask communities to wait longer?";
            }
            else if (GameState.instance.currentStory == 1)
            {
                StoryTitle.text = "The River Doesn't Lie";
                QuestionText.text = "A coalition meeting has been called. Two proposals are on the table.";
                preambleA = "Immediate enforcement sends a message. The river cannot wait for slow programmes.";
                preambleB = "Alternative programmes take years. Is there really time for this?";
            }
            else if (GameState.instance.currentStory == 2)
            {
                StoryTitle.text = "The Path Home";
                QuestionText.text = "The district health council is convening. Two approaches are proposed.";
                preambleA = "The law exists for a reason. Girls cannot wait for conversations to reach the right conclusion.";
                preambleB = "Conversation takes years. While we talk, girls are being taken across borders.";
            }
        }
    }

    void OnCardSelected(Button card, string preamble)
    {
        card.image.color = Color.green;
        Button otherCard = (card == CardA) ? CardB : CardA;
        otherCard.image.color = Color.white;
        PreambleText.text = preamble;
        preambleBackground.gameObject.SetActive(true);
        PreambleText.gameObject.SetActive(true);
        ConfirmBtn.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(true);
    }

    public void OnCardAClick()
    {
        decisionValue = 1;
        OnCardSelected(CardA, preambleA); // uses preamble loaded from JSON
    }

    public void OnCardBClick()
    {
        decisionValue = 2;
        OnCardSelected(CardB, preambleB); // uses preamble loaded from JSON
    }

    public void OnConfirmClick()
    {
        GameState.instance.decisionMade = decisionValue;
        StartCoroutine(LoadScene("MiniGame"));
    }

    public void OnCancelClick()
    {
        preambleBackground.gameObject.SetActive(false);
        PreambleText.gameObject.SetActive(false);
        ConfirmBtn.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(false);
        decisionValue = 0;
        CardA.image.color = Color.white;
        CardB.image.color = Color.white;
    }
}