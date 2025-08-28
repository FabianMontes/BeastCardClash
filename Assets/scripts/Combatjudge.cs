using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// Lista de elementos 
/// </summary>
public enum Element
{
    Fire,
    Earth,
    Water,
    Air
}

// TODO: Corregir si esta lista de estados está mal comentada
/// <summary>
/// Lista de momentos de la batalla
/// </summary>
public enum SetMoments
{
    PickDice, // Elegir dado
    RollDice, // Tirar dado
    RevealDice, // Revelar valor del dado
    GlowRock, // Resaltar las rocas disponibles
    MoveToRock, // Moverse a la roca elegida
    SelectCombat, // Seleccionar tipo de combate
    PickCard, // Elegir carta
    Reveal, // Revelar carta
    Result, // Mostrar resultados
    End, // Finalizar partida
    Loop, // Reiniciar para la siguiente ronda
    Round, // Muestra la ronda
    Rounded // Fin de ronda
}

/// <summary>
/// Lista de resultados de la batalla 
/// </summary>
public enum Results
{
    Lose,
    Draw,
    Win
}

/// <summary>
/// Lista de tipos de combate (elementos)
/// </summary>
public enum CombatType
{
    Fire,
    Earth,
    Water,
    Air,
    Full
}

// TODO: Renombrar esta clase en formato PascalCase

/// <summary>
/// CombatJudge se encarga de gestionar las batallas de cartas, los jugadores y los resultados
/// </summary>
[DefaultExecutionOrder(-1)]
public class Combatjudge : MonoBehaviour
{
    public int round { get; private set; } // Getter público de la ronda actual
    
    // Variables
    [Header("Players")] [SerializeField] GameObject player; // Jugador principal
    [SerializeField] GameObject bots; // Bots
    [SerializeField] int manyFigthers; // Cantidad de jugadores
    Figther[] figthers; // Array de jugadores y bots

    [Header("GameRules")] [SerializeField] SetMoments actualAction; // Estado actual del juego
    [SerializeField] public int initialLives; // Cantidad de vidas iniciales de cada jugador
    [SerializeField] public int maxDice; // Valor máximo del dado (6)
    [SerializeField] int figtherTurn; // Turno actual
    [SerializeField] int damageDealt; // Cantidad de daño por ataque
    [SerializeField] int damageHeal; // Cantidad de curación por atacar

    public CombatType combatType { get; private set; } // Getter público del tipo de combate
    public static Combatjudge combatjudge; // 
    private int playersFigthing; // Cantidad de jugadores que están en juego
    private bool all; // Indica si todos los jugadores han elegido du carta

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

            // Si "a" es igual a "numero" quiere decir que el jugador ya estaba bien colocado. Entonces pasa al siguiente jugador
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
                    if (figthers[0].GetTeam() == figthers[1].GetTeam() &&
                        figthers[0].GetTeam() == figthers[2].GetTeam())
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

    // Variable para controlar el tiempo en diferentes estados del juego
    private float _time;

