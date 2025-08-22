# `Figther.cs`

## 1. Propósito General
Este script principal gestiona la representación y las mecánicas fundamentales de un "luchador" o personaje en el juego "Beast Card Clash". Controla sus estadísticas vitales, su identidad (nombre, equipo, especie), su representación visual en el juego, y su interacción básica con el sistema de cartas (mano y mazo) y el tablero de juego.

## 2. Componentes Clave

### `Team` (Enum)
- **Descripción:** Una enumeración que define los diferentes equipos o "facultades" a los que puede pertenecer un luchador. Los valores representan las inspiraciones académicas del juego, como `ingeniosos` (Ingeniería) o `adn` (Medicina).

### `Specie` (Enum)
- **Descripción:** Una enumeración que define las diferentes especies de animales que un luchador puede representar. Los valores actuales son `bear`, `frog`, `chameleon`, `condor`, inspirados en la biodiversidad colombiana.

### `Figther` (Clase)
- **Descripción:** Esta clase hereda de `MonoBehaviour` y es el componente central para cada personaje jugable o controlado por la IA en el juego. Encapsula toda la lógica y datos relacionados con la salud, identidad, apariencia y comportamiento básico del personaje durante una partida. El atributo `[DefaultExecutionOrder(0)]` asegura que este script se ejecute muy temprano en el ciclo de vida de los scripts de Unity, antes que la mayoría de los demás.

- **Variables Públicas / Serializadas:**
    - `figtherLive` (`int`): La vida actual del luchador.
    - `figtherName` (`public string`): El nombre identificativo del luchador, visible en el Inspector de Unity.
    - `team` (`Team`): El equipo al que pertenece el luchador, seleccionado desde el enum `Team`.
    - `specie` (`Specie`): La especie de animal que representa el luchador, seleccionado desde el enum `Specie`.
    - `skin` (`public int { get; private set; }`): El índice de la apariencia visual (skin) del luchador. Es de solo lectura fuera de la clase, pero se puede modificar internamente con métodos `setSkin`, `setNoSkin`, etc.
    - `deckSize` (`int`): El número total de cartas que componen el mazo inicial del luchador.
    - `handSize` (`int`): El número máximo de cartas que el luchador puede tener en su mano (por defecto 6).
    - `avalaibleCard` (`public int`): Un contador que parece estar relacionado con la disponibilidad de acciones de robar cartas o el estado de la mano.
    - `tokenPrefab` (`GameObject`): Prefab del token visual que representa al luchador en el tablero de juego.
    - `cardPrefab` (`GameObject`): Prefab base para las cartas que formarán el mazo del luchador.
    - `playerToken` (`public PlayerToken`): Una referencia a la instancia del `PlayerToken` que representa a este luchador en el tablero.
    - `visualFigther` (`public int`): El índice de un objeto hijo específico que se activa para representar el modelo 3D actual del luchador. Permite cambiar dinámicamente la apariencia visual del personaje.
    - `indexFigther` (`public int`): Un índice numérico único para este luchador, utilizado para identificarlo en el sistema de combate global.
    - `initialStone` (`public RockBehavior`): Una referencia a un objeto `RockBehavior` que define la posición inicial del token del jugador en el tablero.
    - `noHurt` (`public bool {get; private set;}`): Un indicador booleano que es `true` si la última modificación de vida fue una ganancia o recuperación de vida, y `false` si fue una pérdida de vida.

