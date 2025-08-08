# `RockBehavior.cs`

## 1. Propósito General
El script `RockBehavior` gestiona el comportamiento y la representación de una "roca" individual en el entorno de juego. Cada roca es un punto estratégico en el mapa donde los jugadores (`PlayerToken`) pueden posicionarse, y puede tener una "inscripción" elemental. Este script interactúa principalmente con el sistema `PlayZone` para su posicionamiento y con el `Combatjudge` para la lógica de combate y movimiento de los jugadores.

## 2. Componentes Clave

### `Inscription` (Enum)
- **Descripción:** Este `enum` define los diferentes tipos de "inscripciones" o elementos que una roca puede tener. Estas inscripciones probablemente influyen en la estrategia de juego o en las habilidades de los personajes.
- **Valores:**
    - `fire`: Fuego (0)
    - `earth`: Tierra (1)
    - `water`: Agua (2)
    - `air`: Aire (3)
    - `duel`: Duelo (4)
    - `empty`: Vacío (5)
    - `pick`: Selección (6)

### `RockBehavior` (Clase)
- **Descripción:** Esta clase es un `MonoBehaviour` que se adjunta a un objeto de juego para representar una roca. Se encarga de su inicialización visual, la detección de clics, la gestión de los jugadores que están sobre ella, y la lógica para encontrar rocas vecinas. Su orden de ejecución predeterminado es `-2`, lo que significa que se inicializa antes que la mayoría de los otros scripts de Unity.

- **Variables Públicas / Serializadas:**
    - `public PlayZone father;`: Una referencia al objeto `PlayZone` que actúa como contenedor o "padre" de esta roca. Es crucial para el posicionamiento inicial y para acceder a las rocas vecinas.
    - `[SerializeField] private Sprite[] sprite;`: Un arreglo de `Sprite`s que se utiliza para mostrar la imagen correspondiente a cada tipo de `Inscription`. Se asigna en el Inspector de Unity.
    - `[SerializeField] public Inscription inscription = Inscription.empty;`: El tipo de inscripción elemental asignado a esta roca específica. También se configura en el Inspector.
    - `public Vector3 direction = Vector3.forward;`: Un vector que se usa para calcular la posición de la roca respecto a su `father`.
    - `public float angle = 0;`: Un ángulo que se utiliza para calcular la rotación de la roca.
    - `public int numbchild = 0;`: Un índice que representa la posición de esta roca dentro de la colección de rocas de su `father`, utilizado para la lógica de vecinos.
    - `public bool shiny = false;`: Un flag booleano que controla si la roca debe mostrarse "brillante" (resaltada visualmente). Esto suele indicar que la roca es un destino válido para el movimiento de un jugador.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se llama una vez al inicio. Inicializa las referencias a los componentes `SpriteRenderer` (`simbol` para la inscripción y `itself` para la roca en sí). Establece el sprite del símbolo basándose en la `inscription` de la roca. Si tiene una referencia a `father`, calcula y aplica la posición, escala y rotación de la roca en relación con el `PlayZone` padre.

    - `void Update()`: Este método del ciclo de vida de Unity se llama una vez por fotograma. Verifica el estado del juego a través de `Combatjudge.combatjudge.GetSetMoments()`. Si el estado actual no es `SetMoments.GlowRock`, el flag `shiny` se establece en `false`. Luego, actualiza el color de la propia roca (`itself.color`) a amarillo si `shiny` es `true`, o a negro si es `false`, proporcionando un feedback visual de si la roca está activa o no.

    - `private void OnMouseDown()`: Un método de callback de Unity que se invoca cuando se hace clic con el botón del mouse sobre el collider de este objeto. Si la roca está marcada como `shiny`, significa que es un destino válido para una acción. En ese caso, busca el `Combatjudge` en la escena y le notifica que se intente un movimiento hacia esta roca (`MoveToRock(this)`).

    - `public RockBehavior[] getNeighbor(int al)`: Este método calcula y devuelve un arreglo de dos objetos `RockBehavior` que son "vecinos" de la roca actual. `al` representa un desplazamiento. La lógica utiliza el `numbchild` (índice de la roca) y `father.many` (total de rocas en el `PlayZone`) para calcular los índices de los vecinos en un arreglo circular, envolviendo los índices si superan los límites.

    - `public void AddPlayer(PlayerToken token)`: Añade un `PlayerToken` al arreglo interno `playersOn`, que rastrea qué jugadores están actualmente en esta roca. Si el arreglo es nulo, lo inicializa; de lo contrario, crea un nuevo arreglo de mayor tamaño, copia los jugadores existentes y añade el nuevo token.

    - `public void RemovePlayer(PlayerToken token)`: Elimina un `PlayerToken` específico del arreglo `playersOn`. Recorre el arreglo para encontrar y "nular" las instancias del token a remover, luego construye un nuevo arreglo más pequeño excluyendo los elementos nulos.

    - `public bool manyOn()`: Devuelve `true` si hay más de un `PlayerToken` actualmente en esta roca (es decir, `playersOn.Length > 1`). Devuelve `false` en caso contrario o si no hay jugadores.

    - `public int GetPlayersOn()`: Calcula y devuelve un entero que actúa como un bitmask, representando la presencia de diferentes jugadores en la roca. Para cada `PlayerToken` en la roca, se suma `2` elevado a la potencia de `player.indexFigther` del token. Esto permite codificar la presencia de múltiples jugadores en un solo entero.

    - `public int ManyPlayerOn()`: Devuelve la cantidad exacta de `PlayerToken`s que se encuentran actualmente en esta roca, es decir, el tamaño del arreglo `playersOn`.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`, pero su funcionalidad depende de la presencia de un componente `SpriteRenderer` en el mismo GameObject y en su primer hijo. También asume que el GameObject padre tiene un componente `PlayZone` si la variable `father` no es nula.

- **Eventos (Entrada):**
    - Se suscribe implícitamente al evento de entrada del mouse `OnMouseDown()` a través del sistema de eventos de Unity, respondiendo a los clics del usuario sobre la roca.

- **Eventos (Salida):**
    - Este script invoca el método `MoveToRock()` del `Combatjudge` cuando se hace clic en una roca "brillante". Esto actúa como un evento que notifica al sistema de combate sobre la intención de movimiento del jugador.
    - No invoca `UnityEvent`s o `Action`s explícitamente definidos en este script.