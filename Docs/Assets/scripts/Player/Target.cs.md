# `Target.cs`

## 1. Propósito General
Este script gestiona el movimiento de un objeto en el juego, ofreciendo dos modos de control de entrada seleccionables: movimiento continuo mediante teclado/WASD y teleportación instantánea a la posición de un clic de ratón. Interactúa principalmente con el sistema de entrada de Unity y el motor de físicas para la detección de colisiones en el modo de ratón.

## 2. Componentes Clave

### `Target`
- **Descripción:** `Target` es una clase que hereda de `MonoBehaviour`, lo que le permite ser adjuntada a un GameObject en Unity y participar en el ciclo de vida del motor. Su función principal es controlar cómo el objeto al que está adjunto se mueve en respuesta a la entrada del jugador, proporcionando una interfaz flexible para diferentes esquemas de control.

- **Variables Públicas / Serializadas:**
    - `playerCamera` (`Transform`): Esta variable, marcada con `[SerializeField]`, es una referencia al `Transform` de la cámara del jugador. Es fundamental para el modo de movimiento de teclado/WASD, ya que permite que el movimiento del objeto sea relativo a la orientación de la cámara (hacia adelante, hacia los lados de la cámara), en lugar de direcciones fijas globales. El comentario en el código aclara que no puede llamarse `Camera` debido a posibles solapamientos con la clase `UnityEngine.Camera`.
    - `speed` (`float`): También serializada, esta variable define la velocidad a la que el objeto se mueve cuando el modo de teclado/WASD está activo. Un valor mayor resultará en un movimiento más rápido del objeto en cada fotograma.
    - `useArrows` (`bool`): Una variable booleana serializada que actúa como un interruptor para alternar entre los dos modos de control de movimiento. Si su valor es `true`, el objeto se moverá mediante las teclas de flecha o WASD. Si es `false`, el objeto se moverá a la posición del mundo donde se haga clic con el ratón.

- **Métodos Principales:**
    - `void Start()`: Este es un método de ciclo de vida de Unity que se llama una vez, justo antes de la primera actualización del fotograma, después de que el script ha sido inicializado. En la implementación actual, el método `Start` está vacío y no realiza ninguna acción de inicialización específica.

    - `void Update()`: Este método de ciclo de vida de Unity se llama una vez por fotograma. Es el corazón de la lógica de movimiento del script.
        - Dentro de `Update`, se evalúa la variable `useArrows` para determinar el modo de movimiento activo.
        - Si `useArrows` es `true`, el script invoca el método `GetInputDirection()` para obtener un vector de dirección basado en la entrada del teclado y lo utiliza para actualizar incrementalmente la posición del objeto. El movimiento se calcula multiplicando la dirección por `speed` y `Time.deltaTime` (para asegurar que el movimiento sea suave e independiente de la tasa de fotogramas):
          ```csharp
          transform.position += GetInputDirection() * speed * Time.deltaTime;
          ```
        - Si `useArrows` es `false`, el script espera un clic del botón izquierdo del ratón (`Input.GetMouseButtonDown(0)`). Si se detecta un clic, se crea un `Ray` desde la cámara principal hacia la posición del cursor del ratón en la pantalla. Luego, se realiza un `Physics.Raycast` para detectar si el rayo impacta con algún `Collider` en la escena. Si hay un impacto, la posición del objeto se actualiza instantáneamente al punto de impacto del rayo:
          ```csharp
          Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
          if (Physics.Raycast(ray, out RaycastHit hit))
          {
              transform.position = hit.point;
          }
          ```

    - `Vector3 GetInputDirection()`: Este es un método auxiliar privado que se encarga de procesar la entrada del teclado y calcular la dirección de movimiento correspondiente para el modo de teclado/WASD.
        - Inicializa un vector de dirección (`dir`) a `Vector3.zero`.
        - Comprueba si las teclas de flecha (Arriba, Abajo, Izquierda, Derecha) o sus equivalentes WASD (`W`, `A`, `S`, `D`) están siendo presionadas usando `Input.GetKey()`. Si una tecla está presionada, se añade o resta la dirección `playerCamera.forward` o `playerCamera.right` al vector `dir`. Se usa una serie de `if` statements en lugar de un `switch` para permitir que múltiples teclas sean presionadas simultáneamente (por ejemplo, adelante y derecha).
        - Antes de devolver el vector, el componente `y` de `dir` se establece en `0` (`dir.y = 0;`). Esto asegura que el movimiento sea puramente horizontal, evitando que el objeto se mueva hacia arriba o hacia abajo.
        - Finalmente, el vector `dir` se normaliza (`dir.normalized`). Esto garantiza que la magnitud del vector sea 1, lo que previene que la velocidad de movimiento aumente cuando se presionan múltiples teclas (por ejemplo, adelante y derecha al mismo tiempo) y asegura una velocidad constante en cualquier dirección.

- **Lógica Clave:** La lógica principal del script es la implementación de un sistema de control de movimiento dual. El método `Update` actúa como un despachador, dirigiendo el flujo de ejecución hacia el control basado en teclado/WASD (utilizando `GetInputDirection`) o hacia el control basado en clic del ratón (utilizando raycasting) según el valor de la variable `useArrows`. El método `GetInputDirection` es crucial para traducir la entrada del jugador en un vector de movimiento normalizado y horizontal, que es luego aplicado al `Transform` del objeto.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]` para forzar la presencia de otros componentes en el mismo GameObject. Sin embargo, funcionalmente requiere un `Transform` (que es inherente a todos los GameObjects). En el modo de movimiento con el ratón, depende de la existencia de un `Camera.main` en la escena para generar el rayo y necesita que haya `Collider`s en el entorno para que el raycast pueda detectar un punto de impacto válido. Para el modo de movimiento relativo a la cámara, la variable `playerCamera` debe estar asignada a un `Transform` de cámara válido en el Inspector de Unity.

- **Eventos (Entrada):**
    El script reacciona directamente a los eventos de entrada del usuario proporcionados por la clase `Input` de Unity:
    - `Input.GetKey(KeyCode)`: Utilizado para detectar la pulsación continua de teclas de flecha o WASD en el modo de movimiento de teclado.
    - `Input.GetMouseButtonDown(0)`: Empleado para detectar un clic inicial con el botón izquierdo del ratón en el modo de movimiento basado en clic.

- **Eventos (Salida):**
    Este script no define ni invoca ningún evento personalizado de Unity (como `UnityEvent`) ni acciones de C# (`System.Action`) para notificar a otros sistemas o scripts sobre cambios en su estado o acciones realizadas. Su efecto principal es la modificación directa de la `transform.position` del GameObject al que está adjunto.