- **Métodos Principales:**
    - `void Start()`: Se ejecuta una vez al inicio del juego.
        - Desactiva todos los modelos visuales del luchador (hijos en los índices 1, 2, 3, 4) y activa el especificado por `visualFigther`, asegurando que solo un modelo sea visible.
        - Si `playerToken` no está asignado, instancia el `tokenPrefab`, lo configura con la posición inicial (`initialStone`) y le asigna esta instancia de `Figther` como su propietario.
        - Inicializa un "mazo provisional" instanciando `deckSize` copias del `cardPrefab` como hijos de una transformación específica (presumiblemente el mazo visual), asignando un índice a cada `Card`.
        ```csharp
        // Activación/desactivación de modelos visuales
        transform.GetChild(lastVisualPlayer).gameObject.SetActive(false);
        transform.GetChild(visualFigther).gameObject.SetActive(true);
        // ...
        // Instancia el token del jugador
        if (playerToken == null) playerToken = Instantiate(tokenPrefab, initialStone.transform.position + Vector3.up * 1, Quaternion.identity).transform.GetComponent<PlayerToken>();
        // ...
        // Crea el mazo
        for (int i = 0; i < deckSize; i++)
        {
            Card card = Instantiate(cardPrefab, transform.GetChild(0).GetChild(0)).GetComponent<Card>();
            card.indexer = i;
        }
        ```
    - `void Update()`: Se ejecuta en cada frame del juego.
        - Monitorea cambios en `visualFigther` para actualizar el modelo 3D activo del luchador.
        - Durante la fase de "PickCard" (definida por `Combatjudge.combatjudge.GetSetMoments()`) y si el luchador está activo en el combate (`IsFigthing()`): si `avalaibleCard` es 0, fuerza la revelación de todas las cartas en la mano del jugador.
    - `void addPlayerLive(int pL)`: Ajusta la vida del luchador sumando `pL`. La propiedad `noHurt` se establece a `true` si `pL` es no negativo. La vida del luchador se limita para no exceder la vida inicial (`Combatjudge.combatjudge.initialLives`) ni ser menor que 0.
        ```csharp
        public void addPlayerLive(int pL)
        {
            noHurt = pL >= 0; // true if life increases or stays same
            figtherLive += pL;
            if(figtherLive > Combatjudge.combatjudge.initialLives)
            {
                figtherLive = Combatjudge.combatjudge.initialLives;
            }
            if(figtherLive < 0)
            {
                figtherLive = 0;
            }
        }
        ```
    - `void movePlayer(RockBehavior rocker)`: Actualiza la posición del token del jugador en el tablero, asignándole una nueva `RockBehavior` como su destino.
    - `Card getPicked()`: Recupera la carta que ha sido "seleccionada" o "preparada para jugar" a través del componente `HolderPlay` (buscado entre los componentes hijos).
    - `void DrawCard(int index, int HandDex)`: Roba una carta específica del mazo identificada por su `index` dentro del mazo, la desactiva en el mazo y la asigna a una posición específica en la mano (`HandDex`).
    - `void PlayCard(Card card)`: Delega la acción de jugar una carta al componente `HolderPlay`, pasándole la referencia a la `Card`.
    - `private void DrawCard(int index)`: Una sobrecarga privada de `DrawCard` que roba una carta *aleatoria* del mazo que esté activa (`gameObject.activeSelf`). La carta robada se desactiva en el mazo y se asigna a la posición `index` en la mano.
    - `void RefillHand()`: Itera sobre todas las posiciones de la mano. Si una posición está vacía (no tiene una carta asignada), llama al método `private DrawCard(i)` para robar una nueva carta y llenarla. Finalmente, resetea `avalaibleCard` a 0.
    - `bool IsFigthing()`: Determina si este luchador está participando activamente en el combate actual. Realiza una verificación bit a bit usando el `indexFigther` del luchador contra un valor entero (`Combatjudge.combatjudge.GetPlayersFigthing()`) que codifica qué jugadores están activos en el combate.
        ```csharp
        public bool IsFigthing()
        {
            int figthers = Combatjudge.combatjudge.GetPlayersFigthing();
            int a = 0; // Counter for fighter index

            while (figthers > 0)
            {
                int red = figthers % 2; // Check the last bit
                figthers = (int)Mathf.Floor(figthers / 2); // Shift bits

                if (a == indexFigther) return red != 0; // If current fighter's bit is set
                a++;
            }
            return false;
        }
        ```
    - `void ThrowCard()`: Llama a `PlayCard(null)` en el `HolderPlay`, lo que sugiere una acción de descarte o eliminación de la carta actualmente "preparada" por el `HolderPlay`.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Este script no utiliza explícitamente el atributo `[RequireComponent]`. Sin embargo, funcionalmente depende de la existencia de ciertos componentes y estructuras de GameObjects hijos en tiempo de ejecución:
        - Necesita un componente `HolderPlay` en alguno de sus hijos (o en sí mismo) para manejar la lógica de jugar y recoger cartas.
        - Interactúa con instancias de `PlayerToken`, `Card` y `HandCard` que deben estar presentes en la escena o ser instanciadas correctamente.
        - Asume una estructura específica de GameObjects hijos para manejar los modelos visuales del luchador, el mazo y la mano (ej. `transform.GetChild(0).GetChild(0)` para el mazo).
- **Eventos (Entrada):**
    - Este script no se suscribe directamente a eventos (como `UnityEvent` o `Action`) definidos por otros scripts.
    - Obtiene información de estado directamente del sistema `Combatjudge` (ej. `Combatjudge.combatjudge.GetSetMoments()`, `Combatjudge.combatjudge.GetPlayersFigthing()`, `Combatjudge.combatjudge.initialLives`).
- **Eventos (Salida):**
    - Este script no invoca explícitamente `UnityEvent`s o `Action`s para notificar a otros sistemas sobre sus cambios de estado. Su interacción con otros componentes (`PlayerToken`, `Card`, `HandCard`, `HolderPlay`) es principalmente a través de llamadas directas a sus métodos públicos.