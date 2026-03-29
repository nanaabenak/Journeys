using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DameManager : SceneBase
{
    // ── Board Data ──────────────────────────────────────────
    // 0=empty  1=player piece  2=AI piece  3=player king  4=AI king
    private int[,] board = new int[8, 8];
    private int playerPiecesCaptured = 0;
    private int aiPiecesCaptured = 0;
    private bool isPlayerTurn = true;
    private bool gameOver = false;

    // Selected piece tracking
    private int selectedRow = -1;
    private int selectedCol = -1;
    private List<int[]> validMoves = new List<int[]>(); // each entry: [toRow, toCol, midRow, midCol, isCapture]

    // ── UI References ───────────────────────────────────────
    // Assign the 64 board buttons in the Inspector as an 8x8 grid
    // Button[0] = Square_0_0 (top left), Button[63] = Square_7_7 (bottom right)
    public Button[] squareButtons = new Button[64];
    public TMP_Text playerPiecesText;
    public TMP_Text aiPiecesText;
    public TMP_Text turnText;
    public TMP_Text statusText;

    // ── Colors ──────────────────────────────────────────────
    private Color emptyDark   = new Color(0.30f, 0.18f, 0.09f); // dark brown
    private Color emptyLight  = new Color(0.85f, 0.72f, 0.55f); // light brown
    private Color playerColor = new Color(0.80f, 0.10f, 0.10f); // red
    private Color aiColor     = new Color(0.25f, 0.25f, 0.25f); // dark grey
    private Color highlightColor = new Color(0.20f, 0.70f, 0.20f); // green
    private Color selectedColor  = new Color(0.90f, 0.80f, 0.10f); // yellow
    private Color kingPlayerColor = new Color(1.00f, 0.30f, 0.10f); // bright red for kings
    private Color kingAiColor     = new Color(0.45f, 0.45f, 0.45f); // lighter grey for kings

    // ── Start ───────────────────────────────────────────────
    void Start()
    {
        InitialiseBoard();
        WireUpButtons();
        UpdateUI();
    }

    // ── Board Initialisation ─────────────────────────────────
    void InitialiseBoard()
    {
        // Clear board
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                board[r, c] = 0;

        // AI pieces on rows 0, 1, 2 — only dark squares where (r+c)%2==1
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 8; c++)
                if ((r + c) % 2 == 1)
                board[r, c] = 2;

        // Player pieces on rows 5, 6, 7
        for (int r = 5; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if ((r + c) % 2 == 1)
                    board[r, c] = 1;
    }

    // ── Wire Up Buttons ──────────────────────────────────────
    void WireUpButtons()
    {
        for (int i = 0; i < 64; i++)
        {
            int index = i;
            int row = index / 8;
            int col = index % 8;
            squareButtons[i].onClick.AddListener(() => OnSquareClicked(row, col));
        }
    }

    // ── Helper: Is Valid Square ──────────────────────────────
    bool IsValid(int r, int c)
    {
        return r >= 0 && r < 8 && c >= 0 && c < 8;
    }

    // ── Helper: Is Player Piece ──────────────────────────────
    bool IsPlayerPiece(int r, int c)
    {
        return board[r, c] == 1 || board[r, c] == 3;
    }

    // ── Helper: Is AI Piece ──────────────────────────────────
    bool IsAIPiece(int r, int c)
    {
        return board[r, c] == 2 || board[r, c] == 4;
    }

    // ── Helper: Is King ──────────────────────────────────────
    bool IsKing(int r, int c)
    {
        return board[r, c] == 3 || board[r, c] == 4;
    }

    // ── Helper: Count Pieces ─────────────────────────────────
    int CountPieces(bool player)
    {
        int count = 0;
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (player && IsPlayerPiece(r, c)) count++;
                else if (!player && IsAIPiece(r, c)) count++;
        return count;
    }

    // ── Helper: Has Any Capture ──────────────────────────────
    // Returns true if the given side has at least one capture available anywhere
    bool HasAnyCapture(bool player)
    {
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (player && IsPlayerPiece(r, c))
                { if (GetCaptures(r, c).Count > 0) return true; }
                else if (!player && IsAIPiece(r, c))
                { if (GetCaptures(r, c).Count > 0) return true; }
        return false;
    }

    // ── Helper: Get Moves ────────────────────────────────────
    // Returns list of valid simple moves for a piece (no captures)
    List<int[]> GetMoves(int r, int c)
    {
        List<int[]> moves = new List<int[]>();
        bool king = IsKing(r, c);
        bool player = IsPlayerPiece(r, c);

        // Player moves UP (decreasing row). AI moves DOWN (increasing row). Kings move both.
        int[] rowDirs = player
            ? (king ? new int[] { -1, 1 } : new int[] { -1 })
            : (king ? new int[] { -1, 1 } : new int[] { 1 });

        int[] colDirs = new int[] { -1, 1 };

        foreach (int dr in rowDirs)
            foreach (int dc in colDirs)
            {
                int nr = r + dr;
                int nc = c + dc;
                if (IsValid(nr, nc) && board[nr, nc] == 0)
                    // [toRow, toCol, midRow=-1, midCol=-1, isCapture=0]
                    moves.Add(new int[] { nr, nc, -1, -1, 0 });
            }
        return moves;
    }

    // ── Helper: Get Captures ─────────────────────────────────
    // Returns list of valid captures for a piece
    List<int[]> GetCaptures(int r, int c)
    {
        List<int[]> captures = new List<int[]>();
        bool king = IsKing(r, c);
        bool player = IsPlayerPiece(r, c);

        int[] rowDirs = king ? new int[] { -1, 1 } : (player ? new int[] { -1 } : new int[] { 1 });
        int[] colDirs = new int[] { -1, 1 };

        foreach (int dr in rowDirs)
            foreach (int dc in colDirs)
            {
                int midR = r + dr;
                int midC = c + dc;
                int landR = r + 2 * dr;
                int landC = c + 2 * dc;

                if (!IsValid(landR, landC)) continue;
                if (board[landR, landC] != 0) continue;

                // Check opponent piece in middle
                bool opponentInMiddle = player ? IsAIPiece(midR, midC) : IsPlayerPiece(midR, midC);
                if (opponentInMiddle)
                    // [toRow, toCol, midRow, midCol, isCapture=1]
                    captures.Add(new int[] { landR, landC, midR, midC, 1 });
            }
        return captures;
    }

    // ── On Square Clicked ────────────────────────────────────
    public void OnSquareClicked(int row, int col)
    {
        if (gameOver || !isPlayerTurn) return;

        // If clicking a player piece — select it
        if (IsPlayerPiece(row, col))
        {
            SelectPiece(row, col);
            return;
        }

        // If a piece is selected and clicking a valid move destination — execute move
        if (selectedRow >= 0)
        {
            foreach (int[] move in validMoves)
            {
                if (move[0] == row && move[1] == col)
                {
                    StartCoroutine(ExecutePlayerMove(move));
                    return;
                }
            }
        }

        // Clicking empty square with no selection — deselect
        ClearSelection();
        UpdateUI();
    }

    // ── Select Piece ─────────────────────────────────────────
    void SelectPiece(int row, int col)
    {
        ClearSelection();
        selectedRow = row;
        selectedCol = col;

        // Forced capture — if any capture exists anywhere, only show captures for this piece
        if (HasAnyCapture(true))
            validMoves = GetCaptures(row, col);
        else
            validMoves = GetMoves(row, col);

        UpdateUI();
    }

    // ── Clear Selection ──────────────────────────────────────
    void ClearSelection()
    {
        selectedRow = -1;
        selectedCol = -1;
        validMoves.Clear();
    }

    // ── Execute Player Move ──────────────────────────────────
    IEnumerator ExecutePlayerMove(int[] move)
    {
        // Move piece
        board[move[0], move[1]] = board[selectedRow, selectedCol];
        board[selectedRow, selectedCol] = 0;

        bool continueCapturing = false;

        // If capture — remove jumped piece
        if (move[4] == 1)
        {
            board[move[2], move[3]] = 0;
            aiPiecesCaptured++;

            // Check for multi-capture from new position
            List<int[]> furtherCaptures = GetCaptures(move[0], move[1]);
            if (furtherCaptures.Count > 0)
            {
                selectedRow = move[0];
                selectedCol = move[1];
                validMoves = furtherCaptures;
                PromoteToKing(move[0], move[1]);
                UpdateUI();
                continueCapturing = true;
            }
        }

        if (!continueCapturing)
        {
            PromoteToKing(move[0], move[1]);
            ClearSelection();
            UpdateUI();
            CheckGameOver();
            if (!gameOver)
            {
                isPlayerTurn = false;
                UpdateUI();
                yield return StartCoroutine(AITurn());
            }
        }
        else
        {
            yield return null;
        }
    }

    // ── Promote To King ──────────────────────────────────────
    void PromoteToKing(int r, int c)
    {
        if (board[r, c] == 1 && r == 0) board[r, c] = 3; // player reaches AI back row
        if (board[r, c] == 2 && r == 7) board[r, c] = 4; // AI reaches player back row
    }

    // ── AI Turn ──────────────────────────────────────────────
    IEnumerator AITurn()
    {
        if (gameOver) yield break;
        yield return new WaitForSeconds(1.5f);

        bool aiMoved = false;

        // Build list of all AI captures
        List<int[]> allCaptures = new List<int[]>();
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (IsAIPiece(r, c))
                    foreach (int[] cap in GetCaptures(r, c))
                        allCaptures.Add(new int[] { r, c, cap[0], cap[1], cap[2], cap[3] });
                        // [fromRow, fromCol, toRow, toCol, midRow, midCol]

        if (allCaptures.Count > 0)
        {
            // Execute a random capture
            int[] chosen = allCaptures[Random.Range(0, allCaptures.Count)];
            yield return StartCoroutine(ExecuteAICapture(chosen[0], chosen[1],chosen[2], chosen[3], chosen[4], chosen[5]));
            aiMoved = true;
        }
        else
        {
            // No captures — find all moves
            List<int[]> allMoves = new List<int[]>();
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    if (IsAIPiece(r, c))
                        foreach (int[] mv in GetMoves(r, c))
                            allMoves.Add(new int[] { r, c, mv[0], mv[1] });
                            // [fromRow, fromCol, toRow, toCol]

            if (allMoves.Count > 0)
            {
                int[] chosen = allMoves[Random.Range(0, allMoves.Count)];
                board[chosen[2], chosen[3]] = board[chosen[0], chosen[1]];
                board[chosen[0], chosen[1]] = 0;
                PromoteToKing(chosen[2], chosen[3]);
                aiMoved = true;
            }
        }

        if (!aiMoved)
        {
            // AI has no moves — player wins
            CheckGameOver();
            yield break;
        }

        UpdateUI();
        CheckGameOver();
        if (!gameOver)
        {
            isPlayerTurn = true;
            UpdateUI();
        }
    }

    // ── Execute AI Capture ───────────────────────────────────
    IEnumerator ExecuteAICapture(int fromR, int fromC, int toR, int toC, int midR, int midC)
    {
        board[toR, toC] = board[fromR, fromC];
        board[fromR, fromC] = 0;
        board[midR, midC] = 0;
        playerPiecesCaptured++;
        PromoteToKing(toR, toC);
        UpdateUI();

        // Multi-capture for AI
        List<int[]> further = GetCaptures(toR, toC);
        if (further.Count > 0)
        {
            yield return new WaitForSeconds(0.8f);
            int[] next = further[0];
            yield return StartCoroutine(ExecuteAICapture(toR, toC, next[0], next[1], next[2], next[3]));
        }
        else
        {
            yield return null;
        }
    }

    // ── Check Game Over ──────────────────────────────────────
    void CheckGameOver()
    {
        bool playerHasPieces = CountPieces(true) > 0;
        bool aiHasPieces = CountPieces(false) > 0;
        bool playerHasMoves = false;
        bool aiHasMoves = false;

        if (playerHasPieces)
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    if (IsPlayerPiece(r, c))
                        if (GetMoves(r, c).Count > 0 || GetCaptures(r, c).Count > 0)
                            playerHasMoves = true;

        if (aiHasPieces)
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    if (IsAIPiece(r, c))
                        if (GetMoves(r, c).Count > 0 || GetCaptures(r, c).Count > 0)
                            aiHasMoves = true;

        if (!playerHasPieces || !playerHasMoves)
        {
            EndGame(false); // player loses
            return;
        }
        if (!aiHasPieces || !aiHasMoves)
        {
            EndGame(true); // player wins
        }
    }

    // ── End Game ─────────────────────────────────────────────
    void EndGame(bool playerWins)
    {
        gameOver = true;
        GameState.instance.miniGameResult = aiPiecesCaptured; // score = pieces captured
        turnText.text = playerWins ? "You Win!" : "Forces of Evil Win!";
        statusText.text = playerWins
            ? "The community holds its ground."
            : "The river keeps running orange.";
        UpdateUI();
        StartCoroutine(LoadOutcomeAfterDelay());
    }

    // ── Update UI ────────────────────────────────────────────
    void UpdateUI()
    {
        // Update score texts
        playerPiecesText.text = "Your pieces: " + CountPieces(true);
        aiPiecesText.text = "Opponent pieces: " + CountPieces(false);
        turnText.text = isPlayerTurn ? "Your Move" : "Thinking...";

        // Update board visuals
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                int index = r * 8 + c;
                Button btn = squareButtons[index];
                TMP_Text label = btn.GetComponentInChildren<TMP_Text>();

                // Determine square color
                if (r == selectedRow && c == selectedCol)
                    btn.image.color = selectedColor;
                else if (IsValidMoveDestination(r, c))
                    btn.image.color = highlightColor;
                else if (board[r, c] == 1)
                    btn.image.color = playerColor;
                else if (board[r, c] == 2)
                    btn.image.color = aiColor;
                else if (board[r, c] == 3)
                    btn.image.color = kingPlayerColor;
                else if (board[r, c] == 4)
                    btn.image.color = kingAiColor;
                else
                    btn.image.color = (r + c) % 2 == 1 ? emptyDark : emptyLight;

                // Label — show K for kings, dot for pieces, empty otherwise
                if (label != null)
                {
                    if (board[r, c] == 3 || board[r, c] == 4) label.text = "K";
                    else if (board[r, c] != 0) label.text = "●";
                    else label.text = "";
                }

                // Only dark squares are interactable
                btn.interactable = isPlayerTurn && !gameOver && (r + c) % 2 == 1;
            }
        }
        }

    // ── Is Valid Move Destination ────────────────────────────
    bool IsValidMoveDestination(int r, int c)
    {
        foreach (int[] move in validMoves)
            if (move[0] == r && move[1] == c) return true;
        return false;
    }

    // ── Load Outcome After Delay ─────────────────────────────
    IEnumerator LoadOutcomeAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        GoToScene("Outcome");
    }
}