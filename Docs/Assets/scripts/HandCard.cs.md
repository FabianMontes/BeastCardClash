# `HandCard.cs`

## 1. Propósito General
`HandCard.cs` es un script de Unity que gestiona la representación visual y la interacción de una única carta dentro de la mano del jugador en la interfaz de usuario del juego. Su rol principal es mostrar la información de una `Card` específica y controlar su interactividad basada en el estado actual del combate, permitiendo al jugador seleccionar y jugar la carta. Interactúa directamente con el sistema de combate (`Combatjudge`) para conocer la fase del juego y con el `Figther` (el jugador) para ejecutar acciones de la carta.

## 2. Componentes Clave

### `HandCard`
- **Descripción:** Esta clase es un `MonoBehaviour` que se adjunta a un GameObject en la jerarquía de Unity, representando visualmente una carta en la mano del jugador. Se encarga de actualizar la apariencia de la carta (texto, color) según los datos de una `Card` subyacente y de habilitar/deshabilitar la interacción (hacer clic) en momentos específicos del juego. El atributo `[DefaultExecutionOrder(-4)]` indica que este script se ejecuta muy temprano en el ciclo de vida de Unity, asegurando que su lógica de interfaz de usuario se procese antes que la mayoría de los demás scripts.

- **Variables Públicas / Serializadas:**
    - `handPos` (int): Una variable serializada que probablemente representa la posición de la carta dentro de la mano del jugador. Aunque no se utiliza directamente en la lógica proporcionada para modificar la posición, es visible en el Inspector de Unity para configuración.
    - `card` (Card): Una referencia serializada al objeto de datos `Card` que esta instancia de `HandCard` está visualizando y controlando. Es la representación de los datos de la carta.
    - `textMeshPro` (TextMeshProUGUI): Una referencia privada al componente `TextMeshProUGUI` hijo, utilizado para mostrar el identificador (ID) de la carta en la interfaz de usuario.
    - `player` (Figther): Una referencia privada al componente `Figther` que se encuentra en un padre de este GameObject. Representa al jugador o entidad que posee esta carta en su mano.
    - `button` (Button): Una referencia privada al componente `Button` asociado a este GameObject. Se utiliza para controlar la interactividad de la carta, permitiendo o impidiendo que el jugador haga clic en ella.
    - `prevSetMoment` (SetMoments): Una variable privada que almacena el `SetMoments` (fase de juego) anterior, permitiendo detectar cuándo ha habido un cambio en la fase del combate y reaccionar a ello.

- **Métodos Principales:**
    - `void Start()`: Este es un método de ciclo de vida de Unity que se llama una vez al inicio. Aquí se inicializan las referencias a los componentes `TextMeshProUGUI`, `Figther` y `Button` encontrados en el GameObject actual o en sus padres/hijos. También se establece el `prevSetMoment` inicial a `SetMoments.PickDice`, se desactiva la visibilidad inicial de la carta y se llama a `SetCard` para configurar su apariencia con la `card` inicial.
    - `void Update()`: Otro método de ciclo de vida de Unity, llamado en cada frame. Su función principal es monitorear el `SetMoments` actual del juego desde el `Combatjudge`. Si la fase del juego ha cambiado (`momo != prevSetMoment`), o si el jugador ya ha seleccionado una carta, el script ajusta la visibilidad y la interactividad de la carta. Durante la fase `SetMoments.PickCard`, la carta se hace visible y el jugador puede interactuar con ella, siempre que el tipo de combate sea compatible con el elemento de la carta o sea un combate `full`.
    - `private void Visib(bool isVisible)`: Un método auxiliar que controla la interactividad del botón de la carta. Si `isVisible` es `true`, el botón se vuelve interactuable; de lo contrario, no lo es.
    - `public void ForceReveal()`: Un método público que fuerza la visibilidad e interactividad de la carta, independientemente del estado del combate.
    - `void OnPointerEnter(PointerEventData eventData)`: Implementación de la interfaz `IPointerEnterHandler`. Este método se llama cuando el puntero del ratón entra en el área de la carta. Actualmente está vacío, pero podría usarse para efectos de "hover" (por ejemplo, escalar la carta o mostrar información adicional).
    - `void OnPointerExit(PointerEventData eventData)`: Implementación de la interfaz `IPointerExitHandler`. Se llama cuando el puntero del ratón sale del área de la carta. Actualmente está vacío, similar a `OnPointerEnter`.
    - `public void SetCard(Card card)`: Este método es crucial para actualizar la carta que esta instancia de `HandCard` representa. Recibe un objeto `Card` y actualiza el texto (ID de la carta) y el color de la imagen del GameObject basándose en el elemento de la carta (`fire`, `earth`, `water`, `air`). Si el `card` pasado es `null`, la carta se "vacía" visualmente, ocultando su texto e imagen.
    - `public void SelectedCard()`: Se espera que este método sea invocado cuando el botón de la carta se hace clic (generalmente configurado en el Inspector de Unity). Cuando se llama, informa al `player` (Figther) que "juegue" la `card` actualmente asociada. Después de jugar la carta, `SetCard(null)` es llamado para "eliminar" visualmente la carta de la mano, asumiendo que ha sido usada.
    - `public Card GetCard()`: Un método simple para recuperar el objeto `Card` que esta instancia de `HandCard` está representando actualmente.

