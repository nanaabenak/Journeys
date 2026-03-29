using UnityEngine;

public class MiniGameLoader : MonoBehaviour
{
    public GameObject owareBoard; // drag OwareBoard GameObject here
    public GameObject dameBoard;  // drag DameBoard GameObject here

    void Start()
    {
        owareBoard.SetActive(false);
        dameBoard.SetActive(false);

        int story = GameState.instance.currentStory;

        if (story == 0)
            owareBoard.SetActive(true);  // Education — Oware
        else if (story == 1)
            dameBoard.SetActive(true);   // Galamsey — Dame
    }
}