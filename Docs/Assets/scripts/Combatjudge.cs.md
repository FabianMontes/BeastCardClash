Aquí está la documentación técnica para el script `Combatjudge.cs`.

---

# `Combatjudge.cs`

## 1. Propósito General
Este script central actúa como el principal controlador de la lógica de combate y el flujo del juego en "Beast Card Clash". Gestiona los estados del juego, los turnos de los jugadores, la resolución de los combates entre cartas y la inicialización de los elementos clave del juego, como los jugadores y sus zonas de inicio. Interactúa principalmente con sistemas de jugadores (`Figther`), cartas (`Card`) y elementos del tablero (`RockBehavior`, `PlayZone`).

## 2. Componentes Clave

### Enums
El archivo define varios enums que son fundamentales para la lógica del juego:

*   **`Element`**: Define los cuatro tipos elementales (fuego, tierra, agua, aire) que pueden tener las cartas y las inscripciones en las rocas.
*   **`SetMoments`**: Representa los diferentes estados o "momentos" en el ciclo de juego, guiando el flujo de los turnos y las fases de combate. Incluye fases como `PickDice`, `RollDice`, `RevealDice`, `GlowRock`, `MoveToRock`, `SelecCombat`, `PickCard`, `Reveal`, `Result`, `End` y `Loop`.
*   **`Results`**: Define los posibles resultados de un combate individual: `lose` (perder), `draw` (empate) o `win` (ganar).
*   **`CombatType`**: Extiende los elementos con una opción `full`, indicando si el combate es específico de un elemento o general.

### `Combatjudge`
Esta clase, que hereda de `MonoBehaviour`, es el núcleo de la lógica del juego. Está configurada para ejecutarse antes que la mayoría de los otros scripts (`[DefaultExecutionOrder(-1)]`) y sigue el patrón de diseño Singleton para asegurar que solo haya una instancia en la escena.

*   **Variables Públicas / Serializadas:**
    *   `manyPlayers` (int): Determina la cantidad esperada de jugadores en la partida. Se serializa para ser configurado en el Inspector de Unity.
    *   `player` (GameObject): Un prefab de GameObject que se utilizará para instanciar nuevos jugadores si no se encuentran suficientes `Figther` existentes en la escena.
    *   `players` (`Figther[]`): Un array que almacena las instancias de los jugadores (`Figther`) presentes en el juego.
    *   `playerTurn` (int): Almacena el índice del jugador cuyo turno está activo.
    *   `setMoments` (`SetMoments`): La variable más crítica, ya que controla la fase actual del juego. Es visible y configurable en el Inspector.
    *   `maxDice` (int): Un valor utilizado para la mecánica del dado (aunque su uso directo no está completamente expuesto en este fragmento).
    *   `initialLives` (int): La cantidad de puntos de vida con los que cada jugador comienza la partida.
    *   `combatType` (`CombatType`): Define el tipo de combate activo, ya sea por un elemento específico o `full` (combate general).

