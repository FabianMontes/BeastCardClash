Aquí tienes la documentación técnica para el archivo `PlayerToken.cs`:

---

# `PlayerToken.cs`

## 1. Propósito General
Este script gestiona la representación visual y la posición física de un token de jugador en el tablero de juego. Se encarga de sincronizar la ubicación del token con la "roca" a la que está asignado y de notificar los movimientos clave a otros sistemas del juego, como el `Combatjudge`.

## 2. Componentes Clave

### `PlayerToken`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, controla el GameObject visual que actúa como el token de un jugador (`Figther`) en el campo de juego. Su función principal es asegurar que la posición del token siempre coincida con la de la `RockBehavior` a la que está asociado. Además, inicializa la apariencia visual del token, como su color, basándose en la especie del jugador (`Figther`). El atributo `[DefaultExecutionOrder(1)]` indica que este script se ejecuta temprano en el ciclo de vida de los scripts de Unity, después de los scripts con orden de ejecución por defecto.

- **Variables Públicas / Serializadas:**
    - `public RockBehavior rocky;`: Una referencia al script `RockBehavior` de la "roca" actual donde el token del jugador está posicionado o donde se espera que esté. Esta variable se configura típicamente en el Inspector de Unity y puede ser actualizada por otros sistemas para mover el token.
    - `public Figther player;`: Una referencia al script `Figther` que representa al personaje del jugador asociado con este token. Se utiliza para obtener información sobre el jugador, como su especie, para propósitos visuales.
    - `public RockBehavior lastRock;`: Almacena una referencia al script `RockBehavior` de la última roca en la que estuvo el token. Esto es crucial para notificar a la roca anterior cuando el token se mueve de ella.

- **Métodos Principales:**
    - `void Start()`: Este método se invoca una vez al inicio del ciclo de vida del script.
        - Si `rocky` está asignado, el token se posiciona inicialmente en la misma ubicación que `rocky`. `lastRock` se establece en `rocky`, y se invoca `rocky.AddPlayer(this)` para registrar el token con la roca actual.
        - Se obtiene el `SpriteRenderer` del GameObject y su color se ajusta dináneos de acuerdo con la `Specie` del `player` asociado (camaleón, oso, serpiente, rana), proporcionando una diferenciación visual instantánea.

    - `void Update()`: Se llama una vez por cada frame del juego.
        - Si `rocky` está asignado, el método verifica continuamente si la posición actual del token (`transform.position`) difiere de la posición de la `rocky` asignada (`rocky.transform.position`).
        - Si detecta una diferencia (lo que implica que el token debe moverse o que su `rocky` de referencia ha cambiado), realiza una serie de acciones:
            - Notifica a la `lastRock` que el token se ha ido, llamando a `lastRock.RemovePlayer(this)`.
            - Actualiza la posición del token (`transform.position`) para que coincida con la de la `rocky` actual.
            - Registra el token con la nueva `rocky` invocando `rocky.AddPlayer(this)`.
            - Actualiza `lastRock` para que apunte a la `rocky` actual.
            - Finalmente, notifica al sistema de combate central (`Combatjudge`) que el token ha llegado a una nueva roca, llamando a `Combatjudge.combatjudge.ArriveAtRock()`, lo que puede desencadenar lógica de combate u otros eventos de juego.

- **Lógica Clave:**
    - **Sincronización de Posición:** El método `Update` implementa un mecanismo de "seguimiento" que asegura que el `PlayerToken` siempre se posicione visualmente en el mismo lugar que su `rocky` asignada. Esto permite que otros sistemas manipulen solo la referencia `rocky` o su posición, y el token visual se actualizará automáticamente.
    - **Gestión de Ocupación de Rocas:** El script coordina la entrada y salida de tokens en las `RockBehavior` llamando a los métodos `AddPlayer` y `RemovePlayer`. Esto sugiere que las `RockBehavior` mantienen un registro de los tokens que se encuentran sobre ellas.
    - **Visualización de Especies:** Durante la inicialización (`Start`), el token obtiene un color distintivo basado en la especie del jugador asociado. Esto facilita la identificación visual de los diferentes tipos de jugadores o criaturas en el tablero.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcto funcionamiento, el GameObject al que está adjunto debe tener un `SpriteRenderer` ya que intenta acceder y modificar su color en el método `Start`.
- **Eventos (Entrada):** Este script no se suscribe explícitamente a UnityEvents o eventos C# desde otros objetos. Sus entradas principales son los valores de sus variables públicas (`rocky`, `player`), que pueden ser configuradas en el Inspector o por otros scripts en tiempo de ejecución.
- **Eventos (Salida):**
    - Este script notifica a las instancias de `RockBehavior` cuando un token entra (`AddPlayer`) o sale (`RemovePlayer`) de una roca.
    - También notifica al sistema global `Combatjudge` cuando el token llega a una nueva roca, a través de la llamada a `Combatjudge.combatjudge.ArriveAtRock()`.

---