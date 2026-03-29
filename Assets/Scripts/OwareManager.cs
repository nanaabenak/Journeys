using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class OwareManager : SceneBase
{
    public int[] playerPits = new int[6];
    public int[] aiPits = new int[6];
    public Button[] playerPitButtons = new Button [6];
    public Button[] aiPitButtons = new Button [6];
    public int playerCapturedSeeds = 0;
    public int aiCapturedSeeds = 0;
    public TMP_Text playerScore;
    public TMP_Text aiScore;

    public TMP_Text turnIndicator;
    private bool isPlayerTurn = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool gameOver = false;
    // private bool captureHappened = false;

    
    void Start()
    {
        // ai = new OwareAI();
        for (int i = 0; i < 6; i++)
        {
            playerPits[i] = 4;
            aiPits[i] = 4;
            int index = i;
            playerPitButtons[i].onClick.AddListener(() => OnPitClicked(index));
        }
        UpdateUI();
    }

    // Update is called once per frame
    void UpdateUI()
    {
        playerScore.text = "Player: " + playerCapturedSeeds.ToString(); // Update player score
        aiScore.text = "Forces of Evil:" + aiCapturedSeeds.ToString();  // Update AI score
        turnIndicator.text = isPlayerTurn ? "Your Turn" : "AI's Turn";  // Update turn indicator
        
        // Update pit buttons
        for (int i = 0; i < 6; i++)
        {
            playerPitButtons[i].GetComponentInChildren<TMP_Text>().text = playerPits[i].ToString();
            playerPitButtons[i].interactable = isPlayerTurn && playerPits[i] > 0 && !gameOver; // Only interactable if it's player's turn, pit has seeds, and game is not over
            aiPitButtons[i].GetComponentInChildren<TMP_Text>().text = aiPits[i].ToString();
            aiPitButtons[i].interactable = false; // AI pits are not interactable
        }
    }

    int GetPitCount(int pos)
    {
        return pos < 6 ? playerPits[pos] : aiPits[pos - 6];
    }

    void SetPitCount(int pos, int count)
    {
        if (pos < 6)
        {
            playerPits[pos] = count;
        }
        else
        {
            aiPits[pos - 6] = count; // maybe return the pits will see later
        }
    }

    int IncrementPit(int pos)
    {
        if (pos < 6)
        {
            playerPits[pos]++;
            return playerPits[pos];
        }
        else
        {
            aiPits[pos - 6]++;
            return aiPits[pos - 6];
        }
    }

    int EmptyPit(int pos)
    {
        int count = GetPitCount(pos);
        SetPitCount(pos, 0);
        return count;
    }

    bool IsPlayerSide(int pos)
    {
        return pos < 6;
    }

// mid-sow capture
    bool OwnerCaptures(int pos)
    {
        if (GetPitCount(pos) != 4) return false;

        if (IsPlayerSide(pos))
        {
            playerCapturedSeeds += 4;
        }
        else
        {
            aiCapturedSeeds += 4;
        }
        SetPitCount(pos, 0);
        return true;
    }

// last seed capture

    bool ActivePlayerCapture(int pos, bool isPlayerTurn)
    {
        if (GetPitCount(pos) != 4) return false;
        if (isPlayerTurn)
        {
            playerCapturedSeeds +=4;
        }
        else
        {
            aiCapturedSeeds += 4;
        }
        SetPitCount(pos, 0);
        return true;
    }

    bool IsSideEmpty(bool playerSide)
    {
        for (int i = 0; i < 6 ; i++)
        {
            if(playerSide)
            {
                if(playerPits[i] > 0) return false;
            }
            else
            {
                if(aiPits[i] > 0) return false;
            }
        }
        return true;
    }

    int TotalSeedsOnBoard()
    {
        int total = 0;

        for (int i = 0; i < 6; i++)
        {
            total += playerPits[i];
            total += aiPits[i];
        }

        return total;
    }

    int GetRandomValidPit(bool playerSide)
    {
        int index;

        do
        {
            index = Random.Range(0,6);
        }
        while ((playerSide ? playerPits[index] : aiPits[index]) == 0);
        // while(pit[index] == 0);

        return index;
    }

    void SowSeeds (int startPos, int seeds, bool isPlayerTurn)
    {
        int safety = 0;
        int currentPos = startPos;
        while (true)
        {
            safety++;
            if (safety > 100) 
            {
                Debug.LogWarning("Safety Trigered infinite loop occurs somewhwer");
                break;
            };
            bool captureHappened = false;
            for (int i =1; i <= seeds; i++)
            {
                int pos = (currentPos + i) % 12;
                int previousCount = GetPitCount(pos);
                int newCount = IncrementPit(pos);

                if (previousCount==3 && newCount == 4)
            {
                bool isLast = (i  == seeds);
                bool isOpponent = IsPlayerSide(pos) != isPlayerTurn;

                if (isLast && isOpponent)
                { 
                    ActivePlayerCapture(pos, isPlayerTurn);
                    captureHappened = true;
                }
                else if (isLast && !isOpponent)
                {
                    OwnerCaptures(pos);
                    captureHappened = true;
                }
                else
                {
                    OwnerCaptures(pos);
                }

                
            }
        }

        int lastPos = (currentPos + seeds) % 12;

        if (GetPitCount(lastPos) == 1 || captureHappened)
        {
            break;
        }
        seeds = EmptyPit(lastPos);
        currentPos = lastPos;
        }
    }

    public void OnPitClicked(int pitIndex)
    {
        if (gameOver || !isPlayerTurn) return;
        if (playerPits[pitIndex] == 0) return;

        Debug.Log("Player chooses pit: " + pitIndex);


        int startPos = pitIndex;
        int seeds = EmptyPit(startPos);

        SowSeeds(startPos, seeds, true);

        UpdateUI();
        CheckGameOver();

        if (gameOver) return;

        if (IsSideEmpty(true))
        {
            StartCoroutine(AITurn());
            return;
        }

        isPlayerTurn = false;
        StartCoroutine(AITurn());
    }

    IEnumerator AITurn()
    {
        if (gameOver) yield break;
        yield return new WaitForSeconds(1f);

        if (IsSideEmpty(false))
        {
            if (TotalSeedsOnBoard() == 4)
            {
                CheckGameOver();
            }
            else
            {
                isPlayerTurn = true;
                UpdateUI();
            }
            yield break;
        }

        int pitIndex = GetRandomValidPit(false);
        // int pitIndex = ai.GetMove(playerPits,aiPits);
        Debug.Log("AI chooses pit: " + pitIndex);
        pitIndex += 6; // Adjust for AI pits
        Debug.Log("AI chooses pit: " + pitIndex);
        // int startPos = pitIndex + 6; 
        int seeds = EmptyPit(pitIndex);
        SowSeeds (pitIndex,seeds,false);
        UpdateUI();
        CheckGameOver();
        
        if (gameOver) yield break;

        if (IsSideEmpty(false))
        {
            isPlayerTurn = true;
            Debug.Log($"Player: {string.Join(",", playerPits)}");
            Debug.Log($"AI: {string.Join(",", aiPits)}");
            UpdateUI();
            yield break;
        }

        isPlayerTurn = true;
        UpdateUI();
        Debug.Log($"Player: {string.Join(",", playerPits)}");
        Debug.Log($"AI: {string.Join(",", aiPits)}");
        CheckGameOver();


    }

    void CheckGameOver()
    {
        if (TotalSeedsOnBoard() > 4) return;

        gameOver = true;
        playerCapturedSeeds += TotalSeedsOnBoard();

        for (int i = 0; i < 6; i++)
        {
            playerPits[i] = 0;
            aiPits[i] = 0;
        }

        GameState.instance.miniGameResult = playerCapturedSeeds;
        UpdateUI();

        if (playerCapturedSeeds > aiCapturedSeeds)
        {
            turnIndicator.text = "You Win";
            
        }
        else if (aiCapturedSeeds > playerCapturedSeeds)
        {
            turnIndicator.text = "Forces of Evil Win!";
        }
        else
        {
            turnIndicator.text = "It is a Tie!";

        }
            
        StartCoroutine(LoadOutcomeAfterDelay());
    }

    IEnumerator LoadOutcomeAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        GoToScene("Outcome");
    }
}
