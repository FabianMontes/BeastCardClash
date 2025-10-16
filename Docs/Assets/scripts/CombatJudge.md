# `CombatJudge`
`CombatJudge` es un componente `MonoBehaviour` central en el juego Beast Card Clash, responsable de la gestión y orquestación de las batallas de cartas, el flujo de juego turno a turno, la interacción entre jugadores (humanos y bots) y el cálculo de los resultados de los combates. Implementa el patrón Singleton a través de la propiedad `Instance` para asegurar que solo exista una instancia activa en la escena, permitiendo un acceso global fácil desde otros scripts. Su orden de ejecución (`[DefaultExecutionOrder(-1)]`) asegura que se inicialice y ejecute antes que la mayoría de los demás scripts en la escena, lo cual es fundamental para establecer el estado del juego y los jugadores desde el principio.

El script define una serie de estados de juego (`SetMoments`) que controlan el flujo de las rondas, desde la elección del dado hasta la resolución del combate y el reinicio para la siguiente ronda. También gestiona los tipos de combate (`CombatType`) basados en elementos, los resultados individuales (`Results`) de las batallas entre cartas y las propiedades de los elementos (`Element`). `CombatJudge` inicializa a los jugadores (`Fighter`), tanto humanos como bots, les asigna equipos, skins, vidas iniciales y su posición de inicio en el tablero, asegurando que estén listos para la partida.

A lo largo del juego, `CombatJudge` coordina las acciones de los jugadores, verifica si todos han elegido sus cartas, calcula el daño basado en los elementos y valores de las cartas, y gestiona la eliminación de jugadores que se quedan sin vidas. También se encarga de determinar cuándo termina el juego (victoria o derrota del jugador humano) y de preparar el estado para nuevas rondas.

# Métodos

## Métodos de Unity

### `Start()`
`Start()` es el método de inicialización principal para el `CombatJudge`. Se ejecuta una vez al inicio del juego, después de que todos los objetos han sido instanciados y se han inicializado las variables.

Su función principal es:
1.  **Establecer el Singleton**: Asegura que solo haya una instancia de `CombatJudge` en la escena. Si ya existe una, destruye el objeto actual.
    ```csharp
    if (Instance == null)
    {
        Instance = this;
    }
    else
    {
        Destroy(gameObject);
    }
    ```
2.  **Inicializar Variables de Juego**: Configura variables clave como `manyFighters` (número aleatorio de jugadores entre 2 y 4), el estado inicial del juego (`actualAction` a `SetMoments.Loop`), el `fighterTurn` a -1 (para que el jugador humano comience) y la `Round` a 0.
3.  **Localizar y Ordenar Jugadores Existentes**: Busca todos los componentes `Fighter` en la escena. Los ordena basándose en el primer dígito de su nombre de objeto (asumiendo que los nombres siguen un patrón numérico `0Jugador`, `1Bot`, etc.) para asegurar un orden consistente.
4.  **Eliminar Jugadores Excedentes**: Si hay más componentes `Fighter` en la escena de los que `manyFighters` indica, los destruye.
5.  **Obtener Componentes de Escena**: Encuentra instancias de `PlayZone` (la zona de juego o arena) y `Canvas` (la interfaz de usuario).
6.  **Instanciar o Asignar Jugadores**:
    *   Crea un array `_fighters` del tamaño de `manyFighters`.
    *   Itera para poblar el array: Si ya existen `Fighter` en la escena (basado en `players.Length`), los reutiliza. Si no, instancia nuevos `Fighter` a partir de los `GameObject` `player` (para el humano) o `bots` (para los bots).
    *   Asigna propiedades a cada `Fighter`:
        *   Para el jugador humano (`i == 0`), usa el `Team` y `Skin` del `GameState.Singleton`.
        *   Para los bots, asigna `Team` y `Skin` aleatoriamente o de forma estratégica para asegurar variedad en los equipos, especialmente en batallas de 2, 3 o 4 jugadores.
        *   Establece `initialLives` (tomado de `initialLives`), `visualFighter` (para ID visual), `indexFighter` (índice en el array), y `fighterName` (del `GameState` para el humano o un identificador "O_O" para bots).
        *   Asigna una `initialStone` (roca de inicio) a cada `Fighter` en la `PlayZone`, calculando el espaciado para distribuirlos uniformemente.

