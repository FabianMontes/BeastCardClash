using System;
using UnityEngine;

public enum Elements
{
    fire, earth, water, air
}

public enum SetMoments
{
    PickDice,RollDice,RevealDice,
    GlowRock, MoveToRock,SelecCombat,
    PickCard,Reveal, Result,
    End

}

public enum Results
{
    one,two, draw
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
            print(rocky);
            players[i].initialStone = rocky;
            


        }


            

    }

    // Update is called once per frame
    void Update()
    {
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
            case SetMoments.End:
                break;
        }
    }

    public Results indvCombat(Card one,Card two)
    {
        int countelements = Enum.GetValues(typeof(Elements)).Length;
        int halflements = countelements / 2;
        int diferer = Math.Abs((int)one.element - (int)two.element) % countelements;
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
        print(value);

    }

    public SetMoments GetSetMoments()
    {
        return setMoments;
    }
}
