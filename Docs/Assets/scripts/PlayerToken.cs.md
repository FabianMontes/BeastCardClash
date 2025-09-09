# PlayerToken
`PlayerToken` es un script clave para la representación visual y el control del movimiento del personaje de un jugador dentro del tablero de juego. Este componente actúa como el avatar de una instancia de `Figther` (luchador), encargado de navegar entre nodos del tablero, que se modelan como objetos `RockBehavior` (rocas). Su función principal es gestionar la inicialización de la posición del token en una roca de partida, coordinar su movimiento hacia nuevas rocas basándose en la lógica del juego (específicamente cuando el turno del `Figther` asociado está activo y el estado del juego permite el movimiento), y notificar al `CombatJudge` central sobre su llegada.

El script utiliza el componente `CharacterController` de Unity para un movimiento suave y gestiona su interacción con las instancias de `RockBehavior` para mantener un registro de su ubicación en el tablero. El movimiento está intrínsecamente ligado al patrón Singleton `CombatJudge`, que asegura que las acciones solo se ejecuten durante el turno asignado al jugador y en la fase de juego correcta.

Es importante destacar la directiva `[DefaultExecutionOrder(1)]`, que garantiza que este script se ejecute después de la mayoría de los scripts con la orden de ejecución predeterminada. Esto puede ser crucial para asegurar que otros componentes de los que `PlayerToken` depende (como `RockBehavior` o `Figther`) estén completamente inicializados antes de que `PlayerToken` intente acceder a ellos en `Start`.

# Métodos

## Métodos de Unity

### Start
Este método se invoca una única vez antes del primer frame de actualización, siendo el punto de inicialización para el `PlayerToken` y su posición inicial en el tablero de juego.

El proceso de `Start` incluye:
1.  **Obtención del `CharacterController`**: Se busca y asigna el componente `CharacterController` adjunto al mismo GameObject. Este componente es fundamental para el manejo del movimiento del personaje.
2.  **Inicialización de posición**: Si se ha asignado una instancia de `RockBehavior` a la variable pública `rocky` (lo que indica una roca inicial para el token del jugador):
    *   La posición del `PlayerToken` se ajusta para que coincida exactamente con la posición de la `rocky` asignada.
    *   La `rocky` actual se almacena en la variable `lastRock` para registrar la ubicación previa del token.
    *   Se invoca el método `AddPlayer` en la instancia de `rocky` para registrar que este `PlayerToken` se encuentra ahora sobre ella.
    *   La rotación del `PlayerToken` se alinea en el eje Y con la rotación de la `rocky` para una orientación consistente.
    *   La `destiny` (posición objetivo) se establece en la posición de la `rocky`, asegurando que el token se considere "en su destino" desde el principio del juego.

```csharp
void Start()
{
    characterController = GetComponent<CharacterController>();

    if (rocky != null)
    {
        transform.position = rocky.transform.position;
        lastRock = rocky;
        rocky.AddPlayer(this);
        Vector3 rot = Vector3.Scale(Vector3.up,  rocky.transform.rotation.eulerAngles);
        transform.rotation = Quaternion.Euler(rot);
        destiny = rocky.transform.position;
    }
}
```

### Update
Este método se ejecuta en cada frame del juego y alberga la lógica principal para el movimiento del token del jugador y sus interacciones basadas en turnos.

La ejecución de toda la lógica dentro de `Update` está sujeta a dos condiciones fundamentales:
*   La variable `rocky` no debe ser nula, garantizando que el token siempre tenga una roca objetivo o actual.
*   El `indexFigther` (identificador único del jugador/luchador asociado) del `player` debe coincidir con el `Turn()` actual devuelto por la instancia Singleton `CombatJudge.CombatJudgeInstance`. Esta condición asegura que el token solo realice acciones durante el turno de su jugador correspondiente.

Dentro de estas condiciones, `Update` realiza dos comprobaciones principales:

