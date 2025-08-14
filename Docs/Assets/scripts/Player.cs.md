# `Player.cs`

## 1. Propósito General
Este script `Player.cs` es un componente central en Beast Card Clash que gestiona el estado, la representación visual y las interacciones fundamentales de un jugador dentro del juego. Se encarga de aspectos como la vida del jugador, su especie, la gestión de su mazo y mano de cartas, y su participación en el sistema de combate.

## 2. Componentes Clave

### `Specie`
El `enum` `Specie` define un conjunto de tipos de animales que representan las diferentes especies de los personajes en el juego. Cada valor (`chameleon`, `bear`, `snake`, `frog`) identifica una de las posibles afiliaciones o características elementales del jugador y sus cartas, permitiendo una fácil categorización y referencia a través del código.

```csharp
public enum Specie
{
    chameleon, bear, snake, frog
}
```

### `Player`
La clase `Player` hereda de `MonoBehaviour`, lo que significa que puede adjuntarse a un GameObject en Unity y participar en el ciclo de vida del motor. Se ejecuta muy temprano (`DefaultExecutionOrder(0)`) para asegurar que su inicialización ocurra antes que otros scripts dependientes. Esta clase encapsula toda la información y comportamiento específicos de un jugador.

Sus variables importantes, que son visibles y configurables desde el Inspector de Unity, incluyen:

*   `playerLive`: Un entero que representa la cantidad de vida actual del jugador.
*   `specie`: Un valor del `enum Specie` que identifica la especie del jugador.
*   `deckSize`: El tamaño total del mazo del jugador, usado para inicializar la cantidad de cartas.
*   `handSize`: El número máximo de cartas que el jugador puede tener en su mano.
*   `avalaibleCard`: Una variable pública que aparentemente lleva un conteo de cartas disponibles o un estado relacionado con el manejo de la mano.
*   `tokenPrefab`: Una referencia a un prefab de GameObject que se utiliza para instanciar el "token" o ficha que representa al jugador en el tablero.
*   `cardPrefab`: Una referencia al prefab de la carta base, usado para generar las cartas iniciales del mazo.
*   `playerToken`: Una instancia del script `PlayerToken` que controla el movimiento y la posición del jugador en el tablero.
*   `visualPlayer`: Un entero que determina qué modelo visual (hijo del GameObject `Player`) debe estar activo para representar al jugador.
*   `indexPlayer`: Un índice numérico que identifica a este jugador, crucial para la lógica de combate.
*   `initialStone`: Una referencia a un objeto `RockBehavior` que define la posición inicial del `playerToken`.

La lógica clave de la clase `Player` se encuentra en varios de sus métodos:

*   **`Start()`**: Este método del ciclo de vida de Unity se ejecuta una vez al inicio del juego. Su función principal es inicializar la representación visual del jugador, activando el GameObject hijo correspondiente a `visualPlayer` y desactivando los demás. También instancia el `playerToken` si no ha sido asignado, lo vincula a este `Player` y a su `initialStone`. Finalmente, se encarga de crear un mazo provisional instanciando `cardPrefab` objetos y colocándolos como hijos de una jerarquía específica (`transform.GetChild(0).GetChild(0)`).

    ```csharp
    void Start()
    {
        // ... Lógica de activación/desactivación de modelos visuales ...

        if (playerToken == null) playerToken = Instantiate(tokenPrefab).transform.GetComponent<PlayerToken>();
        playerToken.player = this;
        playerToken.rocky = initialStone;

        for (int i = 0; i < deckSize; i++)
        {
            Card card = Instantiate(cardPrefab, transform.GetChild(0).GetChild(0)).GetComponent<Card>();
            card.indexer = i;
        }
    }
    ```

*   **`Update()`**: Ejecutado en cada frame. Contiene lógica para actualizar la apariencia del jugador si `visualPlayer` ha cambiado, asegurando que solo el modelo correcto esté activo. También monitoriza el estado del juego a través de `Combatjudge.combatjudge.GetSetMoments()`. Si el juego está en la fase de `PickCard` y el jugador está en combate (`IsFigthing()` es verdadero) y no tiene cartas disponibles (`avalaibleCard == 0`), este método fuerza la revelación de todas las cartas en la mano.

    ```csharp
    void Update()
    {
        if (lastVisualPlayer != visualPlayer)
        {
            // Lógica para cambiar la representación visual del jugador
        }
        if (Combatjudge.combatjudge.GetSetMoments() == SetMoments.PickCard && IsFigthing())
        {
            // Lógica para revelar la mano
        }
    }
    ```

