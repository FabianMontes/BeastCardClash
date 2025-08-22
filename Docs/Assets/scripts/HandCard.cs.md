# `HandCard.cs`

## 1. Propósito General
El script `HandCard.cs` es el componente principal que gestiona la representación visual y la interactividad de una carta individual dentro de la mano del jugador o en un panel de selección. Su rol es asegurar que la carta se muestre correctamente y que sea interactuable (o no) en función del estado actual del combate y las reglas del juego, interactuando con los sistemas de `Figther` (jugador) y `Combatjudge` (juez de combate).

## 2. Componentes Clave

### `HandCard`
-   **Descripción:** Esta clase hereda de `MonoBehaviour` y se encarga de la lógica de una carta específica en la interfaz de usuario. También implementa las interfaces `IPointerEnterHandler` y `IPointerExitHandler` para manejar eventos del puntero del mouse sobre la carta, aunque estos métodos están actualmente vacíos. Su función principal es mostrar los datos de una `Card` asociada y controlar si el jugador puede interactuar con ella (por ejemplo, seleccionarla o jugarla) en momentos específicos del combate.
-   **Variables Públicas / Serializadas:**
    *   `[SerializeField] int handPos;`: Representa la posición numérica de esta carta dentro de la mano del jugador.
    *   `[SerializeField] Card card;`: Es el objeto `ScriptableObject` o clase que contiene los datos lógicos de la carta (nombre, elemento, habilidades, etc.). Este script se encarga de mostrar visualmente la información de esta `Card`.
    *   `[SerializeField] bool playable = true;`: Un booleano que determina si esta instancia de `HandCard` está diseñada para ser una carta que el jugador puede jugar (`true`) o si es un espacio especial/no jugable (`false`), como una ranura de revelación.
    *   `[SerializeField] public bool picker = false;`: Un booleano que, si es `true`, indica que esta carta funciona como un "selector" en lugar de una carta de mano regular. Los selectores pueden tener un comportamiento de visibilidad e interactividad distinto, a menudo relacionados con fases de "revelación" o selección inicial.
    *   `Figther player;`: Una referencia al script `Figther` del jugador al que pertenece esta carta. Se utiliza para consultar el estado del jugador y para enviarle la acción de "jugar carta".
    *   `Button button;`: Una referencia al componente `UnityEngine.UI.Button` asociado a este GameObject. Se usa para controlar la interactividad del botón y visualmente indicar si la carta es clickeable.
    *   `SetMoments prevSetMoment;`: Almacena el estado de `SetMoments` del ciclo de combate del frame anterior. Se utiliza para detectar cambios en el estado del combate y aplicar la lógica de interactividad y visibilidad en consecuencia.