1.  **Detección y orientación hacia una nueva roca:**
    Este bloque de código se activa si la posición de la `rocky` actual ha cambiado con respecto a la `destiny` almacenada. Esto generalmente ocurre cuando se le asigna una nueva `rocky` al jugador para que se mueva.
    *   El token se remueve de su `lastRock` utilizando el método `RemovePlayer()`.
    *   La variable `destiny` se actualiza con la nueva `rocky.transform.position`.
    *   Se calcula un vector `direction` desde la posición actual del token hasta la `destiny`, se normaliza y se anula su componente Y para garantizar un movimiento estrictamente horizontal.
    *   El token se registra con la *nueva* `rocky` utilizando `AddPlayer()` y se actualiza `lastRock`.
    *   La rotación del token se ajusta para que mire hacia la `direction` calculada, utilizando `Mathf.Atan2` para calcular el ángulo en el plano horizontal.
    *   Una sentencia `return;` finaliza la ejecución de `Update` en este frame, permitiendo que el token se reoriente completamente antes de intentar el movimiento en el siguiente frame.

    ```csharp
    if (rocky.transform.position != destiny)
    {
        lastRock.RemovePlayer(this);
        // Código comentado: Indicaciones de ideas de desarrollo previas sobre la gestión de múltiples jugadores en una misma roca.
        destiny = rocky.transform.position;
        direction = destiny - transform.position;
        direction.y = 0;
        direction= direction.normalized;
        rocky.AddPlayer(this);
        lastRock = rocky;
        float angulo = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion rotacion = Quaternion.Euler(0, angulo, 0);
        transform.rotation = rotacion;
        return;
    }
    ```

2.  **Movimiento hacia la roca destino:**
    Este bloque de código se ejecuta únicamente si el estado actual del juego, obtenido a través de `CombatJudge.CombatJudgeInstance.GetSetMoments()`, es `SetMoments.MoveToRock`.
    *   El método `characterController.Move()` se emplea para desplazar el token a lo largo del vector `direction` a una velocidad fija de `50` unidades por segundo.
    *   Se realiza una comprobación para determinar si el token ha alcanzado o sobrepasado su `destiny`. Se recalcula un vector `dir` desde la posición actual hasta la `destiny` y se compara con el `direction` original. Debido a la precisión de punto flotante, la condición `dir != direction` puede indicar efectivamente que el token ha llegado o excedido su objetivo.
    *   Si se confirma que el token ha llegado a su `destiny`:
        *   Su posición se ajusta precisamente a `destiny` para evitar cualquier sobrepaso.
        *   Se invoca `CombatJudge.CombatJudgeInstance.ArriveAtRock()` para notificar al juez del combate que la fase de movimiento para este token ha concluido.
        *   Se llama a `characterController.Move(Vector3.zero)` para asegurar que el CharacterController detenga cualquier movimiento residual.

    ```csharp
    if (CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.MoveToRock)
    {
        characterController.Move(direction * Time.deltaTime * 50);
        Vector3 dir = destiny - transform.position;
        dir.y = 0;
        dir = dir.normalized;
        if (dir != direction && CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.MoveToRock)
        {
            transform.position = destiny;
            CombatJudge.CombatJudgeInstance.ArriveAtRock();
            characterController.Move(Vector3.zero);
        }
    }
    ```

## Otros métodos

### Speed(): float
Este método público ofrece una interfaz para consultar la velocidad normalizada actual del `PlayerToken`.
Devuelve la magnitud de la velocidad actual del `CharacterController` dividida por `50`. La división por `50` sirve para normalizar la velocidad en relación con la velocidad de movimiento fija de `50` unidades por segundo utilizada en el método `Update`. Esta información puede ser valiosa para propósitos como la mezcla de animaciones, la retroalimentación en la interfaz de usuario, o cualquier otro sistema de juego que requiera conocer la velocidad actual del token.

```csharp
public float Speed()
{
    return characterController.velocity.magnitude/50f;
}
```

## Getters y Setters
Este script utiliza campos públicos para permitir que otros componentes accedan y modifiquen directamente datos relevantes.

1.  `rocky: RockBehavior`: Permite establecer o consultar la instancia de `RockBehavior` que representa la roca actual o el destino al que el `PlayerToken` debe moverse.
2.  `player: Figther`: Permite establecer o consultar la instancia de `Figther` asociada a este `PlayerToken`, la cual es crucial para identificar al jugador y acceder a sus propiedades, como `indexFigther`.
3.  `lastRock: RockBehavior`: Permite establecer o consultar la instancia de `RockBehavior` que el token ocupó inmediatamente antes, siendo útil para gestionar la salida de una roca y la entrada a otra.