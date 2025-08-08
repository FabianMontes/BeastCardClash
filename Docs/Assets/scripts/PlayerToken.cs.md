# `PlayerToken.cs`

## 1. Propósito General
Este script gestiona la representación visual y la posición en el tablero de un token de jugador. Actúa como el avatar del jugador en el juego, moviéndose entre "rocas" (nodos del tablero) y actualizando su apariencia según la especie del jugador.

## 2. Componentes Clave

### `PlayerToken`
- **Descripción:** La clase `PlayerToken` es un `MonoBehaviour` que representa la pieza física de un jugador en el tablero de juego. Se encarga de su posicionamiento, su asociación con la roca actual y su apariencia visual basada en la especie del jugador. El atributo `[DefaultExecutionOrder(1)]` asegura que este script se ejecute temprano en el ciclo de vida de Unity, permitiendo una configuración inicial rápida de la posición del token.

- **Variables Públicas / Serializadas:**
    - `public RockBehavior rocky;`: La instancia del script `RockBehavior` que representa la roca (o nodo) a la que el token del jugador está actualmente asignado o hacia la que se está moviendo. Se utiliza para obtener la posición y registrar la presencia del jugador.
    - `public Player player;`: La instancia del script `Player` asociado con este token. Contiene información sobre el jugador, como su especie, que se utiliza para la personalización visual del token.
    - `public RockBehavior lastRock;`: Almacena la instancia de `RockBehavior` de la roca donde el token del jugador estuvo previamente. Es crucial para desvincular el token de su roca anterior cuando se mueve a una nueva.

- **Métodos Principales:**
    - `void Start()`: Se llama una vez al inicio del ciclo de vida del script.
        - Inicializa la posición del `PlayerToken` a la posición de la `rocky` asignada, si `rocky` no es nula.
        - Registra el `PlayerToken` en la `rocky` inicial llamando a `rocky.AddPlayer(this)`.
        - Recupera el componente `SpriteRenderer` adjunto al GameObject y ajusta su color según la especie (`Specie`) del `player` asociado. Esto proporciona una diferenciación visual inmediata entre los tokens de los diferentes jugadores.

    - `void Update()`: Se llama una vez por cada frame.
        - Monitoriza continuamente si la posición del `PlayerToken` difiere de la posición de su `rocky` asignada.
        - Si hay un cambio de posición (indicando que la `rocky` ha cambiado o el token ha sido movido a una nueva `rocky`):
            - Desvincula el token de su `lastRock` (roca anterior) llamando a `lastRock.RemovePlayer(this)`.
            - Actualiza la posición del token a la de la nueva `rocky`.
            - Vincula el token a la nueva `rocky` llamando a `rocky.AddPlayer(this)`.
            - Actualiza `lastRock` para que apunte a la `rocky` actual.
            - Notifica al sistema de combate (`Combatjudge`) que un jugador ha llegado a una roca llamando a `Combatjudge.combatjudge.ArriveAtRock()`.

- **Lógica Clave:**
    La lógica principal de `PlayerToken` se centra en mantener su posición sincronizada con la de su roca asignada (`rocky`) y notificar a los sistemas relevantes cuando ocurre un movimiento. El script utiliza la variable `lastRock` para gestionar la transición entre rocas, asegurándose de que el token se desvincule correctamente de su ubicación anterior antes de vincularse a la nueva. La actualización del color en `Start` permite una fácil identificación visual del jugador.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    - Este script requiere un componente `SpriteRenderer` en el mismo GameObject para poder cambiar el color del token y representarlo visualmente.

- **Eventos (Entrada):**
    - Este script no se suscribe directamente a `UnityEvent`s o `Action`s externos. Su comportamiento reactivo se basa en el cambio de la propiedad `transform.position` de la `rocky` asignada, la cual es verificada en el método `Update`.

- **Eventos (Salida):**
    - **Interacción con `RockBehavior`:** El script invoca métodos en instancias de `RockBehavior` (`AddPlayer(this)` y `RemovePlayer(this)`) para registrar y desregistrar la presencia del token en una roca.
    - **Notificación al `Combatjudge`:** Notifica al sistema de combate principal (`Combatjudge`) llamando a `Combatjudge.combatjudge.ArriveAtRock()` cada vez que el token llega a una nueva roca. Esto probablemente desencadena la lógica de combate o eventos relacionados con el movimiento en el tablero.