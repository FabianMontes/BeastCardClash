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
    [SerializeField] int manyFigthers;
    public int round { get; private set; }
    [SerializeField] GameObject player;
    [SerializeField] GameObject Bots;
    Figther[] figthers;

    [Header("GameRules")]
    [SerializeField] int figtherTurn;
    [SerializeField] SetMoments actualAction;
    [SerializeField] public int maxDice;
    [SerializeField] public int initialLives;
    [SerializeField] int damageDealt;
    [SerializeField] int damageHeal;

    public CombatType combatType { get; private set; }
    private int playersFigthing;
    private int manyFigthersFigthing;
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

        actualAction = SetMoments.Loop;
        Figther[] playeres = FindObjectsByType<Figther>(FindObjectsSortMode.InstanceID);

        if (playeres.Length > manyFigthers)
        {
            manyFigthers = playeres.Length;
        }

        PlayZone zone = FindFirstObjectByType<PlayZone>();
        Canvas canvas = FindFirstObjectByType<Canvas>();

        int div = zone.many / manyFigthers;

        figthers = new Figther[manyFigthers];

        for (int i = 0; i < manyFigthers; i++)
        {
            GameObject figther = i == 0 ? player : Bots;
            if (playeres.Length <= i)
            {
                figthers[i] = Instantiate(figther).GetComponent<Figther>();
                figthers[i].transform.SetParent(canvas.transform, false);
                figthers[i].randomSpecie();
            }
            else
            {
                figthers[i] = playeres[i];
            }
            figthers[i].setPlayerLive(initialLives);
            figthers[i].visualFigther = i+1;
            figthers[i].indexFigther = i;

            RockBehavior rocky = zone.transform.GetChild(i * div).GetComponent<RockBehavior>();
            figthers[i].initialStone = rocky;
        }
    }

    // Update is called once per frame
    void Update()
    {


        switch (actualAction)
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
                foreach( Figther  player in figthers)

                {
                    if (player.getPicked() == null && player.IsFigthing())
                    {
                        all = false;
                        break;
                    }
                }
                if (all)
                {
                    actualAction = SetMoments.Reveal;
                }

                break;
            case SetMoments.Reveal:
                actualAction = SetMoments.Result;
                break;
            case SetMoments.Result:
                Card[] card = new Card[manyFigthers];
                int a = 0;
                foreach (Figther player in figthers)
                {
                    card[a] = player.getPicked();
                    a++;
                }

                Results[,] results = new Results[manyFigthers, manyFigthers];
                for (int i = 0; i < manyFigthers; i++)
                {
                    for (int j = 0; j < manyFigthers; j++)
                    {
                        results[i, j] = IndvCombat(card[i], card[j]);
                        print(results[i, j]);
                    }
                    print("dd");
                }

                int[] destiny = new int[manyFigthers];
                for (int i = 0; i < manyFigthers; i++)
                {
                    destiny[i] = 0;
                    for (int j = 0; j < manyFigthers; j++)
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
                    figthers[i].addPlayerLive(destiny[i]);
                }

                actualAction = SetMoments.Loop;
                break;
            case SetMoments.Loop:
                figtherTurn = (figtherTurn + 1) % manyFigthers;
                for (int i = 0; i < manyFigthers; i++)
                {
                    figthers[i].RefillHand();
                    figthers[i].ThrowCard();
                }
                actualAction = SetMoments.PickDice;
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
        RockBehavior lander = figthers[figtherTurn].playerToken.rocky;
        RockBehavior[] rocker = lander.getNeighbor(value);
        rocker[0].shiny = true;
        rocker[1].shiny = true;
        actualAction = SetMoments.GlowRock;
        if(figtherTurn != 0)
        {
            figthers[figtherTurn].transform.GetComponent<BotPlayer>().pickRock(rocker);
        }
    }

    public SetMoments GetSetMoments()
    {
        return actualAction;
    }
    public void ArriveAtRock()
    {
        RockBehavior rocky = figthers[figtherTurn].playerToken.rocky;
        if (rocky.manyOn())
        {
            playersFigthing = rocky.GetPlayersOn();
            manyFigthersFigthing = rocky.ManyPlayerOn();
        }
        else
        {
            playersFigthing = (int)Mathf.Pow(2, manyFigthers) - 1;

            manyFigthersFigthing = manyFigthers;
        }
        if (rocky.inscription == Inscription.pick)
        {
            actualAction = SetMoments.SelecCombat;
        }
        else
        {
            actualAction = SetMoments.PickCard;
            combatType = (CombatType)(int)rocky.inscription;
        }
    }

    public void MoveToRock(RockBehavior rocker)
    {
        figthers[figtherTurn].playerToken.rocky = rocker;
        actualAction = SetMoments.MoveToRock;
    }

    public bool pickElement(Element element)
    {
        if (SetMoments.SelecCombat == actualAction)
        {
            try
            {
                combatType = (CombatType)(int)element;
                actualAction = SetMoments.PickCard;
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
        return figthers[figtherTurn].visualFigther == 1;
    }
}
