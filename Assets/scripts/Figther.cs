using System;
using UnityEngine;

public enum Specie
{
    chameleon, bear, snake, frog
}

[DefaultExecutionOrder(0)]
public class Figther : MonoBehaviour
{
    [Header("PlayerData")]
    [SerializeField] int playerLive;
    [SerializeField] Specie specie;
    [SerializeField] int deckSize;
    [SerializeField] int handSize = 6;
    public int avalaibleCard = 0;

    [Header("ExtraData")]
    [SerializeField] GameObject tokenPrefab;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] public PlayerToken playerToken;
    [SerializeField] public int visualPlayer = 0;
    [SerializeField] public int indexPlayer = -1;
    [SerializeField] public RockBehavior initialStone;

    int lastVisualPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastVisualPlayer = visualPlayer;
        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(visualPlayer).gameObject.SetActive(true);

        if (playerToken == null) playerToken = Instantiate(tokenPrefab).transform.GetComponent<PlayerToken>();
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
        if (lastVisualPlayer != visualPlayer)
        {
            transform.GetChild(lastVisualPlayer).gameObject.SetActive(false);
            transform.GetChild(visualPlayer).gameObject.SetActive(true);
            lastVisualPlayer = visualPlayer;
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

    public int GetPlayerLive()
    {
        return playerLive;
    }

    public void setPlayerLive(int pL)
    {
        playerLive = pL;
    }

    public void addPlayerLive(int pL)
    {
        playerLive += pL;
    }

    public void randomSpecie()
    {
        specie = (Specie)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Specie)).Length);
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
    }
    public bool IsFigthing()
    {
        int figthers = Combatjudge.combatjudge.GetPlayersFigthing();
        int a = 0;

        while (figthers > 0)
        {
            int red = figthers % 2;
            figthers = (int)Mathf.Floor(figthers / 2);

            if (a == indexPlayer) return red != 0;

            a++;
        }
        
        return false;
    }

    public void ThrowCard()
    {
        GetComponentInChildren<HolderPlay>().PlayCard(null);
    }
}
