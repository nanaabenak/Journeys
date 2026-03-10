using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public Image fadePanel; // reference to the fade panel component

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }
    // Fading in Coroutine
    IEnumerator FadeIn(float duration)
    {
        float time_elapsed = 0; // tracking time elapsed
        while (time_elapsed < duration)
        {
            time_elapsed += Time.deltaTime; // increasing the time elapsed
            // setting alpha panel
            float alpha = time_elapsed/duration; 
            fadePanel.color = new Color(0,0,0,alpha);
            yield return null; // pausing for one frame and then continuing

        }

    }
    // Fading Out Coroutine
    IEnumerator FadeOut(float duration)
    {
        float time_elapsed = 0; // tracking time elapsed
        while (time_elapsed < duration)
        {
            time_elapsed += Time.deltaTime; // increasing the time elapsed
            // setting alpha panel
            float alpha = time_elapsed/duration; 
            fadePanel.color = new Color(0,0,0,(1-alpha));
            yield return null; // pausing for one frame and then continuing

        }

    }

    // Coroutine to make sure things finish before things start
    IEnumerator LoadScene(string sceneName)
    {
        // Fade In
        yield return StartCoroutine(FadeIn(0.5f));

        // Loadscene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade Out
        yield return StartCoroutine(FadeOut(0.5f));
    }

    public void GoToScene(string sceneName)
    {
     StartCoroutine(LoadScene(sceneName)); // starting the coroutine to load the scene
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
