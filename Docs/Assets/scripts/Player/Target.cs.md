# `Target.cs`

## 1. Propósito General
Este script gestiona el movimiento de un GameObject dentro del entorno de Unity, ofreciendo dos modos de control distintos: un movimiento libre basado en teclado (flechas o WASD) relativo a la orientación de una cámara, y un modo de teletransporte directo a la posición de un clic de ratón mediante raycasting. Su función principal es permitir la manipulación posicional de un objeto "objetivo" en el juego, probablemente para fines de selección o interacción.

## 2. Componentes Clave

### `Target`
- **Descripción:** La clase `Target` hereda de `MonoBehaviour`, lo que le permite ser adjuntada a cualquier GameObject en una escena de Unity para controlar su posición. Su lógica central reside en alternar y ejecutar el comportamiento de movimiento seleccionado por el desarrollador o el diseñador de niveles.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] Transform playerCamera;`: Una referencia al `Transform` de la cámara del jugador. Es crucial para el modo de movimiento por teclado, ya que las direcciones de entrada (adelante, atrás, izquierda, derecha) se calculan en relación con la orientación de esta cámara para un control intuitivo.
    - `[SerializeField] float speed = 7f;`: Determina la velocidad a la que se mueve el GameObject cuando se utiliza el modo de control por teclado/WASD. Un valor más alto resultará en un movimiento más rápido.
    - `[SerializeField] bool useArrows = true;`: Un booleano que actúa como un selector de modo. Si es `true`, el movimiento se gestiona con las teclas de flecha o WASD. Si es `false`, el movimiento se realiza haciendo clic con el ratón.

- **Métodos Principales:**
    - `void Start()`:
        Este es un método del ciclo de vida de Unity que se invoca una vez al inicio del ciclo de vida del script. En la implementación actual, este método está vacío, lo que indica que no se requiere ninguna inicialización específica al inicio del juego para este componente.

    - `void Update()`:
        Este método del ciclo de vida se llama una vez por cada frame del juego. Contiene la lógica principal de movimiento del GameObject.
        *   Verifica el valor de la variable `useArrows` para determinar el modo de control activo.
        *   Si `useArrows` es `true`, invoca `GetInputDirection()` para calcular el vector de movimiento basado en la entrada del teclado y aplica este movimiento al `transform.position` del GameObject, multiplicando por `speed` y `Time.deltaTime` para un movimiento independiente de la tasa de frames.
        *   Si `useArrows` es `false`, detecta un clic izquierdo del ratón (`Input.GetMouseButtonDown(0)`). Si se detecta, lanza un rayo desde la posición del cursor del ratón en la pantalla hacia el mundo 3D (`Camera.main.ScreenPointToRay(Input.mousePosition)`). Si este rayo colisiona con algún objeto (`Physics.Raycast`), el `transform.position` del GameObject se actualiza instantáneamente a la posición del punto de impacto del rayo.

    - `Vector3 GetInputDirection()`:
        Este método auxiliar es privado y se utiliza para calcular el vector de dirección de movimiento cuando el modo `useArrows` está activo.
        *   Inicializa un vector `dir` a `Vector3.zero`.
        *   Comprueba individualmente si las teclas de flecha (Arriba, Abajo, Izquierda, Derecha) o sus equivalentes WASD (`W`, `S`, `A`, `D`) están siendo presionadas.
        *   Para cada tecla presionada, añade o resta la dirección `playerCamera.forward` o `playerCamera.right` al vector `dir`. Esto asegura que el movimiento sea relativo a la orientación de la cámara, proporcionando una experiencia de usuario más natural (e.g., 'W' siempre mueve hacia adelante respecto a la vista de la cámara).
        *   Fija el componente `y` del vector `dir` a `0`. Esto garantiza que el movimiento siempre ocurra en el plano horizontal (e.g., en el plano XZ), evitando cualquier movimiento vertical no deseado.
        *   Finalmente, devuelve el vector `dir` normalizado (`dir.normalized`). La normalización asegura que la magnitud del vector de dirección sea siempre 1, lo que previene que la velocidad del objeto aumente cuando se presionan múltiples teclas simultáneamente (e.g., adelante y derecha).

- **Lógica Clave:**
La lógica clave del script reside en la bifurcación del flujo de control dentro del método `Update`, basada en la variable `useArrows`. Esto establece un patrón de diseño que permite dos formas fundamentalmente diferentes de interactuar con la posición del objeto `Target`, ya sea a través de un control de movimiento continuo (teclado) o una interacción de teletransporte instantáneo (ratón). La función `GetInputDirection` encapsula la complejidad de mapear múltiples entradas de teclado a un vector de dirección cohesivo y relativo a la cámara.

## 3. Dependencias y Eventos

- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, al heredar de `MonoBehaviour`, requiere estar adjunto a un `GameObject` en la escena de Unity para funcionar. Además, para el modo de movimiento con el ratón, depende implícitamente de la existencia de una cámara principal en la escena (`Camera.main`).

- **Eventos (Entrada):**
    El script no se suscribe a eventos de Unity. En cambio, realiza un sondeo directo del estado de entrada del usuario (`Input.GetKey`, `Input.GetMouseButtonDown`) cada frame en el método `Update`. Esto es un patrón común para la gestión de entrada en Unity.

- **Eventos (Salida):**
    Este script no invoca ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Su única función es modificar directamente la posición del GameObject al que está adjunto.