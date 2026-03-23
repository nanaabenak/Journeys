using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState instance; // Singleton instance of the GameState class
    public string currentCountry;
    public int storyProgress;
    public int decisionMade;
    public int currentStory;
    public int miniGameResult;


    void Awake()
    {
        // Debug.Log("GameState created: " + currentCountry);

        if (instance == null) // If there is no instance of game state, set this as the instance
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Don't destroy this game object when loading a scene   
        }
        else
        {
            Destroy(gameObject); // If there is already an instance of game state, destroy this game object
        }
    }

   
}