-   **Métodos Principales:**
    *   `void Start()`:
        *   Este método del ciclo de vida de Unity se llama una vez al inicio.
        *   Inicializa las referencias a `player` (obtenido del componente `Figther` en un padre) y `button`.
        *   Establece el `prevSetMoment` inicial a `SetMoments.PickDice`.
        *   Configura la clickabilidad inicial de la carta y su visibilidad (`Visib(false)`) si `playable` es `false`.
        *   Llama a `SetCard()` para inicializar la visualización de la carta asignada.
        *   Controla la visibilidad de un elemento visual secundario (índice 1 del hijo) basado en el valor de `picker`.
    *   `void Update()`:
        *   Este método del ciclo de vida se ejecuta en cada frame.
        *   Consulta el estado actual del combate (`SetMoments`) desde `Combatjudge.combatjudge`.
        *   Si la carta no es un `picker`:
            *   Detecta cambios en `SetMoments`.
            *   Si el momento es `SetMoments.PickCard` y el `player` está en combate, habilita la carta para ser clickeable si el tipo de combate coincide con el elemento de la carta o es un combate "completo". También incrementa un contador de cartas disponibles en el jugador (`player.avalaibleCard`).
            *   Si el momento no es `SetMoments.PickCard`, la carta se hace no clickeable.
            *   Si el jugador ya ha "seleccionado" una carta (`player.getPicked() != null`), esta carta se hace no clickeable para evitar múltiples selecciones.
        *   Si la carta es un `picker`:
            *   Maneja la visibilidad de la carta basándose en el `SetMoments` actual.
            *   En `SetMoments.PickCard`, hace la carta "halfVisible" (parcialmente visible) y ajusta su transparencia.
            *   En `SetMoments.Reveal`, la hace completamente visible.
    *   `void Visib(bool isVisible)`:
        *   Controla la activación/desactivación de un GameObject hijo específico (índice 1 del hijo de la carta), que probablemente representa la información o el arte de la carta.
    *   `void ForceReveal()`:
        *   Fuerza la carta a ser clickeable, activando la interactividad de su botón.
    *   `void SetCard(Card card)`:
        *   Asigna un objeto `Card` a esta instancia de `HandCard`.
        *   Gestiona la visibilidad de la carta. Si no hay `card` asignada o si la carta no es jugable y no es un `picker`, la oculta. De lo contrario, la muestra según sea `picker` o no.
    *   `void SelectedCard()`:
        *   Este método está diseñado para ser invocado cuando el botón de la carta es presionado (a menudo configurado en el Inspector de Unity).
        *   Notifica al objeto `player` para que "juegue" la `card` actualmente asignada a esta `HandCard`.
        *   Después de jugar, establece la `card` de esta `HandCard` a `null`, ocultando su representación visual.
    *   `void clickable(bool isClick)`:
        *   Activa o desactiva la interactividad del `button` de la carta.
        *   También cambia el color de tres imágenes hijas específicas (índices 0, 1 y 2 del hijo con índice 1), volviéndolas blancas si es clickeable o grises si no lo es, proporcionando un feedback visual de interactividad.
    *   `private void halfVisible(bool visible)`:
        *   Controla la activación/desactivación de otro GameObject hijo (índice 0 del hijo de la carta), probablemente el elemento visual principal o frontal de la carta.

-   **Lógica Clave:**
    La lógica principal reside en el método `Update`, que actúa como una máquina de estados simplificada para la interactividad de la carta. Monitorea constantemente el `SetMoments` del `Combatjudge`. Dependiendo de si la `HandCard` es una carta `playable` normal o un `picker`, ajusta su visibilidad y clickabilidad. Por ejemplo, las cartas jugables solo son clickeables durante la fase `SetMoments.PickCard` y solo si el tipo de combate actual permite jugar esa carta en particular (por su elemento). Los `picker` cambian su visibilidad para "revelar" una carta en fases específicas.

## 3. Dependencias y Eventos
-   **Componentes Requeridos:**
    *   Este script no utiliza el atributo `[RequireComponent]`. Sin embargo, es fundamental que el GameObject al que se adjunta tenga un componente `UnityEngine.UI.Button` para la interactividad, y que su `player` tenga un componente `Figther` en un GameObject padre.
-   **Eventos (Entrada):**
    *   `IPointerEnterHandler`, `IPointerExitHandler`: La clase implementa estas interfaces, lo que permite que Unity llame a sus métodos `OnPointerEnter` y `OnPointerExit` cuando el puntero del mouse entra o sale del área de la carta. Actualmente, estos métodos están vacíos, lo que indica que no hay lógica de "hover" implementada aún.
    *   `Button.onClick`: Aunque no se suscribe directamente mediante código en el `Start`, el método `SelectedCard()` está diseñado para ser el `onClick` listener del componente `Button` de la carta (típicamente configurado en el Inspector de Unity).
-   **Eventos (Salida):**
    *   Este script no invoca explícitamente `UnityEvent`s o `Action`s propios. En cambio, se comunica con otros sistemas llamando a métodos directamente:
        *   `player.PlayCard(card)`: Notifica al jugador que se ha jugado una carta.
        *   `Combatjudge.combatjudge.GetSetMoments()`: Consulta el estado actual del combate.
        *   `player.IsFigthing()`, `player.avalaibleCard++`, `player.getPicked()`: Consulta o modifica el estado del jugador.