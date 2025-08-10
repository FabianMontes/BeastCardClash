Aquí tienes la documentación técnica para el script `PlayerToken.cs`:

# `PlayerToken.cs`

## 1. Propósito General
Este script gestiona la representación visual y la posición de un token de jugador en el tablero de juego. Su rol principal es asegurar que el token se alinee con la "roca" (un objeto `RockBehavior`) designada para el jugador y notificar al sistema de combate cuando el jugador llega a una nueva ubicación.

## 2. Componentes Clave

### `PlayerToken`
- **Descripción:** Esta clase es un `MonoBehaviour` que controla el comportamiento visual y posicional de un token de jugador. Se encarga de colocar el token en la roca correcta, actualizar su posición en movimiento y notificar a los sistemas relevantes sobre la llegada a una nueva roca. El atributo `[DefaultExecutionOrder(1)]` asegura que su método `Start` se ejecute después de los scripts con orden de ejecución por defecto (0) pero antes de otros con números mayores, lo que puede ser importante si depende de la inicialización de otros componentes.
- **Variables Públicas / Serializadas:**
    - `public RockBehavior rocky;`: Referencia a la instancia de `RockBehavior` que representa la roca *actual* o *objetivo* sobre la que debe estar el token del jugador.
    - `public Figther player;`: Referencia al objeto `Figther` (presumiblemente el personaje del jugador) asociado con este token. Se utiliza para obtener información como la especie del jugador.
    - `public RockBehavior lastRock;`: Almacena la referencia a la `RockBehavior` en la que el token se encontraba *previamente*, útil para gestionar la salida de una roca y la entrada a otra.
- **Métodos Principales:**
    - `void Start()`:
        - **Descripción:** Se llama una vez en el ciclo de vida del script cuando el objeto se activa.
        - **Funcionalidad:**
            - Si `rocky` está asignado, inicializa la posición del `PlayerToken` a la misma posición que la de `rocky`.
            - Guarda la `rocky` actual en `lastRock`.
            - Llama a `rocky.AddPlayer(this)` para registrar este token en la roca actual.
            - Obtiene el componente `SpriteRenderer` del GameObject y establece su color basándose en la `Specie` del `player` asociado, proporcionando una distinción visual.
    - `void Update()`:
        - **Descripción:** Se llama una vez por frame.
        - **Funcionalidad:**
            - Monitorea continuamente si la posición de la `rocky` (la roca objetivo) ha cambiado con respecto a la posición actual del `PlayerToken`.
            - Si detecta un cambio (lo que indica que el jugador ha "movido" su token a una nueva roca a través de la actualización de `rocky` por otro sistema):
                - Elimina el token de la `lastRock` llamando a `lastRock.RemovePlayer(this)`.
                - Actualiza la posición del `PlayerToken` a la nueva `rocky.transform.position`.
                - Añade el token a la nueva `rocky` llamando a `rocky.AddPlayer(this)`.
                - Actualiza `lastRock` para que apunte a la nueva `rocky`.
                - Notifica al sistema de combate llamando a `Combatjudge.combatjudge.ArriveAtRock()`, lo que probablemente desencadena verificaciones o lógica de combate.
- **Lógica Clave:**
    La lógica principal de este script reside en su método `Update`, que actúa como un sistema de detección de movimiento pasivo. No inicia el movimiento por sí mismo, sino que reacciona a los cambios externos en la variable `rocky`. Cuando `rocky` se actualiza a una nueva posición (presumiblemente por una entrada del jugador o lógica de IA que designa una nueva roca para el jugador), el `PlayerToken` se reposiciona automáticamente. Esta reposición no solo es visual, sino que también gestiona las asociaciones del token con las rocas (entrada/salida) y crucialmente, informa al `Combatjudge` para posibles interacciones de combate.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - No utiliza explícitamente `[RequireComponent]`. Sin embargo, para que la funcionalidad de coloración del token en `Start()` funcione, el GameObject al que está adjunto este script debe tener un componente `SpriteRenderer`.
    - Implícitamente depende de la existencia y asignación de instancias de `RockBehavior` para sus variables `rocky` y `lastRock`, y una instancia de `Figther` para `player`.
- **Eventos (Entrada):**
    - Este script no se suscribe a eventos de `UnityEvent` o `Action` directamente. Su lógica de movimiento es reactiva a la asignación de la variable pública `rocky` por otros sistemas (por ejemplo, un controlador de jugador o un sistema de IA).
- **Eventos (Salida):**
    - Este script invoca métodos en otros sistemas, actuando como una notificación:
        - `rocky.AddPlayer(this)`: Notifica a la roca actual que este token está ahora sobre ella.
        - `lastRock.RemovePlayer(this)`: Notifica a la roca anterior que este token ya no está sobre ella.
        - `Combatjudge.combatjudge.ArriveAtRock()`: Notifica al sistema de `Combatjudge` que un token de jugador ha llegado a una nueva roca, lo que podría desencadenar la lógica de inicio de combate o verificación de condiciones.