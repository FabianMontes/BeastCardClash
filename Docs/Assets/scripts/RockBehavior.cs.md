# `RockBehavior.cs`

## 1. Propósito General
Este script gestiona el comportamiento individual de los objetos "roca" dentro del juego, que representan nodos en el tablero donde los `PlayerToken` pueden posicionarse. Se encarga de su inicialización visual y posicional, la gestión de jugadores sobre ellas y la interacción del usuario para movimientos durante el combate. Interactúa principalmente con el sistema `Combatjudge` y la `PlayZone` padre.

## 2. Componentes Clave

### `Inscription` (Enum)
- **Descripción:** Una enumeración que define los diferentes tipos de "inscripciones" o elementos que una roca puede tener. Estos elementos son cruciales para las mecánicas de juego relacionadas con el combate o las habilidades.
- **Valores:**
    - `fire`: Fuego
    - `earth`: Tierra
    - `water`: Agua
    - `air`: Aire
    - `duel`: Duelo (probablemente un tipo especial de interacción)
    - `pick`: Selección/Recolección
    - `empty`: Vacía (sin inscripción)

### `RockBehavior` (Clase)
- **Descripción:** Hereda de `MonoBehaviour` y controla el comportamiento de una roca individual en el tablero de juego. Cada instancia de `RockBehavior` es un nodo en el que los `PlayerToken` pueden moverse y que puede tener una "inscripción" elemental. Utiliza `DefaultExecutionOrder(-2)` para asegurar que su método `Start` se ejecute antes que la mayoría de otros scripts.

- **Variables Públicas / Serializadas:**
    - `public PlayZone father;`: Una referencia al script `PlayZone` padre. Este es el componente que organiza múltiples rocas, y esta referencia se utiliza para posicionar la roca y obtener información del entorno.
    - `[SerializeField] private Sprite[] sprite;`: Un array de `Sprite` que almacena las representaciones visuales de las diferentes `Inscription` para ser mostradas en la roca.
    - `[SerializeField] public Inscription inscription = Inscription.empty;`: Define el tipo elemental de inscripción que posee esta roca específica. Es configurable desde el Inspector de Unity.
    - `public Vector3 direction = Vector3.forward;`: La dirección relativa a su `father` para su posicionamiento inicial.
    - `public float angle = 0;`: El ángulo utilizado para el posicionamiento y rotación inicial de la roca con respecto a su `father`.
    - `public int numbchild = 0;`: Un índice que identifica esta roca dentro de la colección de rocas gestionadas por su `father` (`PlayZone`). Es crucial para determinar sus vecinos.
    - `public bool shiny = false;`: Un flag booleano que controla si la roca debe mostrarse visualmente "brillante" (resaltada). Se utiliza para indicar al jugador que la roca es interactuable o un posible destino de movimiento.

- **Métodos Principales:**
    - `void Start()`: Este método de ciclo de vida de Unity se ejecuta una vez al inicio. Inicializa las referencias a los `SpriteRenderer` del símbolo y de la roca misma. Asigna el sprite correcto para la inscripción de la roca. Si tiene una `father` asignada, calcula y establece la posición, escala y rotación de la roca en relación con la `PlayZone` central, creando una disposición circular o lineal.
    - `void Update()`: Se ejecuta cada frame. Actualiza el color de la roca (`itself.color`). Si el momento actual del combate no es `SetMoments.GlowRock` (indicando que las rocas interactivas deben brillar), `shiny` se establece en `false`, lo que hace que la roca vuelva a su color "normal" (`Color.black`). Si `shiny` es `true`, la roca se vuelve amarilla.
    - `private void OnMouseDown()`: Este método de evento de Unity se invoca cuando el usuario hace clic con el ratón sobre el collider 2D de la roca. Si la roca está marcada como `shiny` (es decir, es un destino de movimiento válido), invoca el método `MoveToRock` del `Combatjudge` para iniciar el proceso de movimiento del jugador hacia esta roca.
    - `public RockBehavior[] getNeighbor(int al)`: Calcula y devuelve un array de dos `RockBehavior`s que son vecinos de esta roca. `al` determina qué tan lejos están los vecinos (por ejemplo, `al=1` para vecinos inmediatos). Utiliza la variable `numbchild` y la cantidad total de rocas (`father.many`) para calcular las posiciones de los vecinos en una disposición circular.
    - `public void AddPlayer(PlayerToken token)`: Añade un `PlayerToken` al array `playersOn`, que rastrea qué jugadores están actualmente sobre esta roca. Si el array es nulo o está lleno, se recrea un nuevo array de mayor tamaño y se copian los elementos existentes junto con el nuevo jugador.
    - `public void RemovePlayer(PlayerToken token)`: Elimina un `PlayerToken` específico del array `playersOn`. Recorre el array, marca el jugador a eliminar como `null`, y luego reconstruye un nuevo array sin los jugadores nulos, manteniendo solo los jugadores restantes.
    - `public bool manyOn()`: Retorna `true` si hay más de un `PlayerToken` actualmente en esta roca; de lo contrario, `false`. Esto es útil para determinar si hay un "duelo" o una situación de múltiples jugadores en la misma roca.
    - `public int GetPlayersOn()`: Calcula y devuelve un entero que representa una combinación única de los jugadores actualmente en la roca. Utiliza `Mathf.Pow(2, p.player.indexFigther)` para cada jugador, lo que sugiere el uso de un bitmask para identificar la presencia de jugadores específicos.
    - `public int ManyPlayerOn()`: Retorna el número total de `PlayerToken` objetos que están actualmente sobre esta roca.

- **Lógica Clave:**
    La inicialización de las rocas es dinámica, calculando su posición y rotación basándose en el componente `PlayZone` padre. El sistema de `shiny` permite resaltar las rocas interactivas, y la detección de clics (`OnMouseDown`) es el disparador para la interacción del jugador. La gestión de `PlayerToken`s sobre la roca se realiza mediante la adición y eliminación dinámica de elementos en un array, lo que permite que múltiples jugadores ocupen la misma roca. Los métodos para obtener vecinos y el estado de los jugadores sobre la roca son fundamentales para la lógica de movimiento y combate del juego.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script no utiliza la anotación `[RequireComponent]`, pero asume la presencia de un `SpriteRenderer` en el GameObject al que está adjunto y en su primer hijo.

- **Eventos (Entrada):**
    - Se suscribe al evento intrínseco de Unity `OnMouseDown()`, que se activa cuando el usuario hace clic con el ratón sobre el collider 2D del objeto.

- **Eventos (Salida):**
    - Este script no invoca `UnityEvent`s ni `Action`s personalizados. En su lugar, llama directamente al método `MoveToRock()` de la instancia de `Combatjudge` encontrada en la escena al hacer clic.