using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WorldMapManager : SceneBase
{
    // Story Cards 
    // Drag the three story card buttons into these in the Inspector
    public Button storyCard1; // Education — Oware
    public Button storyCard2; // Galamsey — Dame
    public Button storyCard3; // FGM — Ludo

    // Card highlight colors
    private Color selectedCardColor   = new Color(0.0f, 0.54f, 0.48f);  // teal
    private Color unselectedCardColor = new Color(0.95f, 0.92f, 0.87f); // cream

    // Start 
    void Start()
    {
        // Fix button transitions so color changes work
        storyCard1.transition = Selectable.Transition.None;
        storyCard2.transition = Selectable.Transition.None;
        storyCard3.transition = Selectable.Transition.None;

        // Wire up card clicks
        storyCard1.onClick.AddListener(() => OnStorySelected(0));
        storyCard2.onClick.AddListener(() => OnStorySelected(1));
        storyCard3.onClick.AddListener(() => OnStorySelected(2));

        // Reset card colors
        ResetCards();
    }

    //  Story Selected
    void OnStorySelected(int storyIndex)
    {
        // Store in GameState
        GameState.instance.currentStory = storyIndex;
        GameState.instance.currentCountry = "ghana";

        // Reset decision and mini game result for fresh run
        GameState.instance.decisionMade = 0;
        GameState.instance.miniGameResult = 0;

        // Tell StoryLoader to load the correct JSON file
        if (StoryLoader.instance != null)
            StoryLoader.instance.LoadCurrentStory();

        // Highlight selected card
        ResetCards();
        Button selected = storyIndex == 0 ? storyCard1
                        : storyIndex == 1 ? storyCard2
                        : storyCard3;
        selected.image.color = selectedCardColor;

        // Load CountryContext
        StartCoroutine(LoadScene("CountryContext"));
    }

    // Reset Cards 
    void ResetCards()
    {
        storyCard1.image.color = unselectedCardColor;
        storyCard2.image.color = unselectedCardColor;
        storyCard3.image.color = unselectedCardColor;
    }
}