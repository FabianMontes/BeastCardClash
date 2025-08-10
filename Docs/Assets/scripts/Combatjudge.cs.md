# `Combatjudge.cs`

## 1. Propósito General
Este script centraliza la gestión del flujo de juego y la lógica de combate en "Beast Card Clash". Actúa como una máquina de estados que orquesta las diferentes fases de la partida, maneja los turnos de los jugadores, resuelve los enfrentamientos entre cartas y actualiza el estado de los jugadores. Interactúa principalmente con los sistemas de jugadores (`Figther`), cartas (`Card`), el tablero de juego (`PlayZone`) y los elementos interactivos del escenario (`RockBehavior`).

## 2. Componentes Clave

### Enums
Este archivo define varios enums esenciales para la lógica del juego:
*   `Element`: Representa los tipos elementales (fuego, tierra, agua, aire) que pueden tener las cartas y las zonas de combate.
*   `SetMoments`: Define las fases o estados clave del ciclo de juego (por ejemplo, `PickDice`, `PickCard`, `Result`, `Loop`). La lógica del juego transiciona entre estos estados.
*   `Results`: Enumera los posibles resultados de un combate individual entre cartas (`lose`, `draw`, `win`).
*   `CombatType`: Similar a `Element`, pero se usa para especificar el tipo de combate que se está llevando a cabo, incluyendo un tipo `full` para combate general.

### `Combatjudge`
*   **Descripción:**
    `Combatjudge` es un script de tipo `MonoBehaviour` que implementa el patrón Singleton, asegurando que solo haya una instancia activa en la escena. Su función principal es controlar el flujo del juego, desde la inicialización de los jugadores hasta la resolución de los combates de cartas y la gestión de los turnos. Mantiene el estado actual del juego a través de la variable `setMoments` y coordina las acciones de los jugadores y el tablero.

*   **Variables Públicas / Serializadas:**
    *   `[SerializeField] int manyPlayers`: Define el número esperado de jugadores en la partida. Se utiliza para dimensionar el arreglo de jugadores y en la lógica de inicialización.
    *   `[SerializeField] GameObject player`: Una referencia a un prefab de `GameObject` que contiene el componente `Figther`. Se usa para instanciar nuevos jugadores si no se encuentran suficientes en la escena.
    *   `Figther[] players`: Un arreglo que almacena referencias a todos los objetos `Figther` (jugadores) que participan en la partida.
    *   `[SerializeField] int playerTurn`: Un índice que indica qué jugador tiene el turno actualmente dentro del arreglo `players`.
    *   `[SerializeField] SetMoments setMoments`: La variable más crucial del script, ya que define la fase actual en la que se encuentra el juego. El método `Update` utiliza esta variable para ejecutar la lógica correspondiente a la fase.
    *   `[SerializeField] public int maxDice`: El valor máximo que puede tener un dado, probablemente usado para el movimiento o acciones basadas en dados.
    *   `[SerializeField] int initialLives`: El número de vidas con las que cada jugador comienza la partida.
    *   `public CombatType combatType`: El tipo de combate activo para el turno actual, que puede influir en la resolución de las batallas de cartas.
    *   `public static Combatjudge combatjudge`: La referencia estática al patrón Singleton de este script, permitiendo que otros componentes accedan fácilmente a esta instancia central.

