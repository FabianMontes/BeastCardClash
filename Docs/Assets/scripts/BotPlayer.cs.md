# `BotPlayer.cs`

## 1. Propósito General
El script `BotPlayer.cs` es el componente central que gestiona la inteligencia artificial de un jugador bot dentro del juego **Beast Card Clash**. Su rol principal es simular el comportamiento de un jugador humano, tomando decisiones automáticamente en diferentes fases del combate, como la selección de cartas, el lanzamiento de dados y la elección de movimientos o elementos de ataque. Este script interactúa fuertemente con sistemas como `Combatjudge` para determinar la fase actual del juego y el turno, así como con los componentes de `Figther` y `HandCard` para manipular las acciones del bot.

## 2. Componentes Clave

### `BotPlayer`
-   **Descripción:** `BotPlayer` es una clase que hereda de `MonoBehaviour`, lo que le permite ser adjuntada a un GameObject en la escena de Unity y participar en el ciclo de vida del juego. Su función principal es implementar la lógica de decisión y acción para un jugador controlado por la IA, adaptándose a las diferentes etapas de un turno de combate. Utiliza un sistema de temporizadores y banderas para simular "tiempos de pensamiento" y coordinar sus acciones con el flujo del juego.

-   **Variables Públicas / Serializadas:**
    *   `Figther figther`: Esta variable almacena una referencia al componente `Figther` del mismo GameObject. `Figther` representa al personaje del bot en el combate y es esencial para que `BotPlayer` acceda a información relevante del estado del bot, como si está en combate (`IsFigthing()`) o la carta que ha elegido (`getPicked()`).
    *   `Transform hand`: Representa el GameObject que contiene las cartas disponibles en la mano del bot. El script espera que las cartas (`HandCard`) sean hijos directos de este `Transform`. Se inicializa en `Start()` buscando un `Transform` específico dentro de la jerarquía del GameObject.
    *   `float time`: Registra el `Time.time` (el tiempo transcurrido desde el inicio del juego) en el momento en que el bot inicia un periodo de "pensamiento" o espera. Se utiliza para calcular cuánto tiempo ha transcurrido desde ese punto.
    *   `float total`: Define la duración total del tiempo de "pensamiento" o espera que el bot debe cumplir antes de realizar una acción. Este valor se establece aleatoriamente dentro de un rango predefinido para cada tipo de acción, añadiendo una simulación de variabilidad en el comportamiento del bot.
    *   `bool picking`: Una bandera de estado que se utiliza para controlar el flujo de la lógica del bot y evitar que ciertas secciones de código se ejecuten repetidamente en cada frame. Por ejemplo, se activa al inicio de un período de "pensamiento" para la selección de cartas o elementos de ataque, y se desactiva una vez que la acción se ha completado, garantizando que el bot solo tome una decisión por fase.
    *   `RockBehavior[] rocks`: Un arreglo que almacena referencias a objetos `RockBehavior`. Estos objetos representan ubicaciones posibles en el tablero a las que el bot puede moverse. Este arreglo se llena cuando el método `PickRock` es invocado desde otro sistema.

-   **Métodos Principales:**
    *   `void Start()`: Este método se invoca una vez al inicio del ciclo de vida del script, cuando el objeto está habilitado. Su propósito es inicializar las referencias a los componentes necesarios. Obtiene el componente `Figther` adjunto al mismo GameObject y establece la referencia a la mano del bot (`hand`) navegando por la jerarquía de Transforms: `transform.GetChild(0).GetChild(1)`. Finalmente, inicializa la bandera `picking` a `false`.

    *   `void Update()`: Se llama una vez por frame y es el corazón de la lógica de decisión del bot. Contiene una compleja cadena de sentencias `if-else if` que reacciona a los diferentes estados de juego (`SetMoments`) gestionados por el `Combatjudge` y al turno actual. En cada fase, `Update` determina si el bot debe realizar una acción (como elegir una carta, lanzar un dado o seleccionar un tipo de ataque), inicia un temporizador de "pensamiento" si es necesario y ejecuta la acción correspondiente una vez que el tiempo ha transcurrido. La bandera `picking` es crucial aquí para gestionar el estado y evitar repeticiones.

    *   `public void PickRock(RockBehavior[] rocks)`: Este método se llama externamente para informar al bot sobre las rocas (ubicaciones) disponibles a las que puede moverse. Desactiva la bandera `picking`, inicia un temporizador de "pensamiento" con un tiempo aleatorio entre 1 y 2 segundos, y almacena el arreglo de `RockBehavior` recibido para su posterior procesamiento.

    *   `public void ThinkingRocks()`: Se invoca después de que el bot ha sido informado sobre las rocas disponibles a través de `PickRock`. Este método verifica si el tiempo de "pensamiento" establecido en `PickRock` ha transcurrido. Si es así, elige una roca aleatoria del arreglo `rocks` y notifica al `Combatjudge` que el bot desea moverse a esa roca específica.

