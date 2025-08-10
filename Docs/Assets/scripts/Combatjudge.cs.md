Aquí tienes la documentación técnica para el script `Combatjudge.cs`, diseñada para un nuevo miembro del equipo:

---

# `Combatjudge.cs`

## 1. Propósito General
Este script actúa como el director principal de la lógica de combate y el flujo de turnos en "Beast Card Clash". Gestiona el estado general del juego en cada ronda, desde la selección de cartas hasta la resolución de combates y la asignación de resultados, interactuando directamente con los personajes (`Figther`) y los elementos del tablero (`RockBehavior`, `PlayZone`).

## 2. Componentes Clave

El archivo define varias enumeraciones que son fundamentales para la lógica del juego, y la clase `Combatjudge` que orquesta la experiencia.

### `Element`
Esta enumeración simple define los cuatro tipos elementales principales presentes en el juego: `fire` (fuego), `earth` (tierra), `water` (agua) y `air` (aire). Es utilizada para categorizar las cartas y determinar las ventajas elementales en combate.

### `SetMoments`
Es una enumeración crucial que representa los distintos estados o fases por los que transita una ronda de juego, actuando como una máquina de estados para la lógica principal. Estos momentos incluyen `PickDice` (selección de dado), `RollDice` (lanzamiento de dado), `RevealDice` (revelación de dado), `GlowRock` (rocas brillan), `MoveToRock` (movimiento a una roca), `SelecCombat` (selección de tipo de combate), `PickCard` (selección de carta), `Reveal` (revelación de cartas), `Result` (resolución de resultados), `End` (fin de la ronda) y `Loop` (inicio de una nueva ronda).

### `Results`
Una enumeración que especifica los posibles desenlaces de un combate individual entre dos cartas: `lose` (derrota), `draw` (empate) y `win` (victoria).

### `CombatType`
Esta enumeración define los posibles modos de combate. Puede ser de un tipo elemental específico (`fire`, `earth`, `water`, `air`) donde solo las cartas de ese elemento son relevantes, o `full`, donde todos los elementos y sus interacciones son considerados.

### `Combatjudge`
Esta clase es un `MonoBehaviour` y es el cerebro central del sistema de juego. Está diseñada como un **singleton**, lo que significa que solo puede haber una instancia de ella en la escena, asegurando que toda la lógica de juego se centralice.

**Variables Públicas / Serializadas:**
La clase expone varias variables configurables a través del Inspector de Unity, y otras internas para el manejo del estado:
*   `manyPlayers`: Un entero que define la cantidad total de jugadores en la partida. Es utilizado para la inicialización y el manejo de turnos.
*   `player`: Un `GameObject` que se espera sea un prefab del personaje `Figther`. Se utiliza para instanciar nuevos jugadores si no hay suficientes presentes en la escena al inicio.
*   `players`: Un array de objetos `Figther` que mantiene la referencia a todos los personajes participantes en el juego.
*   `playerTurn`: Un entero que indica el índice del jugador que tiene el turno actual.
*   `setMoments`: Una variable de tipo `SetMoments` que almacena el estado actual de la ronda de juego, controlando el flujo mediante una máquina de estados.
*   `maxDice`: Un entero que define el valor máximo que puede tener un dado, influyendo en las reglas del juego (aunque la lógica de tirada de dado no está directamente en este script).
*   `initialLives`: Un entero que especifica la cantidad de vida inicial para cada jugador al comienzo del juego.
*   `combatType`: Una variable de tipo `CombatType` que define el tipo de combate activo en la ronda actual (ej. combate solo de fuego, o combate general).
*   `combatjudge` (static): Esta es la referencia estática a la única instancia de `Combatjudge`, implementando el patrón singleton.

**Métodos Principales:**

*   `void Start()`: Este es un método del ciclo de vida de Unity. Se ejecuta una vez al inicio del juego. Su función principal es asegurar que `Combatjudge` sea un singleton (destruyendo cualquier instancia duplicada). Inicializa el estado `setMoments` a `Loop` para comenzar el ciclo de juego. Luego, identifica o instancia los objetos `Figther` para todos los jugadores, asignándoles vidas iniciales, propiedades visuales (como `visualFigther` e `indexFigther`) y su roca inicial (`initialStone`) en el tablero.

*   `void Update()`: Este es otro método del ciclo de vida de Unity, ejecutado una vez por frame. Contiene la lógica principal de la máquina de estados del juego.
    *   Incluye una sección de depuración que permite asignar números a los jugadores presionando las teclas numéricas, utilizando el método `AsignarNumeros`.
    *   La parte central es una estructura `switch` que, según el valor de `setMoments`, ejecuta la lógica correspondiente a la fase actual de la ronda:
        *   En `PickCard`, verifica si todos los jugadores activos han seleccionado su carta, y si es así, transiciona a `Reveal`.
        *   `Reveal` simplemente transiciona a `Result`.
        *   `Result` es donde ocurre la lógica de combate: recopila las cartas elegidas por cada jugador, ejecuta `IndvCombat` para cada par de jugadores para determinar los resultados individuales, y luego calcula el impacto en la vida de cada jugador basándose en estos resultados. Finalmente, transiciona de nuevo a `Loop` para la siguiente ronda.
        *   `Loop` se encarga de preparar la siguiente ronda: avanza el `playerTurn` al siguiente jugador, indica a todos los jugadores que repongan sus manos y "lancen" una carta, y luego transiciona a `PickDice`.