### `Update()`
`Update()` es un método que se llama una vez por frame y es el corazón del ciclo de juego en `CombatJudge`. Contiene un `switch` que gestiona el flujo del juego a través de los diferentes `SetMoments` (estados).

Los estados gestionados son:
*   `SetMoments.PickDice`: No realiza ninguna acción directa en este estado, asumiendo que la interfaz de usuario o otro script gestiona la elección del dado.
*   `SetMoments.RollDice`: Reinicia el temporizador `_time` para futuras esperas.
*   `SetMoments.RevealDice`: Espera 0.5 segundos y luego llama a `SetGlowing()` para resaltar las rocas disponibles a las que el jugador puede moverse, basándose en el valor del dado.
*   `SetMoments.GlowRock`: Si el turno actual es de un bot (`fighterTurn != 0`), llama al método `ThinkingRocks()` del `BotPlayer` correspondiente para que elija una roca.
*   `SetMoments.MoveToRock` y `SetMoments.SelectCombat`: No realizan acciones directas, esperando la interacción del jugador o bot.
*   `SetMoments.PickCard`: Verifica si todos los jugadores (`_fighters`) han elegido su carta (`GetPicked()`) y están activos en la batalla (`IsFigthing()`). Si todos han elegido, el estado cambia a `SetMoments.Reveal`.
*   `SetMoments.Result`: Espera 5 segundos después de que se han procesado los resultados del combate.
    *   Si el jugador humano (`_fighters[0]`) tiene 0 o menos vidas, el juego termina y se llama a `EndGamer(false)` (derrota).
    *   De lo contrario, itera sobre los bots (`_fighters` desde el índice 1) y elimina a aquellos con 0 o menos vidas. Se actualiza el array `_fighters` y se corrigen los `indexFighter` de los jugadores restantes.
    *   Si queda un jugador o menos, el juego termina (`EndGamer(true)` para victoria) y se otorgan puntos extra al jugador humano a través de `DeckManager.Instance.SetPoints(true)`.
    *   Si quedan más de un jugador, el juego continúa y el estado cambia a `SetMoments.Loop` para la siguiente ronda.
*   `SetMoments.Reveal`:
    *   Recopila las cartas elegidas por cada jugador en un array `card`.
    *   Calcula los resultados de cada combate individual entre todos los jugadores en una matriz `results`. Si los jugadores están en el mismo equipo, el resultado es un empate. De lo contrario, utiliza `IndividualCombat()` para determinar el resultado.
    *   Calcula el `destiny` (daño/curación neta) para cada jugador sumando los resultados de sus combates individuales (1 para ganar, 0 para empatar, -1 para perder).
    *   Aplica el `destiny` a la vida de cada jugador a través de `AddPlayerLive()`.
    *   Reinicia el temporizador `_time` y cambia el estado a `SetMoments.Result`.
*   `SetMoments.Loop`: Prepara el juego para la siguiente ronda.
    *   Actualiza `fighterTurn` al siguiente jugador en el ciclo.
    *   Hace que cada jugador (`_fighters`) rellene su mano (`RefillHand()`) y descarte cartas de la ronda anterior (`ThrowCard()`).
    *   Reinicia `_playersFighting` (máscara de bits de jugadores en combate).
    *   Si es el turno del jugador humano (`fighterTurn == 0`), el estado cambia a `SetMoments.Round`; de lo contrario, a `SetMoments.PickDice`.
*   `SetMoments.Round`:
    *   Incrementa la `Round`.
    *   Inicia la animación de inicio de ronda a través de `FindFirstObjectByType<RoundAnimation>().startRound()`.
    *   Cambia el estado a `SetMoments.Rounded`.
