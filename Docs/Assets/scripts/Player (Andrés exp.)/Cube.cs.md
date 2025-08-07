Aquí está la documentación técnica para el archivo `Cube.cs`, siguiendo las directrices proporcionadas:

# `Cube.cs`

## 1. Propósito General
Este script `Cube.cs` gestiona el movimiento de un objeto en la escena de Unity, permitiendo al jugador controlar su posición a través de dos modos de entrada diferentes: navegación con teclado/WASD o teletransporte mediante clic del mouse. Su función principal es proporcionar una base para la interacción del jugador con un objeto móvil, adaptándose a diversas mecánicas de control.

## 2. Componentes Clave

### `Cube`
- **Descripción:** La clase `Cube` es un `MonoBehaviour`, lo que significa que puede adjuntarse a un `GameObject` en Unity y participa en el ciclo de vida del motor. Se encarga de procesar la entrada del usuario y aplicar el movimiento correspondiente al `GameObject` al que está asociado.

- **Variables Públicas / Serializadas:**
    - `[SerializeField] Transform playerCamera`: Un `Transform` que debe ser asignado en el Inspector de Unity. Se utiliza para determinar las direcciones "adelante" y "derecha" relativas a la cámara, asegurando que el movimiento del objeto sea intuitivo desde la perspectiva del jugador.
    - `[SerializeField] float speed = 5f`: Un valor flotante que controla la velocidad a la que el objeto se mueve cuando se utiliza el modo de control por teclado/WASD. Permite ajustar fácilmente la velocidad de desplazamiento desde el editor.
    - `[SerializeField] bool mode = true`: Un booleano que define el modo de control activo. Si es `true`, el objeto se moverá continuamente con las teclas de dirección o WASD. Si es `false`, el objeto se teletransportará a la posición del mouse al hacer clic izquierdo.

- **Métodos Principales:**
    - `void Start()`: Este método del ciclo de vida de Unity se llama una vez antes de la primera actualización del frame. En la implementación actual de `Cube.cs`, no contiene lógica, lo que indica que no se requiere una inicialización específica del script al inicio del juego.
    - `void Update()`: Este método del ciclo de vida de Unity se llama una vez por frame. Contiene la lógica principal para el movimiento del objeto. Dependiendo del valor de la variable `mode`, ejecuta uno de los dos comportamientos de movimiento:
        - Si `mode` es `true`, calcula la dirección de entrada del teclado y actualiza la posición del objeto continuamente, multiplicando la dirección por la `speed` y `Time.deltaTime` para un movimiento suave e independiente de la tasa de frames.
        - Si `mode` es `false`, detecta un clic izquierdo del mouse (`Input.GetMouseButtonDown(0)`). Al detectar el clic, crea un rayo desde la cámara principal hacia la posición del mouse en la pantalla y realiza un `Physics.Raycast`. Si el rayo impacta con cualquier colisionador en la escena, el objeto se teletransporta instantáneamente a la posición del punto de impacto (`hit.point`).
    - `Vector3 GetInputDirection()`: Un método auxiliar invocado cuando `mode` es `true`. Este método privado calcula y devuelve un vector de dirección basado en las entradas del teclado (flechas o WASD). Identifica si se presionan las teclas de "adelante" (Arriba, W), "atrás" (Abajo, S), "izquierda" (Izquierda, A) o "derecha" (Derecha, D). La dirección se construye en relación con la orientación de la `playerCamera`. Finalmente, normaliza el vector resultante para asegurar una velocidad constante en cualquier dirección diagonal y fija el componente `y` a cero, restringiendo el movimiento al plano horizontal.

- **Lógica Clave:**
    La lógica central del script reside en el método `Update`, donde se implementa una bifurcación de comportamiento basada en la variable booleana `mode`. Esto permite al desarrollador alternar fácilmente entre dos paradigmas de control del jugador:
    1.  **Movimiento Continúo (modo `true`):** La posición del objeto se actualiza incrementalmente en cada frame. La dirección se obtiene del método `GetInputDirection()`, que mapea las entradas del teclado a vectores direccionales relativos a la cámara. El uso de `Time.deltaTime` garantiza que el movimiento sea fluido y consistente en diferentes tasas de frames.
    2.  **Teletransporte por Clic (modo `false`):** Este modo transforma la entrada del mouse en un punto en el espacio 3D. Un `Raycast` desde la cámara al mundo es crucial aquí; el objeto se mueve instantáneamente al punto donde el rayo colisiona con la geometría de la escena. Esto es útil para interacciones de apuntar y hacer clic o navegación de mapa.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no impone la presencia de otros componentes específicos en el `GameObject` al que se adjunta. Sin embargo, para su funcionalidad completa en el modo de teletransporte, requiere la existencia de una `Camera.main` en la escena y colisionadores en los objetos con los que se espera interactuar mediante raycast.
- **Eventos (Entrada):**
    - El script lee directamente el estado de las teclas del teclado (`Input.GetKey`) para el modo de movimiento continuo (flechas y WASD).
    - Para el modo de teletransporte, se suscribe implícitamente al evento de clic del botón izquierdo del mouse (`Input.GetMouseButtonDown(0)`).
    - Para ambos modos, interactúa con la `playerCamera` para obtener vectores direccionales relativos.
- **Eventos (Salida):** Este script no invoca explícitamente ningún `UnityEvent` o `Action` personalizado para notificar a otros sistemas sobre su estado o acciones. Su impacto se limita a la manipulación directa de la posición de su propio `Transform`.