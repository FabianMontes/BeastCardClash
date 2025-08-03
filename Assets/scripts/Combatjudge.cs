using System;
using UnityEngine;

public enum Element
{
    fire, earth, water, air
}

public enum SetMoments
{
    PickDice, RollDice, RevealDice,
    GlowRock, MoveToRock, SelecCombat,
    PickCard, Reveal, Result,
    End, Loop
}

public enum Results
{
    lose, draw, win
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

    public CombatType combatType;
    private int playersFigthing;
    private int manyplayersFigthing;
    public static Combatjudge combatjudge;
    private bool all;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (combatjudge == null)
        {
            combatjudge = this;
        }
        else
        {
            Destroy(gameObject);
        }

        setMoments = SetMoments.Loop;
        Player[] playeres = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID);

        if (playeres.Length > manyPlayers)
        {
            manyPlayers = playeres.Length;
        }

        PlayZone zone = FindFirstObjectByType<PlayZone>();
        Canvas canvas = FindFirstObjectByType<Canvas>();

        int div = zone.many / manyPlayers;

        players = new Player[manyPlayers];

        for (int i = 0; i < manyPlayers; i++)
        {
            if (playeres.Length <= i)
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
            players[i].visualPlayer = i + 1;
            players[i].indexPlayer = i;
            RockBehavior rocky = zone.transform.GetChild(i * div).GetComponent<RockBehavior>();
            players[i].initialStone = rocky;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString())) AsignarNumeros(i);
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
                all = true;
                foreach (Player player in players)
                {
                    if (player.getPicked() == null && player.IsFigthing())
                    {
                        all = false;
                        break;
                    }
                }
                if (all)
                {
                    setMoments = SetMoments.Reveal;
                }

                break;
            case SetMoments.Reveal:
                setMoments = SetMoments.Result;
                break;
            case SetMoments.Result:
                Card[] card = new Card[manyPlayers];
                int a = 0;
                foreach (Player player in players)
                {
                    card[a] = player.getPicked();
                    a++;
                }

                Results[,] results = new Results[manyPlayers, manyPlayers];
                for (int i = 0; i < manyPlayers; i++)
                {
                    for (int j = 0; j < manyPlayers; j++)
                    {
                        results[i, j] = IndvCombat(card[i], card[j]);
                        print(results[i, j]);
                    }
                    print("dd");
                }

                int[] destiny = new int[manyPlayers];
                for (int i = 0; i < manyPlayers; i++)
                {
                    destiny[i] = 0;
                    for (int j = 0; j < manyPlayers; j++)
                    {
                        int resulta = (int)results[i, j] - 1;
                        if (destiny[i] == 0)
                        {
                            destiny[i] = resulta;
                        }
                        else if (destiny[i] != resulta && resulta != 0)
                        {
                            destiny[i] = 0;
                            break;
                        }
                    }
                    print(destiny[i]);
                    players[i].addPlayerLive(destiny[i]);
                }

                setMoments = SetMoments.Loop;
                break;
            case SetMoments.Loop:
                playerTurn = (playerTurn + 1) % manyPlayers;
                for (int i = 0; i < manyPlayers; i++)
                {
                    players[i].RefillHand();
                    players[i].ThrowCard();
                }
                setMoments = SetMoments.PickDice;
                playersFigthing = 0;

                break;
            case SetMoments.End:
                break;
        }
    }

    public Results IndvCombat(Card one, Card two)
    {
        if (one == null || two == null)
        {
            return Results.draw;
        }

        print(combatType);
        if (combatType != CombatType.full)
        {
            if (one.GetElement() != two.GetElement())
            {
                return (int)one.GetElement() == (int)combatType ? Results.win : Results.lose;
            }
        }
        int countelements = Enum.GetValues(typeof(Element)).Length;
        int halflements = countelements / 2;
        int diferer = (one.GetElement() - two.GetElement() + countelements) % countelements;
        if (countelements % 2 == 0)
        {

            if (diferer == 0 || diferer == halflements)
            {
                if (one.GetValue() > two.GetValue())
                {
                    return Results.win;
                }
                else if (one.GetValue() < two.GetValue())
                {
                    return Results.lose;
                }
                else
                {
                    return Results.draw;
                }
            }
            else
            {
                return diferer > halflements ? Results.win : Results.lose;
            }
        }
        else
        {
            if (diferer == 0)
            {
                if (one.GetValue() > two.GetValue())
                {
                    return Results.win;
                }
                else if (one.GetValue() < two.GetValue())
                {
                    return Results.lose;
                }
                else
                {
                    return Results.draw;
                }
            }
            else
            {
                return diferer > halflements ? Results.win : Results.lose;
            }
        }
    }

    public void Roled(int value)
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
        RockBehavior rocky = players[playerTurn].playerToken.rocky;
        if (rocky.manyOn())
        {
            playersFigthing = rocky.GetPlayersOn();
            manyplayersFigthing = rocky.ManyPlayerOn();
        }
        else
        {
            playersFigthing = (int)Mathf.Pow(2, manyPlayers) - 1;

            manyplayersFigthing = manyPlayers;
        }
        if (rocky.inscription == Inscription.pick)
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
                players[i].visualPlayer = 1;
            }
            else
            {
                // Calcular distancia circular desde baseIndex
                int offset = (i - baseIndex + manyPlayers) % manyPlayers + 1;
                players[i].visualPlayer = offset;
            }
        }

    }

    public bool pickElement(Element element)
    {
        if (SetMoments.SelecCombat == setMoments)
        {
            try
            {
                combatType = (CombatType)(int)element;
                setMoments = SetMoments.PickCard;
            }
            catch (Exception e)
            {
                print(e);
                return false;
            }
            return true;
        }
        return false;
    }

    public int GetPlayersFigthing()
    {
        return playersFigthing;
    }
    public bool FocusONTurn()
    {
        return players[playerTurn].visualPlayer == 1;
    }
}
