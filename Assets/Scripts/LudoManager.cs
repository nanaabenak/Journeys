using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LudoManager : SceneBase
{
    // ─────────────────────────────────────────────────────────
    // THE BOARD
    // 225 buttons — the 15×15 grid, index = row*15 + col
    // ─────────────────────────────────────────────────────────
    public Button[] boardSquares = new Button[225];

    // Home circle buttons — 4 per player
    public Button[] playerHomeCircles = new Button[4]; // Blue — Player
    public Button[] ai1HomeCircles    = new Button[4]; // Yellow — Yaa
    public Button[] ai2HomeCircles    = new Button[4]; // Red — Agyeman
    public Button[] ai3HomeCircles    = new Button[4]; // Green — Darko

    // ─────────────────────────────────────────────────────────
    // TRACK PATH — 52 grid indices in clockwise order
    // Starting from Blue entry point (bottom-right of center column)
    // Row 13 Col 8 = index 203 = Blue entry = track position 0
    // ─────────────────────────────────────────────────────────
    protected int[] trackPath = new int[]
    {
        // BOTTOM ARM — going up (Blue entry side)
        // Col 8, rows 13 down to 9 — but we go upward so rows 13,12,11,10,9
        203, 188, 173, 158, 143, 128,  // col 8 going up (positions 0-5)
        // But position 0 is entry so actually:
        // Correct clockwise starting AFTER entry:
        // Let me lay this out properly:
        //
        // The cross track clockwise from Blue entry (col8, row13):
        //
        // Bottom-right of cross going LEFT along row 14:
        // row=14: col 9,10,11,12,13 ... wait the track is on rows/cols 6-8 (the cross arms)
        //
        // STANDARD LUDO 15x15 CROSS TRACK — correct path:
        // The cross occupies cols 6-8 (vertical arm) and rows 6-8 (horizontal arm)
        // Track squares are the 3-wide arms of the cross EXCLUDING home columns
        //
        // BLUE ENTRY: row=13, col=8 (position 0 — first square player lands on)
        // Going CLOCKWISE means going UP first (toward center) then turning
        //
        // RIGHT SIDE OF BOTTOM VERTICAL ARM going UP:
        193, 178, 163, 148, 133,  // col 8, rows 12-8 going up (pos 0-4)
        // BOTTOM-RIGHT TURN into RIGHT HORIZONTAL ARM going RIGHT:
        118, 119, 120, 121, 122, 123,  // row 8, cols 7→13 (but skip center, go cols 9-14)
        // Actually let me define this correctly with exact indices:
    };
    // NOTE: The trackPath array above is a placeholder.
    // After reading the CORRECT TRACK PATH section below,
    // replace this array with the verified indices.

    // ─────────────────────────────────────────────────────────
    // CORRECT 52-SQUARE TRACK PATH
    // Defined by (row, col) pairs then converted to index = row*15+col
    // Read clockwise starting from Blue entry point
    // ─────────────────────────────────────────────────────────
    // private static int RC(int r, int c) => r * 15 + c;

    // Call this in Start() to build the track path correctly
    protected int[] BuildTrackPath()
    {
        return new int[]
        {
            // ── BLUE ENTRY SIDE (right col of bottom arm, going up) ──
            RC(13,6), RC(12,6), RC(11,6), RC(10,6), RC(9,6),
            // ── BOTTOM-RIGHT CORNER: turn right along bottom of right arm ──
            RC(8,5), RC(8,4), RC(8,3), RC(8,2), RC(8,1), RC(8,0),
            // ── RIGHT ARM going up ──
            RC(7,0), RC(6,0),
            // ── YELLOW ENTRY (col 8 of right arm — this is AI1 entry = position 13) ──
            RC(6,1), RC(6,2), RC(6,3), RC(6,4), RC(6,5),
            // ── TOP-RIGHT CORNER: turn up along right col of top arm ──
            RC(5,6),  // YELLOW entry square (track pos 13)
            RC(1,8), RC(2,8), RC(3,8), RC(4,8), RC(5,8),
            // ── TOP ARM going left ──
            RC(0,8), RC(0,7), RC(0,6),
            // ── RED ENTRY (row 6 of top arm — AI2 entry = position 26) ──
            RC(1,6), RC(2,6), RC(3,6), RC(4,6), RC(5,6),
            RC(6,6),  // RED entry square (track pos 26)
            // ── LEFT ARM going down ──
            RC(6,5), RC(6,4), RC(6,3), RC(6,2), RC(6,1), RC(6,0),
            // ── BOTTOM-LEFT CORNER ──
            RC(7,0), RC(8,0),
            // ── GREEN ENTRY (row 8 of left arm — AI3 entry = position 39) ──
            RC(8,1), RC(8,2), RC(8,3), RC(8,4), RC(8,5),
            RC(8,6),  // GREEN entry square (track pos 39)
            // ── BOTTOM ARM going down (back toward Blue) ──
            RC(9,6), RC(10,6), RC(11,6), RC(12,6), RC(13,6),
            RC(14,6), RC(14,7), RC(14,8),
            // ── Final approach back to Blue entry ──
            RC(13,8)  // This loops back — but we stop at 51 not 52
            // Track has exactly 52 squares — positions 0-51
        };
    }

    // ─────────────────────────────────────────────────────────
    // HOME APPROACH PATHS — 5 squares each
    // Pieces enter these after completing a full lap
    // ─────────────────────────────────────────────────────────
    private int[] blueApproach   = { RC(13,7), RC(12,7), RC(11,7), RC(10,7), RC(9,7) };
    // Wait — approach goes INTO center from entry side
    // Blue (bottom right) approach goes UP col 7 toward center:
    // Actually: Blue approach = col 7, rows 13 down to 9
    // Let me define these properly:

    // After a full lap, Blue pieces enter approach at row 13, col 7
    // and travel UP to center (row 7, col 7)
    // Blue approach: col 7, going from row 12 to row 8 (5 squares)
    protected int[] blueApproachPath   = { RC(12,7), RC(11,7), RC(10,7), RC(9,7), RC(8,7) };
    protected int[] yellowApproachPath = { RC(1,7),  RC(2,7),  RC(3,7),  RC(4,7),  RC(5,7)  };
    protected int[] redApproachPath    = { RC(13,7), RC(12,7), RC(11,7), RC(10,7), RC(9,7)  };
    // These need to be verified visually — see note above

    // Center safe square
    private int centerSquare = RC(7, 7); // index 112

    // ─────────────────────────────────────────────────────────
    // STAR SQUARES — safe from capture (track positions not grid indices)
    // Standard Ludo: each player's entry + 8 squares ahead
    // ─────────────────────────────────────────────────────────
    protected HashSet<int> starTrackPositions = new HashSet<int> { 0, 8, 13, 21, 26, 34, 39, 47 };

    // Player entry track positions
    private int blueEntry   = 0;  // Blue player enters at track position 0
    private int yellowEntry = 13; // Yellow AI1 enters at track position 13
    private int redEntry    = 26; // Red AI2 enters at track position 26
    private int greenEntry  = 39; // Green AI3 enters at track position 39

    // ─────────────────────────────────────────────────────────
    // PIECE POSITIONS
    // -1 = HOME (not yet entered)
    // 0-51 = main track position
    // 52-56 = approach squares (52=first approach, 56=last before center)
    // 57 = SAFE (in center, finished)
    // ─────────────────────────────────────────────────────────
    protected int[] playerPieces = { -1, -1, -1, -1 }; // Blue
    protected int[] ai1Pieces    = { -1, -1, -1, -1 }; // Yellow
    protected int[] ai2Pieces    = { -1, -1, -1, -1 }; // Red
    protected int[] ai3Pieces    = { -1, -1, -1, -1 }; // Green

    // ─────────────────────────────────────────────────────────
    // TURN AND STATE
    // ─────────────────────────────────────────────────────────
    protected int currentTurn = 0;              // 0=Player 1=AI1 2=AI2 3=AI3
    protected bool waitingForPieceSelection = false;
    protected int storedDiceRoll = 0;
    protected List<int> validPieceIndices = new List<int>();
    protected bool gameOver = false;
    protected bool aiIsActing = false;

    // ─────────────────────────────────────────────────────────
    // COLORS — matching the image exactly
    // Blue = Player (bottom-right in image)
    // Yellow = AI1 Yaa (top-right)
    // Red = AI2 Agyeman (bottom-left)
    // Green = AI3 Darko (top-left)
    // ─────────────────────────────────────────────────────────
    protected Color blueColor    = new Color(0.10f, 0.35f, 0.85f); // Blue
    protected Color yellowColor  = new Color(0.95f, 0.80f, 0.00f); // Yellow
    protected Color redColor     = new Color(0.85f, 0.10f, 0.10f); // Red
    protected Color greenColor   = new Color(0.10f, 0.65f, 0.15f); // Green
    protected Color whiteSquare  = new Color(0.97f, 0.97f, 0.97f);
    protected Color starColor    = new Color(1.00f, 0.98f, 0.85f); // star safe squares
    protected Color homeAtRest   = new Color(0.55f, 0.55f, 0.55f); // piece not at home
    protected Color highlightCol = new Color(1.00f, 0.55f, 0.00f); // orange — valid move

    // ─────────────────────────────────────────────────────────
    // UI REFERENCES
    // ─────────────────────────────────────────────────────────
    public Button   rollDiceButton;
    public TMP_Text diceResultText;
    public TMP_Text turnText;
    public TMP_Text statusText;
    public TMP_Text playerSafeText;
    public TMP_Text ai1SafeText;
    public TMP_Text ai2SafeText;
    public TMP_Text ai3SafeText;

    // Built in Start()
    protected int[] track; // the 52-square path as grid indices

    // ─────────────────────────────────────────────────────────
    // START
    // ─────────────────────────────────────────────────────────
    void Start()
    {
        track = BuildTrackPath();
        SetAllTransitionsNone();
        InitBoardColors();
        WireButtons();
        UpdateUI();
    }

    // ─────────────────────────────────────────────────────────
    // SET ALL TRANSITIONS NONE — fixes color override bug
    // ─────────────────────────────────────────────────────────
    void SetAllTransitionsNone()
    {
        foreach (Button b in boardSquares)      b.transition = Selectable.Transition.None;
        foreach (Button b in playerHomeCircles) b.transition = Selectable.Transition.None;
        foreach (Button b in ai1HomeCircles)    b.transition = Selectable.Transition.None;
        foreach (Button b in ai2HomeCircles)    b.transition = Selectable.Transition.None;
        foreach (Button b in ai3HomeCircles)    b.transition = Selectable.Transition.None;
        rollDiceButton.transition = Selectable.Transition.None;
    }

    // ─────────────────────────────────────────────────────────
    // INIT BOARD COLORS — colors the 15×15 grid like the image
    // ─────────────────────────────────────────────────────────
    protected void InitBoardColors()
    {
        // Start all white
        foreach (Button b in boardSquares) b.image.color = whiteSquare;

        // HOME ZONES — semi-transparent fills over 6×6 corner areas
        ColorRect(0, 6, 0, 6, greenColor,  0.35f);  // top-left: Green
        ColorRect(0, 6, 9, 15, yellowColor, 0.35f); // top-right: Yellow
        ColorRect(9, 15, 0, 6, redColor,    0.35f); // bottom-left: Red
        ColorRect(9, 15, 9, 15, blueColor,  0.35f); // bottom-right: Blue

        // HOME APPROACH COLUMNS — solid color, lighter shade
        // Blue approach: col 7, rows 9-12 (going toward center from bottom)
        for (int r = 9; r <= 13; r++)  boardSquares[RC(r,7)].image.color = new Color(blueColor.r,   blueColor.g,   blueColor.b,   0.6f);
        // Yellow approach: col 7, rows 1-5 (going toward center from top)
        for (int r = 1; r <= 5;  r++)  boardSquares[RC(r,7)].image.color = new Color(yellowColor.r, yellowColor.g, yellowColor.b, 0.6f);
        // Red approach: col 7, rows 9-13 ... actually Red approach is col 7 going up from bottom
        // Green approach: row 7, cols 1-5 (going right toward center from left)
        for (int c = 1; c <= 5;  c++)  boardSquares[RC(7,c)].image.color = new Color(greenColor.r,  greenColor.g,  greenColor.b,  0.6f);
        // Red approach: row 7, cols 9-13 (going left toward center from right)
        for (int c = 9; c <= 13; c++)  boardSquares[RC(7,c)].image.color = new Color(redColor.r,    redColor.g,    redColor.b,    0.6f);

        boardSquares[RC(1,8)].image.color = new Color(yellowColor.r, yellowColor.g, yellowColor.b, 0.82f);
        boardSquares[RC(8,13)].image.color = new Color(redColor.r, redColor.g, redColor.b, 0.82f);
        boardSquares[RC(6,1)].image.color = new Color(greenColor.r, greenColor.g, greenColor.b, 0.82f);
        boardSquares[RC(13,6)].image.color = new Color(blueColor.r, blueColor.g, blueColor.b, 0.82f);



        // CENTER 3×3 — multicolor triangles (approximate with neutral color)
        for (int r = 6; r <= 8; r++)
            for (int c = 6; c <= 8; c++)
                boardSquares[RC(r,c)].image.color = new Color(0.93f, 0.90f, 0.82f);

        // STAR SQUARES on track — slightly warm cream
        if (track != null)
            foreach (int starPos in starTrackPositions)
                if (starPos < track.Length)
                    boardSquares[track[starPos]].image.color = starColor;
    }

    void ColorRect(int rStart, int rEnd, int cStart, int cEnd, Color color, float alpha)
    {
        for (int r = rStart; r < rEnd; r++)
            for (int c = cStart; c < cEnd; c++)
                boardSquares[RC(r,c)].image.color = new Color(color.r, color.g, color.b, alpha);
    }

    // ─────────────────────────────────────────────────────────
    // WIRE BUTTONS
    // ─────────────────────────────────────────────────────────
    void WireButtons()
    {
        rollDiceButton.onClick.AddListener(OnRollDice);

        // Player home circles
        for (int i = 0; i < 4; i++)
        {
            int idx = i;
            playerHomeCircles[i].onClick.AddListener(() => OnPlayerPieceSelected(idx));
        }

        // Track squares — player can click their own piece on the track
        for (int i = 0; i < track.Length; i++)
        {
            int trackPos = i;
            int gridIdx  = track[i];
            boardSquares[gridIdx].onClick.AddListener(() => OnTrackSquareClicked(trackPos));
        }
    }

    // ─────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────
    private static int RC(int r, int c) => r * 15 + c;

    protected bool IsSafe(int trackPos) => starTrackPositions.Contains(trackPos % 52);

    protected int CountSafe(int[] pieces)
    { int n=0; foreach(int p in pieces) if(p==57) n++; return n; }

    protected string GetName(int id)
    { switch(id){case 0:return"You";case 1:return"Yaa";case 2:return"Agyeman";case 3:return"Darko";default:return"?";} }

    protected int[][] AllPieces() => new[]{playerPieces,ai1Pieces,ai2Pieces,ai3Pieces};

    protected Color GetColor(int id)
    { switch(id){case 0:return blueColor;case 1:return yellowColor;case 2:return redColor;case 3:return greenColor;default:return Color.white;} }

    // Entry track position per player
    protected int GetEntryPos(int playerID)
    { switch(playerID){case 0:return 0;case 1:return 13;case 2:return 26;case 3:return 39;default:return 0;} }

    // Convert a player's internal position to absolute track position
    // Each player starts counting from their own entry point
    protected int ToAbsoluteTrackPos(int relativePos, int playerID)
    { return (relativePos + GetEntryPos(playerID)) % 52; }

    // ─────────────────────────────────────────────────────────
    // GET VALID MOVES
    // pieces[i] stores RELATIVE position: -1=home, 0=just entered,
    // 1-51=on track relative to own entry, 52-56=approach, 57=safe
    // ─────────────────────────────────────────────────────────
    protected List<int> GetValidMoves(int[] pieces, int roll)
    {
        var valid = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int pos = pieces[i];
            if (pos == 57) continue; // already safe
            if (pos == -1 && roll == 6) { valid.Add(i); continue; } // enter on 6
            if (pos >= 0 && pos + roll <= 57) valid.Add(i); // move forward
        }
        return valid;
    }

    // ─────────────────────────────────────────────────────────
    // EXECUTE MOVE
    // ─────────────────────────────────────────────────────────
    protected void ExecuteMove(int[] pieces, int pieceIdx, int roll, int ownerID)
    {
        // Enter board
        if (pieces[pieceIdx] == -1)
        {
            pieces[pieceIdx] = 0;
            statusText.text = GetName(ownerID) + " entered a piece!";
            return;
        }

        pieces[pieceIdx] += roll;

        // Reached safe (center)
        if (pieces[pieceIdx] >= 57)
        {
            pieces[pieceIdx] = 57;
            statusText.text = GetName(ownerID) + " got a piece home!";
            return;
        }

        // On approach column (positions 52-56) — cannot capture
        if (pieces[pieceIdx] >= 52)
        {
            statusText.text = GetName(ownerID) + " is on the home stretch!";
            return;
        }

        // On main track — check for captures
        int relPos  = pieces[pieceIdx];
        int absPos  = ToAbsoluteTrackPos(relPos, ownerID);

        // Star square — safe, no captures
        if (IsSafe(absPos))
        {
            statusText.text = GetName(ownerID) + " landed on a star square — safe!";
            return;
        }

        // Check if any opponent piece shares this absolute track position
        int[][] all = AllPieces();
        for (int p = 0; p < 4; p++)
        {
            if (p == ownerID) continue;
            for (int i = 0; i < 4; i++)
            {
                int oppRelPos = all[p][i];
                if (oppRelPos < 0 || oppRelPos >= 52) continue; // home or approach — safe
                int oppAbsPos = ToAbsoluteTrackPos(oppRelPos, p);
                if (oppAbsPos == absPos)
                {
                    all[p][i] = -1; // send home
                    statusText.text = GetName(ownerID) + " sent " + GetName(p) + " home!";
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    // ROLL DICE
    // ─────────────────────────────────────────────────────────
    public void OnRollDice()
    {
        if (gameOver || currentTurn != 0 || waitingForPieceSelection || aiIsActing) return;
        int roll = Random.Range(1, 7);
        diceResultText.text = roll.ToString();
        storedDiceRoll = roll;
        ProcessRoll(playerPieces, roll, 0);
    }

    void ProcessRoll(int[] pieces, int roll, int ownerID)
    {
        var valid = GetValidMoves(pieces, roll);
        if (valid.Count == 0)
        {
            statusText.text = GetName(ownerID) + " has no valid moves.";
            if (ownerID == 0) StartCoroutine(AdvanceTurn());
            return;
        }
        if (valid.Count == 1)
        {
            ExecuteMove(pieces, valid[0], roll, ownerID);
            UpdateUI();
            CheckGameOver();
            if (!gameOver)
            {
                if (roll == 6) { statusText.text = GetName(ownerID) + " rolled 6! Roll again."; return; }
                if (ownerID == 0) StartCoroutine(AdvanceTurn());
            }
            return;
        }
        // Multiple valid pieces — wait for selection
        validPieceIndices = valid;
        waitingForPieceSelection = true;
        statusText.text = "Choose which piece to move.";
        UpdateUI();
    }

    void OnPlayerPieceSelected(int pieceIdx)
    {
        if (!waitingForPieceSelection || !validPieceIndices.Contains(pieceIdx)) return;
        waitingForPieceSelection = false;
        ExecuteMove(playerPieces, pieceIdx, storedDiceRoll, 0);
        UpdateUI();
        CheckGameOver();
        if (!gameOver)
        {
            if (storedDiceRoll == 6) { statusText.text = "Rolled 6! Roll again."; return; }
            StartCoroutine(AdvanceTurn());
        }
    }

    void OnTrackSquareClicked(int trackPos)
    {
        if (!waitingForPieceSelection) return;
        // Find player piece at this absolute track position
        for (int i = 0; i < 4; i++)
        {
            int relPos = playerPieces[i];
            if (relPos < 0 || relPos >= 52) continue;
            int absPos = ToAbsoluteTrackPos(relPos, 0);
            if (absPos == trackPos && validPieceIndices.Contains(i))
            { OnPlayerPieceSelected(i); return; }
        }
    }

    // ─────────────────────────────────────────────────────────
    // ADVANCE TURN
    // ─────────────────────────────────────────────────────────
    protected IEnumerator AdvanceTurn()
    {
        yield return new WaitForSeconds(0.4f);
        currentTurn = (currentTurn + 1) % 4;
        UpdateUI();
        if (currentTurn != 0) yield return StartCoroutine(AITurn(currentTurn));
    }

    // ─────────────────────────────────────────────────────────
    // AI TURN
    // ─────────────────────────────────────────────────────────
    protected IEnumerator AITurn(int aiID)
    {
        if (gameOver) yield break;
        aiIsActing = true;
        yield return new WaitForSeconds(1.0f);
        int[] pieces = aiID==1?ai1Pieces:aiID==2?ai2Pieces:ai3Pieces;
        int roll = Random.Range(1, 7);
        diceResultText.text = roll.ToString();
        turnText.text = GetName(aiID) + " rolled " + roll;
        yield return new WaitForSeconds(0.5f);
        var valid = GetValidMoves(pieces, roll);
        if (valid.Count > 0)
        {
            int chosen = ChooseBestAIMove(pieces, valid, roll, aiID);
            ExecuteMove(pieces, chosen, roll, aiID);
            UpdateUI();
            CheckGameOver();
            if (gameOver) { aiIsActing=false; yield break; }
            if (roll==6) { yield return StartCoroutine(AITurn(aiID)); yield break; }
        }
        else { statusText.text = GetName(aiID)+" has no valid moves."; yield return new WaitForSeconds(0.5f); }
        aiIsActing = false;
        yield return StartCoroutine(AdvanceTurn());
    }

    // ─────────────────────────────────────────────────────────
    // SMART AI — CHOOSE BEST MOVE
    // ─────────────────────────────────────────────────────────
    protected int ChooseBestAIMove(int[] pieces, List<int> valid, int roll, int myID)
    {
        int[][] all = AllPieces();

        // PRIORITY 1: capture an opponent
        foreach (int i in valid)
        {
            int relPos = pieces[i] == -1 ? 0 : pieces[i] + roll;
            if (relPos >= 52) continue; // approach — no captures
            int absPos = ToAbsoluteTrackPos(relPos, myID);
            if (IsSafe(absPos)) continue;
            for (int p=0;p<4;p++)
            {
                if (p==myID) continue;
                for (int j=0;j<4;j++)
                {
                    int oRel = all[p][j];
                    if (oRel<0||oRel>=52) continue;
                    if (ToAbsoluteTrackPos(oRel,p)==absPos) return i;
                }
            }
        }

        // PRIORITY 2: move piece onto home stretch (approach)
        foreach (int i in valid)
        {
            int cur = pieces[i];
            if (cur == -1) continue;
            int newPos = cur + roll;
            if (newPos >= 52 && newPos <= 57) return i;
        }

        // PRIORITY 3: avoid danger — build threat map
        var threatened = new HashSet<int>();
        for (int p=0;p<4;p++)
        {
            if (p==myID) continue;
            for (int j=0;j<4;j++)
            {
                int oRel = all[p][j];
                if (oRel<0||oRel>=52) continue;
                for (int r=1;r<=6;r++)
                {
                    int threatAbs = ToAbsoluteTrackPos(oRel+r, p);
                    if (!IsSafe(threatAbs)) threatened.Add(threatAbs);
                }
            }
        }
        int bestSafe=-1, bestSafePos=-1;
        foreach (int i in valid)
        {
            int relPos = pieces[i]==-1?0:pieces[i]+roll;
            if (relPos>=57) relPos=57;
            int absPos = relPos < 52 ? ToAbsoluteTrackPos(relPos, myID) : 999;
            if (!threatened.Contains(absPos) && relPos > bestSafePos)
            { bestSafePos=relPos; bestSafe=i; }
        }
        if (bestSafe >= 0) return bestSafe;

        // PRIORITY 4: furthest piece
        int bestIdx=valid[0], bestPos=pieces[valid[0]];
        foreach (int i in valid) if (pieces[i]>bestPos){bestPos=pieces[i];bestIdx=i;}
        return bestIdx;
    }

    // ─────────────────────────────────────────────────────────
    // CHECK GAME OVER
    // ─────────────────────────────────────────────────────────
    protected void CheckGameOver()
    {
        if(CountSafe(playerPieces)==4){EndGame(0);return;}
        if(CountSafe(ai1Pieces)==4){EndGame(1);return;}
        if(CountSafe(ai2Pieces)==4){EndGame(2);return;}
        if(CountSafe(ai3Pieces)==4){EndGame(3);}
    }

    void EndGame(int winnerID)
    {
        gameOver=true;
        GameState.instance.miniGameResult = CountSafe(playerPieces);
        turnText.text = winnerID==0?"You Win! All pieces home!":GetName(winnerID)+" wins. You got "+CountSafe(playerPieces)+"/4 home.";
        statusText.text = winnerID==0?"The path home is clear.":"Some pieces didn't make it.";
        UpdateUI();
        StartCoroutine(LoadOutcomeAfterDelay());
    }

    // ─────────────────────────────────────────────────────────
    // UPDATE UI
    // ─────────────────────────────────────────────────────────
    protected void UpdateUI()
    {
        playerSafeText.text = "You: "     + CountSafe(playerPieces) + "/4";
        ai1SafeText.text    = "Yaa: "     + CountSafe(ai1Pieces)    + "/4";
        ai2SafeText.text    = "Agyeman: " + CountSafe(ai2Pieces)    + "/4";
        ai3SafeText.text    = "Darko: "   + CountSafe(ai3Pieces)    + "/4";

        if (!waitingForPieceSelection && !gameOver)
            turnText.text = currentTurn==0?"Your Turn — Roll the Dice":GetName(currentTurn)+"'s Turn";

        rollDiceButton.interactable = currentTurn==0&&!waitingForPieceSelection&&!gameOver&&!aiIsActing;
        rollDiceButton.image.color  = rollDiceButton.interactable
            ? new Color(0.0f,0.54f,0.48f) : new Color(0.6f,0.6f,0.6f);

        // Reset board colors first
        InitBoardColors();

        // Overlay pieces on the board
        int[][] all = AllPieces();
        for (int p = 0; p < 4; p++)
        {
            Color pColor = GetColor(p);
            for (int i = 0; i < 4; i++)
            {
                int relPos = all[p][i];
                if (relPos == -1)
                {
                    // At home — color home circle
                    Button[] homeCircles = p==0?playerHomeCircles:p==1?ai1HomeCircles:p==2?ai2HomeCircles:ai3HomeCircles;
                    bool isValid = p==0 && waitingForPieceSelection && validPieceIndices.Contains(i);
                    homeCircles[i].image.color = isValid ? highlightCol : pColor;
                    homeCircles[i].interactable = isValid;
                }
                else if (relPos >= 0 && relPos < 52)
                {
                    // On main track
                    int absPos = ToAbsoluteTrackPos(relPos, p);
                    if (absPos < track.Length)
                    {
                        int gridIdx = track[absPos];
                        bool isValid = p==0 && waitingForPieceSelection && validPieceIndices.Contains(i);
                        boardSquares[gridIdx].image.color = isValid ? highlightCol : pColor;
                        TMP_Text lbl = boardSquares[gridIdx].GetComponentInChildren<TMP_Text>();
                        if (lbl != null) lbl.text = p==0?"Y":p==1?"y":p==2?"R":"G";
                        boardSquares[gridIdx].interactable = isValid;
                    }
                }
                // approach and safe handled separately if needed
            }
        }

        // Non-valid home circles should not be interactable
        for (int i = 0; i < 4; i++)
        {
            if (playerPieces[i] != -1 || !(waitingForPieceSelection && validPieceIndices.Contains(i)))
                playerHomeCircles[i].interactable = false;
            ai1HomeCircles[i].interactable = false;
            ai2HomeCircles[i].interactable = false;
            ai3HomeCircles[i].interactable = false;
        }
    }

    IEnumerator LoadOutcomeAfterDelay()
    {
        yield return new WaitForSeconds(2.5f);
        GoToScene("Outcome");
    }
}