- **Lógica Clave:**
    La lógica central reside en el método `Update`, que actúa como una máquina de estados simplificada para la visibilidad y la interacción de la carta. Detecta cambios en la fase de combate (`SetMoments`) gestionada por el `Combatjudge`. Durante la fase `PickCard`, la carta se hace interactuable si el `player` está activamente en combate y si el elemento de la carta es compatible con el tipo de combate actual (o si el combate es de tipo "full"). Fuera de esta fase, la carta permanece no interactuable. Adicionalmente, si el jugador ya ha seleccionado una carta, todas las cartas en la mano se vuelven no interactuables. El método `SetCard` gestiona la actualización visual, mapeando los datos de la `Card` a las propiedades de la UI (texto, color de la imagen), lo que permite que una sola instancia de `HandCard` represente diferentes cartas a lo largo del juego.

## 3. Dependencias y Eventos
- **Componentes Requeridos:**
    Este script no utiliza el atributo `[RequireComponent]`, pero funcionalmente requiere la presencia de ciertos componentes en el GameObject o sus hijos para operar correctamente. Necesita un `TextMeshProUGUI` como hijo para mostrar el texto de la carta, un `Button` en el mismo GameObject para la interactividad y un `Image` en el mismo GameObject para mostrar el color de la carta. También requiere que un componente `Figther` esté presente en uno de sus GameObjects padres para acceder a la lógica del jugador.

- **Eventos (Entrada):**
    El script implementa las interfaces `IPointerEnterHandler` y `IPointerExitHandler`, lo que significa que Unity invocará `OnPointerEnter` y `OnPointerExit` automáticamente cuando el puntero del ratón entre o salga del área de la UI de la carta.
    Aunque no se muestra explícitamente en el código `Start()`, se espera que el método `SelectedCard()` esté suscrito al evento `onClick` del componente `Button` del GameObject, permitiendo al jugador seleccionar la carta.

- **Eventos (Salida):**
    Este script no invoca directamente `UnityEvents` ni `Actions` para notificar a otros sistemas. En su lugar, interactúa con otros sistemas a través de llamadas directas a métodos y acceso a propiedades de componentes obtenidos por referencia:
    - Llama a métodos y accede a propiedades del `player` (componente `Figther`): `IsFigthing()`, `avalaibleCard++`, `getPicked()`, `PlayCard(card)`.
    - Accede a métodos y propiedades estáticas del `Combatjudge`: `Combatjudge.combatjudge.GetSetMoments()` y `Combatjudge.combatjudge.combatType`.