*   `Results IndvCombat(Card one, Card two)`: Este método es fundamental para la resolución de combates individuales. Recibe dos objetos `Card` y, basándose en el `combatType` actual, determina el resultado (`win`, `lose`, `draw`) del enfrentamiento. La lógica considera si el combate es de un tipo elemental específico (donde solo la coincidencia elemental es clave) o `full`. Implementa una comparación circular de elementos para determinar ventajas y, en caso de empate elemental o combate `full`, compara los valores de las cartas para decidir el ganador.

*   `void Roled(int value)`: Se espera que este método sea invocado externamente (por ejemplo, después de una tirada de dado). Obtiene los vecinos de la roca actual del jugador en turno basándose en el `value` del dado y los marca para que "brillen" (`shiny = true`), luego transiciona el estado a `GlowRock`.

*   `void ArriveAtRock()`: Este método es invocado cuando un jugador ha llegado a una `RockBehavior`. Determina qué jugadores están involucrados en el combate (`playersFigthing`, `manyplayersFigthing`) según si hay varios jugadores en la misma roca. Dependiendo de la `inscription` (inscripción) de la roca, transiciona el estado a `SelecCombat` (si la roca permite elegir tipo de combate) o directamente a `PickCard`, asignando el `combatType` basado en la inscripción de la roca.

*   `void MoveToRock(RockBehavior rocker)`: Actualiza la `rocky` (roca actual) del `playerToken` del jugador en turno a la `rocker` proporcionada y establece el estado `setMoments` a `MoveToRock`, indicando un movimiento en curso.

*   `bool pickElement(Element element)`: Permite al jugador en turno seleccionar el tipo de combate elemental si el juego está en el estado `SelecCombat`. Si la selección es exitosa, actualiza `combatType` y transiciona a `PickCard`.

*   `SetMoments GetSetMoments()`: Un simple método getter para obtener el estado actual del juego (`setMoments`).

*   `int GetPlayersFigthing()`: Un simple método getter para obtener el valor de `playersFigthing`, que indica el estado de los jugadores que están actualmente involucrados en un combate.

*   `bool FocusONTurn()`: Devuelve `true` si el `visualFigther` del jugador en turno es 1, lo que podría indicar que este jugador es el "enfocado" o el principal en la UI para el turno actual.

**Lógica Clave:**
La lógica central de `Combatjudge` se basa en su **máquina de estados** impulsada por la enumeración `SetMoments` en el método `Update`. Este sistema secuencial asegura que las fases del juego (desde la preparación de la mano hasta el resultado del combate) se ejecuten en el orden correcto. El método `IndvCombat` encapsula el **algoritmo de resolución de combate**, que maneja tanto las reglas de ventaja elemental (utilizando aritmética modular para comparaciones circulares entre elementos) como la comparación de valores de cartas. La **inicialización dinámica de jugadores** en `Start` permite que el juego se adapte a jugadores preexistentes en la escena o cree nuevos según la configuración.

## 3. Dependencias y Eventos

*   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la presencia de otros componentes en el mismo `GameObject` en el editor de Unity.

*   **Eventos (Entrada):**
    *   Este script responde a la entrada de teclado (`Input.GetKeyDown`) para el método `AsignarNumeros`, lo cual parece ser una funcionalidad de depuración o desarrollo.
    *   Depende de llamadas externas a sus métodos públicos como `Roled()`, `ArriveAtRock()`, `MoveToRock()` y `pickElement()`. Estas llamadas probablemente provienen de otros scripts (ej. lógica de dado, scripts de interacción con el tablero, o controladores de UI) que notifican al `Combatjudge` sobre acciones del jugador o eventos del juego.

*   **Eventos (Salida):**
    `Combatjudge` no emite eventos explícitos de `UnityEvent` o `Action` para notificar a otros sistemas. En su lugar, gestiona el flujo de juego internamente a través de cambios en su variable de estado `setMoments`, y realiza **llamadas directas a métodos** de otros objetos, principalmente `Figther` (ej. `player.setPlayerLive()`, `player.RefillHand()`, `player.ThrowCard()`) y `RockBehavior` (ej. `rocker.shiny = true`), actuando como un controlador central que dicta las acciones de otros componentes. Otros sistemas necesitarían consultar el estado actual a través de `GetSetMoments()` o reaccionar a las manipulaciones directas de `Combatjudge` sobre ellos.

---