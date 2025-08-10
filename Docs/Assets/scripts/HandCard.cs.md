# `HandCard.cs`

## 1. Propósito General
El script `HandCard` gestiona la representación visual y la interactividad de una única carta en la mano del jugador dentro de la interfaz de usuario del juego. Su rol principal es sincronizar el estado UI de una carta (su visibilidad, color y texto) con el estado lógico del juego, particularmente durante las diferentes fases del combate, y facilitar la selección de la carta por parte del jugador.

## 2. Componentes Clave

### `HandCard`
-   **Descripción:** Esta clase, que hereda de `MonoBehaviour`, actúa como el controlador para un objeto de interfaz de usuario que representa una carta individual en la mano de un `Figther` (jugador). Implementa las interfaces `IPointerEnterHandler` y `IPointerExitHandler` para gestionar interacciones del puntero (mouse/toque), aunque sus métodos asociados están actualmente sin implementar. La directiva `[DefaultExecutionOrder(-4)]` indica que este script se ejecuta muy temprano en el ciclo de vida de los scripts de Unity, lo cual puede ser crucial para el orden de inicialización con otros sistemas.
-   **Variables Públicas / Serializadas:**
    -   `[SerializeField] int handPos`: Un entero que probablemente indica la posición de la carta dentro de la mano del jugador. Su uso directo no se observa en el código proporcionado, pero es una variable serializada.
    -   `[SerializeField] Card card`: Referencia al objeto de datos `Card` que esta representación UI está mostrando. Es el modelo de datos asociado a esta vista.
    -   `TextMeshProUGUI textMeshPro`: Componente de TextMeshPro que se usa para mostrar el identificador (`ID`) de la carta en la UI.
    -   `Figther player`: Referencia al objeto `Figther` (el jugador) al que pertenece esta carta. Este script interactúa con el `Figther` para notificarle sobre la selección de cartas.
    -   `Button button`: Referencia al componente `Button` de Unity UI, utilizado para controlar la interactividad (habilitado/deshabilitado) de la carta en la mano.
    -   `SetMoments prevSetMoment`: Almacena el estado de la fase de combate (`SetMoments`) del frame anterior para detectar cambios de fase y reaccionar apropiadamente.

-   **Métodos Principales:**
    -   `void Start()`: Este método de ciclo de vida de Unity se llama una vez al inicio. Es responsable de inicializar las referencias a los componentes hijos (`TextMeshProUGUI`) y a los componentes padre (`Figther`), así como al propio `Button`. Inicialmente, la carta se establece como no visible/interactuable (`Visib(false)`) y luego se invoca `SetCard()` con la `card` serializada para inicializar su apariencia.
    -   `void Update()`: Este método de ciclo de vida de Unity se ejecuta en cada frame. Su función principal es monitorear el cambio de fases de combate (`SetMoments`) a través de `Combatjudge.combatjudge.GetSetMoments()`. Si la fase de combate cambia a `SetMoments.PickCard` y el jugador asociado está en combate (`player.IsFigthing()`), la carta se hace visible y interactuable si su elemento (`card.GetElement()`) coincide con el tipo de combate actual (`Combatjudge.combatjudge.combatType`) o si el combate es de tipo `full`. Si la fase de combate no es `PickCard`, la carta se oculta. También se oculta si el jugador ya ha "elegido" algo (`player.getPicked() != null`), previniendo interacciones adicionales.
    -   `private void Visib(bool isVisible)`: Un método auxiliar que controla la interactividad del `Button` asociado. Si `isVisible` es `true`, el botón se hace interactuable; de lo contrario, se desactiva.
    -   `public void ForceReveal()`: Un método público que simplemente llama a `Visib(true)` para forzar que la carta se muestre y sea interactuable, independientemente de la lógica de la fase de combate.
    -   `public void OnPointerEnter(PointerEventData eventData)`: Método de la interfaz `IPointerEnterHandler`, llamado cuando el puntero del mouse entra en el área de la carta. Actualmente está vacío, lo que sugiere una funcionalidad de hover pendiente o no implementada.
    -   `public void OnPointerExit(PointerEventData eventData)`: Método de la interfaz `IPointerExitHandler`, llamado cuando el puntero del mouse sale del área de la carta. También está vacío actualmente.
    -   `public void SetCard(Card card)`: Este método actualiza la carta de datos asociada a este componente UI y refresca su representación visual. Si la `card` es `null`, limpia el texto y deshabilita la imagen. Si hay una `card`, establece el texto con su `ID` y cambia el color de la `Image` del botón según el `Element` de la carta (rojo para fuego, verde para tierra, azul para agua, blanco para aire).
    -   `public void SelectedCard()`: Este método está diseñado para ser invocado cuando el jugador selecciona esta carta (probablemente a través del evento `onClick` del `Button`). Notifica al `player` llamando a `player.PlayCard(card)` con la carta actual y luego limpia el slot de la mano invocando `SetCard(null)`.
    -   `public Card GetCard()`: Un método simple que devuelve el objeto `Card` que actualmente está asociado con esta representación UI.

-   **Lógica Clave:**
    La lógica central reside en el método `Update`, que actúa como un observador del estado global del juego (gestionado por `Combatjudge`). La visibilidad e interactividad de la carta se rigen por un sistema de máquina de estados implícito basado en `SetMoments`. Solo se permite la interacción con la carta si la fase del juego es `PickCard` y si el tipo de combate actual permite el elemento de la carta, o si es un combate `full`. Este control dinámico asegura que los jugadores solo puedan interactuar con las cartas en los momentos apropiados del juego.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que el GameObject al que está adjunto tenga componentes `Button` y `Image` (para el fondo de la carta), y que un componente `TextMeshProUGUI` esté presente como hijo para mostrar el ID de la carta. También depende de que un componente `Figther` exista en un GameObject padre y que la clase `Combatjudge` esté disponible globalmente (posiblemente como un singleton).

-   **Eventos (Entrada):**
    El script implementa las interfaces `IPointerEnterHandler` y `IPointerExitHandler`, lo que significa que Unity invoca `OnPointerEnter` y `OnPointerExit` cuando el puntero del mouse (o el toque) entra o sale del área de la UI de la carta.
    Además, aunque no se ve explícitamente en el código C#, el método público `SelectedCard()` está diseñado para ser llamado por el evento `OnClick()` del componente `Button` de Unity UI, que normalmente se configura en el Inspector de Unity.

-   **Eventos (Salida):**
    Este script no invoca sus propios `UnityEvent` o `Action` para notificar a otros sistemas. En su lugar, se comunica directamente con el sistema `Figther` (al cual pertenece) llamando a `player.PlayCard(card)` cuando la carta es seleccionada. También consulta el estado del sistema de combate a través de `Combatjudge.combatjudge.GetSetMoments()` y `Combatjudge.combatjudge.combatType`.