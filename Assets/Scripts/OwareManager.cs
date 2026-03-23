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
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            playerPits[i] = 4;
            aiPits[i] = 4;
        }
        for (int i = 0; i < 6; i++)
        {
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
            playerPitButtons[i].interactable = isPlayerTurn && playerPits[i] > 0;       
        }

        for (int i = 0; i < 6; i++)
        {
            aiPitButtons[i].GetComponentInChildren<TMP_Text>().text = aiPits[i].ToString();
            aiPitButtons[i].interactable = false; // AI pits are not interactable
        }
    }

    public void OnPitClicked(int pitIndex)
    {
        int seeds = playerPits[pitIndex]; // pick up seeds
        playerPits[pitIndex] = 0; // empty the pit
        int currentPos = pitIndex;
        

        while (seeds > 0)
        {   
            bool stopSowing = false;
            // sow seeds
            for (int i = 1; i <= seeds; i++)
            {
                int pos = (currentPos + i) % 12;
                if (pos < 6)
                {
                    playerPits[pos]++;
                    if (playerPits[pos] == 4 )
                    {
                    playerCapturedSeeds += playerPits[pos];
                    playerPits[pos] = 0; // capture seeds and empty the pit


                    }
                }     
                else
                {
                    aiPits[pos - 6]++;
                    if (aiPits[pos - 6] == 4)
                    {
                        if (i == seeds) // last seed — player captures
                        {
                            playerCapturedSeeds += 4;
                            stopSowing = true;
                            break; // stop sowing
                        }
                        else // mid sow — AI captures
                        {
                            aiCapturedSeeds += 4;
                            aiPits[pos - 6] = 0;
                        }
                        
                    }
                }
            }

            if (stopSowing)
            {
                break; // stop sowing if player captured on last seed
            }

            // check last position
            int lastPos = (currentPos + seeds) % 12;
            
            bool lastPitEmpty = lastPos < 6 ? playerPits[lastPos] == 0 : aiPits[lastPos - 6] == 0;
            if (lastPitEmpty)
            {
                break; // stop sowing
            }
            else
            {
                // pick up seeds from last pit and keep going
                seeds = lastPos < 6 ? playerPits[lastPos] : aiPits[lastPos - 6];
                currentPos = lastPos;
                // empty that pit
                if (lastPos < 6)
                {
                    playerPits[lastPos] = 0; // empty the pit
                }
                else                {
                    aiPits[lastPos - 6] = 0; // empty the pit
                }
            }

           
        }
        

       

        // update UI and switch turn
        UpdateUI();

        CheckGameOver();


        // if row is empty after player move, check if game over or skip AI turn
        if (System.Array.TrueForAll(playerPits, p => p == 0))
        {
            int totalLeft = 48 - (playerCapturedSeeds + aiCapturedSeeds);
            if (totalLeft == 4)
            {
                CheckGameOver();
                return;
            }
            else
            {
                // skip player turn, AI keeps going
                StartCoroutine(AITurn());
                return;
            }
        }

        isPlayerTurn = false;
        StartCoroutine(AITurn());

    }

    IEnumerator AITurn()
    {
        yield return new WaitForSeconds(2f); // Wait for player to see board update

        // If all AI pits are empty, check if game should end or skip turn
        if (System.Array.TrueForAll(aiPits, p => p == 0))
        {
            int totalLeft = 48 - (playerCapturedSeeds + aiCapturedSeeds);
            if (totalLeft == 4)
            {
                CheckGameOver();
                yield break;
            }
            else
            {
                isPlayerTurn = true; // skip AI turn, player keeps going
                UpdateUI();
                yield break;
            }
        }

        // Simple AI: pick random non-empty pit
        int aiPitIndex;
        do
        {
            aiPitIndex = Random.Range(0, 6);
        } while (aiPits[aiPitIndex] == 0);

        int seeds = aiPits[aiPitIndex]; // pick up seeds from chosen pit
        aiPits[aiPitIndex] = 0; // empty the chosen pit
        int currentPos = aiPitIndex;

        // Relay sowing loop — keeps going until last seed lands in empty pit
        while (seeds > 0)
        {
            bool stopSowing = false;

            // Sow seeds one by one around the board
            for (int i = 1; i <= seeds; i++)
            {
                int pos = (currentPos + i) % 12;

                if (pos < 6)
                {
                    // Landing on AI's side
                    aiPits[pos]++;
                    if (aiPits[pos] == 4)
                    {
                        aiCapturedSeeds += aiPits[pos]; // AI captures their own pit reaching 4
                        aiPits[pos] = 0;
                    }
                }
                else
                {
                    // Landing on player's side
                    playerPits[pos - 6]++;
                    if (playerPits[pos - 6] == 4)
                    {
                        if (i == seeds) // last seed — AI captures opponent's pit
                        {
                            aiCapturedSeeds += 4;
                            stopSowing = true;
                            break;
                        }
                        else // mid sow — player captures their own pit
                        {
                            playerCapturedSeeds += 4;
                        }
                        playerPits[pos - 6] = 0;
                    }
                }
            }

            // Stop if AI captured on last seed
            if (stopSowing) break;

            // Check where last seed landed
            int lastPos = (currentPos + seeds) % 12;
            bool lastPitEmpty = lastPos < 6 ? aiPits[lastPos] == 0 : playerPits[lastPos - 6] == 0;

            if (lastPitEmpty)
            {
                break; // last seed landed in empty pit — end turn
            }
            else
            {
                // Relay — pick up seeds from last pit and keep sowing
                seeds = lastPos < 6 ? aiPits[lastPos] : playerPits[lastPos - 6];
                currentPos = lastPos;
                if (lastPos < 6)
                    aiPits[lastPos] = 0; // empty the relay pit
                else
                    playerPits[lastPos - 6] = 0; // empty the relay pit
            }
        }

        // Update board and hand control back to player
        UpdateUI();
        CheckGameOver();
        isPlayerTurn = true;
        UpdateUI(); // refresh interactable state for player buttons
    }

    public void CheckGameOver()
    {
        int totalSeedsLeft = 48 - (playerCapturedSeeds + aiCapturedSeeds);
        if (totalSeedsLeft == 4)
        {
            playerCapturedSeeds += 4; // Player captures remaining seeds
            GameState.instance.miniGameResult = playerCapturedSeeds;
            UpdateUI();
            // Game over logic here (e.g., display winner)
            if (playerCapturedSeeds > aiCapturedSeeds)
            {
                turnIndicator.text = "You Win!";
            }
            else if (aiCapturedSeeds > playerCapturedSeeds)
            {
                turnIndicator.text = "Forces of Evil Win!";
            }
            else
            {
                turnIndicator.text = "It's a Tie!";
            }
            StartCoroutine(LoadOutcomeAfterDelay());

            

            
        }
    }

    IEnumerator LoadOutcomeAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        GoToScene("Outcome");
    }
}
