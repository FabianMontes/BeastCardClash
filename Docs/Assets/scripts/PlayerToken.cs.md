# `PlayerToken.cs`

## 1. Propósito General
Este script gestiona la representación visual y el movimiento de un token de jugador en el campo de juego. Su rol principal es asegurar que el token se posicione y se mueva correctamente entre las "rocas" (`RockBehavior`) designadas, coordinándose con el sistema de combate central (`Combatjudge`) para ejecutar movimientos durante el turno apropiado del jugador asociado.

## 2. Componentes Clave

### `PlayerToken`
-   **Descripción:** `PlayerToken` es un componente de Unity (`MonoBehaviour`) que controla el comportamiento de un token de jugador en 3D. Se encarga de la inicialización de la posición del token en una roca de inicio, la detección de la necesidad de movimiento hacia nuevas rocas, y la ejecución del movimiento físico utilizando un `CharacterController`.
-   **Variables Públicas / Serializadas:**
    -   `public RockBehavior rocky;`: Referencia a la instancia de `RockBehavior` que representa la roca *actual* o *destino* del `PlayerToken`. Se asigna en el Inspector o mediante código en tiempo de ejecución. Es crucial para el posicionamiento y la lógica de movimiento.
    -   `public Figther player;`: Referencia a la instancia de `Figther` asociada a este token de jugador. Se utiliza para determinar si es el turno de este jugador, comparando su `indexFigther` con el turno actual gestionado por `Combatjudge`.
    -   `public RockBehavior lastRock;`: Almacena la referencia a la `RockBehavior` de la cual el `PlayerToken` se ha movido (o se está moviendo). Se usa para notificar a la roca anterior que el jugador ya no está sobre ella.
-   **Métodos Principales:**
    -   `void Start()`: Método de ciclo de vida de Unity que se ejecuta una vez al inicio.
        -   Inicializa la referencia al componente `CharacterController` en el mismo GameObject.
        -   Si `rocky` está asignado, posiciona el token en la ubicación de `rocky`, lo registra en `rocky` llamando a `rocky.AddPlayer(this)`, y establece la rotación inicial basada en la rotación Y de `rocky`. También inicializa `destiny` a la posición de `rocky`.
    -   `void Update()`: Método de ciclo de vida de Unity que se ejecuta en cada frame. Contiene la lógica principal de movimiento del token.
        -   **Lógica de detección de movimiento:** Comprueba si `rocky` no es nulo y si es el turno del jugador (`player.indexFigther == Combatjudge.combatjudge.turn()`).
        -   Si la posición actual de `rocky` difiere de `destiny` (lo que indica que la roca actual ha cambiado o ha sido reasignada, o se ha movido), se desencadena una actualización de la ruta:
            -   El token se desvincula de `lastRock` (llamando a `lastRock.RemovePlayer(this)`).
            -   Se actualiza `destiny` a la nueva posición de `rocky`.
            -   Se calcula la `direction` hacia el `destiny` y se normaliza (ignorando el componente Y).
            -   El token se vincula a la nueva `rocky` (llamando a `rocky.AddPlayer(this)`).
            -   Se actualiza `lastRock` a la `rocky` actual.
            -   Se calcula y aplica una nueva rotación al token para que mire hacia `direction`.
            -   La función retorna, evitando que la lógica de movimiento se ejecute en el mismo frame que se inició una nueva ruta.
        -   **Lógica de ejecución de movimiento:** Si el `Combatjudge` indica que el estado actual es `SetMoments.MoveToRock`, el token se mueve físicamente hacia su `destiny` usando `characterController.Move()`.
            -   Una vez que la dirección restante hacia el destino (`dir`) difiere de la `direction` original (indicando que se ha llegado o pasado el destino), el token se ajusta con precisión a la `destiny`, se notifica al `Combatjudge` mediante `Combatjudge.combatjudge.ArriveAtRock()`, y el movimiento se detiene.
    -   `public float Speed()`: Un método auxiliar que calcula y devuelve la "velocidad" relativa del `CharacterController`. Divide la magnitud de la velocidad actual por un factor de 50, posiblemente para normalizarla para propósitos de animación o UI.

-   **Lógica Clave:**
    La lógica central del `PlayerToken` se basa en dos fases controladas por el método `Update`:
    1.  **Detección y Preparación del Movimiento:** Cada frame, el script verifica si la roca a la que está asignado (`rocky`) ha cambiado de posición o si se ha reasignado una nueva roca. Si esto ocurre, recalcula el destino y la dirección, y notifica a las rocas sobre el cambio de estado del jugador.
    2.  **Ejecución del Movimiento:** Una vez que se ha establecido un destino y la condición de `Combatjudge.combatjudge.GetSetMoments() == SetMoments.MoveToRock` se cumple, el `CharacterController` mueve el token hacia el destino. Se realiza una verificación de proximidad para "encajar" el token en la posición exacta del destino una vez que está lo suficientemente cerca, y se notifica al sistema de combate sobre la llegada. Este proceso solo ocurre si es el turno del jugador (`player.indexFigther == Combatjudge.combatjudge.turn()`).

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, pero *requiere* la presencia de un componente `CharacterController` en el mismo GameObject para su funcionamiento (`characterController = GetComponent<CharacterController>();`).
-   **Eventos (Entrada):** Este script no se suscribe directamente a `UnityEvent` o `Action` explícitos. En su lugar, se basa en la **consulta de estado** del sistema `Combatjudge` para determinar cuándo debe realizar acciones:
    -   Consulta `Combatjudge.combatjudge.turn()` para saber si es el turno del jugador asociado.
    -   Consulta `Combatjudge.combatjudge.GetSetMoments()` para verificar el estado actual del combate (específicamente `SetMoments.MoveToRock`).
-   **Eventos (Salida):** Este script invoca un método en el sistema `Combatjudge` para notificar un evento:
    -   `Combatjudge.combatjudge.ArriveAtRock()`: Se llama cuando el `PlayerToken` ha llegado a su roca de destino.