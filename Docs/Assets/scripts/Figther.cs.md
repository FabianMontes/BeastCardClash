Aquí está la documentación técnica para el script `Figther.cs`, diseñada para un nuevo miembro del equipo:

# `Figther.cs`

## 1. Propósito General
Este script `Figther.cs` actúa como la representación central de un "luchador" o "jugador" en Beast Card Clash. Gestiona atributos clave del jugador como la vida, la especie y el manejo de su mano y mazo de cartas. Interactúa principalmente con el sistema de combate (`Combatjudge`), el movimiento del jugador (`PlayerToken`, `RockBehavior`) y los sistemas de gestión de cartas (`Card`, `HandCard`, `HolderPlay`).

## 2. Componentes Clave

### `Specie`
- **Descripción:** Un `enum` simple que define los tipos de especies de animales que un luchador puede representar. Estas especies probablemente influyen en la jugabilidad o en las habilidades asociadas al luchador.
- **Valores:** `chameleon`, `bear`, `snake`, `frog`.

### `Figther`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, es el controlador principal para cada entidad de luchador en el juego. Se encarga de la inicialización, la representación visual, la gestión de vida, el control de la mano y el mazo de cartas, y la interacción con el sistema de combate. El atributo `[DefaultExecutionOrder(0)]` asegura que este script se ejecute muy temprano en el ciclo de vida de Unity, antes que la mayoría de otros scripts.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] int figtherLive`: La cantidad de puntos de vida actuales del luchador.
    - `[SerializeField] Specie specie`: La especie de animal asignada a este luchador.
    - `[SerializeField] int deckSize`: El tamaño inicial del mazo de cartas del luchador.
    - `[SerializeField] int handSize`: El número máximo de cartas que el luchador puede tener en su mano.
    - `public int avalaibleCard`: Utilizado como un contador o flag, posiblemente para indicar si hay cartas disponibles para una acción específica (observado en `Update` para revelar cartas).
    - `[SerializeField] GameObject tokenPrefab`: Prefab del token visual que representa al luchador en el tablero.
    - `[SerializeField] GameObject cardPrefab`: Prefab de una carta individual, usado para instanciar el mazo.
    - `public PlayerToken playerToken`: La instancia del token del jugador asociada a este luchador, gestiona su posición en el tablero.
    - `public int visualFigther`: Un índice que selecciona qué modelo visual (hijo del GameObject del Figther) está activo.
    - `public int indexFigther`: Un índice único para identificar a este luchador en el sistema de combate (utilizado en `IsFigthing`).
    - `public RockBehavior initialStone`: La posición inicial del token del jugador en el tablero.
- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Se llama una vez cuando el script se habilita por primera vez.
        - **Lógica clave:** Inicializa el estado visual del luchador (activando el modelo `visualFigther` y desactivando los demás). Si `playerToken` no está asignado, lo instancia y lo asocia a este luchador. Finalmente, crea el mazo provisional (`deckSize` cartas) instanciando `cardPrefab` como hijos del contenedor del mazo.
        ```csharp
        transform.GetChild(visualFigther).gameObject.SetActive(true);
        if (playerToken == null) playerToken = Instantiate(tokenPrefab).transform.GetComponent<PlayerToken>();
        playerToken.player = this;
        // ...
        for (int i = 0; i < deckSize; i++)
        {
            Card card = Instantiate(cardPrefab, transform.GetChild(0).GetChild(0)).GetComponent<Card>();
            card.indexer = i;
        }
        ```
    - `void Update()`:
        - **Descripción:** Se llama una vez por fotograma.
        - **Lógica clave:** Detecta cambios en `visualFigther` para actualizar dinámicamente el modelo 3D activo del luchador. También comprueba el estado del juego a través de `Combatjudge`: si el momento actual es `PickCard` y el luchador está "luchando" (`IsFigthing()` devuelve true), y `avalaibleCard` es 0, fuerza la revelación de todas las cartas en la mano.
    - `Specie GetSpecie()`: Devuelve la especie asignada al luchador.
    - `int GetPlayerLive()`: Devuelve los puntos de vida actuales del luchador.
    - `void setPlayerLive(int pL)`: Establece los puntos de vida del luchador a un valor específico.
    - `void addPlayerLive(int pL)`: Añade una cantidad a los puntos de vida actuales del luchador.
    - `void randomSpecie()`: Asigna una `Specie` aleatoria al luchador.
    - `void movePlayer(RockBehavior rocker)`: Actualiza la posición del `playerToken` del luchador en el tablero.
    - `Card getPicked()`: Recupera la carta actualmente "elegida" o "recogida" a través del componente `HolderPlay` hijo.
    - `void DrawCard(int index, int HandDex)`: Roba una carta específica del mazo (por `index`) y la coloca en una posición específica de la mano (`HandDex`). La carta robada se desactiva en el mazo.
    - `void PlayCard(Card card)`: Delega la acción de jugar una carta al componente `HolderPlay` hijo.
    - `private void DrawCard(int index)`:
        - **Descripción:** Una sobrecarga privada de `DrawCard` que roba una carta *aleatoria activa* del mazo y la coloca en la mano en el `index` especificado.
        - **Lógica clave:** Busca una carta activa en el mazo, lo que implica que las cartas desactivadas ya han sido robadas o están fuera de juego. Una vez encontrada, la asigna a la `HandCard` correspondiente y la desactiva en el mazo.
    - `void RefillHand()`: Recorre la mano y roba nuevas cartas (`DrawCard` aleatorio) para cualquier espacio vacío (`GetCard() == null`).
    - `bool IsFigthing()`:
        - **Descripción:** Determina si este luchador está activo en el combate actual.
        - **Lógica clave:** Utiliza una lógica de bitmask (`figthers % 2`, `figthers / 2`) con el valor devuelto por `Combatjudge.combatjudge.GetPlayersFigthing()`. Cada bit en el número entero representa el estado de un luchador (activo o inactivo) basado en su `indexFigther`. Si el bit correspondiente al `indexFigther` de este luchador es 1, el luchador está en combate.
    - `void ThrowCard()`: Llama a `PlayCard(null)` en el `HolderPlay`, lo que probablemente indica una acción de descarte o "pasar turno" sin jugar una carta.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script no usa explícitamente el atributo `[RequireComponent]`.
    - Sin embargo, depende fuertemente de una jerarquía de GameObjects específica. Espera encontrar GameObjects hijos en índices específicos para la representación visual (modelos de luchadores) y para los contenedores de mazo y mano (ej: `transform.GetChild(0).GetChild(0)` para el mazo).
    - Requiere la presencia de un componente `HolderPlay` como hijo (o en un hijo) para gestionar las cartas jugadas.
- **Eventos (Entrada):**
    - Este script no se suscribe directamente a eventos de Unity (`UnityEvent`) o delegates (`Action`).
    - Obtiene información de estado directamente de la instancia singleton de `Combatjudge` (`Combatjudge.combatjudge.GetSetMoments()` y `Combatjudge.combatjudge.GetPlayersFigthing()`).
- **Eventos (Salida):**
    - Este script no invoca ningún `UnityEvent` o `Action` público para notificar a otros sistemas.
    - Las interacciones de salida se realizan a través de la modificación directa de estados o la llamada a métodos en otros objetos (`playerToken`, `HolderPlay`, `Card`, `HandCard`).