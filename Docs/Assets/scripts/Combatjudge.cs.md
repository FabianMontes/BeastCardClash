# `Combatjudge.cs`

## 1. Propósito General
Este script es el controlador central del flujo de juego y la lógica de combate en "Beast Card Clash". Gestiona el estado general del juego, la progresión por turnos, la inicialización de los jugadores y la resolución de los combates entre ellos. Interactúa principalmente con los componentes `Figther` (jugadores) y `Card` (cartas), así como con la infraestructura del tablero (`PlayZone`, `RockBehavior`).

## 2. Componentes Clave

### Enums
Este archivo define varios enumeradores que son cruciales para entender los diferentes estados y tipos del juego:
*   `Element`: Representa los tipos elementales de las cartas y combates (fuego, tierra, agua, aire).
*   `SetMoments`: Define las fases principales del turno de juego o del proceso de combate, funcionando como un sistema de máquina de estados para el `Update` del juego.
*   `Results`: Enumera los posibles resultados de un combate individual (perder, empatar, ganar).
*   `CombatType`: Describe el tipo de combate en curso, que puede ser de un elemento específico o un combate "completo" donde todos los elementos son relevantes.

### `Combatjudge`
- **Descripción:** La clase `Combatjudge` es un `MonoBehaviour` que actúa como un singleton central para orquestar la lógica del juego. Se asegura de que solo haya una instancia de sí mismo en la escena para gestionar la progresión del juego, los turnos de los jugadores y el proceso de resolución de combates. Se ejecuta con una orden de ejecución temprana (`-1`) para asegurar que esté listo antes que otros scripts dependientes.

- **Variables Públicas / Serializadas:**
    *   `manyPlayers` (int): Determina el número esperado de jugadores en la partida.
    *   `player` (GameObject): Una referencia al prefab del `GameObject` del jugador (`Figther`) que se instanciará si no hay suficientes jugadores preexistentes en la escena.
    *   `players` (Figther[]): Un array que almacena las referencias a todos los objetos `Figther` (jugadores) participantes en el juego.
    *   `playerTurn` (int): Indica el índice del jugador que tiene el turno actual.
    *   `setMoments` (SetMoments): La variable más importante para la lógica del flujo del juego, ya que su valor determina la fase actual del turno o del combate.
    *   `maxDice` (int): El valor máximo que un dado puede mostrar.
    *   `initialLives` (int): La cantidad de vidas con las que cada jugador comienza la partida.
    *   `combatType` (CombatType): Define el tipo elemental del combate actual, influenciando la lógica de `IndvCombat`.
    *   `combatjudge` (static Combatjudge): La instancia estática de la clase, que implementa el patrón singleton.

