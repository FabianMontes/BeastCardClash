using NUnit.Framework.Constraints;
using System;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;

public enum Elements
{
    fire, earth, water, air
}

public enum SetMoments
{
    PickDice,RollDice,RevealDice,
    GlowRock, MoveToRock,SelecCombat,
    PickCard,Reveal, Result,
    End, Loop

}

public enum Results
{
    one,two, draw
}

public enum CombatType
{
    fire, earth, water, air, full
}

[DefaultExecutionOrder(-1)]

public class Combatjudge : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] int manyPlayers;
    [SerializeField] GameObject player;
    Player[] players;

    [Header("GameRules")]
    [SerializeField] int playerTurn;
    [SerializeField] SetMoments setMoments;
    [SerializeField] public int maxDice;
    [SerializeField] int initialLives;

    public int diceRolled;
    public CombatType combatType;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setMoments = SetMoments.PickDice;
        Player[] playeres = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID);

        if (playeres.Length > manyPlayers)
        {
            manyPlayers = playeres.Length;
        }

        PlayZone zone = FindFirstObjectByType<PlayZone>();
        Canvas canvas = FindFirstObjectByType<Canvas>();

        int div = zone.many/ manyPlayers;

        players = new Player[manyPlayers];

        for (int i = 0; i < manyPlayers; i++)
        {
            if (playeres.Length <= i )
            {
                players[i] = Instantiate(player).GetComponent<Player>();
                players[i].transform.SetParent(canvas.transform, false);
                players[i].randomSpecie();
            }
            else
            {
                players[i] = playeres[i];
            }
            players[i].setPlayerLive(initialLives);
            players[i].visualPlayer = i;
            RockBehavior rocky = zone.transform.GetChild(i * div).GetComponent<RockBehavior>();
            players[i].initialStone = rocky;
            


        }


            

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                AsignarNumeros(i);
            }
        }

        switch (setMoments)
        {
            case SetMoments.PickDice:
                break;
            case SetMoments.RollDice:
                break;
            case SetMoments.RevealDice:
                break;
            case SetMoments.GlowRock:
                break;
            case SetMoments.MoveToRock:
                break;
            case SetMoments.SelecCombat:
                break;
            case SetMoments.PickCard:
                break;
            case SetMoments.Reveal:
                break;
            case SetMoments.Result:
                break;
            case SetMoments.Loop:
                playerTurn = (playerTurn + 1) % manyPlayers;
                setMoments = SetMoments.PickDice;
                break;
            case SetMoments.End:
                break;
        }
    }

    public Results indvCombat(Card one,Card two)
    {
        int countelements = Enum.GetValues(typeof(Elements)).Length;
        int halflements = countelements / 2;
        int diferer = (one.element - two.element + countelements) % countelements;
        if (countelements % 2 == 0)
        {
            
            if (diferer == 0 || diferer == halflements) 
            {
                if (one.value > two.value)
                {
                    return Results.one;
                }
                else if (one.value < two.value)
                {
                    return Results.two;
                }
                else
                {
                    return Results.draw;
                }
            }
            else
            {
                    return diferer > halflements ? Results.one : Results.two;
            }
        }
        else       
        {
            if(diferer == 0)
            {
                if (one.value > two.value)
                {
                    return Results.one;
                }
                else if (one.value < two.value)
                {
                    return Results.two;
                }
                else
                {
                    return Results.draw;
                }
            }
            else
            {
                return diferer>halflements ? Results.one : Results.two;
            }
        }
        
    }

    public void roled(int value)
    {
        RockBehavior lander = players[playerTurn].playerToken.rocky;
        RockBehavior[] rocker = lander.getNeighbor(value);
        rocker[0].shiny = true;
        rocker[1].shiny = true;
        setMoments = SetMoments.GlowRock;

    }

    public SetMoments GetSetMoments()
    {
        return setMoments;
    }
    public void ArriveAtRock()
    {
        setMoments =  SetMoments.Loop;
        return;

        RockBehavior rocky =  players[playerTurn].playerToken.rocky;
        if (rocky.manyOn() || rocky.inscription == Inscription.pick)
        {
            setMoments = SetMoments.SelecCombat;
            
        }
        else
        {
            setMoments = SetMoments.PickCard;
            combatType = (CombatType)(int)rocky.inscription;
        }


    }

    public void MoveToRock(RockBehavior rocker)
    {
        players[playerTurn].playerToken.rocky = rocker;
        setMoments = SetMoments.MoveToRock;
    }

    void AsignarNumeros(int baseIndex)
    {
        baseIndex = baseIndex % manyPlayers;
        for (int i = 0; i < manyPlayers; i++)
        {
            if (i == baseIndex)
            {
                players[i].visualPlayer = 0;
            }
            else
            {
                // Calcular distancia circular desde baseIndex
                int offset = (i - baseIndex + manyPlayers) % manyPlayers;
                players[i].visualPlayer = offset;
            }
        }

    }
}
