
using UnityEngine;

public enum Team
{
    acetiles, ingeniosos, adn, zootecnicos, RCPTeam, pluma_dorada,real_pincel, photo_agros,va_games
}

public enum Specie
{
    bear, frog, chameleon, condor
}


[DefaultExecutionOrder(0)]
public class Figther : MonoBehaviour
{
    [Header("FigtherData")]
    [SerializeField] int figtherLive;
    [SerializeField] public string figtherName;
    [SerializeField] Team team;
    [SerializeField] Specie specie;
    public int skin { get; private set; }
    [SerializeField] int deckSize;
    [SerializeField] int handSize = 6;
    public int avalaibleCard = 0;

    [Header("ExtraData")]
    [SerializeField] GameObject tokenPrefab;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] public PlayerToken playerToken;
    [SerializeField] public int visualFigther = 0;
    [SerializeField] public int indexFigther = -1;
    [SerializeField] public RockBehavior initialStone;

    int lastVisualPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastVisualPlayer = visualFigther;
        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(visualFigther).gameObject.SetActive(true);

        if (playerToken == null) playerToken = Instantiate(tokenPrefab, initialStone.transform.position + Vector3.up * 1, Quaternion.identity).transform.GetComponent<PlayerToken>();
        playerToken.player = this;
        playerToken.rocky = initialStone;

        // Create provitionalDeck

        for (int i = 0; i < deckSize; i++)
        {
            Card card = Instantiate(cardPrefab, transform.GetChild(0).GetChild(0)).GetComponent<Card>();
            card.indexer = i;
        }

        // Create Hand
    }

    // Update is called once per frame
    void Update()
    {
        if (lastVisualPlayer != visualFigther)
        {
            transform.GetChild(lastVisualPlayer).gameObject.SetActive(false);
            transform.GetChild(visualFigther).gameObject.SetActive(true);
            lastVisualPlayer = visualFigther;
        }
        if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard && IsFigthing())
        {
            if (avalaibleCard == 0)
            {
                Transform hand = transform.GetChild(0).GetChild(1);
                for (int i = 0; i < handSize; i++)
                {
                    hand.GetChild(i).GetComponent<HandCard>().ForceReveal();
                }
            }
        }
    }

    public Specie GetSpecie()
    {
        return specie;
    }

    public void setTeam(Team team)
    {
        this.team = team;
    }

    public void setSkin(int skin)
    {
        this.skin=skin;
    }

    public void setNoSkin(int skin)
    {
        int a = Random.Range(0, 6);
        while (a== skin)
        {
            a = Random.Range(0, 6);
        }
        this.skin = a;
    }

    public void setRSkin()
    {
        skin = Random.Range(0, 6);
    }

    public void setNoTeam(Team team)
    {
        this.team = (Team)Random.Range(0, 8);
        while (this.team == team)
        {
            this.team = (Team)Random.Range(0, 8);
        }
    }

    public void FreeTeam()
    {
        team = (Team)Random.Range(0, 8);
    }

    public Team GetTeam()
    {
        return team;
    }

    public int GetPlayerLive()
    {
        return figtherLive;
    }

    public void setPlayerLive(int pL)
    {
        figtherLive = pL;
    }

    public bool noHurt {get;private set;}

    public void addPlayerLive(int pL)
    {
        noHurt = pL >= 0;

        figtherLive += pL;
        if(figtherLive> Combatjudge.combatjudge.initialLives)
        {
            figtherLive = Combatjudge.combatjudge.initialLives;
        }
        if(figtherLive< 0)
        {
            figtherLive = 0;
        }
    }

    public void randomSpecie()
    {
        specie = Specie.bear;
        skin = Random.Range(0,2);
    }

    public void movePlayer(RockBehavior rocker)
    {
        playerToken.rocky = rocker;
    }

    public Card getPicked()
    {
        return GetComponentInChildren<HolderPlay>().GetPicked();
    }

    public void DrawCard(int index, int HandDex)
    {
        Transform deck = transform.GetChild(0).GetChild(0);

        if (index >= deck.childCount || index < 0) return;
        
        Card card = deck.GetChild(index).GetComponent<Card>();
        deck.GetChild(index).gameObject.SetActive(false);
        Transform hand = transform.GetChild(0).GetChild(1);
        hand.GetChild(HandDex).GetComponent<HandCard>().SetCard(card);
    }

    public void PlayCard(Card card)
    {
        GetComponentInChildren<HolderPlay>().PlayCard(card);
    }

    private void DrawCard(int index)
    {
        int cardDrawedindex = UnityEngine.Random.Range(0, deckSize);
        Transform deck = transform.GetChild(0).GetChild(0);
        Transform hand = transform.GetChild(0).GetChild(1);
        while (!deck.GetChild(cardDrawedindex).gameObject.activeSelf)
        {
            cardDrawedindex = (cardDrawedindex + 1) % deckSize;
        }

        hand.GetChild(index).GetComponent<HandCard>().SetCard(deck.GetChild(cardDrawedindex).GetComponent<Card>());
        deck.GetChild(cardDrawedindex).gameObject.SetActive(false);
    }

    public void RefillHand()
    {
        Transform hand = transform.GetChild(0).GetChild(1);
        for (int i = 0; i < handSize; i++)
        {
            if (hand.GetChild(i).GetComponent<HandCard>().GetCard() == null) DrawCard(i);
        }
        avalaibleCard = 0;
    }
    public bool IsFigthing()
    {
        int figthers = Combatjudge.combatjudge.GetPlayersFigthing();
        int a = 0;

        while (figthers > 0)
        {
            int red = figthers % 2;
            figthers = (int)Mathf.Floor(figthers / 2);

            if (a == indexFigther) return red != 0;
            a++;
        }
        
        return false;
    }

    public void ThrowCard()
    {
        GetComponentInChildren<HolderPlay>().PlayCard(null);
    }
}
