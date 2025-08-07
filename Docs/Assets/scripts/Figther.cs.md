Aquí tienes la documentación técnica para el script `Figther.cs`, siguiendo el formato solicitado:

---

# `Figther.cs`

## 1. Propósito General
Este script principal gestiona la entidad de un "luchador" (jugador) dentro del juego. Es responsable de mantener sus atributos vitales, controlar su representación visual, y orquestar las interacciones fundamentales con su mazo de cartas, su mano y el tablero de juego.

## 2. Componentes Clave

### `Specie`
- **Descripción:** Es un `enum` simple que define los tipos de especies de animales que puede representar un luchador.
- **Valores:**
    - `chameleon`
    - `bear`
    - `snake`
    - `frog`

### `Figther`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, representa a un jugador o entidad combatiente en el juego. Controla sus estadísticas básicas, su modelo visual en escena, y la lógica para la gestión de su mano y mazo de cartas. También interactúa con el sistema de combate para determinar su estado en la partida.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] int figtherLive`: Los puntos de vida actuales del luchador.
    - `[SerializeField] Specie specie`: La especie de animal asociada a este luchador.
    - `[SerializeField] int deckSize`: El tamaño total del mazo del luchador.
    - `[SerializeField] int handSize`: El número máximo de cartas que el luchador puede tener en su mano. Por defecto, es `6`.
    - `public int avalaibleCard`: Un contador o flag que parece indicar la disponibilidad de una carta, utilizado en la lógica de `Update` para revelar cartas.
    - `[SerializeField] GameObject tokenPrefab`: El prefab del token que representa al jugador en el tablero.
    - `[SerializeField] GameObject cardPrefab`: El prefab utilizado para instanciar las cartas del mazo.
    - `[SerializeField] public PlayerToken playerToken`: La instancia del token del jugador que se mueve por el tablero. Se referencia directamente aquí después de ser instanciado.
    - `[SerializeField] public int visualFigther`: Un índice que determina cuál de los objetos hijos del `Figther` se activa para representar visualmente al personaje.
    - `[SerializeField] public int indexFigther`: Un identificador único para este luchador, usado en `IsFigthing()` para determinar su participación en el combate.
    - `[SerializeField] public RockBehavior initialStone`: Una referencia a un objeto `RockBehavior` que probablemente define la posición inicial del luchador en el tablero.
- **Métodos Principales:**
    - `void Start()`: Este método se ejecuta una vez al inicio.
        - Inicializa la visibilidad de los modelos 3D del luchador, asegurándose de que solo el modelo especificado por `visualFigther` esté activo.
        - Instancia el `tokenPrefab` si no ha sido asignado, y lo asocia a este luchador, enlazándolo también a la `initialStone` para su posición inicial.
        - Crea un mazo provisional instanciando `cardPrefab` un número de veces igual a `deckSize` como hijos de una transformación específica.
    - `void Update()`: Se ejecuta en cada frame.
        - Detecta cambios en `visualFigther` y actualiza la visibilidad del modelo del luchador para reflejar el cambio.
        - Si el `Combatjudge` indica que es el momento de `PickCard` (`SetMoments.PickCard`) y el luchador está en combate (`IsFigthing()`) y `avalaibleCard` es `0`, fuerza la revelación de todas las cartas en la mano.
    - `public Specie GetSpecie()`: Devuelve la `Specie` actual del luchador.
    - `public int GetPlayerLive()`: Devuelve los puntos de vida (`figtherLive`) actuales.
    - `public void setPlayerLive(int pL)`: Establece los puntos de vida del luchador.
    - `public void addPlayerLive(int pL)`: Suma un valor a los puntos de vida actuales del luchador.
    - `public void randomSpecie()`: Asigna una especie aleatoria al luchador de entre las definidas en el `Specie` enum.
    - `public void movePlayer(RockBehavior rocker)`: Actualiza la posición del `playerToken` del luchador en el tablero, asignándole una nueva `rocker` (piedra).
    - `public Card getPicked()`: Recupera la carta que ha sido seleccionada o "recogida" a través del componente `HolderPlay` en los hijos de este `GameObject`.
    - `public void DrawCard(int index, int HandDex)`: Permite dibujar una carta específica del mazo (usando su `index` en la jerarquía del mazo) y la coloca en una posición específica de la mano (`HandDex`). La carta dibujada se desactiva en el mazo.
    - `public void PlayCard(Card card)`: Delega la acción de jugar una `card` al componente `HolderPlay` encontrado en los hijos.
    - `private void DrawCard(int index)`: Esta es una sobrecarga privada para dibujar una carta de forma aleatoria del mazo. Selecciona una carta activa al azar del mazo, la asigna a una posición `index` en la mano, y la desactiva en el mazo. Contiene lógica para buscar la siguiente carta disponible si la seleccionada inicialmente no está activa.
    - `public void RefillHand()`: Recorre la mano y, para cada espacio de carta vacío, llama a la versión privada de `DrawCard` para rellenar ese espacio con una carta aleatoria del mazo.
    - `public bool IsFigthing()`: Determina si el luchador está actualmente participando en el combate. Utiliza un valor entero `figthers` obtenido de `Combatjudge.combatjudge.GetPlayersFigthing()` y lo interpreta bit a bit en relación con el `indexFigther` del luchador.
    - `public void ThrowCard()`: Llama a `PlayCard(null)` en el `HolderPlay` hijo, lo que podría usarse para cancelar una selección de carta o descartar una carta sin jugarla.
- **Lógica Clave:**
    - **Gestión Visual Dinámica:** El script puede cambiar el modelo 3D activo del luchador en tiempo de ejecución, lo que sugiere que diferentes modelos representan diferentes estados o apariencias del personaje.
    - **Inicialización de Mazo y Mano:** Durante el `Start`, se crea un "mazo" inicial de `cardPrefab`s. Los métodos `DrawCard` y `RefillHand` se encargan de mover cartas de este mazo a la mano del jugador.
    - **Determinación de Estado de Combate:** La función `IsFigthing()` es crucial para filtrar qué luchadores deben reaccionar a ciertos eventos o reglas del juego, utilizando una forma de codificación por bits para representar a los luchadores activos.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Aunque no se utiliza el atributo `[RequireComponent]`, este script espera encontrar componentes específicos en sus hijos o en el propio `GameObject` para funcionar correctamente, como `HolderPlay`, `Card`, y `HandCard`.
- **Eventos (Entrada):**
    - Se suscribe implícitamente a los eventos del ciclo de vida de Unity (`Start`, `Update`).
    - Depende del estado global gestionado por el singleton `Combatjudge.combatjudge` para determinar el momento del juego (`SetMoments.PickCard`) y los jugadores que están activos en combate (`GetPlayersFigthing()`).
- **Eventos (Salida):**
    - Este script no emite eventos (`UnityEvent` o `Action`) directamente para notificar a otros sistemas de sus cambios de estado. Las interacciones se realizan a través de la manipulación directa de referencias (como `playerToken.rocky`) o invocando métodos en otros componentes (`GetComponentInChildren<HolderPlay>().PlayCard(...)`).

---