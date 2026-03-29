using System.Collections;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{
    [Header("Settings")]
    public float charactersPerSecond = 20f;
    public AudioClip typingSound;
    public string startingText = "Hello, world!"; // Set in Inspector

    private TMP_Text _textComponent;
    private Coroutine _typingCoroutine;
    private bool _isTyping = false;

    void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    void Start()
    {
        ShowText(startingText);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isTyping)
            Skip();
    }

    public void ShowText(string fullText)
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    public void Skip()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _textComponent.maxVisibleCharacters = int.MaxValue;
        _isTyping = false;
    }

    private IEnumerator TypeText(string fullText)
    {
        _textComponent.text = fullText;
        _textComponent.maxVisibleCharacters = 0;
        _isTyping = true;

        float interval = 1f / charactersPerSecond;

        foreach (char _ in fullText)
        {
            _textComponent.maxVisibleCharacters++;

            if (typingSound != null)
                AudioSource.PlayClipAtPoint(typingSound, transform.position);

            yield return new WaitForSeconds(interval);
        }

        _isTyping = false;
    }
    public bool IsTyping()
{
    return _isTyping;
}
}