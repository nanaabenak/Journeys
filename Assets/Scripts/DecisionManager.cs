using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DecisionManager : SceneBase
{
    public TMP_Text StoryTitle; // reference to the story title text component  
    public TMP_Text QuestionText; // reference to the question text component
    public Image DelegatePlaceholder; // reference to the delegate placeholder image component
    public Button CardA;    // reference to the card A button
    public Button CardB;    // reference to the card B button
    public TMP_Text PreambleText; // reference to the preamble text component
    public Button ConfirmBtn;// reference to the confirm button
    public Button CancelButton; // reference to the cancel button
    public int decisionValue; // 0 for no decision, 1 for card A, 2 for card B
    public string cardText; // text to display in the preamble based on the selected card

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Hiding preamble text ,confirm  and cancel button at the start of the scene
        PreambleText.gameObject.SetActive(false); // hide the preamble text at the start
        ConfirmBtn.gameObject.SetActive(false); // hide the confirm button at the start
        CancelButton.gameObject.SetActive(false); // hide the cancel button at the start

        // Story Title and Question Text are set based on the current story progress in the GameState
        if (GameState.instance.currentStory == 0)
        {
            StoryTitle.text = "The Educated";
            QuestionText.text = "You find yourself at a crossroads. Do you take the left path or the right path?";
        }
        else if (GameState.instance.currentStory == 1)
        {
            StoryTitle.text = "End Galemsey";
            QuestionText.text = "You are faced with a difficult decision. Do you choose option A or option B?";
        }
        else if (GameState.instance.currentStory == 2)
        {
            StoryTitle.text = "She Hurts For Nothing";
            QuestionText.text = "You are faced with a difficult decision. Do you choose option A or option B?";
        }
    }

    void OnCardHoverEnter(Button card)
    {
        // Change the color of the card to indicate hover state
        card.image.color = Color.yellow; // Change to a hover color (e.g., yellow)
    }

    void OnCardSelected(Button card, string preamble)
    {
        // Change the color of the card to indicate selection state
        card.image.color = Color.green; // Change to a selected color (e.g., green)
        Button otherCard = (card == CardA) ? CardB : CardA;
        otherCard.image.color = Color.white;
        PreambleText.text = preamble; // Set the preamble text based on the decision
        PreambleText.gameObject.SetActive(true); // Show the preamble text
        ConfirmBtn.gameObject.SetActive(true); // Show the confirm button
        CancelButton.gameObject.SetActive(true); // Show the cancel button

    }

    public void OnCardAClick()
    {
        decisionValue = 1; // Set decision value to 1 for card A
        cardText = "You have chosen option A. This decision will lead you down a path of knowledge and enlightenment, but it may also come with challenges and sacrifices. Are you sure you want to proceed with this choice?";
        OnCardSelected(CardA, cardText); // Call the card selected method for card A
    }

    public void OnCardBClick()
    {
        decisionValue = 2; // Set decision value to 2 for card B
        cardText = "You have chosen option B. This decision will lead you down a path of comfort and security, but it may also limit your growth and potential. Are you sure you want to proceed with this choice?";
        OnCardSelected(CardB, cardText); // Call the card selected method for card B
    }

    public void OnConfirmClick()
    {
        GameState.instance.decisionMade = decisionValue; // Store the decision value in the GameState
        // Load the minigame scene
        
        StartCoroutine(LoadScene("MiniGame"));
    }

    public void OnCancelClick()
    {
        
        PreambleText.gameObject.SetActive(false); // Hide the preamble text
        ConfirmBtn.gameObject.SetActive(false); // Hide the confirm button
        CancelButton.gameObject.SetActive(false); // Hide the cancel button
        decisionValue = 0; // Reset decision value to 0 for no decision
        CardA.image.color = Color.white; // Reset card A color
        CardB.image.color = Color.white; // Reset card B color
    }
}
