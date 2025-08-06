# `HandCard.cs`

## 1. Propósito General
Este script `HandCard` gestiona la representación individual de una carta dentro de la mano de un jugador en la interfaz de usuario. Su rol principal es controlar la visibilidad e interactividad de la carta basándose en las fases del combate y permitir que los jugadores la seleccionen para jugar.

## 2. Componentes Clave

### HandCard
La clase `HandCard` es un `MonoBehaviour` que se adjunta a un elemento de interfaz de usuario (UI) en Unity, representando una carta específica en la mano del jugador. Se encarga de actualizar visualmente la carta, responder a la interacción del usuario y comunicarse con otros sistemas de juego clave para procesar la selección de la carta.

#### Variables Públicas / Serializadas
La clase `HandCard` utiliza varias variables para mantener su estado y referencias a otros componentes. `handPos` es una variable serializada (`[SerializeField]`) que, aunque no se usa explícitamente en el código proporcionado, típicamente representaría la posición de la carta dentro de la mano del jugador. La variable `card` (también serializada) es el objeto `Card` que este componente `HandCard` representa, conteniendo toda la información lógica de la carta como su ID y elemento. Internamente, el script obtiene referencias a un `TextMeshProUGUI` para mostrar texto en la carta (como su ID), al componente `Player` (el jugador propietario de esta carta, obtenido del padre del GameObject) y a un `Button` para controlar la interactividad. Además, `prevSetMoment` rastrea la fase de combate anterior para detectar cambios.

#### Métodos Principales
La funcionalidad de `HandCard` se articula a través de varios métodos clave, incluyendo métodos del ciclo de vida de Unity y funciones personalizadas.

El método `Start()` se ejecuta una vez al inicio del script. En este método, `HandCard` inicializa sus referencias a los componentes `TextMeshProUGUI`, `Player` y `Button` que necesita para operar. También establece un estado inicial de `prevSetMoment` y llama a `Visib(false)` para asegurar que la carta esté inicialmente no interactuable, antes de asignar y mostrar los datos de la carta con `SetCard(card)`.

El método `Update()` se invoca cada fotograma y es fundamental para la lógica de interactividad de la carta. Monitorea continuamente la fase actual del combate (`SetMoments`) obtenida del sistema `Combatjudge`. Si la fase de combate ha cambiado, el script evalúa si la carta debe ser visible y seleccionable. Específicamente, durante la fase `SetMoments.PickCard`, la carta se vuelve interactuable (`Visib(true)`) si el jugador está en combate y si el tipo de combate actual (`Combatjudge.combatjudge.combatType`) coincide con el elemento de la carta o es un tipo de combate "completo". En cualquier otra fase que no sea `PickCard`, la carta se oculta (`Visib(false)`). Finalmente, la carta también se oculta si el jugador ya ha seleccionado una carta.

El método `private void Visib(bool isVisible)` es una función auxiliar que controla la interactividad del botón de la carta. Al pasar `true` o `false`, se habilita o deshabilita la interacción con la carta.

`public void ForceReveal()` es un método público que permite forzar la visibilidad e interactividad de la carta, independientemente de la lógica de fase de combate del `Update`.

Los métodos `public void OnPointerEnter(PointerEventData eventData)` y `public void OnPointerExit(PointerEventData eventData)` implementan las interfaces `IPointerEnterHandler` e `IPointerExitHandler` respectivamente. Estos métodos se invocan cuando el cursor del ratón entra o sale del área de la carta. Actualmente, están vacíos, sirviendo como marcadores de posición para futuras funcionalidades visuales, como el resaltado o el zoom de la carta al pasar el ratón.

El método `public void SetCard(Card card)` es crucial para actualizar los datos y la apariencia visual de la carta. Cuando se le proporciona un objeto `Card`, este método actualiza el texto de la carta con su ID y establece el color de la imagen de fondo de la carta según su elemento (Rojo para Fuego, Verde para Tierra, Azul para Agua, Blanco para Aire). Si se le pasa `null`, el método vacía el texto y deshabilita la imagen, haciendo que el espacio de la carta parezca vacío.

Finalmente, `public void SelectedCard()` es el método que se invoca cuando el jugador hace clic en la carta (a través del componente `Button`). Este método notifica al `Player` propietario que se ha seleccionado esta `card`, y luego establece la carta en `null` con `SetCard(null)`, eliminándola visual y lógicamente de la mano. `public Card GetCard()` es un simple método getter para recuperar la instancia del objeto `Card` que esta `HandCard` está representando actualmente.

#### Lógica Clave
La lógica central de `HandCard` reside en su capacidad para reaccionar dinámicamente a la fase de combate actual del juego. Utiliza el singleton `Combatjudge` para obtener el estado del juego y decide si la carta debe ser seleccionable. Durante la fase `PickCard`, la visibilidad e interactividad de la carta se ajustan no solo por el estado general del juego, sino también por una condición de elegibilidad basada en el tipo de combate y el elemento de la carta. Esta mecánica garantiza que los jugadores solo puedan interactuar con las cartas apropiadas en el momento adecuado. Además, la actualización visual de la carta (su texto y color) está completamente desacoplada y se controla a través del método `SetCard`, permitiendo una fácil asignación y vaciado de las cartas.

## 3. Dependencias y Eventos

*   **Componentes Requeridos:**
    Este script no usa explícitamente el atributo `[RequireComponent]`, pero su funcionalidad depende directamente de la presencia de varios componentes en el GameObject o en sus jerarquías. Requiere un componente `Button` en el mismo GameObject, un `TextMeshProUGUI` en un GameObject hijo (para el texto) y un componente `Player` en un GameObject padre (para la gestión del jugador). También asume la presencia de un componente `Image` en el mismo GameObject para la representación visual de la carta.

*   **Eventos (Entrada):**
    `HandCard` implementa las interfaces `IPointerEnterHandler` y `IPointerExitHandler`, lo que le permite reaccionar a eventos de entrada del puntero (como el ratón sobre la carta). Además, se asume que un método como `SelectedCard()` está configurado para ser invocado por el evento `onClick` del componente `Button` al que está adjunto.

*   **Eventos (Salida):**
    Este script notifica a otros sistemas invocando métodos. Específicamente, llama a `player.PlayCard(card)` cuando la carta es seleccionada, informando al componente `Player` sobre la acción realizada. También incrementa `player.avalaibleCard`, lo que sugiere que actualiza un contador de cartas disponibles en el objeto `Player`. Finalmente, interactúa directamente con el singleton `Combatjudge` para consultar el estado del combate, lo que implica una dependencia con ese sistema central.