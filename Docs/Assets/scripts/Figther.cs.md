# `Figther.cs`

## 1. Propósito General
Este script gestiona la entidad principal de un jugador, el "Figther", en el juego "Beast Card Clash". Se encarga de mantener los atributos del personaje (salud, especie, tamaño del mazo y la mano), su representación visual en el juego, y coordina las interacciones clave relacionadas con el manejo de cartas (robo y juego) y el movimiento del token del jugador en el tablero.

## 2. Componentes Clave

### `enum Specie`
- **Descripción:** Un tipo enumerado que define las posibles especies de animales que puede representar un `Figther`. Estas especies probablemente influyen en la jugabilidad o las habilidades del personaje.

```csharp
public enum Specie
{
    chameleon, bear, snake, frog
}
```

### `Figther`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, representa a un personaje "Figther" en el juego. Controla sus características fundamentales, su apariencia dinámica y su interacción con los sistemas de juego de cartas y movimiento.

- **Variables Públicas / Serializadas:**
    - `figtherLive` (int): Los puntos de vida actuales del `Figther`.
    - `specie` (Specie): La especie de animal asignada a este `Figther`.
    - `deckSize` (int): El número total de cartas que componen el mazo inicial de este `Figther`.
    - `handSize` (int): El número máximo de cartas que el `Figther` puede tener en su mano. Por defecto es `6`.
    - `avalaibleCard` (int): Un contador o indicador. Su uso en `Update` sugiere que controla si las cartas en la mano deben revelarse durante una fase específica.
    - `tokenPrefab` (GameObject): El prefab del token visual del jugador que se mueve en el tablero.
    - `cardPrefab` (GameObject): El prefab de las cartas individuales usadas para construir el mazo.
    - `playerToken` (PlayerToken): La instancia del token del jugador creada a partir de `tokenPrefab`. Este objeto se mueve por el tablero.
    - `visualFigther` (int): Un índice que determina qué modelo visual (hijo del GameObject) se activa para representar a este `Figther`.
    - `indexFigther` (int): Un identificador numérico único para este `Figther`, utilizado para determinar su participación en el combate.
    - `initialStone` (RockBehavior): La posición inicial en el tablero (un objeto `RockBehavior`) donde se colocará el `playerToken` del `Figther`.

- **Métodos Principales:**
    - `void Start()`:
        - Se ejecuta una vez al inicio. Inicializa la visibilidad del modelo 3D del `Figther` según `visualFigther`.
        - Instancia el `playerToken` si no ha sido asignado, lo vincula a este `Figther` y establece su posición inicial (`rocky`) en `initialStone`.
        - Crea un "mazo provisional" instanciando `deckSize` `cardPrefab` como hijos.
    - `void Update()`:
        - Se ejecuta cada fotograma.
        - Gestiona el cambio dinámico del modelo visual del `Figther` si el valor de `visualFigther` cambia en tiempo de ejecución.
        - Durante la fase de juego `SetMoments.PickCard` (determinada por `Combatjudge`) y si el `Figther` está participando en el combate (`IsFigthing()`) y `avalaibleCard` es `0`, fuerza a que todas las cartas en la mano sean reveladas.
    - `Specie GetSpecie()`: Devuelve la `Specie` actual del `Figther`.
    - `int GetPlayerLive()`: Devuelve los puntos de vida (`figtherLive`) actuales del `Figther`.
    - `void setPlayerLive(int pL)`: Establece los puntos de vida del `Figther` a un valor específico `pL`.
    - `void addPlayerLive(int pL)`: Añade un valor `pL` a los puntos de vida actuales del `Figther`.
    - `void randomSpecie()`: Asigna una `Specie` aleatoria al `Figther` de entre las definidas en el `enum Specie`.
    - `void movePlayer(RockBehavior rocker)`: Actualiza la posición del `playerToken` del `Figther` en el tablero a la `RockBehavior` proporcionada.
    - `Card getPicked()`: Recupera la carta actualmente "seleccionada" (o en el área de juego) a través de un componente hijo `HolderPlay`.
    - `void DrawCard(int index, int HandDex)`: Roba una carta específica del mazo usando su `index` y la coloca en la mano del jugador en la posición `HandDex`. La carta robada se desactiva en el mazo.
    - `void PlayCard(Card card)`: Delega la acción de jugar una `card` al componente hijo `HolderPlay`.
    - `private void DrawCard(int index)`: Roba una carta *aleatoria* que esté activa en el mazo y la coloca en la mano del jugador en la posición `index`. Esta es una versión interna que asegura que solo se roben cartas disponibles.
    - `void RefillHand()`: Recorre los espacios de la mano del `Figther` y, si algún espacio está vacío, llama a la versión `private DrawCard(int index)` para robar una nueva carta aleatoria y rellenar ese espacio.
    - `bool IsFigthing()`: Consulta al `Combatjudge` para determinar si este `Figther` (identificado por `indexFigther`) está actualmente participando en el combate. La lógica interna utiliza operaciones bit a bit para verificar el estado de los jugadores.
    - `void ThrowCard()`: Llama al método `PlayCard` del `HolderPlay` pasando `null`, lo que podría significar descartar una carta o pasar el turno sin jugar.

- **Lógica Clave:**
    - **Gestión de Modelos Visuales:** El método `Update` optimiza la activación/desactivación de los modelos 3D del `Figther`. Solo actualiza el modelo activo si el índice `visualFigther` ha cambiado desde el último fotograma, mejorando el rendimiento.
    - **Manejo de Mazo y Mano:** El script proporciona dos mecanismos de robo de cartas: uno para robar una carta específica por índice (`DrawCard(int index, int HandDex)`) y otro para robar una carta aleatoria disponible del mazo (`private void DrawCard(int index)`), usado para rellenar la mano.
    - **Determinación de Participación en Combate:** La función `IsFigthing()` utiliza un patrón de bits para verificar la información de `Combatjudge.GetPlayersFigthing()` y determinar eficientemente si el `Figther` actual está activo en un combate.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    - Aunque no se usa explícitamente el atributo `[RequireComponent]`, este script depende fundamentalmente de la existencia de ciertos componentes en sus GameObjects hijos para la gestión de cartas y juego: `Card` (en el mazo), `HandCard` (en la mano) y `HolderPlay` (para jugar cartas). También asume la existencia de una instancia global o singleton de `Combatjudge`.
- **Eventos (Entrada):**
    - Este script se suscribe indirectamente al estado del juego a través de consultas a `Combatjudge.combatjudge.GetSetMoments()`, lo que le permite reaccionar a fases específicas del combate (como `SetMoments.PickCard`).
- **Eventos (Salida):**
    - `Figther.cs` no invoca `UnityEvent` ni `Action` personalizados para notificar a otros sistemas. En su lugar, interactúa directamente con otros componentes y scripts (ej., `playerToken`, `HolderPlay`, `Combatjudge`) llamando a sus métodos públicos.