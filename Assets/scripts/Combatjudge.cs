using System;
using UnityEngine;
using System.Linq;


public enum Element
{
    fire, earth, water, air
}

public enum SetMoments
{
    PickDice, RollDice, RevealDice,
    GlowRock, MoveToRock, SelecCombat,
    PickCard, Reveal, Result,
    End, Loop, round, rounded
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
        manyFigthers = UnityEngine.Random.Range(2, 5);
        actualAction = SetMoments.Loop;
        figtherTurn = -1;
        round = 0;
        Figther[] playeres = FindObjectsByType<Figther>(FindObjectsSortMode.InstanceID);
        int a = 0;
        while (a < playeres.Length)
        {
            int numero = int.Parse(playeres[a].name[0].ToString());
            Figther f = playeres[numero];
            playeres[numero] = playeres[a];
            playeres[a] = f;
            if (a == numero)
            {
                a++;
            }
        }

        int dif = playeres.Length - manyFigthers;
        for (; dif > 0; dif--)
        {
            Destroy(playeres[playeres.Length - dif].gameObject);
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
            if (i == 0)
            {
                figthers[i].setTeam(GameState.singleton.team);
                figthers[i].setSkin(GameState.singleton.skin);
            }
            else
            {
                if (manyFigthers == 2)
                {
                    figthers[i].setNoTeam(GameState.singleton.team);
                }
                else if (manyFigthers == 3 && i == 2)
                {

                    if (figthers[0].GetTeam() == figthers[1].GetTeam())
                    {
                        figthers[i].setNoTeam(GameState.singleton.team);
                    }
                    else
                    {
                        figthers[i].FreeTeam();
                    }
                }
                else if (manyFigthers == 4 && i == 3)
                {
                    if (figthers[0].GetTeam() == figthers[1].GetTeam() && figthers[0].GetTeam() == figthers[2].GetTeam())
                    {
                        figthers[i].setNoTeam(GameState.singleton.team);
                    }
                    else
                    {
                        figthers[i].FreeTeam();
                    }
                }
                else
                {
                    figthers[i].FreeTeam();
                }
                figthers[i].setRSkin();
                //figthers[i].setNoTeam(figthers[0].GetTeam());

            }

            figthers[i].setPlayerLive(initialLives);
            figthers[i].visualFigther = i + 1;
            figthers[i].indexFigther = i;

            figthers[i].figtherName = i == 0 ? GameState.singleton.playerName : $"O{i}O";

            RockBehavior rocky = zone.transform.GetChild(i * div).GetComponent<RockBehavior>();
            figthers[i].initialStone = rocky;
        }
    }

    float time;


    // Update is called once per frame
    void Update()
    {


        switch (actualAction)
        {
            case SetMoments.PickDice:
                break;
            case SetMoments.RollDice:
                time = Time.time;
                break;
            case SetMoments.RevealDice:
                if (Time.time - time > 0.5f)
                {

                    SetGlowing(FindFirstObjectByType<dice>().value);
                }
                break;
            case SetMoments.GlowRock:
                if (figtherTurn != 0)
                {
                    figthers[figtherTurn].transform.GetComponent<BotPlayer>().ThinkingRocks();
                }
                break;
            case SetMoments.MoveToRock:
                break;
            case SetMoments.SelecCombat:

                break;
            case SetMoments.PickCard:
                all = true;
                foreach (Figther player in figthers)

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
            case SetMoments.Result:
                if (Time.time - time > 5f)
                {
                    if (figthers[0].GetPlayerLive() <= 0)
                    {
                        actualAction = SetMoments.End;
                        FindFirstObjectByType<EndGame>().EndGamer(false);
                    }
                    else
                    {
                        for (int i = 1; i < figthers.Length; i++)
                        {
                            if (figthers[i].GetPlayerLive() <= 0)
                            {
                                Figther elmi = figthers[i];
                                figthers = figthers.Where(f => f != elmi).ToArray();
                                elmi.playerToken.rocky.RemovePlayer(elmi.playerToken);
                                Destroy(elmi.gameObject);
                                manyFigthers--;
                            }

                        }
                        for (int i = 1; i < figthers.Length; i++)
                        {
                            figthers[i].indexFigther = i;
                        }
                        if (figthers.Length <= 1)
                        {
                            actualAction = SetMoments.End;
                            FindFirstObjectByType<EndGame>().EndGamer(true);
                        }
                        else
                        {
                            actualAction = SetMoments.Loop;
                        }

                    }

                }



                break;
            case SetMoments.Reveal:
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
                        if (figthers[i].GetTeam() == figthers[j].GetTeam())
                        {
                            results[i, j] = Results.draw;
                        }
                        else
                        {
                            results[i, j] = IndvCombat(card[i], card[j]);
                        }


                    }

                }

                int[] destiny = new int[manyFigthers];
                for (int i = 0; i < manyFigthers; i++)
                {
                    destiny[i] = 0;
                    for (int j = 0; j < manyFigthers; j++)
                    {
                        int resulta = (int)results[i, j] - 1;
                        destiny[i] += resulta;
                    }
                    //print(destiny[i]);
                    figthers[i].addPlayerLive(destiny[i]);
                }


                time = Time.time;
                actualAction = SetMoments.Result;
                break;
            case SetMoments.Loop:
                figtherTurn = (figtherTurn + 1) % manyFigthers;
                for (int i = 0; i < manyFigthers; i++)
                {
                    figthers[i].RefillHand();
                    figthers[i].ThrowCard();
                }

                playersFigthing = 0;
                if (figtherTurn == 0)
                {
                    actualAction = SetMoments.round;
                }
                else
                {
                    actualAction = SetMoments.PickDice;
                }

                break;
            case SetMoments.round:
                round++;
                FindFirstObjectByType<Roundanimation>().startRound();
                actualAction = SetMoments.rounded;
                break;


            case SetMoments.End:

                break;

            default:
                break;
        }
    }

    public Results IndvCombat(Card one, Card two)
    {
        if (one == null || two == null)
        {
            return Results.draw;
        }

        //print(combatType);
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
            if (turn() != 0)
            {
                actualAction = SetMoments.PickCard;
                combatType = (CombatType)UnityEngine.Random.Range(0, 4);
            }
            else
            {
                actualAction = SetMoments.SelecCombat;
            }

        }
        else
        {
            actualAction = SetMoments.PickCard;
            print(rocky.inscription);
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

    public void endRoundeded()
    {
        if (actualAction == SetMoments.rounded)
        {
            actualAction = SetMoments.PickDice;
        }
    }

    public void StartRoling()
    {
        actualAction = SetMoments.RollDice;
    }

    public void Roled()
    {
        if (actualAction == SetMoments.RollDice)
            actualAction = SetMoments.RevealDice;
    }



    public void SetGlowing(int value)
    {
        RockBehavior lander = figthers[figtherTurn].playerToken.rocky;
        RockBehavior[] rocker = lander.getNeighbor(value);
        rocker[0].shiny = true;
        rocker[1].shiny = true;
        actualAction = SetMoments.GlowRock;
        if (figtherTurn != 0)
        {
            figthers[figtherTurn].transform.GetComponent<BotPlayer>().PickRock(rocker);
        }
    }

    public int turn()
    {
        return figtherTurn;
    }
    public bool hurtPlayer()
    {
        return figthers[0].noHurt;
    }

    public void Surrender()
    {
        actualAction = SetMoments.End;
        FindFirstObjectByType<EndGame>().EndGamer(false);

    }
}
