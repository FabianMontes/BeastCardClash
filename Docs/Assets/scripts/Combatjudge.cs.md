# `Combatjudge.cs`

## 1. Propósito General
Este script actúa como el controlador central del juego para la lógica de combate y la progresión de turnos. Gestiona el flujo del juego a través de distintas fases de combate, inicializa y organiza a los jugadores (`Figther`), y determina los resultados de las batallas entre cartas. Interactúa directamente con los sistemas de jugadores, cartas y el entorno de juego (rocas).

## 2. Componentes Clave

### `Element`
- **Descripción:** Un `enum` que define los cuatro tipos elementales principales que pueden tener las cartas y los combates: `fire`, `earth`, `water`, `air`.

### `SetMoments`
- **Descripción:** Un `enum` crucial que representa los distintos estados o fases por los que transita una ronda de juego. Este `enum` impulsa la máquina de estados dentro del método `Update` de `Combatjudge`, controlando la secuencia de eventos como la elección de dados, el combate de cartas y la determinación de resultados.
    - Las fases incluyen: `PickDice`, `RollDice`, `RevealDice`, `GlowRock`, `MoveToRock`, `SelecCombat`, `PickCard`, `Reveal`, `Result`, `End`, `Loop`.

### `Results`
- **Descripción:** Un `enum` simple que define los posibles resultados de un combate individual: `lose`, `draw`, `win`.

### `CombatType`
- **Descripción:** Similar al `enum` `Element`, pero extendido para incluir `full`, indicando un combate donde no hay un elemento dominante predefinido. Define el tipo de elemento que rige un combate específico o un combate general.

### `Combatjudge`
- **Descripción:** Esta es la clase principal del script, que hereda de `MonoBehaviour`. Es el cerebro del sistema de combate, orquestando el flujo de juego, la gestión de jugadores, la resolución de combates y la configuración inicial del entorno.
    Se ejecuta con una `DefaultExecutionOrder(-1)`, lo que asegura que este script se inicialice antes que la mayoría de los demás scripts en la escena.

- **Variables Públicas / Serializadas:**
    - `manyPlayers` (`[SerializeField] int`): Define la cantidad de jugadores que participarán en el juego. Se utiliza para dimensionar el array de jugadores y para la lógica de instanciación.
    - `player` (`[SerializeField] GameObject`): Una referencia al prefab o GameObject de tipo `Figther` que se instanciará para crear nuevos jugadores si no hay suficientes `Figther` ya presentes en la escena.
    - `players` (`Figther[]`): Un array que almacena referencias a todas las instancias de `Figther` activas en el juego.
    - `playerTurn` (`[SerializeField] int`): El índice del jugador que tiene el turno actual. Se actualiza en la fase `Loop` de cada ronda.
    - `setMoments` (`[SerializeField] SetMoments`): La variable más importante para la máquina de estados. Almacena la fase actual de la ronda de juego, controlando la lógica ejecutada en `Update`.
    - `maxDice` (`[SerializeField] public int`): Un valor entero que probablemente indica el valor máximo de un dado de juego.
    - `initialLives` (`[SerializeField] int`): La cantidad de puntos de vida con los que cada jugador comienza la partida.
    - `combatType` (`public CombatType`): El tipo elemental del combate actual. Se establece en función de la "roca" en la que se encuentran los jugadores.
    - `combatjudge` (`public static Combatjudge`): Una referencia estática a sí mismo, implementando el patrón Singleton para asegurar que solo haya una instancia de `Combatjudge` en la escena en todo momento.

