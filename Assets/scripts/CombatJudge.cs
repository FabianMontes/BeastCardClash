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
/// Lista de tipos de combate (elementos) en las rocas de la zona de batalla
/// </summary>
public enum CombatType
{
    Fire,
    Earth,
    Water,
    Air,
    Full // Full es para las rocas que permiten elegir elemento
}

/// <summary>
/// CombatJudge se encarga de gestionar las batallas de cartas, los jugadores y los resultados
/// </summary>
[DefaultExecutionOrder(-1)]
public class CombatJudge : MonoBehaviour
{
    // Getters e instancias
    public int Round { get; private set; } // Getter público de la ronda actual
    public CombatType CombatType { get; private set; } // Getter público del tipo de combate
    public static CombatJudge CombatJudgeInstance; // Instancia pública de CombatJudge

    // Variables de jugador
    [Header("Players")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject bots;
    [SerializeField] int manyFighters; // Cantidad de jugadores
    Figther[] _fighters; // Array de jugadores y bots

    // Variables de juego
    [Header("GameRules")]
    [SerializeField] SetMoments actualAction; // Estado actual del juego
    [SerializeField] public int initialLives; // Cantidad de vidas iniciales de cada jugador
    [SerializeField] public int maxDice; // Valor máximo del dado (6)
    [SerializeField] int fighterTurn; // Turno actual
    [SerializeField] int damageDealt; // Cantidad de daño por ataque
    [SerializeField] int damageHeal; // Cantidad de curación por atacar

    // Otras variables
    int _playersFighting; // Máscara de bits que representa a los jugadores en el combate actual
    bool _allPlayersChose; // Indica si todos los jugadores han elegido su carta

    void Start()
    {
        if (CombatJudgeInstance == null)
        {
            CombatJudgeInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Inicializa la cantidad de jugadores, bots turno, ronda y jugadores humanos
        manyFighters = UnityEngine.Random.Range(2, 5); // Entre dos y cuatro jugadores
        actualAction = SetMoments.Loop; // Loop es el inicio de ronda
        fighterTurn = -1; // -1 fuerza a que el jugador sea el que comience
        Round = 0; // Primera ronda (vale 0 para cuando se ejecute Round++ más tarde)

        // Obtiene todos los jugadores
        Figther[] players = FindObjectsByType<Figther>(FindObjectsSortMode.InstanceID);

        // Ordena los jugadores usando el dígito inicial de su nombre de objeto
        int a = 0;
        while (a < players.Length)
        {
            // Extrae el primer carácter del nombre de los jugadores y lo pasa a entero para saber su orden
            int numero = int.Parse(players[a].name[0].ToString());

            // Almacena al jugador en la posición correcta de la lista, haciendo un intercambio hacia su sitio
            (players[a], players[numero]) = (players[numero], players[a]);

            // Si "a" es igual a "número" quiere decir que el jugador ya estaba bien colocado. Entonces pasa al siguiente jugador
            if (a == numero) a++;
        }

        // Recorta la lista al tamaño real de jugadores activos. Itera desde el primer jugador sobrante hasta el final y los destruye.
        for (int i = manyFighters; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }

        // Inicializa el círculo de rocas de la arena y la UI
        PlayZone zone = FindFirstObjectByType<PlayZone>();
        Canvas canvas = FindFirstObjectByType<Canvas>();

        // Calcula el espaciado en rocas entre los jugadores
        int div = zone.many / manyFighters;

        // Crea el array que almacenará a los jugadores y luego lo llena
        _fighters = new Figther[manyFighters];

        for (int i = 0; i < manyFighters; i++)
        {
            // El primer jugador es humano, el resto son bots
            GameObject fighter = i == 0 ? player : bots;

            // Reutiliza los luchadores que ya están en la escena verificando si estamos dentro de la lista de jugadores ya existentes
            // Si no hay suficientes, crea nuevos a partir de los prefabs. Si los hay, entonces asigna los existentes
            if (i < players.Length)
            {
                // Asigna al jugador existente
                _fighters[i] = players[i];
            }
            else
            {
                // Crea la instancia nueva de jugador, su espacio en la UI y una especie aleatoria
                _fighters[i] = Instantiate(fighter).GetComponent<Figther>();
                _fighters[i].transform.SetParent(canvas.transform, false);
                _fighters[i].randomSpecie();
            }

            // Asigna el jugador y skin a cada jugador. Si es el primero, lo asigna como humano. Si no, lo hará como bot
            if (i == 0)
            {
                // Asigna el equipo y skin elegidos por el jugador (están en el GameState)
                _fighters[i].setTeam(GameState.singleton.team);
                _fighters[i].setSkin(GameState.singleton.skin);
            }
            else
            {
                // Si hay dos jugadores, simplemente asignamos un equipo diferente al del humano, con setNoTeam
                if (manyFighters == 2)
                {
                    _fighters[i].setNoTeam(GameState.singleton.team);
                }
                // Si hay tres jugadores y estamos con el último, verificamos que el humano y el otro bot tengan el mismo equipo
                // Si es así, ponemos un equipo diferente. Si no, lo asignamos al azar
                else if (manyFighters == 3 && i == 2)
                {
                    if (_fighters[0].GetTeam() == _fighters[1].GetTeam())
                    {
                        _fighters[i].setNoTeam(GameState.singleton.team);
                    }
                    else
                    {
                        _fighters[i].FreeTeam();
                    }
                }
                // Si hay cuatro jugadores y estamos con el último, verificamos que los tres primeros jugadores comparten equipo
                // Si es así, ponemos un equipo diferente. Si no, lo asignamos al azar
                else if (manyFighters == 4 && i == 3)
                {
                    if (_fighters[0].GetTeam() == _fighters[1].GetTeam() && _fighters[0].GetTeam() == _fighters[2].GetTeam())
                    {
                        _fighters[i].setNoTeam(GameState.singleton.team);
                    }
                    else
                    {
                        _fighters[i].FreeTeam();
                    }
                }
                // Si no es ningún caso, asignamos al azar los equipos de los bots
                else
                {
                    _fighters[i].FreeTeam();
                }

                // Damos una skin aleatoria al bot
                _fighters[i].setRSkin();
            }

            // Asignamos al jugador sus valores iniciales
            _fighters[i].setPlayerLive(initialLives); // Vida inicial
            _fighters[i].visualFigther = i + 1; // Identificador de jugador
            _fighters[i].indexFigther = i; // Identificador de jugador (en el arreglo)

            // Asignamos al jugador su nombre. Si es humano, usa el nombre desde GameState. Si no, le pone un identificador
            _fighters[i].figtherName = i == 0 ? GameState.singleton.playerName : $"O{i}O";

            // Referencia a la roca usando el espaciado (div) y se la asigna al jugador como su punto de inicio
            RockBehavior rocky = zone.transform.GetChild(i * div).GetComponent<RockBehavior>();
            _fighters[i].initialStone = rocky;
        }
    }

    // Variable para controlar el tiempo en diferentes estados del juego
    float _time;

    void Update()
    {
        // Cambia de acción dependiendo del estado actual
        switch (actualAction)
        {
            case SetMoments.PickDice:
                break;
            // RollDice: actualiza la hora para los contadores
            case SetMoments.RollDice:
                _time = Time.time;
                break;
            // RevealDice: espera medio segundo, y si se cumple, resalta las rocas a las que puede moverse
            case SetMoments.RevealDice:
                if (Time.time - _time > 0.5f) SetGlowing(FindFirstObjectByType<Dice>().Value);
                break;
            // GlowRock: si es el turno del bot, llama a ThinkingRocks() para elegir roca
            case SetMoments.GlowRock:
                if (fighterTurn != 0) _fighters[fighterTurn].transform.GetComponent<BotPlayer>().ThinkingRocks();
                break;
            case SetMoments.MoveToRock:
            case SetMoments.SelectCombat:
                break;
            // PickCard: comprueba que todos han elegido carta y las revela si es así
            case SetMoments.PickCard:
                // Asume que todos han elegido carta
                _allPlayersChose = true;

                // Itera sobre todos los jugadores
                // Si un jugador ha elegido o no está en batalla, lo ignora. De lo contrario, indica que aún falta alguno
                foreach (Figther fighter in _fighters)
                {
                    if (fighter.getPicked() != null || !fighter.IsFigthing()) continue;
                    _allPlayersChose = false;
                    break;
                }

                // Si todos han elegido como lo sugiere _allPlayersChose, pasa a revelar las cartas
                if (_allPlayersChose) actualAction = SetMoments.Reveal;
                break;
            // Result: espera 5 segundos y elimina a los jugadores eliminados. También actúa en caso de ganar, perder, continuar, etc.
            case SetMoments.Result:
                if (Time.time - _time > 5f)
                {
                    // Si el jugador humano pierde, termina el juego y llama a EndGamer() para eso.
                    // De lo contrario hace otra cosa para continuar, según corresponda
                    if (_fighters[0].GetPlayerLive() <= 0)
                    {
                        actualAction = SetMoments.End;
                        FindFirstObjectByType<EndGame>().EndGamer(false);
                    }
                    else
                    {
                        // Elimina a los jugadores que han perdido
                        for (int i = 1; i < _fighters.Length; i++)
                        {
                            // Los que tengan vida se ignoran
                            if (_fighters[i].GetPlayerLive() > 0) continue;

                            // Los que no, se eliminan
                            Figther deletedFighter = _fighters[i]; // Referencia al jugador a eliminar
                            _fighters = _fighters.Where(f => f != deletedFighter).ToArray(); // Busca y actualiza a todos los jugadores que no sean el eliminado
                            deletedFighter.playerToken.rocky.RemovePlayer(deletedFighter.playerToken); // Remueve al jugador
                            Destroy(deletedFighter.gameObject); // Destruye el GameObject asociado
                            manyFighters--; // Le resta uno al conteo de jugadores
                        }

                        // Corrige los índices después de eliminar al jugador, si es necesario
                        for (int i = 1; i < _fighters.Length; i++)
                        {
                            _fighters[i].indexFigther = i;
                        }

                        // Si hay un jugador o menos, salta al final de la partida. De lo contrario pasa a loop, para continuar
                        if (_fighters.Length <= 1)
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
                // El arreglo almacena las cartas elegidas por todos. El bucle las guarda
                Card[] card = new Card[manyFighters];

                int a = 0;
                foreach (Figther fighter in _fighters)
                {
                    card[a] = fighter.getPicked();
                    a++;
                }

                // Calcula los resultados de todos los jugadores en forma de matriz. Así detecta todos los ataques y su influencia en el puntaje
                Results[][] results = new Results[manyFighters][];
                for (int index = 0; index < manyFighters; index++)
                {
                    results[index] = new Results[manyFighters];
                }

                for (int i = 0; i < manyFighters; i++)
                {
                    for (int j = 0; j < manyFighters; j++)
                    {
                        results[i][j] = _fighters[i].GetTeam() == _fighters[j].GetTeam()
                            ? Results.Draw
                            : IndividualCombat(card[i], card[j]);
                    }
                }

                // Calcula el daño de los jugadores con base en los resultados. Destiny almacena el daño de cada jugador
                int[] destiny = new int[manyFighters];
                for (int i = 0; i < manyFighters; i++)
                {
                    // Inicia el daño en 0, luego empieza a revisar sus rivales
                    destiny[i] = 0;
                    for (int j = 0; j < manyFighters; j++)
                    {
                        int result = (int)results[i][j] - 1;
                        destiny[i] += result;
                    }

                    // Le aplica el resultado del combate al jugador
                    _fighters[i].addPlayerLive(destiny[i]);
                }

                // Actualiza el contador y el estado actual del juego
                _time = Time.time;
                actualAction = SetMoments.Result;
                break;
            // Loop: resetea los componentes para la siguiente ronda
            case SetMoments.Loop:
                // Calcula el turno actual
                fighterTurn = (fighterTurn + 1) % manyFighters;

                // Rellena la mano y descarta las cartas de la ronda anterior para todos
                for (int i = 0; i < manyFighters; i++)
                {
                    _fighters[i].RefillHand();
                    _fighters[i].ThrowCard();
                }

                // Establece el turno actual y el estado del juego
                _playersFighting = 0;
                actualAction = fighterTurn == 0 ? SetMoments.Round : SetMoments.PickDice;
                break;
            // Round: inicia la nueva ronda
            case SetMoments.Round:
                Round++;

                // Acciona la nueva ronda con RoundAnimation y establece el siguiente estado
                // TODO: Cambia esta parte para no usar FindFirstObjectByType cada vez
                // TODO: Corrige el nombre de esta clase en el archivo RoundAnimation
                FindFirstObjectByType<Roundanimation>().startRound();
                actualAction = SetMoments.Rounded;
                break;
            case SetMoments.End:
                break;
        }
    }

    /// <summary>
    /// Determina el resultado de un combate individual entre dos cartas
    /// </summary>
    /// <param name="one">Carta uno</param>
    /// <param name="two">Carta dos</param>
    /// <returns>Resultado para la carta uno: ganar, perder o empatar</returns>
    Results IndividualCombat(Card one, Card two)
    {
        // Si alguna de las cartas no existe, es empate
        if (one == null || two == null) return Results.Draw;

        // Si el tipo de combate es de un elemento y los elementos de las cartas son diferentes
        // El resultado se determina por si el elemento de la primera carta coincide con el tipo de combate.
        if (CombatType != CombatType.Full && one.GetElement() != two.GetElement())
            return (int)one.GetElement() == (int)CombatType ? Results.Win : Results.Lose;

        // Cantidad de elementos, su mitad y la diferencia de elementos. Necesarios para calcular el resultado
        int countElements = Enum.GetValues(typeof(Element)).Length;
        int halfElements = countElements / 2;
        int elementDiff = (one.GetElement() - two.GetElement() + countElements) % countElements;

        // Si la cantidad de elementos es par
        if (countElements % 2 == 0)
        {
            // Si los elementos no son ni diferentes in opuestos, evalúa quien gana usando la diferencia
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
    /// Obtiene el estado actual del juego, del enum SetMoments
    /// </summary>
    public SetMoments GetSetMoments()
    {
        return actualAction;
    }

    /// <summary>
    /// Configura el juego para cuando un jugador terminó de moverse a la roca que eligió, sea elegir elemento o solo carta
    /// </summary>
    public void ArriveAtRock()
    {
        RockBehavior rocky = _fighters[fighterTurn].playerToken.rocky;
        
        // Si hay más de un jugador en la misma roca, averigua cuáles son y los almacena para una batalla cuerpo a cuerpo
        // Si solo hay uno, entonces es una batalla de todos contra todos
        if (rocky.manyOn())
        {
            _playersFighting = rocky.GetPlayersOn();
            rocky.ManyPlayerOn();
        }
        else
        {
            _playersFighting = (int)Mathf.Pow(2, manyFighters) - 1;
        }

        // Si la roca de destino permite elegir elemento, nos pone a elegir elemento y carta, si no, solo carta
        if (rocky.inscription == Inscription.pick)
        {
            // Solo podemos elegir en nuestro turno
            if (Turn() == 0)
            {
                actualAction = SetMoments.SelectCombat;
            }
            else
            {
                actualAction = SetMoments.PickCard;
                CombatType = (CombatType)UnityEngine.Random.Range(0, 4);
            }
        }
        else
        {
            actualAction = SetMoments.PickCard;
            
            // El ataque será el asignado a la roca
            CombatType = (CombatType)(int)rocky.inscription;
            print($"Ataque elegido: {rocky.inscription}");
        }
    }

    /// <summary>
    /// Mueve al jugador a la roca elegida, y establece el estado del juego en MoveToRock
    /// </summary>
    /// <param name="rocker"></param>
    public void MoveToRock(RockBehavior rocker)
    {
        _fighters[fighterTurn].playerToken.rocky = rocker;
        actualAction = SetMoments.MoveToRock;
    }

    /// <summary>
    /// Elige el elemento en las rocas que lo permiten (en las rocas que lo permiten)
    /// </summary>
    /// <param name="element">Elemento a elegir</param>
    /// <returns>True si elegimos el elemento, false si no es así</returns>
    public bool PickElement(Element element)
    {
        if (actualAction != SetMoments.SelectCombat) return false;

        // Intenta escoger el elemento. Si marca error, lo indica y devuelve falso. Si no, marca verdadero
        try
        {
            CombatType = (CombatType)(int)element;
            actualAction = SetMoments.PickCard;
        }
        catch (Exception e)
        {
            print(e);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Obtiene los jugadores en juego en forma de máscara de bits
    /// </summary>
    public int GetPlayersFighting()
    {
        return _playersFighting;
    }

    /// <summary>
    /// Obtiene si es nuestro turno
    /// </summary>
    public bool FocusOnTurn()
    {
        return _fighters[fighterTurn].visualFigther == 1;
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
    /// Resalta las rocas disponibles para moverse, según el valor del dado
    /// </summary>
    /// <param name="value">Cantidad de casillas a moverse, escogido por el dado</param>
    void SetGlowing(int value)
    {
        RockBehavior lander = _fighters[fighterTurn].playerToken.rocky;
        RockBehavior[] rocker = lander.getNeighbor(value);
        rocker[0].shiny = true;
        rocker[1].shiny = true;
        actualAction = SetMoments.GlowRock;

        if (fighterTurn != 0) _fighters[fighterTurn].transform.GetComponent<BotPlayer>().PickRock(rocker);
    }

    /// <summary>
    /// Obtiene el turno actual
    /// </summary>
    public int Turn()
    {
        return fighterTurn;
    }

    /// <summary>
    /// Indica si un jugador no recibió daño
    /// </summary>
    public bool HurtPlayer()
    {
        return _fighters[0].noHurt;
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
