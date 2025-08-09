# `Figther.cs`

## 1. Propósito General
Este script (`Figther.cs`) es un componente central para la gestión de cada combatiente o "jugador" en **Beast Card Clash**. Su rol principal es encapsular los atributos fundamentales de un personaje (como vida, especie, y datos de cartas), controlar su representación visual en el juego, y manejar las interacciones relacionadas con su mazo y mano de cartas, así como su estado en el combate. Interactúa con sistemas clave como el controlador de combate (`Combatjudge`) y el manejo de tokens de jugador (`PlayerToken`).

## 2. Componentes Clave

### `Specie`
- **Descripción:** `Specie` es una enumeración (`enum`) que define los tipos de animales o especies que un combatiente puede representar en el juego. Cada valor de este `enum` (`chameleon`, `bear`, `snake`, `frog`) corresponde a una categoría específica, lo que podría influir en las habilidades, debilidades o sinergias del combatiente en el juego.

### `Figther`
- **Descripción:** La clase `Figther` es un `MonoBehaviour`, lo que significa que debe adjuntarse a un `GameObject` en la jerarquía de Unity para funcionar. Sirve como el controlador de la lógica y los datos para una unidad de combatiente individual. Su comportamiento de ejecución está establecido en `DefaultExecutionOrder(0)`, lo que asegura que sus métodos del ciclo de vida (como `Start` y `Update`) se ejecuten muy temprano, antes que la mayoría de otros scripts, lo cual es útil para asegurar una inicialización adecuada.

- **Variables Públicas / Serializadas:**
    *   `figtherLive` (`[SerializeField] int`): Esta variable almacena la cantidad actual de puntos de vida o salud del combatiente. Su valor es editable directamente desde el Inspector de Unity.
    *   `specie` (`[SerializeField] Specie`): Define la especie del combatiente, seleccionable a través del `enum Specie`. Es configurable en el Inspector.
    *   `deckSize` (`[SerializeField] int`): Representa el número total de cartas que componen el mazo de este combatiente. Se utiliza para instanciar el mazo inicial.
    *   `handSize` (`[SerializeField] int`): Indica la cantidad máxima de cartas que el combatiente puede tener simultáneamente en su mano. Por defecto, su valor inicial es 6.
    *   `avalaibleCard` (`public int`): Un contador público que parece indicar la disponibilidad de acciones o la finalización de una fase de elección de cartas. Su valor se comprueba en el método `Update` para ciertas lógicas de juego.
    *   `tokenPrefab` (`[SerializeField] GameObject`): Una referencia al prefab de `GameObject` que se utilizará para instanciar el token del jugador en el tablero.
    *   `cardPrefab` (`[SerializeField] GameObject`): Una referencia al prefab de `GameObject` que se usará para instanciar las cartas que componen el mazo del combatiente.
    *   `playerToken` (`[SerializeField] public PlayerToken`): Una referencia a la instancia del componente `PlayerToken` que gestiona la posición visual y la interacción del combatiente en el mapa de juego. Se puede asignar en el Inspector o se instancia en `Start`.
    *   `visualFigther` (`[SerializeField] public int`): Un índice entero que determina cuál de los objetos hijos del `GameObject` actual (que representan los diferentes modelos visuales del combatiente) debe estar activo y visible.
    *   `indexFigther` (`[SerializeField] public int`): Un identificador numérico único para este combatiente, utilizado en la lógica de combate para distinguirlo de otros jugadores.
    *   `initialStone` (`[SerializeField] public RockBehavior`): Una referencia a un componente `RockBehavior` que define la posición inicial en el tablero donde el `playerToken` de este combatiente será colocado al inicio.

