using System;
using UnityEngine;
using System.Linq;

// Lista de elementos
public enum Element
{
    fire, earth, water, air
}

// Lista de momentos de la batalla
// TODO: Corregir si esta lista de estados está mal comentada
// TODO: Pasar este enum a camelCase
public enum SetMoments
{
    PickDice, RollDice, RevealDice, // Elegir dado | Tirar dado | Revelar valor del dado
    GlowRock, MoveToRock, SelecCombat, // Resaltar las rocas disponibles | Moverse a la roca elegida | Seleccionar tipo de combate
    PickCard, Reveal, Result, // Elegir carta | Revelar cartas | Mostrar resultados
    End, Loop, round, rounded // Terminar | Bucle | Muestra ronda | Fin de ronda
}

// Lista de resultados de la batalla
public enum Results
{
    lose, draw, win
}

// Lista de tipos de combate (elementos)
public enum CombatType
{
    fire, earth, water, air, full
}

// TODO: Renombrar esta clase en formato PascalCase
// TODO: Averiguar el uso de todas las variables y documentarlo bien

[DefaultExecutionOrder(-1)]
public class Combatjudge : MonoBehaviour
{
    // Variables
    [Header("Players")]
    [SerializeField] GameObject player; // Jugador principal
    [SerializeField] GameObject bots; // Bots
    [SerializeField] int manyFigthers; // Cantidad de jugadores
    public int round { get; private set; } // Getter público de la ronda actual
    Figther[] figthers; // Array de jugadores y bots

    [Header("GameRules")]
    [SerializeField] SetMoments actualAction; // Estado actual del juego
    [SerializeField] public int initialLives; // Cantidad de vidas iniciales de cada jugador
    [SerializeField] public int maxDice; // Valor máximo del dado (6)
    [SerializeField] int figtherTurn; // Turno actual
    [SerializeField] int damageDealt; // Cantidad de daño por ataque
    [SerializeField] int damageHeal; // Cantidad de curación por atacar

    public CombatType combatType { get; private set; } // Getter público del tipo de combate
    public static Combatjudge combatjudge; // 
    private int playersFigthing; // Cantidad de jugadores que están en juego
    private int manyFigthersFigthing; // TODO: Eliminar esta variable, no esta siendo leída en ningún lado
    private bool all; // TODO: Averiguar que hace esta variable

