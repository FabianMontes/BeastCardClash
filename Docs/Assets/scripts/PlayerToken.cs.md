# `PlayerToken.cs`

## 1. Propósito General
Este script gestiona la representación visual y el posicionamiento de un "token" de jugador en el tablero de juego. Se encarga de sincronizar la posición del token con la "roca" (RockBehavior) en la que se encuentra y de notificar a otros sistemas cuando el token se mueve a una nueva roca, lo que puede desencadenar fases de juego como el combate.

## 2. Componentes Clave

### `PlayerToken`
- **Descripción:** La clase `PlayerToken` es un `MonoBehaviour` que se adjunta a un GameObject en la escena para representar visualmente a un jugador en el tablero. Su principal responsabilidad es asegurar que el token esté correctamente posicionado sobre la roca actual y gestionar las transiciones entre rocas.

- **Variables Públicas / Serializadas:**
    - `public RockBehavior rocky;`: Esta variable mantiene una referencia a la instancia de `RockBehavior` que representa la roca actual sobre la que se encuentra o debe moverse el token. Es fundamental para determinar la posición del token en el tablero.
    - `public Figther player;`: Una referencia al objeto `Figther` (notar el nombre `Figther` tal como aparece en el código) asociado a este token. Se utiliza para obtener información sobre el jugador, como su tipo de `Specie`, lo que influye en la apariencia del token.
    - `public RockBehavior lastRock;`: Almacena una referencia a la roca en la que el token se encontraba justo antes de su movimiento actual. Es crucial para desvincular el token de la roca anterior al moverse a una nueva.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se ejecuta una vez al inicio del juego, antes del primer `Update`.
        *   Inicializa la posición del `PlayerToken` moviéndolo a la posición de la `rocky` asignada, si existe.
        *   Establece la `rocky` inicial como `lastRock`.
        *   Invoca el método `AddPlayer()` en la `rocky` actual, indicando que este token está ahora sobre ella.
        *   Recupera el componente `SpriteRenderer` del GameObject para cambiar el color del token basándose en la `Specie` del `player` asociado. Se asignan colores específicos (verde para camaleón, un tono de marrón para oso, magenta para serpiente y amarillo para rana).

    - `void Update()`: Este método del ciclo de vida de Unity se ejecuta una vez por cada frame.
        *   Su función principal es detectar si la referencia `rocky` ha cambiado (lo que implica que el token debe moverse a una nueva roca) o si la posición de la `rocky` ha cambiado.
        *   Si la posición del token difiere de la posición de su `rocky` actual, indica un movimiento a una nueva roca.
        *   Al detectar un movimiento, primero llama a `RemovePlayer()` en la `lastRock` para desvincularse de la roca anterior.
        *   Luego, actualiza su propia posición a la de la nueva `rocky` y llama a `AddPlayer()` en la nueva `rocky` para vincularse a ella.
        *   Finalmente, actualiza `lastRock` para que apunte a la roca actual y llama a `Combatjudge.combatjudge.ArriveAtRock()`, lo que sugiere que llegar a una nueva roca puede desencadenar un evento relacionado con el sistema de combate o de fases de juego.

- **Lógica Clave:**
    La lógica principal de `PlayerToken` reside en su método `Update`. Monitorea constantemente si la `rocky` a la que está asociado ha cambiado de posición o si la referencia a `rocky` en sí misma ha sido actualizada externamente (lo que haría que la posición de `rocky.transform.position` sea diferente de `transform.position`). Cuando detecta esta discrepancia, el script orquesta la transición del token de una roca a otra, incluyendo la actualización de las referencias en las rocas y la notificación al `Combatjudge`.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`, pero asume la presencia de un `SpriteRenderer` en el mismo GameObject para controlar su apariencia visual.

- **Eventos (Entrada):**
    Este script no se suscribe explícitamente a eventos de Unity (`UnityEvent` o `Action`) o a eventos del sistema de UI. Su lógica se basa en el ciclo de vida de Unity (`Start`, `Update`) y en los valores de sus variables públicas que son probablemente establecidos por otros scripts.

- **Eventos (Salida):**
    Este script no invoca `UnityEvent`s o `Action`s propios para notificar a otros sistemas. Sin embargo, interactúa directamente con otros sistemas clave:
    *   Llama a `AddPlayer(this)` y `RemovePlayer(this)` en instancias de `RockBehavior`, indicando su presencia o ausencia en una roca específica.
    *   Invoca `Combatjudge.combatjudge.ArriveAtRock()` cuando el token se mueve a una nueva roca, lo que actúa como una notificación directa al sistema de combate (o de juego general) sobre la finalización de un movimiento.