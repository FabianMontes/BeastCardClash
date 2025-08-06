# `Cube.cs`

## 1. Propósito General
Este script gestiona el movimiento de un objeto (`GameObject`) en la escena de Unity, ofreciendo dos modos de control distintos: un desplazamiento continuo basado en la entrada del teclado (flechas o WASD) o una teletransportación instantánea al punto donde el usuario haga clic con el mouse en el entorno. Interactúa principalmente con los sistemas de entrada de Unity y la transformación del propio objeto.

## 2. Componentes Clave

### `Cube`
- **Descripción:** Esta clase, que hereda de `MonoBehaviour`, es responsable de la lógica de movimiento y posicionamiento del `GameObject` al que esté adjunta. Permite al jugador elegir entre dos esquemas de control para mover el objeto en el mundo 3D.
- **Variables Públicas / Serializadas:**
    - `playerCamera` (`Transform`): Referencia a la cámara principal del jugador. Se utiliza para calcular las direcciones de movimiento relativas a la orientación de la cámara, asegurando que "adelante" para el cubo sea "adelante" desde la perspectiva del jugador.
    - `speed` (`float`): Determina la velocidad de movimiento del cubo cuando el modo de control es por teclado. Un valor mayor resultará en un movimiento más rápido.
    - `mode` (`bool`): Un interruptor booleano que define el esquema de control activo.
        - Si es `true`, el cubo se mueve usando las teclas de flecha o WASD.
        - Si es `false`, el cubo se teletransporta a la posición del mouse al hacer clic.

- **Métodos Principales:**
    - `void Start()`: Este método se llama una vez al inicio del ciclo de vida del script. En la implementación actual, no contiene ninguna lógica de inicialización.
    - `void Update()`: Se invoca una vez por cada fotograma. Es el corazón de la lógica de movimiento del cubo.
        - Si `mode` es `true`, calcula la dirección de entrada del teclado y actualiza la posición del cubo continuamente utilizando `transform.position += GetInputDirection() * speed * Time.deltaTime;`.
        - Si `mode` es `false`, detecta clics del botón izquierdo del mouse (`Input.GetMouseButtonDown(0)`). Al detectar un clic, lanza un rayo desde la cámara hacia el punto del mouse. Si el rayo impacta con cualquier colisionador en la escena (`Physics.Raycast`), la posición del cubo se establece directamente en el punto de impacto.

    - `Vector3 GetInputDirection()`: Este método auxiliar es llamado cuando `mode` es `true`. Su propósito es traducir las pulsaciones de teclas del jugador (flechas o WASD) en una dirección de movimiento `Vector3` relativa a la cámara del jugador. Combina las entradas para permitir movimiento diagonal y normaliza el vector resultante para mantener una velocidad constante, independientemente de si se presionan una o más teclas. Además, establece el componente `y` del vector de dirección a cero (`dir.y = 0`) para asegurar que el movimiento sea estrictamente horizontal y no afecte la altura del cubo.

- **Lógica Clave:**
    La lógica principal reside en el método `Update`, que actúa como un simple selector entre dos modos de movimiento. El modo de "navegación" continua (`mode = true`) se basa en la acumulación de un vector de dirección (`GetInputDirection`) multiplicado por la velocidad y el tiempo transcurrido, resultando en un desplazamiento suave. El modo de "teletransporte" (`mode = false`) utiliza una técnica de `Raycasting` para detectar el punto en el espacio 3D que el mouse está "apuntando" y reubica el objeto instantáneamente a esa posición.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no requiere explícitamente que ningún otro componente esté presente en el mismo `GameObject`.
- **Eventos (Entrada):** Este script se suscribe a los eventos de entrada del usuario de Unity:
    - Lectura de pulsaciones de teclas (`Input.GetKey` para `KeyCode.UpArrow`, `KeyCode.DownArrow`, `KeyCode.LeftArrow`, `KeyCode.RightArrow`, `KeyCode.W`, `KeyCode.A`, `KeyCode.S`, `KeyCode.D`).
    - Detección de clics del botón izquierdo del mouse (`Input.GetMouseButtonDown(0)`).
    - Obtención de la posición actual del mouse en la pantalla (`Input.mousePosition`).
- **Eventos (Salida):** El script `Cube.cs` no invoca ni emite ningún evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas. Su efecto se limita a modificar la propia transformación del `GameObject` al que está adjunto.