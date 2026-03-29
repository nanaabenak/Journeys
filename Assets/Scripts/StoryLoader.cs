using UnityEngine;

public class StoryLoader : MonoBehaviour
{
    public static StoryLoader instance;
    public StoryData currentStory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void LoadStory(string country, int storyIndex) {
        // Build file name from country and story index
        string fileName = country.ToLower() + "_story" + (storyIndex + 1);
        TextAsset jsonFile = Resources.Load<TextAsset>("Stories/" + fileName);

        if (jsonFile == null) {
            Debug.LogError("Story file not found: " + fileName);
            return;
        }

        currentStory = JsonUtility.FromJson<StoryData>(jsonFile.text);
        Debug.Log("Loaded story: " + currentStory.story_id);
    }

    // Call this from WorldMap when a story is selected
    public void LoadCurrentStory() {
        LoadStory(GameState.instance.currentCountry, GameState.instance.currentStory);
    }
}
