Aquí tienes la documentación técnica para el script `SelectType.cs`, dirigida a un nuevo miembro del equipo.

---

# `SelectType.cs`

## 1. Propósito General
Este script es responsable de gestionar la visibilidad y la interacción del panel de selección de tipo elemental en la interfaz de usuario (UI) durante una fase específica del combate. Su función principal es mostrar el panel cuando se requiere que el jugador elija un elemento y ocultarlo una vez que la selección ha sido realizada o la fase de juego cambia.

## 2. Componentes Clave

### `SelectType`
-   **Descripción:** Esta clase, que hereda de `MonoBehaviour`, actúa como un controlador para la UI de selección de elementos. Se encarga de alternar la visibilidad de los elementos de UI hijos que representan las opciones de selección y de notificar al sistema de combate principal (`Combatjudge`) la elección del jugador.

-   **Variables Clave Internas:**
    -   `prevSetMoment` (tipo `SetMoments`): Una variable privada que almacena el estado (`SetMoments`) del juego del frame anterior. Se utiliza para detectar cuándo hay un cambio en la fase del juego gestionada por `Combatjudge`, lo que activa la lógica de visibilidad de la UI.

-   **Métodos Principales:**
    -   `void Start()`: Se invoca una vez al inicio del ciclo de vida del script. Inicializa `prevSetMoment` al estado `SetMoments.PickDice` y llama inmediatamente a `Visib(false)` para asegurar que el panel de selección de elementos esté oculto al inicio del juego.
    -   `void Update()`: Este método se ejecuta en cada frame. Su tarea es monitorear el estado actual del juego (`SetMoments`) obtenido de `Combatjudge.combatjudge`. Si el estado actual es diferente del estado anterior (`prevSetMoment`), y el nuevo estado es `SetMoments.SelecCombat`, entonces llama a `Visib(true)` para hacer visible el panel de selección. Finalmente, actualiza `prevSetMoment` al estado actual.
    -   `private void Visib(bool isVisible)`: Un método auxiliar privado que controla la visibilidad de los elementos de UI. Recibe un booleano `isVisible`. Establece el estado activo de los primeros cuatro GameObjects hijos (índices 0 a 3) del GameObject que contiene este script. También habilita o deshabilita el componente `Image` adjunto al GameObject principal. Estos hijos y la imagen principal probablemente conforman los botones y el fondo del panel de selección.
    -   `public void PickElement(int element)`: Este método público está diseñado para ser invocado desde un evento de UI (por ejemplo, el `onClick` de un botón). Recibe un entero `element` que representa el elemento seleccionado por el jugador. Intenta comunicar esta selección al sistema de combate a través de `Combatjudge.combatjudge.pickElement()`, casteando el entero recibido al tipo `Element`. Si la selección es procesada exitosamente por `Combatjudge` (implica que `pickElement` devuelve `true`), este método llama a `Visib(false)` para ocultar el panel de selección, indicando que la elección ya ha sido hecha.

-   **Lógica Clave:**
    La lógica central del script reside en la detección de cambios de estado del juego. En el método `Update`, el script consulta activamente el estado del juego global (`SetMoments`) a través del sistema `Combatjudge`. Cuando el juego entra en la fase `SetMoments.SelecCombat`, el script automáticamente revela la UI de selección. Una vez que el jugador ha hecho una elección a través del método `PickElement`, y esta elección es aceptada por el `Combatjudge`, el script oculta nuevamente la UI, completando el ciclo de interacción.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`, lo que significa que no impone la necesidad de otros componentes específicos en el GameObject al que está adjunto. Sin embargo, su funcionalidad depende de que los GameObjects hijos sean los elementos de UI esperados para la selección (ej. botones) y que el GameObject padre tenga un componente `Image` si su visibilidad también necesita ser controlada.
-   **Dependencias de Clase/Enum:** Depende fuertemente de la clase estática o singleton `Combatjudge` y de las enumeraciones `SetMoments` y `Element`, las cuales deben estar definidas en el proyecto.
-   **Eventos (Entrada):**
    *   Este script se "suscribe" implícitamente a los cambios de estado del juego al consultar repetidamente `Combatjudge.combatjudge.GetSetMoments()` en su método `Update`.
    *   El método público `PickElement(int element)` está diseñado para ser invocado por eventos de UI (ej. `onClick` de botones en el Inspector de Unity), sirviendo como el punto de entrada para la selección del jugador.
-   **Eventos (Salida):**
    *   Este script no emite directamente `UnityEvent` o `Action`. Sin embargo, comunica la elección del jugador al sistema de combate invocando `Combatjudge.combatjudge.pickElement()`. Esta llamada puede considerarse una forma de "salida" o notificación a otro sistema central del juego.
    *   Controla la visibilidad de elementos de UI, que es un efecto visual "saliente" hacia la presentación del juego.

---