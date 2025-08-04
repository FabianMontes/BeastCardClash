# `Combatjudge.cs`

## 1. Propósito General
El script `Combatjudge` es un componente central del sistema de juego, actuando como el cerebro que gestiona el flujo de las partidas. Su rol principal es controlar el estado general del juego, orquestar los turnos de los jugadores, resolver los combates entre cartas y actualizar el estado de los jugadores y el tablero. Interactúa estrechamente con sistemas clave como los objetos `Player`, `Card` y `RockBehavior` para coordinar la lógica de juego.

## 2. Componentes Clave

Este archivo define varias enumeraciones que tipifican elementos del juego, además de la clase `Combatjudge` que es el controlador principal.

### Enumeraciones

*   **`Element`**: Define los tipos elementales posibles para las cartas y, posiblemente, para el combate general. Los valores son `fire`, `earth`, `water`, `air`.
*   **`SetMoments`**: Representa las diferentes fases o estados por los que transita el juego durante un turno o una ronda de combate. Funciona como una máquina de estados para la lógica del juego.
*   **`Results`**: Tipifica los posibles resultados de un enfrentamiento individual entre dos cartas: `lose`, `draw`, `win`.
*   **`CombatType`**: Extiende los `Element` para definir el tipo de combate actual, incluyendo un tipo `full` que implica reglas de combate generales, no restringidas a un elemento específico.

### `Combatjudge`

-   **Descripción:** `Combatjudge` es una clase `MonoBehaviour` que implementa el patrón Singleton para asegurar que solo exista una instancia de este controlador en la escena. Es responsable de la inicialización de los jugadores, la gestión de los turnos, la progresión a través de las fases de juego (`SetMoments`), y la lógica de resolución de combate entre cartas.

-   **Variables Públicas / Serializadas:**
    *   `manyPlayers` (int): Determina la cantidad de jugadores que participarán en la partida.
    *   `player` (GameObject): Una referencia al prefab del objeto `Player` que se instanciará para cada participante.
    *   `players` (Player[]): Un array que almacena las instancias de los objetos `Player` activos en la partida.
    *   `playerTurn` (int): Un índice que indica qué jugador tiene el turno actualmente.
    *   `setMoments` (SetMoments): La variable más crítica para la gestión del estado, indica la fase actual del juego.
    *   `maxDice` (int): Un valor para el dado, aunque su uso directo no es evidente en el código proporcionado.
    *   `initialLives` (int): La cantidad de vidas iniciales que cada jugador recibe al comienzo del juego.
    *   `combatType` (CombatType): El tipo de combate que se está llevando a cabo en la ronda actual (ej. un tipo elemental específico o `full` para combate general).
    *   `combatjudge` (static Combatjudge): La instancia estática del Singleton para el acceso global.

-   **Métodos Principales:**

    *   `void Start()`:
        Este método se invoca una vez al inicio del ciclo de vida del script. Implementa el patrón Singleton, asegurando que solo haya una instancia de `Combatjudge`. Inicializa el array de `players`, busca y asigna la zona de juego (`PlayZone`) y el lienzo (`Canvas`). Instancia nuevos objetos `Player` si no existen suficientes en la escena, asigna propiedades iniciales como vidas, índices y la piedra inicial (`RockBehavior`) en el tablero a cada jugador.

    *   `void Update()`:
        Este método se llama una vez por fotograma. Contiene la lógica principal del bucle de juego y el controlador de la máquina de estados.
        *   Incluye un bucle `for` para detectar entradas numéricas (`Input.GetKeyDown(i.ToString())`) que llaman a `AsignarNumeros(i)`, lo que sugiere una funcionalidad de depuración o desarrollo para asignar propiedades visuales a los jugadores.
        *   Un bloque `switch` central gestiona las transiciones y la lógica para cada fase definida en `SetMoments`.
        *   **Fase `PickCard`**: Comprueba si todos los jugadores que están en combate han seleccionado una carta. Si es así, transiciona a la fase `Reveal`.
        *   **Fase `Reveal`**: Actualmente, transiciona inmediatamente a la fase `Result`.
        *   **Fase `Result`**: Recopila las cartas seleccionadas por todos los jugadores, resuelve los combates individuales llamando a `IndvCombat` para cada par de cartas, y luego actualiza las vidas de los jugadores basándose en los resultados. Tras resolver, vuelve a la fase `Loop`.
        *   **Fase `Loop`**: Marca el final de una ronda de combate. Actualiza el `playerTurn` al siguiente jugador, llama a `RefillHand()` y `ThrowCard()` para cada jugador, y transiciona a la fase `PickDice` para iniciar una nueva ronda.

    *   `Results IndvCombat(Card one, Card two)`:
        Este método es el núcleo de la lógica de resolución de combate. Toma dos objetos `Card` como parámetros y determina el resultado del enfrentamiento (`win`, `lose`, `draw`).
        *   Maneja casos donde una o ambas cartas son `null` (resultado `draw`).
        *   Si `combatType` no es `full`, aplica una regla elemental: si las cartas tienen elementos diferentes, la carta cuyo elemento coincide con `combatType` gana.
        *   Si los elementos de las cartas son iguales, o si el `combatType` es `full`, la victoria se determina por el valor numérico de la carta.
        *   Incorpora una lógica compleja para la ventaja elemental circular cuando los elementos son diferentes y `combatType` es `full`, calculando la diferencia de elementos con módulo para determinar el ganador.

    *   `void Roled(int value)`:
        Se invoca cuando un dado es "tirado". Obtiene los `RockBehavior` vecinos de la piedra actual del jugador activo basándose en el `value` del dado, los marca como "brillantes" (`shiny = true`), y cambia el estado del juego a `GlowRock`.

    *   `void ArriveAtRock()`:
        Este método se llama cuando un jugador llega a una nueva "roca" o ubicación en el tablero. Determina qué jugadores están actualmente sobre esa roca y, por ende, participarán en el combate. Ajusta las variables `playersFigthing` y `manyplayersFigthing` en consecuencia. Si la inscripción de la roca (`rocky.inscription`) es `pick`, el estado del juego cambia a `SelecCombat`; de lo contrario, cambia a `PickCard` y establece el `combatType` según la inscripción de la roca.

    *   `void MoveToRock(RockBehavior rocker)`:
        Actualiza la `rocky` (piedra) actual del `playerToken` del jugador en turno a la `rocker` proporcionada y establece el estado del juego a `MoveToRock`.

    *   `bool pickElement(Element element)`:
        Permite seleccionar un elemento para el combate si el juego se encuentra en la fase `SelecCombat`. Intenta convertir el `Element` seleccionado a un `CombatType` y cambia el estado del juego a `PickCard`. Devuelve `true` si la selección fue exitosa, `false` en caso de error.