*   **Métodos para obtener y establecer propiedades**: La clase ofrece métodos públicos como `GetSpecie()`, `GetPlayerLive()`, `setPlayerLive(int pL)`, y `addPlayerLive(int pL)` para interactuar con las propiedades de `specie` y `playerLive`.

*   **`randomSpecie()`**: Un método utilitario que asigna una especie aleatoria al jugador, seleccionando entre los valores definidos en el `enum Specie`.

*   **`movePlayer(RockBehavior rocker)`**: Este método actualiza la referencia `rocky` en el `playerToken`, indicando una nueva "roca" o punto en el tablero al que el token del jugador debe asociarse o moverse.

*   **`DrawCard(int index, int HandDex)`**: Este método permite "robar" una carta específica del mazo (`deck`) a una posición particular en la mano (`hand`). La carta se desactiva del mazo y se asigna al slot de la mano.

*   **`DrawCard(int index)` (privado)**: Una versión sobrecargada y privada de `DrawCard`. Esta versión roba una carta *aleatoria* que esté activa en el mazo y la coloca en el slot de la mano indicado por `index`. Busca una carta activa iterando si la seleccionada inicialmente no lo está.

    ```csharp
    private void DrawCard(int index)
    {
        // Lógica para encontrar y dibujar una carta aleatoria activa del mazo
    }
    ```

*   **`RefillHand()`**: Utiliza la versión privada de `DrawCard` para llenar los slots vacíos en la mano del jugador. Itera sobre cada slot de la mano y, si encuentra uno sin una carta (`GetCard() == null`), dibuja una nueva carta aleatoria para ese slot.

*   **`PlayCard(Card card)` y `ThrowCard()`**: Ambos métodos delegan la acción de jugar o descartar una carta al componente `HolderPlay`, que se espera sea un hijo de este GameObject `Player`. `ThrowCard()` simplemente llama a `PlayCard` con un valor `null` para indicar que se está descartando.

*   **`IsFigthing()`**: Este método determina si el jugador está actualmente involucrado en el combate. Lo hace interpretando un valor `figthers` (presumiblemente un bitmask) obtenido de `Combatjudge.combatjudge.GetPlayersFigthing()`. Comprueba si el bit correspondiente a su `indexPlayer` está activado.

    ```csharp
    public bool IsFigthing()
    {
        int figthers = Combatjudge.combatjudge.GetPlayersFigthing();
        int a = 0;
        while (figthers > 0)
        {
            int red = figthers % 2;
            figthers = (int)Mathf.Floor(figthers / 2);
            if (a == indexPlayer) return red != 0;
            a++;
        }
        return false;
    }
    ```

## 3. Dependencias y Eventos
El script `Player.cs` no utiliza atributos `[RequireComponent]` para forzar la presencia de otros componentes en el mismo GameObject. Sin embargo, tiene varias dependencias y participa en flujos de eventos clave:

*   **Dependencias de Componentes y Objetos**:
    *   **`PlayerToken`**: Instancia y controla una instancia de `PlayerToken` para manejar la posición del jugador en el tablero.
    *   **`RockBehavior`**: Requiere una referencia a un `initialStone` (`RockBehavior`) para posicionar el `playerToken` inicialmente. También interactúa con `RockBehavior` para mover el jugador (`movePlayer`).
    *   **`Card` y `HandCard`**: Instancia objetos `Card` para el mazo y espera que los slots de la mano sean `HandCard`s para establecer y revelar cartas.
    *   **`HolderPlay`**: Espera encontrar un componente `HolderPlay` entre sus hijos (`GetComponentInChildren<HolderPlay>()`) para delegar la lógica de jugar o descartar cartas.
    *   **`Combatjudge`**: Se suscribe implícitamente a información del estado de combate a través de la propiedad estática `Combatjudge.combatjudge`. Esto le permite reaccionar a los `SetMoments` del juego y determinar si está en combate (`IsFigthing()`).

*   **Eventos (Entrada)**:
    *   El script reacciona a los cambios en `visualPlayer` para actualizar su representación.
    *   Monitoriza el estado global del juego consultando `Combatjudge.combatjudge.GetSetMoments()`, lo que le permite saber cuándo es el momento de ciertas acciones (como revelar cartas en la mano durante la fase `PickCard`).

*   **Eventos (Salida)**:
    *   Aunque no invoca `UnityEvent`s o `Action`s explícitas, el script modifica directamente el estado de otros objetos. Por ejemplo, al instanciar `PlayerToken` y `Card`s, o al llamar a métodos en `playerToken`, `HandCard` y `HolderPlay`. Estas interacciones actúan como notificaciones o comandos a otros sistemas, indicando cambios en la posición del jugador, el contenido de su mano o las cartas jugadas.