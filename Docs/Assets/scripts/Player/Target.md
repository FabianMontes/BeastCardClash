# `Target`
Este script de `MonoBehaviour` está diseñado para controlar el movimiento de un `GameObject` al que se adjunta, ofreciendo dos modos de entrada principales: por teclado (flechas o WASD) o por ratón. Su propósito principal es permitir a los jugadores desplazar un objeto "objetivo" o "cursor" dentro del entorno del juego, lo que sugiere que podría utilizarse para la selección de casillas en un tablero, el posicionamiento de unidades, o la interacción con elementos del escenario en un juego de estrategia por turnos como **Beast Card Clash**.

El script expone varias variables configurables en el Inspector de Unity, lo que facilita a los diseñadores y a otros programadores ajustar el comportamiento sin necesidad de modificar el código directamente:

*   **`playerCamera`**: Una referencia al `Transform` de la cámara del jugador, crucial para determinar las direcciones de movimiento relativas a la vista del jugador.
*   **`speed`**: Controla la velocidad de movimiento del objeto cuando se utiliza el modo de teclado.
*   **`useArrows`**: Un `booleano` que alterna entre los dos modos de entrada (teclado si es `true`, ratón si es `false`).

En resumen, `Target.cs` proporciona una base versátil para la interacción del jugador con el entorno, permitiendo el movimiento del objeto objetivo mediante dos paradigmas de control comunes en los videojuegos.

# Métodos

## Métodos de Unity

### `Update()`
El método `Update()` se invoca una vez por cada frame del juego y es el corazón de la lógica de movimiento del script. Su función principal es detectar la entrada del jugador y mover el `GameObject` en consecuencia, basándose en el modo de control (`useArrows`) que esté activo.

```csharp
void Update()
{
    if (useArrows)
    {
        transform.position += GetInputDirection() * speed * Time.deltaTime;
    }
    else
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) transform.position = hit.point;
        }
    }
}
```

*   **Movimiento por teclado (cuando `useArrows` es `true`):**
    *   Llama a `GetInputDirection()` para obtener el vector de dirección normalizado basado en las teclas presionadas (flechas o WASD).
    *   Multiplica este vector por la variable `speed` (velocidad de movimiento) y `Time.deltaTime` (para asegurar un movimiento independiente de la tasa de frames) y lo añade a la posición actual del `GameObject` (`transform.position`). Esto resulta en un movimiento suave y continuo.

*   **Movimiento por ratón (cuando `useArrows` es `false`):**
    *   Detecta un clic del botón izquierdo del ratón (`Input.GetMouseButtonDown(0)`).
    *   Si se detecta un clic, crea un `Ray` (rayo) desde la posición de la cámara principal hacia la posición actual del puntero del ratón en la pantalla (`Camera.main.ScreenPointToRay(Input.mousePosition)`).
    *   Realiza un `Physics.Raycast` para verificar si este rayo colisiona con algún `Collider` en la escena.
    *   Si el rayo colisiona (`if (Physics.Raycast(ray, out RaycastHit hit)`), el `GameObject` se mueve instantáneamente a la posición del punto de impacto de la colisión (`hit.point`).

## Otros métodos

### `Vector3 GetInputDirection()`
Este método privado es responsable de calcular la dirección de movimiento del jugador cuando el modo de control por teclado está activo (`useArrows = true`). Traduce las pulsaciones de teclas (flechas o WASD) en un vector de dirección 3D.

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

*   **Inicialización**: Comienza con un vector de dirección `dir` inicializado a `Vector3.zero`.
*   **Detección de Teclas**: Utiliza múltiples sentencias `if (Input.GetKey(...))` para detectar las pulsaciones de teclas simultáneas. Esto permite al jugador presionar varias teclas a la vez (por ejemplo, "W" y "A" para moverse diagonalmente).
    *   Las direcciones se calculan relativas al `Transform` de la `playerCamera` (`playerCamera.forward` y `playerCamera.right`), lo que significa que "adelante" siempre será hacia donde mira la cámara, independientemente de la rotación del `GameObject` al que está adjunto el script.
*   **Normalización y Planaridad**:
    *   `dir.y = 0;`: Fija el componente `y` del vector de dirección a cero, asegurando que el movimiento del `GameObject` sea estrictamente horizontal y no se desplace verticalmente.
    *   `return dir.normalized;`: Normaliza el vector de dirección. Esto es crucial para mantener una velocidad de movimiento constante, incluso cuando se presionan múltiples teclas (evitando que el movimiento diagonal sea más rápido que el movimiento ortogonal).

## Getters y Setters

Este script no implementa métodos públicos explícitos de tipo getter o setter para sus variables. Las variables `playerCamera`, `speed` y `useArrows` son campos serializados (`[SerializeField]`), lo que significa que son privadas pero pueden ser configuradas y modificadas directamente a través del Inspector de Unity en el editor. Para la interacción programática desde otros scripts, sería necesario hacerlas públicas o implementar propiedades.