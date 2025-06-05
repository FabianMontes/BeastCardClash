using System;
using Unity.Mathematics;
using UnityEngine;

public enum Specie { 
    chameleon, bear, snake, frog
}

[DefaultExecutionOrder(0)]
public class Player : MonoBehaviour
{
    [Header("PlayerData")]
    [SerializeField] int playerLive;
    [SerializeField] Specie specie;
    [SerializeField] Card[] originalDeck;

    Card[] actualDeck;
    Card[] hand;

    [Header("ExtraData")]
    [SerializeField] GameObject tokenPrefab;
    [SerializeField] PlayerToken playerToken;
    [SerializeField] public int visualPlayer = 0;
    [SerializeField] public RockBehavior initialStone;

    int lastVisualPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastVisualPlayer = visualPlayer;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(visualPlayer).gameObject.SetActive(true);

        if(playerToken == null)
        {
            playerToken = Instantiate(tokenPrefab).transform.GetComponent<PlayerToken>();
            
        }
        playerToken.player = this;
        playerToken.rocky = initialStone;
    }

    // Update is called once per frame
    void Update()
    {
        if(lastVisualPlayer != visualPlayer)
        {
            transform.GetChild(lastVisualPlayer).gameObject.SetActive(false);
            transform.GetChild(visualPlayer).gameObject.SetActive(true);
            lastVisualPlayer = visualPlayer;
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
        specie = (Specie) UnityEngine.Random.Range(0,Enum.GetValues(typeof(Specie)).Length);
    }

    public void movePlayer(RockBehavior rocker)
    { 
        playerToken.rocky = rocker;
    }
}