*   **Métodos Principales:**
    *   `void Start()`: Método del ciclo de vida de Unity. Se ejecuta una vez al inicio.
        *   Establece el patrón Singleton: si no hay una instancia de `Combatjudge`, esta se convierte en la instancia global; de lo contrario, se destruye para evitar duplicados.
        *   Inicializa `setMoments` a `SetMoments.Loop` para que el primer `Update` comience el ciclo de juego.
        *   Busca o instancia objetos `Figther` para poblar el arreglo `players`.
        *   Asigna vidas iniciales (`initialLives`), un identificador visual (`visualFigther`), un índice de jugador (`indexFigther`) y una roca inicial (`initialStone`) a cada `Figther`.
        ```csharp
        if (combatjudge == null)
        {
            combatjudge = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // ... inicialización de jugadores ...
        ```
    *   `void Update()`: Método del ciclo de vida de Unity. Se ejecuta en cada frame.
        *   Monitorea la entrada del teclado para números (0-9) y llama a `AsignarNumeros`, lo que podría ser una función de depuración o de asignación dinámica de roles visuales a los jugadores.
        *   Contiene la lógica principal de la máquina de estados del juego, utilizando un `switch` para gestionar las transiciones y acciones de cada `SetMoments`. Por ejemplo:
            *   `PickCard`: Verifica si todos los jugadores han seleccionado su carta. Si es así, transiciona a `Reveal`.
            *   `Result`: Recopila las cartas seleccionadas por los jugadores, ejecuta `IndvCombat` para cada par de cartas, calcula el resultado final para cada jugador (ganancia/pérdida de vidas) y transiciona a `Loop`.
            *   `Loop`: Prepara el siguiente turno (avanza `playerTurn`), permite a los jugadores reponer y "lanzar" cartas, y transiciona a `PickDice` para iniciar la fase del dado.
        ```csharp
        switch (setMoments)
        {
            case SetMoments.PickCard:
                // ... lógica para esperar selección de carta ...
                if (all) setMoments = SetMoments.Reveal;
                break;
            case SetMoments.Result:
                // ... lógica de resolución de combate ...
                setMoments = SetMoments.Loop;
                break;
            // ... otras fases ...
        }
        ```
    *   `public Results IndvCombat(Card one, Card two)`: Este método privado crucial determina el resultado de un combate uno a uno entre dos cartas.
        *   Primero, maneja casos donde alguna carta es nula (resultado `draw`).
        *   Si `combatType` no es `full`, aplica una regla donde solo las cartas del `combatType` específico pueden ganar si los elementos son diferentes.
        *   Para el combate general (`CombatType.full`) o cuando los elementos son iguales/neutrales bajo un `combatType` específico, compara los elementos de las cartas usando lógica modular para determinar ventajas (simulando un "piedra-papel-tijera" elemental).
        *   Si los elementos son neutrales entre sí (o iguales), se resuelve la batalla comparando los valores numéricos de las cartas.
    *   `public void Roled(int value)`: Es llamado cuando un dado ha sido lanzado. Recibe el `value` del dado. Calcula qué `RockBehavior` (rocas) deben "brillar" (hacerse seleccionables) y transiciona el juego a `SetMoments.GlowRock`.
    *   `public void ArriveAtRock()`: Se invoca cuando el token de un jugador llega a una `RockBehavior`.
        *   Determina cuántos jugadores están en la misma roca para establecer `playersFigthing`.
        *   Basado en la `inscription` de la roca, establece la siguiente fase del juego (`SelecCombat` si la roca permite elegir tipo de combate, o `PickCard` si ya define el `combatType`).
    *   `public void MoveToRock(RockBehavior rocker)`: Inicia el movimiento del token del jugador actual a la `rocker` especificada y establece la fase del juego a `SetMoments.MoveToRock`.
    *   `public bool pickElement(Element element)`: Permite al jugador seleccionar un `Element` para el `combatType` si el juego está en la fase `SetMoments.SelecCombat`. Si la selección es exitosa, transiciona a `SetMoments.PickCard`.
    *   `public SetMoments GetSetMoments()`: Un método getter para obtener el estado actual del juego (`setMoments`).
    *   `public int GetPlayersFigthing()`: Devuelve un entero (probablemente un bitmask) indicando qué jugadores están activos en el combate actual.
    *   `public bool FocusONTurn()`: Devuelve `true` si el jugador del turno actual tiene su `visualFigther` establecido en `1`, lo que podría indicar que es el jugador enfocado en la interfaz de usuario.

*   **Lógica Clave:**
    La lógica central del `Combatjudge` reside en su implementación de una **máquina de estados finitos** controlada por la variable `setMoments`. El método `Update` es el corazón de esta máquina, evaluando el estado actual y ejecutando las acciones pertinentes antes de transicionar al siguiente estado. Este diseño asegura un flujo de juego ordenado y predecible. La resolución de combate en `IndvCombat` es otro algoritmo clave que combina reglas elementales predefinidas (tipo "piedra, papel o tijera" con ventajas circulares) y comparación de valores numéricos de las cartas para determinar el ganador de cada enfrentamiento individual. La inicialización y gestión de los jugadores en el método `Start` también es fundamental para configurar la partida correctamente.

## 3. Dependencias y Eventos
*   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`.

*   **Eventos (Entrada):**
    *   Este script directamente lee la entrada del teclado (`Input.GetKeyDown`) en el método `Update` para propósitos de control o depuración (`AsignarNumeros`).
    *   Depende de llamadas externas de otros sistemas del juego para avanzar sus estados, por ejemplo:
        *   `Roled()`: Probablemente llamado por un sistema de dados cuando se ha completado una tirada.
        *   `ArriveAtRock()`: Invocado por un sistema de movimiento cuando un jugador llega a una roca.
        *   `MoveToRock()`: Llamado por un sistema de selección de movimiento.
        *   `pickElement()`: Invocado por un sistema de UI o selección de elemento.

*   **Eventos (Salida):**
    Este script no expone eventos públicos (`UnityEvent` o `Action`) a los que otros scripts puedan suscribirse. En cambio, opera mediante la modificación directa de propiedades de otros objetos (ej. `players[i].setPlayerLive()`, `rocker.shiny = true`) y el cambio de su propio estado interno (`setMoments`), lo que implica que otros sistemas deben "observar" estos cambios o interactuar directamente con los métodos públicos de `Combatjudge` para reaccionar.