    void Start()
    {
        // Verifica que la instancia de Combatjudge no esté creada
        if (combatjudge == null)
        {
            combatjudge = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Inicializa la cantidad de jugadores, bots turno, ronda y jugadores humanos
        manyFigthers = UnityEngine.Random.Range(2, 5); // Entre dos y cuatro jugadores
        actualAction = SetMoments.Loop; // Estado inicial del juego: loop
        figtherTurn = -1; // Primer turno (o turno indefinido)
        round = 0; // Primera ronda

        // Obtiene los jugadores humanos
        Figther[] players = FindObjectsByType<Figther>(FindObjectsSortMode.InstanceID);

        // TODO: Implementa o elimina esto
        // // Ordena a los jugadores usando el dígito inicial de su nombre de objeto (mejorado, opcional)
        // // Usa LINQ para consultar, extraer y ordenar los componentes por su nombre, con una función nativa de C#
        // players = players.OrderBy(p => int.Parse(p.name[0].ToString())).ToArray();

        // Ordena los jugadores usando el dígito inicial de su nombre de objeto
        int a = 0;
        while (a < players.Length)
        {
            // Extrae el primer caracter del nombre de los jugadores y lo pasa a entero
            // Actúa como indicador de su posición en la lista de jugadores
            int numero = int.Parse(players[a].name[0].ToString());

            // Almacena al jugador en la posición correcta de la lista, haciendo un intercambio hacia su sitio
            (players[a], players[numero]) = (players[numero], players[a]);

            // Si a es igual a numero quiere decir que el jugador ya estaba bien colocado. Entonces pasa al siguiente jugador
            if (a == numero) a++;
        }

        // Recorta la lista al tamaño real de jugadores activos
        // Itera desde el primer jugador sobrante hasta el final y los destruye.
        for (int i = manyFigthers; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }

        // Inicializa el círculo de rocas de la arena y la UI
        PlayZone zone = FindFirstObjectByType<PlayZone>();
        Canvas canvas = FindFirstObjectByType<Canvas>();

        // Calcula el espaciado en rocas entre los jugadores
        int div = zone.many / manyFigthers;

        // Crea el array que almacenará a los jugadores
        figthers = new Figther[manyFigthers];

        for (int i = 0; i < manyFigthers; i++)
        {
            // El primer jugador es humano, el resto son bots
            GameObject figther = i == 0 ? player : bots;

            // Reutiliza los luchadores que ya están en la escena verificando si estamos dentro de la lista de jugadores ya existentes
            // Si no hay suficientes, crea nuevos a partir de los prefabs. Si los hay, entonces asigna los existentes
            if (i >= players.Length)
            {
                // Crea la instancia nueva de jugador, su espacio en la UI y una especie aleatoria
                figthers[i] = Instantiate(figther).GetComponent<Figther>();
                figthers[i].transform.SetParent(canvas.transform, false);
                figthers[i].randomSpecie();
            }
            else
            {
                // Asigna al jugador existente
                figthers[i] = players[i];
            }

            // Asigna el jugador y skin a cada jugador
            // Si es el primero, lo asigna como jugador humano. Si no, lo hará como bot
            if (i == 0)
            {
                // Asigna el equipo y skin elegidos por el jugador (estan en el GameState)
                figthers[i].setTeam(GameState.singleton.team);
                figthers[i].setSkin(GameState.singleton.skin);
            }
            else
            {
                // Si hay dos jugadores, simplemente asignamos un equipo diferente al del humano, con setNoTeam
                if (manyFigthers == 2)
                {
                    figthers[i].setNoTeam(GameState.singleton.team);
                }
                // Si hay tres jugadores y estamos con el último bot, verificamos que el humano y el otro bot tengan el mismo equipo
                // Si es así, ponemos un equipo diferente. Si no, lo asignamos al azar
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
                // Si hay cuatro jugadores y estamos con el último bot, verificamos que el humano tengan el mismo equipo que alguno de los otros dos bots
                // Si es así, ponemos un equipo diferente. Si no, lo asignamos al azar
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
                // Si no es ningún caso, es poco práctico seguir la misma lógica de antes
                // Simplemente asignamos al azar los equipos de los bots
                else
                {
                    figthers[i].FreeTeam();
                }

                // Damos una skin aleatoria al bot
                figthers[i].setRSkin();

                // TODO: Implementar o eliminar esto
                // // Le damos un equipo diferente al del humano al bot
                // figthers[i].setNoTeam(figthers[0].GetTeam());
            }

            // Asignamos al jugador sus valores iniciales
            figthers[i].setPlayerLive(initialLives); // Vida inicial
            figthers[i].visualFigther = i + 1; // Identificador de jugador
            figthers[i].indexFigther = i; // Identificador de jugador (en el arreglo)

            // Asignamos al jugador su nombre
            // Si es humano (el primero) usa el nombre desde GameState. Si es bot, le pone un identificador
            figthers[i].figtherName = i == 0 ? GameState.singleton.playerName : $"O{i}O";

            // Instancia a la roca usando el espaciado (div) y se la asigna al jugador como su punto de inicio
            RockBehavior rocky = zone.transform.GetChild(i * div).GetComponent<RockBehavior>();
            figthers[i].initialStone = rocky;
        }
    }

    float time;

    // TODO: Vas aquí
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
                if (Time.time - time > 0.5f) SetGlowing(FindFirstObjectByType<dice>().value);
                break;
            case SetMoments.GlowRock:
                if (figtherTurn != 0) figthers[figtherTurn].transform.GetComponent<BotPlayer>().ThinkingRocks();
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

                if (all) actualAction = SetMoments.Reveal;
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
        if (one == null || two == null) return Results.draw;

        //print(combatType);
        if ((combatType != CombatType.full) && (one.GetElement() != two.GetElement())) return (int)one.GetElement() == (int)combatType ? Results.win : Results.lose;

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
        if (actualAction == SetMoments.rounded) actualAction = SetMoments.PickDice;
    }

    public void StartRoling()
    {
        actualAction = SetMoments.RollDice;
    }

    public void Roled()
    {
        if (actualAction == SetMoments.RollDice) actualAction = SetMoments.RevealDice;
    }

    public void SetGlowing(int value)
    {
        RockBehavior lander = figthers[figtherTurn].playerToken.rocky;
        RockBehavior[] rocker = lander.getNeighbor(value);
        rocker[0].shiny = true;
        rocker[1].shiny = true;
        actualAction = SetMoments.GlowRock;

        if (figtherTurn != 0) figthers[figtherTurn].transform.GetComponent<BotPlayer>().PickRock(rocker);
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