-   **Lógica Clave:**
    La lógica del `BotPlayer` se basa en una máquina de estados implícita definida por los `SetMoments` del `Combatjudge` y la variable `picking`. El bot "piensa" por un tiempo aleatorio antes de cada acción crucial, como elegir una carta o un tipo de ataque, lo que se simula mediante el uso de las variables `time` y `total`.

    Para la **selección de cartas**, el bot intenta elegir una carta al azar de su mano. Si la carta elegida no es válida o seleccionable (según `card.isClickable()`), el bot itera por las siguientes cartas en su mano hasta encontrar una que pueda seleccionar, con un límite de 100 intentos para evitar bucles infinitos. Una vez que encuentra una carta válida, invoca `SelectedCard()` en ella.

    Para la **selección de elementos de ataque**, el bot analiza las cartas en su mano. Recorre todas sus cartas, clasifica el "elemento" de cada una (asumiendo que hay 4 tipos de elementos) y cuenta cuántas cartas tiene de cada tipo. Luego, selecciona el elemento del que tiene la mayor cantidad de cartas disponibles, intentando maximizar su potencial ofensivo. Esta decisión se comunica al sistema a través de `GetComponentInChildren<SelectType>().PickElement()`.

    En las fases de **lanzamiento de dado** y **movimiento entre rocas**, el bot simplemente espera un tiempo aleatorio y luego ejecuta la acción correspondiente (lanzar/dejar de lanzar el dado o moverse a una roca aleatoria).

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    El script `BotPlayer` espera y depende de la existencia de un componente `Figther` en el mismo GameObject donde está adjunto. El método `Start()` intenta obtener esta referencia mediante `GetComponent<Figther>()`.

-   **Eventos (Entrada):**
    `BotPlayer` no se suscribe explícitamente a eventos de `UnityEvent` o `Action` utilizando `AddListener`. En su lugar, su comportamiento reactivo se basa en:
    *   La invocación constante del método `Update()` por el ciclo de vida de Unity.
    *   Consultas directas al estado global del juego a través de `Combatjudge.combatjudge.GetSetMoments()` y `Combatjudge.combatjudge.turn()`, lo que le permite reaccionar a los cambios de fase y turno del juego.
    *   Llamadas directas a sus métodos `public void PickRock(RockBehavior[] rocks)` y `public void ThinkingRocks()` desde otros sistemas (presumiblemente el `Combatjudge` o un sistema de manejo de turnos) cuando el bot necesita decidir su movimiento en el tablero.

-   **Eventos (Salida):**
    `BotPlayer` no emite eventos explícitos (`UnityEvent`, `Action`) para notificar a otros sistemas. En cambio, interactúa con ellos directamente mediante la invocación de métodos:
    *   Llama `card.SelectedCard()` en el componente `HandCard` de la carta elegida, lo que probablemente desencadena la lógica de selección de la carta.
    *   Invoca `FindFirstObjectByType<dice>().Roll()` y `FindFirstObjectByType<dice>().Unroll()` para controlar el comportamiento del dado en el juego.
    *   Llama `GetComponentInChildren<SelectType>().PickElement(big)` para informar al sistema de selección de tipo de ataque sobre el elemento elegido.
    *   Finalmente, notifica al `Combatjudge` sobre las acciones del bot, como el movimiento a una roca específica, mediante `Combatjudge.combatjudge.MoveToRock(rocks[chosenRockIndex])`.