- **Métodos Principales:**
    - `void Start()`:
        *   **Propósito:** Se invoca una vez al inicio del ciclo de vida del script. Inicializa el `Combatjudge` y configura el juego.
        *   **Lógica Clave:** Implementa el patrón Singleton, destruyendo cualquier instancia duplicada. Establece el estado inicial de `setMoments` a `Loop` para iniciar la primera ronda. Busca `Figther` existentes en la escena y, si es necesario, instancia nuevos a partir del prefab `player` hasta alcanzar `manyPlayers`. Para cada jugador, configura sus vidas iniciales, su ID visual (`visualFigther`), su índice interno (`indexFigther`) y su roca inicial (`initialStone`). También localiza `PlayZone` y `Canvas` para el posicionamiento de los jugadores.

    - `void Update()`:
        *   **Propósito:** Se llama una vez por fotograma. Es el corazón de la máquina de estados del juego, controlando la progresión de las fases de la ronda.
        *   **Lógica Clave:** Contiene un bucle para detectar entradas de teclado (números del 0 al 9) que invocan `AsignarNumeros`, aparentemente una función de depuración o visualización. La mayor parte del método es un `switch` basado en `setMoments`:
            *   **`PickCard`**: Espera a que todos los jugadores en combate elijan una carta. Una vez que lo hacen, transiciona a `Reveal`.
            *   **`Reveal`**: Inmediatamente transiciona a `Result`.
            *   **`Result`**: Recoge las cartas elegidas por los jugadores, resuelve los combates individuales llamando a `IndvCombat` para cada par de cartas, calcula el resultado general para cada jugador y actualiza sus vidas. Luego, transiciona a `Loop`.
            *   **`Loop`**: Avanza el turno al siguiente jugador, instruye a todos los jugadores a rellenar su mano y lanzar una carta (`RefillHand`, `ThrowCard`), y transiciona el juego a la fase `PickDice` para iniciar una nueva secuencia de turno.

    - `Results IndvCombat(Card one, Card two)`:
        *   **Propósito:** Resuelve el resultado de un combate entre dos cartas específicas.
        *   **Parámetros:** `one` y `two` (ambos de tipo `Card`) son las cartas a comparar.
        *   **Retorno:** Un valor del `enum` `Results` (`win`, `lose`, `draw`) desde la perspectiva de la carta `one`.
        *   **Lógica Clave:** Maneja casos donde una carta es `null` (resultando en empate). Si `combatType` no es `full`, la victoria o derrota puede depender únicamente de si una carta coincide con el `combatType`. De lo contrario, compara los elementos de las cartas usando una lógica de "piedra-papel-tijeras" circular (`diferer`) y, en caso de empate elemental o condiciones específicas, utiliza los valores numéricos de las cartas (`GetValue()`) como desempate.

    - `void Roled(int value)`:
        *   **Propósito:** Se llama cuando se "lanza" un dado, indicando posibles movimientos en el tablero.
        *   **Parámetro:** `value` (int) es el resultado del lanzamiento del dado.
        *   **Lógica:** Identifica las rocas vecinas a la roca actual del jugador según el `value` del dado, las marca como "brillantes" (`shiny = true`) para una indicación visual, y actualiza el estado a `GlowRock`.

    - `void ArriveAtRock()`:
        *   **Propósito:** Se invoca cuando el token de un jugador llega a una nueva roca.
        *   **Lógica:** Determina qué jugadores están en la misma roca para el combate. Si hay múltiples jugadores en la roca, `playersFigthing` y `manyplayersFigthing` se configuran para reflejar solo a esos jugadores. Si no, se configuran para incluir a todos los jugadores. Si la roca tiene una `inscription` de tipo `pick`, permite al jugador seleccionar el tipo de combate (`SelecCombat`); de lo contrario, establece `combatType` según la inscripción de la roca y avanza a `PickCard`.

    - `void MoveToRock(RockBehavior rocker)`:
        *   **Propósito:** Actualiza la posición del token del jugador actual a una nueva roca.
        *   **Parámetro:** `rocker` (RockBehavior) es la roca de destino.
        *   **Lógica:** Asigna la nueva roca al token del jugador actual (`players[playerTurn].playerToken.rocky`) y actualiza el estado a `MoveToRock`.

    - `bool pickElement(Element element)`:
        *   **Propósito:** Permite al jugador seleccionar un tipo elemental para el combate si el juego está en la fase `SelecCombat`.
        *   **Parámetro:** `element` (Element) es el elemento elegido.
        *   **Retorno:** `true` si la selección fue exitosa y el estado se actualizó, `false` en caso contrario (por ejemplo, si no está en la fase `SelecCombat`).
        *   **Lógica:** Si el estado actual es `SelecCombat`, intenta convertir el `Element` proporcionado a `CombatType` y lo asigna a `combatType`, luego transiciona a `PickCard`.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    *   `Combatjudge` no utiliza explícitamente el atributo `[RequireComponent]`. Sin embargo, su funcionalidad depende fundamentalmente de la presencia de otros scripts y objetos en la escena, como `Figther`, `Card`, `RockBehavior`, `PlayZone` y `Canvas`. Estos son componentes que debe poder encontrar o instanciar para operar correctamente.

-   **Eventos (Entrada):**
    *   Este script responde a la entrada directa del teclado (`Input.GetKeyDown`) para fines de depuración o desarrollo, invocando `AsignarNumeros`.
    *   La progresión de su máquina de estados (`setMoments`) se basa en llamadas externas a sus métodos públicos como `Roled`, `ArriveAtRock`, `MoveToRock` y `pickElement`. Esto implica que otros sistemas (como la UI para la selección de dados o movimientos, o las interacciones del jugador con las rocas) son responsables de invocar estos métodos en el momento oportuno.

-   **Eventos (Salida):**
    *   El script no invoca explícitamente `UnityEvent`s o `Action`s para notificar a otros sistemas. En su lugar, modifica directamente las propiedades y llama a métodos de otros objetos (`Figther`, `RockBehavior`), como `players[i].setPlayerLive()`, `rocker[0].shiny = true`, `players[i].RefillHand()`, `players[i].ThrowCard()`. Estas modificaciones actúan como una forma de comunicación, informando a esos objetos sobre cambios de estado o acciones requeridas.