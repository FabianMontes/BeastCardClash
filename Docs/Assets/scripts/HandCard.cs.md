# `HandCard.cs`

## 1. Propósito General
Este script gestiona la representación visual y la interactividad de una carta individual en la mano de un jugador dentro de la interfaz de usuario. Su rol principal es sincronizar el estado de la carta (visible, jugable, información) con el flujo de juego y las acciones del jugador. Interactúa estrechamente con el sistema `Combatjudge` para determinar cuándo las cartas pueden ser seleccionadas y con el script `Figther` del jugador para ejecutar las acciones de la carta.

## 2. Componentes Clave

### `HandCard`
- **Descripción:** La clase `HandCard` es un `MonoBehaviour` que se adjunta a un objeto de UI (probablemente un `Button` o `Image`) que representa una carta en la mano del jugador. Es responsable de mostrar la información de la carta asociada, controlar su visibilidad e interactividad, y manejar eventos de puntero (ratón) y clics. Implementa las interfaces `IPointerEnterHandler` e `IPointerExitHandler` para manejar eventos de "hover". La anotación `[DefaultExecutionOrder(-4)]` asegura que este script se ejecute muy temprano en el ciclo de vida de los scripts de Unity, incluso antes que la mayoría de los demás.

- **Variables Públicas / Serializadas:**
    - `handPos` (`int`): Una variable serializada (`[SerializeField]`) que probablemente indica la posición de esta carta dentro de la mano del jugador. Aunque no se utiliza explícitamente en la lógica de posicionamiento en este script, es un dato configurable desde el Inspector de Unity.
    - `card` (`Card`): La referencia al objeto `Card` real que esta representación de UI está mostrando. Es serializada (`[SerializeField]`) para permitir asignar la carta inicial desde el Inspector, aunque su valor se actualiza dinámicamente durante el juego.

- **Métodos Principales:**
    - `void Start()`: Este método se invoca una vez al inicio del ciclo de vida del script.
        - Obtiene referencias a componentes clave en el mismo GameObject o en sus padres:
            - `textMeshPro`: Un componente `TextMeshProUGUI` encontrado en los hijos, utilizado para mostrar el ID de la carta.
            - `player`: Un componente `Figther` encontrado en los padres, representando al jugador al que pertenece esta mano.
            - `button`: Un componente `Button` en el propio GameObject, usado para controlar la interactividad del botón.
        - Inicializa `prevSetMoment` a `SetMoments.PickDice` para la lógica de detección de cambio de fase.
        - Llama a `Visib(false)` para asegurar que la carta no sea interactuable al inicio.
        - Llama a `SetCard(card)` para configurar la visualización de la carta inicial (si `card` tiene un valor asignado).
    - `void Update()`: Se llama una vez por frame. Este método es crucial para la lógica de interactividad de la carta:
        - Constantemente obtiene el momento actual del juego (`SetMoments`) desde el `Combatjudge` (un singleton global, `Combatjudge.combatjudge`).
        - Si el `SetMoments` actual es diferente del `prevSetMoment` (detecta un cambio de fase):
            - Si el momento es `SetMoments.PickCard` y el jugador (`player`) está en combate (`IsFigthing()`), y además el tipo de combate (`Combatjudge.combatjudge.combatType`) coincide con el elemento de la carta (`card.GetElement()`) o es `CombatType.full`, entonces la carta se vuelve visible e interactuable (`Visib(true)`), y se incrementa el contador de cartas disponibles del jugador (`player.avalaibleCard++`). Esto permite que la carta se pueda seleccionar en la fase de elección de carta.
            - Si el momento *no* es `SetMoments.PickCard`, la carta se oculta e inhabilita (`Visib(false)`).
        - Finalmente, actualiza `prevSetMoment` al momento actual.
        - Adicionalmente, si el jugador ya ha "elegido" algo (`player.getPicked() != null`), la carta también se vuelve no interactuable.
    - `private void Visib(bool isVisible)`: Controla la capacidad de interactuar con la carta.
        - Establece la propiedad `interactable` del componente `Button` a `isVisible`.
        - Un fragmento de código comentado sugiere que previamente pudo haber controlado la visibilidad de un hijo del GameObject, pero ahora solo afecta la interactividad del botón.
    - `public void ForceReveal()`: Un método público simple que llama a `Visib(true)`, forzando que la carta se vuelva interactuable.
    - `public void OnPointerEnter(PointerEventData eventData)`: Método de la interfaz `IPointerEnterHandler`. Actualmente vacío, pero destinado a implementar lógica cuando el puntero (ratón) entra en el área de la carta (por ejemplo, para mostrar un tooltip de la carta).
    - `public void OnPointerExit(PointerEventData eventData)`: Método de la interfaz `IPointerExitHandler`. Actualmente vacío, pero destinado a implementar lógica cuando el puntero (ratón) sale del área de la carta.
    - `public void SetCard(Card card)`: Este método se utiliza para asignar y actualizar la información visual de la carta que este `HandCard` representa.
        - Si la `card` pasada es `null`, vacía el texto, y desactiva el componente `Image` (haciendo que la "ranura" de la carta esté vacía).
        - Si la `card` no es `null`, muestra el `ID` de la carta en el `textMeshPro`, activa el componente `Image` y cambia su color según el elemento de la carta (`fire`=rojo, `earth`=verde, `water`=azul, `air`=blanco).
    - `public void SelectedCard()`: Este método está diseñado para ser invocado cuando el `Button` de la carta es clicado.
        - Llama al método `PlayCard()` del `player` al que pertenece, pasándole la `card` actual. Esto le indica al jugador que ha intentado jugar esa carta.
        - Luego, llama a `SetCard(null)` para "vaciar" la ranura de la mano, indicando que la carta ha sido jugada.
    - `public Card GetCard()`: Un método simple para obtener la referencia al objeto `Card` que este `HandCard` está actualmente representando.

- **Lógica Clave:**
    La lógica principal reside en el método `Update()`, que actúa como un observador del estado global del juego (gestionado por `Combatjudge`). El script ajusta la visibilidad y capacidad de interacción de la carta basándose en el "momento" actual del combate (`SetMoments`). Esto asegura que las cartas solo sean seleccionables cuando el juego se encuentra en la fase de "PickCard" y bajo las condiciones de combate adecuadas (tipo de combate coincidente). Una vez que el jugador selecciona una carta, esta ranura se vacía visualmente.

## 3. Dependencias y Eventos
- **Componentes Requeridos:** Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, funcionalmente requiere que el GameObject al que está adjunto contenga un componente `Button` y un componente `Image`. También espera que exista un `TextMeshProUGUI` en alguno de sus hijos.

- **Eventos (Entrada):**
    - **Interacción de Usuario:** La clase implementa `IPointerEnterHandler` y `IPointerExitHandler` para responder a eventos de mouse hover (aunque sus métodos están vacíos actualmente). La funcionalidad `SelectedCard()` se asume que es el callback para el evento `onClick` del `Button` adjunto al GameObject.
    - **Estado del Juego:** Se suscribe implícitamente a los cambios en el `SetMoments` del `Combatjudge` a través de la consulta constante en el método `Update()`.

- **Eventos (Salida):**
    - Este script invoca métodos directamente sobre el componente `Figther` (el jugador) al que pertenece, específicamente `player.PlayCard(card)` cuando la carta es seleccionada y modifica `player.avalaibleCard` cuando la carta se vuelve seleccionable. No emite `UnityEvent`s o `Action`s para otros sistemas.