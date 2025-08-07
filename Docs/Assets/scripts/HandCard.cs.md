# `HandCard.cs`

## 1. Propósito General
Este script gestiona la representación visual y la interactividad de una única carta dentro de la mano del jugador en la interfaz de usuario. Su rol principal es mostrar la carta correcta, controlar su visibilidad e interacción en función de la fase de combate actual, y manejar las acciones del jugador, como seleccionar la carta. Interactúa principalmente con el sistema de combate (`Combatjudge`) y el combatiente (`Figther`) al que pertenece la carta.

## 2. Componentes Clave

### `HandCard`
- **Descripción:** `HandCard` es un `MonoBehaviour` que se adhiere a un GameObject UI, representando una carta individual en la mano de un jugador. Implementa las interfaces `IPointerEnterHandler` y `IPointerExitHandler` para manejar eventos de interacción del puntero. Controla la apariencia de la carta (texto, imagen, color) y su capacidad de ser seleccionada, adaptándose a las diferentes etapas del combate.
- **Variables Públicas / Serializadas:**
    - `[SerializeField] int handPos`: Un entero que, aunque no se usa explícitamente en el código proporcionado, típicamente indica la posición de la carta dentro de la mano del jugador.
    - `[SerializeField] Card card`: Una referencia al objeto de datos `Card` que este elemento de UI representa. Este objeto `Card` contiene la lógica y las propiedades del juego para la carta.
    - `TextMeshProUGUI textMeshPro`: Una referencia al componente `TextMeshProUGUI` hijo del GameObject, utilizado para mostrar texto en la carta (ej. el ID de la carta). Se inicializa en `Start` buscando el componente.
    - `Figther player`: Una referencia al componente `Figther` (el combatiente) que posee esta carta de mano. Se obtiene de un GameObject padre en `Start`.
    - `Button button`: Una referencia al componente `Button` adjunto al mismo GameObject. Se utiliza para gestionar las interacciones de clic y controlar si la carta es interactuable. Se inicializa en `Start`.
    - `SetMoments prevSetMoment`: Almacena el estado de `SetMoments` de la iteración anterior del `Update` para detectar cuándo cambia la fase de combate.
- **Métodos Principales:**
    - `void Start()`:
        Este método se invoca una vez al inicio del ciclo de vida del script. Su función es inicializar las referencias a los componentes necesarios (`TextMeshProUGUI`, `Figther`, `Button`) y establecer el estado inicial de la carta. Al inicio, la carta se vuelve no visible (`Visib(false)`) y se configura con el objeto `card` inicial (`SetCard(card)`).
    - `void Update()`:
        Se ejecuta en cada fotograma. Su tarea principal es monitorear los cambios en la fase de combate (`SetMoments`) a través de `Combatjudge.combatjudge.GetSetMoments()`.
        - **Lógica Clave:** Si la fase de combate cambia, el script reacciona:
            - Si la nueva fase es `SetMoments.PickCard` y el jugador (`player`) está activamente combatiendo (`IsFigthing()`), la carta se hace visible e interactuable (`Visib(true)`). También incrementa el contador de cartas disponibles del jugador (`player.avalaibleCard++`). La visibilidad en esta fase también considera el `combatType` actual del `Combatjudge` y el elemento de la carta, permitiendo que la carta sea jugable si el tipo de combate es "full" o coincide con su elemento.
            - Si la fase de combate no es `SetMoments.PickCard`, la carta se oculta y se vuelve no interactuable (`Visib(false)`).
        - Además, si el jugador ya ha "elegido" una carta (`player.getPicked() != null`), esta carta se desactiva también, para asegurar que solo una carta sea seleccionada por turno o que todas se oculten después de una selección.
    - `private void Visib(bool isVisible)`:
        Controla la capacidad de interacción del `Button` de la carta. Si `isVisible` es `true`, el botón se hace interactuable; de lo contrario, se desactiva.
    - `public void ForceReveal()`:
        Fuerza la visibilidad e interactividad de la carta, independientemente de la fase de combate actual. Puede ser útil para escenarios específicos donde la carta debe ser revelada de forma excepcional.
    - `public void OnPointerEnter(PointerEventData eventData)`:
        Método de la interfaz `IPointerEnterHandler` que se activa cuando el puntero del ratón entra en el área del UI de la carta. Actualmente, está vacío, pero es un punto de extensión para futuros efectos visuales al pasar el ratón.
    - `public void OnPointerExit(PointerEventData eventData)`:
        Método de la interfaz `IPointerExitHandler` que se activa cuando el puntero del ratón sale del área del UI de la carta. También está vacío actualmente, para futuros efectos visuales.
    - `public void SetCard(Card card)`:
        Asigna un objeto `Card` a esta instancia de `HandCard` y actualiza su representación visual en la UI.
        - **Parámetros:** `card` (Tipo: `Card`) - El objeto de datos de la carta a mostrar.
        - **Lógica Clave:** Si el `card` proporcionado es `null`, el texto de la carta se vacía y el componente `Image` se desactiva. Si es una carta válida, el texto se actualiza con el ID de la carta, la `Image` se habilita, y su color se ajusta según el elemento de la carta (`fire` a rojo, `earth` a verde, `water` a azul, `air` a blanco).
    - `public void SelectedCard()`:
        Este método está diseñado para ser invocado cuando la carta es seleccionada por el jugador (normalmente a través del evento `onClick` del `Button` en el Inspector de Unity). Notifica al `player` que se ha jugado la `card` actual (`player.PlayCard(card)`) y luego "vacía" este espacio de la mano llamando a `SetCard(null)`.
    - `public Card GetCard()`:
        Devuelve el objeto `Card` que actualmente está asociado con esta instancia de `HandCard`.
        - **Retorna:** El objeto `Card` representado por este UI.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Aunque no se utilizan atributos `[RequireComponent]`, este script depende implícitamente de la existencia de ciertos componentes en su GameObject o en sus hijos/padres:
    - Un componente `TextMeshProUGUI` como hijo (para mostrar el texto de la carta).
    - Un componente `Button` en el mismo GameObject (para la interacción de clic).
    - Un componente `Image` en el mismo GameObject (para la representación visual y el cambio de color de la carta).
    - Un componente `Figther` en uno de sus GameObjects padres (para interactuar con el jugador que posee la carta).
    - El script también interactúa con una instancia estática de `Combatjudge` (`Combatjudge.combatjudge`).
- **Eventos (Entrada):**
    - **Eventos de Puntero UI**: Implementa `IPointerEnterHandler` y `IPointerExitHandler` para responder cuando el puntero del ratón entra o sale del área de la carta.
    - **Clic del Botón**: El método `SelectedCard()` está destinado a ser conectado al evento `onClick` del `Button` de la carta en el editor de Unity, permitiendo que la carta responda a los clics del usuario.
    - **Polling de Estado de Juego**: En `Update`, el script consulta activamente (`polls`) el estado de `SetMoments` del `Combatjudge` para adaptar su comportamiento a la fase de combate actual.
- **Eventos (Salida):**
    Este script no emite directamente `UnityEvent`s o `Action`s. En su lugar, interactúa directamente con el componente `Figther` al que pertenece, llamando a métodos como `player.PlayCard(card)` y modificando propiedades como `player.avalaibleCard++`. Estas llamadas notifican al sistema del jugador sobre la interacción con la carta.