- **Métodos Principales:**
    *   `void Start()`: Este método del ciclo de vida de Unity se ejecuta una única vez cuando el script se activa por primera vez.
        *   **Funcionalidad:** Inicializa la apariencia visual del combatiente activando el `GameObject` hijo correspondiente al índice `visualFigther` y desactivando los demás posibles modelos visuales (se espera que estén en los índices 1, 2, 3, 4). Si `playerToken` no ha sido asignado en el Inspector, instancia un `tokenPrefab`, lo asigna como `playerToken`, y vincula este `Figther` a él. Finalmente, crea el mazo de cartas instanciando `deckSize` copias de `cardPrefab` como hijos del objeto que representa el mazo.
    *   `void Update()`: Este método del ciclo de vida de Unity se ejecuta en cada frame del juego.
        *   **Funcionalidad:** Gestiona la actualización del modelo visual del combatiente si el valor de `visualFigther` ha cambiado desde el último frame. Además, contiene lógica para la fase de "PickCard" (elección de cartas) del juego: si el sistema de combate (`Combatjudge`) indica que es el momento de elegir cartas y el combatiente está actualmente involucrado en la lucha (`IsFigthing()` devuelve `true`) y `avalaibleCard` es 0, entonces fuerza la revelación de todas las cartas en la mano del combatiente.
    *   `Specie GetSpecie()`: Un método de acceso público que devuelve el valor de la propiedad `specie` del combatiente.
    *   `int GetPlayerLive()`: Un método de acceso público que devuelve el valor actual de los puntos de vida (`figtherLive`) del combatiente.
    *   `void setPlayerLive(int pL)`: Permite establecer los puntos de vida del combatiente a un valor específico `pL`.
    *   `void addPlayerLive(int pL)`: Añade una cantidad `pL` a los puntos de vida actuales del combatiente.
    *   `void randomSpecie()`: Asigna aleatoriamente una de las especies definidas en el `enum Specie` al combatiente.
    *   `void movePlayer(RockBehavior rocker)`: Actualiza la `RockBehavior` a la que está asociado el `playerToken` del combatiente, moviéndolo visualmente en el tablero a la posición de la nueva roca.
    *   `Card getPicked()`: Recupera la carta que ha sido "recogida" o seleccionada del componente `HolderPlay` que se espera esté adjunto como hijo del combatiente.
    *   `void DrawCard(int index, int HandDex)`: Roba una carta específica del mazo del combatiente, identificada por su `index` en el mazo, y la asigna a una posición específica en la mano (`HandDex`), ocultando la carta original en el mazo.
    *   `void PlayCard(Card card)`: Delega la acción de jugar una carta específica al componente `HolderPlay` del combatiente.
    *   `private void DrawCard(int index)`: Esta es una sobrecarga privada de `DrawCard` que se encarga de robar una carta *aleatoria* del mazo. Busca una carta activa en el mazo, la asigna a la posición `index` de la mano y luego la desactiva en el mazo para simular que ha sido robada.
    *   `void RefillHand()`: Este método rellena la mano del combatiente. Itera sobre las posiciones de la mano y, para cada posición que esté vacía (es decir, no contiene una carta), llama a la versión privada de `DrawCard` para robar una carta aleatoria y colocarla allí.
    *   `bool IsFigthing()`: Determina si el combatiente está actualmente participando en la fase de combate activa.
        *   **Lógica:** Obtiene un valor entero de `Combatjudge.combatjudge.GetPlayersFigthing()`, el cual parece ser un bitmask o un valor codificado que indica qué jugadores están luchando. Este método itera a través de los bits de este valor para verificar si el bit correspondiente al `indexFigther` de este combatiente está activado, lo que significa que está en combate.
    *   `void ThrowCard()`: Llama a `PlayCard(null)` en el `HolderPlay` del combatiente, lo que sugiere que se usa para descartar o "lanzar" una carta sin que sea una carta específica.

- **Lógica Clave:**
    *   **Gestión de Apariencia Dinámica:** El script permite cambiar la apariencia visual del combatiente en tiempo de ejecución manipulando la activación de sus GameObjects hijos, indexados por `visualFigther`.
    *   **Inicialización y Flujo de Cartas:** `Start` se encarga de poblar el mazo inicial. Los métodos `DrawCard` (ambas versiones) y `RefillHand` gestionan el flujo de cartas del mazo a la mano, asegurando que las cartas se roben correctamente y la mano se mantenga llena según sea necesario.
    *   **Delegación a Componentes Especializados:** La clase `Figther` delega las operaciones específicas de manejo de cartas en juego (como "recoger" o "jugar" cartas) al componente `HolderPlay`, lo que ayuda a mantener una separación de responsabilidades.
    *   **Integración con el Sistema de Juego:** Interactúa directamente con el singleton `Combatjudge` para consultar el estado del juego y determinar la participación del combatiente en momentos clave del combate.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    *   Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente depende de la presencia de ciertos componentes en la escena o como hijos del `GameObject` al que está adjunto:
        *   Espera encontrar un componente `HolderPlay` entre sus hijos para gestionar las cartas jugadas y recogidas (`GetComponentInChildren<HolderPlay>()`).
        *   Requiere que los `GameObject`s que representan los modelos visuales del combatiente, el mazo y la mano estén organizados en una jerarquía específica como hijos del `GameObject` principal de `Figther` (como se ve en las llamadas a `transform.GetChild(index)`).
        *   Necesita la existencia y accesibilidad de las clases `PlayerToken`, `Card`, `HandCard`, `RockBehavior` (para `playerToken`) y, crucialmente, el singleton `Combatjudge` (`Combatjudge.combatjudge`).
-   **Eventos (Entrada):**
    *   El script `Figther` no se suscribe explícitamente a eventos de UI (como `Button.onClick`) ni a eventos de Unity (`UnityEvent` o `Action`) definidos en otros scripts. Su comportamiento principal se desencadena por los métodos de ciclo de vida de Unity (`Start`, `Update`) y por llamadas directas desde otros sistemas del juego (ej. un `GameManager` o un `Combatjudge` llamando a `setPlayerLive` o `movePlayer`).
    *   Recibe indirectamente información del estado del juego a través de consultas al `Combatjudge` (ej., `Combatjudge.combatjudge.GetSetMoments()` y `GetPlayersFigthing()`).
-   **Eventos (Salida):**
    *   Actualmente, el script `Figther` no expone ni invoca ningún `UnityEvent` o `Action` para notificar a otros sistemas sobre cambios en su estado (ej., vida modificada, carta robada, etc.). Las interacciones se manejan principalmente a través de llamadas directas a métodos o la modificación de propiedades públicas de otros componentes (`playerToken.rocky = rocker;`).