*   `SetMoments.End`: No realiza ninguna acción directa, ya que el final del juego se gestiona mediante llamadas a `EndGamer()`.

## Otros métodos

### `Results IndividualCombat(Card one, Card two)`
`IndividualCombat()` determina el resultado de un enfrentamiento uno a uno entre dos cartas (`Card`).

*   **Empate por cartas nulas**: Si alguna de las cartas es `null`, el combate es un `Results.Draw`.
*   **Combate por elemento específico de la roca**: Si `CombatType` no es `CombatType.Full` (es decir, la roca impone un elemento de combate), y los elementos de las cartas `one` y `two` son diferentes, el resultado depende de si el elemento de `one` coincide con el `CombatType` de la roca.
    *   Si `one.GetElement()` es igual al `CombatType`, `one` gana (`Results.Win`).
    *   De lo contrario, `one` pierde (`Results.Lose`).
*   **Combate elemental general**: Si no se impone un elemento de combate específico o los elementos de las cartas son iguales:
    *   Calcula `elementDiff`, la diferencia cíclica entre los elementos de `one` y `two`, considerando la cantidad total de `Element`s.
    *   **Número par de elementos**: Si la cantidad de elementos es par (e.g., 4 elementos: Fuego, Tierra, Agua, Aire):
        *   Si `elementDiff` no es 0 (elementos diferentes) ni `halfElements` (elementos opuestos), entonces el elemento más "fuerte" en la secuencia cíclica gana. Por ejemplo, si Fuego > Tierra y Tierra > Agua, etc., un `elementDiff` mayor a `halfElements` implica victoria.
        *   Si `elementDiff` es 0 (mismo elemento) o `halfElements` (elementos opuestos), el resultado se decide por el valor de las cartas: `one.GetValue()` contra `two.GetValue()`.
    *   **Número impar de elementos**: Si la cantidad de elementos es impar, el resultado se decide directamente por la `elementDiff`: si `elementDiff` es mayor que `halfElements`, `one` gana.
*   **Desempate por valor**: En caso de empate elemental o reglas específicas, la carta con mayor `Value` gana. Si los valores son iguales, es un empate.

### `void ArriveAtRock()`
`ArriveAtRock()` se invoca cuando un jugador (`_fighters[fighterTurn]`) ha completado su movimiento a una roca específica.

1.  **Determinar jugadores en combate**:
    *   Obtiene la roca `rocky` actual del jugador.
    *   Si `rocky.manyOn()` es `true` (hay más de un jugador en la misma roca), `_playersFighting` se establece como una máscara de bits que representa a todos los jugadores en esa roca específica.
    *   Si no, `_playersFighting` se establece para incluir a *todos* los jugadores activos en el juego, indicando una batalla "todos contra todos".
2.  **Definir el `CombatType` y el siguiente `SetMoments`**:
    *   Si `rocky.inscription` es `Inscription.pick` (la roca permite elegir elemento):
        *   Si es el turno del jugador humano (`Turn() == 0`), el estado cambia a `SetMoments.SelectCombat` para que el jugador elija un elemento.
        *   Si es el turno de un bot, el estado cambia a `SetMoments.PickCard` y el `CombatType` se elige aleatoriamente entre los 4 elementos.
    *   Si la roca no permite elegir elemento (su `inscription` es un elemento específico), el estado cambia a `SetMoments.PickCard` y `CombatType` se establece al elemento de la inscripción de la roca.

### `void MoveToRock(RockBehavior rocker)`
`MoveToRock()` se utiliza para establecer la roca de destino del jugador en turno y actualizar el estado del juego.

*   Asigna el objeto `RockBehavior` (`rocker`) como la `rocky` actual del `playerToken` del jugador en turno (`_fighters[fighterTurn]`).
*   Cambia el estado del juego a `SetMoments.MoveToRock`, indicando que un movimiento está en progreso.

### `bool PickElement(Element element)`
`PickElement()` permite al jugador en turno elegir un elemento para el combate si el estado actual es `SetMoments.SelectCombat`.

