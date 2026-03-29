using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Attach this to an empty GameObject in CountryContext scene.
// Wire all fields in the Inspector.
public class CountryContextManager : SceneBase
{
    // ── Slideshow ─────────────────────────────────────────
    [Header("Slideshow")]
    public Sprite[] slideImages;        // drag slide sprites in order
    public Image    slideImage;          // the full-screen Image component
    public float    slideFadeDuration = 0.4f;

    // ── Typewriter ────────────────────────────────────────
    [Header("Typewriter")]
    public Typewriter typewriter;        // drag your Typewriter GameObject here
    public string[] slideTexts;          // one string per slide — fox narration

    // ── Navigation ────────────────────────────────────────
    [Header("Navigation")]
    public Button nextButton;            // 'Next' button
    public TMP_Text nextButtonText;      // text on the Next button

    // ── Story Picker ──────────────────────────────────────
    [Header("Story Picker")]
    public GameObject storyPickerPanel;  // the panel with two buttons
    public Button educationButton;       // 'Education' button
    public Button galenseyButton;        // 'Galamsey' button

    // ── Delegate ──────────────────────────────────────────
    [Header("Delegate")]
    public GameObject delegateImage;     // the fox portrait

    // ── State ─────────────────────────────────────────────
    private int currentSlide = 0;
    private bool transitioning = false;

    // ─────────────────────────────────────────────────────
    void Start()
    {
        // Fix button transitions
        nextButton.transition      = Selectable.Transition.None;
        educationButton.transition = Selectable.Transition.None;
        galenseyButton.transition  = Selectable.Transition.None;

        // Hide story picker at start
        storyPickerPanel.SetActive(false);

        // Wire buttons
        nextButton.onClick.AddListener(OnNextClicked);
        educationButton.onClick.AddListener(() => OnStorySelected(0));
        galenseyButton.onClick.AddListener(() => OnStorySelected(1));

        // Show first slide
        StartCoroutine(ShowSlide(0));
    }

    // ─────────────────────────────────────────────────────
    // NEXT BUTTON CLICKED
    // ─────────────────────────────────────────────────────
    void OnNextClicked()
    {
        if (transitioning) return;

        // If typewriter is still typing — skip it first
        if (typewriter != null && typewriter.IsTyping())
        {
            typewriter.Skip();
            return;
        }

        currentSlide++;

        if (currentSlide >= slideImages.Length)
        {
            // All slides done — show story picker
            ShowStoryPicker();
            return;
        }

        StartCoroutine(ShowSlide(currentSlide));
    }

    // ─────────────────────────────────────────────────────
    // SHOW SLIDE — fades in new image and starts typewriter
    // ─────────────────────────────────────────────────────
    IEnumerator ShowSlide(int index)
    {
        transitioning = true;

        // Fade out current slide
        if (index > 0)
            yield return StartCoroutine(FadeImage(slideImage, 1f, 0f, slideFadeDuration));

        // Set new sprite
        if (index < slideImages.Length)
            slideImage.sprite = slideImages[index];

        // Fade in new slide
        yield return StartCoroutine(FadeImage(slideImage, 0f, 1f, slideFadeDuration));

        // Start typewriter for this slide's text
        if (typewriter != null && index < slideTexts.Length)
            typewriter.ShowText(slideTexts[index]);

        // Update next button text
        if (nextButtonText != null)
            nextButtonText.text = (index == slideImages.Length - 1) ? "What's the problem?" : "Next";

        transitioning = false;
    }

    // ─────────────────────────────────────────────────────
    // FADE IMAGE ALPHA
    // ─────────────────────────────────────────────────────
    IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = img.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            c.a = Mathf.Lerp(from, to, t);
            img.color = c;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }

    // ─────────────────────────────────────────────────────
    // SHOW STORY PICKER
    // ─────────────────────────────────────────────────────
    void ShowStoryPicker()
    {
        // Hide slideshow elements
        nextButton.gameObject.SetActive(false);
        if (typewriter != null) typewriter.gameObject.SetActive(false);

        // Show picker
        storyPickerPanel.SetActive(true);
    }

    // ─────────────────────────────────────────────────────
    // STORY SELECTED
    // ─────────────────────────────────────────────────────
    void OnStorySelected(int storyIndex)
    {
        // Store in GameState
        GameState.instance.currentStory   = storyIndex;
        GameState.instance.currentCountry = "ghana";
        GameState.instance.decisionMade   = 0;
        GameState.instance.miniGameResult  = 0;

        // Load Decision scene
        StartCoroutine(LoadScene("Decision"));
    }
}