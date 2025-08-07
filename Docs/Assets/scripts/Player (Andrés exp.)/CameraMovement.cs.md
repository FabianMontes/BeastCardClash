# `CameraMovement.cs`

## 1. Propósito General
Este script gestiona el movimiento de la cámara en el juego, asegurando que siga la posición de un objeto específico, típicamente el personaje del jugador. Interactúa directamente con los componentes `Transform` de la cámara y del objetivo para actualizar su posición en cada fotograma del juego.

## 2. Componentes Clave

### `CameraMovement`
-   **Descripción:** `CameraMovement` es una clase que extiende `MonoBehaviour`, lo que le permite ser adjuntada a un GameObject en la escena de Unity (en este caso, la cámara del juego). Su propósito principal es implementar un comportamiento de cámara "sígueme" (follow camera), donde la cámara se posiciona constantemente en la ubicación de un objetivo predefinido.
-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] Transform player;`: Esta variable de tipo `Transform` representa el objetivo que la cámara debe seguir. El uso de `[SerializeField]` permite asignar cualquier componente `Transform` (de cualquier GameObject) a esta variable directamente desde el Inspector de Unity, lo cual es esencial para configurar qué GameObject será seguido por la cámara.
-   **Métodos Principales:**
    -   `void Start()`: Este es un método del ciclo de vida de Unity que se invoca una vez al inicio, justo antes del primer `Update`. En la implementación actual, este método está vacío, lo que indica que no se requiere ninguna inicialización especial para el movimiento de la cámara al comienzo del juego.
    -   `void Update()`: Este es un método del ciclo de vida de Unity que se llama una vez por cada fotograma del juego. Constituye el núcleo de la lógica de seguimiento de la cámara. En cada ciclo, actualiza la posición del GameObject al que este script está adjunto (la cámara) para que coincida exactamente con la posición del `Transform` asignado a la variable `player`.
-   **Lógica Clave:**
    La lógica fundamental del script reside en el método `Update`. En cada ciclo de renderizado del juego, la línea `transform.position = player.position;` se ejecuta. Esto significa que la posición (`position`) del `Transform` de la cámara (representado por `transform`) se establece igual a la posición del `Transform` del `player`. Este proceso continuo asegura que la cámara siempre se mantenga centrada o alineada con la posición del `player` en el mundo del juego, creando un seguimiento suave y en tiempo real.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, para su correcta ejecución, el GameObject al que se adjunte este script (la cámara) y el GameObject asignado a la variable `player` deben poseer un componente `Transform`, que es inherente a todos los GameObjects en Unity.
-   **Eventos (Entrada):** Este script no se suscribe a eventos personalizados del juego ni a eventos de interfaz de usuario. Su ejecución se basa exclusivamente en los métodos de ciclo de vida de Unity (`Start` y `Update`), que son invocados automáticamente por el motor.
-   **Eventos (Salida):** `CameraMovement.cs` no invoca ni publica ningún tipo de evento (`UnityEvent`, `Action`, etc.) para notificar a otros sistemas sobre su estado o acciones. Su función es autónoma y se limita al control de posición de la cámara.