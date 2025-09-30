# Target
Este script `Target.cs` es un componente de MonoBehaviour diseñado para controlar la posición de un GameObject en la escena de Unity, sirviendo como un "selector" o "marcador" que el jugador puede manipular. Ofrece dos modos de interacción distintos para el movimiento del objeto, seleccionables a través de una variable booleana, permitiendo adaptarse a diferentes contextos de juego:

1.  **Movimiento por Teclado (WASD/Flechas):** El objeto se mueve continuamente en el plano horizontal según la dirección de las teclas de entrada (WASD o flechas), tomando como referencia la orientación de una cámara designada. La velocidad de movimiento es ajustable.
2.  **Movimiento por Clic de Ratón:** El objeto se posiciona instantáneamente en el punto del mundo detectado por un raycast lanzado desde la cámara principal hacia la posición del clic izquierdo del ratón. Esto es útil para la selección precisa de puntos en el entorno.

La flexibilidad en los modos de control sugiere que este componente podría utilizarse para diversas funciones dentro de *Beast Card Clash*, como la selección de casillas en un tablero, el marcado de un objetivo para una habilidad de carta, o el control de un cursor interactivo en un entorno 3D. El diseño prioriza la experiencia del desarrollador al permitir una fácil configuración de las opciones de control directamente desde el Inspector de Unity.

# Métodos

## Métodos de Unity

### Update
El método `Update` se ejecuta una vez por cada frame del juego y es el encargado de gestionar la lógica de movimiento del GameObject al que está adjunto el script, según el modo de entrada configurado.

```csharp
void Update()
{
    // Cambiar el modo de movimiento segun el modo elegido
    // true = flechas o WASD, false = mouse
    if (useArrows)
    {
        // ... lógica de movimiento por teclado ...
    }
    else
    {
        // ... lógica de movimiento por ratón ...
    }
}
```

La lógica principal dentro de `Update` es un condicional que evalúa el valor de la variable booleana `useArrows`:

*   **Si `useArrows` es `true` (Movimiento por Teclado):**
    *   El script llama al método `GetInputDirection()` para obtener un vector de dirección basado en las teclas presionadas por el jugador.
    *   Este vector se multiplica por la variable `speed` (velocidad de movimiento) y `Time.deltaTime` (para asegurar un movimiento fluido e independiente del framerate).
    *   El resultado se añade a la posición actual del `transform` del GameObject (`transform.position += ...`), provocando que se mueva continuamente.

*   **Si `useArrows` es `false` (Movimiento por Clic de Ratón):**
    *   El script detecta si se ha presionado el botón izquierdo del ratón (`Input.GetMouseButtonDown(0)`).
    *   Si se ha hecho clic, se crea un rayo (`Ray`) desde la cámara principal del juego (`Camera.main`) hacia la posición actual del ratón en la pantalla (`Input.mousePosition`).
    *   Se realiza una detección de colisiones (`Physics.Raycast`) con este rayo. Si el rayo impacta con cualquier objeto en la escena, la posición del GameObject se actualiza instantáneamente para coincidir con el punto exacto del impacto (`hit.point`). Esto permite al jugador "teletransportar" el objeto a la ubicación deseada con un simple clic.

## Otros métodos

### Vector3 GetInputDirection()
Este método es responsable de procesar la entrada del teclado y calcular la dirección de movimiento para el GameObject cuando el modo `useArrows` está activado.

```csharp
Vector3 GetInputDirection()
{
    Vector3 dir = Vector3.zero;

    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) dir += playerCamera.forward;
    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) dir -= playerCamera.right;
    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) dir -= playerCamera.forward;
    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) dir += playerCamera.right;

    dir.y = 0;
    return dir.normalized;
}
```

El funcionamiento detallado es el siguiente:

1.  **Inicialización:** Se declara un vector `dir` inicializado a `Vector3.zero`. Este vector acumulará la dirección total de la entrada del jugador.
2.  **Detección de Teclas:** Se verifica si se están presionando las teclas de flecha (Arriba, Abajo, Izquierda, Derecha) o sus equivalentes WASD.
    *   `Input.GetKey()` se utiliza en lugar de un `switch` para permitir que múltiples teclas se presionen simultáneamente (por ejemplo, "W" y "A" para movimiento diagonal), lo cual es una elección común para una experiencia de movimiento fluida.
    *   Las direcciones de movimiento (`playerCamera.forward` y `playerCamera.right`) se toman de la `playerCamera` asignada en el Inspector. Esto asegura que el movimiento sea relativo a la orientación de la cámara, es decir, "adelante" siempre será la dirección a la que apunta la cámara, "izquierda" su izquierda, y así sucesivamente. Esta flexibilidad es útil para sistemas de cámara dinámicos.
3.  **Restricción Vertical:** La componente `y` del vector `dir` se establece a `0` (`dir.y = 0;`). Esto asegura que el movimiento del GameObject sea puramente horizontal en el plano XZ, evitando cualquier elevación o descenso vertical no deseado.
4.  **Normalización:** Finalmente, el vector `dir` se normaliza (`return dir.normalized;`). Normalizar el vector garantiza que la magnitud de la dirección sea siempre 1, lo que significa que la velocidad del GameObject será consistente, independientemente de si se presiona una o varias teclas (evitando un aumento de velocidad al moverse en diagonal, por ejemplo).

## Getters y Setters

1.  `Transform playerCamera`: Establece una referencia a un componente `Transform` que representa la cámara del jugador. Esta cámara se utiliza para determinar las direcciones "adelante" y "derecha" relativas para el movimiento por teclado y para generar los rayos de detección de clics. La nota "No puede llamarse Camera por solapamiento" es una consideración importante para los desarrolladores, indicando un posible conflicto de nombres si se intentara usar un nombre genérico.
2.  `float speed`: Establece la velocidad a la que el GameObject se mueve cuando está activo el modo de control por teclado (WASD/Flechas). Este valor es un multiplicador aplicado a la dirección de entrada.
3.  `bool useArrows`: Controla el modo de entrada que el script utilizará para mover el GameObject. Si es `true`, el movimiento se gestionará mediante las teclas WASD o las flechas del teclado. Si es `false`, el movimiento se realizará mediante clics de ratón, teletransportando el objeto al punto de impacto del raycast.