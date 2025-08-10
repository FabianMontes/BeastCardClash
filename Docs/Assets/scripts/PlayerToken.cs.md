# `PlayerToken.cs`

## 1. Propósito General
Este script (`MonoBehaviour`) gestiona la representación visual y el posicionamiento de un token de jugador en el tablero de juego. Su rol principal es asegurar que el token se ubique correctamente sobre el "rock" (baldosa/nodo) asignado al jugador, ajustar su apariencia según la especie del personaje asociado y notificar a los sistemas de juego principales (como el de combate) cuando el token se mueve a una nueva posición.

## 2. Componentes Clave

### `PlayerToken`
-   **Descripción:** La clase `PlayerToken` es un componente de Unity (`MonoBehaviour`) que actúa como el avatar visual de un personaje jugador dentro del escenario del juego. Su función principal es mantener la sincronización entre la posición visual del token y el "rock" (baldosa o nodo del tablero) que representa la ubicación actual del jugador. Se ejecuta con una `DefaultExecutionOrder` de 1, lo que significa que se inicializa y actualiza antes que muchos otros scripts con el orden predeterminado (0) o superior, asegurando que su estado posicional sea procesado tempranamente.
-   **Variables Públicas / Serializadas:**
    *   `public RockBehavior rocky;`: Una referencia al objeto `RockBehavior` que representa el "rock" o baldosa actual sobre la que se encuentra el `PlayerToken`. Este es el punto de control para la posición del token en el tablero. Cuando un jugador se mueve, esta variable se actualiza para apuntar al nuevo rock.
    *   `public Figther player;`: Una referencia al script o clase `Figther` (presumiblemente, "Fighter") que contiene los datos del personaje del jugador al que este token representa. Se utiliza para acceder a propiedades del personaje, como su especie, para influir en la apariencia del token.
    *   `public RockBehavior lastRock;`: Almacena una referencia al `RockBehavior` del "rock" en el que el `PlayerToken` se encontraba antes de su movimiento actual. Es crucial para desvincular el token del rock anterior cuando se desplaza.
-   **Métodos Principales:**
    *   `void Start()`: Este método se invoca una vez al inicio, justo antes de la primera actualización del componente.
        *   Si la referencia `rocky` ya está asignada en el Inspector de Unity, el token se posiciona inmediatamente en la ubicación del `rocky` inicial. Además, se registra a sí mismo con ese `rocky` a través de `rocky.AddPlayer(this)` y guarda este `rocky` como `lastRock`.
        *   Obtiene el componente `SpriteRenderer` adjunto al mismo GameObject.
        *   Ajusta el color del `SpriteRenderer` basándose en la especie del `player` asociado, utilizando una instrucción `switch` para aplicar colores específicos según `Specie.chameleon`, `Specie.bear`, `Specie.snake` y `Specie.frog`.
        ```csharp
        // Fragmento de la inicialización del color en Start()
        SpriteRenderer rnd = transform.GetComponent<SpriteRenderer>();
        switch (player.GetSpecie())
        {
            case Specie.chameleon:
                rnd.color = Color.green;
                break;
            // ... otros casos
        }
        ```
    *   `void Update()`: Este método se ejecuta en cada frame del juego.
        *   Su función principal es monitorear cambios en la posición del `PlayerToken` o en la referencia a `rocky`. Si el `PlayerToken` detecta que su posición actual no coincide con la de su `rocky` asignado (lo que implica que el `rocky` se ha actualizado o el token necesita moverse), inicia un proceso de actualización de posición.
        *   Desvincula el token del `lastRock` llamando a `lastRock.RemovePlayer(this)`.
        *   Mueve su `transform.position` a la del nuevo `rocky`.
        *   Vincula el token al nuevo `rocky` mediante `rocky.AddPlayer(this)`.
        *   Actualiza `lastRock` para que apunte al nuevo `rocky`.
        *   Finalmente, notifica al sistema de combate principal (a través del singleton `Combatjudge.combatjudge`) que el token ha llegado a un nuevo rock, invocando `Combatjudge.combatjudge.ArriveAtRock()`.
        ```csharp
        // Lógica clave de detección de movimiento y actualización en Update()
        if (rocky != null && rocky.transform.position != transform.position)
        {
            lastRock.RemovePlayer(this);
            transform.position = rocky.transform.position;
            rocky.AddPlayer(this);
            lastRock = rocky;
            Combatjudge.combatjudge.ArriveAtRock();
        }
        ```
-   **Lógica Clave:** La lógica más importante de `PlayerToken` reside en su método `Update`. Este método implementa un sistema reactivo donde el token ajusta su posición y notifica a otros sistemas tan pronto como su `rocky` asignado cambia o se mueve. Esto es fundamental para un juego basado en tablero, donde la posición visual del jugador debe reflejar con precisión su posición lógica en el juego, y donde los eventos de movimiento (como llegar a un nuevo rock) pueden desencadenar otras acciones de juego.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcto funcionamiento visual, se espera que el GameObject al que se adjunta tenga un componente `SpriteRenderer` (que el script intenta obtener en `Start()`) para poder manipular el color del token.
-   **Eventos (Entrada):** `PlayerToken` no se suscribe a eventos de Unity (`UnityEvent`) ni a `Action`s personalizados. Su comportamiento se desencadena principalmente por los métodos del ciclo de vida de Unity (`Start`, `Update`) y por los cambios en las referencias a sus variables públicas (`rocky`, `player`), las cuales son gestionadas por otros sistemas del juego que controlan el movimiento y la asignación de rocks.
-   **Eventos (Salida):** Este script no invoca `UnityEvent`s ni `Action`s propios para notificar a otros sistemas. En su lugar, interactúa directamente con otras clases y singletons a través de llamadas a métodos:
    *   `RockBehavior.AddPlayer(this)` y `RockBehavior.RemovePlayer(this)`: Se utiliza para informar a los objetos `RockBehavior` sobre la presencia o ausencia del `PlayerToken`.
    *   `Combatjudge.combatjudge.ArriveAtRock()`: Esta llamada notifica al sistema global `Combatjudge` que un `PlayerToken` ha completado un movimiento a un nuevo rock, lo cual puede desencadenar la evaluación de combate o eventos de terreno.