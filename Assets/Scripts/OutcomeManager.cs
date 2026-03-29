using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OutcomeManager : SceneBase
{
    // ── UI References ─────────────────────────────────────
    public TMP_Text storyLabel;
    public TMP_Text decisionLabel;
    public TMP_Text impactLabel;
    public TMP_Text outcomeText;
    public Button   playAgainButton;
    public Image    background;

    // ── Impact tier colors ────────────────────────────────
    private Color tier1Color = new Color(0.55f, 0.55f, 0.55f); // grey — minimal
    private Color tier2Color = new Color(0.90f, 0.65f, 0.10f); // amber — moderate
    private Color tier3Color = new Color(0.10f, 0.45f, 0.85f); // blue — strong
    private Color tier4Color = new Color(0.10f, 0.65f, 0.20f); // green — maximum

    // ─────────────────────────────────────────────────────
    void Start()
    {
        playAgainButton.transition = Selectable.Transition.None;
        playAgainButton.onClick.AddListener(() => StartCoroutine(LoadScene("CountryContext")));

        int story   = GameState.instance.currentStory;
        int decision = GameState.instance.decisionMade;
        int score    = GameState.instance.miniGameResult;

        // Story label
        storyLabel.text = story == 0 ? "Education — Ghana" : "Galamsey — Ghana";

        // Decision label
        decisionLabel.text = decision == 1 ? "You chose: Option A" : "You chose: Option B";

        // Calculate tier from score
        int tier = GetTier(story, score);
        SetImpactLabel(tier);

        // Show outcome text
        outcomeText.text = GetOutcomeText(story, decision, tier);
    }

    // ─────────────────────────────────────────────────────
    // GET TIER — converts mini game score to impact tier 1-4
    // ─────────────────────────────────────────────────────
    int GetTier(int story, int score)
    {
        if (story == 0) // Oware — seeds captured out of 48
        {
            if (score <= 12) return 1;
            if (score <= 24) return 2;
            if (score <= 36) return 3;
            return 4;
        }
        else // Dame — AI pieces captured out of 12
        {
            if (score <= 3)  return 1;
            if (score <= 6)  return 2;
            if (score <= 9)  return 3;
            return 4;
        }
    }

    // ─────────────────────────────────────────────────────
    // SET IMPACT LABEL
    // ─────────────────────────────────────────────────────
    void SetImpactLabel(int tier)
    {
        switch (tier)
        {
            case 1: impactLabel.text = "Impact: Minimal";  impactLabel.color = tier1Color; break;
            case 2: impactLabel.text = "Impact: Moderate"; impactLabel.color = tier2Color; break;
            case 3: impactLabel.text = "Impact: Strong";   impactLabel.color = tier3Color; break;
            case 4: impactLabel.text = "Impact: Maximum";  impactLabel.color = tier4Color; break;
        }
    }

    // ─────────────────────────────────────────────────────
    // GET OUTCOME TEXT — hardcoded for submission
    // ─────────────────────────────────────────────────────
    string GetOutcomeText(int story, int decision, int tier)
    {
        if (story == 0) // Education
        {
            if (decision == 1) // Option A — Expansion
            {
                switch (tier)
                {
                    case 1: return "The schools were announced. Half were built. Enrollment ticked up slightly. Reading rates did not move. Kwame dropped out last month.";
                    case 2: return "The new schools are open and enrollment climbs. Three years later a quiet assessment finds learning poverty unchanged. The buildings exist. The teaching does not.";
                    case 3: return "Enrollment hits a record high. One young teacher starts a reading circle on her own. Fifteen children improve. It is not policy. It is one person refusing to accept the system.";
                    case 4: return "Enrollment hits ninety-three percent. Ghana wins an international award. Kofi attends the ceremony. On the walk home he passes a classroom where sixty children share three textbooks.";
                    default: return "";
                }
            }
            else // Option B — Foundation Fix
            {
                switch (tier)
                {
                    case 1: return "The reform passes but funding arrives incomplete. Some progress is real but fragile. Kwame learns to read his own name by end of term. Kofi celebrates it like it is enormous. Because it is.";
                    case 2: return "Class sizes fall to forty. Teachers are trained. Reading rates improve eighteen percent in pilot areas. Kwame is referred to a technical aptitude programme. He thrives.";
                    case 3: return "The reform takes hold. Class sizes average thirty-five. Reading rates jump thirty-one percent. Kwame reads his first full book. He reads it twice.";
                    case 4: return "Reading rates jump thirty-eight percent in one year — among the best results on the continent. Kwame wins the district science fair with a working water filtration model he built from scrap. Kofi is there. He just nods.";
                    default: return "";
                }
            }
        }
        else // Galamsey
        {
            if (decision == 1) // Option A — Crackdown
            {
                switch (tier)
                {
                    case 1: return "Mining stops in the district. Three thousand young men scatter. Six months later Ama gets a call from a water keeper two districts east. The river there is also orange now.";
                    case 2: return "Enforcement is partial. Connected operators continue. Mercury drops slightly then plateaus. The river is still orange. Kofi moved to a new site further north.";
                    case 3: return "The crackdown holds for eight months. Then the funding dries up. The military rotates out. Within sixty days mining restarts. They had more patience than the operation had budget.";
                    case 4: return "The district is declared mining-free. Ama tests the river. Mercury is falling. Then she gets a message — a water keeper near the border. Orange water. New machinery. Familiar faces. The problem found a new address.";
                    default: return "";
                }
            }
            else // Option B — Alternative
            {
                switch (tier)
                {
                    case 1: return "Forty young men enroll. Most stay mining — the stipend doesn't cover what galamsey pays. One man finishes and finds work three hours away. It is not enough to call success. It is enough to call a beginning.";
                    case 2: return "Three hundred young men enroll. Kofi finishes training in six months. He gets a job. Mining drops thirty percent in the pilot district. Ama tests the water. Still orange. But the trend is moving.";
                    case 3: return "Mining drops fifty-two percent in eighteen months. Legal licences fast-tracked for local youth. Ama tests the water — best readings in a decade. Still above safe. But the direction is down.";
                    case 4: return "Mining drops sixty percent. A cooperative of former miners farms palm oil on land that was stripped for gold three years ago. Ama tests the water at year two. Still above safe. But she notes the direction. Down. Slowly. Truthfully.";
                    default: return "";
                }
            }
        }
        return "Something went wrong loading the outcome.";
    }
}