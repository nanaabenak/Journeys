using System;

[Serializable]
public class StoryData {
    public string story_id;
    public string country;
    public string mini_game;
    public DelegateData delegate_info;
    public DialogueLine[] dialogue;
    public DecisionData decision;
    public OutcomeData outcomes;
    public NegotiationData negotiation;
}

[Serializable]
public class DelegateData {
    public string name;
    public string title;
    public string sprite;
}

[Serializable]
public class DialogueLine {
    public string speaker;
    public string text;
    public string sprite;  // optional — can be null
}

[Serializable]
public class DecisionData {
    public string title;
    public string question;
    public CardData option_a;
    public CardData option_b;
}

[Serializable]
public class CardData {
    public string label;
    public string description;
    public string preamble;
}

[Serializable]
public class OutcomeData {
    public TierData a;
    public TierData b;
}

[Serializable]
public class TierData {
    public string tier_1;
    public string tier_2;
    public string tier_3;
    public string tier_4;
}

[Serializable]
public class NegotiationData
{
    public StakeholderInfo[] stakeholders;
    public NegotiationExchange[] milestones;
    public MomentumPool momentum;
}

[Serializable]
public class StakeholderInfo
{
    public string name;
    public string sprite;
}

[Serializable]
public class NegotiationExchange
{
    public string trigger;           // milestone key e.g. 'first_capture'
    public string speaker;           // delegate name
    public string text;              // delegate line
    public string response_speaker;  // stakeholder name
    public string response_text;     // stakeholder response
    public int active_stakeholder_index; // for Ludo — which stakeholder portrait highlights (0,1,2)
}

[Serializable]
public class MomentumPool
{
    public NegotiationLinePool losing;
    public NegotiationLinePool even;
    public NegotiationLinePool winning;
}

[Serializable]
public class NegotiationLinePool
{
    public NegotiationExchange[] lines;
}