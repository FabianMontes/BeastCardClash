# `PlayerToken`
Este script, adjunto a un GameObject, representa la ficha visual de un jugador o "luchador" en el tablero de juego de `Beast Card Clash`. Su función principal es gestionar la posición, movimiento y orientación de esta ficha en relación con los `RockBehavior` (rocas) del tablero, así como su interacción con el sistema de turnos y estados de combate a través del `CombatJudge`. El script se asegura de que el token se inicialice en una roca designada, se mueva entre rocas según las decisiones de combate y se mantenga sincronizado con la roca actual durante el turno del jugador asociado.

El uso de `DefaultExecutionOrder(1)` indica que este script se ejecutará ligeramente después de los scripts con la orden de ejecución por defecto, lo que podría ser útil para asegurar que ciertos componentes o estados del juego (como la configuración inicial de las rocas o el `CombatJudge`) ya estén listos antes de que el `PlayerToken` intente acceder a ellos.

# Métodos

## Métodos de Unity

### `Start()`
Este método se ejecuta una vez al inicio del ciclo de vida del script. Su propósito es inicializar el `PlayerToken` en el tablero.

1.  **Obtención del `CharacterController`**:
    Se obtiene una referencia al componente `CharacterController` adjunto al mismo GameObject. Este componente se utiliza para el movimiento y la gestión de colisiones del token en el juego.
    ```csharp
    characterController = GetComponent<CharacterController>();
    ```
2.  **Inicialización en una roca**:
    Si la variable pública `rocky` (que representa la roca inicial del token) no es nula, el token se posiciona en la ubicación de esa roca.
    *   Se establece la posición del `PlayerToken` en la misma posición que la `rocky` asignada.
    *   `lastRock` se asigna a `rocky`, almacenando la roca actual.
    *   Se llama al método `AddPlayer(this)` en el objeto `rocky`, lo que sugiere que cada `RockBehavior` lleva un registro de los jugadores que están sobre ella.
    *   La rotación del token se ajusta para coincidir con la rotación Y de la roca, asegurando una orientación coherente.
    *   `destiny` (el punto de destino del movimiento) se establece en la posición de la roca.
    ```csharp
    if (rocky != null)
    {
        transform.position = rocky.transform.position;
        lastRock = rocky;
        rocky.AddPlayer(this);
        Vector3 rot = Vector3.Scale(Vector3.up, rocky.transform.rotation.eulerAngles);
        transform.rotation = Quaternion.Euler(rot);
        destiny = rocky.transform.position;
    }
    ```

### `Update()`
Este método se llama una vez por cada frame del juego y es el corazón del comportamiento del `PlayerToken` en tiempo real.

1.  **Condiciones de ejecución**:
    La lógica principal de `Update` solo se ejecuta si:
    *   Hay una `rocky` asignada al token.
    *   El `indexFighter` del jugador asociado (`player.indexFighter`) coincide con el turno actual del combate, obtenido a través de `CombatJudge.Instance.Turn()`. Esto asegura que el token solo sea interactivo y se mueva durante el turno de su jugador.
    ```csharp
    if (rocky != null && player.indexFighter == CombatJudge.Instance.Turn())
    {
        // ... lógica de movimiento ...
    }
    ```
2.  **Sincronización con la roca**:
    Si la posición de la `rocky` actual no coincide con la `destiny` registrada (lo que podría ocurrir si la roca misma se mueve o se reubica el token), se actualiza la posición del token y se recalcula la dirección hacia la roca.
    *   Se elimina el token de la `lastRock` anterior y se asigna la nueva `rocky` a `lastRock`.
    *   Se actualiza `destiny` a la posición actual de `rocky`.
    *   Se calcula `direction` como el vector normalizado desde la posición actual del token hasta `destiny` (ignorando el eje Y).
    *   Se vuelve a añadir el token a la `rocky` actual.
    *   El token rota para mirar hacia la `direction` calculada.
    *   Se incluye una sentencia `return;` que detiene la ejecución del resto del `Update` para este frame si esta lógica de resincronización se ejecuta. Esto evita que el token intente moverse antes de estar correctamente posicionado y orientado.
    ```csharp
    if (rocky.transform.position != destiny)
    {
        lastRock.RemovePlayer(this);
        destiny = rocky.transform.position;
        direction = destiny - transform.position;
        direction.y = 0;
        direction = direction.normalized;
        rocky.AddPlayer(this);
        lastRock = rocky;
        float angulo = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion rotacion = Quaternion.Euler(0, angulo, 0);
        transform.rotation = rotacion;
        return; // Sale de Update para evitar movimientos erróneos en el mismo frame.
    }
    ```
3.  **Gestión del movimiento hacia la roca**:
    Si el estado actual del combate, obtenido de `CombatJudge.Instance.GetSetMoments()`, es `SetMoments.MoveToRock`, el token se mueve hacia su `destiny`.
    *   El `characterController` mueve el token en la `direction` a una velocidad de `50` unidades por segundo.
    *   Se verifica si el token ha alcanzado (o sobrepasado ligeramente) su `destiny`. Si la `direction` actual hacia el destino difiere de la `direction` calculada previamente (lo que indica que el token ha pasado el punto de destino), se considera que ha llegado.
    *   En ese caso, la posición del token se ajusta exactamente a `destiny`.
    *   Se notifica al `CombatJudge` que el token ha llegado a la roca llamando a `CombatJudge.Instance.ArriveAtRock()`.
    *   El `characterController` detiene su movimiento para evitar que el token se desplace más allá del punto de destino.
    ```csharp
    if (CombatJudge.Instance.GetSetMoments() == SetMoments.MoveToRock)
    {
        characterController.Move(direction * Time.deltaTime * 50);
        Vector3 dir = destiny - transform.position;
        dir.y = 0;
        dir = dir.normalized;
        if (dir != direction && CombatJudge.Instance.GetSetMoments() == SetMoments.MoveToRock)
        {
            transform.position = destiny;
            CombatJudge.Instance.ArriveAtRock();
            characterController.Move(Vector3.zero);
        }
    }
    ```
    > [!NOTE]
    > La condición `dir != direction` para detectar la llegada a destino es una técnica común en juegos indie para saber si un objeto ha "sobrepasado" su punto de destino, ya que la aritmética de punto flotante puede hacer que nunca sea exactamente igual.

## Otros métodos

### `Speed(): float`
Este método público devuelve la velocidad actual del `CharacterController` del token, normalizada.
Divide la magnitud de la velocidad del `characterController` por `50f` (el factor de velocidad utilizado en el método `Update`). Esto proporciona una medida de la velocidad del token en relación con su velocidad máxima de movimiento definida.

```csharp
public float Speed()
{
    return characterController.velocity.magnitude / 50f;
}
```

## Getters y Setters
Aunque no hay propiedades con `get` y `set` explícitos en este script, la naturaleza de algunos campos y métodos puede ser vista como un "getter".

1.  `Speed()`: Devuelve la velocidad normalizada del `PlayerToken`.