*   **Métodos Principales:**

    *   `void Start()`: Este método se ejecuta una vez al inicio del juego.
        *   Implementa el patrón Singleton, asegurando que solo una instancia de `Combatjudge` persista en la escena.
        *   Inicializa `setMoments` a `SetMoments.Loop` para comenzar el ciclo de juego.
        *   Busca instancias existentes de `Figther` en la escena. Si no hay suficientes, instancia nuevos jugadores desde el prefab `player`, los asigna al `Canvas` y les da una especie aleatoria.
        *   Asigna los puntos de vida iniciales, índices y referencias a las rocas iniciales (`initialStone`) a cada jugador.
        *   Encuentra la `PlayZone` y `Canvas` en la escena para la configuración inicial.

    *   `void Update()`: Se llama una vez por cada frame del juego.
        *   Maneja la entrada de teclado (teclas 0-9) para invocar `AsignarNumeros`, lo cual parece ser una funcionalidad de depuración o de reasignación visual de jugadores.
        *   Contiene la máquina de estados principal del juego mediante un `switch` basado en la variable `setMoments`. Cada `case` representa una fase del juego:
            *   `PickCard`: Espera a que todos los jugadores en combate hayan seleccionado una carta. Una vez todas las cartas son recogidas, el estado transiciona a `Reveal`.
            *   `Reveal`: Simplemente transiciona al estado `Result`.
            *   `Result`: Recopila las cartas seleccionadas por cada jugador. Realiza combates individuales (`IndvCombat`) entre cada par de cartas para determinar resultados. Basándose en estos resultados, calcula el destino de vida de cada jugador y actualiza sus puntos de vida. Finalmente, transiciona al estado `Loop`.
            *   `Loop`: Es el inicio de un nuevo turno. Incrementa `playerTurn`, pide a todos los jugadores que recarguen su mano y "lancen" una carta (probablemente preparar la siguiente ronda). Transiciona a `PickDice` y resetea el contador de jugadores en combate.

    *   `Results IndvCombat(Card one, Card two)`: Un método crucial que resuelve el combate entre dos cartas.
        *   Si alguna carta es nula, el combate es un empate.
        *   Si `combatType` no es `full`, se verifica si el elemento de una de las cartas coincide con `combatType` para determinar si gana o pierde directamente.
        *   Si los elementos son diferentes y el `combatType` es `full`, calcula la diferencia circular entre los elementos para determinar el ganador (parece usar una lógica de "rueda" elemental).
        *   Si los elementos son iguales o la diferencia circular es 0 o la mitad de los elementos, el combate se decide por el valor numérico de las cartas (`GetValue()`).

    *   `void Roled(int value)`: Se invoca cuando un dado ha sido "lanzado". Identifica las rocas vecinas a la roca actual del jugador en turno basándose en el valor del dado y las marca como "brillantes" (`shiny`). Cambia el estado a `GlowRock`.

    *   `void ArriveAtRock()`: Se invoca cuando un jugador llega a una roca. Determina qué jugadores están en la roca (o todos si la roca está vacía de otros jugadores) para establecer `playersFigthing` y `manyplayersFigthing`. Si la roca tiene una inscripción `pick`, el juego transiciona a `SelecCombat`, de lo contrario, transiciona a `PickCard` y establece `combatType` basándose en la inscripción de la roca.

    *   `void MoveToRock(RockBehavior rocker)`: Actualiza la referencia de la roca actual del token del jugador en turno y cambia el estado del juego a `MoveToRock`.

    *   `bool pickElement(Element element)`: Permite al jugador seleccionar un tipo de elemento para el combate si el juego está en la fase `SelecCombat`. Establece `combatType` y transiciona a `PickCard`.

*   **Lógica Clave:**
    La lógica principal se centra en la máquina de estados en el método `Update`, que orquesta el flujo de cada turno de juego desde la selección de cartas hasta la resolución de combates y la actualización de vidas. El método `IndvCombat` encapsula las reglas complejas de combate, considerando tipos elementales y valores de carta. El script también gestiona la creación dinámica de jugadores y su posicionamiento inicial en el tablero.

## 3. Dependencias y Eventos

*   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, espera encontrar y interactuar con componentes como `Figther` (en los jugadores), `Card` (en las manos de los jugadores y para el combate), `RockBehavior` (para las rocas del tablero) y `PlayZone` (para gestionar las zonas del tablero).

*   **Eventos (Entrada):**
    El script monitoriza la entrada del teclado directamente a través de `Input.GetKeyDown(i.ToString())` dentro del método `Update`. Esto se usa para invocar la función `AsignarNumeros`. No se suscribe a eventos de `UnityEvent` o `Action` de otros componentes en el código proporcionado.

*   **Eventos (Salida):**
    Este script no invoca explícitamente `UnityEvent`s o `Action`s para notificar a otros sistemas. Su interacción con otros sistemas se realiza principalmente mediante la llamada directa a métodos en las instancias de los jugadores (`Figther`) y las rocas (`RockBehavior`), y actualizando su propio estado interno (`setMoments`) para que otros scripts que monitoreen `GetSetMoments()` puedan reaccionar.

---