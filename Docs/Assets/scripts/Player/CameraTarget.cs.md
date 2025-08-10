# `CameraTarget.cs`

## 1. Propósito General
Este script gestiona la posición de un `GameObject` auxiliar en la escena, que actúa como el objetivo o punto de referencia para la cámara principal del juego. Su rol principal es asegurar que este objetivo de la cámara siga de manera precisa y continua al personaje del jugador, manteniendo la perspectiva de la cámara centrada en el jugador sin replicar su rotación.

## 2. Componentes Clave

### `CameraTarget`
- **Descripción:** `CameraTarget` es una clase que hereda de `MonoBehaviour`, lo que le permite ser adjuntada a un `GameObject` en la jerarquía de Unity. Su propósito fundamental es actualizar la posición de su propio `GameObject` para que coincida con la posición de un `Transform` de seguimiento, que en este contexto es el `Player`.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] Transform Player;`
        Esta variable es un `Transform` y es serializada, lo que significa que puede ser asignada y configurada directamente desde el Inspector de Unity. Representa el objeto que el `CameraTarget` debe seguir. Para el correcto funcionamiento de este script, es crucial que se asigne el `Transform` del personaje jugador en esta ranura.
- **Métodos Principales:**
    - `void Start()`:
        Este es un método del ciclo de vida de `MonoBehaviour` que se invoca una vez al inicio del juego, antes de que se ejecute el primer `Update`. En la implementación actual, este método está vacío, indicando que no se requiere ninguna inicialización específica para este componente al arrancar.
    - `void Update()`:
        Este método es invocado una vez por cada fotograma del juego. Su función es actualizar constantemente la posición del `GameObject` al que está adjunto el script `CameraTarget`. Dentro de este método se ejecuta la lógica principal para el seguimiento.
- **Lógica Clave:**
    La lógica central del script reside en el método `Update`. En cada fotograma, la posición del `GameObject` que contiene este `CameraTarget` se actualiza para ser idéntica a la posición del `Player` asignado.
    ```csharp
    transform.position = Player.position;
    ```
    Esta simple línea de código asegura que el objetivo de la cámara siempre esté directamente encima o en la misma posición que el jugador. La ventaja de usar un `GameObject` separado como objetivo de la cámara, y que no rota (como se menciona en el comentario del código), es que permite a la cámara tener su propia lógica de rotación y movimiento, evitando que se vea afectada por las rotaciones del personaje jugador, lo que generalmente resulta en una experiencia de cámara más suave y predecible.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, por lo que no impone la presencia de ningún otro componente específico en el `GameObject` al que se adjunta. Sin embargo, su funcionalidad depende fundamentalmente de que una referencia válida al `Transform` del `Player` sea asignada en el Inspector de Unity.
- **Eventos (Entrada):** El script `CameraTarget.cs` no se suscribe a eventos de entrada del usuario (como clics de ratón o pulsaciones de teclado) ni a eventos externos de otros sistemas. Su operación es autónoma y se basa en el ciclo de vida de Unity (`Update`).
- **Eventos (Salida):** Este script no invoca ni emite ningún evento (como `UnityEvent` o `Action`) para notificar a otros sistemas o componentes del juego. Su función es exclusivamente actualizar su propia posición basándose en la posición del `Player`.