# `CameraMovement.cs`

## 1. Propósito General
Este script tiene como propósito principal gestionar el posicionamiento de la cámara en el juego. Su rol es asegurar que la cámara siga continuamente a un objeto específico, denominado `player`, replicando su posición en tiempo real para mantenerlo siempre centrado en la vista.

## 2. Componentes Clave

### `CameraMovement`
-   **Descripción:** `CameraMovement` es una clase que hereda de `MonoBehaviour`, lo que significa que puede ser adjuntada a cualquier GameObject en una escena de Unity, típicamente la cámara principal. Su función es implementar un seguimiento de cámara simple, donde la posición de la cámara se actualiza para coincidir con la de un objeto objetivo.

-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] Transform player;`: Esta variable es de tipo `Transform` y está marcada con `[SerializeField]`. Esto la hace editable directamente desde el Inspector de Unity, permitiendo a los diseñadores asignar el GameObject (que representa al "jugador" o cualquier otro objeto que la cámara deba seguir) cuyo `Transform` se utilizará como referencia para la posición de la cámara.

-   **Métodos Principales:**
    -   `void Start()`: Este es un método del ciclo de vida de Unity que se ejecuta una única vez al inicio del juego, antes de la primera actualización de `Update`. En la implementación actual, este método está vacío, indicando que no hay una lógica de inicialización específica que deba ejecutarse al inicio para el sistema de movimiento de la cámara.
    -   `void Update()`: Este es el método central del script, y también forma parte del ciclo de vida de Unity. Se invoca una vez por cada fotograma del juego. Dentro de este método, se encuentra la lógica de seguimiento principal: `transform.position = player.position;`. Esto establece la posición del GameObject al que está adjunto este script (la cámara) para que sea idéntica a la posición del `Transform` referenciado en la variable `player`.

-   **Lógica Clave:** La lógica principal del script se basa en la actualización constante de la posición de la cámara. Cada fotograma, la posición global del objeto al que `CameraMovement` está adjunto se iguala directamente a la posición global del `player` asignado. Esto genera un efecto de "cámara de seguimiento" simple y directo, donde la cámara siempre estará en la misma ubicación que el `player`, sin desplazamientos ni suavizados adicionales.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no fuerza la presencia de otros componentes en el GameObject al que se adjunta. Para su funcionamiento, solo requiere que se le asigne un `Transform` válido a la variable `player` en el Inspector de Unity.
-   **Eventos (Entrada):** Este script no se suscribe a eventos externos ni a entradas de usuario (como clics de botón o pulsaciones de teclado). Su operación se rige únicamente por el ciclo de vida de Unity (`Update`) y la lectura directa de la posición del `player`.
-   **Eventos (Salida):** Este script no invoca ni publica ningún tipo de evento (`UnityEvent`, `Action` o eventos personalizados) para notificar a otros sistemas sobre cambios o acciones realizadas. Su función es puramente de seguimiento posicional.