- **Métodos Principales:**
    *   `void Start()`: Este es un método del ciclo de vida de Unity. Se encarga de inicializar el singleton `Combatjudge`. También inicializa los jugadores: busca instancias existentes de `Figther` en la escena o instancia nuevas si es necesario, asigna vidas iniciales, establece sus propiedades visuales (`visualFigther`, `indexFigther`) y su roca inicial en el tablero (`initialStone`).
        ```csharp
        if (combatjudge == null) { combatjudge = this; }
        else { Destroy(gameObject); }
        // ... (inicialización de jugadores)
        ```

    *   `void Update()`: Otro método del ciclo de vida de Unity, ejecutado una vez por frame. Contiene la máquina de estados principal del juego, controlada por la variable `setMoments`.
        *   **Manejo de Input:** Responde a la pulsación de teclas numéricas (0-9) para llamar a `AsignarNumeros`, lo cual parece ser para asignar un "foco visual" al jugador cuyo número se presiona.
        *   **Máquina de Estados:** Un `switch` masivo que controla la progresión del juego a través de las diferentes `SetMoments`. Las fases clave incluyen:
            *   `PickCard`: Espera que todos los jugadores que están combatiendo seleccionen una carta. Una vez todas las cartas son seleccionadas, avanza a `Reveal`.
            *   `Reveal`: Transición rápida a `Result`.
            *   `Result`: Recopila las cartas seleccionadas por cada jugador, ejecuta `IndvCombat` para cada par de cartas para determinar el resultado de cada enfrentamiento individual, y luego consolida estos resultados para ajustar las vidas de los jugadores. Transiciona a `Loop`.
            *   `Loop`: Prepara el siguiente turno: incrementa `playerTurn` (circularmente), hace que todos los jugadores rellenen sus manos y "tiren" una carta (probablemente para ponerla en juego o descartarla), y transiciona a `PickDice`.

    *   `Results IndvCombat(Card one, Card two)`: Este método contiene la lógica principal para determinar el resultado de un combate entre dos cartas específicas.
        *   Considera el `combatType` actual del `Combatjudge`. Si el combate no es `full` (completo), el elemento de las cartas es el factor decisivo.
        *   Si los elementos son diferentes o el `combatType` es `full`, aplica una lógica de piedra-papel-tijera basada en los elementos (`diferer`), donde la diferencia entre los índices de los elementos determina el ganador.
        *   En caso de empate elemental o si el `combatType` es `full` y los elementos son iguales, se compara el valor (`GetValue()`) de las cartas para determinar el ganador.

    *   `void Roled(int value)`: Se llama cuando se "tira" un valor de dado. Utiliza el `playerToken` del jugador actual para obtener rocas vecinas (`getNeighbor`) y las marca como "brillantes" (`shiny`), lo que probablemente indica opciones de movimiento. Luego, cambia el estado a `GlowRock`.

    *   `void ArriveAtRock()`: Se invoca cuando un jugador llega a una roca. Determina cuántos jugadores están en esa roca y configura `playersFigthing` y `manyplayersFigthing` en consecuencia. Si la roca tiene una "inscripción" de tipo `pick`, se pasa al estado `SelecCombat`; de lo contrario, se pasa a `PickCard` y se establece el `combatType` según la inscripción de la roca.

    *   `void MoveToRock(RockBehavior rocker)`: Actualiza la roca actual del jugador en turno (`players[playerTurn].playerToken.rocky`) y cambia el estado del juego a `MoveToRock`.

    *   `void AsignarNumeros(int baseIndex)`: Este método ajusta la propiedad `visualFigther` de cada jugador. Parece ser una forma de asignar un índice visual relativo a un `baseIndex`, posiblemente para resaltar quién está "enfocado" o en una posición específica en la UI.

    *   `bool pickElement(Element element)`: Permite al jugador seleccionar un tipo elemental para el `combatType` actual, pero solo si el juego está en la fase `SelecCombat`. Si la selección es exitosa, transiciona el juego a la fase `PickCard`.

- **Lógica Clave:**
    La lógica clave reside en la implementación de una máquina de estados controlada por la variable `setMoments` dentro del método `Update`. Esta máquina de estados dicta la progresión del turno del juego, manejando desde la selección de dados y el movimiento hasta la selección de cartas, la revelación y la compleja resolución del combate. La función `IndvCombat` encapsula las reglas fundamentales del juego para determinar el ganador de cada enfrentamiento individual, combinando la lógica elemental con la comparación de valores de cartas. La inicialización en `Start` también es crucial, ya que maneja la persistencia y creación de jugadores.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no fuerza la presencia de otros componentes en el mismo `GameObject` en el editor de Unity. Sin embargo, funcionalmente requiere la existencia de componentes como `Figther`, `Card`, `PlayZone` y `RockBehavior` en la escena para operar correctamente, ya que interactúa directamente con ellos.

- **Eventos (Entrada):**
    *   Este script se suscribe directamente a la entrada del teclado (`Input.GetKeyDown(i.ToString())`) dentro de su método `Update` para la funcionalidad de `AsignarNumeros`.
    *   Recibe llamadas a sus métodos públicos (`Roled`, `ArriveAtRock`, `MoveToRock`, `pickElement`) desde otros sistemas (presumiblemente UI, `RockBehavior` o scripts de `Figther`) para avanzar el estado del juego.

- **Eventos (Salida):**
    Este script no invoca explícitamente `UnityEvent`s o delegados (`Action`) para notificar a otros sistemas. En su lugar, modifica directamente el estado de otros objetos (ej. `players[i].setPlayerLive(...)`, `players[i].visualFigther = ...`, `rocker[0].shiny = true`) y su propio estado (`setMoments`), lo que implica que otros scripts deben monitorear estos cambios o ser llamados directamente por `Combatjudge`.