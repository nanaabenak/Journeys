using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class IntroManager : SceneBase
{
    public Sprite[] images;
    public Image Background; // reference to the background image component

    // Awake is called when the script instance is being loaded
    void Awake()
    {   
        PlayerPrefs.DeleteKey("hasSeenIntro");
        int seen = PlayerPrefs.GetInt("hasSeenIntro", 0);

        if (seen == 0) // if the player has not seen the intro, start the intro sequence
        {   
            Debug.Log("Awake started, image count: " + images.Length);
            StartCoroutine(PlayIntro());
        }else
        {
            Debug.Log("Already seen intro, loading start screen");
            StartCoroutine(LoadScene("StartScreen")); // starting the coroutine to load the scene

        }

    }

    
    // Coroutine to play the intro sequence
    IEnumerator PlayIntro()
    {   
        Debug.Log("PlayIntro started, image count: " + images.Length);
        yield return new WaitForSeconds(2f);

        for (int i = 0;i < images.Length;i++)
        {
            Debug.Log("Displaying image " + i);
            yield return StartCoroutine(FadeOut(0.5f));
            Background.sprite = images[i];
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FadeIn(0.5f));

        }

        PlayerPrefs.SetInt("hasSeenIntro", 1);
        yield return StartCoroutine(LoadScene("StartScreen"));
            
        
    }



    
}