    void Update()
    {
        // Cambia de acción dependiendo del estado actual
        switch (actualAction)
        {
            // PickDice: sigue sin hacer nada
            case SetMoments.PickDice:
                break;
            // RollDice: actualiza la hora para los contadores
            case SetMoments.RollDice:
                _time = Time.time;
                break;
            // RevealDice: espera medio segundo, y si se cumple, resalta las rocas a las que puede moverse
            case SetMoments.RevealDice:
                if (Time.time - _time > 0.5f) SetGlowing(FindFirstObjectByType<dice>().value);
                break;
            // GlowRock: si es el turno del bot, llama a ThinkingRocks() para elegir roca
            case SetMoments.GlowRock:
                if (figtherTurn != 0) figthers[figtherTurn].transform.GetComponent<BotPlayer>().ThinkingRocks();
                break;
            // MoveToRock: sigue sin hacer nada
            case SetMoments.MoveToRock:
                break;
            // SelectCombat: sigue sin hacer nada
            case SetMoments.SelectCombat:
                break;
            // PickCard: elige la carta y la revela en cuanto todos los jugadores han elegido
            case SetMoments.PickCard:
                // Asume que todos han elegido carta
                all = true;

                // Itera sobre todos los jugadores
                foreach (Figther fighter in figthers)
                {
                    // Si un jugador ha elegido o no esta en batalla, lo ignora
                    if (fighter.getPicked() != null || !fighter.IsFigthing()) continue;

                    // De lo contrario, indica que aún falta alguno
                    all = false;
                    break;
                }

                // Si todos han elegido como lo sugiere "all", pasa a revelar las cartas
                if (all) actualAction = SetMoments.Reveal;
                break;
            // Result: espera 5 segundos y elimina a los jugadores eliminados
            // También actúa en caso de ganar, perder, continuar, etc.
            case SetMoments.Result:
                // Espera 5 segundos
                if (Time.time - _time > 5f)
                {
                    // Si el jugador humano pierde, termina el juego y llama a EndGamer() para eso
                    // De lo contrario hace otra cosa para continuar, según corresponda
                    if (figthers[0].GetPlayerLive() <= 0)
                    {
                        actualAction = SetMoments.End;
                        FindFirstObjectByType<EndGame>().EndGamer(false);
                    }
                    else
                    {
                        // Elimina a los jugadores que han perdido
                        for (int i = 1; i < figthers.Length; i++)
                        {
                            // Los que tengan vida se ignoran
                            if (figthers[i].GetPlayerLive() > 0) continue;

                            // Los que no, se eliminan
                            Figther deletedFigther = figthers[i]; // Indica que el jugador actual y su índice se elimina
                            figthers = figthers.Where(f => f != deletedFigther)
                                .ToArray(); // Busca a todos los jugadores que no sean el eliminado
                            deletedFigther.playerToken.rocky.RemovePlayer(deletedFigther
                                .playerToken); // Remueve al jugador
                            Destroy(deletedFigther.gameObject); // Destruye el GameObject asociado
                            manyFigthers--; // Le resta uno a los jugadores totales
                        }

                        // Corrige los índices después de eliminar al jugador, si es necesario
                        for (int i = 1; i < figthers.Length; i++)
                        {
                            figthers[i].indexFigther = i;
                        }

                        // Si hay un jugador o menos, salta al final de la partida. De lo contrario pasa a loop, para continuar
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
            // Reveal: revela las cartas y procesa los resultados de ellas
            case SetMoments.Reveal:
                // Almacena las cartas elegidas por todos
                Card[] card = new Card[manyFigthers];
                int a = 0;

                // Guarda las cartas de todos los jugadores en el arreglo
                foreach (Figther fighter in figthers)
                {
                    card[a] = fighter.getPicked();
                    a++;
                }

                // Calcula los resultados de todos los jugadores en forma de matriz. Así detecta todos los ataques y su influencia en el puntaje
                Results[,] results = new Results[manyFigthers, manyFigthers];
                for (int i = 0; i < manyFigthers; i++)
                {
                    for (int j = 0; j < manyFigthers; j++)
                    {
                        results[i, j] = figthers[i].GetTeam() == figthers[j].GetTeam()
                            ? Results.Draw
                            : IndividualCombat(card[i], card[j]);
                    }
                }

                // Calcula el daño de los jugadores con base en los resultados
                // Arreglo que almacena el daño de cada jugador
                int[] destiny = new int[manyFigthers];
                for (int i = 0; i < manyFigthers; i++)
                {
                    // Inicia el daño en 0, luego empieza a revisar sus rivales
                    destiny[i] = 0;
                    for (int j = 0; j < manyFigthers; j++)
                    {
                        // Si hay daño, lo resta
                        int result = (int)results[i, j] - 1;
                        destiny[i] += result;
                    }

                    // print(destiny[i]);
                    // Le suma el daño. Como es negativo, le disminuye efectivamente. Si no necesita daño, se le suma 0
                    figthers[i].addPlayerLive(destiny[i]);
                }

                // Actualiza el contador y el estado actual del juego
                _time = Time.time;
                actualAction = SetMoments.Result;
                break;
            // Loop: resetea los componentes para la siguiente ronda
            case SetMoments.Loop:
                // Calcula el turno actual
                figtherTurn = (figtherTurn + 1) % manyFigthers;

                // Rellena y lanza las cartas de todos los jugadores
                for (int i = 0; i < manyFigthers; i++)
                {
                    figthers[i].RefillHand();
                    figthers[i].ThrowCard();
                }

                // Establece el turno actual y el estado del juego
                playersFigthing = 0;
                actualAction = figtherTurn == 0 ? SetMoments.Round : SetMoments.PickDice;
                break;
            // Round: inicia la ronda
            case SetMoments.Round:
                // Incrementa el número de ronda
                round++;

                // Acciona la nueva ronda con Roundanimation y establece el siguiente estado
                // TODO: Cambia esta parte para no usar FindFirstObjectByType cada vez
                // TODO: Corrige el nombre de esta clase
                FindFirstObjectByType<Roundanimation>().startRound();
                actualAction = SetMoments.Rounded;
                break;
            // End: termina el juego
            case SetMoments.End:
                break;
        }
    }
    
    /// <summary>
    /// Calcula los pares de cartas en la batalla, quien gana y lo que pasa
    /// </summary>
    /// <param name="one">Carta uno</param>
    /// <param name="two">Carta dos</param>
    /// <returns>Resultado del duelo: ganar, perder o empatar</returns>
    private Results IndividualCombat(Card one, Card two)
    {
        // Si alguna de las cartas no existe, es empate
        if (one == null || two == null) return Results.Draw;

        // Si el tipo de combate no es "Full" (es decir, es un tipo de elemento específico) y los elementos de las cartas son diferentes
        // El resultado se determina por si el elemento de la primera carta coincide con el tipo de combate.
        if (combatType != CombatType.Full && one.GetElement() != two.GetElement())
            return (int)one.GetElement() == (int)combatType ? Results.Win : Results.Lose;

        // Cantidad de elementos, su mitad y la diferencia de elementos
        int countElements = Enum.GetValues(typeof(Element)).Length;
        int halfElements = countElements / 2;
        int elementDiff = (one.GetElement() - two.GetElement() + countElements) % countElements;

        // Si la cantidad de elementos es par
        if (countElements % 2 == 0)
        {
            // Si los elementos no son ni diferentes in opuestos, evalúa quien gana usando la diferencia de elementos
            if (elementDiff != 0 && elementDiff != halfElements)
                return elementDiff > halfElements ? Results.Win : Results.Lose;

            // Si la carta uno es mayor, gana
            if (one.GetValue() > two.GetValue()) return Results.Win;

            // Si la carta uno es menor, pierde, si no, empate
            return one.GetValue() < two.GetValue() ? Results.Lose : Results.Draw;
        }

        // Si los elementos son diferentes, evalúa quien gana usando la diferencia de elementos
        if (elementDiff != 0) return elementDiff > halfElements ? Results.Win : Results.Lose;

        // Si la carta uno es mayor, gana
        if (one.GetValue() > two.GetValue()) return Results.Win;

        // Si la carta uno es menor, pierde, si no, empate
        return one.GetValue() < two.GetValue() ? Results.Lose : Results.Draw;
    }

    /// <summary> 
    /// Obtiene el estado actual del juego
    /// </summary>
    /// <returns>Estado actual del juego, del enum SetMoments</returns>
    public SetMoments GetSetMoments()
    {
        return actualAction;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ArriveAtRock()
    {
        RockBehavior rocky = figthers[figtherTurn].playerToken.rocky;
        if (rocky.manyOn())
        {
            playersFigthing = rocky.GetPlayersOn();
            rocky.ManyPlayerOn();
        }
        else
        {
            playersFigthing = (int)Mathf.Pow(2, manyFigthers) - 1;
        }

        if (rocky.inscription == Inscription.pick)
        {
            if (Turn() != 0)
            {
                actualAction = SetMoments.PickCard;
                combatType = (CombatType)UnityEngine.Random.Range(0, 4);
            }
            else
            {
                actualAction = SetMoments.SelectCombat;
            }
        }
        else
        {
            actualAction = SetMoments.PickCard;
            print(rocky.inscription);
            combatType = (CombatType)(int)rocky.inscription;
        }
    }

    /// <summary>
    /// Mueve al jugador a la roca elegida, y establece el estado del juego
    /// </summary>
    /// <param name="rocker"></param>
    public void MoveToRock(RockBehavior rocker)
    {
        figthers[figtherTurn].playerToken.rocky = rocker;
        actualAction = SetMoments.MoveToRock;
    }

    /// <summary>
    /// Determina cuando elegir un elemento (en las rocas que lo permiten) y lo hace
    /// </summary>
    /// <param name="element">Elemento a elegir</param>
    /// <returns>true si elegimos el elemento, false si no es así</returns>
    public bool PickElement(Element element)
    {
        // Si no estamos en combate, devuelve falso
        if (SetMoments.SelectCombat != actualAction) return false;

        // Intenta escoger el elemento. Si marca error, lo indica y devuelve falso
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

        // Si funciona, devuelve verdadero
        return true;
    }

    /// <summary>
    /// Obtiene la cantidad de jugadores en juego 
    /// </summary>
    /// <returns>Cantidad de jugadores activos</returns>
    public int GetPlayersFighting()
    {
        return playersFigthing;
    }
    
    
    /// <summary>
    /// Obtiene si el turno actual coincide con el jugador activo
    /// </summary>
    /// <returns>true si estamos en nuestro turno</returns>
    public bool FocusOnTurn()
    {
        return figthers[figtherTurn].visualFigther == 1;
    }

    /// <summary>
    /// Evalúa si acabo la ronda, si es así pasamos a escoger dado
    /// </summary>
    public void EndRounded()
    {
        if (actualAction == SetMoments.Rounded) actualAction = SetMoments.PickDice;
    }

    /// <summary>
    /// Acciona RollDice para iniciar el lanzamiento del dado
    /// </summary>
    public void StartRolling()
    {
        actualAction = SetMoments.RollDice;
    }

    /// <summary>
    /// Si ya lanzamos el dado, pasamos a mostrarlo
    /// </summary>
    public void Rolled()
    {
        if (actualAction == SetMoments.RollDice) actualAction = SetMoments.RevealDice;
    }

    /// <summary>
    /// Resalta las rocas a moverse
    /// </summary>
    /// <param name="value"></param>
    private void SetGlowing(int value)
    {
        RockBehavior lander = figthers[figtherTurn].playerToken.rocky;
        RockBehavior[] rocker = lander.getNeighbor(value);
        rocker[0].shiny = true;
        rocker[1].shiny = true;
        actualAction = SetMoments.GlowRock;

        if (figtherTurn != 0) figthers[figtherTurn].transform.GetComponent<BotPlayer>().PickRock(rocker);
    }

    /// <summary>
    /// Obtiene el turno actual
    /// </summary>
    /// <returns>El turno actual</returns>
    public int Turn()
    {
        return figtherTurn;
    }

    /// <summary>
    /// Determina si un jugador recibió daño
    /// </summary>
    /// <returns>true si el jugador recibió daño</returns>
    public bool HurtPlayer()
    {
        return figthers[0].noHurt;
    }

    /// <summary>
    /// Hace que se rinda el jugador, saliendo de la partida
    /// </summary>
    public void Surrender()
    {
        actualAction = SetMoments.End;
        FindFirstObjectByType<EndGame>().EndGamer(false);
    }
}