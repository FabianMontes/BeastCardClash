# `Figther.cs`

## 1. Propósito General
El script `Figther.cs` es el componente central que gestiona la lógica y el estado de un "Luchador" (Fighter) dentro del juego "Beast Card Clash". Controla atributos como la vida, la especie del animal y la gestión de su mazo y mano de cartas, interactuando directamente con sistemas clave como la gestión del combate (`Combatjudge`) y el manejo de cartas en juego (`HolderPlay`).

## 2. Componentes Clave

### `enum Specie`
- **Descripción:** Esta enumeración define los distintos tipos de "especies" de animales que un luchador puede representar. Cada miembro de la enumeración corresponde a una especie específica que podría influir en las habilidades o características del luchador en el juego.
- **Valores:** `chameleon`, `bear`, `snake`, `frog`.

### `Figther`
- **Descripción:** La clase `Figther` es un `MonoBehaviour` que representa a un jugador o personaje en el campo de batalla. Es responsable de mantener el estado del luchador, inicializar sus componentes visuales y lógicos, y manejar interacciones clave relacionadas con la vida, las cartas y el posicionamiento en el tablero. La clase utiliza una estructura de hijos en la jerarquía de Unity para organizar sus elementos visuales (modelos de personajes) y funcionales (mazo y mano de cartas).

- **Variables Públicas / Serializadas:**
    - `[SerializeField] int figtherLive`: Representa los puntos de vida actuales del luchador. Es crucial para determinar la supervivencia del personaje en combate.
    - `[SerializeField] Specie specie`: Define la especie del luchador, lo que puede afectar sus habilidades y el estilo de juego.
    - `[SerializeField] int deckSize`: El número total de cartas que componen el mazo inicial del luchador.
    - `[SerializeField] int handSize = 6`: El número máximo de cartas que el luchador puede tener en su mano simultáneamente.
    - `public int avalaibleCard`: Utilizado en el ciclo de `Update` para la lógica de mostrar cartas; su nombre sugiere que podría ser un contador de cartas disponibles o un indicador de estado.
    - `[SerializeField] GameObject tokenPrefab`: El prefab del token que representa al luchador en el tablero de juego.
    - `[SerializeField] GameObject cardPrefab`: El prefab base para crear instancias de cartas en el mazo del luchador.
    - `[SerializeField] public PlayerToken playerToken`: Una referencia a la instancia del token del jugador asociada con este luchador en el tablero. Se instancia si no está asignado.
    - `[SerializeField] public int visualFigther`: Un índice que selecciona qué modelo visual de luchador (objeto hijo) debe activarse para representar a este personaje.
    - `[SerializeField] public int indexFigther`: Un índice único que identifica a este luchador en contextos multijugador o de combate, usado por `Combatjudge`.
    - `[SerializeField] public RockBehavior initialStone`: La referencia a un objeto `RockBehavior` que define la posición inicial del token del jugador en el tablero.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio. Es fundamental para la inicialización del luchador:
        -   Gestiona la visibilidad de los modelos 3D del luchador, activando el modelo correspondiente a `visualFigther` y desactivando los demás (asume que los modelos están en hijos con índices 1 a 4).
        -   Instancia un `PlayerToken` si no hay uno asignado, lo vincula a este luchador y le asigna su `initialStone` como posición inicial en el tablero.
        -   Crea un "mazo provisional" instanciando `cardPrefab` múltiples veces (`deckSize`) como hijos de un contenedor específico dentro de la jerarquía (`transform.GetChild(0).GetChild(0)`).
    - `void Update()`: Este método del ciclo de vida se ejecuta en cada frame. Contiene lógica continua:
        -   Actualiza el modelo visual del luchador si el `visualFigther` cambia durante el juego.
        -   Verifica el estado del combate (`Combatjudge.GetSetMoments() == SetMoments.PickCard`) y si este luchador está en combate (`IsFigthing()`). Si ambas condiciones son verdaderas y `avalaibleCard` es 0, fuerza la revelación de todas las cartas en la mano del jugador.
    - `Specie GetSpecie()`: Devuelve la especie actual del luchador.
    - `int GetPlayerLive()`: Devuelve los puntos de vida actuales del luchador.
    - `void setPlayerLive(int pL)`: Establece los puntos de vida del luchador a un valor específico.
    - `void addPlayerLive(int pL)`: Suma un valor a los puntos de vida actuales del luchador.
    - `void randomSpecie()`: Asigna aleatoriamente una especie al luchador de la enumeración `Specie`.
    - `void movePlayer(RockBehavior rocker)`: Actualiza la posición de `playerToken` en el tablero a la `rocker` especificada.
    - `Card getPicked()`: Delega la obtención de la carta seleccionada a un componente `HolderPlay` encontrado en los hijos del GameObject.
    - `void DrawCard(int index, int HandDex)`: Extrae una carta *específica* (por `index` en el mazo) y la coloca en una posición *específica* de la mano (`HandDex`).
    - `void PlayCard(Card card)`: Delega la acción de jugar una carta al componente `HolderPlay`.
    - `private void DrawCard(int index)`: Una sobrecarga privada de `DrawCard`. Extrae una carta *aleatoria* del mazo (asegurándose de que esté activa) y la coloca en la posición de la mano especificada por `index`.
    - `void RefillHand()`: Recorre los espacios de la mano y utiliza `DrawCard(i)` (la versión aleatoria) para llenar cualquier espacio vacío con una nueva carta del mazo.
    - `bool IsFigthing()`: Determina si el luchador está participando activamente en el combate, comparando su `indexFigther` con el estado de los jugadores activos obtenido de `Combatjudge.combatjudge.GetPlayersFigthing()`.
    - `void ThrowCard()`: Delega la acción de "descartar" o "lanzar" una carta al componente `HolderPlay`, pasándole un valor `null`.