*   **Validación de estado**: Si el `actualAction` no es `SetMoments.SelectCombat`, la elección no es válida y devuelve `false`.
*   **Asignación de elemento**: Intenta convertir el `Element` proporcionado al `CombatType` y lo asigna a `CombatType`.
*   **Actualización de estado**: Si la asignación es exitosa, el estado del juego cambia a `SetMoments.PickCard`.
*   **Manejo de errores**: Utiliza un bloque `try-catch` para capturar posibles errores durante la conversión o asignación y los imprime en la consola, devolviendo `false` en caso de fallo. Si tiene éxito, devuelve `true`.

### `void EndRounded()`
`EndRounded()` se invoca para finalizar el estado de "ronda terminada" y pasar al siguiente paso del juego.

*   Si el `actualAction` actual es `SetMoments.Rounded`, el estado cambia a `SetMoments.PickDice`, lo que indica el inicio de la fase de elección de dado para la próxima ronda.

### `void StartRolling()`
`StartRolling()` inicia la secuencia de lanzamiento del dado.

*   Cambia el estado del juego a `SetMoments.RollDice`, lo que probablemente dispara animaciones o lógica de UI para el lanzamiento del dado.

### `void Rolled()`
`Rolled()` se llama una vez que el dado ha sido "lanzado" (su valor se ha determinado).

*   Si el `actualAction` actual es `SetMoments.RollDice`, el estado cambia a `SetMoments.RevealDice`, lo que significa que el valor del dado está listo para ser mostrado al jugador.

### `void SetGlowing(int value)`
`SetGlowing()` es un método privado responsable de resaltar las rocas a las que el jugador actual puede moverse después de un lanzamiento de dado.

1.  **Obtener roca actual y destinos**:
    *   Obtiene la roca actual (`lander`) donde se encuentra el jugador en turno (`_fighters[fighterTurn].playerToken.rocky`).
    *   Utiliza `lander.getNeighbor(value)` para obtener las dos rocas vecinas a las que el jugador puede moverse según el `value` del dado.
2.  **Resaltar rocas**:
    *   Establece la propiedad `shiny` a `true` para ambas rocas resultantes, lo que visualmente las resalta en la escena.
3.  **Actualizar estado**: Cambia el `actualAction` a `SetMoments.GlowRock`.
4.  **Acción del bot**: Si el turno actual no es del jugador humano (`fighterTurn != 0`), llama al método `PickRock()` del `BotPlayer` correspondiente, pasándole las rocas disponibles para que el bot elija.

### `void Surrender()`
`Surrender()` permite al jugador humano rendirse y terminar la partida.

1.  **Actualizar estado**: Cambia el `actualAction` a `SetMoments.End`.
2.  **Finalizar juego**: Llama a `FindFirstObjectByType<EndGame>().EndGamer(false)` para indicar el fin del juego y que el jugador ha perdido.
3.  **Registrar puntos**: Establece puntos para el jugador en el `DeckManager` como no haber ganado (`DeckManager.Instance.SetPoints(false)`).

## Getters y Setters

1.  `int Round`: Devuelve el número de la ronda actual. Se incrementa en cada nueva ronda.
2.  `CombatType CombatType`: Devuelve el tipo de combate elemental actual establecido para la ronda o para una roca específica.
3.  `SetMoments GetSetMoments()`: Devuelve el estado actual del juego, representado por el enum `SetMoments`.
4.  `int GetPlayersFighting()`: Devuelve una máscara de bits (`int`) que representa a los jugadores que están participando en el combate actual. Esto permite saber rápidamente qué jugadores están involucrados.
5.  `bool FocusOnTurn()`: Devuelve `true` si es el turno del jugador humano (cuyo `visualFighter` se asume que es 1); de lo contrario, devuelve `false`.
6.  `int Turn()`: Devuelve el índice del `Fighter` que tiene el turno actual en el array `_fighters`.
7.  `bool HurtPlayer()`: Devuelve `true` si el jugador humano (`_fighters[0]`) no ha recibido daño en la ronda actual (`NoHurt` es `true`); de lo contrario, devuelve `false`.