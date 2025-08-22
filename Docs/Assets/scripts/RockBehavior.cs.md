# `RockBehavior.cs`

## 1. Propósito General
Este script gestiona el comportamiento de las "rocas" individuales en el juego, que actúan como nodos en un tablero circular. Se encarga de su inicialización visual (sprite, color, posición), su capacidad para ser interactuables bajo ciertas condiciones y la gestión de los "PlayerToken" (fichas de jugador) que se encuentran sobre ellas, facilitando la interacción entre jugadores y el sistema de combate.

## 2. Componentes Clave

### `Inscription`
- **Descripción:** Este `enum` define los diferentes tipos de "inscripciones" o elementos que una roca puede tener. Estos representan atributos elementales o tipos de acción, cruciales para la estrategia del juego. Incluye valores como `fire`, `earth`, `water`, `air`, `pick`, `duel` y `empty`.

### `RockBehavior`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, es el controlador principal para cada objeto de roca en el escenario. Define las propiedades visuales y de interacción de una roca, su relación con la zona de juego (`PlayZone`) y cómo gestiona los jugadores que la ocupan. Utiliza `[DefaultExecutionOrder(-2)]` para asegurar que se inicialice antes que la mayoría de otros scripts.

- **Variables Públicas / Serializadas:**
    - `public PlayZone father;`: Una referencia al script `PlayZone` que actúa como el contenedor o la zona de juego a la que pertenece esta roca. Es fundamental para posicionar la roca y acceder a propiedades del tablero.
    - `[SerializeField] private Sprite[] sprite;`: Un array de `Sprite` que almacena las diferentes imágenes para representar las inscripciones de las rocas. El sprite visual de la roca se selecciona de este array.
    - `[SerializeField] public Inscription inscription = Inscription.empty;`: La inscripción específica de esta roca. Se serializa para ser configurable desde el Inspector de Unity y determina el sprite y color inicial de la roca.
    - `[SerializeField] private Color[] colors;`: Un array de `Color` que almacena los colores correspondientes a cada tipo de inscripción. El color de la roca se selecciona de este array.
    - `public Vector3 direction = Vector3.forward;`: Define la dirección inicial para el posicionamiento de la roca respecto a su `father`.
    - `public float angle = 0;`: El ángulo de rotación inicial de la roca, usado en combinación con `direction` para su posicionamiento en un patrón circular.
    - `public int numbchild = 0;`: Un índice que representa la posición de esta roca dentro de la colección de rocas de su `father`. Es clave para calcular vecinos.
    - `public bool shiny = false;`: Un flag que controla si la roca debe mostrar un efecto visual de "brillo". Se usa para indicar que la roca es interactuable o relevante en un momento dado del juego.

- **Métodos Principales:**
    - `void Start()`: Este método se llama una vez al inicio, antes de la primera actualización del frame. Se encarga de inicializar las referencias a los componentes `SpriteRenderer` hijos (el símbolo de la inscripción y el brillo) y el `SpriteRenderer` de la propia roca. Asigna el sprite y el color de la roca basándose en su `inscription`. Si `father` está asignado, calcula y establece la posición, escala y rotación de la roca para que se ajuste al diseño circular de la `PlayZone`.
    - `void Update()`: Se llama una vez por frame. Su función principal es controlar la visibilidad del efecto de brillo (`shyn`). Si el estado actual del juego (obtenido de `Combatjudge`) no es `GlowRock`, el brillo se desactiva.
    - `private void OnMouseDown()`: Un callback de Unity que se invoca cuando se hace clic con el ratón sobre el collider 2D de la roca. Si la roca está marcada como `shiny` y el `Combatjudge` indica que es el turno del jugador enfocado (`FocusONTurn()`), entonces se notifica al `Combatjudge` para que procese el movimiento del jugador hacia esta roca.
    - `public RockBehavior[] getNeighbor(int al)`: Este método devuelve un array de dos `RockBehavior` que son "vecinos" de esta roca. `al` especifica el desplazamiento (número de rocas de distancia) para encontrar a los vecinos. Utiliza la propiedad `numbchild` y la cantidad total de rocas (`father.many`) para calcular los índices de los vecinos en ambas direcciones alrededor del círculo.
    - `public void AddPlayer(PlayerToken token)`: Añade un `PlayerToken` al array `playersOn`, que rastrea qué jugadores están actualmente sobre esta roca. El método gestiona el redimensionamiento dinámico del array para acomodar el nuevo jugador.
    - `public void RemovePlayer(PlayerToken token)`: Elimina un `PlayerToken` específico del array `playersOn`. Recorre el array para encontrar el token a eliminar y luego crea un nuevo array sin él, gestionando el redimensionamiento.
    - `public bool manyOn()`: Retorna `true` si hay más de un `PlayerToken` sobre esta roca, y `false` en caso contrario.
    - `public int GetPlayersOn()`: Calcula un valor entero combinando los índices de los jugadores (`p.player.indexFigther`) que están sobre esta roca. Parece ser un método para generar una firma única basada en los jugadores presentes, posiblemente para lógica de combate o puntuación.
    - `public int ManyPlayerOn()`: Retorna el número total de `PlayerToken` que se encuentran sobre esta roca.

- **Lógica Clave:**
    La inicialización de las rocas se realiza en `Start`, donde cada roca se posiciona dinámicamente en un círculo en relación con su `PlayZone` padre. La interacción con las rocas (a través de `OnMouseDown`) está condicionada por el estado `shiny` de la roca y el estado actual del combate, lo que sugiere un sistema de turnos o fases donde solo ciertas rocas son seleccionables. La gestión de `PlayerToken` en los métodos `AddPlayer` y `RemovePlayer` es fundamental para saber qué personajes ocupan cada roca y para implementar mecánicas de juego que dependen de la ubicación de los jugadores.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, asume la presencia de un `SpriteRenderer` en el propio objeto de juego y en sus dos primeros hijos (`transform.GetChild(0)` y `transform.GetChild(1)`).
- **Eventos (Entrada):**
    - Se suscribe implícitamente al evento de clic del ratón (`OnMouseDown`) proporcionado por Unity para interactuar con el jugador.
- **Eventos (Salida):**
    - Este script invoca el método `MoveToRock()` en una instancia de `Combatjudge` (obtenida mediante `FindFirstObjectByType<Combatjudge>()`) cuando una roca `shiny` es clicada y es el turno del jugador enfocado. Esto notifica al sistema de combate sobre la acción del jugador.