- **Lógica Clave:**
    La clase `Figther` es fundamental para el ciclo de vida del jugador en el juego. Su lógica de inicialización en `Start` es crítica para establecer tanto la representación visual del luchador como su mazo y el token en el tablero. La interacción con los modelos visuales se maneja activando y desactivando GameObjects hijos según el valor de `visualFigther`.

    La gestión de cartas se realiza mediante la interacción con los hijos del objeto `Figther` que representan el mazo (`transform.GetChild(0).GetChild(0)`) y la mano (`transform.GetChild(0).GetChild(1)`). Las cartas se instancian como hijos del contenedor del mazo, y al ser "dibujadas" o "robadas", se mueven o se activan/desactivan en la jerarquía para simular el proceso. El método `RefillHand()` asegura que la mano del jugador siempre tenga el número `handSize` de cartas, robando aleatoriamente del mazo si es necesario.

    Finalmente, la lógica para determinar si un luchador está en combate (`IsFigthing()`) es un aspecto importante. Este método consulta al `Combatjudge` para obtener un entero que representa los luchadores activos (posiblemente usando bits para indicar la presencia de cada luchador) y luego compara su `indexFigther` para determinar su estado.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere la presencia de un componente `HolderPlay` en alguno de sus hijos para la gestión de cartas jugadas y descartadas, ya que accede a él mediante `GetComponentInChildren<HolderPlay>()`.

- **Eventos (Entrada):**
    El script `Figther` no se suscribe explícitamente a eventos de Unity (`UnityEvent`) o a delegados (`Action`) de otros componentes directamente en el código proporcionado. Su lógica se activa principalmente a través de los métodos del ciclo de vida de Unity (`Start`, `Update`) y llamadas directas a sus métodos públicos desde otros scripts (ej. por `Combatjudge` o un gestor de entrada del jugador).

- **Eventos (Salida):**
    El script `Figther` no invoca ni publica ningún evento (`UnityEvent` o `Action`) para notificar a otros sistemas sobre cambios en su estado (ej., cambio de vida, carta jugada). Sus interacciones con otros sistemas son principalmente a través de llamadas a métodos públicos de instancias de otras clases (ej., `Combatjudge.combatjudge.GetSetMoments()`, `playerToken.rocky = rocker;`).