-   **Lógica Clave:**

    La lógica central del juego reside en la máquina de estados implementada a través de la variable `setMoments` y el bloque `switch` en el método `Update`. Esta estructura dicta la progresión del juego a través de fases como la selección de dados, el movimiento, la selección de combate, la selección de cartas, la revelación y la resolución de resultados. Cada fase tiene responsabilidades específicas que coordinan las acciones de los jugadores y las interacciones con el tablero.

    El método `IndvCombat` encapsula las reglas de combate del juego. Combina una lógica de piedra-papel-tijera elemental (aunque expandida a cuatro elementos) con una comparación de valores de cartas. La complejidad radica en cómo maneja los empates elementales o los combates de tipo "completo" (`full`), donde los valores de las cartas se vuelven decisivos, y cómo calcula la ventaja/desventaja circular entre elementos.

## 3. Dependencias y Eventos

-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la necesidad de otros componentes en el mismo GameObject.

-   **Eventos (Entrada):**
    *   **Entrada de Usuario:** Reacciona a las teclas numéricas (`0` a `9`) para llamar a `AsignarNumeros`, lo cual es probablemente una función de depuración o una forma de controlar el juego durante el desarrollo.
    *   **Interacciones con Objetos `Player`**: Recupera y modifica datos de objetos `Player` (e.g., `getPicked()`, `IsFigthing()`, `setPlayerLive()`, `addPlayerLive()`, `RefillHand()`, `ThrowCard()`, `randomSpecie()`, acceso a `playerToken.rocky`).
    *   **Interacciones con Objetos `Card`**: Llama a métodos de objetos `Card` para obtener sus propiedades (`GetElement()`, `GetValue()`).
    *   **Interacciones con Objetos `RockBehavior`**: Consulta el estado de las rocas en el tablero (e.g., `getNeighbor()`, `manyOn()`, `GetPlayersOn()`, `ManyPlayerOn()`, `inscription`) y modifica sus propiedades visuales (`shiny`).
    *   **Búsqueda de Objetos en Escena**: Utiliza `FindObjectsByType<Player>()`, `FindFirstObjectByType<PlayZone>()`, y `FindFirstObjectByType<Canvas>()` al inicio para establecer referencias a otros sistemas y elementos de la interfaz de usuario.

-   **Eventos (Salida):**
    *   Este script no expone directamente eventos de `UnityEvent` o `Action` para que otros sistemas se suscriban.
    *   Sin embargo, genera "eventos" lógicos a través de la modificación de estados de otros objetos:
        *   Cambia el estado visual de `RockBehavior` (`rocker.shiny = true`).
        *   Actualiza las vidas de los `Player` (`players[i].addPlayerLive(destiny[i])`).
        *   Llama a métodos de `Player` para gestionar sus manos de cartas (`RefillHand()`, `ThrowCard()`).
        *   Imprime mensajes de depuración en la consola de Unity (`print()`).