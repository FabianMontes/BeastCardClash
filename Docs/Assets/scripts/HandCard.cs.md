# `HandCard.cs`

## 1. Propósito General
Este script gestiona la representación visual y la interactividad de una carta individual en la mano del jugador dentro de la interfaz de usuario del juego. Su rol principal es sincronizar el estado de la carta con las fases del combate y las acciones del jugador, permitiendo la selección y visualización de la carta.

## 2. Componentes Clave

### `HandCard`
- **Descripción:** La clase `HandCard` es un `MonoBehaviour` que controla el comportamiento y la apariencia de un espacio de carta en la mano del jugador. Implementa las interfaces `IPointerEnterHandler` y `IPointerExitHandler` para posibles interacciones de puntero (aunque los métodos están vacíos actualmente). Su orden de ejecución (`[DefaultExecutionOrder(-4)]`) indica que se inicializa muy temprano en la jerarquía de scripts.
- **Variables Públicas / Serializadas:**
    - `handPos` (`[SerializeField] int`): Un entero que probablemente indica la posición de esta carta dentro de la mano del jugador.
    - `card` (`[SerializeField] Card`): Una referencia al objeto de datos `Card` real que esta ranura de mano está representando actualmente. Es la información subyacente de la carta.
    - `textMeshPro` (`TextMeshProUGUI`): Componente de texto para mostrar información de la carta, como su ID. Se obtiene en `Start` de los hijos del GameObject.
    - `player` (`Figther`): Una referencia al script `Figther` del jugador que posee esta mano. Se espera que el `Figther` sea un componente padre del `GameObject` al que está adjunto este script.
    - `button` (`Button`): El componente `Button` asociado a esta carta, el cual controla su interactividad (clicable o no).
    - `prevSetMoment` (`SetMoments`): Almacena la fase de combate anterior para detectar cambios en el estado del juego y activar la lógica correspondiente.
- **Métodos Principales:**
    - `void Start()`:
        Este método se llama una vez al inicio del ciclo de vida del script. Su función principal es inicializar las referencias a los componentes necesarios (`TextMeshProUGUI`, `Figther`, `Button`) y establecer el estado inicial de la carta. Oculta la carta (`Visib(false)`) y luego intenta asignar una carta inicial (`SetCard(card)`).
    - `void Update()`:
        Se llama una vez por frame. Contiene la lógica principal para reaccionar a los cambios en las fases del combate. Monitorea `Combatjudge.combatjudge.GetSetMoments()` para determinar la fase actual del juego. Si la fase cambia a `SetMoments.PickCard` y el jugador está en combate, la carta se hace visible y seleccionable *solo si* el tipo de combate (`CombatType`) es `full` o coincide con el elemento de la carta. Si la fase cambia de `PickCard`, la carta se oculta. También oculta la carta si el jugador ya ha seleccionado una.
    - `private void Visib(bool isVisible)`:
        Un método auxiliar para controlar la visibilidad e interactividad de la carta en la UI. Actualmente, solo afecta la propiedad `interactable` del componente `Button`, lo que hace que la carta sea clicable o no.
    - `public void ForceReveal()`:
        Fuerza la visibilidad e interactividad de la carta llamando a `Visib(true)`. Podría ser utilizado para forzar la visualización en escenarios específicos del juego.
    - `public void OnPointerEnter(PointerEventData eventData)`:
        Parte de la interfaz `IPointerEnterHandler`. Se llama cuando el puntero del ratón entra en el área de la carta. Actualmente, no contiene ninguna lógica implementada, pero podría usarse para efectos de "hover".
    - `public void OnPointerExit(PointerEventData eventData)`:
        Parte de la interfaz `IPointerExitHandler`. Se llama cuando el puntero del ratón sale del área de la carta. Similar a `OnPointerEnter`, actualmente no contiene lógica.
    - `public void SetCard(Card card)`:
        Este método es crucial para asignar un objeto de datos `Card` a esta ranura de mano y actualizar su representación visual. Si la `card` proporcionada es `null`, limpia el texto y deshabilita el componente `Image`. Si se proporciona una `card` válida, actualiza el texto con la ID de la carta, habilita la imagen y cambia su color basándose en el `Element` de la carta (fuego, tierra, agua, aire).
    - `public void SelectedCard()`:
        Este método es invocado cuando el jugador "selecciona" o "juega" esta carta (probablemente a través del evento `onClick` del `Button`). Llama al método `PlayCard` en el objeto `Figther` del jugador con la carta actual y luego vacía esta ranura de la mano llamando a `SetCard(null)`.
    - `public Card GetCard()`:
        Un método simple para recuperar el objeto `Card` que está siendo representado actualmente por esta ranura de mano.
- **Lógica Clave:**
    La lógica central de `HandCard` reside en su capacidad para reaccionar a los cambios de fase del combate (`SetMoments`) gestionados por `Combatjudge`. Durante la fase `PickCard`, habilita selectivamente las cartas en la mano del jugador según el `CombatType` y el elemento de la carta. Una vez que una carta es jugada mediante `SelectedCard`, la ranura de la mano se reinicia visualmente.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    - Se espera que el GameObject que contiene este script tenga un componente `Button` (referencia obtenida en `Start`).
    - Se espera que tenga un componente `TextMeshProUGUI` en uno de sus hijos (referencia obtenida en `Start`).
    - Se espera que tenga un componente `Image` (referencia obtenida en `SetCard`).
    - También depende de la existencia de un componente `Figther` en un GameObject padre.
- **Eventos (Entrada):**
    - `HandCard` se suscribe implícitamente a los eventos del ciclo de vida de Unity (`Start`, `Update`).
    - Implementa las interfaces `IPointerEnterHandler` y `IPointerExitHandler`, lo que significa que el sistema de eventos de Unity invocará `OnPointerEnter` y `OnPointerExit` cuando el puntero interactúe con el GameObject.
    - El método `SelectedCard()` probablemente está conectado al evento `onClick` del componente `Button` adjunto en el Inspector de Unity.
- **Eventos (Salida):**
    - Este script no invoca explícitamente `UnityEvent`s o `Action`s.
    - Sin embargo, sí llama a métodos en otras clases (`player.PlayCard(card)`, `Combatjudge.combatjudge.GetSetMoments()`) lo que constituye una forma de interacción directa con otros sistemas.